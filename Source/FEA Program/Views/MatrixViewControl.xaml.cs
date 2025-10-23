using UserControl = System.Windows.Controls.UserControl;
using Brushes = System.Windows.Media.Brushes;
using MathNet.Numerics.LinearAlgebra;
using System.Windows.Controls;
using System.Windows;

namespace FEA_Program.Views
{
    /// <summary>
    /// Interaction logic for MatrixViewControl.xaml
    /// </summary>
    public partial class MatrixViewControl : UserControl
    {
        public MatrixViewControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MatrixProperty = DependencyProperty.Register(
            nameof(Matrix), typeof(Matrix<double>), typeof(MatrixViewControl),
            new PropertyMetadata(null, OnMatrixChanged));

        public Matrix<double>? Matrix
        {
            get => (Matrix<double>?)GetValue(MatrixProperty);
            set => SetValue(MatrixProperty, value);
        }

        private static void OnMatrixChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MatrixViewControl view)
                view.BuildMatrixGrid();
        }

        private void BuildMatrixGrid()
        {
            MatrixGrid.Children.Clear();
            MatrixGrid.RowDefinitions.Clear();
            MatrixGrid.ColumnDefinitions.Clear();

            if (Matrix == null)
                return;

            int rowCount = Matrix.RowCount;
            int colCount = Matrix.ColumnCount;

            // Create columns (including index header)
            for (int c = 0; c <= colCount; c++)
                MatrixGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Create rows (including index header)
            for (int r = 0; r <= rowCount; r++)
                MatrixGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Top-left corner cell (empty)
            var corner = new TextBlock
            {
                Text = "",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(4),
                TextAlignment = TextAlignment.Center
            };
            Grid.SetRow(corner, 0);
            Grid.SetColumn(corner, 0);
            MatrixGrid.Children.Add(corner);

            // Column headers
            for (int c = 0; c < colCount; c++)
            {
                var header = new TextBlock
                {
                    Text = c.ToString(),
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(4),
                    TextAlignment = TextAlignment.Center
                };
                Grid.SetRow(header, 0);
                Grid.SetColumn(header, c + 1);
                MatrixGrid.Children.Add(header);
            }

            // Row headers
            for (int r = 0; r < rowCount; r++)
            {
                var header = new TextBlock
                {
                    Text = r.ToString(),
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(4),
                    TextAlignment = TextAlignment.Center
                };
                Grid.SetRow(header, r + 1);
                Grid.SetColumn(header, 0);
                MatrixGrid.Children.Add(header);
            }

            // Matrix values
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    double value = Matrix[r, c];
                    var cell = new Border
                    {
                        BorderBrush = Brushes.LightGray,
                        BorderThickness = new Thickness(0.5),
                        Background = value == 0.0 ? Brushes.White : Brushes.LightBlue,
                        Child = new TextBlock
                        {
                            Text = value.ToString("G3"),
                            Margin = new Thickness(4),
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        }
                    };
                    Grid.SetRow(cell, r + 1);
                    Grid.SetColumn(cell, c + 1);
                    MatrixGrid.Children.Add(cell);
                }
            }
        }
    }
}
