using MathNet.Numerics.LinearAlgebra.Double;
using System.Reflection;

namespace FEA_Program.Models
{
    /// <summary>
    /// Base element subclass - common between all types of elements
    /// </summary>
    internal abstract class Element
    {
        private Material _Material;
        private bool _ReadyToSolve = false; // is true if the nodes of the element are set up properly
        
        public event EventHandler<int>? SolutionInvalidated;

        public int ID { get; private set; }
        public Material Material
        {
            get { return _Material; }
            set { _Material = value; InvalidateSolution(); }
        } // flags the solution invalid if set
        public bool SolutionValid { get; protected set; } = false; // is true if the solution for the element is correct
        public abstract int NumOfNodes { get; }
        public abstract int NodeDOFs { get; protected set; }
        public int ElementDOFs => NumOfNodes * NodeDOFs;

        public Element(int id, Material material)
        {
            _Material = material;
            ID = id;
        }

        // ---------------- General matrix methods ----------------

        /// <summary>
        /// Gets the element stiffness matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        public DenseMatrix K_Matrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var K_local = new DenseMatrix(2, 2);
            K_local[0, 0] = 1;
            K_local[1, 0] = -1;
            K_local[0, 1] = -1;
            K_local[1, 1] = 1;

            var B = B_Matrix(nodeCoordinates);
            var D = D_Matrix();

            // K = Scaling * [B]^T * [D] * [B] 
            return (DenseMatrix)(StiffnessScalingFactor(nodeCoordinates) * B.TransposeThisAndMultiply(D * B));
        }

        /// <summary>
        /// Gets the element stress vector
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <param name="globalNodeQ">Global node displacement vector</param>
        /// <param name="localCoords">Optional local coordinates inside the element</param>
        /// <returns></returns>
        public DenseVector StressMatrix(List<double[]> nodeCoordinates, DenseVector globalNodeQ, double[]? localCoords = null)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);
            ValidateLength(globalNodeQ.Values, ElementDOFs, MethodBase.GetCurrentMethod()?.Name);

            // Stress = [D] * [B] * [Q]
            return D_Matrix() * B_Matrix(nodeCoordinates) * globalNodeQ;
        }

        /// <summary>
        /// Gets a displacement or position at the given location inside the element
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element to calculate the displacement</param>
        /// <param name="globalNodeQ">Global node displacement or position matrix for nodes in this element</param>
        /// <returns></returns>
        public DenseVector Interpolated_Displacement(double[] localCoords, DenseVector globalNodeQ)
        {
            ValidateLength(globalNodeQ.Values, ElementDOFs, MethodBase.GetCurrentMethod()?.Name);
            return N_Matrix(localCoords) * globalNodeQ;
        }

        // ---------------- Matrix Methods which parent must define ----------------

        /// <summary>
        /// Gets the scaling factor for the element's K matrix
        /// </summary>
        protected abstract double StiffnessScalingFactor(List<double[]> nodeCoordinates);

        /// <summary>
        /// Gets the element strain / displacement matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        protected abstract DenseMatrix B_Matrix(List<double[]> nodeCoordinates);

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

        // ---------------- Helper methods ----------------

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
