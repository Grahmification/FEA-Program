using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class ElementsVM: ObservableObject
    {
        private ObservableCollection<NodeVM> _nodes = [];
        private ObservableCollection<MaterialVM> _materials = [];
        
        // ---------------------- Models ----------------------
        public ObservableCollection<ElementVM> Items { get; private set; } = [];

        // ---------------------- Sub VMs ----------------------
        public BaseVM Base { get; private set; } = new();
        
        public ElementAddVM AddEditor { get; private set; } = new();

        // ---------------------- Commands ----------------------

        public ICommand? AddCommand { get; }

        // ---------------------- Public Methods ----------------------
        public ElementsVM()
        {
            AddCommand = new RelayCommand(AddElementWithEditor);
            AddEditor.AcceptEdits += OnAcceptEdits;
        }
        public void LinkCollections(ObservableCollection<NodeVM> nodes, ObservableCollection<MaterialVM> materials)
        {
            _nodes = nodes;
            _materials = materials;
        }

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="elements"></param>
        public void ImportElements(List<IElement> elements)
        {
            Items.Clear();
            foreach (var item in elements)
            {
                var material = _materials.FirstOrDefault(m => m.Model.ID == item.Material.ID);
                if (material is null)
                    throw new IndexOutOfRangeException($"Could not find material {item.Material.ID} corresponding to element {item.ID}");

                List<NodeVM> nodes = [];
                foreach (var node in item.Nodes)
                {
                    NodeVM? vm = _nodes.FirstOrDefault(m => m.Model.ID == node.ID);
                    if (vm is null)
                        throw new IndexOutOfRangeException($"Could not find node {node.ID} corresponding to element {item.ID}");

                    nodes.Add(vm);
                }

                AddVM(new ElementVM(item, [.. nodes], material));
            }
        }

        /// <summary>
        /// Delete the indicated elements, or all if the list is null
        /// </summary>
        /// <param name="elementIDs">Element IDs to delete</param>
        public void Delete(List<int>? elementIDs = null)
        {
            // Delete all elements by default
            var selectedElements = Items.ToList();

            if (elementIDs != null)
            {
                selectedElements = Items
                .Where(element => elementIDs.Contains(element.Model?.ID ?? -1))
                .ToList();
            }

            foreach (var item in selectedElements)
                DeleteVM(item);
        }


        // ---------------------- Event Methods ----------------------
        private void OnEditRequest(object? sender, EventArgs e)
        {
            if (sender is ElementVM vm)
            {
                //Editor.DisplayEditor(vm, true);
            }
        }
        private void OnDeleteRequest(object? sender, EventArgs e)
        {
            if(sender is ElementVM vm)
            {
                Items.Remove(vm);

                vm.DeleteRequest -= OnDeleteRequest;
                vm.EditRequest -= OnEditRequest;
            }
        }

        private void OnAcceptEdits(object? sender, ElementVM e)
        {
            // This is a new material
            if (sender is ElementAddVM vm)
            {
                // Todo - validate the material
                AddVM(e);
            }
        }

        // ---------------------- Private Helpers ----------------------
        private void AddVM(ElementVM vm)
        {
            Items.Add(vm);
            vm.DeleteRequest += OnDeleteRequest;
            vm.EditRequest += OnEditRequest;
        }
        private void DeleteVM(ElementVM vm)
        {
            Items.Remove(vm);
            vm.DeleteRequest -= OnDeleteRequest;
            vm.EditRequest -= OnEditRequest;
        }

        private void AddElementWithEditor()
        {
            // Allocate an unused ID
            int ID = IDClass.CreateUniqueId(Items.Select(m => m.Model).Cast<IHasID>().ToList());
            AddEditor.DisplayEditor(ID, _materials, _nodes);
        }

    }
}
