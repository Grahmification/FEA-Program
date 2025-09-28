using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FEA_Program.Graphics
{
    /// <summary>
    /// Computes the GL view for given rotations and translations
    /// </summary>
    internal class GLView
    {
        private Vector3 _TranslationCurrent = Vector3.Zero;
        private Vector3 _TranslationTarget = Vector3.Zero;
        private Matrix4 _RotationCurrent = Matrix4.CreateRotationX(0); // default to zero rotation
        private Matrix4 _RotationTarget = Matrix4.CreateRotationX(0); // default to zero rotation
        private Vector3 _ZoomCurrent = new(1, 1, 1);
        private Vector3 _ZoomTarget = new(1, 1, 1);

        public Vector3 CurrentTransLation => _TranslationCurrent;
        public Matrix4 CurrentRotation => _RotationCurrent;
        public Vector3 CurrentZoom => _ZoomCurrent;

        public static void PrepCamera(int screenwidth, int screenHeight, bool threeDimensional)
        {
            // First Clear Buffers
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            // Basic Setup for viewing
            Matrix4 perspective = Matrix4.CreateOrthographic(screenwidth, screenHeight, 0.001f, 1000); // Setup 2D Perspective
            if (threeDimensional)
            {
                perspective = Matrix4.CreatePerspectiveFieldOfView(1.1f, 4f / 3f, 1, 10000); // Change to 3D perspective if desired
            }

            GL.MatrixMode(MatrixMode.Projection); // Load Perspective
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);

            Matrix4 lookat = Matrix4.LookAt(0, 0, 100, 0, 0, 0, 0, 1, 0); // Setup camera
            GL.MatrixMode(MatrixMode.Modelview); // Load Camera
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);

            GL.Viewport(0, 0, screenwidth, screenHeight); // Size of window
            GL.Enable(EnableCap.DepthTest); // Enable correct Z Drawings
            GL.DepthFunc(DepthFunction.Less); // Enable correct Z Drawings
        }

        public void SetOrientation(Vector3 translation, Matrix4 rotation, Vector3 zoom)
        {
            _TranslationTarget = translation;
            _RotationTarget = rotation;
            _ZoomTarget = zoom;
        }

        public void SetOrientationRelative(Vector3 Trans, Matrix4 Rot, Vector3 Zoom, bool rotRelativeToScreen)
        {
            _TranslationTarget = _TranslationCurrent + Trans;

            if (rotRelativeToScreen)
            {
                _RotationTarget = _RotationCurrent * Rot;
            }
            else
            {
                _RotationTarget = Rot * _RotationCurrent;
            }

            // don't allow zoom to go negative
            if (_ZoomCurrent.X + Zoom.X > 0 & _ZoomCurrent.Y + Zoom.Y > 0 & _ZoomCurrent.Z + Zoom.Z > 0) 
            {
                _ZoomTarget = _ZoomCurrent + Zoom;
            }
        }

        public Matrix4 ApplyView()
        {
            Matrix4 xform = Matrix4.Identity;
            xform = Matrix4.Mult(xform, _RotationTarget);
            xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(_TranslationTarget.X, _TranslationTarget.Y, _TranslationTarget.Z));
            xform = Matrix4.Mult(xform, Matrix4.CreateScale(_ZoomCurrent.X, _ZoomCurrent.Y, _ZoomCurrent.Z));
            GL.MultMatrix(ref xform);

            _TranslationCurrent = _TranslationTarget;
            _RotationCurrent = _RotationTarget;
            _ZoomCurrent = _ZoomTarget;

            return xform;
        }

    }
}
