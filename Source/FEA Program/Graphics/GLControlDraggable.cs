using OpenTK.GLControl;
using OpenTK.Mathematics;

namespace FEA_Program.Graphics
{
    internal class GLControlDraggable
    {
        private GLView _cameraView = new();

        private double _MouseRotMultiplier = 0.05d;
        private double _MouseTransMultiplier = 0.3d;
        private double _MouseZoomMultiplier = 0.05d;

        private bool _ThreeDimensional = false; // if false sets viewing to 2D style


        public event EventHandler<GLControlViewUpdatedEventArgs>? ViewUpdated;
        public event EventHandler<bool>? DrawStuff;

        /// <summary>
        /// The control managed by this class
        /// </summary>
        public GLControl SubControl { get; private set; }

        public bool ThreeDimensional
        {
            get
            {
                return _ThreeDimensional;
            }
            set
            {
                _ThreeDimensional = value;
                _cameraView = new(); // reset the view
                SubControl.Invalidate();
            }
        }

        public GLControlDraggable(GLControl subControl, bool threeDimensional)
        {
            _ThreeDimensional = threeDimensional;

            SubControl = subControl;
            InputManager.Initialize(SubControl);

            SubControl.MouseMove += OnMouseMove;
            SubControl.MouseWheel += OnMouseWheel;
            SubControl.Paint += OnPaint;
            SubControl.Resize += OnResize;
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (InputManager.IsButtonDown(MouseButtons.Left)) // screen translation
            {
                var mousevector = InputManager.MouseLastVector(e.X, e.Y);
                _cameraView.SetOrientationRelative(new Vector3((float)(mousevector.X * _MouseTransMultiplier), (float)(mousevector.Y * -_MouseTransMultiplier), 0), Matrix4.Identity, Vector3.Zero, true);
            }
            else if (InputManager.IsButtonDown(MouseButtons.Right) & _ThreeDimensional) // only rotate in 3D mode
            {
                var MouseRotVector = InputManager.MouseLastRotationVector(e.X, e.Y);
                Matrix4 RotInput = Matrix4.CreateFromAxisAngle(new Vector3(MouseRotVector.Y, MouseRotVector.X, 0), (float)_MouseRotMultiplier);
                _cameraView.SetOrientationRelative(Vector3.Zero, RotInput, Vector3.Zero, true);
            }

            SubControl.Invalidate();
            InputManager.Update(e.X, e.Y);
        }
        private void OnMouseWheel(object? sender, MouseEventArgs e)
        {
            float zoom = (e.Delta > 0) ? 1 : -1; // Set the direction
            zoom *= (float)_MouseZoomMultiplier;

            _cameraView.SetOrientationRelative(Vector3.Zero, Matrix4.Identity, new Vector3(zoom, zoom, zoom), true);
            SubControl.Invalidate();
        }
        private void OnPaint(object? sender, PaintEventArgs e)
        {
            GLView.PrepCamera(SubControl.Width, SubControl.Height, ThreeDimensional);

            _cameraView.ApplyView();

            DrawStuff?.Invoke(this, ThreeDimensional);
            ViewUpdated?.Invoke(this, new(_cameraView.CurrentTransLation, _cameraView.CurrentRotation, _cameraView.CurrentZoom.X, ThreeDimensional));

            //Finally...
            if (SubControl.Context != null)
                SubControl.Context.SwapInterval = 1;
            SubControl.SwapBuffers(); //Takes from the 'GL' and puts into control 
        }
        private void OnResize(object? Sender, EventArgs e)
        {
            SubControl.Invalidate(); // Refresh the view
        }
    }
}
