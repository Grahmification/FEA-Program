using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for managing the nodes list in the program
    /// </summary>
    internal class NodesVM: ObservableObject
    {
        /// <summary>
        /// The collection of nodes that have non-zero forces
        /// </summary>
        private readonly CollectionViewSource _nonZeroForceCollection;

        /// <summary>
        /// The problem dimension for creating new nodes
        /// </summary>
        private readonly int _problemDimension = -1;

        /// <summary>
        /// Whether the problem has rotation for creating new nodes
        /// </summary>
        private readonly bool _problemHasRotation = false;

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when a new node is being added to the collection
        /// </summary>
        public event EventHandler<NodeVM>? ItemAdding;

        /// <summary>
        /// Fires when a node is being removed from the collection
        /// </summary>
        public event EventHandler<NodeVM>? ItemRemoving;

        // ---------------------- Models ----------------------

        /// <summary>
        /// All nodes in the program
        /// </summary>
        public ObservableCollection<NodeVM> Items { get; private set; } = [];

        /// <summary>
        /// Collection of nodes that have non-zero forces
        /// </summary>
        public ICollectionView NonZeroForceItems { get; }

        // ---------------------- Sub VMs ----------------------

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; private set; } = new();

        /// <summary>
        /// Viewmodel for adding or editing nodes
        /// </summary>
        public NodeEditVM Editor { get; private set; } = new();

        /// <summary>
        /// Viewmodel for adding or editing forces on a node
        /// </summary>
        public ForceEditVM ForceEditor { get; private set; } = new();

        // ---------------------- Commands ----------------------

        /// <summary>
        /// Relay command for <see cref="AddNodeWithEditor"/>
        /// </summary>
        public ICommand? AddCommand { get; }

        /// <summary>
        /// Relay command for displaying the <see cref="ForceEditor"/>
        /// </summary>
        public ICommand? AddForceCommand { get; }

        // ---------------------- Public Methods ----------------------

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="problemDOFs">Number of DOFs used for creating new nodes</param>
        public NodesVM(int problemDOFs = -1, bool problemHasRotation = false)
        {
            _problemDimension = problemDOFs;
            _problemHasRotation = problemHasRotation;
            AddForceCommand = new RelayCommand(() => ForceEditor.DisplayEditorAdd([.. Items], _problemDimension));
            AddCommand = new RelayCommand(AddNodeWithEditor);
            Editor.AcceptEdits += OnAcceptEdits;

            // Filter items without a force
            _nonZeroForceCollection = new CollectionViewSource { Source = Items };
            _nonZeroForceCollection.Filter += (_, e) =>
            {
                if (e.Item is NodeVM node)
                    e.Accepted = node.ForceMagnitude != 0;
                else
                    e.Accepted = false;
            };

            NonZeroForceItems = _nonZeroForceCollection.View;
        }

        /// <summary>
        /// Sets the base, also assigning it to any sub-classes
        /// </summary>
        /// <param name="baseVM"></param>
        public void SetBase(BaseVM baseVM)
        {
            Base = baseVM;
            Editor.Base = baseVM;
            ForceEditor.Base = baseVM;
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

        /// <summary>
        /// Called when a node requests that its forces should be edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditForceRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender is NodeVM vm)
                {
                    ForceEditor.DisplayEditor(vm);
                }
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Called when a node requests that it should be edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEditRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender is NodeVM vm)
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
        /// Called when a node requests that it should be deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeleteRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender is NodeVM vm)
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
        /// Called when edits are accepted from <see cref="Editor"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Adds a node to the program
        /// </summary>
        /// <param name="vm">The node to add</param>
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
                if (e.PropertyName == nameof(Node.Force))
                    _nonZeroForceCollection.View.Refresh();
            };

            Base.SetStatus($"Added Node {vm.Model.ID}");
        }

        /// <summary>
        /// Removes a node to the program
        /// </summary>
        /// <param name="vm">The node to remove</param>
        private void DeleteVM(NodeVM vm)
        {
            // Call first so others can validate the prior
            ItemRemoving?.Invoke(this, vm);

            Items.Remove(vm);
            vm.DeleteRequest -= OnDeleteRequest;
            vm.EditRequest -= OnEditRequest;
            vm.EditForceRequest -= OnEditForceRequest;

            Base.SetStatus($"Deleted Node {vm.Model.ID}");
        }

        /// <summary>
        /// Opens the <see cref="Editor"/> to add a new node
        /// </summary>
        private void AddNodeWithEditor()
        {
            try
            {
                Base.ClearStatus();

                int ID = IDClass.CreateUniqueId(Items.Select(m => m.Model).Cast<IHasID>().ToList());
                var node = new Node(ID, _problemDimension, _problemHasRotation);

                var vm = new NodeVM(node);
                Editor.DisplayEditor(vm, false);
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }
    }
}
