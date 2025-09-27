using MathNet.Numerics.LinearAlgebra.Double;
using System.Reflection;
using OpenTK.Graphics.OpenGL;

namespace FEA_Program.Models
{
    internal class ElementBarLinear : Element, IElement
    {
        private double _Area = 0d; // x-section area in m^2
        private double _BodyForce = 0d; // Body force in N/m^3
        private double _TractionForce = 0d; // Traction force in N/m

        public override Type MyType => GetType();

        private DenseMatrix N_mtrx(double[] IntrinsicCoords)
        {
            if (IntrinsicCoords.Length != 1)
            {
                throw new Exception("Wrong number of coords input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            double eta = IntrinsicCoords[0];

            var N = new DenseMatrix(1, NodeDOFs * NumOfNodes); // u = Nq - size based off total number of element DOFs
            N[0, 0] = (1d - eta) / 2.0d;
            N[0, 1] = (1d + eta) / 2.0d;

            return N;
        }
        public DenseMatrix Interpolated_Displacement(double[] IntrinsicCoords, DenseMatrix GblNodeQ)
        {
            if (GblNodeQ.Values.Length != ElemDOFs)
            {
                throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            return N_mtrx(IntrinsicCoords) * GblNodeQ;
        } // can interpolate either position or displacement
        public DenseMatrix B_mtrx(List<double[]> GblNodeCoords)
        {
            if (GblNodeCoords.Count != NumOfNodes)
            {
                throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            var B_out = new DenseMatrix(1, NodeDOFs * NumOfNodes); // based from total DOFs
            B_out[0, 0] = -1.0d;
            B_out[0, 1] = 1.0d;

            B_out = B_out * (1d / Length(GblNodeCoords));  // B = [-1 1]*1/(x2-x1)

            return B_out;
        } // needs to be given with local node 1 in first spot on list
        public double Length(List<double[]> GblNodeCoords)
        {
            if (GblNodeCoords.Count != NumOfNodes)
            {
                throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            double output = GblNodeCoords[1][0] - GblNodeCoords[0][0];

            if (output < 0d)
            {
                throw new Exception("Nodes given in wrong order to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            return output;
        }
        public DenseMatrix Stress_mtrx(List<double[]> GblNodeCoords, DenseMatrix GblNodeQ, double E, double[] IntrinsicCoords = null)
        {
            if (GblNodeCoords.Count != NumOfNodes | GblNodeQ.Values.Length != ElemDOFs)
            {
                throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            DenseMatrix output = E * B_mtrx(GblNodeCoords) * GblNodeQ;
            return output;
        } // node 1 displacement comes first in disp input, followed by second
        public DenseMatrix K_mtrx(List<double[]> GblNodeCoords, double E)
        {
            if (GblNodeCoords.Count != NumOfNodes)
            {
                throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            var output = new DenseMatrix(ElemDOFs, ElemDOFs);
            output[0, 0] = 1;
            output[1, 0] = -1;
            output[0, 1] = -1;
            output[1, 1] = 1;

            return (DenseMatrix)(output * E * _Area / Length(GblNodeCoords));
        } // node 1 displacement comes first in disp input, followed by second
        public DenseMatrix BodyForce_mtrx(List<double[]> GblNodeCoords)
        {
            if (GblNodeCoords.Count != ElemDOFs)
            {
                throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            var output = new DenseMatrix(ElemDOFs, 1);
            output[0, 0] = 1;
            output[1, 0] = 1;

            output = output * _Area * Length(GblNodeCoords) * _BodyForce * 0.5d;

            return output;
        } // node 1 displacement comes first in disp input, followed by second
        public DenseMatrix TractionForce_mtrx(List<double[]> GblNodeCoords)
        {
            if (GblNodeCoords.Count != NumOfNodes)
            {
                throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            var output = new DenseMatrix(ElemDOFs, 1);
            output[0, 0] = 1;
            output[1, 0] = 1;

            output = output * Length(GblNodeCoords) * _TractionForce * 0.5d;

            return output;
        }

        public ElementBarLinear(double Area, int ID, int Mat = -1) : base(System.Drawing.Color.Green, ID, Mat)
        {

            _Area = Area;
        }

        public void SortNodeOrder(ref List<int> NodeIDs, List<double[]> NodeCoords)
        {
            if (NodeIDs.Count != NumOfNodes | NodeCoords.Count != NumOfNodes) // error handling
            {
                throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ".");
            }

            var SortedIdList = new List<int>();

            if (NodeCoords[0][0] < NodeCoords[1][0]) // larger x-coord is local element 2
            {
                SortedIdList.Add(NodeIDs[0]);
                SortedIdList.Add(NodeIDs[1]);
            }
            else
            {
                SortedIdList.Add(NodeIDs[1]);
                SortedIdList.Add(NodeIDs[0]);
            }

            NodeIDs = SortedIdList;
        }
        public void SetBodyForce(DenseMatrix ForcePerVol)
        {
            if (ForcePerVol.Values.Length != NodeDOFs) // can only have forces in directions of DOFs
            {
                throw new Exception("Wrong number of Forces input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            _BodyForce = ForcePerVol[0, 0];
            InvalidateSolution();
        }
        public void SetTractionForce(DenseMatrix ForcePerLength)
        {
            if (ForcePerLength.Values.Length != NodeDOFs) // can only have forces in directions of DOFs
            {
                throw new Exception("Wrong number of Forces input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            _BodyForce = ForcePerLength[0, 0];
            InvalidateSolution();
        }
        public void Draw(List<double[]> GblNodeCoords)
        {

            if (GblNodeCoords.Count != NumOfNodes)
            {
                throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
            }

            GL.LineWidth(2);

            GL.Begin(PrimitiveType.Lines);

            if (NodeDOFs == 1)
            {
                for (int i = 0, loopTo = NumOfNodes - 1; i <= loopTo; i++)
                {
                    GL.Color3(_Color[i]);
                    GL.Vertex3(GblNodeCoords[i][0], 0, 0);
                }
            }
            else if (NodeDOFs == 2)
            {
                for (int i = 0, loopTo2 = NumOfNodes - 1; i <= loopTo2; i++)
                {
                    GL.Color3(_Color[i]);
                    GL.Vertex3(GblNodeCoords[i][0], GblNodeCoords[i][1], 0);
                }
            }
            else
            {
                for (int i = 0, loopTo1 = NumOfNodes - 1; i <= loopTo1; i++)
                {
                    GL.Color3(_Color[i]);
                    GL.Vertex3(GblNodeCoords[i][0], GblNodeCoords[i][1], GblNodeCoords[i][2]);
                }
            }

            GL.End();

            GL.LineWidth(1);
        }

    }
}
