using MathNet.Numerics.LinearAlgebra.Double;
using System.ComponentModel;

namespace FEA_Program.Models
{
    /// <summary>
    /// Generic definition for an element
    /// </summary>
    internal interface IElement: IHasID, INotifyPropertyChanged
    {
        /// <summary>
        /// Fires when the solve state changes from true to false
        /// </summary>
        public event EventHandler<int>? SolutionInvalidated;

        /// <summary>
        /// The element's material
        /// </summary>
        public Material Material { get; set; }

        /// <summary>
        /// The dimension of the element in global problem coordinates
        /// </summary>
        public Dimensions Dimension { get; }

        /// <summary>
        /// The number of nodes in the element
        /// </summary>
        public int NumOfNodes { get; }

        /// <summary>
        /// Type identifier for the element
        /// </summary>
        public ElementTypes ElementType { get; }

        /// <summary>
        /// All nodes contained in the element
        /// </summary>
        public IReadOnlyList<INode> Nodes { get; }

        /// <summary>
        /// Element body force in N/m^3 [X, Y, Z]^T
        /// </summary>
        public DenseVector BodyForce { get; }

        /// <summary>
        /// Element traction force in N/m [X, Y, Z]^T
        /// </summary>
        public DenseVector TractionForce { get; }

        /// <summary>
        /// True if the solution for the element is current
        /// </summary>
        public bool SolutionValid { get; }

        /// <summary>
        /// Get the max stress in the element
        /// </summary>
        public double MaxStress { get; }

        /// <summary>
        /// Gets the safety factor for yielding
        /// </summary>
        public double SafetyFactorYield { get; }

        /// <summary>
        /// Gets the safety factor for failure
        /// </summary>
        public double SafetyFactorUltimate { get; }

        /// <summary>
        /// Get arguments that may vary between different element types
        /// </summary>
        public double[] ElementArgs { get; set; }  // Note: The whole array must be set, or this call will fail

        // ---------------- Pre solution methods ----------------

        /// <summary>
        /// Sets the element body force
        /// </summary>
        /// <param name="forcePerVol">The body force matrix</param>
        public void SetBodyForce(DenseVector forcePerVolume);

        /// <summary>
        /// Sets the element traction force
        /// </summary>
        /// <param name="forcePerLength">The traction force matrix</param>
        public void SetTractionForce(DenseVector forcePerLength);

        /// <summary>
        /// Gets the element length
        /// </summary>
        /// <returns></returns>
        public double Length();

        /// <summary>
        /// Gets the element stiffness matrix
        /// </summary>
        /// <returns></returns>
        public DenseMatrix K_Matrix();

        /// <summary>
        /// Gets the element body force matrix
        /// </summary>
        /// <returns></returns>
        public DenseVector BodyForceMatrix();

        /// <summary>
        /// Gets the element traction force matrix
        /// </summary>
        /// <returns></returns>
        public DenseVector TractionForceMatrix();

        /// <summary>
        /// Set the element SolutionValid to true
        /// </summary>
        public void Solve();

        // ---------------- Post solution methods ----------------

        /// <summary>
        /// Gets a displacement or position at the given location inside the element
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element to calculate the displacement</param>
        /// <returns></returns>
        public DenseVector Interpolated_Displacement(double[] localCoords);

        /// <summary>
        /// Gets the element stress matrix
        /// </summary>
        /// <param name="localCoords">Optional local coordinates inside the element</param>
        /// <returns></returns>
        public DenseVector StressMatrix(double[]? localCoords = null);

    }
}
