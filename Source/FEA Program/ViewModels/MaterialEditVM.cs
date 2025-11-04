using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class MaterialEditVM: ObservableObject, ISideBarEditor
    {
        private MaterialVM? _inputMaterial;
        
        // ---------------------- Events ----------------------

        public event EventHandler<MaterialVM>? AcceptEdits;
        public event EventHandler<MaterialVM>? CancelEdits;

        /// <summary>
        /// Fires when the sidebar control is opening
        /// </summary>
        public event EventHandler? Opening;

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
        /// Material being edited
        /// </summary>
        public MaterialVM? Material { get; private set; } = null;

        /// <summary>
        /// True if editing an existing material
        /// </summary>
        public bool Editing { get; private set; } = false;

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; set; } = new();

        // ---------------------- Commands ----------------------
        public ICommand AcceptCommand { get; }
        public ICommand CancelCommand { get; }

        // ---------------------- Public Methods ----------------------

        public MaterialEditVM() 
        {
            AcceptCommand = new RelayCommand(AcceptEdit);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        public void DisplayEditor(MaterialVM material, bool editing)
        {
            Opening?.Invoke(this, EventArgs.Empty);
            _inputMaterial = material;

            // Make this so we're editing a copy. The original is preserved in case we cancel
            Material = new MaterialVM((Material)material.Model.Clone());
            Editing = editing;
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
                if (_inputMaterial != null)
                {
                    CancelEdits?.Invoke(this, _inputMaterial);
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        // ---------------------- Private Helpers ----------------------
        private void AcceptEdit()
        {
            try
            {
                if (_inputMaterial != null && Material != null)
                {
                    // Copy edited parameters
                    _inputMaterial.Model.ImportParameters(Material.Model);
                    AcceptEdits?.Invoke(this, _inputMaterial);
                }

                HideEditor(); // Do this after the event in case an error occurs
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }
        private void HideEditor()
        {
            ShowEditor = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }

    }
}
