using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class NodesVM: ObservableObject
    {
        // ---------------------- Models ----------------------
        //private readonly Dictionary<int, MaterialVM> _Materials = []; // reference by ID
        
        public ObservableCollection<NodeVM> Items { get; private set; } = [];

        // ---------------------- Sub VMs ----------------------
        public BaseVM Base { get; private set; } = new();
        //public NodeEditVM Editor { get; private set; } = new();

        // ---------------------- Commands ----------------------

        public ICommand? AddCommand { get; }

        // ---------------------- Public Methods ----------------------
        public NodesVM()
        {
            //AddCommand = new RelayCommand(AddNodeWithEditor);
            //Editor.AcceptEdits += OnAcceptEdits;
        }

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="nodes"></param>
        public void ImportNodes(List<Node> nodes)
        {
            Items.Clear();
            foreach (var node in nodes)
            {
                AddVM(new NodeVM(node));
            }
        }

        // ---------------------- Event Methods ----------------------
        private void OnEditRequest(object? sender, EventArgs e)
        {
            if (sender is NodeVM vm)
            {
                //Editor.DisplayEditor(vm, true);
            }
        }
        private void OnDeleteRequest(object? sender, EventArgs e)
        {
            if(sender is NodeVM vm)
            {
                Items.Remove(vm);

                vm.DeleteRequest -= OnDeleteRequest;
                vm.EditRequest -= OnEditRequest;
            }
        }

        // ---------------------- Private Helpers ----------------------
        private void AddVM(NodeVM vm)
        {
            Items.Add(vm);
            vm.DeleteRequest += OnDeleteRequest;
            vm.EditRequest += OnEditRequest;
        }
        private void DeleteVM(NodeVM vm)
        {
            Items.Remove(vm);
            vm.DeleteRequest -= OnDeleteRequest;
            vm.EditRequest -= OnEditRequest;
        }

    }
}
