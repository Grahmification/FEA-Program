using System.Reflection;


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
        public static int[] ValidDimensions = [1, 2, 3];

        /// <summary>
        /// Coordinates of the node center in program units (m). Length depends on DOFs.
        /// </summary>
        private double[] _Coordinates;

        /// <summary>
        /// Whether each dimension of the node is fixed. 0 = floating, 1 = fixed. Length depends on DOFs.
        /// </summary>
        private int[] _Fixity;

        /// <summary>
        /// The node force + moment in program units. First 3 items  = force [N], last 3 = moments [Nm]. Length depends on DOFs.
        /// </summary>
        private double[] _Force;

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when the solve state changes from true to false
        /// </summary>
        public event EventHandler<int>? SolutionInvalidated;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The dimension of the node in global problem space. 1 = 1D, 2 = 2D, 3 = 3D
        /// </summary>
        public int Dimension { get; private set; }

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
        public double[] Coordinates
        {
            get { return _Coordinates; }
            set
            {
                ValidateDOFs<double>(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Coordinates = value;
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
                ValidateDOFs<int>(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Fixity = value;
                InvalidateSolution();
            }
        }

        /// <summary>
        /// The node force + moment in program units. First 3 items  = force [N], last 3 = moments [Nm]. Length depends on DOFs.
        /// </summary>
        public double[] Force
        {
            get { return _Force; }
            set
            {
                ValidateDOFs<double>(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Force = value;
                InvalidateSolution();
            }
        }

        /// <summary>
        /// Displacement of the node center in program units (m). Length depends on DOFs.
        /// </summary>
        public double[] Displacement { get; private set; }

        /// <summary>
        /// The node reaction force + moment in program units. First 3 items  = force [N], last 3 = moments [Nm]. Length depends on DOFs.
        /// </summary>
        public double[] ReactionForce { get; private set; }

        // ---------------------- Public Methods ----------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id">The idenfier for the class</param>
        /// <param name="dofs">Number of DOFs in the node</param>
        public Node(int id, int dimension, bool hasRotation = false) : base(id)
        {
            Dimension = dimension;
            HasRotation = hasRotation;

            _Coordinates = new double[DOFs];
            _Fixity = new int[DOFs];
            _Force = new double[DOFs];
            Displacement = new double[DOFs];
            ReactionForce = new double[DOFs];
        }

        /// <summary>
        /// Constructor with more parameters
        /// </summary>
        /// <param name="coords">Coordinates of the node center in program units (m). Length depends on DOFs.</param>
        /// <param name="fixity">Whether each dimension of the node is fixed. 0 = floating, 1 = fixed. Length depends on DOFs.</param>
        /// <param name="id">The idenfier for the class</param>
        /// <param name="dofs">Number of DOFs in the node</param>
        /// <exception cref="Exception">A parameter was incorrect</exception>
        public Node(double[] coords, int[] fixity, int id, int dimension, bool hasRotation = false) : base(id)
        {
            Dimension = dimension;
            HasRotation = hasRotation;

            if (!ValidDimensions.Contains(dimension))
            {
                throw new Exception($"Attempted to create element, ID <{id}> with invalid number of dimensions: {dimension}.");
            }

            ValidateDOFs(coords, MethodBase.GetCurrentMethod()?.Name ?? "");
            ValidateDOFs(fixity, MethodBase.GetCurrentMethod()?.Name ?? "");

            _Coordinates = coords;
            _Fixity = fixity;
            _Force = new double[DOFs];
            Displacement = new double[DOFs];
            ReactionForce = new double[DOFs];
        }

        /// <summary>
        /// Sets the node result fields and marks it as solved
        /// </summary>
        /// <param name="displacement">Displacement of the node center in program units (m). Length depends on DOFs.</param>
        /// <param name="reactionForce">The node reaction force + moment in program units. First 3 items  = force [N], last 3 = moments [Nm]. Length depends on DOFs.</param>
        public void Solve(double[] displacement, double[] reactionForce)
        {
            ValidateDOFs(displacement, MethodBase.GetCurrentMethod()?.Name ?? "");
            ValidateDOFs(reactionForce, MethodBase.GetCurrentMethod()?.Name ?? "");

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
            var output = new Node((double[])Coordinates.Clone(), (int[])Fixity.Clone(), ID, Dimension, HasRotation)
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

            // Need to alert parents the state has changed
            if (SolutionValid && !other.SolutionValid)
                SolutionInvalidated?.Invoke(this, ID);

            Dimension = other.Dimension;
            HasRotation = other.HasRotation;
            SolutionValid = other.SolutionValid;
            Displacement = (double[])other.Displacement.Clone();
            ReactionForce = (double[])other.ReactionForce.Clone();
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
                throw new ArgumentOutOfRangeException($"Attempted to execute operation <{methodName}> for node ID <{ID}> with input params having different dimensions than specified node dimension.");
            }
        }

        // ---------------------- Static Methods ----------------------

        /// <summary>
        /// Gets an empty node for reference use
        /// </summary>
        /// <returns></returns>
        public static Node DummyNode(int dimension = 1) => new(InvalidID, dimension);

        /// <summary>
        /// Get the number of node DOFs for a given configuration
        /// </summary>
        /// <param name="dimension">The problem dimension</param>
        /// <param name="hasRotation">Whether the node has rotation</param>
        /// <returns>The number of DOFs a node should have</returns>
        public static int NumberOfDOFs(int dimension, bool hasRotation)
        {
            return hasRotation ? 2 * dimension : dimension;
        }
    }
}
