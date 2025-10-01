using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal interface IElement
    {
        int ID { get; }
        Material Material { get; set; }
        string Name { get; }
        int NumOfNodes { get; }
        int NodeDOFs { get; }
        int ElementDOFs { get; }

        // ---------------- Pre solution methods ----------------

        /// <summary>
        /// Sorts the nodes for the correct ordering in the element
        /// </summary>
        /// <param name="nodes">The list to sort in place</param>
        void SortNodeOrder(ref List<INode> nodes);

        /// <summary>
        /// Sets the element body force
        /// </summary>
        /// <param name="forcePerVol">The body force matrix</param>
        void SetBodyForce(DenseMatrix forcePerVolume);

        /// <summary>
        /// Sets the element traction force
        /// </summary>
        /// <param name="forcePerLength">The traction force matrix</param>
        void SetTractionForce(DenseMatrix forcePerLength);

        /// <summary>
        /// Gets the element length
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        double Length(List<double[]> nodeCoordinates);

        /// <summary>
        /// Gets the element stiffness matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        DenseMatrix K_Matrix(List<double[]> nodeCoordinates);

        /// <summary>
        /// Gets the element body force matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        DenseMatrix BodyForceMatrix(List<double[]> nodeCoordinates);

        /// <summary>
        /// Gets the element traction force matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        DenseMatrix TractionForceMatrix(List<double[]> nodeCoordinates);

        // ---------------- Post solution methods ----------------

        /// <summary>
        /// Gets a displacement or position at the given location inside the element
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element to calculate the displacement</param>
        /// <param name="globalNodeQ">Global node displacement or position matrix for nodes in this element</param>
        /// <returns></returns>
        DenseVector Interpolated_Displacement(double[] localCoords, DenseVector globalNodeQ);

        /// <summary>
        /// Gets the element stress matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <param name="globalNodeQ">Global node displacement matrix</param>
        /// <param name="localCoords">Optional local coordinates inside the element</param>
        /// <returns></returns>
        DenseVector StressMatrix(List<double[]> nodeCoordinates, DenseVector globalNodeQ, double[]? localCoords = null);

    }
}
