using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for managing the elements list in the program
    /// </summary>
    internal class ElementsVM: ObservableObject
    {
        /// <summary>
        /// Nodes in the program
        /// </summary>
        private ObservableCollection<NodeVM> _nodes = [];

        /// <summary>
        /// Materials in the program
        /// </summary>
        private ObservableCollection<MaterialVM> _materials = [];

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when a new element is being added to the collection
        /// </summary>
        public event EventHandler<ElementVM>? ItemAdding;

        /// <summary>
        /// Fires when a element is being removed from the collection
        /// </summary>
        public event EventHandler<ElementVM>? ItemRemoving;

        // ---------------------- Models ----------------------

        /// <summary>
        /// All elements in the program
        /// </summary>
        public ObservableCollection<ElementVM> Items { get; private set; } = [];

        // ---------------------- Sub VMs ----------------------

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; private set; } = new();

        /// <summary>
        /// Viewmodel for an adding new elements
        /// </summary>
        public ElementAddVM AddEditor { get; private set; } = new();

        // ---------------------- Commands ----------------------

        /// <summary>
        /// Relay command for <see cref="AddElementWithEditor"/>
        /// </summary>
        public ICommand? AddCommand { get; }

        // ---------------------- Public Methods ----------------------
        public ElementsVM()
        {
            AddCommand = new RelayCommand(AddElementWithEditor);
            AddEditor.AcceptEdits += OnAcceptEdits;
        }

        /// <summary>
        /// Sets the base
        /// </summary>
        /// <param name="baseVM"></param>
        public void SetBase(BaseVM baseVM)
        {
            Base = baseVM;
            AddEditor.Base = baseVM;
        }

        /// <summary>
        /// Links various collections with this viewmodel
        /// </summary>
        /// <param name="nodes">List of nodes in the program</param>
        /// <param name="materials">List of materials in the program</param>
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

        /// <summary>
        /// Called when an element requests that it should be edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender is ElementVM vm)
                {
                    //Editor.DisplayEditor(vm, true);
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Called when an element requests that it should be deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender is ElementVM vm)
                {
                    DeleteVM(vm);
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Called when edits are accepted from <see cref="AddEditor"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAcceptEdits(object? sender, ElementVM e)
        {
            // This is a new element
            if (sender is ElementAddVM vm)
            {
                AddVM(e);
            }
        }

        // ---------------------- Private Helpers ----------------------
        
        /// <summary>
        /// Adds an element to the program
        /// </summary>
        /// <param name="vm">The element to add</param>
        private void AddVM(ElementVM vm)
        {
            // Call first so others can validate the prior
            ItemAdding?.Invoke(this, vm);
            
            Items.Add(vm);
            vm.DeleteRequest += OnDeleteRequest;
            vm.EditRequest += OnEditRequest;

            Base.SetStatus($"Added Element {vm.Model?.ID}");
        }

        /// <summary>
        /// Deletes an element from the program
        /// </summary>
        /// <param name="vm">The element to delete</param>
        private void DeleteVM(ElementVM vm)
        {
            // Call first so others can validate the prior
            ItemRemoving?.Invoke(this, vm);

            Items.Remove(vm);
            vm.DeleteRequest -= OnDeleteRequest;
            vm.EditRequest -= OnEditRequest;

            Base.SetStatus($"Deleted Element {vm.Model?.ID}");
        }

        /// <summary>
        /// Opens the <see cref="AddEditor"/> to add a new element
        /// </summary>
        private void AddElementWithEditor()
        {
            try
            {
                Base.ClearStatus();
                
                // Allocate an unused ID
                int ID = IDClass.CreateUniqueId(Items.Select(m => m.Model).Cast<IHasID>().ToList());
                AddEditor.DisplayEditor(ID, _materials, _nodes);
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

    }
}
