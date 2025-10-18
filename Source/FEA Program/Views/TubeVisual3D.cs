using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;



namespace FEA_Program.Views
{
    public class TubeVisual3D : GroupModel3D
    {
        private const int DefaultTessellation = 32;

        // The geometry/model we will hold in the Group
        private readonly MeshGeometryModel3D tubeModel;

        // Dependency properties
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register(nameof(StartPoint), typeof(Vector3), typeof(TubeVisual3D),
                new PropertyMetadata(new Vector3(0, 0, 0), OnGeometryChanged));

        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register(nameof(EndPoint), typeof(Vector3), typeof(TubeVisual3D),
                new PropertyMetadata(new Vector3(0, 1, 0), OnGeometryChanged));

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(nameof(Radius), typeof(float), typeof(TubeVisual3D),
                new PropertyMetadata(0.1f, OnGeometryChanged));

        public static readonly DependencyProperty FillColorProperty =
            DependencyProperty.Register(nameof(FillColor), typeof(Color), typeof(TubeVisual3D),
                new PropertyMetadata(Colors.LightGray, OnMaterialChanged));

        public Vector3 StartPoint
        {
            get => (Vector3)GetValue(StartPointProperty);
            set => SetValue(StartPointProperty, value);
        }
        public Vector3 EndPoint
        {
            get => (Vector3)GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }
        public float Radius
        {
            get => (float)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }
        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }


        public TubeVisual3D()
        {
            // Create a model and add to the group's children
            tubeModel = new MeshGeometryModel3D();

            Children.Add(tubeModel);

            // Build initial mesh and set color
            RebuildTube();
            UpdateMaterial();
        }

        private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tv = (TubeVisual3D)d;
            tv.RebuildTube();
        }
        private static void OnMaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tv = (TubeVisual3D)d;
            tv.UpdateMaterial();
        }

        private void RebuildTube()
        {
            // If cylinder length is zero, don't build a degenerate cylinder
            if (StartPoint == EndPoint)
            {
                tubeModel.Geometry = null;
                return;
            }

            // Build cylinder mesh between two points
            var mb = new MeshBuilder(true, true); // generate normals & tangents
            mb.AddCylinder(StartPoint, EndPoint, Radius, DefaultTessellation);

            tubeModel.Geometry = mb.ToMeshGeometry3D();
        }
        private void UpdateMaterial()
        {
            // Update material color (in case FillColor changed)
            tubeModel.Material = new PhongMaterial()
            {
                DiffuseColor = new Color4(FillColor.ScR, FillColor.ScG, FillColor.ScB, FillColor.ScA)
            };
        }

    }
}
