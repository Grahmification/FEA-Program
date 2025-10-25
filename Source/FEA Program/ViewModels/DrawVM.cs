using FEA_Program.Converters;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FEA_Program.ViewModels
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    internal enum ElementColorSchemes
    {
        [Description("Default")]
        None,

        [Description("Max Stress")]
        MaxStress,

        [Description("Yield Safety Factor")]
        SafetyFactorYield,

        [Description("Ultimate Safety Factor")]
        SafetyFactorUltimate,
    }
    
    
    
    internal class DrawVM: ObservableObject
    {
        // Reference by ID for easy lookup
        private readonly Dictionary<int, ElementDrawVM> _Elements = [];
        private readonly Dictionary<int, NodeDrawVM> _Nodes = [];

        // ---------------------- Sub VMs ----------------------
        public ObservableCollection<ElementDrawVM> Elements { get; private set; } = [];
        public ObservableCollection<NodeDrawVM> Nodes { get; private set; } = [];

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; set; } = new();

        // ---------------------- Node Properties ----------------------

        private bool _DrawDisplaced = false;
        private double _DisplacePercentage = 0;
        private double _DisplaceScaling = 10000;

        public bool DrawDisplaced { get => _DrawDisplaced; set { _DrawDisplaced = value; UpdateNodeScaling(); } }
        public double DisplacePercentage { get => _DisplacePercentage; set { _DisplacePercentage = value; UpdateNodeScaling(); } }
        public double DisplaceScaling { get => _DisplaceScaling; set { _DisplaceScaling = value; UpdateNodeScaling(); } }


        private bool _DrawReactions = false;
        public bool DrawReactions { get => _DrawReactions; set { _DrawReactions = value; UpdateNodeReactions(); } }

        // ---------------------- Element Properties ----------------------

        private ElementColorSchemes _ElementColorScheme = ElementColorSchemes.None;
        public ElementColorSchemes ElementColorScheme { get => _ElementColorScheme; set { _ElementColorScheme = value; UpdateElementColors(); } }


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
            try
            {
                double nodeDrawScaling = DrawDisplaced ? DisplaceScaling * DisplacePercentage / 100.0 : 0;

                foreach (NodeDrawVM N in Nodes)
                    N.DisplacementScalingFactor = nodeDrawScaling;
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Update reaction force display for all nodes
        /// </summary>
        private void UpdateNodeReactions()
        {
            try
            {
                if (DrawReactions)
                {
                    NodeDrawVM.DrawReactionForces(Nodes, 2, 0.6);
                }
                else
                {
                    foreach (NodeDrawVM N in Nodes)
                        N.ReactionForces.Clear();
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Update element colors
        /// </summary>
        private void UpdateElementColors()
        {
            try
            {
                if(ElementColorScheme == ElementColorSchemes.MaxStress)
                {
                    ElementDrawVM.ApplyStressColors(Elements);
                }
                else if (ElementColorScheme == ElementColorSchemes.SafetyFactorUltimate)
                {
                    ElementDrawVM.ApplySafetyFactorColors(Elements, 10, false);
                }
                else if (ElementColorScheme == ElementColorSchemes.SafetyFactorYield)
                {
                    ElementDrawVM.ApplySafetyFactorColors(Elements, 10, true);
                }
                else
                {
                    foreach (var e in Elements)
                    {
                        // Set to default color
                        e.ColorOverride = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

    }
}
