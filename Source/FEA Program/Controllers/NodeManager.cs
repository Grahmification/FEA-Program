using FEA_Program.Drawable;
using FEA_Program.Models;
using FEA_Program.UI;
using FEA_Program.UserControls;

namespace FEA_Program.Controllers
{
    internal class NodeManager
    {
        private readonly List<IMainView> _mainViews = [];
        private readonly List<INodeDisplayView> _displayViews = [];

        private readonly SortedDictionary<int, NodeDrawable> _Nodes = []; // reference nodes by ID, always sorting from smallest to largest ID

        public event EventHandler<SortedDictionary<int, NodeDrawable>>? NodeListChanged;  // Length of nodelist has changed
        public event EventHandler<List<int>>? NodesChanged; // Node has changed such that list needs to be updated & screen redrawn
        public event EventHandler? NodeChanged_RedrawOnly; // Node has changed such that screen only needs to be redrawn

        public event EventHandler<int[]>? HangingElementsFound; // Node has been deleted and left hanging elements

        // ---------------------- Public Properties ----------------------------
        public StressProblem Problem { get; set; } = new();

        /// <summary>
        /// Gets all nodes, sorted from smallest to largest ID
        /// </summary>
        public List<NodeDrawable> Nodelist => _Nodes.Values.ToList();

        // ---------------------- Public Methods ----------------------------
        public NodeDrawable GetNode(int ID) => _Nodes[ID];
        public void SelectNodes(bool selected, int[]? ids = null)
        {
            // If IDs isn't specified, select all elements
            ids ??= [.. _Nodes.Keys];

            foreach (int item in ids)
                _Nodes[item].Selected = selected;
            NodeChanged_RedrawOnly?.Invoke(this, new());
        }

        /// <summary>
        /// Sets node forces
        /// </summary>
        /// <param name="forces">The list of forces for each node</param>
        /// <param name="IDs">The accompanying IDs for the nodes</param>
        /// <exception cref="ArgumentException"></exception>
        public void SetNodeForces(List<double[]> forces, List<int> IDs)
        {
            if (forces.Count != IDs.Count)
            {
                throw new ArgumentException("Tried to add node forces with unmatched lengths of input values.");
            }

            for (int i = 0; i < IDs.Count; i++)
                _Nodes[IDs[i]].Node.Force = forces[i];

            NodesChanged?.Invoke(this, IDs);
        }
        public int[] Delete(List<int> ids)
        {
            List<int> elementIds = [];

            foreach (int NodeID in ids) // remove node from list
            {
                _Nodes.Remove(NodeID);
                elementIds.AddRange(Problem.RemoveNode(NodeID));
            }

            if (ids.Count > 0)
            {
                DisplayNodes();
            }

            // Return any elements that were deleted in the process
            return [.. elementIds];
        }
        public void Reset()
        {
            _Nodes.Clear();
            DisplayNodes();
        }

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="nodes"></param>
        public void ImportNodes(List<Node> nodes)
        {
            _Nodes.Clear();

            foreach (Node node in nodes)
            {
                _Nodes[node.ID] = new NodeDrawable(node);
                Problem.AddNode(node);
            }

            DisplayNodes();
        }

        // ---------------------- Public Methods - Views ----------------------------
        public void AddMainView(IMainView view)
        {
            view.NodeAddRequest += OnAddNodeRequest;
            _mainViews.Add(view);
        }

        public void AddDisplayView(INodeDisplayView view)
        {
            //view.NodeEditRequest += OnEditNodeRequest;
            view.NodeDeleteRequest += OnDeleteNodeRequest;
            _displayViews.Add(view);
        }

        // ---------------------- View Event handlers ----------------------------
        private void OnAddNodeRequest(object? sender, EventArgs e)
        {
            try
            {
                if (sender is not null)
                {
                    var view = (IMainView)sender;

                    // Create a new node, but don't add it yet until after the form confirms
                    int dimension = Problem.AvailableNodeDOFs;
                    int Id = IDClass.CreateUniqueId(Problem.Nodes.Cast<IHasID>().ToList());
                    var newNode = new NodeDrawable(new(new double[dimension], new int[dimension], Id, dimension));

                    var editView = view.ShowNodeEditView(newNode, false);
                    editView.NodeEditConfirmed += OnNodeEditsConfirmed;
                }
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
        private void OnNodeEditsConfirmed(object? sender, NodeDrawable e)
        {
            if (sender is not null)
            {
                var view = (INodeEditView)sender;

                // If we're not editing
                if (!view.Editing)
                {
                    var newNode = e.Node;
                    Problem.AddNode(newNode); // This must be done first because it validates the node parameters
                    _Nodes.Add(newNode.ID, e);

                    DisplayNodes();
                }

                // This must be done at the end in case node creation fails
                view.NodeEditConfirmed -= OnNodeEditsConfirmed;
            }
        }

        private void OnEditNodeRequest(object? sender, int e)
        {
            try
            {
                if (sender is not null)
                {
                    var view = (IMainView)sender;

                    // Create a new node, but don't add it yet until after the form confirms
                    int dimension = Problem.AvailableNodeDOFs;
                    int Id = IDClass.CreateUniqueId(Problem.Nodes.Cast<IHasID>().ToList());
                    var newNode = new NodeDrawable(new(new double[dimension], new int[dimension], Id, dimension));

                    var editView = view.ShowNodeEditView(newNode, false);
                    editView.NodeEditConfirmed += OnNodeEditsConfirmed;
                }
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
        private void OnDeleteNodeRequest(object? sender, int e)
        {
            try
            {
                if (sender is not null)
                {
                    var elementIDs = Delete([e]);

                    if (elementIDs.Length > 0)
                        HangingElementsFound?.Invoke(this, elementIDs);
                }
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }

        // ---------------------- Private Helpers ----------------------------

        private void DisplayNodes()
        {
            foreach (INodeDisplayView view in _displayViews)
            {
                view.DisplayNodes(Nodelist);
            }

            NodeListChanged?.Invoke(this, _Nodes);
        }

    }

}
