using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    /// <summary>
    /// Primary class for an FEA problem
    /// </summary>
    internal class StressProblem
    {
        /// <summary>
        /// The problem's elements, referenced by ID
        /// </summary>
        private readonly Dictionary<int, IElement> _Elements = [];

        /// <summary>
        /// The problem's nodes, referenced by ID
        /// </summary>
        private readonly Dictionary<int, Node> _Nodes = [];

        // ---------------------- Public Properties ----------------------------

        /// <summary>
        /// Indicates the classification of problem
        /// </summary>
        public ProblemTypes ProblemType { get; private set; }

        /// <summary>
        /// Manages solving the solution
        /// </summary>
        public Solver Solver { get; private set; } = new Solver();

        /// <summary>
        /// All elements in the problem
        /// </summary>
        public List<IElement> Elements => _Elements.Values.ToList();

        /// <summary>
        /// All nodes in the problem
        /// </summary>
        public List<Node> Nodes => _Nodes.Values.ToList();

        /// <summary>
        /// which elements are available depending on problem type
        /// </summary>
        public ElementTypes[] AvailableElements => ProblemType switch
        {
            ProblemTypes.Truss_1D => [ElementTypes.TrussLinear],
            ProblemTypes.Truss_3D => [ElementTypes.TrussLinear],

            // Default case: return empty
            _ => []
        };

        /// <summary>
        /// Which node DOF should be used for given problem type
        /// </summary>
        public int AvailableNodeDOFs => ProblemType switch
        {
            // Case for 1 DOFs
            ProblemTypes.Truss_1D or ProblemTypes.Beam_1D => 1,

            // Case for 3 DOFs
            ProblemTypes.Truss_3D => 3,

            // Default case (return 0)
            _ => 0
        };

        // ---------------------- Public Methods ----------------------------

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="problemType">Indicates the classification of problem</param>
        public StressProblem(ProblemTypes problemType = ProblemTypes.Truss_1D)
        {
            ProblemType = problemType;
        }

        /// <summary>
        /// Adds a node to the problem
        /// </summary>
        /// <param name="node">The node to add</param>
        /// <exception cref="ArgumentException">A node parameter was invalid</exception>
        /// <exception cref="Exception">A node exists at this position</exception>
        public void AddNode(Node node)
        {
            if (_Nodes.ContainsKey(node.ID))
                throw new ArgumentException($"Could not add node {node.ID} to problem. Problem already contains that ID.");

            if (node.DOFs != AvailableNodeDOFs)
                throw new ArgumentException($"Could not add node {node.ID} to problem. Node has {node.DOFs} DOFs, and problem only supports {AvailableNodeDOFs} DOFs.");

            // dont want to create node where one already is
            if (NodeExtensions.NodeExistsAtLocation(node.Coordinates, Nodes))
                throw new Exception("Tried to add a node at location where one already exists. Nodes cannot be in identical locations.");

            _Nodes[node.ID] = node;
        }

        /// <summary>
        /// Removes a node from the problem
        /// </summary>
        /// <param name="nodeID">ID of the node to remove</param>
        /// <returns></returns>
        public int[] RemoveNode(int nodeID)
        {
            // If we removed a node successfully
            if (_Nodes.Remove(nodeID))
            {
                // Return removed element IDs
                return RemoveHangingElements(nodeID); 
            }
            return []; // We didn't remove any elements
        }

        /// <summary>
        /// Adds an element to the problem
        /// </summary>
        /// <param name="element">The element to add</param>
        /// <exception cref="ArgumentException">An element parameter was invalid</exception>
        public void AddElement(IElement element)
        {
            if (_Elements.ContainsKey(element.ID))
                throw new ArgumentException($"Could not add element {element.ID} to problem. Problem already contains that ID.");

            if (!AvailableElements.Contains(element.ElementType))
                throw new ArgumentException($"Could not add element {element.ID} to problem. Problem does not support element type {Enum.GetName(element.ElementType)}.");

            // Make sure we aren't adding a duplicate element
            if (ElementExtensions.ContainsElementWithSameNodeIDs(Elements, element))
                throw new ArgumentException($"Could not add element {element.ID} to problem. An element already exists that is linked to the same nodes.");

            _Elements[element.ID] = element;

            foreach(var node in element.Nodes)
            {
                if (!_Nodes.ContainsKey(node.ID))
                    AddNode((Node)node);
            }
        }

        /// <summary>
        /// Removes an element from the program
        /// </summary>
        /// <param name="elementID">ID of the element to remove</param>
        public void RemoveElement(int elementID)
        {
            _Elements.Remove(elementID);
        }

        /// <summary>
        /// Solve the stress problem
        /// </summary>
        public bool Solve()
        {
            var K_Matricies = Elements.ToDictionary(element => element.ID, element => element.K_Matrix());
            var nodeDOFS = Nodes.ToDictionary(n => n.ID, n => n.DOFs);
            var connectivityMatrix = ElementExtensions.Get_Connectivity_Matrix(Elements);

            SparseMatrix K_assembled = ElementExtensions.Assemble_K_Matrix(K_Matricies, nodeDOFS, connectivityMatrix);
            var F_assembled = NodeExtensions.F_Matrix(Nodes);
            var Q_assembled = NodeExtensions.Q_Matrix(Nodes);

            DenseVector[] output = Solver.Solve(K_assembled, F_assembled, Q_assembled);
            NodeExtensions.ApplySolution(Nodes, output[0], output[1]);

            foreach (var element in Elements)
                element.Solve(); // Set SolutionValid to true

            var displacements = output[0].Values;

            // False if there's a bad value
            return !(displacements.Contains(double.NaN) || displacements.Contains(double.PositiveInfinity) || displacements.Contains(double.NegativeInfinity));
        }

        // ---------------------- Private Helpers ----------------------------

        /// <summary>
        /// Deletes elements if a node is deleted and leaves one hanging
        /// </summary>
        /// <param name="nodeID"></param>
        private int[] RemoveHangingElements(int nodeID)
        {
            // Elements where any of the nodes matches the given ID
            List<IElement> elementsToDelete = _Elements.Values
                .Where(element => element.Nodes.Any(n => n.ID == nodeID))
                .ToList();

            foreach (var element in elementsToDelete)
                RemoveElement(element.ID);

            return elementsToDelete.Select(element => element.ID).ToArray();
        }

    }
}
