using System.Reflection;
using OpenTK.Mathematics;


namespace FEA_Program.Models
{
    internal class Node
    {

        // ---------------------- ALL MEMBERS HERE SHOWN FOR 6D NODE, MEMBERS ARE SHORTENED ACCORDINGLY BY DIMENSION
        private double[] _Coords = new[] { 0d, 0d, 0d, 0d, 0d, 0d }; // coordinates in m
        private double[] _Disp = new[] { 0d, 0d, 0d, 0d, 0d, 0d }; // displacement in m
        private int[] _Fixity = new[] { 0, 0, 0, 0, 0, 0 }; // 0 = floating, 1 = fixed

        private double[] _Force = new[] { 0d, 0d, 0d, 0d, 0d, 0d }; // first 3 items  = force [N], last 3 = moments [Nm]
        private double[] _ReactionForce = new[] { 0d, 0d, 0d, 0d, 0d, 0d }; // reaction force in [N], reaction moments in [Nm]

        private int[] _ValidDimensions = new[] { 1, 2, 3, 6 }; // provides a list of available dimsensions for error checking

        private bool _SolutionValid = false;

        public event SolutionInvalidatedEventHandler SolutionInvalidated;

        public delegate void SolutionInvalidatedEventHandler(int NodeID);


        public double[] Coords_mm
        {
            get
            {
                var output = new double[Dimension];

                for (int i = 0, loopTo = _Coords.Length - 1; i <= loopTo; i++)
                    output[i] = _Coords[i] * 1000.0d; // convert to mm

                return output;
            }
        }
        public double[] Coords
        {
            get
            {
                return _Coords;
            }
            set
            {
                if (value.Length != Dimension)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                }

                _Coords = value;
                InvalidateSolution();
            }
        }
        public double[] Force
        {
            get
            {
                return _Force;
            }
            set
            {
                if (value.Length != Dimension)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                }

                _Force = value;
                InvalidateSolution();
            }
        }
        public int[] Fixity
        {
            get
            {
                return _Fixity;
            }
            set
            {
                if (value.Length != Dimension)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                }

                _Fixity = value;
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
                    return new[] { (double)output.X };
                }

                else if (Dimension == 2)
                {
                    output = new Vector3((float)_Force[0], (float)_Force[1], 0);
                    output.Normalize();
                    return new[] { (double)output.X, (double)output.Y };
                }

                else // dimensions = 3 or 6
                {
                    output = new Vector3((float)_Force[0], (float)_Force[1], (float)_Force[2]);
                    output.Normalize();
                    return new[] { (double)output.X, (double)output.Y, (double)output.Z };

                }
            }
        }
        public double[] Displacement
        {
            get
            {
                return _Disp;
            }
        }
        public double[] ReactionForce
        {
            get
            {
                return _ReactionForce;
            }
        }
        public double[] FinalPos
        {
            get
            {
                var output = new double[Dimension + 1];

                for (int i = 0, loopTo = Dimension - 1; i <= loopTo; i++)
                    output[i] = _Coords[i] + _Disp[i]; // add disp to each coord

                return output;
            }
        }

        /// <summary>
        /// The node dimension 1 = 1D, 2 = 2D, 3 = 3D, 6 = 6D
        /// </summary>
        public int Dimension { get; private set; }
        public int ID { get; private set; }

        public Node(double[] coords, int[] fixity, int id, int dimension)
        {

            if (_ValidDimensions.Contains(dimension) == false)
            {
                throw new Exception("Attempted to create element, ID <" + id.ToString() + "> with invalid number of dimensions: " + dimension.ToString());
            }

            if (coords.Length != dimension | fixity.Length != dimension)
            {
                throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + ID.ToString() + "> with input params having different dimensions than specified node dimension.");
            }

            _Coords = coords;
            _Fixity = fixity;
            ID = id;
            Dimension = dimension;
        }
        public void Solve(double[] Disp, double[] R)
        {

            if (Disp.Length != Dimension | Disp.Length != Dimension)
            {
                throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + ID.ToString() + "> with input params having different dimensions than specified node dimension.");
            }

            _Disp = Disp;
            _ReactionForce = R;
            _SolutionValid = true;
        }
        public double[] CalcDispIncrementPos_mm(double Percentage, double ScaleFactor)
        {
            var output = new double[Dimension + 1];

            for (int i = 0, loopTo = _Coords.Length - 1; i <= loopTo; i++)
                output[i] = (_Coords[i] + _Disp[i] * Percentage * ScaleFactor) * 1000.0d; // convert to mm

            return output;
        }

        private void InvalidateSolution()
        {
            _SolutionValid = false;
            SolutionInvalidated?.Invoke(ID);
        }
    }
}
