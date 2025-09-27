using OpenTK.GLControl;
using OpenTK.Mathematics;
using FEA_Program.Graphics;

namespace FEA_Program.Controls
{
    public partial class GLControlMod : GLControl
    {
        private Color _backcolor = Color.Black;
        private Graphics.View _view = new();
        private Matrix4 _Orientation = Matrix4.Identity;

        public event DrawStuffBeforeOrientationEventHandler DrawStuffBeforeOrientation;

        public delegate void DrawStuffBeforeOrientationEventHandler(bool ThreeDimensional);
        public event ViewUpdatedEventHandler ViewUpdated;

        public delegate void ViewUpdatedEventHandler(Vector3 Trans, Matrix4 rot, Vector3 zoom, bool ThreeDimensional);
        public event DrawStuffAfterOrientationEventHandler DrawStuffAfterOrientation;

        public delegate void DrawStuffAfterOrientationEventHandler(bool ThreeDimensional);

        private double _MouseRotMultiplier = 0.05d;
        private double _MouseTransMultiplier = 0.3d;
        private double _MouseZoomMultiplier = 0.05d;

        private bool _ThreeDimensional = false; // if false sets viewing to 2D style

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
                this.Invalidate();
            }
        }


        public GLControlMod()
        {
            _backcolor = BackColor;
            _ThreeDimensional = ThreeDimensional;
            Input.Initialize(this);

            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            _view.PrepCamera(this.Width, this.Height, _backcolor, _ThreeDimensional);
            DrawStuffBeforeOrientation?.Invoke(_ThreeDimensional);

            _Orientation = _view.ApplyView();
            ViewUpdated?.Invoke(_view.CurrentTransLation, _view.CurrentRotation, _view.CurrentZoom, _ThreeDimensional);
            DrawStuffAfterOrientation?.Invoke(_ThreeDimensional);

            if (Context != null)
                Context.SwapInterval = 1;

            this.SwapBuffers();
        }
    }
}
