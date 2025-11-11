using MathNet.Numerics.LinearAlgebra.Double;
using System.Reflection;

namespace FEA_Program.Models
{
    /// <summary>
    /// A linear beam element that works pure bending (no axial forces). Supports 1 Dimension only (2 DOF nodes)
    /// </summary>
    internal class ElementBeamLinear : Element, IElement
    {
        /// <summary>
        /// The area moment of inertia of the element in program units [m^4]
        /// </summary>
        private double _inertia = 0;
        
        /// <summary>
        /// Type identifier for the element
        /// </summary>
        public ElementTypes ElementType => ElementTypes.BeamLinear;

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
        public override double MaxStress => throw new NotImplementedException(); // TODO: This requires numerical iteration to find the max

        /// <summary>
        /// Get arguments that may vary between different element types
        /// </summary>
        public double[] ElementArgs { get => [_inertia]; set => _inertia = value[0]; }

        // ---------------- Public methods ----------------

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <param name="nodes">All nodes in the element</param>
        /// <param name="material">The element's material</param>
        /// <exception cref="ArgumentException">Raised if a parameter is invalid</exception>
        public ElementBeamLinear(int id, List<INode> nodes, Material material) : base(id, nodes, material, Node.NumberOfDOFs(Dimensions.One, true))
        {   
            // Only support 1D problems for now
            Dimension = Dimensions.One;
            
            if (Dimension != Dimensions.One)
                throw new ArgumentException($"Cannot create {ElementType} element with {NodeDOFs} dimensions. Unsupported");

            BodyForce = new DenseVector((int)Dimension);
            TractionForce = new DenseVector((int)Dimension);
        }

        /// <summary>
        /// Sets the element body force
        /// </summary>
        /// <param name="forcePerVol">The body force matrix [Element Dimension x 1]</param>
        public void SetBodyForce(DenseVector forcePerVol)
        {
            ValidateLength(forcePerVol.Values, Dimension, MethodBase.GetCurrentMethod()?.Name);
            BodyForce = forcePerVol;
            InvalidateSolution();
        }

        /// <summary>
        /// Sets the element traction force
        /// </summary>
        /// <param name="forcePerLength">The traction force matrix [Element Dimension x 1]</param>
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
            // This only works for the 1D case
            return Math.Abs(Nodes[1].Coordinates[0] - Nodes[0].Coordinates[0]);
        }

        /// <summary>
        /// Gets the element body force matrix
        /// </summary>
        /// <returns></returns>
        public DenseVector BodyForceMatrix()
        {
            var local_body = new DenseMatrix(ElementDOFs, (int)Dimension);
            var l = Length();

            if (Dimension == Dimensions.One)
            {
                // [4x1] = [4x1] * [1x1]
                local_body[0, 0] = 1 / 2.0;
                local_body[1, 0] = l / 12.0;
                local_body[2, 0] = 1 / 2.0;
                local_body[3, 0] = -l / 12.0;
            }
            else
            {
                throw new NotImplementedException();
            }


            // [Fb] = L * [body_local] * [body force]
            return (DenseVector)(l * local_body.Multiply(BodyForce));
        }

        /// <summary>
        /// Gets the element traction force matrix
        /// </summary>
        /// <returns></returns>
        public DenseVector TractionForceMatrix()
        {
            // Traction force typically isn't implemented for beam elements
            throw new NotImplementedException();
        }

        // ---------------- Base override methods ----------------

        /// <summary>
        /// Gets the element stiffness matrix
        /// </summary>
        /// <returns></returns>
        public override DenseMatrix K_Matrix()
        {
            var l = Length();

            // The K matrix for the 1D case
            var K_base = new DenseMatrix(4, 4);
            K_base[0, 0] = 12;
            K_base[0, 1] = 6 * l;
            K_base[0, 2] = -12;
            K_base[0, 3] = 6 * l;

            K_base[1, 0] = 6 * l;
            K_base[1, 1] = 4 * l * l;
            K_base[1, 2] = -6 * l;
            K_base[1, 3] = 2 * l * l;

            K_base[2, 0] = -12;
            K_base[2, 1] = -6 * l;
            K_base[2, 2] = 12;
            K_base[2, 3] = -6 * l;

            K_base[3, 0] = 6 * l;
            K_base[3, 1] = 2 * l * l;
            K_base[3, 2] = -6 * l;
            K_base[3, 3] = 4 * l * l;

            var E = D_Matrix()[0, 0];

            // K = Integral { [B]^T * [D] * [B] } dv = EI/L^3 * K_base
            return E * _inertia / (l*l*l) * K_base;
        }

        /// <summary>
        /// Sorts the nodes for the correct ordering in the element
        /// </summary>
        /// <param name="nodes">The list to sort in place</param>
        protected override void SortNodeOrder(ref List<INode> nodes)
        {
            //Order from smallest to largest X coordinate
            nodes = nodes.OrderBy(n => n.Coordinates[0]).ToList();
        }

        /// <summary>
        /// Gets the element's volume. Note: Not implemented
        /// </summary>
        protected override double Volume()
        {
            // Don't implement volume for a beam element - it's not used in the K matrix calculation
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the element strain / displacement matrix
        /// </summary>
        /// <returns></returns>
        protected override DenseMatrix B_Matrix(double[]? localCoords)
        {
            // This type of element requires local coordinates for the B matrix
            ArgumentNullException.ThrowIfNull(localCoords);

            ValidateLength(localCoords, LocalDimension, MethodBase.GetCurrentMethod()?.Name);
            double eta = localCoords[0];
            double l = Length();

            // B is based on the derivative of N -> dN/de
            var B = new DenseMatrix((int)Dimension, ElementDOFs);

            // Element Dimension 1
            // B = [1 x 4]
            B[0, 0] = (-6 + 12 * eta) / (l * l);
            B[0, 1] = (-4 + 6 * eta) / l;
            B[0, 2] = (6 - 12 * eta) / (l * l);
            B[0, 3] = (-2 + 6 * eta) / l;

            return B;
        }

        /// <summary>
        /// Gets the element shape function (interpolation) matrix
        /// </summary>
        /// <param name="localCoords">Local coordinates inside the element in the range of (0,1)</param>
        /// <returns></returns>
        protected override DenseMatrix N_Matrix(double[] localCoords)
        {
            ValidateLength(localCoords, LocalDimension, MethodBase.GetCurrentMethod()?.Name);
            double eta = localCoords[0];
            double n1 = 0.25 * (1 - eta) * (1 - eta) * (2 + eta);
            double n2 = 0.25 * (1 - eta) * (1 - eta) * (1 + eta) * Length() / 2.0;
            double n3 = 0.25 * (1 + eta) * (1 + eta) * (2 - eta);
            double n4 = 0.25 * (1 + eta) * (1 + eta) * (eta - 1) * Length() / 2.0;

            // u = Nq - size based element dimension
            var n = new DenseMatrix((int)Dimension, ElementDOFs);

            // Element Dimension 1
            // u[1x1] = N[1x4] * q[4x1]
            n[0, 0] = n1;
            n[0, 1] = n2;
            n[0, 2] = n3;
            n[0, 3] = n4;
    
            return n;
        }

        // ---------------- Private helpers ----------------

    }
}
