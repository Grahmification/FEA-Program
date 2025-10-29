using FEA_Program.ViewModels;
using HelixToolkit.Wpf.SharpDX;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace FEA_Program.Views
{
    /// <summary>
    /// Interaction logic for ViewPortControl.xaml
    /// </summary>
    public partial class ViewPortControl : UserControl
    {
        public ViewPortControl()
        {
            InitializeComponent();

            // Ensure the collection exists by default
            DrawItems = [];

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            if (DataContext is ViewPortVM viewModel)
            {
                viewModel.ZoomExtentsRequested += ViewModel_ZoomRequested;
            }
        }


        public static readonly DependencyProperty DrawItemsProperty =
            DependencyProperty.Register(nameof(DrawItems),
                typeof(ObservableCollection<Element3D>),
                typeof(ViewPortControl),
                new PropertyMetadata(null));

        public ObservableCollection<Element3D> DrawItems
        {
            get => (ObservableCollection<Element3D>)GetValue(DrawItemsProperty);
            set => SetValue(DrawItemsProperty, value);
        }

        private void ViewModel_ZoomRequested(object? sender, EventArgs e)
        {
            viewPort.ZoomExtents();
        }
    }
}
