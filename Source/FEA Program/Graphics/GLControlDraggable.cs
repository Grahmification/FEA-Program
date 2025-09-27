using OpenTK.GLControl;
using OpenTK.Mathematics;

namespace FEA_Program.Graphics
{
    internal class GLControlDraggable
    {

        private Color _backcolor;
        private Graphics.View _view = new();
        private Matrix4 _Orientation = Matrix4.Identity;

        public event DrawStuffBeforeOrientationEventHandler? DrawStuffBeforeOrientation;

        public delegate void DrawStuffBeforeOrientationEventHandler(bool ThreeDimensional);
        public event ViewUpdatedEventHandler? ViewUpdated;

        public delegate void ViewUpdatedEventHandler(Vector3 Trans, Matrix4 rot, Vector3 zoom, bool ThreeDimensional);
        public event DrawStuffAfterOrientationEventHandler? DrawStuffAfterOrientation;

        public delegate void DrawStuffAfterOrientationEventHandler(bool ThreeDimensional);

        private double _MouseRotMultiplier = 0.05d;
        private double _MouseTransMultiplier = 0.3d;
        private double _MouseZoomMultiplier = 0.05d;

        private bool _ThreeDimensional = false; // if false sets viewing to 2D style

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
                _view = new(); // reset the view
                SubControl.Invalidate();
            }
        }

        public GLControlDraggable(GLControl subControl, Color BackColor, bool ThreeDimensional)
        {
            _backcolor = BackColor;
            _ThreeDimensional = ThreeDimensional;

            SubControl = subControl;
            Input.Initialize(SubControl);

            SubControl.MouseMove += OnMouseMove;
            SubControl.MouseWheel += OnMouseWheel;
            SubControl.Paint += OnPaint;

            OnPaint(this, default);
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (Input.buttonDown(System.Windows.Forms.MouseButtons.Left)) // screen translation
            {
                var mousevector = Input.MouseLastVector(e.X, e.Y);
                _view.SetOrientation_Relative(new Vector3((float)(mousevector.X * _MouseTransMultiplier), (float)(mousevector.Y * -_MouseTransMultiplier), 0), Matrix4.Identity, Vector3.Zero, true);

                SubControl.Invalidate();
            }

            else if (Input.buttonDown(System.Windows.Forms.MouseButtons.Right) & _ThreeDimensional) // only rotate in 3D mode
            {
                var MouseRotVector = Input.MouseLastRotationVector(e.X, e.Y);
                Matrix4 RotInput = Matrix4.CreateFromAxisAngle(new Vector3(MouseRotVector.Y, MouseRotVector.X, 0), (float)_MouseRotMultiplier);
                _view.SetOrientation_Relative(Vector3.Zero, RotInput, Vector3.Zero, true);

                SubControl.Invalidate();
            }

            Input.update(e.X, e.Y);
        }
        private void OnMouseWheel(object? sender, MouseEventArgs e)
        {

            if (e.Delta > 0)
            {
                _view.SetOrientation_Relative(Vector3.Zero, Matrix4.Identity, new Vector3((float)_MouseZoomMultiplier, (float)_MouseZoomMultiplier, (float)_MouseZoomMultiplier), true);
            }
            else
            {
                _view.SetOrientation_Relative(Vector3.Zero, Matrix4.Identity, new Vector3((float)-_MouseZoomMultiplier, (float)-_MouseZoomMultiplier, (float)-_MouseZoomMultiplier), true);
            }

            SubControl.Invalidate();
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            _view.PrepCamera(SubControl.Width, SubControl.Height, _backcolor, _ThreeDimensional);
            DrawStuffBeforeOrientation?.Invoke(_ThreeDimensional);

            _Orientation = _view.ApplyView();
            ViewUpdated?.Invoke(_view.CurrentTransLation, _view.CurrentRotation, _view.CurrentZoom, _ThreeDimensional);
            DrawStuffAfterOrientation?.Invoke(_ThreeDimensional);

            //Finally...
            if (SubControl.Context != null)
                SubControl.Context.SwapInterval = 1;
            SubControl.SwapBuffers(); //Takes from the 'GL' and puts into control 
        }
    }
}
