using System.Reflection;


namespace FEA_Program.Models
{
    /// <summary>
    /// A node in an FEA problem with arbitrary degrees of freedom
    /// </summary>
    internal class Node : IDClass, INode, ICloneable
    {
        /// <summary>
        /// Position of the node center in program units (m). Length depends on Dimension.
        /// </summary>
        private double[] _Position;

        /// <summary>
        /// Whether each DOF of the node is fixed. 0 = floating, 1 = fixed. Length depends on Dimension.
        /// </summary>
        private int[] _Fixity;

        /// <summary>
        /// Whether each rotational DOF of the node is fixed. 0 = floating, 1 = fixed. Length depends on Dimension.
        /// </summary>
        private int[] _RotationFixity = [];

        /// <summary>
        /// The node force in program units [N]. Length depends on Dimension.
        /// </summary>
        private double[] _Force;

        /// <summary>
        /// The node moment in program units [Nm]. Length depends on Dimension.
        /// </summary>
        private double[] _Moment = [];

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when the solve state changes from true to false
        /// </summary>
        public event EventHandler<int>? SolutionInvalidated;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The dimension of the node in global problem space.
        /// </summary>
        public Dimensions Dimension { get; private set; }

        /// <summary>
        /// True if the node includes rotary DOFs and moments
        /// </summary>
        public bool HasRotation { get; private set; }

        /// <summary>
        /// Number of DOFs in the node
        /// </summary>
        public int DOFs => NumberOfDOFs(Dimension, HasRotation);

        /// <summary>
        /// True if the solution for the node is current
        /// </summary>
        public bool SolutionValid { get; private set; } = false;

        /// <summary>
        /// Coordinates of the node center in program units (m). Length depends on DOFs.
        /// </summary>
        public double[] Position
        {
            get { return _Position; }
            set
            {
                ValidateDimensions(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Position = value;
                InvalidateSolution();
            }
        }

        /// <summary>
        /// Whether each dimension of the node is fixed. 0 = floating, 1 = fixed. Length depends on DOFs.
        /// </summary>
        public int[] Fixity
        {
            get { return _Fixity; }
            set
            {
                ValidateDimensions(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Fixity = value;
                InvalidateSolution();
            }
        }

        /// <summary>
        /// Whether each rotary dimension of the node is fixed. 0 = floating, 1 = fixed. Length depends on DOFs.
        /// </summary>
        public int[] RotationFixity
        {
            get { return _RotationFixity; }
            set
            {
                // Allow setting the value to blank without throwing an error
                if (!HasRotation && value.Length == 0)
                    return;

                ValidateRotation(MethodBase.GetCurrentMethod()?.Name ?? "");
                ValidateDimensions(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _RotationFixity = value;
                InvalidateSolution();
            }
        }

        /// <summary>
        /// Get the fixity vector with correct ordering for solving the FEA problem
        /// </summary>
        public int[] FixityVector => HasRotation ? InterleaveArrays(_Fixity, _RotationFixity) : _Fixity;

        /// <summary>
        /// The node force in program units [N]. Length depends on dimension.
        /// </summary>
        public double[] Force
        {
            get { return _Force; }
            set
            {
                ValidateDimensions(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Force = value;
                InvalidateSolution();
            }
        }

        /// <summary>
        /// The node moment in program units [Nm]. Length depends on dimension.
        /// </summary>
        public double[] Moment
        {
            get { return _Moment; }
            set
            {
                // Allow setting the value to blank without throwing an error
                if (!HasRotation && value.Length == 0)
                    return;
                
                ValidateRotation(MethodBase.GetCurrentMethod()?.Name ?? "");
                ValidateDimensions(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Moment = value;
                InvalidateSolution();
            }
        }

        /// <summary>
        /// Get the force vector with correct ordering for solving the FEA problem
        /// </summary>
        public double[] ForceVector => HasRotation ? InterleaveArrays(_Force, _Moment) : _Force;

        /// <summary>
        /// Displacement of the node center in program units (m). Length depends on Dimension.
        /// </summary>
        public double[] Displacement { get; private set; }

        /// <summary>
        /// Displacement of the node center in program units (rad). Length depends on Dimension.
        /// </summary>
        public double[] AngularDisplacement { get; private set; } = [];

        /// <summary>
        /// The node reaction force in program units [N]. Length depends on Dimension.
        /// </summary>
        public double[] ReactionForce { get; private set; }

        /// <summary>
        /// The node reaction moment in program units [Nm]. Length depends on Dimension.
        /// </summary>
        public double[] ReactionMoment { get; private set; } = [];

        // ---------------------- Public Methods ----------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id">The idenfier for the class</param>
        /// <param name="dimension">The dimension of the node</param>
        /// <param name="hasRotation">Whether the node supports rotation</param>
        public Node(int id, Dimensions dimension, bool hasRotation = false) : base(id)
        {
            Dimension = dimension;
            HasRotation = hasRotation;

            _Position = new double[(int)dimension];
            _Fixity = new int[(int)dimension];
            _Force = new double[(int)dimension];
            Displacement = new double[(int)dimension];
            ReactionForce = new double[(int)dimension];

            // These depend on the whether the element supports rotation
            if (HasRotation)
            {
                _RotationFixity = new int[(int)dimension];
                _Moment = new double[(int)dimension];
                AngularDisplacement = new double[(int)dimension];
                ReactionMoment = new double[(int)dimension];
            }
        }

        /// <summary>
        /// Sets the node result fields and marks it as solved
        /// </summary>
        /// <param name="displacementVector">Displacement of the node center in program units (m). Length depends on DOFs.</param>
        /// <param name="reactionVector">The node reaction force + moment in program units. First 3 items  = force [N], last 3 = moments [Nm]. Length depends on DOFs.</param>
        public void Solve(double[] displacementVector, double[] reactionVector)
        {
            // These vectors will have length of the DOFs, not the dimension
            ValidateDOFs(displacementVector, MethodBase.GetCurrentMethod()?.Name ?? "");
            ValidateDOFs(reactionVector, MethodBase.GetCurrentMethod()?.Name ?? "");

            if (HasRotation)
            {
                // Decompose the vectors into linear and rotation
                (Displacement, AngularDisplacement) = Deinterleave(displacementVector);
                (ReactionForce, ReactionMoment) = Deinterleave(reactionVector);
            }
            else
            {
                Displacement = displacementVector;
                ReactionForce = reactionVector;
            }

            SolutionValid = true;
        }

        /// <summary>
        /// Clone the class
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var output = new Node(ID, Dimension, HasRotation)
            {
                Position = (double[])Position.Clone(),
                Fixity = (int[])Fixity.Clone(),
                RotationFixity = (int[])RotationFixity.Clone(),
                Force = (double[])Force.Clone(),
                Moment = (double[])Moment.Clone(),
            };

            double[] displacementVector = HasRotation ? InterleaveArrays(Displacement, AngularDisplacement) : Displacement;
            double[] reactionVector = HasRotation ? InterleaveArrays(ReactionForce, ReactionMoment) : ReactionForce;

            output.Solve(displacementVector, reactionVector);

            if(!SolutionValid)
            {
                // This will invalidate the solution
                output.Position = (double[])Position.Clone();
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
            _Position = (double[])other.Position.Clone();
            _Fixity = (int[])other.Fixity.Clone();
            _RotationFixity = (int[])other.RotationFixity.Clone();
            _Force = (double[])other.Force.Clone();
            _Moment = (double[])other.Moment.Clone();
            OnPropertyChanged(nameof(Position));
            OnPropertyChanged(nameof(Fixity));
            OnPropertyChanged(nameof(Force));

            if (other.HasRotation)
            {
                OnPropertyChanged(nameof(RotationFixity));
                OnPropertyChanged(nameof(Moment));
            }

            // Need to alert parents the state has changed
            if (SolutionValid && !other.SolutionValid)
                SolutionInvalidated?.Invoke(this, ID);

            Dimension = other.Dimension;
            HasRotation = other.HasRotation;
            SolutionValid = other.SolutionValid;
            Displacement = (double[])other.Displacement.Clone();
            AngularDisplacement = (double[])other.AngularDisplacement.Clone();
            ReactionForce = (double[])other.ReactionForce.Clone();
            ReactionMoment = (double[])other.ReactionMoment.Clone();
        }

        // ---------------------- Private Helpers ----------------------

        /// <summary>
        /// Changes the solved state from true to false, indicating the result is no longer valid
        /// </summary>
        private void InvalidateSolution()
        {
            // Only raise the event if we're switching from true to false
            bool solutionWasvalid = SolutionValid; 
            SolutionValid = false;

            if (solutionWasvalid)
                SolutionInvalidated?.Invoke(this, ID);
        }

        /// <summary>
        /// Checks the length of a collection is correct for the number of DOFs in the node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection to check</param>
        /// <param name="methodName">Name of the calling method</param>
        /// <exception cref="ArgumentOutOfRangeException">The length was incorrect</exception>
        private void ValidateDOFs<T>(IReadOnlyCollection<T> collection, string methodName)
        {
            if (collection.Count != DOFs)
            {
                throw new ArgumentOutOfRangeException($"Attempted to execute operation <{methodName}> for node ID <{ID}> with input params having different DOFs than specified node DOFs.");
            }
        }

        /// <summary>
        /// Checks the length of a collection is correct for the dimension of the node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection to check</param>
        /// <param name="methodName">Name of the calling method</param>
        /// <exception cref="ArgumentOutOfRangeException">The length was incorrect</exception>
        private void ValidateDimensions<T>(IReadOnlyCollection<T> collection, string methodName)
        {
            if (collection.Count != (int)Dimension)
            {
                throw new ArgumentOutOfRangeException($"Attempted to execute operation <{methodName}> for node ID <{ID}> with input params having different length than the node dimension.");
            }
        }

        /// <summary>
        /// Checks whether the element supports rotation
        /// </summary>
        /// <param name="methodName">The calling method name</param>
        /// <exception cref="InvalidOperationException">Thrown if rotation isn't supported</exception>
        private void ValidateRotation(string methodName)
        {
            if (!HasRotation)
                throw new InvalidOperationException($"Cannot set parameter {methodName}. Node does not support rotation.");
        }

        /// <summary>
        /// Combines two arrays of the same length and type by interleaving their elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the arrays</typeparam>
        /// <param name="array1">The first array.</param>
        /// <param name="array2">The second array.</param>
        /// <returns>A new array containing the interleaved elements.</returns>
        /// <exception cref="ArgumentException">Thrown if the arrays are null or have different lengths.</exception>
        public static T[] InterleaveArrays<T>(T[] array1, T[] array2)
        {
            // Input validation
            if (array1 == null || array2 == null)
            {
                throw new ArgumentException("Both arrays must be non-null.");
            }
            if (array1.Length != array2.Length)
            {
                throw new ArgumentException("Both arrays must have the same length.");
            }

            int length = array1.Length;
            T[] interleavedArray = new T[length * 2];

            for (int i = 0; i < length; i++)
            {
                interleavedArray[2 * i] = array1[i];
                interleavedArray[(2 * i) + 1] = array2[i];
            }

            return interleavedArray;
        }

        /// <summary>
        /// Decomposes a single interleaved array back into two separate arrays.
        /// Array 1 contains elements from the even indices (0, 2, 4, ...).
        /// Array 2 contains elements from the odd indices (1, 3, 5, ...).
        /// </summary>
        /// <typeparam name="T">The type of the elements in the array.</typeparam>
        /// <param name="interleavedArray">The single array to be split.</param>
        /// <returns>A ValueTuple containing the two decomposed arrays (array1, array2).</returns>
        /// <exception cref="ArgumentException">Thrown if the array is null or has an odd length.</exception>
        public static (T[] array1, T[] array2) Deinterleave<T>(T[] interleavedArray)
        {
            if (interleavedArray == null)
            {
                throw new ArgumentException("Input array must be non-null.");
            }
            // An interleaved array must have an even length.
            if (interleavedArray.Length % 2 != 0)
            {
                throw new ArgumentException("Input array must have an even number of elements to be perfectly deinterleaved.");
            }

            int halfLength = interleavedArray.Length / 2;
            T[] array1 = new T[halfLength]; // Elements from even indices
            T[] array2 = new T[halfLength]; // Elements from odd indices

            for (int i = 0; i < halfLength; i++)
            {
                array1[i] = interleavedArray[2 * i];
                array2[i] = interleavedArray[2 * i + 1];
            }

            return (array1, array2);
        }

        // ---------------------- Static Methods ----------------------

        /// <summary>
        /// Gets an empty node for reference use
        /// </summary>
        /// <returns></returns>
        public static Node DummyNode(Dimensions dimension = Dimensions.One) => new(InvalidID, dimension);

        /// <summary>
        /// Get the number of node DOFs for a given configuration
        /// </summary>
        /// <param name="dimension">The problem dimension</param>
        /// <param name="hasRotation">Whether the node has rotation</param>
        /// <returns>The number of DOFs a node should have</returns>
        public static int NumberOfDOFs(Dimensions dimension, bool hasRotation)
        {
            return hasRotation ? 2 * (int)dimension : (int)dimension;
        }
    }
}
