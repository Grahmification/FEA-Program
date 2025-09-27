using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal interface IElement : IBaseElement
    {
        Type MyType { get; }
        DenseMatrix Interpolated_Displacement(double[] IntrinsicCoords, DenseMatrix GblNodeQ); // can interpolate either position or displacement
        DenseMatrix B_mtrx(List<double[]> GblNodeCoords); // needs to be given with local node 1 in first spot on list
        double Length(List<double[]> GblNodeCoords);
        DenseMatrix Stress_mtrx(List<double[]> GblNodeCoords, DenseMatrix GblNodeQ, double E, double[]? IntrinsicCoords = null);

        DenseMatrix K_mtrx(List<double[]> GblNodeCoords, double E); // node 1 displacement comes first in disp input, followed by second
        DenseMatrix BodyForce_mtrx(List<double[]> GblNodeCoords); // node 1 displacement comes first in disp input, followed by second
        DenseMatrix TractionForce_mtrx(List<double[]> GblNodeCoords);


        void SortNodeOrder(ref List<int> NodeIDs, List<double[]> NodeCoords);
        void SetBodyForce(DenseMatrix ForcePerVol);
        void SetTractionForce(DenseMatrix ForcePerLength);
        void Draw(List<double[]> GblNodeCoords);
    }
}
