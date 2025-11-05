using FEA_Program.Converters;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Defines different coloring schemes for drawn elements
    /// </summary>
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

    /// <summary>
    /// Defines different coloring schemes for drawn nodes
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    internal enum NodeColorSchemes
    {
        [Description("Default")]
        None,

        [Description("Max Displacement")]
        MaxDisplacement,
    }

    /// <summary>
    /// Manages drawing items in the 3D view window
    /// </summary>
    internal class DrawVM: ObservableObject
    {
        // Reference drawn elements by ID for easy lookup
        private readonly Dictionary<int, ElementDrawVM> _Elements = [];
        private readonly Dictionary<int, NodeDrawVM> _Nodes = [];

        // ---------------------- Sub VMs ----------------------

        /// <summary>
        /// Elements to draw in the 3D view
        /// </summary>
        public ObservableCollection<ElementDrawVM> Elements { get; private set; } = [];

        /// <summary>
        /// Nodes to draw in the 3D view
        /// </summary>
        public ObservableCollection<NodeDrawVM> Nodes { get; private set; } = [];

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; set; } = new();

        // ---------------------- Node Properties ----------------------

        private bool _DrawDisplaced = false;
        private double _DisplacePercentage = 0;
        private double _DisplaceScaling = 10000;
        private bool _DrawReactions = false;
        private NodeColorSchemes _NodeColorScheme = NodeColorSchemes.None;

        /// <summary>
        /// Whether to draw nodes at their displaced position
        /// </summary>
        public bool DrawDisplaced { get => _DrawDisplaced; set { _DrawDisplaced = value; UpdateNodeScaling(); } }

        /// <summary>
        /// What percentage of displacement to draw displaced nodes at
        /// </summary>
        public double DisplacePercentage { get => _DisplacePercentage; set { _DisplacePercentage = value; UpdateNodeScaling(); } }

        /// <summary>
        /// General scaling factor for node displacement
        /// </summary>
        public double DisplaceScaling { get => _DisplaceScaling; set { _DisplaceScaling = value; UpdateNodeScaling(); } }

        /// <summary>
        /// Whether to draw node reaction forces
        /// </summary>
        public bool DrawReactions { get => _DrawReactions; set { _DrawReactions = value; UpdateNodeReactions(); } }

        /// <summary>
        /// The active node coloring scheme in the 3D view
        /// </summary>
        public NodeColorSchemes NodeColorScheme { get => _NodeColorScheme; set { _NodeColorScheme = value; UpdateNodeColors(); } }

        // ---------------------- Element Properties ----------------------

        private ElementColorSchemes _ElementColorScheme = ElementColorSchemes.None;

        /// <summary>
        /// The active element coloring scheme in the 3D view
        /// </summary>
        public ElementColorSchemes ElementColorScheme { get => _ElementColorScheme; set { _ElementColorScheme = value; UpdateElementColors(); } }

        // ---------------------- Public Methods ----------------------
        public DrawVM() { }

        /// <summary>
        /// Adds a node to be drawn
        /// </summary>
        /// <param name="node">The node to draw</param>
        /// <param name="pending">Whether the node is pending (not yet added to the problem)</param>
        public void AddNode(NodeVM node, bool pending = false) 
        {
            var vm = new NodeDrawVM(node, pending);
            int id = node.Model.ID;

            // This node already exists. Replace it
            if (_Nodes.TryGetValue(id, out var existing))
                Nodes.Remove(existing);

            _Nodes[id] = vm;
            Nodes.Add(vm);
        }

        /// <summary>
        /// Adds an element to be drawn
        /// </summary>
        /// <param name="element">The element to draw</param>
        public void AddElement(ElementVM element)
        {
            if (element.Model != null)
            {
                // Get nodes corresponding to the element
                NodeDrawVM[] nodes = element.NodeIds
                    .Select(id => _Nodes[id])
                    .ToArray();

                var vm = new ElementDrawVM(element, nodes);
                int elementId = element.Model.ID;

                // This item already exists. Replace it
                if (_Elements.TryGetValue(elementId, out var existing))
                    Elements.Remove(existing);

                _Elements.Add(elementId, vm);
                Elements.Add(vm);
            }
        }

        /// <summary>
        /// Removes a node from being drawn
        /// </summary>
        /// <param name="id">ID of the node to remove</param>
        public void RemoveNode(int id)
        {
            if (_Nodes.TryGetValue(id, out var vm))
            {
                Nodes.Remove(vm);
                _Nodes.Remove(id);
            }
        }

        /// <summary>
        /// Removes an element from being drawn
        /// </summary>
        /// <param name="id">ID of the element to remove</param>
        public void RemoveElement(int id)
        {
            if (_Elements.TryGetValue(id, out var vm))
            {
                Elements.Remove(vm);
                _Elements.Remove(id);
            }
        }

        /// <summary>
        /// Reset all elements and nodes in the VM
        /// </summary>
        public void ResetCollections()
        {
            _Elements.Clear();
            Elements.Clear();
            _Nodes.Clear();
            Nodes.Clear();
        }

        /// <summary>
        /// Apply all active settings, redrawing all elements and nodes
        /// </summary>
        public void ApplySettings()
        {
            UpdateNodeScaling();
            UpdateNodeReactions();
            UpdateElementColors();
            UpdateNodeColors();
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
        /// Update the coloring scheme for all nodes
        /// </summary>
        private void UpdateNodeColors()
        {
            try
            {
                if (NodeColorScheme == NodeColorSchemes.MaxDisplacement)
                {
                    NodeDrawVM.ApplyDisplacementColors(Nodes);
                }
                else
                {
                    foreach (var e in Nodes)
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
