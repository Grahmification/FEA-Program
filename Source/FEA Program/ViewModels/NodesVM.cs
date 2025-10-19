using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class NodesVM: ObservableObject
    {
        private readonly CollectionViewSource _nonZeroForceCollection;

        /// <summary>
        /// Number of DOFs used for creating new nodes
        /// </summary>
        private int _problemDOFs = -1;

        // ---------------------- Events ----------------------

        public event EventHandler<NodeVM>? ItemAdding;
        public event EventHandler<NodeVM>? ItemRemoving;

        // ---------------------- Models ----------------------
        //private readonly Dictionary<int, MaterialVM> _Materials = []; // reference by ID

        public ObservableCollection<NodeVM> Items { get; private set; } = [];
        public ICollectionView NonZeroForceItems { get; }

        // ---------------------- Sub VMs ----------------------
        public BaseVM Base { get; private set; } = new();
        public NodeEditVM Editor { get; private set; } = new();
        public ForceEditVM ForceEditor { get; private set; } = new();

        // ---------------------- Commands ----------------------
        public ICommand? AddCommand { get; }
        public ICommand? AddForceCommand { get; }

        // ---------------------- Public Methods ----------------------
        public NodesVM(int problemDOFs = -1)
        {
            _problemDOFs = problemDOFs;
            AddForceCommand = new RelayCommand(() => ForceEditor.DisplayEditorAdd([.. Items], _problemDOFs));
            AddCommand = new RelayCommand(AddNodeWithEditor);
            Editor.AcceptEdits += OnAcceptEdits;

            // Filter items without a force
            _nonZeroForceCollection = new CollectionViewSource { Source = Items };
            _nonZeroForceCollection.Filter += (_, e) =>
            {
                if (e.Item is NodeVM node)
                    e.Accepted = node.Model.ForceMagnitude != 0;
                else
                    e.Accepted = false;
            };

            NonZeroForceItems = _nonZeroForceCollection.View;
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
        private void OnEditForceRequest(object? sender, EventArgs e)
        {
            if (sender is NodeVM vm)
            {
                ForceEditor.DisplayEditor(vm);
            }
        }
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
                DeleteVM(vm);
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
            // Call first so others can validate the prior
            ItemAdding?.Invoke(this, vm);
            
            Items.Add(vm);
            vm.DeleteRequest += OnDeleteRequest;
            vm.EditRequest += OnEditRequest;
            vm.EditForceRequest += OnEditForceRequest;

            // Update force list when a force value changes
            vm.Model.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(NodeVM.Force))
                    _nonZeroForceCollection.View.Refresh();
            };
        }
        private void DeleteVM(NodeVM vm)
        {
            // Call first so others can validate the prior
            ItemRemoving?.Invoke(this, vm);

            Items.Remove(vm);
            vm.DeleteRequest -= OnDeleteRequest;
            vm.EditRequest -= OnEditRequest;
            vm.EditForceRequest -= OnEditForceRequest;
        }
        private void AddNodeWithEditor()
        {
            int ID = IDClass.CreateUniqueId(Items.Select(m => m.Model).Cast<IHasID>().ToList());
            var node = new Node(ID, _problemDOFs);

            var vm = new NodeVM(node);
            Editor.DisplayEditor(vm, false);
        }

    }
}
