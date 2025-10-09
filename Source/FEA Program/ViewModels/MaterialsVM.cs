using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;

namespace FEA_Program.ViewModels
{
    internal class MaterialsVM: ObservableObject
    {
        // ---------------------- Models ----------------------
        private readonly Dictionary<int, MaterialVM> _Materials = []; // reference by ID
        
        public ObservableCollection<MaterialVM> Items { get; private set; } = [];

        // ---------------------- Sub VMs ----------------------
        public BaseVM Base { get; private set; } = new();
        public MaterialEditVM Editor { get; private set; } = new();

        // ---------------------- Public Methods ----------------------
        public MaterialsVM()
        {
            Editor.AcceptEdits += OnAcceptEdits;
            Editor.CancelEdits += OnCancelEdits;
        }

        public void Add(string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, MaterialType subtype)
        {
            int ID = IDClass.CreateUniqueId(_Materials.Values.Select(m => m.Model).Cast<IHasID>().ToList());
            var material = new Material(Name, E_GPa, ID, subtype)
            {
                V = V,
                E = E_GPa * Math.Pow(1000, 3),
                Sy = Sy_MPa * Math.Pow(1000, 2),
                Sut = Sut_MPa * Math.Pow(1000, 2),
            };

            var vm = new MaterialVM(material);

            _Materials.Add(ID, vm);
            Items.Add(_Materials[ID]);
            vm.DeleteRequest += OnDeleteRequest;
            vm.EditRequest += OnEditRequest;
        }
        public void Delete(MaterialVM vm)
        {
            _Materials.Remove(vm.Model.ID);
            Items.Remove(vm);
            vm.DeleteRequest += OnDeleteRequest;
            vm.EditRequest += OnEditRequest;
        }
        public void AddDefaultMaterials()
        {
            Add("Aluminum 6061-T6", 69, 0.3, 276, 210, MaterialType.Aluminum_Alloy);
            Add("1045 Steel", 200, 0.29, 565, 310, MaterialType.Steel_Alloy);
        }

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="materials"></param>
        public void ImportMaterials(List<Material> materials)
        {
            _Materials.Clear();
            Items.Clear();
            foreach (var material in materials)
            {
                var vm = new MaterialVM(material);
                _Materials[material.ID] = vm;
                Items.Add(vm);
                vm.DeleteRequest += OnDeleteRequest;
                vm.EditRequest += OnEditRequest;
            }
        }

        // ---------------------- Event Methods ----------------------
        private void OnEditRequest(object? sender, EventArgs e)
        {
            if (sender is MaterialVM vm)
            {
                Editor.DisplayEditor(vm, true);
            }
        }
        private void OnDeleteRequest(object? sender, EventArgs e)
        {
            if(sender is MaterialVM vm)
            {
                _Materials.Remove(vm.Model.ID);
                Items.Remove(vm);

                vm.DeleteRequest -= OnDeleteRequest;
                vm.EditRequest -= OnEditRequest;
            }
        }

        private void OnAcceptEdits(object? sender, MaterialVM e)
        {
            if (sender is MaterialEditVM vm)
            {
                // Todo - validate the material
            }
        }
        private void OnCancelEdits(object? sender, MaterialVM e)
        {
            if (sender is MaterialEditVM vm)
            {
                // If we were adding a material but it was cancelled, delete it
                if (!vm.Editing)
                {
                    Delete(e);
                }
            }
        }
    }
}
