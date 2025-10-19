using FEA_Program.ViewModels.Base;

namespace FEA_Program.ViewModels
{
    internal class ElementDrawVM: ObservableObject
    {
        // ---------------------- Properties ----------------------
        public NodeDrawVM[] Nodes { get; private set; } = [];
        public ElementVM Element { get; private set; } = new();

        // ---------------------- Commands ----------------------

        public ElementDrawVM() { }

        public ElementDrawVM(ElementVM element, NodeDrawVM[] nodes)
        {
            Element = element;
            Nodes = nodes;
        }
    }
}
