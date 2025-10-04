using FEA_Program.Drawable;
using FEA_Program.Models;
using FEA_Program.UI;
using FEA_Program.UserControls;

namespace FEA_Program.Controllers
{
    internal class NodeManager
    {
        private readonly List<IMainView> _mainViews = [];
        
        private readonly SortedDictionary<int, NodeDrawable> _Nodes = []; // reference nodes by ID, always sorting from smallest to largest ID

        public event EventHandler<SortedDictionary<int, NodeDrawable>>? NodeListChanged;  // Length of nodelist has changed
        public event EventHandler<List<int>>? NodesChanged; // Node has changed such that list needs to be updated & screen redrawn
        public event EventHandler? NodeChanged_RedrawOnly; // Node has changed such that screen only needs to be redrawn

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
                _Nodes[IDs[i]].Force = forces[i];

            NodesChanged?.Invoke(this, IDs);
        }
        public void Delete(List<int> ids)
        {
            foreach (int NodeID in ids) // remove node from list
            {
                _Nodes.Remove(NodeID);
                Problem.RemoveNode(NodeID);
            }

            if (ids.Count > 0)
            {
                NodeListChanged?.Invoke(this, _Nodes);
            }
        }
        public void Reset()
        {
            _Nodes.Clear();
            NodeListChanged?.Invoke(this, _Nodes);
        }

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="nodes"></param>
        public void ImportNodes(List<NodeDrawable> nodes)
        {
            _Nodes.Clear();

            foreach (NodeDrawable node in nodes)
            {
                _Nodes[node.ID] = node;
                Problem.AddNode(node);
            }

            NodeListChanged?.Invoke(this, _Nodes); // this will redraw so leave it until all have been updated
        }

        // ---------------------- Public Methods - Views ----------------------------
        public void AddMainView(IMainView view)
        {
            view.NodeAddRequest += OnAddNodeRequest;
            _mainViews.Add(view);
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
                    var newNode = new NodeDrawable(new double[dimension], new int[dimension], Id, dimension);

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
                    var newNode = e;
                    Problem.AddNode(newNode); // This must be done first because it validates the node parameters
                    _Nodes.Add(newNode.ID, newNode);

                    NodeListChanged?.Invoke(this, _Nodes); // this will redraw so leave it until all have been updated
                }

                // This must be done at the end in case node creation fails
                view.NodeEditConfirmed -= OnNodeEditsConfirmed;
            }
        }

    }

}
