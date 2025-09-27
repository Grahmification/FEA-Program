using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FEA_Program.Graphics
{
    internal class View
    {

        private Vector3 _Trans_Current = Vector3.Zero;
        private Vector3 _Trans_GoTo = Vector3.Zero;
        private Matrix4 _Rot_Current = Matrix4.CreateRotationX(0); // default to zero rotation
        private Matrix4 _Rot_GoTo = Matrix4.CreateRotationX(0); // default to zero rotation
        private Vector3 _Zoom_Current = new Vector3(1, 1, 1);
        private Vector3 _Zoom_GoTo = new Vector3(1, 1, 1);

        public Vector3 CurrentTransLation
        {
            get
            {
                return _Trans_Current;
            }
        }
        public Matrix4 CurrentRotation
        {
            get
            {
                return _Rot_Current;
            }
        }
        public Vector3 CurrentZoom
        {
            get
            {
                return _Zoom_Current;
            }
        }


        public void PrepCamera(int Screenwidth, int ScreenHeight, Color _backcolor, bool ThreeDimensional)
        {
            // First Clear Buffers
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);


            // Basic Setup for viewing

            Matrix4 perspective = Matrix4.CreateOrthographic(Screenwidth, ScreenHeight, 0.001f, 1000); // Setup 2D Perspective
            if (ThreeDimensional)
            {
                perspective = Matrix4.CreatePerspectiveFieldOfView(1.1f, 4f / 3f, 1, 10000); // Change to 3D perspective if desired
            }

            GL.MatrixMode(MatrixMode.Projection); // Load Perspective
            GL.LoadIdentity();
            GL.LoadMatrix(SpriteBatch.MatrixToArray(perspective));

            Matrix4 lookat = Matrix4.LookAt(0, 0, 100, 0, 0, 0, 0, 1, 0); // Setup camera
            GL.MatrixMode(MatrixMode.Modelview); // Load Camera
            GL.LoadIdentity();
            GL.LoadMatrix(SpriteBatch.MatrixToArray(lookat));

            GL.ClearColor(_backcolor); // set background color

            GL.Viewport(0, 0, Screenwidth, ScreenHeight); // Size of window
            GL.Enable(EnableCap.DepthTest); // Enable correct Z Drawings
            GL.DepthFunc(DepthFunction.Less); // Enable correct Z Drawings

        }

        public void SetOrientation_ABS(Vector3 Trans, Matrix4 Rot, Vector3 Zoom)
        {

            _Trans_GoTo = Trans;
            _Rot_GoTo = Rot;
            _Zoom_GoTo = Zoom;

        }
        public void SetOrientation_Relative(Vector3 Trans, Matrix4 Rot, Vector3 Zoom, bool rotRelativeToScreen)
        {

            _Trans_GoTo = _Trans_Current + Trans;

            if (rotRelativeToScreen)
            {
                _Rot_GoTo = _Rot_Current * Rot;
            }
            else
            {
                _Rot_GoTo = Rot * _Rot_Current;
            }

            if (_Zoom_Current.X + Zoom.X > 0 & _Zoom_Current.Y + Zoom.Y > 0 & _Zoom_Current.Z + Zoom.Z > 0) // don't allow zoom to go negative
            {
                _Zoom_GoTo = _Zoom_Current + Zoom;
            }

        }
        public Matrix4 ApplyView()
        {
            Matrix4 xform = Matrix4.Identity;

            xform = Matrix4.Mult(xform, _Rot_GoTo);
            xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(_Trans_GoTo.X, _Trans_GoTo.Y, _Trans_GoTo.Z));
            xform = Matrix4.Mult(xform, Matrix4.CreateScale(_Zoom_Current.X, _Zoom_Current.Y, _Zoom_Current.Z));

            GL.MultMatrix(SpriteBatch.MatrixToArray(xform));

            _Trans_Current = _Trans_GoTo;
            _Rot_Current = _Rot_GoTo;
            _Zoom_Current = _Zoom_GoTo;

            return xform;
        }

    }
}
