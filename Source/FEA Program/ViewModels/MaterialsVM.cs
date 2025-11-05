using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for managing the materials list in the program
    /// </summary>
    internal class MaterialsVM: ObservableObject
    {
        // ---------------------- Models ----------------------

        /// <summary>
        /// All materials in the program
        /// </summary>
        public ObservableCollection<MaterialVM> Items { get; private set; } = [];

        // ---------------------- Sub VMs ----------------------

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; private set; } = new();

        /// <summary>
        /// Viewmodel for adding or editing materials
        /// </summary>
        public MaterialEditVM Editor { get; private set; } = new();

        // ---------------------- Commands ----------------------

        /// <summary>
        /// Relay command for <see cref="AddMaterialWithEditor"/>
        /// </summary>
        public ICommand? AddCommand { get; }

        // ---------------------- Public Methods ----------------------
        public MaterialsVM()
        {
            AddCommand = new RelayCommand(AddMaterialWithEditor);
            Editor.AcceptEdits += OnAcceptEdits;
        }

        /// <summary>
        /// Sets the base, also assigning it to any sub-classes
        /// </summary>
        /// <param name="baseVM"></param>
        public void SetBase(BaseVM baseVM)
        {
            Base = baseVM;
            Editor.Base = baseVM;
        }

        /// <summary>
        /// Adds a material with the given parameters to the program
        /// </summary>
        /// <param name="Name">Material Name</param>
        /// <param name="E_GPa">Young's modulus in GPa</param>
        /// <param name="V">Poison's Ratio</param>
        /// <param name="Sy_MPa">Yield strength in MPa</param>
        /// <param name="Sut_MPa">Ultimate strength in MPa</param>
        /// <param name="subtype">Material subtype</param>
        public void Add(string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, MaterialType subtype)
        {
            int ID = IDClass.CreateUniqueId(Items.Select(m => m.Model).Cast<IHasID>().ToList());
            var material = new Material(Name, E_GPa, ID, subtype)
            {
                V = V,
                E = E_GPa * Math.Pow(1000, 3),
                Sy = Sy_MPa * Math.Pow(1000, 2),
                Sut = Sut_MPa * Math.Pow(1000, 2),
            };

            AddVM(new MaterialVM(material));
        }

        /// <summary>
        /// Adds default materials to the program
        /// </summary>
        public void AddDefaultMaterials()
        {
            Add("Aluminum 6061-T6", 69, 0.3, 210, 276, MaterialType.Aluminum_Alloy);
            Add("1045 Steel", 200, 0.29, 310, 565, MaterialType.Steel_Alloy);
        }

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="materials"></param>
        public void ImportMaterials(List<Material> materials)
        {
            Items.Clear();
            foreach (var material in materials)
            {
                AddVM(new MaterialVM(material));
            }
        }

        // ---------------------- Event Methods ----------------------

        /// <summary>
        /// Called when a material requests that it should be edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender is MaterialVM vm)
                {
                    Editor.DisplayEditor(vm, true);
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Called when a material requests that it should be deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender is MaterialVM vm)
                {
                    Items.Remove(vm);

                    vm.DeleteRequest -= OnDeleteRequest;
                    vm.EditRequest -= OnEditRequest;
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Called when edits are accepted from <see cref="Editor"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAcceptEdits(object? sender, MaterialVM e)
        {
            if (sender is MaterialEditVM vm)
            {
                // Todo - validate the material

                // This is a new material
                if (!vm.Editing)
                {
                    AddVM(e);
                }
            }
        }

        // ---------------------- Private Helpers ----------------------

        /// <summary>
        /// Adds a material to the program
        /// </summary>
        /// <param name="vm">The material to add</param>
        private void AddVM(MaterialVM vm)
        {
            Items.Add(vm);
            vm.DeleteRequest += OnDeleteRequest;
            vm.EditRequest += OnEditRequest;

            Base.SetStatus($"Added {vm.Model.Name}");
        }

        /// <summary>
        /// Deletes a material to the program
        /// </summary>
        /// <param name="vm">The material to delete</param>
        private void DeleteVM(MaterialVM vm)
        {
            Items.Remove(vm);
            vm.DeleteRequest -= OnDeleteRequest;
            vm.EditRequest -= OnEditRequest;

            Base.SetStatus($"Deleted {vm.Model.Name}");
        }

        /// <summary>
        /// Opens the <see cref="Editor"/> to add a new material
        /// </summary>
        private void AddMaterialWithEditor()
        {
            try
            {
                Base.ClearStatus();

                int ID = IDClass.CreateUniqueId(Items.Select(m => m.Model).Cast<IHasID>().ToList());
                var material = new Material($"Material {Items.Count + 1}", 70 * 1e9, ID, MaterialType.Other);

                var vm = new MaterialVM(material);
                Editor.DisplayEditor(vm, false);
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }
    }
}
