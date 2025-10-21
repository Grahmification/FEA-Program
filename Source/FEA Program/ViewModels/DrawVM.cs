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

        // ---------------------- Displacement Properties ----------------------

        private bool _DrawDisplaced = false;
        private double _DisplacePercentage = 0;
        private double _DisplaceScaling = 10000;

        public bool DrawDisplaced { get => _DrawDisplaced; set { _DrawDisplaced = value; UpdateNodeScaling(); } }
        public double DisplacePercentage { get => _DisplacePercentage; set { _DisplacePercentage = value; UpdateNodeScaling(); } }
        public double DisplaceScaling { get => _DisplaceScaling; set { _DisplaceScaling = value; UpdateNodeScaling(); } }

        // ---------------------- Element Properties ----------------------

        private bool _DrawElementStressColors = false;
        public bool DrawElementStressColors { get => _DrawElementStressColors; set { _DrawElementStressColors = value; UpdateElementColors(); } }


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

        // ---------------------- Private Helpers ----------------------

        /// <summary>
        /// Update internal value of displace scaling on all the nodes
        /// </summary>
        private void UpdateNodeScaling()
        {
            double nodeDrawScaling = DrawDisplaced ? DisplaceScaling * DisplacePercentage / 100.0 : 0;

            foreach (NodeDrawVM N in Nodes)
                N.DisplacementScalingFactor = nodeDrawScaling;
        }

        /// <summary>
        /// Update element colors
        /// </summary>
        private void UpdateElementColors()
        {
            if (DrawElementStressColors)
            {
                ElementDrawVM.ApplyStressColors(Elements);
            }
            else
            {
                foreach(var e in Elements)
                {
                    // Set to default color
                    e.ColorOverride = null;
                }
            }
        }

    }
}
