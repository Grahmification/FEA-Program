using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Used for displaying a list of nodes with checkboxes
    /// </summary>
    /// <param name="node">The node to diplay</param>
    internal class NodeSelectVM : ObservableObject
    {
        public NodeVM Node { get; private set; }
        public bool IsSelected { get; set; } = false;

        public NodeSelectVM(NodeVM node)
        {
            Node = node;
            PropertyChanged += OnThisPropertyChanged;
            Node.PropertyChanged += OnNodePropertyChanged;
        }
        private void OnThisPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSelected))
            {
                // Select the node if the box is checked
                Node.Selected = IsSelected;
            }
        }
        private void OnNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is NodeVM vm)
            {
                if (e.PropertyName == nameof(NodeVM.Selected))
                {
                    // Select the box if the node has been selected
                    IsSelected = vm.Selected;
                }
            }
        }
    }

    /// <summary>
    /// Viewmodel for editing node forces
    /// </summary>
    internal class ForceEditVM: ObservableObject
    {
        private NodeVM? _inputItem;
            
        // ---------------------- Properties ----------------------

        /// <summary>
        /// Components of the force being edited
        /// </summary>
        public ObservableCollection<CoordinateVM> ForceComponents { get; } = [];

        /// <summary>
        /// All available nodes when adding a new force
        /// </summary>
        public ObservableCollection<NodeSelectVM> Nodes { get; private set; } = [];

        /// <summary>
        /// Node being edited
        /// </summary>
        public NodeVM? EditItem { get; private set; } = null;

        /// <summary>
        /// True if editing an existing item
        /// </summary>
        public bool Editing { get; private set; } = false;

        /// <summary>
        /// Whether to show the editor
        /// </summary>
        public bool? ShowEditor { get; private set; } = null;

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; set; } = new();

        /// <summary>
        /// Handles node and element selection
        /// </summary>
        public SelectionVM SelectionManager { get; set; } = new();


        // ---------------------- Commands ----------------------
        public ICommand AcceptCommand { get; }
        public ICommand CancelCommand { get; }

        // ---------------------- Public Methods ----------------------

        public ForceEditVM() 
        {
            AcceptCommand = new RelayCommand(AcceptEdit);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        /// <summary>
        /// Display the editor when adding a new force
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="dimension"></param>
        public void DisplayEditorAdd(List<NodeVM> nodes, int dimension)
        {
            SelectionManager.AllowMultiSelect = true;
            SelectionManager.DeselectAll();
            Base.ClearStatus();
            ResetItems();
            Editing = false;

            foreach (var node in nodes)
                Nodes.Add(new(node));

            for (int i = 0; i < dimension; i++)
            {
                var coordVM = new CoordinateVM(i, 0, false);
                coordVM.ValueChanged += OnCoordinateValueChanged;
                ForceComponents.Add(coordVM);
            }

            ShowEditor = nodes.Count > 0;
        }

        /// <summary>
        /// Display the editor when editing an existing force
        /// </summary>
        /// <param name="editItem"></param>
        public void DisplayEditor(NodeVM editItem)
        {
            Base.ClearStatus();
            ResetItems();

            // If we're editing, select the node
            editItem.Selected = true;
            _inputItem = editItem;

            // Make this so we're editing a copy. The original is preserved in case we cancel
            EditItem = new NodeVM((Node)editItem.Model.Clone());
            Editing = true;

            for (int i = 0; i < EditItem.Model.Dimension; i++)
            {
                var coordVM = new CoordinateVM(i, EditItem.Force[i], false);
                coordVM.ValueChanged += OnCoordinateValueChanged;
                ForceComponents.Add(coordVM);
            }

            ShowEditor = true;
        }
        public void HideEditor()
        {
            SelectionManager.DeselectAll();
            SelectionManager.AllowMultiSelect = false;
            ShowEditor = null;  // Do this instead of false because of how converter is setup
        }

        // ---------------------- Event Handers ----------------------

        /// <summary>
        /// Fires when one of the coordinates is updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCoordinateValueChanged(object? sender, int e)
        {
            if (sender is CoordinateVM vm && EditItem is not null)
            {
                EditItem.Model.Force[e] = App.Units.Force.FromUser(vm.Value); 
            }
        }

        // ---------------------- Private Helpers ----------------------
        private void ResetItems()
        {
            Nodes.Clear();
            ForceComponents.Clear();
            _inputItem = null;
            EditItem = null;
        }
        private void AcceptEdit()
        {
            try
            {
                double[] force = ForceComponents.Select(component => App.Units.Force.FromUser(component.Value)).ToArray();

                // We're editing, update the changed parameters
                if (Editing && _inputItem != null)
                {
                    _inputItem.Model.Force = force;
                }
                // We're adding new forces
                else if (!Editing)
                {
                    // Set the edited forces for each checked node
                    foreach (var vm in Nodes.Where(vm => vm.IsSelected))
                    {
                        vm.Node.Model.Force = force;
                    }

                    Base.SetStatus("Added force");
                }

                HideEditor(); // Do this after the event in case an error occurs
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }
        private void CancelEdit()
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

    }
}
