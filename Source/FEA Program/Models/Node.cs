using System.Reflection;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

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

        private int _ID = -1;
        private int _Dimensions = 0; // 1 = 1D, 2 = 2D, 3 = 3D, 6 = 6D
        private int[] _ValidDimensions = new[] { 1, 2, 3, 6 }; // provides a list of available dimsensions for error checking

        private Color _DefaultColor = Color.Blue;
        private Color _DefaultForceColor = Color.Purple;
        private Color _DefaultFixityColor = Color.Red;
        private Color _FixityColor = Color.Red;
        private Color _Color = Color.Blue;
        private Color _ForceColor = Color.Purple;
        private Color _SelectedColor = Color.Yellow;
        private Color _ReactionColor = Color.Green;

        private bool _SolutionValid = false;

        public event SolutionInvalidatedEventHandler SolutionInvalidated;

        public delegate void SolutionInvalidatedEventHandler(int NodeID);

        public bool Selected
        {
            get
            {
                if (_Color == _SelectedColor)
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    _Color = _SelectedColor;
                    _ForceColor = _SelectedColor;
                    _FixityColor = _SelectedColor;
                }
                else if (_Color == _SelectedColor)
                {
                    _Color = _DefaultColor;
                    _ForceColor = _DefaultForceColor;
                    _FixityColor = _DefaultFixityColor;
                }
            }
        }
        public double[] Coords_mm
        {
            get
            {
                var output = new double[_Dimensions];

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
                if (value.Length != _Dimensions)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
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
                if (value.Length != _Dimensions)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
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
                if (value.Length != _Dimensions)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
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

                if (_Dimensions == 1)
                {
                    output = new Vector3((float)_Force[0], 0, 0);
                }

                else if (_Dimensions == 2)
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

                if (_Dimensions == 1)
                {
                    output = new Vector3((float)_Force[0], 0, 0);
                    output.Normalize();
                    return new[] { (double)output.X };
                }

                else if (_Dimensions == 2)
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
                var output = new double[_Dimensions + 1];

                for (int i = 0, loopTo = _Dimensions - 1; i <= loopTo; i++)
                    output[i] = _Coords[i] + _Disp[i]; // add disp to each coord

                return output;
            }
        }
        public int Dimension
        {
            get
            {
                return _Dimensions;
            }
        }
        public int ID
        {
            get
            {
                return _ID;
            }
        }

        public Node(double[] NewCoords, int[] NewFixity, int NewID, int Dimensions)
        {

            if (_ValidDimensions.Contains(Dimensions) == false)
            {
                throw new Exception("Attempted to create element, ID <" + NewID.ToString() + "> with invalid number of dimensions: " + Dimensions.ToString());
            }

            if (NewCoords.Length != Dimensions | NewFixity.Length != Dimensions)
            {
                throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
            }

            _Coords = NewCoords;
            _Fixity = NewFixity;
            _ID = NewID;
            _Dimensions = Dimensions;
        }
        public void Solve(double[] Disp, double[] R)
        {

            if (Disp.Length != _Dimensions | Disp.Length != _Dimensions)
            {
                throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
            }

            _Disp = Disp;
            _ReactionForce = R;
            _SolutionValid = true;
        }
        public void DrawNode(double[] N_mm)
        {

            if (N_mm.Length != _Dimensions)
            {
                throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
            }

            DrawNode(N_mm, _Color);
            DrawForce(N_mm, _Force, _ForceColor);
            DrawFixity(N_mm, _Fixity, _FixityColor);
        } // draw always has mm input
        public void DrawReaction(double[] N_mm)
        {

            if (N_mm.Length != _Dimensions)
            {
                throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
            }

            DrawForce(N_mm, _ReactionForce, _ReactionColor);
        } // draw always has mm input


        public double[] CalcDispIncrementPos_mm(double Percentage, double ScaleFactor)
        {
            var output = new double[_Dimensions + 1];

            for (int i = 0, loopTo = _Coords.Length - 1; i <= loopTo; i++)
                output[i] = (_Coords[i] + _Disp[i] * Percentage * ScaleFactor) * 1000.0d; // convert to mm

            return output;
        }


        // ---------- Draw is not yet setup for 6D nodes ---- need to be able to display rotation displacement

        private void DrawNode(double[] N, Color Color)
        {

            double[] tmp = null;

            if (_Dimensions == 1)
            {
                tmp = new[] { N[0], 0d, 0d };
            }

            else if (_Dimensions == 2)
            {
                tmp = new[] { N[0], N[1], 0d };
            }

            else // dimensions 3 or 6
            {
                tmp = N;
            }

            GL.Color3(Color);
            GL.Begin(PrimitiveType.Quads);

            if (_Dimensions == 1 | _Dimensions == 2)
            {
                GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2]);
                GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2]);
                GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2]);
                GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2]);
            }

            else
            {
                GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] - 1d);
                GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] - 1d);

                GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] - 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] - 1d);

                GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] + 1d);

                GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] - 1d);
                GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] - 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] - 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] - 1d);

                GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] - 1d);
                GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] - 1d);

                GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] + 1d);
                GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] - 1d);
                GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] - 1d);

            }
            GL.End();
        } // draw always has mm input
        private void DrawForce(double[] N, double[] F, Color Color)
        {
            if (Array.TrueForAll(F, ValEqualsZero) == false) // dont draw a force if its zero
            {

                double[] tmp = [];
                double forcelength = 10.0d;
                Vector3 vect = default;

                if (_Dimensions == 1)
                {
                    tmp = new[] { N[0], 0d, 0d };
                    vect = new Vector3((float)F[0], 0, 0);
                }

                else if (_Dimensions == 2)
                {
                    tmp = new[] { N[0], N[1], 0d };
                    vect = new Vector3((float)F[0], (float)F[1], 0);
                }

                else // dimensions 3 or 6
                {
                    tmp = N;
                    vect = new Vector3((float)F[0], (float)F[1], (float)F[2]);
                }

                vect.Normalize();

                // ------------------- create normal vectors to use for drawing arrows -----------------
                var X = new Vector3(1, 0, 0);
                Vector3 normal1 = Vector3.Cross(vect, X);
                normal1 = Vector3.Multiply(normal1, (float)(forcelength * 0.2));

                var Z = new Vector3(0, 0, 1);
                Vector3 normal2 = Vector3.Cross(vect, Z);
                normal2 = Vector3.Multiply(normal2, (float)(forcelength * 0.2d));

                // ---------------- set arrow to proper length after calculating normals ---------
                vect = Vector3.Multiply(vect, (float)(forcelength));

                GL.Color3(Color);
                GL.Begin(PrimitiveType.Lines);

                GL.Vertex3(tmp[0], tmp[1], tmp[2]);
                GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);

                GL.End();

                GL.Begin(PrimitiveType.Triangles);
                Vector3 endPt = Vector3.Multiply(vect, 0.8f);
                GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);
                GL.Vertex3(tmp[0] + endPt.X + normal1.X, tmp[1] + endPt.Y + normal1.Y, tmp[2] + endPt.Z + normal1.Z);
                GL.Vertex3(tmp[0] + endPt.X + normal2.X, tmp[1] + endPt.Y + normal2.Y, tmp[2] + endPt.Z + normal2.Z);

                GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);
                GL.Vertex3(tmp[0] + endPt.X - normal1.X, tmp[1] + endPt.Y - normal1.Y, tmp[2] + endPt.Z - normal1.Z);
                GL.Vertex3(tmp[0] + endPt.X - normal2.X, tmp[1] + endPt.Y - normal2.Y, tmp[2] + endPt.Z - normal2.Z);

                GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);
                GL.Vertex3(tmp[0] + endPt.X + normal1.X, tmp[1] + endPt.Y + normal1.Y, tmp[2] + endPt.Z + normal1.Z);
                GL.Vertex3(tmp[0] + endPt.X - normal2.X, tmp[1] + endPt.Y - normal2.Y, tmp[2] + endPt.Z - normal2.Z);

                GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);
                GL.Vertex3(tmp[0] + endPt.X - normal1.X, tmp[1] + endPt.Y - normal1.Y, tmp[2] + endPt.Z - normal1.Z);
                GL.Vertex3(tmp[0] + endPt.X + normal2.X, tmp[1] + endPt.Y + normal2.Y, tmp[2] + endPt.Z + normal2.Z);

                GL.End();
            }
        } // draw always has mm input
        private void DrawFixity(double[] N, int[] Fix, Color Color)
        {
            double squareoffset = 1.5d;
            double[] tmp = null;

            if (_Dimensions == 1)
            {
                tmp = new[] { N[0], 0d, 0d };
            }

            else if (_Dimensions == 2)
            {
                tmp = new[] { N[0], N[1], 0d };
            }

            else // dimensions 3 or 6
            {
                tmp = N;
            }

            GL.Color3(Color); // set drawing color


            if (Fix[0] == 1) // X Axis
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex3(tmp[0], tmp[1] + squareoffset, tmp[2] + squareoffset);
                GL.Vertex3(tmp[0], tmp[1] - squareoffset, tmp[2] + squareoffset);
                GL.Vertex3(tmp[0], tmp[1] - squareoffset, tmp[2] - squareoffset);
                GL.Vertex3(tmp[0], tmp[1] + squareoffset, tmp[2] - squareoffset);
                GL.End();
            }

            if (_Dimensions > 1) // or else will error when searching for invalid value
            {
                if (Fix[1] == 1) // Y Axis
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex3(tmp[0] + squareoffset, tmp[1], tmp[2] + squareoffset);
                    GL.Vertex3(tmp[0] - squareoffset, tmp[1], tmp[2] + squareoffset);
                    GL.Vertex3(tmp[0] - squareoffset, tmp[1], tmp[2] - squareoffset);
                    GL.Vertex3(tmp[0] + squareoffset, tmp[1], tmp[2] - squareoffset);
                    GL.End();
                }
            }

            if (_Dimensions > 2) // or else will error when searching for invalid value
            {
                if (Fix[2] == 1) // Z Axis
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex3(tmp[0] + squareoffset, tmp[1] + squareoffset, tmp[2]);
                    GL.Vertex3(tmp[0] - squareoffset, tmp[1] + squareoffset, tmp[2]);
                    GL.Vertex3(tmp[0] - squareoffset, tmp[1] - squareoffset, tmp[2]);
                    GL.Vertex3(tmp[0] + squareoffset, tmp[1] - squareoffset, tmp[2]);
                    GL.End();
                }
            }
        } // draw always has mm input
        private bool ValEqualsZero(double value)
        {
            if (value == 0d)
            {
                return true;
            }
            else
            {
                return false;
            }
        }  // used to check if all force values are 0 for drawing


        private void InvalidateSolution()
        {
            _SolutionValid = false;
            SolutionInvalidated?.Invoke(_ID);
        }

    }
}
