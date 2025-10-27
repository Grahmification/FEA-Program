using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;

namespace FEA_Program.Views.Helix
{
    /// <summary>
    /// A visual that displays text above all other elements
    /// </summary>
    internal class TextVisual3D : TopMostGroup3D
    {
        private readonly BillboardSingleText3D _textGeometry;
        private readonly BillboardTextModel3D _textVisual;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                nameof(Text), typeof(string), typeof(TextVisual3D),
                new PropertyMetadata("", OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public TextVisual3D()
        {
            // Create text
            _textGeometry = new BillboardSingleText3D()
            {
                FontColor = new Color4(0, 0, 0, 1),
                FontSize = 36,
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

            Children.Add(_textVisual);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextVisual3D group)
                group.UpdateText();
        }

        private void UpdateText()
        {
            _textGeometry.TextInfo = new TextInfo(Text, new Vector3(0.9f, 0.9f, 0.9f)) { Scale = 0.01f };
            _textVisual.Visibility = Text == "" ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
