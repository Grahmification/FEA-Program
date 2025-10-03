using FEA_Program.Drawable;

namespace FEA_Program.Models
{
    internal class NodeManager
    {
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
        public void AddNodes(List<double[]> coords, List<int[]> fixity, List<int> dimensions)
        {
            if (coords.Count != fixity.Count | coords.Count != dimensions.Count)
            {
                throw new ArgumentException("Tried to create node with unmatched lengths of input values.");
            }

            for (int i = 0; i < coords.Count; i++)
            {
                int ID = IDClass.CreateUniqueId(Problem.Nodes.Cast<IHasID>().ToList());

                var newnode = new NodeDrawable(coords[i], fixity[i], ID, dimensions[i]);
                _Nodes.Add(newnode.ID, newnode);
                Problem.AddNode(newnode);
            }

            NodeListChanged?.Invoke(this, _Nodes); // this will redraw so leave it until all have been updated
        }
        public void EditNode(List<double[]> coords, List<int[]> fixity, List<int> ids)
        {
            if (coords.Count != fixity.Count | coords.Count != ids.Count)
            {
                throw new ArgumentException("Tried to apply node edits with unmatched lengths of input values.");
            }

            for (int i = 0; i < ids.Count; i++)
            {
                _Nodes[ids[i]].Coordinates = coords[i];
                _Nodes[ids[i]].Fixity = fixity[i];
            }

            NodesChanged?.Invoke(this, ids);
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

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="nodes"></param>
        public void ImportNodes(List<NodeDrawable> nodes)
        {
            _Nodes.Clear();

            foreach(NodeDrawable node in nodes)
            {
                _Nodes[node.ID] = node;
                Problem.AddNode(node);
            }

            NodeListChanged?.Invoke(this, _Nodes); // this will redraw so leave it until all have been updated
        }

    }

}
