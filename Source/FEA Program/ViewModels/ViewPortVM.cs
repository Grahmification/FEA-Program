using FEA_Program.ViewModels.Base;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using Media3D = System.Windows.Media.Media3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using System.IO;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class ViewPortVM: ObservableObject
    {
        private bool _is3Dimensional = true;
        
        public EffectsManager? EffectsManager { get; }
        public Camera Camera { get; private set; }
        public Stream? BackgroundTexture { get; }

        public bool Is3Dimensional 
        { get => _is3Dimensional;
            set 
            {
                _is3Dimensional = value;
                ResetCamera();
            }
        }
        public string CoordZText => Is3Dimensional ? "Z" : "";


        // ----------------------- Commands -----------------------------
        public ICommand ResetCameraCommand { get; }

        // ----------------------- Public Methods -----------------------------
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
        }
        public void ResetCamera()
        {
            Camera = Is3Dimensional ? DefaultPerspectiveCamera() : DefaultOrthographicCamera();
        }

        public static OrthographicCamera DefaultOrthographicCamera() => new()
        {
            Position = new Media3D.Point3D(0, 0, 60),
            LookDirection = new Vector3D(0, 0, -60),
            UpDirection = new Vector3D(0, 1, 0),
            Width = 20,
            NearPlaneDistance = 1,
            FarPlaneDistance = 1500,
        };
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
