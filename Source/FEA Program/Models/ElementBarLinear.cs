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

        public ElementBarLinear(double area, int id, Material material) : base(id, material)
        {
            if (area <= 0)
                throw new ArgumentException($"Cannot create {Name} element with non-positive area.");

            _Area = area;
        }

        /// <summary>
        /// Gets a displacement or position at the given location inside the element
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element to calculate the displacement</param>
        /// <param name="globalNodeQ">Global node displacement or position matrix for nodes in this element</param>
        /// <returns></returns>
        public DenseMatrix Interpolated_Displacement(double[] localCoords, DenseMatrix globalNodeQ)
        {
            ValidateLength(globalNodeQ.Values, ElementDOFs, MethodBase.GetCurrentMethod()?.Name);
            return N_matrix(localCoords) * globalNodeQ;
        }

        /// <summary>
        /// Gets the element strain matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        public DenseMatrix B_Matrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var B_out = new DenseMatrix(1, ElementDOFs); // based from total DOFs
            B_out[0, 0] = -1.0;
            B_out[0, 1] = 1.0;

            return B_out * (1.0 / Length(nodeCoordinates)); // B = [-1 1]*1/(x2-x1)
        }

        /// <summary>
        /// Gets the element length
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        public double Length(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);
            return Math.Abs(nodeCoordinates[1][0] - nodeCoordinates[0][0]);
        }

        /// <summary>
        /// Gets the element stress matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <param name="globalNodeQ">Global node displacement matrix</param>
        /// <param name="localCoords">Optional local coordinates inside the element</param>
        /// <returns></returns>
        public DenseMatrix StressMatrix(List<double[]> nodeCoordinates, DenseMatrix globalNodeQ, double[]? localCoords = null)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);
            ValidateLength(globalNodeQ.Values, ElementDOFs, MethodBase.GetCurrentMethod()?.Name);

            return Material.E * B_Matrix(nodeCoordinates) * globalNodeQ;
        }
        
        /// <summary>
        /// Gets the element stiffness matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        public DenseMatrix K_Matrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var output = new DenseMatrix(ElementDOFs, ElementDOFs);
            output[0, 0] = 1;
            output[1, 0] = -1;
            output[0, 1] = -1;
            output[1, 1] = 1;

            return (DenseMatrix)(output * Material.E * _Area / Length(nodeCoordinates));
        }

        /// <summary>
        /// Gets the element body force matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        public DenseMatrix BodyForceMatrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var output = new DenseMatrix(ElementDOFs, 1);
            output[0, 0] = 1;
            output[1, 0] = 1;

            return output * _Area * Length(nodeCoordinates) * _BodyForce * 0.5d;
        }

        /// <summary>
        /// Gets the element traction force matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        public DenseMatrix TractionForceMatrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var output = new DenseMatrix(ElementDOFs, 1);
            output[0, 0] = 1;
            output[1, 0] = 1;

            output = output * Length(nodeCoordinates) * _TractionForce * 0.5d;

            return output;
        }

        /// <summary>
        /// Sorts the nodes for the correct ordering in the element
        /// </summary>
        /// <param name="nodes">The list to sort in place</param>
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

        /// <summary>
        /// Gets the element shape function (interpolation) matrix
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element</param>
        /// <returns></returns>
        private DenseMatrix N_matrix(double[] localCoords)
        {
            ValidateLength(localCoords, NodeDOFs, MethodBase.GetCurrentMethod()?.Name);
            double eta = localCoords[0];

            var n = new DenseMatrix(1, ElementDOFs); // u = Nq - size based off total number of element DOFs
            n[0, 0] = (1 - eta) / 2.0d;
            n[0, 1] = (1 + eta) / 2.0d;

            return n;
        }
    }
}
