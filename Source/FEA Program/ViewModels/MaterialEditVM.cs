using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for a GUI that allows editing material properties
    /// </summary>
    internal class MaterialEditVM: ObservableObject, ISideBarEditor
    {
        /// <summary>
        /// The original item input when the editor was opened
        /// </summary>
        private MaterialVM? _inputMaterial;

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when edits are accepted, with the item being edited as the argument
        /// </summary>
        public event EventHandler<MaterialVM>? AcceptEdits;

        /// <summary>
        /// Fires when edits are canceled, with the item being edited as the argument
        /// </summary>
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

        /// <summary>
        /// Relay command to accept edits
        /// </summary>
        public ICommand AcceptCommand { get; }

        /// <summary>
        /// Relay command to cancel edits
        /// </summary>
        public ICommand CancelCommand { get; }

        // ---------------------- Public Methods ----------------------

        /// <summary>
        /// Primary constructor
        /// </summary>
        public MaterialEditVM() 
        {
            AcceptCommand = new RelayCommand(AcceptEdit);
            CancelCommand = new RelayCommand(CancelEdit);
        }

        /// <summary>
        /// Displays the editor when editing a material
        /// </summary>
        /// <param name="material">The material to edit or add</param>
        /// <param name="editing">True if we're editing an existing material</param>
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

        /// <summary>
        /// Accepts edits, hiding the editor
        /// </summary>
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

        /// <summary>
        /// Hide the attached control
        /// </summary>
        private void HideEditor()
        {
            ShowEditor = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }

    }
}
