using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Helper ViewModel for selecting a node from a dropdown menu
    /// </summary>
    internal class NodeSelectionVM : ObservableObject
    {
        private NodeVM? _SelectedNode;

        /// <summary>
        /// Fires when the selection has changed, but before the value has been updated
        /// </summary>
        public event EventHandler? SelectionChanging;

        /// <summary>
        /// Fires after the selection and value has changed
        /// </summary>
        public event EventHandler? SelectionChanged;
        
        /// <summary>
        /// The node number in the element
        /// </summary>
        public int NodeNumber { get; }

        /// <summary>
        /// The list of nodes to select from
        /// </summary>
        public ObservableCollection<NodeVM> AvailableNodes { get; }

        /// <summary>
        /// The currently selected node, or null for nothing
        /// </summary>
        public NodeVM? SelectedNode {
            get => _SelectedNode;
            set
            {
                // Only if the value has changed to prevent raising excessive events
                if (value != _SelectedNode)
                {
                    SelectionChanging?.Invoke(this, EventArgs.Empty);
                    _SelectedNode = value;
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Create the VM
        /// </summary>
        /// <param name="nodeNumber">The node number in the element</param>
        /// <param name="availableNodes">The list of nodes to select from</param>
        public NodeSelectionVM(int nodeNumber, ObservableCollection<NodeVM> availableNodes)
        {
            NodeNumber = nodeNumber;
            AvailableNodes = availableNodes;
        }

        /// <summary>
        /// Check if a list of selectors has valid selections
        /// </summary>
        /// <param name="selectors"></param>
        /// <returns></returns>
        public static bool CheckForValidSelections(ObservableCollection<NodeSelectionVM> selectors)
        {
            // Check for null selections
            if (selectors.Where(s => s.SelectedNode is null).Any())
                return false;
            
            // Check for duplicate selections
            var selectedNodes = selectors
                .Where(s => s.SelectedNode != null)
                .Select(s => s.SelectedNode?.Model.ID)
                .ToList();

            // Check for duplicates: count selected IDs and see if any count is > 1
            return !selectedNodes.GroupBy(id => id).Any(g => g.Count() > 1);
        }
    }

    /// <summary>
    /// Viewmodel for an add element dialog
    /// </summary>
    internal class ElementAddVM: ObservableObject, ISideBarEditor
    {
        /// <summary>
        /// ID of the element to be created
        /// </summary>
        private int _NewID = 0;

        /// <summary>
        /// List of nodes in the problem
        /// </summary>
        private ObservableCollection<NodeVM> _nodes = [];

        /// <summary>
        /// The number of nodes required for the given type of element
        /// </summary>
        private int NumberOfNodes => SelectedElementType == null ? 0 : ElementVM.NumOfNodes(SelectedElementType.Value);

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when edits are accepted, with the new element as the argument
        /// </summary>
        public event EventHandler<ElementVM>? AcceptEdits;

        /// <summary>
        /// Fires when the sidebar control is opening
        /// </summary>
        public event EventHandler? Opening;

        /// <summary>
        /// Fires when the sidebar control has closed
        /// </summary>
        public event EventHandler? Closed;

        // ---------------------- Public Properties ----------------------

        /// <summary>
        /// Whether to show the editor
        /// </summary>
        public bool ShowEditor { get; private set; } = false;

        /// <summary>
        /// The list of element types which can be selected from for the given type of problem
        /// </summary>
        public ObservableCollection<ElementTypes> AvailableElementTypes { get; set; } = [];

        /// <summary>
        /// The currently selected element type
        /// </summary>
        public ElementTypes? SelectedElementType { get; set; } = null;

        /// <summary>
        /// The list of materials which can be selected for the element
        /// </summary>
        public ObservableCollection<MaterialVM> Materials { get; private set; } = [];

        /// <summary>
        /// The currently selected material
        /// </summary>
        public MaterialVM? SelectedMaterial { get; set; } = new();

        // Properties that depend on element type selection

        /// <summary>
        /// The list of node selection boxes for the given element type
        /// </summary>
        public ObservableCollection<NodeSelectionVM> NodeSelectors { get; private set; } = [];

        /// <summary>
        /// The list of arguments for the given element type
        /// </summary>
        public ObservableCollection<ElementArgVM> ElementArguments { get; private set; } = [];

        // Validation properties

        /// <summary>
        /// True if an element can be created (all fields are valid)
        /// </summary>
        public bool CanCreateElement => NodeSelectionValid & SelectedMaterial != null & ArgumentsValid;

        /// <summary>
        /// True if the node selections are valid for the given element type
        /// </summary>
        public bool NodeSelectionValid { get; private set; } = false;

        /// <summary>
        /// True if all element arguments are valid
        /// </summary>
        public bool ArgumentsValid => ElementArguments.Count == 0 || ElementArguments.All(arg => arg.ValueValid);

        // ---------------------- Sub VMs ----------------------

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; set; } = new();

        /// <summary>
        /// Manages selecting elements and nodes from various sources
        /// </summary>
        public SelectionVM SelectionManager { get; set; } = new();

        // ---------------------- Commands ----------------------

        /// <summary>
        /// Relay command to accept edits
        /// </summary>
        public ICommand AcceptCommand { get; }

        /// <summary>
        /// Relay command to cancel edits
        /// </summary>
        public ICommand CancelCommand { get; }

        // ---------------------- Public Methods ----------------------
        public ElementAddVM()
        {
            AcceptCommand = new RelayCommand(AcceptEdit, () => CanCreateElement);
            CancelCommand = new RelayCommand(CancelEdit);
            PropertyChanged += OnThisPropertyChanged;
        }

        /// <summary>
        /// Displays the editor
        /// </summary>
        /// <param name="newID">ID of the element to be created</param>
        /// <param name="materials">Available materials for the element</param>
        /// <param name="nodes">Available nodes for the element</param>
        public void DisplayEditor(int newID, ObservableCollection<MaterialVM> materials, ObservableCollection<NodeVM> nodes)
        {
            Opening?.Invoke(this, EventArgs.Empty);
            SelectionManager.AllowMultiSelect = true;

            foreach (var node in nodes)
                node.PropertyChanged += OnNodeSelectionChanged;

            
            // Reset fields
            SelectedElementType = null;
            SelectedMaterial = null;

            _NewID = newID;
            Materials = materials;
            _nodes = nodes;
            SelectedElementType = AvailableElementTypes.FirstOrDefault();
            SelectedMaterial = Materials.FirstOrDefault() ?? null;

            // Make sure validation updates
            OnNodeSelectorValueChanged(this, EventArgs.Empty);

            ShowEditor = true;
        }

        /// <summary>
        /// Cancels editing, hiding the editor
        /// </summary>
        public void CancelEdit()
        {
            try
            {
                HideEditor();
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        // ---------------------- Events ----------------------

        /// <summary>
        /// Called when a property in this class changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnThisPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(sender is ElementAddVM)
            {
                if(e.PropertyName == nameof(SelectedElementType))
                {
                    OnElementTypeChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Called when <see cref="SelectedElementType"/> changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnElementTypeChanged(object? sender, EventArgs e)
        {
            // Update list of nodes
            NodeSelectors = [];

            for (int i = 0; i < NumberOfNodes; i++)
            {
                var selector = new NodeSelectionVM(i, _nodes);
                selector.SelectionChanged += OnNodeSelectorValueChanged;
                selector.SelectionChanging += OnNodeSelectorValueChanging;

                NodeSelectors.Add(selector);
            }

            // Update list of element arguments
            ElementArguments = SelectedElementType == null ? [] : new(ElementVM.ElementArgs(SelectedElementType.Value));
        }

        /// <summary>
        /// Called when a node selector selection is changing, but before the value has updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNodeSelectorValueChanging(object? sender, EventArgs e)
        {
            if (sender is NodeSelectionVM vm)
            {
                // Deselect the old node without firing the OnNodeSelectionChanged method
                SelectNodeWithoutEvent(vm.SelectedNode, false);
            }
        }

        /// <summary>
        /// Called when a node selector selection has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNodeSelectorValueChanged(object? sender, EventArgs e)
        {
            if (sender is NodeSelectionVM vm)
            {
                // Select the new node without firing the OnNodeSelectionChanged method
                SelectNodeWithoutEvent(vm.SelectedNode, true);
            }
            NodeSelectionValid = NodeSelectionVM.CheckForValidSelections(NodeSelectors);
        }

        /// <summary>
        /// Called when a NodeVM's selected property was changed, typically from the 3D view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNodeSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is NodeVM vm && e.PropertyName == nameof(NodeVM.Selected))
            {
                // A node was selected
                if(vm.Selected)
                {
                    // If we have a null item, set that one first
                    foreach (var selector in NodeSelectors)
                    {
                        if (selector.SelectedNode == null)
                        {
                            selector.SelectedNode = vm;
                            return;
                        }
                    }

                    // No items are null, set the last one
                    if(NodeSelectors.Count > 0)
                    {
                        NodeSelectors.Last().SelectedNode = vm;
                    }
                }
                // A node was deselected
                else
                {
                    // Deselect any matching selectors
                    foreach(var selector in NodeSelectors)
                    {
                        if(selector.SelectedNode == vm)
                            selector.SelectedNode = null;
                    }
                }
            }
        }

        // ---------------------- Private Helpers ----------------------

        /// <summary>
        /// Accepts edits, creating the element and closing the editor
        /// </summary>
        /// <exception cref="ArgumentException">No material was selected</exception>
        private void AcceptEdit()
        {
            try
            {
                // Get nodes and material corresponding to element
                if (SelectedMaterial is null)
                    throw new ArgumentException("Cannot create element: No material selected.");

                var elementMaterial = SelectedMaterial;

                List<NodeVM> elementNodes = NodeSelectors
                .Where(selector => selector.SelectedNode != null)
                .Select(selector => selector.SelectedNode)
                .Cast<NodeVM>()
                .ToList();

                // Create the new Element
                IElement? element = null;

                switch (SelectedElementType)
                {
                    case ElementTypes.TrussLinear:
                        element = new ElementTrussLinear(1, _NewID, [.. elementNodes.Select(n => n.Model).Cast<INode>()], elementMaterial.Model, elementNodes[0].Model.Dimension);
                        break;
                }

                if (element is not null)
                {
                    // Apply variable arguments
                    element.ElementArgs = ElementArgVM.GetArgumentsArray(ElementArguments);

                    // Create the output viewmodel
                    var elementVM = new ElementVM(element, [.. elementNodes], elementMaterial);
                    AcceptEdits?.Invoke(this, elementVM);

                    HideEditor(); // Do this after the event in case an error occurs
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Hides the associated control
        /// </summary>
        private void HideEditor()
        {
            // We're closing, deselect all nodes
            SelectionManager.AllowMultiSelect = false;
            SelectionManager.DeselectAll();

            foreach (var node in _nodes)
                node.PropertyChanged -= OnNodeSelectionChanged;

            ShowEditor = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Select or deselect a node without calling the SelectionChanged handler
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="select"></param>
        private void SelectNodeWithoutEvent(NodeVM? nodeVM, bool select)
        {
            if (nodeVM is not null)
            {
                nodeVM.PropertyChanged -= OnNodeSelectionChanged;
                nodeVM.Selected = select;
                nodeVM.PropertyChanged += OnNodeSelectionChanged;
            }
        }

    }
}
