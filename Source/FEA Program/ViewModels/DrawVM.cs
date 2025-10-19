using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;

namespace FEA_Program.ViewModels
{
    internal class DrawVM: ObservableObject
    {
        // Reference by ID for easy lookup
        private readonly Dictionary<int, ElementDrawVM> _Elements = [];
        private readonly Dictionary<int, NodeDrawVM> _Nodes = [];

        // ---------------------- Sub VMs ----------------------
        public ObservableCollection<ElementDrawVM> Elements { get; private set; } = [];
        public ObservableCollection<NodeDrawVM> Nodes { get; private set; } = [];

        // ---------------------- Public Methods ----------------------
        public DrawVM() { }

        public void AddNode(NodeVM node) 
        {
            var vm = new NodeDrawVM(node);
            _Nodes.Add(node.Model.ID, vm);
            Nodes.Add(vm);
        }
        public void AddElement(ElementVM element)
        {
            if (element.Model != null)
            {
                // Get nodes corresponding to the element
                NodeDrawVM[] nodes = element.NodeIds
                    .Select(id => _Nodes[id])
                    .ToArray();

                var vm = new ElementDrawVM(element, nodes);
                _Elements.Add(element.Model.ID, vm);
                Elements.Add(vm);
            }
        }
        public void RemoveNode(int id)
        {
            var vm = _Nodes[id];
            Nodes.Remove(vm);
            _Nodes.Remove(id);
        }
        public void RemoveElement(int id)
        {
            var vm = _Elements[id];
            Elements.Remove(vm);
            _Elements.Remove(id);
        }

    }
}
