using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace FEA_Program.Views
{
    /// <summary>
    /// Visual for a floor plane grid
    /// </summary>
    internal class GridVisual3D : GroupElement3D
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            nameof(Color),
            typeof(Color),
            typeof(GridVisual3D),
            new PropertyMetadata(Colors.Gray, OnColorChanged));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }


        public LineGeometryModel3D GridLines { get; private set; }

        public GridVisual3D()
        {
            // Create the lines
            LineGeometry3D gridGeometry = LineBuilder.GenerateGrid(Vector3.UnitY, -5, 5);

            GridLines = new LineGeometryModel3D()
            {
                Color = Color,
                Geometry = gridGeometry
            };

            Children.Add(GridLines);
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GridVisual3D visual)
                visual.GridLines.Color = visual.Color;
        }
    }
}
