using System.Reflection;
using MathNet.Numerics.LinearAlgebra.Double;


namespace FEA_Program.Models
{
    /// <summary>
    /// A node in an FEA problem with arbitrary degrees of freedom
    /// </summary>
    internal class Node : IDClass, INode, ICloneable
    {
        /// <summary>
        /// Provides a list of available dimsensions for error checking
        /// </summary>
        public static int[] ValidDimensions = [1, 2, 3, 6];

        private double[] _Coordinates; // coordinates in m
        private int[] _Fixity; // 0 = floating, 1 = fixed
        private double[] _Force; // first 3 items  = force [N], last 3 = moments [Nm]

        public event EventHandler<int>? SolutionInvalidated;

        /// <summary>
        /// The node dimension 1 = 1D, 2 = 2D, 3 = 3D, 6 = 6D
        /// </summary>
        public int Dimension { get; private set; }
        public bool SolutionValid { get; private set; } = false;

        public double[] Coordinates
        {
            get { return _Coordinates; }
            set
            {
                ValidateDimension<double>(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Coordinates = value;
                InvalidateSolution();
            }
        }
        public int[] Fixity
        {
            get { return _Fixity; }
            set
            {
                ValidateDimension<int>(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Fixity = value;
                InvalidateSolution();
            }
        }
        public double[] Force
        {
            get { return _Force; }
            set
            {
                ValidateDimension<double>(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Force = value;
                InvalidateSolution();
            }
        } // First 3 items  = force [N], last 3 = moments [Nm]
        public double ForceMagnitude
        {
            get
            {
                // Dimension == 1
                DenseVector output = new([_Force[0], 0, 0]);

                if (Dimension == 2)
                {
                    output[1] = _Force[1];
                }
                else if (Dimension == 3 || Dimension == 6)
                {
                    output[2] = _Force[2];
                }

                // Calculate the L2-norm (magnitude)
                return output.L2Norm();
            }
        } // will eventually need moment functions too
        public double[] ForceDirection
        {
            get
            {
                if (Dimension == 1)
                {
                    return [.. new DenseVector([_Force[0]]).Normalize(1)];
                }
                else if (Dimension == 2)
                {
                    return [.. new DenseVector([_Force[0], _Force[1]]).Normalize(1)];
                }
                else // dimensions = 3 or 6
                {
                    return [.. new DenseVector([_Force[0], _Force[1], _Force[2]]).Normalize(1)];
                }
            }
        }

        public double[] Displacement { get; private set; } 
        public double[] ReactionForce { get; private set; } // reaction force in [N], reaction moments in [Nm]
        public double[] FinalPos => _Coordinates.Zip(Displacement, (coord, disp) => coord + disp).ToArray();


        public Node(double[] coords, int[] fixity, int id, int dimension) : base(id)
        {
            Dimension = dimension;

            if (!ValidDimensions.Contains(dimension))
            {
                throw new Exception($"Attempted to create element, ID <{id}> with invalid number of dimensions: {dimension}.");
            }

            ValidateDimension(coords, MethodBase.GetCurrentMethod()?.Name ?? "");
            ValidateDimension(fixity, MethodBase.GetCurrentMethod()?.Name ?? "");

            _Coordinates = coords;
            _Fixity = fixity;
            _Force = new double[dimension];
            Displacement = new double[dimension];
            ReactionForce = new double[dimension];
        }
        public void Solve(double[] displacement, double[] reactionForce)
        {
            ValidateDimension(displacement, MethodBase.GetCurrentMethod()?.Name ?? "");
            ValidateDimension(reactionForce, MethodBase.GetCurrentMethod()?.Name ?? "");

            Displacement = displacement;
            ReactionForce = reactionForce;
            SolutionValid = true;
        }

        /// <summary>
        /// Clone the class
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var output = new Node((double[])Coordinates.Clone(), (int[])Fixity.Clone(), ID, Dimension)
            {
                Force = (double[])Force.Clone(),
            };

            output.Solve((double[])Displacement.Clone(), (double[])ReactionForce.Clone());

            if(!SolutionValid)
            {
                // This will invalidate the solution
                output.Coordinates = (double[])Coordinates.Clone();
            }

            return output;
        }

        /// <summary>
        /// Import parameters from another node, while retaining ID.
        /// </summary>
        /// <param name="other"></param>
        public void ImportParameters(Node other)
        {
            // Set these underlying so SolutionInvalidate event doesn't get called
            _Coordinates = (double[])other.Coordinates.Clone();
            _Fixity = (int[])other.Fixity.Clone();
            _Force = (double[])other.Force.Clone();
            OnPropertyChanged(nameof(Coordinates));
            OnPropertyChanged(nameof(Fixity));
            OnPropertyChanged(nameof(Force));

            Dimension = other.Dimension;
            SolutionValid = other.SolutionValid;
            Displacement = (double[])other.Displacement.Clone();
            ReactionForce = (double[])other.ReactionForce.Clone();
        }


        private void InvalidateSolution()
        {
            // Only raise the event if we're switching from true to false
            bool solutionWasvalid = SolutionValid; 
            SolutionValid = false;

            if (solutionWasvalid)
                SolutionInvalidated?.Invoke(this, ID);
        }
        private void ValidateDimension<T>(IReadOnlyCollection<T> collection, string methodName)
        {
            if (collection.Count != Dimension)
            {
                throw new ArgumentOutOfRangeException($"Attempted to execute operation <{methodName}> for node ID <{ID}> with input params having different dimensions than specified node dimension.");
            }
        }

        /// <summary>
        /// Gets an empty node for reference use
        /// </summary>
        /// <returns></returns>
        public static Node DummyNode(int dimension = 1) => new(new double[dimension], new int[dimension], Constants.InvalidID, dimension);
    }
}
