using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class MaterialEditVM: ObservableObject
    {
        private MaterialVM? _inputMaterial;
        
        //private Material? _startingParameters;
        
        // ---------------------- Events ----------------------

        public event EventHandler<MaterialVM>? AcceptEdits;
        public event EventHandler<MaterialVM>? CancelEdits;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// Material being edited
        /// </summary>
        public MaterialVM? Material { get; private set; } = null;

        /// <summary>
        /// True if editing an existing material
        /// </summary>
        public bool Editing { get; private set; } = false;

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
            _inputMaterial = material;

            // Make this so we're editing a copy. The original is preserved in case we cancel
            Material = new MaterialVM((Material)material.Model.Clone());
            Editing = editing;
        }
        public void HideEditor()
        {
            Material = null; // This hides the editor
        }

        // ---------------------- Private Helpers ----------------------
        private void AcceptEdit()
        {
            if (_inputMaterial != null && Material != null)
            {
                // Copy edited parameters
                _inputMaterial.Model.ImportParameters(Material.Model);
                AcceptEdits?.Invoke(this, _inputMaterial);
            }

            HideEditor(); // Do this after the event in case an error occurs
        }
        private void CancelEdit()
        {
            HideEditor();
            if (_inputMaterial != null)
            {
                CancelEdits?.Invoke(this, _inputMaterial);
            }
        }
    }
}
