using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;

namespace FEA_Program.Views
{
    internal class NodeVisual3Dx : GroupModel3D
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color),
            typeof(Color4),
            typeof(NodeVisual3Dx),
            new PropertyMetadata(new Color4(1, 1, 1, 1), OnColorChanged));

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
                nameof(Position),
                typeof(Vector3),
                typeof(NodeVisual3Dx),
                new PropertyMetadata(new Vector3(), OnGeometryChanged));

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

        public NodeVisual3Dx()
        {
            var meshBuilder = new MeshBuilder(true);
            meshBuilder.AddBox(new Vector3(0, 0, 0), Size, Size, Size);

            // Create a box
            var box = new MeshGeometryModel3D
            {
                Geometry = meshBuilder.ToMeshGeometry3D(),
                Material = PhongMaterials.PolishedGold
            };

            Children.Add(box);
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeVisual3Dx group)
                group.UpdateChildrenColor();
        }
        private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeVisual3Dx group)
                group.UpdateGeometry();
        }

        private void UpdateChildrenColor()
        {
            // Create a shared PhongMaterial with the new color
            var mat = new PhongMaterial { DiffuseColor = Color };

            // Apply to all MeshGeometryModel3D children
            foreach (var mesh in Children.OfType<MeshGeometryModel3D>())
                mesh.Material = mat;
        }
        private void UpdateGeometry()
        {
            Transform = new System.Windows.Media.Media3D.TranslateTransform3D(Position.X, Position.Y, Position.Z);
        }
    }
}
