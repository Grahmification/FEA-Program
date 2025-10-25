using FEA_Program.Converters;
using MathNet.Numerics.LinearAlgebra.Double;
using System.ComponentModel;

namespace FEA_Program.Models
{
    internal interface IElement: IHasID, INotifyPropertyChanged
    {
        public event EventHandler<int>? SolutionInvalidated;
        
        public Material Material { get; set; }

        public int NumOfNodes { get; }
        public int NodeDOFs { get; }
        public int ElementDOFs { get; }
        public ElementTypes ElementType { get; }
        public IReadOnlyList<INode> Nodes { get; }

        /// <summary>
        /// Element body force in N/m^3 [X, Y, Z]^T
        /// </summary>
        public DenseVector BodyForce { get; }

        /// <summary>
        /// Element traction force in N/m [X, Y, Z]^T
        /// </summary>
        public DenseVector TractionForce { get; }

        public bool SolutionValid { get; }
        public double MaxStress { get; }


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

    /// <summary>
    /// Different types of elements
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ElementTypes
    {
        [Description("Linear Bar")]
        BarLinear
    }
}
