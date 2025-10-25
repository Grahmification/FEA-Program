using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

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

        // ---------------------- Commands ----------------------

        public ICommand? AddCommand { get; }

        // ---------------------- Public Methods ----------------------
        public MaterialsVM()
        {
            AddCommand = new RelayCommand(AddMaterialWithEditor);
            Editor.AcceptEdits += OnAcceptEdits;
        }
        public void SetBase(BaseVM baseVM)
        {
            Base = baseVM;
            Editor.Base = baseVM;
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

            AddVM(new MaterialVM(material));
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
                AddVM(new MaterialVM(material));
            }
        }

        // ---------------------- Event Methods ----------------------
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
        private void OnDeleteRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender is MaterialVM vm)
                {
                    _Materials.Remove(vm.Model.ID);
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
        private void AddVM(MaterialVM vm)
        {
            _Materials[vm.Model.ID] = vm;
            Items.Add(vm);
            vm.DeleteRequest += OnDeleteRequest;
            vm.EditRequest += OnEditRequest;
        }
        private void DeleteVM(MaterialVM vm)
        {
            _Materials.Remove(vm.Model.ID);
            Items.Remove(vm);
            vm.DeleteRequest -= OnDeleteRequest;
            vm.EditRequest -= OnEditRequest;
        }
        private void AddMaterialWithEditor()
        {
            try
            {
                int ID = IDClass.CreateUniqueId(_Materials.Values.Select(m => m.Model).Cast<IHasID>().ToList());
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
