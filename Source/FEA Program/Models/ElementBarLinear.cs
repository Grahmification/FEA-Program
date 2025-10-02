using MathNet.Numerics.LinearAlgebra.Double;
using System.Reflection;

namespace FEA_Program.Models
{
    internal class ElementBarLinear : Element, IElement
    {
        private double _Area = 0; // x-section area in m^2

        public ElementTypes ElementType => ElementTypes.BarLinear;
        public override int NumOfNodes => 2;
        public override int NodeDOFs { get; protected set; } = 1;

        /// <summary>
        /// Element body force in N/m^3 [X, Y, Z]^T
        /// </summary>
        public DenseVector BodyForce { get; private set; }

        /// <summary>
        /// Element traction force in N/m [X, Y, Z]^T
        /// </summary>
        public DenseVector TractionForce { get; private set; }

        /// <summary>
        /// Get arguments that may vary between different element types
        /// </summary>
        public double[] ElementArgs { get => [_Area]; set => _Area = value[0]; }


        public ElementBarLinear(double area, int id, Material material, int nodeDOFs = 1) : base(id, material)
        {
            if (area <= 0)
                throw new ArgumentException($"Cannot create {ElementType} element with non-positive area.");

            if (nodeDOFs != 1 & nodeDOFs != 2 & nodeDOFs != 3)
                throw new ArgumentException($"Cannot create {ElementType} element with {nodeDOFs} DOFs. Unsupported");

            NodeDOFs = nodeDOFs;
            _Area = area;
            BodyForce = new DenseVector(NodeDOFs);
            TractionForce = new DenseVector(NodeDOFs);
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
            nodes = nodes.OrderBy(n => n.Coordinates[0]).ToList();
        }

        /// <summary>
        /// Sets the element body force
        /// </summary>
        /// <param name="forcePerVol">The body force matrix [Node DOF x 1]</param>
        public void SetBodyForce(DenseVector forcePerVol)
        {
            ValidateLength(forcePerVol.Values, NodeDOFs, MethodBase.GetCurrentMethod()?.Name);
            BodyForce = forcePerVol;
            InvalidateSolution();
        }

        /// <summary>
        /// Sets the element traction force
        /// </summary>
        /// <param name="forcePerLength">The traction force matrix [Node DOF x 1]</param>
        public void SetTractionForce(DenseVector forcePerLength)
        {
            ValidateLength(forcePerLength.Values, NodeDOFs, MethodBase.GetCurrentMethod()?.Name);
            TractionForce = forcePerLength;
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
        public DenseVector BodyForceMatrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var local_body = new DenseMatrix(ElementDOFs, NodeDOFs);
            if (NodeDOFs == 1)
            {
                // [1x1] = [2x1] * [1x1]
                local_body[0, 0] = 1;
                local_body[1, 0] = 1;
            }
            else if (NodeDOFs == 2)
            {
                // [2x1] = [4x2] * [2x1]
                local_body[0, 0] = 1;
                local_body[1, 1] = 1;
            }
            else // Node DOFs == 3
            {
                // [3x1] = [6x3] * [3x1]
                local_body[0, 0] = 1;
                local_body[1, 1] = 1;
                local_body[2, 2] = 1;
                local_body[0, 3] = 1;
                local_body[1, 4] = 1;
                local_body[2, 5] = 1;
            }

            // [Fb] = 0.5 * A * L * [body_local] * [body force]
            return (DenseVector)(0.5 * _Area * Length(nodeCoordinates) * local_body.Multiply(BodyForce));
        }

        /// <summary>
        /// Gets the element traction force matrix
        /// </summary>
        /// <param name="nodeCoordinates">Node coordinates, starting with element node 1</param>
        /// <returns></returns>
        public DenseVector TractionForceMatrix(List<double[]> nodeCoordinates)
        {
            ValidateLength(nodeCoordinates, NumOfNodes, MethodBase.GetCurrentMethod()?.Name);

            var local_traction = new DenseMatrix(ElementDOFs, NodeDOFs);
            if (NodeDOFs == 1)
            {
                // [1x1] = [2x1] * [1x1]
                local_traction[0, 0] = 1;
                local_traction[1, 0] = 1;
            }
            else if (NodeDOFs == 2)
            {
                // [2x1] = [4x2] * [2x1]
                local_traction[0, 0] = 1;
                local_traction[1, 1] = 1;
            }
            else // Node DOFs == 3
            {
                // [3x1] = [6x3] * [3x1]
                local_traction[0, 0] = 1;
                local_traction[1, 1] = 1;
                local_traction[2, 2] = 1;
                local_traction[0, 3] = 1;
                local_traction[1, 4] = 1;
                local_traction[2, 5] = 1;
            }

            // [Ft] = 0.5 * L * [body_traction] * [traction force]
            return (DenseVector)(0.5 * Length(nodeCoordinates) * local_traction.Multiply(TractionForce));
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
            double n1 = (1 - eta) / 2.0;
            double n2 = (1 + eta) / 2.0;

            // u = Nq - size based node DOFs
            var n = new DenseMatrix(NodeDOFs, ElementDOFs); 

            if (NodeDOFs == 1)
            {
                // u[1x1] = N[1x2] * q[2x1]
                n[0, 0] = n1;
                n[0, 1] = n2;
            }
            else if (NodeDOFs == 2)
            {
                // u[2x1] = N[2x4] * q[4x1]
                n[0, 0] = n1;
                n[1, 1] = n1;
                n[0, 2] = n2;
                n[1, 3] = n2;
            }
            else // Node DOFs == 3
            {
                // u[3x1] = N[3x6] * q[6x1]
                n[0, 0] = n1;
                n[1, 1] = n1;
                n[2, 2] = n1;
                n[0, 3] = n2;
                n[1, 4] = n2;
                n[2, 5] = n2;
            }

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
