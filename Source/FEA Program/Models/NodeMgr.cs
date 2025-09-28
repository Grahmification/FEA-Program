using FEA_Program.Drawable;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Reflection;


namespace FEA_Program.Models
{
    internal class NodeMgr
    {
        private Dictionary<int, NodeDrawable> _Nodes = new Dictionary<int, NodeDrawable>(); // reference nodes by ID

        public event NodeListChangedEventHandler NodeListChanged;

        public delegate void NodeListChangedEventHandler(Dictionary<int, NodeDrawable> NodeList); // Length of nodelist has changed
        public event NodeChangedEventHandler NodeChanged;

        public delegate void NodeChangedEventHandler(List<int> IDs); // Node has changed such that list needs to be updated & screen redrawn
        public event NodeChanged_RedrawOnlyEventHandler NodeChanged_RedrawOnly;

        public delegate void NodeChanged_RedrawOnlyEventHandler(); // Node has changed such that screen only needs to be redrawn
        public event NodeAddedEventHandler NodeAdded;

        public delegate void NodeAddedEventHandler(int NodeID, int Dimension); // dont use for redrawing lists or screen
        public event NodeDeletedEventHandler NodeDeleted;

        public delegate void NodeDeletedEventHandler(int NodeID, int Dimension); // dont use for redrawing lists or screen

        public List<NodeDrawable> Nodelist => _Nodes.Values.ToList();
        public List<Node> BaseNodelist => Nodelist.Cast<Node>().ToList();

        public List<int> AllIDs
        {
            get
            {
                var output = _Nodes.Keys.ToList();
                output.Sort();
                return output;
            }
        } // all node ids
        public Node NodeObj(int ID)
        {
            return _Nodes[ID];
        }

        public Dictionary<int, double[]> NodeCoords // gets coords of all nodes sorted by ID
        {
            get
            {
                var output = new Dictionary<int, double[]>();

                foreach (Node N in _Nodes.Values)
                    output.Add(N.ID, N.Coords);

                return output;
            }
        } // gets all coords referenced by ID
        public int ProblemSize
        {
            get
            {
                int output = 0;

                foreach (Node Val in _Nodes.Values)
                    output += Val.Dimension;

                return output;
            }
        } // overall number of node dimensions in the list
        public DenseMatrix F_Mtx
        {
            get
            {
                var output = new DenseMatrix(ProblemSize, 1);
                var ids = AllIDs;
                ids.Sort();

                int currentrow = 0;

                for (int i = 0, loopTo = ids.Count - 1; i <= loopTo; i++) // iterate through each node
                {
                    for (int j = 0, loopTo1 = _Nodes[ids[i]].Dimension - 1; j <= loopTo1; j++) // iterate through each dimension of the node
                    {

                        output[currentrow, 0] = _Nodes[ids[i]].Force[j];
                        currentrow += 1;
                    }

                }

                return output;
            }
        } // output sorted by node ID
        public DenseMatrix Q_Mtx
        {
            get
            {
                var output = new DenseMatrix(ProblemSize, 1);
                var ids = AllIDs;
                ids.Sort();

                int currentrow = 0;

                for (int i = 0, loopTo = ids.Count - 1; i <= loopTo; i++) // iterate through each node
                {
                    for (int j = 0, loopTo1 = _Nodes[ids[i]].Dimension - 1; j <= loopTo1; j++) // iterate through each dimension of the node
                    {

                        output[currentrow, 0] = _Nodes[ids[i]].Fixity[j];
                        currentrow += 1;
                    }
                }

                return output;
            }
        } // output sorted by node ID

        public void SelectNodes(int[] IDs, bool selected)
        {
            foreach (int item in IDs)
                _Nodes[item].Selected = selected;
            NodeChanged_RedrawOnly?.Invoke();
        }
        public void AddNodes(List<double[]> Coords, List<int[]> Fixity, List<int> Dimensions)
        {
            if (Coords.Count != Fixity.Count | Coords.Count != Dimensions.Count)
            {
                throw new Exception("Tried to run sub <" + MethodBase.GetCurrentMethod().Name + "> with unmatched lengths of input values.");
            }

            for (int i = 0, loopTo = Coords.Count - 1; i <= loopTo; i++)
            {
                if (ExistsAtLocation(Coords[i])) // dont want to create node where one already is
                {
                    throw new Exception("Tried to create node at location where one already exists. Nodes cannot be in identical locations.");
                }

                var newnode = new NodeDrawable(Coords[i], Fixity[i], CreateNodeId(), Dimensions[i]);
                _Nodes.Add(newnode.ID, newnode);
                NodeAdded?.Invoke(newnode.ID, newnode.Dimension);
            }

            NodeListChanged?.Invoke(_Nodes); // this will redraw so leave it until all have been updated
        }
        public void EditNode(List<double[]> Coords, List<int[]> fixity, List<int> IDs)
        {
            if (Coords.Count != fixity.Count | Coords.Count != IDs.Count)
            {
                throw new Exception("Tried to run sub <" + MethodBase.GetCurrentMethod().Name + "> with unmatched lengths of input values.");
            }

            for (int i = 0, loopTo = IDs.Count - 1; i <= loopTo; i++)
            {
                _Nodes[IDs[i]].Coords = Coords[i];
                _Nodes[IDs[i]].Fixity = fixity[i];
            }

            NodeChanged?.Invoke(IDs);
        }
        public void Delete(List<int> IDs)
        {

            foreach (int NodeID in IDs) // remove node from list
            {
                var tmp = _Nodes[NodeID]; // needed to raise event
                _Nodes.Remove(NodeID);

                NodeDeleted?.Invoke(NodeID, tmp.Dimension);
            }

            if (IDs.Count > 0)
            {
                NodeListChanged?.Invoke(_Nodes);
            }
        }
        public void SetSolution(DenseMatrix Q, DenseMatrix R)
        {
            var Ids = AllIDs;
            int currentRow = 0; // tracks the current row being used from the input matrix

            for (int i = 0, loopTo = AllIDs.Count - 1; i <= loopTo; i++) // iterate through each node
            {
                var reactions = new List<double>();
                var displacements = new List<double>();

                for (int j = 0, loopTo1 = _Nodes[Ids[i]].Dimension - 1; j <= loopTo1; j++) // iterate through each dimension
                {
                    reactions.Add(R[currentRow, 0]);
                    displacements.Add(Q[currentRow, 0]);

                    currentRow += 1;
                }

                _Nodes[Ids[i]].Solve(displacements.ToArray(), reactions.ToArray());
                currentRow += 1;
            }
            NodeChanged?.Invoke(Ids);
        }
        public void Addforce(List<double[]> force, List<int> IDs)
        {
            if (force.Count != IDs.Count)
            {
                throw new Exception("Tried to run sub <" + MethodBase.GetCurrentMethod().Name + "> with unmatched lengths of input values.");
            }

            for (int i = 0, loopTo = IDs.Count - 1; i <= loopTo; i++)
                _Nodes[IDs[i]].Force = force[i];

            NodeChanged?.Invoke(IDs);
        }
        public bool ExistsAtLocation(double[] Coords)
        {
            foreach (Node N in _Nodes.Values)
            {
                if (N.Dimension == 1)
                {
                    if (N.Coords[0] == Coords[0]) // node already exists at this location
                    {
                        return true;
                    }
                }
                else if (N.Dimension == 2)
                {
                    if (N.Coords[0] == Coords[0] & N.Coords[1] == Coords[1]) // node already exists at this location
                    {
                        return true;
                    }
                }
                else if (N.Coords[0] == Coords[0] & N.Coords[1] == Coords[1] & N.Coords[2] == Coords[2]) // 3 or 6 DOFs
                                                                                                         // node already exists at this location
                {
                    return true;
                }
            }

            return false;
        }

        private int CreateNodeId()
        {
            int NewID = 1;
            bool IDUnique = false;

            while (_Nodes.Keys.Contains(NewID))
                NewID += 1;

            return NewID;
        }

    }

}
