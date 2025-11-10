using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for a control to edit or add a node
    /// </summary>
    internal class NodeEditVM: ObservableObject, ISideBarEditor
    {
        /// <summary>
        /// The original item input when the editor was opened
        /// </summary>
        private NodeVM? _inputItem;

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when edits are accepted, with the item being edited as the argument
        /// </summary>
        public event EventHandler<NodeVM>? AcceptEdits;

        /// <summary>
        /// Fires when edits are canceled, with the item being edited as the argument
        /// </summary>
        public event EventHandler<NodeVM>? CancelEdits;

        /// <summary>
        /// Fires when the sidebar control is opening
        /// </summary>
        public event EventHandler? Opening;

        /// <summary>
        /// Fires when the sidebar control has opened
        /// </summary>
        public event EventHandler? Opened;

        /// <summary>
        /// Fires when the sidebar control has closed
        /// </summary>
        public event EventHandler? Closed;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// Whether to show the editor
        /// </summary>
        public bool ShowEditor { get; private set; } = false;

        /// <summary>
        /// Node being edited
        /// </summary>
        public NodeVM? EditItem { get; private set; } = null;

        /// <summary>
        /// Coordinates of the node being edited
        /// </summary>
        public ObservableCollection<CoordinateVM> EditCoordinates { get; } = [];

        /// <summary>
        /// True if editing an existing item
        /// </summary>
        public bool Editing { get; private set; } = false;

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; set; } = new();

        // ---------------------- Commands ----------------------

        /// <summary>
        /// Relay command to accept edits
        /// </summary>
        public ICommand AcceptCommand { get; }

        /// <summary>
        /// Relay command to cancel edits
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Relay command to fix all coordinates of the node
        /// </summary>
        public ICommand FixAllCommand { get; }

        /// <summary>
        /// Relay command to free all coordinates of the node
        /// </summary>
        public ICommand UnfixAllCommand { get; }

        // ---------------------- Public Methods ----------------------

        /// <summary>
        /// Primary constructor
        /// </summary>
        public NodeEditVM() 
        {
            AcceptCommand = new RelayCommand(AcceptEdit);
            CancelCommand = new RelayCommand(CancelEdit);
            FixAllCommand = new RelayCommand(()=> SetFixity(true));
            UnfixAllCommand = new RelayCommand(() => SetFixity(false));
        }

        /// <summary>
        /// Displays the control for editing a node
        /// </summary>
        /// <param name="item">The node to edit or add</param>
        /// <param name="editing">True if we're editing an existing node</param>
        public void DisplayEditor(NodeVM item, bool editing)
        {
            Opening?.Invoke(this, new EventArgs());

            // Make a copy of the input item so we can restore parameters if editing is cancelled
            _inputItem = new NodeVM((Node)item.Model.Clone());

            EditItem = item;
            Editing = editing;

            // If we're editing, select the node being edited for clarity
            if (editing)
                EditItem.Selected = true;

            EditCoordinates.Clear();

            for (int i = 0; i < EditItem.Model.DOFs; i++)
            {
                var userCoord = App.Units.Length.ToUser(EditItem.Model.Coordinates[i]);  // Convert to user units
                var coordVM = new CoordinateVM(i, userCoord, EditItem.Model.Fixity[i] == 1);
                coordVM.ValueChanged += OnCoordinateValueChanged;
                EditCoordinates.Add(coordVM);
            }

            ShowEditor = true;
            Opened?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Cancels editing, hiding the editor
        /// </summary>
        public void CancelEdit()
        {
            try
            {
                // Do this first in case something else throws an error - we always want to close the editor
                HideEditor();

                if (_inputItem != null && EditItem != null)
                {
                    // Restore parameters we started with
                    EditItem.Model.ImportParameters(_inputItem.Model);
                }

                if (EditItem != null)
                {
                    CancelEdits?.Invoke(this, EditItem);
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
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
                EditItem.Model.Coordinates[e] = App.Units.Length.FromUser(vm.Value);  // Convert from user units
                EditItem.Model.Fixity[e] = vm.Fixed ? 1 : 0;

                // Set the whole array to itself to make sure PropertyChanged event gets raised.
                // This will also invalidate the solution
                EditItem.Model.Fixity = [.. EditItem.Model.Fixity.ToList()];
                EditItem.Model.Coordinates = [.. EditItem.Model.Coordinates.ToList()];
            }
        }

        // ---------------------- Private Helpers ----------------------

        /// <summary>
        /// Accepts edits, hiding the editor
        /// </summary>
        private void AcceptEdit()
        {
            try
            {
                if (EditItem != null)
                {
                    AcceptEdits?.Invoke(this, EditItem);
                }

                HideEditor(); // Do this after the event in case an error occurs
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Hide the attached control
        /// </summary>
        private void HideEditor()
        {
            // Deselect the item when edits finish
            if (EditItem != null)
                EditItem.Selected = false;

            ShowEditor = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the fixity for all coordinates of the node
        /// </summary>
        /// <param name="fix">True to fix all coordinates, false to free</param>
        private void SetFixity(bool fix)
        {
            try
            {
                foreach (var vm in EditCoordinates)
                {
                    vm.Fixed = fix;
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }
    }
}
