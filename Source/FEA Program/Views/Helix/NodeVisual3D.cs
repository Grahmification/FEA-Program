using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;

namespace FEA_Program.Views.Helix
{
    internal class NodeVisual3D : GroupModel3D
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Color4), typeof(NodeVisual3D),
            new PropertyMetadata(new Color4(0, 1, 0, 1), OnColorChanged));

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
                nameof(Position), typeof(Vector3), typeof(NodeVisual3D),
                new PropertyMetadata(new Vector3(), OnPositionChanged));

        public Color4 Color
        {
            get => (Color4)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public Vector3 Position
        {
            get => (Vector3)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public double Size { get; set; } = 1;

        public NodeVisual3D()
        {
            var meshBuilder = new MeshBuilder(true);
            meshBuilder.AddBox(new Vector3(0, 0, 0), Size, Size, Size);

            // Create a box
            var box = new MeshGeometryModel3D
            {
                Geometry = meshBuilder.ToMeshGeometry3D(),
                Material = new PhongMaterial
                {
                    DiffuseColor = Color,
                    AmbientColor = new Color4(0.3f),
                    SpecularColor = new Color4(0.5f),
                }
            };

            Children.Add(box);
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeVisual3D group)
                group.UpdateChildrenColor();
        }
        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeVisual3D group)
                group.UpdateTransform();
        }

        private void UpdateChildrenColor()
        {
            // Create a shared PhongMaterial with the new color
            var mat = new PhongMaterial { DiffuseColor = Color };

            // Apply to all MeshGeometryModel3D children
            foreach (var mesh in Children.OfType<MeshGeometryModel3D>())
                mesh.Material = mat;
        }
        private void UpdateTransform()
        {
            Transform = new TranslateTransform3D(Position.X, Position.Y, Position.Z);
        }
    }
}
