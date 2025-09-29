using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal interface IElement
    {
        int ID { get; }
        int Material { get; set; }
        string Name { get; }
        int NumOfNodes { get; }
        int NodeDOFs { get; }
        int ElementDOFs { get; }

        DenseMatrix Interpolated_Displacement(double[] intrinsicCoords, DenseMatrix globalNodeQ); // can interpolate either position or displacement
        DenseMatrix B_Matrix(List<double[]> nodeCoordinates); // needs to be given with local node 1 in first spot on list
        double Length(List<double[]> nodeCoordinates);
        DenseMatrix StressMatrix(List<double[]> nodeCoordinates, DenseMatrix globalNodeQ, double e, double[]? intrinsicCoords = null);

        DenseMatrix K_Matrix(List<double[]> nodeCoordinates, double e); // node 1 displacement comes first in disp input, followed by second
        DenseMatrix BodyForceMatrix(List<double[]> nodeCoordinates); // node 1 displacement comes first in disp input, followed by second
        DenseMatrix TractionForceMatrix(List<double[]> nodeCoordinates);


        void SortNodeOrder(ref List<INode> nodes);
        void SetBodyForce(DenseMatrix forcePerVolume);
        void SetTractionForce(DenseMatrix forcePerLength);
    }
}
