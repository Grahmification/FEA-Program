using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Color = System.Windows.Media.Color;
using Matrix = SharpDX.Matrix;

namespace FEA_Program.Views.Helix
{
    /// <summary>
    /// 3D visual for an arrow with adjustable length and direction
    /// </summary>
    internal class ArrowVisual3D : GroupModel3D
    {
        // Shaft geometry (cylinder)
        private readonly MeshGeometryModel3D shaftModel;

        // Head geometry (cone)
        private readonly MeshGeometryModel3D headModel;

        // DependencyProperty: Direction
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
            nameof(Direction), typeof(Vector3), typeof(ArrowVisual3D),
            new PropertyMetadata(new Vector3(0, 1, 0), OnTransformChanged));

        public Vector3 Direction
        {
            get => (Vector3)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        // DependencyProperty: Length
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            nameof(Length), typeof(double), typeof(ArrowVisual3D),
            new PropertyMetadata(1.0, OnTransformChanged));

        public double Length
        {
            get => (double)GetValue(LengthProperty);
            set => SetValue(LengthProperty, value);
        }

        // DependencyProperty: Color
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Color), typeof(ArrowVisual3D),
            new PropertyMetadata(Colors.Red, OnColorChanged));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        // Constructor
        public ArrowVisual3D()
        {
            // Create shaft (cylinder)
            var builder = new MeshBuilder();
            builder.AddCylinder(Vector3.Zero, new Vector3(0, 0.8f, 0), 0.05f);
            shaftModel = new MeshGeometryModel3D
            {
                Geometry = builder.ToMeshGeometry3D(),
                Material = new PhongMaterial
                {
                    DiffuseColor = Utils.ToColor4(Color),
                    AmbientColor = new Color4(0.3f),
                    SpecularColor = new Color4(0.5f),
                }
            };

            // Create head (cone)
            builder = new MeshBuilder();
            builder.AddCone(new Vector3(0, 0.8f, 0), new Vector3(0, 1.0f, 0), 0.12f, true, 20);
            headModel = new MeshGeometryModel3D
            {
                Geometry = builder.ToMeshGeometry3D(),
                Material = shaftModel.Material
            };

            // Add both to children
            Children.Add(shaftModel);
            Children.Add(headModel);

            UpdateTransform();
        }

        // Update direction/length
        private static void OnTransformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ArrowVisual3D arrow)
                arrow.UpdateTransform();
        }

        // Update color
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ArrowVisual3D arrow)
                arrow.UpdateColor();
        }

        private void UpdateColor()
        {
            var color4 = Utils.ToColor4(Color);
            if (shaftModel.Material is PhongMaterial mat1)
                mat1.DiffuseColor = color4;
            if (headModel.Material is PhongMaterial mat2)
                mat2.DiffuseColor = color4;
        }

        private void UpdateTransform()
        {
            // If length is zero, hide the arrow
            Visibility = Length == 0 ? Visibility.Hidden : Visibility.Visible;

            var dir = Direction;
            if (dir.LengthSquared() < 1e-6f) dir = new Vector3(0, 1, 0);
            dir.Normalize();

            // Compute rotation
            var baseDir = new Vector3(0, 1, 0);
            var dot = MathUtil.Clamp(Vector3.Dot(baseDir, dir), -1f, 1f);

            Matrix rotation;

            // Check if vectors are nearly the same or opposite
            if (dot > 0.9999f)
            {
                rotation = Matrix.Identity;  // Same direction - no rotation
            }
            else if (dot < -0.9999f)
            {
                // Opposite direction - flip by 180° rotation around *any* perpendicular axis
                // Since baseDir is (0,1,0), we can use X axis (1,0,0)
                rotation = Matrix.RotationAxis(Vector3.UnitX, MathUtil.Pi);
            }
            else
            {
                // Normal rotation using cross product
                var axis = Vector3.Cross(baseDir, dir);
                axis.Normalize();
                float angle = MathF.Acos(dot);
                rotation = Matrix.RotationAxis(axis, angle);
            }

            Matrix scale = Matrix.Scaling((float)Length * 2);  // Scale the arrow twice as large arbitrarily
            Matrix transform = scale * rotation;

            // Apply transform to group
            Transform = new MatrixTransform3D(
                new Matrix3D(
                    transform.M11, transform.M12, transform.M13, transform.M14,
                    transform.M21, transform.M22, transform.M23, transform.M24,
                    transform.M31, transform.M32, transform.M33, transform.M34,
                    transform.M41, transform.M42, transform.M43, transform.M44
                ));
        }
    }
}
