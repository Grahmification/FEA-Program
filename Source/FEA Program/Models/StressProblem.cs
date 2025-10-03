using FEA_Program.Drawable;
using FEA_Program.SaveData;
using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal class StressProblem
    {
        public Mainform Loadedform { get; private set; }
        public ProblemTypes ProblemType { get; private set; }
        public NodeManager Nodes { get; private set; }
        public ElementManager Elements { get; private set; }
        public MaterialManager Materials { get; private set; }
        public Connectivity Connect { get; private set; }
        public Solver Solver { get; private set; } = new Solver();

        /// <summary>
        /// which elements are available depending on problem type
        /// </summary>
        public Type[]? AvailableElements => ProblemType switch
        {
            ProblemTypes.Bar_1D => new[] { typeof(ElementBarLinear) },
            ProblemTypes.Truss_3D => new[] { typeof(ElementBarLinear) },

            // Default case: return null
            _ => null
        };

        /// <summary>
        /// Which node DOF should be used for given problem type
        /// </summary>
        public int AvailableNodeDOFs => ProblemType switch
        {
            // Case for 1 DOFs
            ProblemTypes.Bar_1D or ProblemTypes.Beam_1D => 1,

            // Case for 3 DOFs
            ProblemTypes.Truss_3D => 3,

            // Default case (return 0)
            _ => 0
        };

        /// <summary>
        /// Whether the screen should be 3D based on the problem type
        /// </summary>
        public bool ThreeDimensional => ProblemType switch
        {
            // Combine cases that return false using the 'or' pattern
            ProblemTypes.Bar_1D or ProblemTypes.Beam_1D => false,

            // Case for true
            ProblemTypes.Truss_3D => true,

            // Default case (return false)
            _ => false
        };

        /// <summary>
        /// Gets available command line commands
        /// </summary>
        public Dictionary<string, List<Type>> CommandList
        {
            get
            {
                // ------------- Add Node Command-------------
                var addNodeArgs = new List<Type>();

                for (int i = 0; i < AvailableNodeDOFs; i++)
                    addNodeArgs.Add(typeof(double)); // node position

                for (int i = 0; i < AvailableNodeDOFs; i++)
                    addNodeArgs.Add(typeof(bool)); // node fixity

                // ------------- Add Material Command-------------
                var addMaterialArgs = new List<Type>
                {
                    typeof(string),
                    typeof(double),
                    typeof(double),
                    typeof(double),
                    typeof(double),
                    typeof(int)
                };

                return new Dictionary<string, List<Type>>
                {
                    { "AddNode", addNodeArgs },
                    { "AddMaterial", addMaterialArgs },
                };
            }
        }

        // ---------------------- Public Methods ----------------------------
        public StressProblem(Mainform form, ProblemTypes Type, MaterialManager? materials = null)
        {
            Nodes = new NodeManager();
            Nodes.NodeListChanged += (s, e) => OnListRedrawNeeded();
            Nodes.NodesChanged += (s, e) => OnListRedrawNeeded();
            Nodes.NodeChanged_RedrawOnly += OnScreenRedrawOnlyNeeded;
            Nodes.NodeDeleted += OnNodeDeletion;


            Elements = new ElementManager();
            Elements.ElementListChanged += (s, e) => OnListRedrawNeeded();
            Elements.ElementChanged += (s, e) => OnListRedrawNeeded();
            Elements.ElementChanged_RedrawOnly += OnScreenRedrawOnlyNeeded;
            Elements.ElementAdded += OnElementCreation;
            Elements.ElementDeleted += OnElementDeletion;

            Materials = materials ?? new MaterialManager();
            Materials.MaterialListChanged += (s, e) => OnListRedrawNeeded();

            Connect = new Connectivity();

            Loadedform = form;
            ProblemType = Type;
        }
        public ProblemData GetSaveData()
        {
            var output = new ProblemData();

            // ------------------- Nodes --------------------
            foreach(var node in Nodes.Nodelist) 
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
            foreach (var element in Elements.Elemlist)
            {
                output.Elements.Add(new ElementProblemData
                {
                    ID = element.ID,
                    MaterialID = element.Material.ID,
                    NodeDOFs = element.NodeDOFs,
                    ElementType = element.ElementType,
                    ElementArgs = element.ElementArgs,
                    BodyForce = [.. element.BodyForce],
                    TractionForce = [.. element.TractionForce],
                });
            }

            // ------------------- Materials --------------------
            foreach (var item in Materials.MaterialList)
            {
                output.Materials.Add(new MaterialSaveData
                {
                    ID = item.ID,
                    Name = item.Name,
                    Subtype = item.Subtype,
                    E= item.E,
                    V= item.V,
                    Sy = item.Sy,
                    Sut = item.Sut,
                });
            }

            // ------------------- Other --------------------
            output.ConnectivityMatrix = Connect.ConnectivityMatrix;
            output.ProblemType = ProblemType;

            return output;
        }
        public void LoadData(ProblemData data)
        {
            ProblemType = data.ProblemType;
            // This should be done early as other things raise events which might affect it
            Connect = new Connectivity(data.ConnectivityMatrix);

            // ----------- Import Materials ---------------
            List<Material> materials = [];
            foreach (var item in data.Materials)
            {
                materials.Add(new Material(item.Name, item.E, item.ID, item.Subtype)
                {
                    V = item.V,
                    Sy = item.Sy,
                    Sut = item.Sut
                });
            }

            Materials.ImportMaterials(materials);

            // ----------- Import Nodes ---------------
            Dictionary<int, NodeDrawable> nodes = [];
 
            foreach (var item in data.Nodes)
            {
                nodes.Add(item.ID, new NodeDrawable(item.Coords, item.Fixity, item.ID, item.Dimension)
                {
                    Force = item.Force
                });
            }

            Nodes.ImportNodes([.. nodes.Values]);

            // ----------- Import Elements ---------------
            List<IElementDrawable> elements = [];
            foreach (var item in data.Elements)
            {
                var elementMaterial = Materials.GetMaterial(item.MaterialID);
                var elementNodeIds = data.ConnectivityMatrix[item.ID];
                List<NodeDrawable> elementNodes = elementNodeIds
                    .Select(nodeId => nodes[nodeId])
                    .ToList();

                IElementDrawable? element = null;

                switch (item.ElementType)
                {
                    case ElementTypes.BarLinear:
                        element = new ElementBarLinearDrawable(1, item.ID, elementNodes, elementMaterial, item.NodeDOFs)
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
        /// Solve the stress problem
        /// </summary>
        public bool Solve()
        {
            Dictionary<int, int> nodeDOFS = Nodes.Nodelist.ToDictionary(n => n.ID, n => n.Dimension);
            Dictionary<int, double[]> nodeCoordinates = Nodes.Nodelist.ToDictionary(n => n.ID, n => n.Coordinates);

            var K_Matricies = ElementExtensions.Get_K_Matricies(Elements.Elemlist.Cast<IElement>().ToList());
            SparseMatrix K_assembled = Connect.Assemble_K_Matrix(K_Matricies, nodeDOFS);
            var F_assembled = NodeExtensions.F_Matrix(Nodes.BaseNodelist);
            var Q_assembled = NodeExtensions.Q_Matrix(Nodes.BaseNodelist);

            DenseVector[] output = Solver.Solve(K_assembled, F_assembled, Q_assembled);
            Nodes.SetSolution(output[0], output[1]);

            var displacements = output[0].Values;

            // False if there's a bad value
            return !(displacements.Contains(double.NaN) || displacements.Contains(double.PositiveInfinity) || displacements.Contains(double.NegativeInfinity));
        }


        // ---------------------- Event Handlers ----------------------------

        private void OnListRedrawNeeded()
        {
            Loadedform.ReDrawLists();
            Loadedform.GlCont.SubControl.Invalidate();
        }
        private void OnScreenRedrawOnlyNeeded(object? sender, EventArgs e)
        {
            Loadedform.GlCont.SubControl.Invalidate();
        }


        private void OnElementCreation(object? sender, int elementID)
        {
            var nodeIDs = Elements.GetElement(elementID).Nodes.Select(node => node.ID).ToArray();
            Connect.AddConnection(elementID, nodeIDs);
        }
        private void OnElementDeletion(object? sender, IElement e)
        {
            Connect.RemoveConnection(e.ID);
        }
        private void OnNodeDeletion(object? sender, INode e)
        {
            RemoveHangingElements(e.ID);
        }

        // ---------------------- Private Helpers ----------------------------

        /// <summary>
        /// Deletes elements if a node is deleted and leaves one hanging
        /// </summary>
        /// <param name="nodeID"></param>
        private void RemoveHangingElements(int nodeID)
        {
            var ElementsToDelete = Connect.GetNodeElements(nodeID);
            Elements.Delete(ElementsToDelete);
        }
    }

    public enum ProblemTypes
    {
        Bar_1D,
        Beam_1D,
        Truss_3D
    }
}
