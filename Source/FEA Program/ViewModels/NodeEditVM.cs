using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class NodeEditVM: ObservableObject
    {
        private NodeVM? _inputItem;
            
        // ---------------------- Events ----------------------

        public event EventHandler<NodeVM>? AcceptEdits;
        public event EventHandler<NodeVM>? CancelEdits;

        // ---------------------- Properties ----------------------

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
        public ICommand AcceptCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand FixAllCommand { get; }
        public ICommand UnfixAllCommand { get; }

        // ---------------------- Public Methods ----------------------

        public NodeEditVM() 
        {
            AcceptCommand = new RelayCommand(AcceptEdit);
            CancelCommand = new RelayCommand(CancelEdit);
            FixAllCommand = new RelayCommand(()=> SetFixity(true));
            UnfixAllCommand = new RelayCommand(() => SetFixity(false));
        }

        public void DisplayEditor(NodeVM item, bool editing)
        {
            _inputItem = item;

            // If we're editing, select the node being edited for clarity
            if (editing)
                _inputItem.Selected = true;

            // Make this so we're editing a copy. The original is preserved in case we cancel
            EditItem = new NodeVM((Node)item.Model.Clone());
            Editing = editing;

            EditCoordinates.Clear();

            for (int i = 0; i < EditItem.Model.Dimension; i++)
            {
                var userCoord = App.Units.Length.ToUser(EditItem.Model.Coordinates[i]);  // Convert to user units
                var coordVM = new CoordinateVM(i, userCoord, EditItem.Model.Fixity[i] == 1);
                coordVM.ValueChanged += OnCoordinateValueChanged;
                EditCoordinates.Add(coordVM);
            }
        }
        public void HideEditor()
        {
            // Deselect the item when edits finish
            if (_inputItem != null)
                _inputItem.Selected = false;

            EditItem = null; // This hides the editor
            EditCoordinates.Clear();
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
            }
        }


        // ---------------------- Private Helpers ----------------------
        private void AcceptEdit()
        {
            try
            {
                if (_inputItem != null && EditItem != null)
                {
                    // Copy edited parameters
                    _inputItem.Model.ImportParameters(EditItem.Model);
                    AcceptEdits?.Invoke(this, _inputItem);
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
                if (_inputItem != null)
                {
                    CancelEdits?.Invoke(this, _inputItem);
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }
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
