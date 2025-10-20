using FEA_Program.ViewModels.Base;
using System.ComponentModel;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace FEA_Program.ViewModels
{
    internal class ElementDrawVM: ObservableObject
    {
        public static Color SelectedColor = Colors.Yellow;
        public static Color DefaultElementColor = Colors.LightGray;
        
        // ---------------------- Properties ----------------------
        public NodeDrawVM[] Nodes { get; private set; } = [];
        public ElementVM Element { get; private set; } = new();

        public Color ElementColor => Element.Selected ? SelectedColor : DefaultElementColor;

        // ---------------------- Commands ----------------------

        public ElementDrawVM() { }

        public ElementDrawVM(ElementVM element, NodeDrawVM[] nodes)
        {
            Nodes = nodes;
            Element = element;
            Element.PropertyChanged += OnElementPropertyChanged;
        }

        private void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ElementVM element)
            {
                if (e.PropertyName == (nameof(ElementVM.Selected)))
                {
                    OnPropertyChanged(nameof(ElementColor));
                }
            }
        }
    }
}
