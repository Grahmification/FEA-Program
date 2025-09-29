using MathNet.Numerics.LinearAlgebra.Double;
using System.Reflection;

namespace FEA_Program.Models
{
    internal class ElementBarLinear : Element, IElement
    {
        private double _Area = 0; // x-section area in m^2
        private double _BodyForce = 0; // Body force in N/m^3
        private double _TractionForce = 0; // Traction force in N/m

        public override string Name => "Bar_Linear";
        public override int NumOfNodes => 2;
        public override int NodeDOFs => 1;

        public ElementBarLinear(double area, int id, int material = -1) : base(id, material)
        {
            if (area <= 0)
                throw new ArgumentException($"Cannot create {Name} element with non-positive area.");

            _Area = area;
        }

        public DenseMatrix Interpolated_Displacement(double[] intrinsicCoords, DenseMatrix globalNodeQ)
        {
            ValidateLength(globalNodeQ.Values, ElementDOFs, MethodBase.GetCurrentMethod()?.Name);
            return N_matrix(intrinsicCoords) * globalNodeQ;
        } // can interpolate either position or displacement
        public DenseMatrix B_Matrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var B_out = new DenseMatrix(1, ElementDOFs); // based from total DOFs
            B_out[0, 0] = -1.0d;
            B_out[0, 1] = 1.0d;

            return B_out * (1d / Length(nodeCoordinates)); // B = [-1 1]*1/(x2-x1)
        } // needs to be given with local node 1 in first spot on list
        public double Length(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);
            return Math.Abs(nodeCoordinates[1][0] - nodeCoordinates[0][0]);
        }
        public DenseMatrix StressMatrix(List<double[]> nodeCoordinates, DenseMatrix globalNodeQ, double E, double[]? intrinsicCoords = null)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);
            ValidateLength(globalNodeQ.Values, ElementDOFs, MethodBase.GetCurrentMethod()?.Name);

            return E * B_Matrix(nodeCoordinates) * globalNodeQ;
        } // node 1 displacement comes first in disp input, followed by second
        public DenseMatrix K_Matrix(List<double[]> nodeCoordinates, double E)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var output = new DenseMatrix(ElementDOFs, ElementDOFs);
            output[0, 0] = 1;
            output[1, 0] = -1;
            output[0, 1] = -1;
            output[1, 1] = 1;

            return (DenseMatrix)(output * E * _Area / Length(nodeCoordinates));
        } // node 1 displacement comes first in disp input, followed by second
        public DenseMatrix BodyForceMatrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var output = new DenseMatrix(ElementDOFs, 1);
            output[0, 0] = 1;
            output[1, 0] = 1;

            return output * _Area * Length(nodeCoordinates) * _BodyForce * 0.5d;
        } // node 1 displacement comes first in disp input, followed by second
        public DenseMatrix TractionForceMatrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var output = new DenseMatrix(ElementDOFs, 1);
            output[0, 0] = 1;
            output[1, 0] = 1;

            output = output * Length(nodeCoordinates) * _TractionForce * 0.5d;

            return output;
        }

        public void SortNodeOrder(ref List<INode> nodes)
        {
            ValidateLength(nodes, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            //Order from smallest to largest X coordinate
            nodes = nodes.OrderBy(n => n.Coords[0]).ToList();
        }
        public void SetBodyForce(DenseMatrix forcePerVol)
        {
            ValidateLength(forcePerVol.Values, NodeDOFs, MethodBase.GetCurrentMethod()?.Name);
            _BodyForce = forcePerVol[0, 0];
            InvalidateSolution(); // TODO: Confirm this line is correct. Believe this should set solved true
        }
        public void SetTractionForce(DenseMatrix forcePerLength)
        {
            ValidateLength(forcePerLength.Values, NodeDOFs, MethodBase.GetCurrentMethod()?.Name);
            _TractionForce = forcePerLength[0, 0];
            InvalidateSolution(); // TODO: Confirm this line is correct. Believe this should set solved true
        }

        private DenseMatrix N_matrix(double[] intrinsicCoords)
        {
            ValidateLength(intrinsicCoords, 1, MethodBase.GetCurrentMethod()?.Name);
            double eta = intrinsicCoords[0];

            var n = new DenseMatrix(1, ElementDOFs); // u = Nq - size based off total number of element DOFs
            n[0, 0] = (1 - eta) / 2.0d;
            n[0, 1] = (1 + eta) / 2.0d;

            return n;
        }
    }
}
