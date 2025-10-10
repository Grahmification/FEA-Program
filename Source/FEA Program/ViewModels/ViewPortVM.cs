using FEA_Program.ViewModels.Base;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows.Media;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Media3D = System.Windows.Media.Media3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using System.Collections.ObjectModel;
using System.IO;

namespace FEA_Program.ViewModels
{
    internal class ViewPortVM: ObservableObject
    {
        public EffectsManager? EffectsManager { get; }
        public Camera Camera { get; }

        public Color AmbientLightColor { get; }
        public Color DirectionalLightColor { get; }
        public Vector3D DirectionalLightDirection { get; }

        public Stream? BackgroundTexture { get; }


        public LineGeometry3D Grid { get; private set; }
        public Color GridColor { get; private set; }
        public Transform3D GridTransform { get; private set; }

        public MeshGeometry3D SphereModel { private set; get; }

        public ObservableCollection<MeshDataModel> ItemsSource { private set; get; } = [];

        public ObservableCollection<Element3D> SceneObjects { get; } = [];

        public ObservableCollection<ColorDataModel> ColorItems { private set; get; } = [];


        public ViewPortVM()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = DefaultOrthographicCamera();

            // Setup lighting
            AmbientLightColor = Colors.DimGray;
            DirectionalLightColor = Colors.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            // Background coloring
            BackgroundTexture = BitmapExtensions.CreateLinearGradientBitmapStream(EffectsManager, 128, 128, Direct2DImageFormat.Bmp,
                new Vector2(0, 0), new Vector2(0, 128), new SharpDX.Direct2D1.GradientStop[]
                {
                                new(){ Color = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1f), Position = 0f },
                                new(){ Color = new SharpDX.Mathematics.Interop.RawColor4(1f, 1f, 1f, 1f), Position = 1f }
                });

            // Floor plane grid
            Grid = LineBuilder.GenerateGrid(Vector3.UnitY, -5, 5);
            GridColor = Colors.Gray;
            GridTransform = new Media3D.TranslateTransform3D(0, 0, 0);



            var meshBuilder = new MeshBuilder(true);
            meshBuilder.AddSphere(new Vector3(0, 0, 0), 0.5f);
            SphereModel = meshBuilder.ToMeshGeometry3D();

            for (int i = 0; i < 5; i++)
                AddItemsModel();

            AddGroupItems();

            for (int i = 0; i < 5; i++)
            {
                var tmp = new ColorDataModel() { Position = new(1 * i, 1 * i, 1 * i) };
                ColorItems.Add(tmp);
            }
        }


        private void AddGroupItems()
        {
            // Materials
            var red = new PhongMaterial { DiffuseColor = new Color4(1, 0, 0, 1) };
            var blue = new PhongMaterial { DiffuseColor = new Color4(0, 0, 1, 1) };

            var meshBuilder = new MeshBuilder(true);
            meshBuilder.AddBox(new Vector3(0, 0, 0), 1, 1, 1);

            // Create a box
            var box = new MeshGeometryModel3D
            {
                Geometry = meshBuilder.ToMeshGeometry3D(),
                Material = red
            };

            meshBuilder = new MeshBuilder(true);
            meshBuilder.AddSphere(new Vector3(2, 0, 0), 0.5);

            // Create a sphere
            var sphere = new MeshGeometryModel3D
            {
                Geometry = meshBuilder.ToMeshGeometry3D(),
                Material = blue
            };

            // Add to the collection — the GroupModel3D in XAML will render them as a group
            SceneObjects.Add(box);
            SceneObjects.Add(sphere);
        }


        private void AddItemsModel()
        {
            var model = new MeshDataModel
            {
                Geometry = SphereModel,
                Material = PhongMaterials.Green,
                Transform = new Media3D.TranslateTransform3D(0, -ItemsSource.Count * 2, 0)
            };

            ItemsSource.Add(model);
        }




        public static OrthographicCamera DefaultOrthographicCamera() => new()
        {
            Position = new Media3D.Point3D(0, 0, 20),
            LookDirection = new Vector3D(0, 0, -20),
            UpDirection = new Vector3D(0, 1, 0),
            Width = 20,
            //NearPlaneDistance = 1,
            //FarPlaneDistance = 100,
        };
        public static PerspectiveCamera DefaultPerspectiveCamera() => new()
        {
            Position = new Media3D.Point3D(0, 0, 5),
            LookDirection = new Vector3D(0, 0, -5),
            UpDirection = new Vector3D(0, 1, 0),
            NearPlaneDistance = 0.5,
            FarPlaneDistance = 150
        };


        public class MeshDataModel
        {
            public required Geometry3D Geometry { set; get; }
            public required Material Material { set; get; }
            public required Transform3D Transform { set; get; }
        }

        public class ColorDataModel
        {
            public Vector3 Position { set; get; } = new Vector3();
        }


    }
}
