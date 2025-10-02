using FEA_Program.Drawable;
using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal class NodeManager
    {
        private readonly SortedDictionary<int, NodeDrawable> _Nodes = []; // reference nodes by ID, always sorting from smallest to largest ID

        public event EventHandler<SortedDictionary<int, NodeDrawable>>? NodeListChanged;  // Length of nodelist has changed
        public event EventHandler<List<int>>? NodesChanged; // Node has changed such that list needs to be updated & screen redrawn
        public event EventHandler? NodeChanged_RedrawOnly; // Node has changed such that screen only needs to be redrawn
        public event EventHandler<INode>? NodeAdded; // dont use for redrawing lists or screen
        public event EventHandler<INode>? NodeDeleted; // dont use for redrawing lists or screen

        // ---------------------- Public Properties ----------------------------

        /// <summary>
        /// Gets all nodes, sorted from smallest to largest ID
        /// </summary>
        public List<NodeDrawable> Nodelist => _Nodes.Values.ToList();

        /// <summary>
        /// Gets all base nodes, sorted from smallest to largest ID
        /// </summary>
        public List<Node> BaseNodelist => Nodelist.Cast<Node>().ToList();

        /// <summary>
        /// Gets coords of all nodes sorted by ID
        /// </summary>
        public Dictionary<int, double[]> NodeCoordinates => _Nodes.Values.ToDictionary(
            node => node.ID,    // Key selector: The ID of the Node object
            node => node.Coordinates // Value selector: The Coords property of the Node object
        );

        /// <summary>
        /// The overall number of node dimensions
        /// </summary>
        public int ProblemSize => _Nodes.Values.Sum(node => node.Dimension);
 
        /// <summary>
        /// Gets the global force vector, sorted from smallest to largest node ID
        /// </summary>
        public DenseVector F_Mtx => Node.BuildVector(BaseNodelist, n => n.Force);

        /// <summary>
        /// Gets the global fixity vector, sorted from smallest to largest node ID
        /// </summary>
        public DenseVector Q_Mtx => Node.BuildVector(BaseNodelist, n => Array.ConvertAll(n.Fixity, x => (double)x));

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
                if (ExistsAtLocation(coords[i])) // dont want to create node where one already is
                {
                    throw new Exception("Tried to create node at location where one already exists. Nodes cannot be in identical locations.");
                }

                var newnode = new NodeDrawable(coords[i], fixity[i], CreateNodeId(), dimensions[i]);
                _Nodes.Add(newnode.ID, newnode);
                NodeAdded?.Invoke(this, newnode);
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
                var node = _Nodes[NodeID];
                _Nodes.Remove(NodeID);
                NodeDeleted?.Invoke(this, node);
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
            }

            NodeListChanged?.Invoke(this, _Nodes); // this will redraw so leave it until all have been updated
        }


        /// <summary>
        /// Sets the solution for all nodes
        /// </summary>
        /// <param name="Q">The global displacement vector</param>
        /// <param name="R">The global reaction force vector</param>
        public void SetSolution(DenseVector Q, DenseVector R)
        {
            int index = 0;

            foreach (Node node in Nodelist)
            {
                var nodeDisplacements = new double[node.Dimension];
                var nodeReactions = new double[node.Dimension];

                for (int i = 0; i < node.Dimension; i++)
                {
                    nodeDisplacements[i] = Q[index];
                    nodeReactions[i] = R[index];
                    index++;
                }

                node.Solve(nodeDisplacements, nodeReactions);
            }

            NodesChanged?.Invoke(this, [.. _Nodes.Keys]);
        }
        
        // ---------------------- Private Helpers ----------------------------

        /// <summary>
        /// Returns true if a node already exists at the given location
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        private bool ExistsAtLocation(double[] coords)
        {
            foreach (Node node in _Nodes.Values)
            {
                // This should work regardless of dimension
                if (node.Coordinates.Take(node.Dimension).SequenceEqual(coords.Take(node.Dimension)))
                    return true;
            }

            return false;
        }

        private int CreateNodeId()
        {
            int NewID = 1;

            while (_Nodes.Keys.Contains(NewID))
                NewID += 1;

            return NewID;
        }
    }

}
