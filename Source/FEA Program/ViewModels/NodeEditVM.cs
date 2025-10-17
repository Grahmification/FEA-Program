using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
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
        /// True if editing an existing item
        /// </summary>
        public bool Editing { get; private set; } = false;

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

            // Make this so we're editing a copy. The original is preserved in case we cancel
            EditItem = null;
            EditItem = new NodeVM((Node)item.Model.Clone());
            Editing = editing;
        }
        public void HideEditor()
        {
            EditItem = null; // This hides the editor
        }

        // ---------------------- Private Helpers ----------------------
        private void AcceptEdit()
        {
            if (_inputItem != null && EditItem != null)
            {
                // Copy edited parameters
                _inputItem.Model.ImportParameters(EditItem.Model);
                AcceptEdits?.Invoke(this, _inputItem);
            }

            HideEditor(); // Do this after the event in case an error occurs
        }
        private void CancelEdit()
        {
            HideEditor();
            if (_inputItem != null)
            {
                CancelEdits?.Invoke(this, _inputItem);
            }
        }
        private void SetFixity(bool fix)
        {
            foreach(var vm in EditItem?.Coordinates ?? [])
            {
                vm.Fixed = fix;
            }
        }
    }
}
