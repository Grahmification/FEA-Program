using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
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

        public ProblemTypes ProblemType { get; private set; }
        public Solver Solver { get; private set; } = new Solver();
        public List<IElement> Elements => _Elements.Values.ToList();
        public List<Node> Nodes => _Nodes.Values.ToList();

        /// <summary>
        /// which elements are available depending on problem type
        /// </summary>
        public ElementTypes[] AvailableElements => ProblemType switch
        {
            ProblemTypes.Bar_1D => [ElementTypes.BarLinear],
            ProblemTypes.Truss_3D => [ElementTypes.BarLinear],

            // Default case: return empty
            _ => []
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

        // ---------------------- Public Methods ----------------------------
        public StressProblem(ProblemTypes problemType = ProblemTypes.Bar_1D)
        {
            ProblemType = problemType;
        }
        public void AddNode(Node node)
        {
            if (_Nodes.ContainsKey(node.ID))
                throw new ArgumentException($"Could not add node {node.ID} to problem. Problem already contains that ID.");

            // dont want to create node where one already is
            if (NodeExistsAtLocation(node.Coordinates, Nodes))
                throw new Exception("Tried to add a node at location where one already exists. Nodes cannot be in identical locations.");

            _Nodes[node.ID] = node;
        }
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
        public void AddElement(IElement element)
        {
            if (_Elements.ContainsKey(element.ID))
                throw new ArgumentException($"Could not add element {element.ID} to problem. Problem already contains that ID.");

            // Make sure we aren't adding a duplicate element
            if(ContainsElementWithSameNodeIDs(Elements, element))
                throw new ArgumentException($"Could not add element {element.ID} to problem. An element already exists that is linked to the same nodes.");

            _Elements[element.ID] = element;

            foreach(var node in element.Nodes)
            {
                if (!_Nodes.ContainsKey(node.ID))
                    AddNode((Node)node);
            }
        }
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
            var nodeDOFS = Nodes.ToDictionary(n => n.ID, n => n.Dimension);
            var connectivityMatrix = ElementExtensions.Get_Connectivity_Matrix(Elements);

            SparseMatrix K_assembled = ElementExtensions.Assemble_K_Matrix(K_Matricies, nodeDOFS, connectivityMatrix);
            var F_assembled = NodeExtensions.F_Matrix(Nodes);
            var Q_assembled = NodeExtensions.Q_Matrix(Nodes);

            DenseVector[] output = Solver.Solve(K_assembled, F_assembled, Q_assembled);
            NodeExtensions.ApplySolution(Nodes, output[0], output[1]);

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

        // ---------------------- Static Methods ----------------------------

        /// <summary>
        /// Returns true if a node already exists at the given location
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static bool NodeExistsAtLocation(double[] coords, List<Node> existingNodes)
        {
            foreach (Node node in existingNodes)
            {
                // This should work regardless of dimension
                if (node.Coordinates.Take(node.Dimension).SequenceEqual(coords.Take(node.Dimension)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a list of elements contains an element whose NodeIDs array is structurally
        /// equivalent to the input element's NodeIDs, regardless of the order of IDs.
        /// </summary>
        /// <param name="elements">The list of elements to search through (the haystack).</param>
        /// <param name="inputElement">The element to compare against (the needle).</param>
        /// <returns>True if a match is found, otherwise false.</returns>
        public static bool ContainsElementWithSameNodeIDs(List<IElement> elements, IElement inputElement)
        {
            if (elements == null || inputElement == null)
            {
                return false;
            }

            // 1. Get the canonical (sorted) representation of the input element's NodeIDs.
            //    Using ToList() is often necessary for SequenceEqual to work reliably against another list/array.
            List<int> sortedInputIDs = inputElement.Nodes.Select(n => n.ID).OrderBy(id => id).ToList();

            // 2. Use LINQ's Any() method to iterate and perform the comparison efficiently.
            return elements.Any(elementInList =>
            {
                // First, quickly check if the counts are different. If so, they cannot match.
                if (elementInList.Nodes.Count != sortedInputIDs.Count)
                {
                    return false;
                }

                // 3. For the element in the list, get its own canonical (sorted) representation.
                List<int> sortedListIDs = elementInList.Nodes.Select(n => n.ID).OrderBy(id => id).ToList();

                // 4. Use LINQ's SequenceEqual() to check if the two sorted sequences are identical.
                // This is the core logic for order-agnostic array comparison.
                return sortedInputIDs.SequenceEqual(sortedListIDs);
            });
        }

    }

    public enum ProblemTypes
    {
        Bar_1D,
        Beam_1D,
        Truss_3D
    }
}
