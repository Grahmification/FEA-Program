using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    /// <summary>
    /// Base element subclass - common between all types of elements
    /// </summary>
    internal abstract class Element: IDClass
    {
        /// <summary>
        /// All nodes contained in the element
        /// </summary>
        private readonly INode[] _nodes;

        /// <summary>
        /// The element's material
        /// </summary>
        private Material _Material;

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when the solve state changes from true to false
        /// </summary>
        public event EventHandler<int>? SolutionInvalidated;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The element's material
        /// </summary>
        public Material Material
        {
            get { return _Material; }
            set { _Material = value; InvalidateSolution(); }
        } // flags the solution invalid if set

        /// <summary>
        /// All nodes contained in the element
        /// </summary>
        public IReadOnlyList<INode> Nodes => _nodes;

        /// <summary>
        /// True if the solution for the element is current
        /// </summary>
        public bool SolutionValid { get; protected set; } = false;

        /// <summary>
        /// The dimension of local coordinates inside the element, indicating the size of localCoordinates arguments. 1 = 1D, 2 = 2D, 3 = 3D.
        /// </summary>
        public abstract int LocalDimension { get; }

        /// <summary>
        /// The number of nodes in the element
        /// </summary>
        public abstract int NumOfNodes { get; }

        /// <summary>
        /// The DOFs in each nodes of the element
        /// </summary>
        public int NodeDOFs { get; private set; }

        /// <summary>
        /// The total DOFs in the element
        /// </summary>
        public int ElementDOFs => NumOfNodes * NodeDOFs;

        /// <summary>
        /// Get the max stress in the element
        /// </summary>
        public abstract double MaxStress { get; }

        /// <summary>
        /// Gets the safety factor for yielding
        /// </summary>
        public virtual double SafetyFactorYield { get { return MaxStress == 0 ? 0 : Math.Abs(Material.Sy / MaxStress); } }

        /// <summary>
        /// Gets the safety factor for failure
        /// </summary>
        public virtual double SafetyFactorUltimate { get { return MaxStress == 0 ? 0 : Math.Abs(Material.Sut / MaxStress); } }

        // ---------------------- Public Methods ----------------------

        public Element(int id, List<INode> nodes, Material material, int nodeDOFs) : base(id)
        {
            NodeDOFs = nodeDOFs; // This needs to be set before validation
            
            // Prepare the nodes
            ValidateNodes(nodes); // Sanity check them
            SortNodeOrder(ref nodes); // Sort them
            _nodes = [.. nodes];

            // If the node solution is invalidated, we also want to invalidate the element
            foreach(var node in nodes)
                node.SolutionInvalidated += (_, _) => InvalidateSolution();

            _Material = material;
        }

        /// <summary>
        /// Set the element SolutionValid to true
        /// </summary>
        public void Solve()
        {
            SolutionValid = true;
        }

        // ---------------- General matrix methods ----------------

        /// <summary>
        /// Gets the element stiffness matrix
        /// </summary>
        /// <returns></returns>
        public virtual DenseMatrix K_Matrix()
        {
            var K_local = new DenseMatrix(2, 2);
            K_local[0, 0] = 1;
            K_local[1, 0] = -1;
            K_local[0, 1] = -1;
            K_local[1, 1] = 1;

            var B = B_Matrix();
            var D = D_Matrix();

            // Note: This equation only works for elements where [B] is constant over the element's volume
            // K = Scaling * [B]^T * [D] * [B] 
            return (DenseMatrix)(Volume() * B.TransposeThisAndMultiply(D * B));
        }

        /// <summary>
        /// Gets the element stress vector
        /// </summary>
        /// <param name="localCoords">Optional local coordinates inside the element</param>
        /// <returns></returns>
        public DenseVector StressMatrix(double[]? localCoords = null)
        {
            DenseVector globalNodeQ = NodeExtensions.BuildVector([.. _nodes], (n) => n.Displacement);

            // Stress = [D] * [B] * [Q]
            return D_Matrix() * B_Matrix(localCoords) * globalNodeQ;
        }

        /// <summary>
        /// Gets a displacement or position at the given location inside the element
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element to calculate the displacement</param>
        /// <returns></returns>
        public DenseVector Interpolated_Displacement(double[] localCoords)
        {
            DenseVector globalNodeQ = NodeExtensions.BuildVector([.. _nodes], (n) => n.Displacement);
            return N_Matrix(localCoords) * globalNodeQ;
        }

        /// <summary>
        /// Get the material's constitutive matrix for the given element
        /// </summary>
        /// <returns></returns>
        protected DenseMatrix D_Matrix()
        {
            // Elements with 1D local coordinate space. D = [1x1]
            if (LocalDimension == 1)
            {
                return new DenseMatrix(1, 1, [Material.E]);
            }
            // Elements with 2D local coordinate space. D = [3x3]
            else if (LocalDimension == 2)
            {
                var V = Material.V;

                var output = new DenseMatrix(3, 3);
                output.Clear(); // Ensure all values are zero

                output[0, 0] = 1;
                output[0, 1] = V;
                output[1, 0] = V;
                output[1, 1] = 1;
                output[2, 2] = (1 - V) / 2.0;

                return (Material.E / (1.0 - V * V)) * output;
            }
            else
            {
                // TODO: Add the 3D case
                throw new NotImplementedException();
            }
        }

        // ---------------- Matrix Methods which parent must define ----------------

        /// <summary>
        /// Gets the element's volume
        /// </summary>
        protected abstract double Volume();

        /// <summary>
        /// Gets the element strain / displacement matrix
        /// </summary>
        /// <returns></returns>
        protected abstract DenseMatrix B_Matrix(double[]? localCoords = null);

        /// <summary>
        /// Gets the element shape function (interpolation) matrix
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element</param>
        /// <returns></returns>
        protected abstract DenseMatrix N_Matrix(double[] localCoords);

        /// <summary>
        /// Sorts the nodes for the correct ordering in the element
        /// </summary>
        /// <param name="nodes">The list to sort in place</param>
        protected abstract void SortNodeOrder(ref List<INode> nodes);

        // ---------------- Helper methods ----------------

        /// <summary>
        /// Checks that the nodes are correct for this type of element
        /// </summary>
        /// <param name="nodes">The nodes to check</param>
        /// <exception cref="ArgumentException">A node was invalid</exception>
        private void ValidateNodes(List<INode> nodes)
        {
            if (nodes.Count != NumOfNodes)
                throw new ArgumentException($"Cannot create element. {nodes.Count} nodes were specified, but element requires {NumOfNodes}.");

            foreach(INode node in nodes)
                if(node.Dimension != NodeDOFs)
                    throw new ArgumentException($"Cannot create element. Node {node.ID} has {node.Dimension} DOFs, but element requires {NodeDOFs}.");
        }

        /// <summary>
        /// Changes the solved state from true to false, indicating the result is no longer valid
        /// </summary>
        protected void InvalidateSolution()
        {
            // Only raise the event if we're switching from true to false
            bool solutionWasvalid = SolutionValid;
            SolutionValid = false;

            if (solutionWasvalid)
                SolutionInvalidated?.Invoke(this, ID);
        }

        /// <summary>
        /// Checks that the length of certain parameters is correct for this element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coordinates">The collection to check</param>
        /// <param name="targetLength">The correct length of the collection</param>
        /// <param name="methodName">The calling method name</param>
        /// <exception cref="ArgumentOutOfRangeException">The length was invalid</exception>
        protected void ValidateLength<T>(IReadOnlyCollection<T> coordinates, int targetLength, string? methodName)
        {
            methodName ??= "Unknown";

            if (coordinates.Count != targetLength)
            {
                throw new ArgumentOutOfRangeException($"Wrong number of array values input to <{methodName}> for Element ID <{ID}>. Element has {NodeDOFs} node DOFs.");
            }
        }
    }
}
