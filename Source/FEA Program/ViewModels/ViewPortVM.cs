using FEA_Program.ViewModels.Base;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Media3D = System.Windows.Media.Media3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using System.IO;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for the 3D viewer
    /// </summary>
    internal class ViewPortVM: ObservableObject
    {
        /// <summary>
        /// True to draw the view in 3D, false for 2D
        /// </summary>
        private bool _is3Dimensional = true;

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when a command for zooming to extents has occurred
        /// </summary>
        public event EventHandler? ZoomExtentsRequested;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The effects manager for lighting
        /// </summary>
        public EffectsManager? EffectsManager { get; }

        /// <summary>
        /// Camera controlling the view perspective
        /// </summary>
        public Camera Camera { get; private set; }

        /// <summary>
        /// Background display texture
        /// </summary>
        public Stream? BackgroundTexture { get; }

        /// <summary>
        /// True to draw the view in 3D, false for 2D
        /// </summary>
        public bool Is3Dimensional 
        { get => _is3Dimensional;
            set 
            {
                _is3Dimensional = value;
                ResetCamera();
            }
        }

        /// <summary>
        /// Text for the coordinate system Z axis
        /// </summary>
        public string CoordZText => Is3Dimensional ? "Z" : "";

        // ---------------------- Sub VMs ----------------------

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; set; } = new();

        // ----------------------- Commands -----------------------------

        /// <summary>
        /// Command for resetting the camera to default
        /// </summary>
        public ICommand ResetCameraCommand { get; }

        /// <summary>
        /// Command for zooming the camera to extents of the model
        /// </summary>
        public ICommand ZoomToExtentsCommand { get; }

        // ----------------------- Public Methods -----------------------------

        /// <summary>
        /// Primary constructor
        /// </summary>
        public ViewPortVM()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = DefaultOrthographicCamera();

            // Background coloring
            BackgroundTexture = BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
                new Vector2(0, 0), new Vector2(0, 128), new SharpDX.Direct2D1.GradientStop[]
                {
                    new(){ Color = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1f), Position = 0f },
                    new(){ Color = new SharpDX.Mathematics.Interop.RawColor4(1f, 1f, 1f, 1f), Position = 1f }
                });

            ResetCameraCommand = new RelayCommand(ResetCamera);
            ZoomToExtentsCommand = new RelayCommand(ZoomToExtents);
        }

        /// <summary>
        /// Resets the camera to default view
        /// </summary>
        public void ResetCamera()
        {
            try
            {
                Camera = Is3Dimensional ? DefaultPerspectiveCamera() : DefaultOrthographicCamera();
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Zooms the camera to extents of the model
        /// </summary>
        public void ZoomToExtents()
        {
            ZoomExtentsRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the default camera for 2D views
        /// </summary>
        /// <returns></returns>
        public static OrthographicCamera DefaultOrthographicCamera() => new()
        {
            Position = new Media3D.Point3D(0, 0, 60),
            LookDirection = new Vector3D(0, 0, -60),
            UpDirection = new Vector3D(0, 1, 0),
            Width = 20,
            NearPlaneDistance = 1,
            FarPlaneDistance = 1500,
        };

        /// <summary>
        /// Gets the default camera for 3D views
        /// </summary>
        /// <returns></returns>
        public static PerspectiveCamera DefaultPerspectiveCamera() => new()
        {
            Position = new Media3D.Point3D(0, 15, 60),
            LookDirection = new Vector3D(0, -15, -60),
            UpDirection = new Vector3D(0, 1, 0),
            NearPlaneDistance = 0.1,
            FarPlaneDistance = 1500
        };
    }
}
