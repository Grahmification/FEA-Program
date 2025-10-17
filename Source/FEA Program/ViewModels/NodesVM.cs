using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class NodesVM: ObservableObject
    {
        /// <summary>
        /// Number of DOFs used for creating new nodes
        /// </summary>
        private int _problemDOFs = -1;

        // ---------------------- Models ----------------------
        //private readonly Dictionary<int, MaterialVM> _Materials = []; // reference by ID
 
        public ObservableCollection<NodeVM> Items { get; private set; } = [];

        // ---------------------- Sub VMs ----------------------
        public BaseVM Base { get; private set; } = new();
        public NodeEditVM Editor { get; private set; } = new();

        // ---------------------- Commands ----------------------

        public ICommand? AddCommand { get; }

        // ---------------------- Public Methods ----------------------
        public NodesVM(int problemDOFs = -1)
        {
            _problemDOFs = problemDOFs;
            AddCommand = new RelayCommand(AddNodeWithEditor);
            Editor.AcceptEdits += OnAcceptEdits;
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
                Editor.DisplayEditor(vm, true);
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

        private void OnAcceptEdits(object? sender, NodeVM e)
        {
            if (sender is NodeEditVM vm)
            {
                // Todo - validate the node

                // This is a new node
                if (!vm.Editing)
                {
                    AddVM(e);
                }
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
        private void AddNodeWithEditor()
        {
            int ID = IDClass.CreateUniqueId(Items.Select(m => m.Model).Cast<IHasID>().ToList());
            double[] coords = new double[_problemDOFs];
            int[] fixity = new int[_problemDOFs];
            var node = new Node(coords, fixity, ID, _problemDOFs);

            var vm = new NodeVM(node);
            Editor.DisplayEditor(vm, false);
        }

    }
}
