using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal interface IElement
    {
        int ID { get; }
        Material Material { get; set; }

        int NumOfNodes { get; }
        int NodeDOFs { get; }
        int ElementDOFs { get; }
        ElementTypes ElementType { get; }
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
        /// Get arguments that may vary between different element types
        /// </summary>
        public double[] ElementArgs { get; set; }

        // ---------------- Pre solution methods ----------------

        /// <summary>
        /// Sets the element body force
        /// </summary>
        /// <param name="forcePerVol">The body force matrix</param>
        void SetBodyForce(DenseVector forcePerVolume);

        /// <summary>
        /// Sets the element traction force
        /// </summary>
        /// <param name="forcePerLength">The traction force matrix</param>
        void SetTractionForce(DenseVector forcePerLength);

        /// <summary>
        /// Gets the element length
        /// </summary>
        /// <returns></returns>
        double Length();

        /// <summary>
        /// Gets the element stiffness matrix
        /// </summary>
        /// <returns></returns>
        DenseMatrix K_Matrix();

        /// <summary>
        /// Gets the element body force matrix
        /// </summary>
        /// <returns></returns>
        DenseVector BodyForceMatrix();

        /// <summary>
        /// Gets the element traction force matrix
        /// </summary>
        /// <returns></returns>
        DenseVector TractionForceMatrix();

        // ---------------- Post solution methods ----------------

        /// <summary>
        /// Gets a displacement or position at the given location inside the element
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element to calculate the displacement</param>
        /// <returns></returns>
        DenseVector Interpolated_Displacement(double[] localCoords);

        /// <summary>
        /// Gets the element stress matrix
        /// </summary>
        /// <param name="localCoords">Optional local coordinates inside the element</param>
        /// <returns></returns>
        DenseVector StressMatrix(double[]? localCoords = null);

    }

    /// <summary>
    /// Different types of elements
    /// </summary>
    public enum ElementTypes
    {
        BarLinear
    }
}
