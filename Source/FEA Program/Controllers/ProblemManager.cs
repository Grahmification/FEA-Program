using FEA_Program.Drawable;
using FEA_Program.Models;
using FEA_Program.SaveData;
using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Controllers
{
    internal class ProblemManager
    {
        public Mainform Loadedform { get; private set; }
        public StressProblem Problem { get; private set; } = new();

        public NodeManager Nodes { get; private set; }
        public ElementManager Elements { get; private set; }
        public MaterialManager Materials { get; private set; }

        /// <summary>
        /// Whether the screen should be 3D based on the problem type
        /// </summary>
        public bool ThreeDimensional => Problem.ProblemType switch
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

                for (int i = 0; i < Problem.AvailableNodeDOFs; i++)
                    addNodeArgs.Add(typeof(double)); // node position

                for (int i = 0; i < Problem.AvailableNodeDOFs; i++)
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
        public ProblemManager(Mainform form)
        {
            Nodes = new NodeManager();
            Nodes.NodeListChanged += (s, e) => OnListRedrawNeeded();
            Nodes.NodesChanged += (s, e) => OnListRedrawNeeded();
            Nodes.NodeChanged_RedrawOnly += OnScreenRedrawOnlyNeeded;

            Elements = new ElementManager();
            Elements.ElementListChanged += (s, e) => OnListRedrawNeeded();
            Elements.ElementChanged += (s, e) => OnListRedrawNeeded();
            Elements.ElementChanged_RedrawOnly += OnScreenRedrawOnlyNeeded;

            Materials = new MaterialManager();
            Materials.MaterialListChanged += (s, e) => OnListRedrawNeeded();

            Loadedform = form;
        }
        public void ResetProblem(ProblemTypes problemType)
        {
            Problem = new StressProblem(problemType);
            Nodes.Reset();
            Elements.Reset();
            Nodes.Problem = Problem;
            Elements.Problem = Problem;
        }

        public ProblemData GetSaveData()
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
            foreach (var element in Elements.Elemlist)
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
            foreach (var item in Materials.MaterialList)
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
        public void LoadData(ProblemData data)
        {
            ResetProblem(data.ProblemType);

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
            List<IElementDrawable> elements = [];
            foreach (var item in data.Elements)
            {
                var elementMaterial = Materials.GetMaterial(item.MaterialID);
                List<NodeDrawable> elementNodes = item.NodeIDs
                    .Select(nodeId => Nodes.GetNode(nodeId)) // Must get drawable objects from the manager
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

    }
}
