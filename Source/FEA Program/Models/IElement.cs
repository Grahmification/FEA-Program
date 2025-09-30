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

        /// <summary>
        /// Gets a displacement or position at the given location inside the element
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element to calculate the displacement</param>
        /// <param name="globalNodeQ">Global node displacement or position matrix for nodes in this element</param>
        /// <returns></returns>
        DenseMatrix Interpolated_Displacement(double[] localCoords, DenseMatrix globalNodeQ);

        /// <summary>
        /// Gets the element strain matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        DenseMatrix B_Matrix(List<double[]> nodeCoordinates); // needs to be given with local node 1 in first spot on list

        /// <summary>
        /// Gets the element length
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        double Length(List<double[]> nodeCoordinates);

        /// <summary>
        /// Gets the element stress matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <param name="globalNodeQ">Global node displacement matrix</param>
        /// <param name="localCoords">Optional local coordinates inside the element</param>
        /// <returns></returns>
        DenseMatrix StressMatrix(List<double[]> nodeCoordinates, DenseMatrix globalNodeQ, double[]? localCoords = null);

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

        /// <summary>
        /// Sorts the nodes for the correct ordering in the element
        /// </summary>
        /// <param name="nodes">The list to sort in place</param>
        void SortNodeOrder(ref List<INode> nodes);
        void SetBodyForce(DenseMatrix forcePerVolume);
        void SetTractionForce(DenseMatrix forcePerLength);
    }
}
