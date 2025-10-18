using FEA_Program.Models;
using FEA_Program.SaveData;
using FEA_Program.UI;
using FEA_Program.ViewModels.Base;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Specialized;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class ProjectVM: ObservableObject
    {
        // ---------------------- Models ----------------------
        public StressProblem Problem { get; private set; } = new();

        // ---------------------- Sub VMs ----------------------
        public BaseVM Base { get; private set; } = new();
        public MaterialsVM Materials { get; private set; } = new();
        public NodesVM Nodes { get; private set; } = new();
        public ElementsVM Elements { get; private set; } = new();

        // ---------------------- Commands ----------------------
        /// <summary>
        /// RelayCommand for <see cref="LoadFile"/>
        /// </summary>
        public ICommand LoadFileCommand { get; private set; }

        /// <summary>
        /// RelayCommand for <see cref="SaveFile"/>
        /// </summary>
        public ICommand SaveFileCommand { get; private set; }


        // ---------------------- Public Methods ----------------------
        public ProjectVM()
        {
            LoadFileCommand = new AsyncRelayCommand(LoadFile);
            SaveFileCommand = new AsyncRelayCommand(SaveFile);

            Materials.AddDefaultMaterials();
            ResetProblem(ProblemTypes.Truss_3D);
        }
        public async Task LoadFile()
        {
            try
            {
                var filePath = IODialogs.DisplayOpenFileDialog([IOFileTypes.JSON]);

                if (filePath != null && filePath != "")
                {
                    var saveData = await SaveData.JsonSerializer.DeserializeFromJsonFile<ProblemData>(filePath);

                    if (saveData != null)
                    {
                        // This this before loading data so changing it doesn't reset the problem
                        //ToolStripComboBox_ProblemMode.SelectedIndex = (int)saveData.ProblemType;

                        LoadData(saveData);
                    }
                }
            }
            catch (InvalidOperationException ex) // Unsure if this will ever happen
            {
                Base.DisplayError(ex.Message);
            }
            catch (FileFormatException ex) // Will fire if there is no data in the file
            {
                Base.DisplayError(ex.Message);
            }
            catch (JsonException)
            {
                Base.DisplayError("Could not load the file. It contains invalid data.");
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }
        public async Task SaveFile()
        {
            try
            {
                var filePath = IODialogs.DisplaySaveFileDialog([IOFileTypes.JSON], "FEA Problem 1");

                if (filePath != null && filePath != "")
                {
                    var saveData = GetSaveData();

                    await SaveData.JsonSerializer.SerializeToJsonFile(saveData, filePath);
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        // ---------------------- Event Methods ----------------------

        /// <summary>
        /// This is the method called whenever a node is added, removed, moved, or replaced.
        /// </summary>
        /// <param name="sender">The ObservableCollection itself.</param>
        /// <param name="e">Event arguments detailing the change.</param>
        private void OnNodesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Items were removed
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
            {
                // e.OldItems contains the list of items that were removed.
                var removedIds = e.OldItems
                    .Cast<NodeVM>()
                    .Select(node => node.Model.ID);

                // Get the list of hanging element IDs
                List<int> elementIds = removedIds
                    .SelectMany(NodeID => Problem.RemoveNode(NodeID))
                    .ToList();

                // Delete hanging elements
                Elements.Delete(elementIds);
            }
            // The list was reset
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // Delete all elements
                Elements.Delete();
            }
        }

        // ---------------------- Private Helpers ----------------------
        private void ResetProblem(ProblemTypes problemType)
        {
            Problem = new StressProblem(problemType);
            Nodes = new NodesVM(Problem.AvailableNodeDOFs);
            Nodes.Items.CollectionChanged += OnNodesCollectionChanged;

            Elements.LinkCollections(Nodes.Items, Materials.Items);
            Elements.AddEditor.AvailableElementTypes = new(Problem.AvailableElements);
        }
        private void LoadData(ProblemData data)
        {
            ResetProblem(data.ProblemType);

            // ----------- Import Materials ---------------
            Dictionary<int, Material> materials = [];
            foreach (var item in data.Materials)
            {
                materials.Add(item.ID, new Material(item.Name, item.E, item.ID, item.Subtype)
                {
                    V = item.V,
                    Sy = item.Sy,
                    Sut = item.Sut
                });
            }

            Materials.ImportMaterials([.. materials.Values]);

            // ----------- Import Nodes ---------------
            Dictionary<int, Node> nodes = [];

            foreach (var item in data.Nodes)
            {
                nodes.Add(item.ID, new Node(item.Coords, item.Fixity, item.ID, item.Dimension)
                {
                    Force = item.Force
                });
            }

            foreach (var node in nodes.Values)
                Problem.AddNode(node);

            Nodes.ImportNodes([.. nodes.Values]);

            // ----------- Import Elements ---------------
            List<IElement> elements = [];

            foreach (var item in data.Elements)
            {
                // Get nodes and material corresponding to element
                var elementMaterial = materials[item.MaterialID];
                List<INode> elementNodes = item.NodeIDs.Select(nodeId => nodes[nodeId]).Cast<INode>().ToList();

                IElement? element = null;

                switch (item.ElementType)
                {
                    case ElementTypes.BarLinear:
                        element = new ElementBarLinear(1, item.ID, elementNodes, elementMaterial, item.NodeDOFs)
                        {
                            ElementArgs = item.ElementArgs
                        };
                        element.SetBodyForce(new DenseVector(item.BodyForce));
                        element.SetTractionForce(new DenseVector(item.TractionForce));
                        break;
                }

                if (element is not null)
                    elements.Add(element);
            }

            foreach (var element in elements)
                Problem.AddElement(element);

            Elements.ImportElements(elements);
        }
        private ProblemData GetSaveData()
        {
            var output = new ProblemData();

            // ------------------- Nodes --------------------
            foreach (var node in Problem.Nodes)
            {
                output.Nodes.Add(new NodeProblemData
                {
                    Dimension = node.Dimension,
                    ID = node.ID,
                    Coords = node.Coordinates,
                    Fixity = node.Fixity,
                    Force = node.Force
                });
            }

            // ------------------- Elements --------------------
            foreach (var element in Problem.Elements)
            {
                output.Elements.Add(new ElementProblemData
                {
                    ID = element.ID,
                    MaterialID = element.Material.ID,
                    NodeIDs = element.Nodes.Select(n => n.ID).ToArray(),
                    NodeDOFs = element.NodeDOFs,
                    ElementType = element.ElementType,
                    ElementArgs = element.ElementArgs,
                    BodyForce = [.. element.BodyForce],
                    TractionForce = [.. element.TractionForce],
                });
            }

            // ------------------- Materials --------------------
            foreach (var item in Materials.Items.Select(m => m.Model))
            {
                output.Materials.Add(new MaterialSaveData
                {
                    ID = item.ID,
                    Name = item.Name,
                    Subtype = item.Subtype,
                    E = item.E,
                    V = item.V,
                    Sy = item.Sy,
                    Sut = item.Sut,
                });
            }

            // ------------------- Other --------------------
            output.ProblemType = Problem.ProblemType;

            return output;
        }
    }
}
