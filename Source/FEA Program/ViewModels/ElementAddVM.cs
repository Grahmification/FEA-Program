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
        public event EventHandler? SelectionChanged;

        public int NodeNumber { get; }
        public ObservableCollection<NodeVM> AvailableNodes { get; }
        public NodeVM? SelectedNode { get; set; } = null;

        public NodeSelectionVM(int nodeNumber, ObservableCollection<NodeVM> availableNodes, int selectedIndex = 0)
        {
            NodeNumber = nodeNumber;
            AvailableNodes = availableNodes;

            if(availableNodes.Count > selectedIndex)
                SelectedNode = AvailableNodes[selectedIndex];
            else if(availableNodes.Count > 0)
                SelectedNode = AvailableNodes[0];

            PropertyChanged += OnThisPropertyChanged;
        }
        private void OnThisPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedNode))
            {
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
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
    internal class ElementAddVM: ObservableObject
    {
        private int _NewID = 0;

        private ObservableCollection<NodeVM> _nodes = [];

        private int NumberOfNodes => SelectedElementType == null ? 0 : ElementVM.NumOfNodes(SelectedElementType.Value);

        // ---------------------- Events ----------------------
        public event EventHandler<ElementVM>? AcceptEdits;

        // ---------------------- Models ----------------------
        /// <summary>
        /// Whether to show the editor
        /// </summary>
        public bool? ShowEditor { get; private set; } = null;
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
            CancelCommand = new RelayCommand(HideEditor);
            PropertyChanged += OnThisPropertyChanged;
        }

        public void DisplayEditor(int newID, ObservableCollection<MaterialVM> materials, ObservableCollection<NodeVM> nodes)
        {
            SelectionManager.AllowMultiSelect = true;
            
            // Reset fields
            SelectedElementType = null;
            SelectedMaterial = null;

            _NewID = newID;
            Materials = materials;
            _nodes = nodes;
            SelectedElementType = AvailableElementTypes.FirstOrDefault();
            SelectedMaterial = Materials.FirstOrDefault() ?? null;

            // Make sure validation updates
            OnNodeSelectionChanged(this, EventArgs.Empty);

            ShowEditor = true;
        }
        public void HideEditor()
        {
            // We're closing, deselect all nodes
            SelectionManager.AllowMultiSelect = false;
            SelectionManager.DeselectAll();

            ShowEditor = null;  // Do this instead of false because of how converter is setup
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
                var selector = new NodeSelectionVM(i, _nodes, i);
                selector.SelectionChanged += OnNodeSelectionChanged;
                NodeSelectors.Add(selector);
            }

            // Update list of element arguments
            ElementArguments = SelectedElementType == null ? [] : new(ElementVM.ElementArgs(SelectedElementType.Value));
        }
        private void OnNodeSelectionChanged(object? sender, EventArgs e)
        {
            // Deselect all, then select the ones which matter
            SelectionManager.DeselectAll();

            foreach (var selector in NodeSelectors)
                if (selector.SelectedNode != null)
                {
                    selector.SelectedNode.Selected = true;
                }

            NodeSelectionValid = NodeSelectionVM.CheckForValidSelections(NodeSelectors);
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
                    case ElementTypes.BarLinear:
                        element = new ElementBarLinear(1, _NewID, [.. elementNodes.Select(n => n.Model).Cast<INode>()], elementMaterial.Model, elementNodes[0].Model.Dimension);
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

    }
}
