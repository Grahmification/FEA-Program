using MathNet.Numerics.LinearAlgebra.Double;
using System.Reflection;

namespace FEA_Program.Models
{
    /// <summary>
    /// A linear truss element which can be used with 1D, 2D, or 3D nodes
    /// </summary>
    internal class ElementTrussLinear : Element, IElement
    {
        /// <summary>
        /// The element x-section area in program units (m^2)
        /// </summary>
        private double _Area = 0;

        /// <summary>
        /// Type identifier for the element
        /// </summary>
        public ElementTypes ElementType => ElementTypes.TrussLinear;

        /// <summary>
        /// The dimension of the element in global problem coordinates.
        /// </summary>
        public Dimensions Dimension { get; private set; }

        /// <summary>
        /// The dimension of local coordinates inside the element, indicating the size of localCoordinates arguments.
        /// </summary>
        public override Dimensions LocalDimension => Dimensions.One;

        /// <summary>
        /// The number of nodes in the element
        /// </summary>
        public override int NumOfNodes => 2;

        /// <summary>
        /// Element body force in program units N/m^3 [X, Y, Z]^T
        /// </summary>
        public DenseVector BodyForce { get; private set; }

        /// <summary>
        /// Element traction force in program units N/m [X, Y, Z]^T
        /// </summary>
        public DenseVector TractionForce { get; private set; }

        /// <summary>
        /// Get the max stress in the element
        /// </summary>
        public override double MaxStress => StressMatrix().Values[0]; // This element has constant stress

        /// <summary>
        /// Get arguments that may vary between different element types
        /// </summary>
        public double[] ElementArgs { get => [_Area]; set => _Area = value[0]; }

        // ---------------- Public methods ----------------

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="area">The element x-section area in program units (m^2)</param>
        /// <param name="id">Element ID</param>
        /// <param name="nodes">All nodes in the element</param>
        /// <param name="material">The element's material</param>
        /// <param name="dimension">The number of DOFs for nodes in the element</param>
        /// <exception cref="ArgumentException">Raised if a parameter is invalid</exception>
        public ElementTrussLinear(double area, int id, List<INode> nodes, Material material, Dimensions dimension = Dimensions.One) : base(id, nodes, material, Node.NumberOfDOFs(dimension, false))
        {
            Dimension = dimension;
            
            if (area <= 0)
                throw new ArgumentException($"Cannot create {ElementType} element with non-positive area.");

            if (Dimension == Dimensions.Invalid)
                throw new ArgumentException($"Cannot create {ElementType} element with {Dimension} dimension. Unsupported");

            _Area = area;
            BodyForce = new DenseVector((int)Dimension);
            TractionForce = new DenseVector((int)Dimension);
        }

        /// <summary>
        /// Sets the element body force
        /// </summary>
        /// <param name="forcePerVol">The body force matrix [Node DOF x 1]</param>
        public void SetBodyForce(DenseVector forcePerVol)
        {
            ValidateLength(forcePerVol.Values, Dimension, MethodBase.GetCurrentMethod()?.Name);
            BodyForce = forcePerVol;
            InvalidateSolution();
        }

        /// <summary>
        /// Sets the element traction force
        /// </summary>
        /// <param name="forcePerLength">The traction force matrix [Node DOF x 1]</param>
        public void SetTractionForce(DenseVector forcePerLength)
        {
            ValidateLength(forcePerLength.Values, Dimension, MethodBase.GetCurrentMethod()?.Name);
            TractionForce = forcePerLength;
            InvalidateSolution();
        }

        /// <summary>
        /// Gets the element length
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Geometry.Length(Nodes[1].Position, Nodes[0].Position);
        }

        /// <summary>
        /// Gets the element body force matrix
        /// </summary>
        /// <returns></returns>
        public DenseVector BodyForceMatrix()
        {
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
            return (DenseVector)(0.5 * _Area * Length() * local_body.Multiply(BodyForce));
        }

        /// <summary>
        /// Gets the element traction force matrix
        /// </summary>
        /// <returns></returns>
        public DenseVector TractionForceMatrix()
        {
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
            return (DenseVector)(0.5 * Length() * local_traction.Multiply(TractionForce));
        }

        // ---------------- Base override methods ----------------

        /// <summary>
        /// Sorts the nodes for the correct ordering in the element
        /// </summary>
        /// <param name="nodes">The list to sort in place</param>
        protected override void SortNodeOrder(ref List<INode> nodes)
        {
            //Order from smallest to largest X coordinate
            nodes = nodes.OrderBy(n => n.Position[0]).ToList();
        }

        /// <summary>
        /// Gets the element's volume
        /// </summary>
        protected override double Volume()
        {
            return Length() * _Area;
        }

        /// <summary>
        /// Gets the element strain / displacement matrix
        /// </summary>
        /// <returns></returns>
        protected override DenseMatrix B_Matrix(double[]? localCoords = null)
        {
            // Always B = [-1 1]*1/(x2-x1) for a 1D element
            var B_local = new DenseMatrix(1, 2); 
            B_local[0, 0] = -1.0;
            B_local[0, 1] = 1.0;
            
            // Convert to global coordinates and scale by the length to get the global B matrix
            return B_local * M_Matrix() * (1.0 / Length());
        }

        /// <summary>
        /// Gets the element shape function (interpolation) matrix
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element</param>
        /// <returns></returns>
        protected override DenseMatrix N_Matrix(double[] localCoords)
        {
            ValidateLength(localCoords, LocalDimension, MethodBase.GetCurrentMethod()?.Name);
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

        // ---------------- Private helpers ----------------

        /// <summary>
        /// Get the transformation matrix from global node coordinates to local node coordinates for higher DOF elements
        /// </summary>
        /// <returns></returns>
        private DenseMatrix M_Matrix()
        {
            if(NodeDOFs == 1)
            {
                return DenseMatrix.CreateIdentity(2); // Leaves B Matrix unchanged
            }
            else if (NodeDOFs == 2) 
            {
                (var l, var m, _) = Geometry.ComputeDirectionCosines(Nodes[1].Position, Nodes[0].Position);
                var matrix = new DenseMatrix(2, 4);
                matrix[0, 0] = l;
                matrix[0, 1] = m;
                matrix[1, 2] = l;
                matrix[1, 3] = m;
                return matrix;
            }
            else // Node DOFs == 3
            {
                (var l, var m, var n) = Geometry.ComputeDirectionCosines(Nodes[1].Position, Nodes[0].Position);
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
