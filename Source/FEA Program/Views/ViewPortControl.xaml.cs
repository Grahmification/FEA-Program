using FEA_Program.ViewModels;
using HelixToolkit.Wpf.SharpDX;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


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
            SetupInputBindings();

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

        /// <summary>
        /// Setup key and mouse bindings for the view
        /// </summary>
        private void SetupInputBindings()
        {
            // Clear the default input bindings - this can't be done in .xaml code
            viewPort.InputBindings.Clear();

            // Mouse commands
            viewPort.InputBindings.Add(new MouseBinding(ViewportCommands.Rotate, new MouseGesture(MouseAction.MiddleClick)));
            viewPort.InputBindings.Add(new MouseBinding(ViewportCommands.Pan, new MouseGesture(MouseAction.MiddleClick, ModifierKeys.Shift)));
            // Note - Zoom gets taken care of automatically

            // Keyboard commands
            viewPort.InputBindings.Add(new KeyBinding(ViewportCommands.ZoomExtents, new KeyGesture(Key.E, ModifierKeys.Control)));
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
