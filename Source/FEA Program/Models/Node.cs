using System.Reflection;
using OpenTK.Mathematics;


namespace FEA_Program.Models
{
    internal class Node
    {
        /// <summary>
        /// Provides a list of available dimsensions for error checking
        /// </summary>
        public static int[] ValidDimensions = [1, 2, 3, 6];

        private double[] _Coords; // coordinates in m
        private int[] _Fixity; // 0 = floating, 1 = fixed
        private double[] _Force; // first 3 items  = force [N], last 3 = moments [Nm]

        public event EventHandler<int>? SolutionInvalidated;

        /// <summary>
        /// The node dimension 1 = 1D, 2 = 2D, 3 = 3D, 6 = 6D
        /// </summary>
        public int Dimension { get; private set; }  // first 3 items  = force [N], last 3 = moments [Nm]
        public int ID { get; private set; }
        public bool SolutionValid { get; private set; } = false;

        public double[] Coords_mm => _Coords.Select(coord => coord * 1000.0d).ToArray();
        public double[] Coords
        {
            get { return _Coords; }
            set
            {
                ValidateDimension<double>(value, MethodBase.GetCurrentMethod()?.Name ?? "");
                _Coords = value;
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
        }
        public double ForceMagnitude
        {
            get
            {
                Vector3 output;

                if (Dimension == 1)
                {
                    output = new Vector3((float)_Force[0], 0, 0);
                }
                else if (Dimension == 2)
                {
                    output = new Vector3((float)_Force[0], (float)_Force[1], 0);
                }
                else // dimensions = 3 or 6
                {
                    output = new Vector3((float)_Force[0], (float)_Force[1], (float)_Force[2]);
                }

                return output.Length;
            }
        } // will eventually need moment functions too
        public double[] ForceDirection
        {
            get
            {
                Vector3 output;

                if (Dimension == 1)
                {
                    output = new Vector3((float)_Force[0], 0, 0);
                    output.Normalize();
                    return [(double)output.X];
                }
                else if (Dimension == 2)
                {
                    output = new Vector3((float)_Force[0], (float)_Force[1], 0);
                    output.Normalize();
                    return [(double)output.X, (double)output.Y];
                }
                else // dimensions = 3 or 6
                {
                    output = new Vector3((float)_Force[0], (float)_Force[1], (float)_Force[2]);
                    output.Normalize();
                    return [(double)output.X, (double)output.Y, (double)output.Z];

                }
            }
        }
        public double[] Displacement { get; private set; } // first 3 items  = force [N], last 3 = moments [Nm]
        public double[] ReactionForce { get; private set; } // reaction force in [N], reaction moments in [Nm]
        public double[] FinalPos => _Coords.Zip(Displacement, (coord, disp) => coord + disp).ToArray();


        public Node(double[] coords, int[] fixity, int id, int dimension)
        {
            ID = id;
            Dimension = dimension;

            if (!ValidDimensions.Contains(dimension))
            {
                throw new Exception($"Attempted to create element, ID <{id}> with invalid number of dimensions: {dimension}.");
            }

            ValidateDimension<double>(coords, MethodBase.GetCurrentMethod()?.Name ?? "");
            ValidateDimension<int>(fixity, MethodBase.GetCurrentMethod()?.Name ?? "");

            _Coords = coords;
            _Fixity = fixity;
            Displacement = new double[dimension];
            ReactionForce = new double[dimension];
        }
        public void Solve(double[] displacement, double[] reactionForce)
        {
            if (displacement.Length != Dimension | displacement.Length != Dimension)
            {
                throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + ID.ToString() + "> with input params having different dimensions than specified node dimension.");
            }

            Displacement = displacement;
            ReactionForce = reactionForce;
            SolutionValid = true;
        }
        public double[] GetScaledDisplacement_mm(double percentage, double scaleFactor)
        {
            var output = new double[Dimension + 1];

            for (int i = 0, loopTo = _Coords.Length - 1; i <= loopTo; i++)
                output[i] = (Coords[i] + Displacement[i] * percentage * scaleFactor) * 1000.0d; // convert to mm

            return output;
        }


        private void InvalidateSolution()
        {
            SolutionValid = false;
            SolutionInvalidated?.Invoke(this, ID);
        }
        private void ValidateDimension<T>(IReadOnlyCollection<T> collection, string methodName)
        {
            if (collection.Count != Dimension)
            {
                throw new ArgumentOutOfRangeException($"Attempted to execute operation <{methodName}> for node ID <{ID}> with input params having different dimensions than specified node dimension.");
            }
        }
    }
}
