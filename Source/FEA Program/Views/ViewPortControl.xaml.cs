using FEA_Program.ViewModels;
using HelixToolkit.Wpf.SharpDX;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;


namespace FEA_Program.Views
{
    /// <summary>
    /// Interaction logic for ViewPortControl.xaml
    /// </summary>
    public partial class ViewPortControl : UserControl
    {
        /// <summary>
        /// Context menu that displays when the scene background is right clicked
        /// </summary>
        public static readonly DependencyProperty BackgroundContextMenuProperty = DependencyProperty.Register(
            nameof(BackgroundContextMenu), typeof(ContextMenu), typeof(ViewPortControl),
            new PropertyMetadata(null)); // Default value is null

        /// <summary>
        /// Context menu that displays when the scene background is right clicked
        /// </summary>
        public ContextMenu? BackgroundContextMenu
        {
            get { return (ContextMenu)GetValue(BackgroundContextMenuProperty); }
            set { SetValue(BackgroundContextMenuProperty, value); }
        }


        public ViewPortControl()
        {
            InitializeComponent();
            SetupInputBindings();

            // Ensure the collection exists by default
            DrawItems = [];

            Loaded += OnLoaded;
            viewPort.MouseRightButtonDown += OnMouseRightButtonDown;
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

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Viewport3DX viewport)
            {
                // Get the mouse position relative to the Viewport3DX
                var position = Mouse.GetPosition(viewport);

                // Perform a hit test at the mouse position
                var hitResult = viewport.FindHits(position).FirstOrDefault();

                // Nothing is selected (we're over the background)
                if (hitResult == null)
                {
                    // The background is selected - display the context menu
                    if (BackgroundContextMenu != null)
                    {
                        BackgroundContextMenu.Placement = PlacementMode.MousePoint;
                        BackgroundContextMenu.IsOpen = true;
                    }
                }
            }
        }
    }
}
