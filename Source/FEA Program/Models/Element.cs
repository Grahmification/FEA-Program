using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    /// <summary>
    /// Base element subclass - common between all types of elements
    /// </summary>
    internal abstract class Element: IDClass
    {
        private readonly INode[] _nodes;
        private Material _Material;
        private bool _ReadyToSolve = false; // is true if the nodes of the element are set up properly
        
        public event EventHandler<int>? SolutionInvalidated;

        public Material Material
        {
            get { return _Material; }
            set { _Material = value; InvalidateSolution(); }
        } // flags the solution invalid if set
        public IReadOnlyList<INode> Nodes => _nodes;
        public bool SolutionValid { get; protected set; } = false; // is true if the solution for the element is correct
        public abstract int NumOfNodes { get; }
        public int NodeDOFs { get; private set; }
        public int ElementDOFs => NumOfNodes * NodeDOFs;

        public Element(int id, List<INode> nodes, Material material, int nodeDOFs) : base(id)
        {
            NodeDOFs = nodeDOFs; // This needs to be set before validation
            
            // Prepare the nodes
            ValidateNodes(nodes); // Sanity check them
            SortNodeOrder(ref nodes); // Sort them
            _nodes = [.. nodes];

            _Material = material;
        }

        // ---------------- General matrix methods ----------------

        /// <summary>
        /// Gets the element stiffness matrix
        /// </summary>
        /// <returns></returns>
        public DenseMatrix K_Matrix()
        {
            var K_local = new DenseMatrix(2, 2);
            K_local[0, 0] = 1;
            K_local[1, 0] = -1;
            K_local[0, 1] = -1;
            K_local[1, 1] = 1;

            var B = B_Matrix();
            var D = D_Matrix();

            // K = Scaling * [B]^T * [D] * [B] 
            return (DenseMatrix)(StiffnessScalingFactor() * B.TransposeThisAndMultiply(D * B));
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
            return D_Matrix() * B_Matrix() * globalNodeQ;
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

        // ---------------- Matrix Methods which parent must define ----------------

        /// <summary>
        /// Gets the scaling factor for the element's K matrix
        /// </summary>
        protected abstract double StiffnessScalingFactor();

        /// <summary>
        /// Gets the element strain / displacement matrix
        /// </summary>
        /// <returns></returns>
        protected abstract DenseMatrix B_Matrix();

        /// <summary>
        /// Get the material's constitutive matrix for the given element
        /// </summary>
        /// <returns></returns>
        protected abstract DenseMatrix D_Matrix();

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

        private void ValidateNodes(List<INode> nodes)
        {
            if (nodes.Count != NumOfNodes)
                throw new ArgumentException($"Cannot create element. {nodes.Count} nodes were specified, but element requires {NumOfNodes}.");

            foreach(INode node in nodes)
                if(node.Dimension != NodeDOFs)
                    throw new ArgumentException($"Cannot create element. Node {node.ID} has {node.Dimension} DOFs, but element requires {NodeDOFs}.");
        }

        protected void InvalidateSolution()
        {
            // Only raise the event if we're switching from true to false
            bool solutionWasvalid = SolutionValid;
            SolutionValid = false;

            if (solutionWasvalid)
                SolutionInvalidated?.Invoke(this, ID);
        }

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
