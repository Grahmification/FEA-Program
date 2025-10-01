using MathNet.Numerics.LinearAlgebra.Double;
using System.Reflection;

namespace FEA_Program.Models
{
    internal class ElementBarLinear : Element, IElement
    {
        private double _Area = 0; // x-section area in m^2
        private double _BodyForce = 0; // Body force in N/m^3
        private double _TractionForce = 0; // Traction force in N/m

        public string Name => "Bar_Linear";
        public override int NumOfNodes => 2;
        public override int NodeDOFs { get; protected set; } = 1;

        public ElementBarLinear(double area, int id, Material material, int nodeDOFs = 1) : base(id, material)
        {
            if (area <= 0)
                throw new ArgumentException($"Cannot create {Name} element with non-positive area.");

            if (nodeDOFs != 1 & nodeDOFs != 2 & nodeDOFs != 3)
                throw new ArgumentException($"Cannot create {Name} element with {nodeDOFs} DOFs. Unsupported");

            NodeDOFs = nodeDOFs;
            _Area = area;
        }

        // ---------------- Public methods ----------------

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

        /// <summary>
        /// Sets the element body force
        /// </summary>
        /// <param name="forcePerVol">The body force matrix</param>
        public void SetBodyForce(DenseMatrix forcePerVol)
        {
            ValidateLength(forcePerVol.Values, NodeDOFs, MethodBase.GetCurrentMethod()?.Name);
            _BodyForce = forcePerVol[0, 0];
            InvalidateSolution();
        }

        /// <summary>
        /// Sets the element traction force
        /// </summary>
        /// <param name="forcePerLength">The traction force matrix</param>
        public void SetTractionForce(DenseMatrix forcePerLength)
        {
            ValidateLength(forcePerLength.Values, NodeDOFs, MethodBase.GetCurrentMethod()?.Name);
            _TractionForce = forcePerLength[0, 0];
            InvalidateSolution();
        }

        /// <summary>
        /// Gets the element length
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        public double Length(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);
            return Geometry.Length(nodeCoordinates[1], nodeCoordinates[0]);
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

        // ---------------- Base override methods ----------------

        /// <summary>
        /// Gets the scaling factor for the element's K matrix
        /// </summary>
        protected override double StiffnessScalingFactor(List<double[]> nodeCoordinates)
        {
            return Length(nodeCoordinates) * _Area;
        }

        /// <summary>
        /// Gets the element strain / displacement matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        protected override DenseMatrix B_Matrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            // Always B = [-1 1]*1/(x2-x1) for a 1D element
            var B_local = new DenseMatrix(1, 2); 
            B_local[0, 0] = -1.0;
            B_local[0, 1] = 1.0;
            
            // Convert to global coordinates and scale by the length to get the global B matrix
            return B_local * M_Matrix(nodeCoordinates) * (1.0 / Length(nodeCoordinates));
        }

        /// <summary>
        /// Gets the element shape function (interpolation) matrix
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element</param>
        /// <returns></returns>
        protected override DenseMatrix N_Matrix(double[] localCoords)
        {
            ValidateLength(localCoords, 1, MethodBase.GetCurrentMethod()?.Name);
            double eta = localCoords[0];

            var n = new DenseMatrix(1, ElementDOFs); // u = Nq - size based off total number of element DOFs
            n[0, 0] = (1 - eta) / 2.0d;
            n[0, 1] = (1 + eta) / 2.0d;

            return n;
        }

        /// <summary>
        /// Get the material's constitutive matrix for the given element
        /// </summary>
        /// <returns></returns>
        protected override DenseMatrix D_Matrix()
        {
            // For a truss element, D = [E] regardless of DOFs
            return new DenseMatrix(1, 1, [Material.E]);
        }

        // ---------------- Private helpers ----------------

        /// <summary>
        /// Get the transformation matrix from global node coordinates to local node coordinates for higher DOF elements
        /// </summary>
        /// <param name="nodeCoordinates"></param>
        /// <returns></returns>
        private DenseMatrix M_Matrix(List<double[]> nodeCoordinates)
        {
            if(NodeDOFs == 1)
            {
                return DenseMatrix.CreateIdentity(2); // Leaves B Matrix unchanged
            }
            else if (NodeDOFs == 2) 
            {
                (var l, var m, _) = Geometry.ComputeDirectionCosines(nodeCoordinates[1], nodeCoordinates[0]);
                var matrix = new DenseMatrix(2, 4);
                matrix[0, 0] = l;
                matrix[0, 1] = m;
                matrix[1, 2] = l;
                matrix[1, 3] = m;
                return matrix;
            }
            else // Node DOFs == 3
            {
                (var l, var m, var n) = Geometry.ComputeDirectionCosines(nodeCoordinates[1], nodeCoordinates[0]);
                var matrix = new DenseMatrix(2, 6);
                matrix[0, 0] = l;
                matrix[0, 1] = m;
                matrix[0, 2] = n;
                matrix[1, 3] = l;
                matrix[1, 4] = m;
                matrix[1, 5] = n;
                return matrix;
            }
        }

    }
}
