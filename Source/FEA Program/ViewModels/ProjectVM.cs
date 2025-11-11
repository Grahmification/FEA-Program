using FEA_Program.Models;
using FEA_Program.Models.SaveData;
using FEA_Program.Utils;
using FEA_Program.ViewModels.Base;
using MathNet.Numerics.LinearAlgebra.Double;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Primary viewmodel to manage the FEA problem
    /// </summary>
    internal class ProjectVM: ObservableObject
    {
        /// <summary>
        /// Fires when the problem is reset, with the problem type as the argument
        /// </summary>
        public event EventHandler<ProblemTypes>? ProblemReset;
        
        // ---------------------- Models ----------------------

        /// <summary>
        /// The FEA problem
        /// </summary>
        public StressProblem Problem { get; private set; } = new();

        // ---------------------- Sub VMs ----------------------

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; private set; } = new();

        /// <summary>
        /// Manages all materials in the program
        /// </summary>
        public MaterialsVM Materials { get; private set; } = new();

        /// <summary>
        /// Manages all nodes in the program
        /// </summary>
        public NodesVM Nodes { get; private set; } = new();

        /// <summary>
        /// Manages all elements in the program
        /// </summary>
        public ElementsVM Elements { get; private set; } = new();

        /// <summary>
        /// Manages drawing items in the 3D viewer
        /// </summary>
        public DrawVM Draw { get; private set; } = new();

        /// <summary>
        /// Manages control for creating a new problem
        /// </summary>
        public NewProblemVM NewProblem { get; private set; } = new();

        /// <summary>
        /// Manages display of problem matricies
        /// </summary>
        public DebugMatrixVM DebugMatrix { get; private set; } = new();

        /// <summary>
        /// Manages item selection within the program
        /// </summary>
        public SelectionVM SelectionManager { get; private set; } = new();

        // ---------------------- Properties ----------------------

        /// <summary>
        /// Whether the screen should be 3D based on the problem type
        /// </summary>
        public bool ThreeDimensional => Problem.ProblemType switch
        {
            // Combine cases that return false using the 'or' pattern
            ProblemTypes.Truss_1D or ProblemTypes.Beam_1D => false,

            // Case for true
            ProblemTypes.Truss_3D => true,

            // Default case (return false)
            _ => false
        };

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
        
        /// <summary>
        /// Primary constructor
        /// </summary>
        public ProjectVM()
        {
            LoadFileCommand = new AsyncRelayCommand(LoadFile);
            SaveFileCommand = new AsyncRelayCommand(SaveFile);
            SolveCommand = new AsyncRelayCommand(Solve);

            NewProblem.Accepted += OnNewProblemAccepted;

            Materials.AddDefaultMaterials();
            ResetProblem(ProblemTypes.Truss_3D);
        }

        /// <summary>
        /// Sets the base, also assigning it to any sub-classes
        /// </summary>
        /// <param name="baseVM"></param>
        public void SetBase(BaseVM baseVM)
        {
            Base = baseVM;
            NewProblem.Base = baseVM;
            Draw.Base = baseVM;
            Nodes.SetBase(baseVM);
            Materials.SetBase(baseVM);
            Elements.SetBase(baseVM);
        }

        /// <summary>
        /// Load an FEA project from a file
        /// </summary>
        /// <returns></returns>
        public async Task LoadFile()
        {
            try
            {
                QueryUserAboutReset();

                var filePath = IODialogs.DisplayOpenFileDialog([IOFileTypes.JSON]);

                if (filePath != null && filePath != "")
                {
                    var saveData = await Models.SaveData.JsonSerializer.DeserializeFromJsonFile<ProblemData>(filePath);

                    if (saveData != null)
                    {
                        LoadData(saveData);
                    }

                    Base.SetStatus($"Loaded {filePath}");
                }
            }
            catch (OperationCanceledException) { } // Do nothing, the user chose to cancel loading the file
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

        /// <summary>
        /// Save an FEA project to a file
        /// </summary>
        /// <returns></returns>
        public async Task SaveFile()
        {
            try
            {
                var filePath = IODialogs.DisplaySaveFileDialog([IOFileTypes.JSON], "FEA Problem 1");

                if (filePath != null && filePath != "")
                {
                    var saveData = GetSaveData();

                    await Models.SaveData.JsonSerializer.SerializeToJsonFile(saveData, filePath);

                    Base.SetStatus($"Saved to {filePath}");
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Solve the FEA problem
        /// </summary>
        /// <returns></returns>
        public async Task Solve()
        {
            try
            {
                bool result = await Task.Run(() => Problem.Solve());

                // Refresh all result display settings
                Draw.ApplySettings();

                if (result)
                {
                    FormattedMessageBox.DisplayMessage("Solved Successfully!");
                }
                else
                {
                    FormattedMessageBox.DisplayMessage("Solution succeeded, but with invalid values. Check for unconstrained degrees of freedom.");
                }

                Base.SetStatus("Solved Problem");
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        // ---------------------- Event Methods ----------------------

        /// <summary>
        /// Called when a node is being added to the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The node being added</param>
        private void OnNodeAdding(object? sender, NodeVM e)
        {
            Problem.AddNode(e.Model);
            SelectionManager.AddItem(e);
            Draw.AddNode(e);
        }

        /// <summary>
        /// Called when a node is being removed from the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The node being removed</param>
        private void OnNodeRemoving(object? sender, NodeVM e)
        {
            Draw.RemoveNode(e.Model.ID);
            SelectionManager.RemoveItem(e);

            // Get the list of hanging element IDs
            List<int> elementIds = [.. Problem.RemoveNode(e.Model.ID)];

            // Also delete hanging elements
            Elements.Delete(elementIds);
        }

        /// <summary>
        /// Called when an element is being added to the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The element being added</param>
        private void OnElementAdding(object? sender, ElementVM e)
        {
            if(e.Model != null)
            {
                Problem.AddElement(e.Model);
                Draw.AddElement(e);
                SelectionManager.AddItem(e);
            }
        }

        /// <summary>
        /// Called when an element is being removed from the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The element being removed</param>
        private void OnElementRemoving(object? sender, ElementVM e)
        {
            SelectionManager.RemoveItem(e);

            if (e.Model != null)
            {
                Problem.RemoveElement(e.Model.ID);
                Draw.RemoveElement(e.Model.ID);
            }
        }


        /// <summary>
        /// Called when the node editor has opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNodeEditorOpened(object? sender, EventArgs e)
        {
            if(sender is NodeEditVM editor)
            {
                // We have a new node being created
                if (!editor.Editing && editor.EditItem != null)
                {
                    // This item will be overwritten when the final node is added to the problem
                    Draw.AddNode(editor.EditItem, true);
                }
            }
        }

        /// <summary>
        /// Called when the node editor has been canceled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The node being edited</param>
        private void OnNodeEditorCanceled(object? sender, NodeVM e)
        {
            if (sender is NodeEditVM editor)
            {
                // We have a new node being created
                if (!editor.Editing && editor.EditItem != null)
                {
                    // Remove the pending mode
                    Draw.RemoveNode(editor.EditItem.Model.ID);
                }
            }
        }


        /// <summary>
        /// Called when a new problem has been accepted for creation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The new problem type</param>
        private void OnNewProblemAccepted(object? sender, ProblemTypes e)
        {
            QueryUserAboutReset();
            ResetProblem(e);
        }


        /// <summary>
        /// Called when the solver indicates problem matricies have been computed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The matricies</param>
        private void OnMatriciesCalculated(object? sender, (Matrix, Matrix, Matrix) e)
        {
            DebugMatrix.K_Matrix = e.Item1;
            DebugMatrix.Q_Matrix = e.Item2;
            DebugMatrix.F_Matrix = e.Item3;
        }

        /// <summary>
        /// Called when the solver indicates problem matricies with fixed displacements removed have been calculated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The matricies</param>
        private void OnPartiallyReducedMatriciesCalculated(object? sender, (Matrix, Matrix) e)
        {
            DebugMatrix.K_Matrix_Reduced = e.Item1;
            DebugMatrix.F_Matrix_Reduced = e.Item2;
        }

        /// <summary>
        /// Called when the solver indicates problem matricies in their final solving form have been calculated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The matricies</param>
        private void OnFullyReducedMatriciesCalculated(object? sender, (Matrix, Matrix) e)
        {
            DebugMatrix.K_Matrix_Fully_Reduced = e.Item1;
            DebugMatrix.F_Matrix_Fully_Reduced = e.Item2;
        }

        // ---------------------- Private Helpers ----------------------

        /// <summary>
        /// Resets the FEA problem
        /// </summary>
        /// <param name="problemType">The new problem type for the blank problem</param>
        private void ResetProblem(ProblemTypes problemType)
        {
            Problem = new StressProblem(problemType);
            Problem.Solver.SolutionStarted += OnMatriciesCalculated;
            Problem.Solver.PartiallyReducedCalculated += OnPartiallyReducedMatriciesCalculated;
            Problem.Solver.FullyReducedCalculated += OnFullyReducedMatriciesCalculated;

            DebugMatrix.ResetMatricies();
            Draw.ResetCollections();
            Nodes = new NodesVM(Problem.AvailableNodeDimensions, Problem.NodesHaveRotation);
            Nodes.SetBase(Base);
            Nodes.ItemAdding += OnNodeAdding;
            Nodes.ItemRemoving += OnNodeRemoving;
            Nodes.ForceEditor.SelectionManager = SelectionManager;
            Nodes.Editor.Opened += OnNodeEditorOpened;
            Nodes.Editor.CancelEdits += OnNodeEditorCanceled;

            Elements = new ElementsVM();
            Elements.SetBase(Base);
            Elements.ItemAdding += OnElementAdding;
            Elements.ItemRemoving += OnElementRemoving;
            Elements.LinkCollections(Nodes.Items, Materials.Items);
            Elements.AddEditor.AvailableElementTypes = new(Problem.AvailableElements);
            Elements.AddEditor.SelectionManager = SelectionManager;

            ProblemReset?.Invoke(this, problemType);

            Base.SetStatus("Ready");
        }

        /// <summary>
        /// Loads project save data into the program
        /// </summary>
        /// <param name="data">The data to load</param>
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
                nodes.Add(item.ID, new Node(item.Coords, item.Fixity, item.ID, item.Dimension, item.HasRotation)
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
                    case ElementTypes.TrussLinear:
                        element = new ElementTrussLinear(1, item.ID, elementNodes, elementMaterial, item.NodeDOFs)
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

        /// <summary>
        /// Gets project save data from the program
        /// </summary>
        /// <returns>Save dat for the current problem</returns>
        private ProblemData GetSaveData()
        {
            var output = new ProblemData();

            // ------------------- Nodes --------------------
            foreach (var node in Problem.Nodes)
            {
                output.Nodes.Add(new NodeProblemData
                {
                    Dimension = node.DOFs,
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

        /// <summary>
        /// Queries the user about whether they want to reset the problem
        /// </summary>
        /// <exception cref="OperationCanceledException">Thrown if the user selects no</exception>
        private void QueryUserAboutReset()
        {
            if (Nodes.Items.Count > 0)
            {
                var result = FormattedMessageBox.DisplayYesNoQuestion("This will clear the current problem. Do you want to continue?", "Reset Problem?");

                if (result == MessageBoxResult.No)
                {
                    throw new OperationCanceledException();
                }
            }
        }
    }
}
