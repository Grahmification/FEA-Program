using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Media;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Color = System.Windows.Media.Color;
using System.Windows.Controls;

namespace FEA_Program.Views.Helix
{
    internal class NodeVisual3D : GroupModel3D
    {

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Color), typeof(NodeVisual3D),
            new PropertyMetadata(Colors.Green, OnColorChanged));

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
                nameof(Position), typeof(Vector3), typeof(NodeVisual3D),
                new PropertyMetadata(new Vector3(), OnPositionChanged));

        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
            nameof(Selected), typeof(bool), typeof(NodeVisual3D), new PropertyMetadata(false));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public Vector3 Position
        {
            get => (Vector3)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
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
                    DiffuseColor = Utils.ToColor4(Color),
                    AmbientColor = new Color4(0.3f),
                    SpecularColor = new Color4(0.5f),
                }
            };

            Children.Add(box);

            //Mouse3DDown += OnMouse3DDown;
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
            var mat = new PhongMaterial { DiffuseColor = Utils.ToColor4(Color) };

            // Apply to all MeshGeometryModel3D children
            foreach (var mesh in Children.OfType<MeshGeometryModel3D>())
                mesh.Material = mat;
        }
        private void UpdateTransform()
        {
            Transform = new TranslateTransform3D(Position.X, Position.Y, Position.Z);
        }

        protected override void OnMouse3DDown(object? sender, RoutedEventArgs e)
        {
            if (e is not Mouse3DEventArgs args)
                return;

            if (args.Viewport is null)
                return;

            Selected = !Selected;

            //ShowContextMenuTest();
        }

        private void ShowContextMenuTest()
        {
            // Show context menu at mouse position
            var menu = new ContextMenu();

            menu.Items.Add(new MenuItem
            {
                Header = "Edit"
            });
            menu.Items.Add(new MenuItem
            {
                Header = "Delete"
            });

            menu.IsOpen = true;
        }
    }
}
