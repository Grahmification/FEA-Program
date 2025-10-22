using FEA_Program.Models;
using FEA_Program.SaveData;
using FEA_Program.UI;
using FEA_Program.ViewModels.Base;
using MathNet.Numerics.LinearAlgebra.Double;
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
        public DrawVM Draw { get; private set; } = new();

        public NewProblemVM NewProblem { get; private set; } = new();


        // ---------------------- Commands ----------------------
        /// <summary>
        /// RelayCommand for <see cref="LoadFile"/>
        /// </summary>
        public ICommand LoadFileCommand { get; private set; }

        /// <summary>
        /// RelayCommand for <see cref="SaveFile"/>
        /// </summary>
        public ICommand SaveFileCommand { get; private set; }

        /// <summary>
        /// RelayCommand for <see cref="Solve"/>
        /// </summary>
        public ICommand SolveCommand { get; private set; }


        // ---------------------- Public Methods ----------------------
        public ProjectVM()
        {
            LoadFileCommand = new AsyncRelayCommand(LoadFile);
            SaveFileCommand = new AsyncRelayCommand(SaveFile);
            SolveCommand = new AsyncRelayCommand(Solve);

            NewProblem.Accepted += OnNewProblemAccepted;

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
        public async Task Solve()
        {
            try
            {
                bool result = await Task.Run(() => Problem.Solve());

                if (result)
                {
                    FormattedMessageBox.DisplayMessage("Solved Successfully!");
                }
                else
                {
                    FormattedMessageBox.DisplayMessage("Solution succeeded, but with invalid values. Check for unconstrained degrees of freedom.");
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }


        // ---------------------- Event Methods ----------------------
        private void OnNodeAdding(object? sender, NodeVM e)
        {
            Problem.AddNode(e.Model);
            Draw.AddNode(e);
        }
        private void OnNodeRemoving(object? sender, NodeVM e)
        {
            // Get the list of hanging element IDs
            List<int> elementIds = [.. Problem.RemoveNode(e.Model.ID)];
            Draw.RemoveNode(e.Model.ID);

            // Also delete hanging elements
            Elements.Delete(elementIds);
        }
        private void OnElementAdding(object? sender, ElementVM e)
        {
            if(e.Model != null)
            {
                Problem.AddElement(e.Model);
                Draw.AddElement(e);
            }
        }
        private void OnElementRemoving(object? sender, ElementVM e)
        {
            if (e.Model != null)
            {
                Problem.RemoveElement(e.Model.ID);
                Draw.RemoveElement(e.Model.ID);
            }
        }

        private void OnNewProblemAccepted(object? sender, ProblemTypes e)
        {
            if(Nodes.Items.Count > 0)
            {
                var result = FormattedMessageBox.DisplayYesNoQuestion("This will clear the current problem. Do you want to continue?", "Reset Problem?");

                if(result == DialogResult.No)
                {
                    throw new OperationCanceledException();
                }
            }

            ResetProblem(e);
        }

        // ---------------------- Private Helpers ----------------------
        private void ResetProblem(ProblemTypes problemType)
        {
            Problem = new StressProblem(problemType);
            Draw = new DrawVM();
            Nodes = new NodesVM(Problem.AvailableNodeDOFs);
            Nodes.ItemAdding += OnNodeAdding;
            Nodes.ItemRemoving += OnNodeRemoving;

            Elements = new ElementsVM();
            Elements.ItemAdding += OnElementAdding;
            Elements.ItemRemoving += OnElementRemoving;
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
