using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Media;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Color = System.Windows.Media.Color;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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

        public static readonly DependencyProperty BlockInteractionProperty = DependencyProperty.Register(
            nameof(BlockInteraction), typeof(bool), typeof(NodeVisual3D), new PropertyMetadata(false));

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

        public bool BlockInteraction
        {
            get => (bool)GetValue(BlockInteractionProperty);
            set => SetValue(BlockInteractionProperty, value);
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

            if (BlockInteraction)
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
