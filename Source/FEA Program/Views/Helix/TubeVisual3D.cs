using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using Matrix = SharpDX.Matrix;



namespace FEA_Program.Views.Helix
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

        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
            nameof(Selected), typeof(bool), typeof(TubeVisual3D), new PropertyMetadata(false));

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

        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }


        public TubeVisual3D()
        {
            // Create a model and add to the group's children
            tubeModel = new MeshGeometryModel3D();

            Children.Add(tubeModel);

            Mouse3DDown += OnMouse3DDown;

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

            // Build a cylinder mesh along the X-axis, centered at the origin
            var mb = new MeshBuilder(true, true); // generate normals & tangents
            var halfLength = Vector3.Distance(StartPoint, EndPoint) / 2f;
            mb.AddCylinder(
                new Vector3(-halfLength, 0, 0), // start
                new Vector3(halfLength, 0, 0),  // end
                Radius,
                DefaultTessellation
            );

            tubeModel.Geometry = mb.ToMeshGeometry3D();

            // Rotate + Translate the cylinder so it ends on both points
            // Compute transform: rotate + translate
            var direction = EndPoint - StartPoint;
            direction.Normalize();

            // Default cylinder direction (local) is along +X
            var defaultDir = Vector3.UnitX;

            // Compute rotation from +X to actual direction
            Quaternion rotation;
            float dot = Vector3.Dot(defaultDir, direction);
            if (dot > 0.9999f)
            {
                rotation = Quaternion.Identity;
            }
            else if (dot < -0.9999f)
            {
                // Opposite direction — rotate 180 deg around Y axis
                rotation = Quaternion.RotationAxis(Vector3.UnitY, MathUtil.Pi);
            }
            else
            {
                var axis = Vector3.Cross(defaultDir, direction);
                axis.Normalize();
                float angle = MathF.Acos(dot);
                rotation = Quaternion.RotationAxis(axis, angle);
            }

            // Center of the cylinder
            var center = (StartPoint + EndPoint) * 0.5f;

            // Apply transform
            var transform = Matrix.AffineTransformation(1.0f, rotation, center);
            Transform = new System.Windows.Media.Media3D.MatrixTransform3D(transform.ToMatrix3D());
        }
        private void UpdateMaterial()
        {
            // Update material color (in case FillColor changed)
            tubeModel.Material = new PhongMaterial()
            {
                DiffuseColor = Utils.ToColor4(FillColor)
            };
        }

        protected override void OnMouse3DDown(object? sender, RoutedEventArgs e)
        {
            if (e is not Mouse3DEventArgs args)
                return;

            if (args.Viewport is null)
                return;

            // Filter different mouse buttons
            if (args.OriginalInputEventArgs is MouseButtonEventArgs mouseArgs)
            {
                switch (mouseArgs.ChangedButton)
                {
                    case MouseButton.Left:
                        // Left click - toggle selection
                        Selected = !Selected;
                        break;

                    case MouseButton.Right:
                        // Right click - show context menu
                        Selected = true;
                        DisplayContextMenu();
                        break;

                    case MouseButton.Middle:
                        // Middle click
                        break;
                }
            }
        }

        /// <summary>
        /// Display the context menu - this can't be done purely from .xaml
        /// </summary>
        private void DisplayContextMenu()
        {
            ContextMenu.DataContext = DataContext;
            ContextMenu.Placement = PlacementMode.MousePoint;
            ContextMenu.IsOpen = true;
        }
    }
}
