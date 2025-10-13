using HelixToolkit.Wpf.SharpDX;
using System.Collections.ObjectModel;
using System.Windows;

namespace FEA_Program.Views
{
    /// <summary>
    /// Interaction logic for ViewPortControl.xaml
    /// </summary>
    public partial class ViewPortControl : System.Windows.Controls.UserControl
    {
        public ViewPortControl()
        {
            InitializeComponent();

            // Ensure the collection exists by default
            DrawItems = [];
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
    }
}
