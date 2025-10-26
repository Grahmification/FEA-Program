using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Media;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using Color = System.Windows.Media.Color;

namespace FEA_Program.Views.Helix
{
    internal class NodeVisual3D : GroupModel3D
    {
        private readonly BillboardSingleText3D _textGeometry;
        private readonly BillboardTextModel3D _textVisual;

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color), typeof(Color), typeof(NodeVisual3D),
            new PropertyMetadata(Colors.Green, OnColorChanged));

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
                nameof(Position), typeof(Vector3), typeof(NodeVisual3D),
                new PropertyMetadata(new Vector3(), OnPositionChanged));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                nameof(Text), typeof(string), typeof(NodeVisual3D),
                new PropertyMetadata("", OnTextChanged));

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

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
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

            // Create node text
            _textGeometry = new BillboardSingleText3D()
            { 
                FontColor = new Color4(0, 0, 0, 1),
                FontSize = 12,
                FontWeight = FontWeights.Regular,
                BackgroundColor = new Color4(0.8f, 0.8f, 0.8f, 0.5f),
                Padding = new Thickness(2),
            };

            _textVisual = new BillboardTextModel3D
            {
                Geometry = _textGeometry,
                Visibility = Visibility.Hidden, // Hide until explicitly shown
                IsHitTestVisible = false,  // Not clickable
                FixedSize = false,  // Prevents scaling with zoom 
            };

            // Keeps the text on top
            var topVisual = new TopMostGroup3D
            {
                IsHitTestVisible = false
            };

            topVisual.Children.Add(_textVisual);
            Children.Add(topVisual);
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

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NodeVisual3D group)
                group.UpdateText();
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

        private void UpdateText()
        {
            _textGeometry.TextInfo = new TextInfo(Text, new Vector3(0.9f, 0.9f, 0.9f)) { Scale = 0.03f };
            _textVisual.Visibility = Text == "" ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
