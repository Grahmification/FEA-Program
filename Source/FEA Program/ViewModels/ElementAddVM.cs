using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Helper ViewModel for a single Node selection
    /// </summary>
    internal class NodeSelectionVM : ObservableObject
    {
        private NodeVM? _SelectedNode;

        public event EventHandler? SelectionChanged;
        public event EventHandler? SelectionChanging;

        public int NodeNumber { get; }
        public ObservableCollection<NodeVM> AvailableNodes { get; }
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
        private int _NewID = 0;

        private ObservableCollection<NodeVM> _nodes = [];

        private int NumberOfNodes => SelectedElementType == null ? 0 : ElementVM.NumOfNodes(SelectedElementType.Value);

        // ---------------------- Events ----------------------
        public event EventHandler<ElementVM>? AcceptEdits;

        /// <summary>
        /// Fires when the sidebar control is opening
        /// </summary>
        public event EventHandler? Opening;

        /// <summary>
        /// Fires when the sidebar control has closed
        /// </summary>
        public event EventHandler? Closed;

        // ---------------------- Models ----------------------
        /// <summary>
        /// Whether to show the editor
        /// </summary>
        public bool ShowEditor { get; private set; } = false;
        public ObservableCollection<ElementTypes> AvailableElementTypes { get; set; } = [];
        public ElementTypes? SelectedElementType { get; set; } = null;
        public ObservableCollection<MaterialVM> Materials { get; private set; } = [];
        public MaterialVM? SelectedMaterial { get; set; } = new();

        // Properties that depend on element type selection
        public ObservableCollection<NodeSelectionVM> NodeSelectors { get; private set; } = [];
        public ObservableCollection<ElementArgVM> ElementArguments { get; private set; } = [];


        // Validation properties
        public bool CanCreateElement => NodeSelectionValid & SelectedMaterial != null & ArgumentsValid;
        public bool NodeSelectionValid { get; private set; } = false;
        public bool ArgumentsValid => ElementArguments.Count == 0 || ElementArguments.All(arg => arg.ValueValid);

        // ---------------------- Sub VMs ----------------------
        public BaseVM Base { get; set; } = new();

        public SelectionVM SelectionManager { get; set; } = new();

        // ---------------------- Commands ----------------------
        public ICommand AcceptCommand { get; }
        public ICommand CancelCommand { get; }

        // ---------------------- Public Methods ----------------------
        public ElementAddVM()
        {
            AcceptCommand = new RelayCommand(AcceptEdit, () => CanCreateElement);
            CancelCommand = new RelayCommand(CancelEdit);
            PropertyChanged += OnThisPropertyChanged;
        }

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

        private void OnNodeSelectorValueChanging(object? sender, EventArgs e)
        {
            if (sender is NodeSelectionVM vm)
            {
                // Deselect the old node without firing the OnNodeSelectionChanged method
                SelectNodeWithoutEvent(vm.SelectedNode, false);
            }
        }
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
        /// Fires when a NodeVM's selected property was changed
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
