using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Media.Media3D;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;

namespace FEA_Program.Views
{
    /// <summary>
    /// Visual for node fixity
    /// </summary>
    internal class FixityVisual3D : GroupModel3D
    {
        private readonly MeshGeometryModel3D _rectX;
        private readonly MeshGeometryModel3D _rectY;
        private readonly MeshGeometryModel3D _rectZ;


        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Color4), typeof(FixityVisual3D),
            new PropertyMetadata(new Color4(0, 0, 1, 1), (d, e) => ((FixityVisual3D)d).UpdateColor()));

        public Color4 Color
        {
            get => (Color4)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public static readonly DependencyProperty ShowXProperty = DependencyProperty.Register(
            nameof(ShowX), typeof(bool), typeof(FixityVisual3D),
            new PropertyMetadata(true, (d, e) => ((FixityVisual3D)d).UpdateVisibility()));

        public bool ShowX
        {
            get => (bool)GetValue(ShowXProperty);
            set => SetValue(ShowXProperty, value);
        }

        public static readonly DependencyProperty ShowYProperty = DependencyProperty.Register(
            nameof(ShowY), typeof(bool), typeof(FixityVisual3D),
            new PropertyMetadata(true, (d, e) => ((FixityVisual3D)d).UpdateVisibility()));

        public bool ShowY
        {
            get => (bool)GetValue(ShowYProperty);
            set => SetValue(ShowYProperty, value);
        }

        public static readonly DependencyProperty ShowZProperty = DependencyProperty.Register(
            nameof(ShowZ), typeof(bool), typeof(FixityVisual3D),
            new PropertyMetadata(true, (d, e) => ((FixityVisual3D)d).UpdateVisibility()));

        public bool ShowZ
        {
            get => (bool)GetValue(ShowZProperty);
            set => SetValue(ShowZProperty, value);
        }

        public double Size { get; set; } = 1.0;

        public FixityVisual3D()
        {
            // Build rectangles
            var builder = new MeshBuilder();
            builder.AddBox(Vector3.Zero, Size*1.5, Size*1.5, 0.1 * Size);
            var rectGeom = builder.ToMeshGeometry3D();

            _rectX = CreateRectModel(rectGeom);
            _rectY = CreateRectModel(rectGeom);
            _rectZ = CreateRectModel(rectGeom);

            // Rotate each rectange to be on each of the major planes
            _rectZ.Transform = new TranslateTransform3D(0, 0, 0);  // No rotation
            _rectX.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 90));  // YZ plane → rotate 90° about Y to make normal +X
            _rectY.Transform = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));  // XZ plane → rotate 90° about X to make normal +Y

            // Add to children
            Children.Add(_rectX);
            Children.Add(_rectY);
            Children.Add(_rectZ);

            UpdateColor();
            UpdateVisibility();
        }
        private void UpdateColor()
        {
            var mat = new PhongMaterial { DiffuseColor = Color };
            _rectX.Material = mat;
            _rectY.Material = mat;
            _rectZ.Material = mat;
        }
        private void UpdateVisibility()
        {
            _rectX.Visibility = ShowX ? Visibility.Visible : Visibility.Collapsed;
            _rectY.Visibility = ShowY ? Visibility.Visible : Visibility.Collapsed;
            _rectZ.Visibility = ShowZ ? Visibility.Visible : Visibility.Collapsed;
        }
        private MeshGeometryModel3D CreateRectModel(MeshGeometry3D mesh)
        {
            return new MeshGeometryModel3D
            {
                Geometry = mesh,
                Material = new PhongMaterial { DiffuseColor = Color }
        };
        }
    }
}
