using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MathNet.Numerics.LinearAlgebra.Double;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FEA_Program
{
    public partial class StressProblem
    {

        private NodeMgr _Nodes;

        public virtual NodeMgr Nodes
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Nodes;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Nodes != null)
                {
                    _Nodes.NodeListChanged -= (_) => ListRedrawNeeded();
                    _Nodes.NodeChanged -= (_) => ListRedrawNeeded();
                    _Nodes.NodeChanged_RedrawOnly -= ScreenRedrawOnlyNeeded;
                    _Nodes.NodeDeleted -= HangingElements;
                }

                _Nodes = value;
                if (_Nodes != null)
                {
                    _Nodes.NodeListChanged += (_) => ListRedrawNeeded();
                    _Nodes.NodeChanged += (_) => ListRedrawNeeded();
                    _Nodes.NodeChanged_RedrawOnly += ScreenRedrawOnlyNeeded;
                    _Nodes.NodeDeleted += HangingElements;
                }
            }
        }
        private ElementMgr _Elements;

        public virtual ElementMgr Elements
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Elements;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Elements != null)
                {
                    _Elements.ElementListChanged -= (_) => ListRedrawNeeded();
                    _Elements.ElementChanged -= (_) => ListRedrawNeeded();
                    _Elements.ElementChanged_RedrawOnly -= ScreenRedrawOnlyNeeded;
                    _Elements.ElementAdded -= ElementCreation;
                    _Elements.ElementDeleted -= ElementDeletion;
                }

                _Elements = value;
                if (_Elements != null)
                {
                    _Elements.ElementListChanged += (_) => ListRedrawNeeded();
                    _Elements.ElementChanged += (_) => ListRedrawNeeded();
                    _Elements.ElementChanged_RedrawOnly += ScreenRedrawOnlyNeeded;
                    _Elements.ElementAdded += ElementCreation;
                    _Elements.ElementDeleted += ElementDeletion;
                }
            }
        }
        private MaterialMgr _Materials;

        public virtual MaterialMgr Materials
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Materials;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Materials != null)
                {
                    _Materials.MatlListChanged -= (_) => ListRedrawNeeded();
                }

                _Materials = value;
                if (_Materials != null)
                {
                    _Materials.MatlListChanged += (_) => ListRedrawNeeded();
                }
            }
        }
        private Connectivity _Connect;

        public virtual Connectivity Connect
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Connect;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _Connect = value;
            }
        }
        public Mainform Loadedform = default;

        private ProblemTypes _Type;

        public Type[] AvailableElements
        {
            get
            {
                switch (_Type)
                {
                    case ProblemTypes.Bar_1D:
                        {
                            return new[] { typeof(ElementMgr.Element_Bar_Linear) };
                            break;
                        }

                    default:
                        {
                            return null;
                        }
                }
            }
        } // which elements are available depending on problem type
        public int AvailableNodeDOFs
        {
            get
            {

                switch (_Type)
                {
                    case ProblemTypes.Bar_1D:
                        {
                            return 1;
                            break;
                        }

                    case ProblemTypes.Beam_1D:
                        {
                            return 1;
                            break;
                        }
                    case ProblemTypes.Truss_3D:
                        {
                            return 3;
                            break;
                        }

                    default:
                        {
                            return default;
                        }
                }
            }
        } // which node DOF should be used for given problem type
        public bool ThreeDimensional
        {
            get
            {
                switch (_Type)
                {
                    case ProblemTypes.Bar_1D:
                        {
                            return false;
                        }

                    case ProblemTypes.Beam_1D:
                        {
                            return false;
                        }

                    case ProblemTypes.Truss_3D:
                        {
                            return true;
                        }

                    default:
                        {
                            return default;
                        }
                }
            }
        } // whether the screen should be 3D based on the problem type
        public Dictionary<string, List<Type>> CommandList
        {
            get
            {
                var output = new Dictionary<string, List<Type>>();

                // ------------- Add Node Command-------------
                var tmp = new List<Type>();

                for (int i = 1, loopTo = AvailableNodeDOFs; i <= loopTo; i++)
                    tmp.Add(typeof(double)); // node position

                for (int i = 1, loopTo1 = AvailableNodeDOFs; i <= loopTo1; i++)
                    tmp.Add(typeof(bool)); // node fixity

                output.Add("AddNode", tmp);

                // ------------- Add Material Command-------------
                var tmp2 = new List<Type>();

                tmp2.Add(typeof(string));
                tmp2.Add(typeof(double));
                tmp2.Add(typeof(double));
                tmp2.Add(typeof(double));
                tmp2.Add(typeof(double));
                tmp2.Add(typeof(int));

                output.Add("AddMaterial", tmp);

                // ------------- Add Element Command-------------

                return output;
            }
        }

        public StressProblem(Mainform form, ProblemTypes Type)
        {
            Nodes = new NodeMgr();
            Elements = new ElementMgr();
            Materials = new MaterialMgr();
            Connect = new Connectivity();
            Loadedform = form;
            _Type = Type;
        }

        private void ListRedrawNeeded()
        {
            Loadedform.ReDrawLists();
            Loadedform.GlCont.Invalidate();
        }
        private void ScreenRedrawOnlyNeeded()
        {
            Loadedform.GlCont.Invalidate();
        }

        private void HangingElements(int NodeID, int Dimension)
        {

            var ElementsToDelete = Connect.get_NodeElements(NodeID);
            Elements.Delete(ElementsToDelete);

        } // deletes elements if a node is deleted and leaves one hanging
        private void ElementCreation(int ElemID, List<int> NodeIDs, Type Type)
        {

            // ---------------------- Get Coords of all of the nodes in the element and then sort Ids

            var NodeCoords = new List<double[]>();

            foreach (int ID in NodeIDs)
                NodeCoords.Add(Nodes.get_NodeObj(ID).Coords);

            Elements.get_ElemObj(ElemID.ToString()).SortNodeOrder(ref NodeIDs, NodeCoords); // nodeIDs is passed byref

            Connect.AddConnection(ElemID, NodeIDs);
        }
        private void ElementDeletion(int ElemID, ElementMgr.IElement Type)
        {
            Connect.RemoveConnection(ElemID);
        }


        public enum ProblemTypes
        {
            Bar_1D,
            Beam_1D,
            Truss_3D
        }

    }


    public partial class Solver
    {

    }


    public partial class Connectivity
    {

        private Dictionary<int, List<int>> _ConnectMatrix = new Dictionary<int, List<int>>(); // dict key is global element ID, list index is local node ID, list value at index is global node ID

        public Dictionary<int, List<int>> ConnectMatrix
        {
            get
            {
                return _ConnectMatrix;
            }
        }

        public List<int> get_ElementNodes(int ElementID)
        {
            return _ConnectMatrix[ElementID];
        } // returns global Node ID's utilized in input element
        public List<int> get_NodeElements(int NodeID)
        {
            var output = new List<int>();

            foreach (KeyValuePair<int, List<int>> KVP in _ConnectMatrix)
            {
                if (KVP.Value.Contains(NodeID)) // check if the nodeID is used in the element
                {
                    output.Add(KVP.Key); // if so add the element ID to the output
                }
            }

            output.Sort(); // sort the output for good measure (lowest element ID comes first)
            return output;
        } // returns all of the element ID's attached to a global node ID

        public void AddConnection(int ElementID, List<int> NodeIDs)
        {
            _ConnectMatrix.Add(ElementID, NodeIDs);
        } // nodeIDs need to be sorted in the correct local order for the element
        public void RemoveConnection(int ElementID)
        {
            _ConnectMatrix.Remove(ElementID);
        }


        private void SortNodeLocalIDs(Dictionary<int, Dictionary<int, double[]>> NodeCoords)
        {

        }
        public SparseMatrix Assemble_K_Mtx(Dictionary<int, DenseMatrix> K_matricies, Dictionary<int, int> NodeDOFs)
        {

            // ---------------------- Get Total Size of the Problem --------------------------

            int problemSize = 0;
            foreach (int DOF in NodeDOFs.Values)
                problemSize += DOF;

            // ----------------------- Setup Output Matrix -----------------------------------

            var output = new SparseMatrix(problemSize, problemSize);

            // ------------------------Iterate Through Each Node ID and allocate regions of large matrix --------------------------

            var NodeIDs = NodeDOFs.Keys.ToList();
            NodeIDs.Sort(); // sort so smallest ID comes first

            var NodeMatrixIndicies = new Dictionary<int, int[]>(); // determines where each displacement will go on the assembled matrix, sorted by node matrix
            int IndexCounter = 0; // to count how many places have been used up in the matrix

            foreach (int ID in NodeIDs)
            {
                int DOF = NodeDOFs[ID]; // get the number of DOFs for the node
                var allocatedIndicies = new int[DOF]; // create array to hold allocated indicies

                for (int i = 0, loopTo = DOF - 1; i <= loopTo; i++) // allocate a value for each DOF incrementally based on the number of DOFs
                {
                    allocatedIndicies[i] = IndexCounter;
                    IndexCounter += 1;
                }

                NodeMatrixIndicies.Add(ID, allocatedIndicies);
            }

            // ------------------------Iterate Through Each Element --------------------------

            foreach (KeyValuePair<int, DenseMatrix> ElemID_and_K in K_matricies)
            {
                // 1. get node ID's - assume in correct order
                var E_nodeIDs = get_ElementNodes(ElemID_and_K.Key);

                // 2. Allocate Regions of the K Matrix for Each Element

                var NodeKmtxIndicies = new Dictionary<int, int[]>(); // holds which regions of the k matrix are claimed by each node
                IndexCounter = 0; // to count how many places have been used up in the matrix

                foreach (int ID in E_nodeIDs)
                {
                    int DOF = NodeDOFs[ID]; // get the number of DOFs for the node
                    var allocatedIndicies = new int[DOF]; // create array to hold allocated indicies

                    for (int i = 0, loopTo1 = DOF - 1; i <= loopTo1; i++) // allocate a value for each DOF incrementally based on the number of DOFs
                    {
                        allocatedIndicies[i] = IndexCounter;
                        IndexCounter += 1;
                    }

                    NodeKmtxIndicies.Add(ID, allocatedIndicies);
                }

                // 3. Move the value range for each node from the local K matrix to the global

                foreach (int i in E_nodeIDs)
                {
                    foreach (int j in E_nodeIDs)
                    {

                        for (int row = 0, loopTo2 = NodeDOFs[i] - 1; row <= loopTo2; row++)
                        {
                            for (int col = 0, loopTo3 = NodeDOFs[j] - 1; col <= loopTo3; col++)
                            {

                                int[] assembled_K_nodeRegion_i = NodeMatrixIndicies[i];
                                int[] assembled_K_nodeRegion_j = NodeMatrixIndicies[j];

                                int[] local_K_nodeRegion_i = NodeKmtxIndicies[i];
                                int[] local_K_nodeRegion_j = NodeKmtxIndicies[j];


                                output(assembled_K_nodeRegion_i[row], assembled_K_nodeRegion_j[col]) = ElemID_and_K.Value(local_K_nodeRegion_i[row], local_K_nodeRegion_j[col]);
                            }
                        }

                    }
                }

            }

            return output;
        }
        public DenseMatrix[] Solve(SparseMatrix K, DenseMatrix F, DenseMatrix Q)
        {

            var FixedIndicies = new List<int>();
            int ProblemSize = Q.RowCount;

            for (int i = 0, loopTo = ProblemSize - 1; i <= loopTo; i++) // get indicies of each fixed displacement
            {
                if (Q(i, 0) == 1)
                {
                    FixedIndicies.Add(i);
                }
            }

            FixedIndicies.Reverse(); // need to remove rows with highest index first

            foreach (int Val in FixedIndicies) // first remove columns - they will be multiplied by 0 anyway - rows are needed later for reaction forces
                K = K.RemoveColumn(Val);

            var Reaction_K_Mtx = new DenseMatrix(FixedIndicies.Count, K.ColumnCount); // need to keep removed values to calculate reaction forces
            var Reaction_F_Mtx = new DenseMatrix(FixedIndicies.Count, 1);

            for (int i = 0, loopTo1 = FixedIndicies.Count - 1; i <= loopTo1; i++)
            {
                Reaction_K_Mtx.SetRow(FixedIndicies.Count - i - 1, K.Row(FixedIndicies[i])); // save values which are going to be removed from K
                Reaction_F_Mtx.Item(FixedIndicies.Count - i - 1, 0) -= F.Item(FixedIndicies[i], 0); // adds any forces that are pointed against the direction of reaction forces
            }

            foreach (int Val in FixedIndicies) // remove rows not needed for solving displacements
            {
                K = K.RemoveRow(Val);
                F = F.RemoveRow(Val);
            }

            SparseMatrix Q_Solved = K.Solve(F); // solve displacements
            Reaction_F_Mtx += Reaction_K_Mtx.Multiply(Q_Solved); // add reactions due to displacements

            var Q_output = new DenseMatrix(ProblemSize, 1);
            var R_output = new DenseMatrix(ProblemSize, 1);

            int k_int = 0;
            int j = 0;
            for (int i = 0, loopTo2 = ProblemSize - 1; i <= loopTo2; i++)
            {
                if (FixedIndicies.Contains(i)) // this row has been fixed
                {
                    Q_output.Item(i, 0) = 0;
                    R_output.Item(i, 0) = Reaction_F_Mtx(k_int, 0);
                    k_int += 1;
                }
                else // floating row
                {
                    Q_output.Item(i, 0) = Q_Solved(j, 0);
                    R_output.Item(i, 0) = 0; // reaction must be 0 for a floating displacement
                    j += 1;
                }
            }

            return new[] { Q_output, R_output };
        }


    } // need to call functions in here from element/node events upon creation/deletion

    public partial class MaterialMgr
    {

        private Dictionary<int, MaterialClass> _Materials = new Dictionary<int, MaterialClass>(); // reference nodes by ID

        public event MatlListChangedEventHandler MatlListChanged;

        public delegate void MatlListChangedEventHandler(Dictionary<int, MaterialClass> MatlList); // Length of Matllist has changed

        public MaterialClass get_MatObj(int ID)
        {
            return _Materials[ID];
        }
        public List<int> AllIDs
        {
            get
            {
                return _Materials.Keys.ToList();
            }
        }
        public List<MaterialClass> MatList
        {
            get
            {
                return _Materials.Values.ToList();
            }
        }
        public Dictionary<int, double> All_E
        {
            get
            {
                var output = new Dictionary<int, double>();

                foreach (MaterialClass Mat in _Materials.Values)
                    output.Add(Mat.ID, Mat.E);
                return output;
            }
        }


        public void Add(string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, MaterialType subtype)
        {

            int ID = CreateMatlId();
            var mat = new MaterialClass(Name, E_GPa, V, Sy_MPa, Sut_MPa, subtype, ID);
            _Materials.Add(ID, mat);

            MatlListChanged?.Invoke(_Materials);

        }
        public void Delete(int ID)
        {
            _Materials.Remove(ID);

            MatlListChanged?.Invoke(_Materials);
        }

        private int CreateMatlId()
        {
            int NewID = 1;
            bool IDUnique = false;

            while (_Materials.Keys.Contains(NewID))
                NewID += 1;

            return NewID;
        }


        public partial class MaterialClass
        {
            private string _Name = "";
            private double _E = 0d; // youngs modulus in Pa
            private double _V = 0d; // poissons ratio
            private double _Sy = 0d; // yield strength in Pa
            private double _Sut = 0d; // ultimate strength in Pa
            private MaterialType _subtype;
            private int _ID = -1;


            public int ID
            {
                get
                {
                    return _ID;
                }
            }
            public string Name
            {
                get
                {
                    return _Name;
                }
            }
            public double Sy_MPa
            {
                get
                {
                    return _Sy / (1000.0d * 1000.0d);
                }
            }
            public double Sut_MPa
            {
                get
                {
                    return _Sut / (1000.0d * 1000.0d);
                }
            }
            public double E_GPa
            {
                get
                {
                    return _E / (1000 * 1000 * 1000); // convert to GPa
                }
            }
            public double E
            {
                get
                {
                    return _E;
                }
            }
            public double V
            {
                get
                {
                    return _V;
                }
            }
            public MaterialType Subtype
            {
                get
                {
                    return _subtype;
                }
            }
            public DenseMatrix D_matrix_2D
            {
                get
                {
                    var output = new DenseMatrix(3, 3);
                    output.Clear(); // set all vals to 0

                    output(0, 0) = 1;
                    output(0, 1) = _V;
                    output(1, 0) = _V;
                    output(1, 1) = 1;
                    output(2, 2) = (1d - _V) / 2d;

                    output = output * _E / (1d - _V * _V);

                    return output;
                }
            }

            public MaterialClass(string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, MaterialType subtype, int InputID)
            {

                _Name = Name;
                _E = E_GPa * 1000d * 1000d * 1000d; // convert to Pa
                _V = V;
                _Sy = Sy_MPa * 1000d * 1000d; // convert to Pa
                _Sut = Sut_MPa * 1000d * 1000d; // convert to Pa
                _subtype = subtype;
                _ID = InputID;
            }


        }
        public enum MaterialType
        {
            Steel_Alloy,
            Aluminum_Alloy
        }

    }

    public partial class NodeMgr
    {

        private Dictionary<int, Node> _Nodes = new Dictionary<int, Node>(); // reference nodes by ID

        public event NodeListChangedEventHandler NodeListChanged;

        public delegate void NodeListChangedEventHandler(Dictionary<int, Node> NodeList); // Length of nodelist has changed
        public event NodeChangedEventHandler NodeChanged;

        public delegate void NodeChangedEventHandler(List<int> IDs); // Node has changed such that list needs to be updated & screen redrawn
        public event NodeChanged_RedrawOnlyEventHandler NodeChanged_RedrawOnly;

        public delegate void NodeChanged_RedrawOnlyEventHandler(); // Node has changed such that screen only needs to be redrawn
        public event NodeAddedEventHandler NodeAdded;

        public delegate void NodeAddedEventHandler(int NodeID, int Dimension); // dont use for redrawing lists or screen
        public event NodeDeletedEventHandler NodeDeleted;

        public delegate void NodeDeletedEventHandler(int NodeID, int Dimension); // dont use for redrawing lists or screen

        public List<Node> Nodelist
        {
            get
            {
                return _Nodes.Values.ToList();
            }
        }
        public List<int> AllIDs
        {
            get
            {
                var output = _Nodes.Keys.ToList();
                output.Sort();
                return output;
            }
        } // all node ids
        public Node get_NodeObj(int ID)
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

                        output(currentrow, 0) = _Nodes[ids[i]].Force[j];
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

                        output(currentrow, 0) = _Nodes[ids[i]].Fixity[j];
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

                var newnode = new Node(Coords[i], Fixity[i], CreateNodeId(), Dimensions[i]);
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
                    reactions.Add(R(currentRow, 0));
                    displacements.Add(Q(currentRow, 0));

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

        public partial class Node
        {

            // ---------------------- ALL MEMBERS HERE SHOWN FOR 6D NODE, MEMBERS ARE SHORTENED ACCORDINGLY BY DIMENSION

            private double[] _Coords = new[] { 0d, 0d, 0d, 0d, 0d, 0d }; // coordinates in m
            private double[] _Disp = new[] { 0d, 0d, 0d, 0d, 0d, 0d }; // displacement in m
            private int[] _Fixity = new[] { 0, 0, 0, 0, 0, 0 }; // 0 = floating, 1 = fixed

            private double[] _Force = new[] { 0d, 0d, 0d, 0d, 0d, 0d }; // first 3 items  = force [N], last 3 = moments [Nm]
            private double[] _ReactionForce = new[] { 0d, 0d, 0d, 0d, 0d, 0d }; // reaction force in [N], reaction moments in [Nm]

            private int _ID = -1;
            private int _Dimensions = 0; // 1 = 1D, 2 = 2D, 3 = 3D, 6 = 6D
            private int[] _ValidDimensions = new[] { 1, 2, 3, 6 }; // provides a list of available dimsensions for error checking

            private Color _DefaultColor = Color.Blue;
            private Color _DefaultForceColor = Color.Purple;
            private Color _DefaultFixityColor = Color.Red;
            private Color _FixityColor = Color.Red;
            private Color _Color = Color.Blue;
            private Color _ForceColor = Color.Purple;
            private Color _SelectedColor = Color.Yellow;
            private Color _ReactionColor = Color.Green;

            private bool _SolutionValid = false;

            public event SolutionInvalidatedEventHandler SolutionInvalidated;

            public delegate void SolutionInvalidatedEventHandler(int NodeID);

            public bool Selected
            {
                get
                {
                    if (_Color == _SelectedColor)
                    {
                        return true;
                    }
                    return false;
                }
                set
                {
                    if (value)
                    {
                        _Color = _SelectedColor;
                        _ForceColor = _SelectedColor;
                        _FixityColor = _SelectedColor;
                    }
                    else if (_Color == _SelectedColor)
                    {
                        _Color = _DefaultColor;
                        _ForceColor = _DefaultForceColor;
                        _FixityColor = _DefaultFixityColor;
                    }
                }
            }
            public double[] Coords_mm
            {
                get
                {
                    var output = new double[_Dimensions];

                    for (int i = 0, loopTo = _Coords.Length - 1; i <= loopTo; i++)
                        output[i] = _Coords[i] * 1000.0d; // convert to mm

                    return output;
                }
            }
            public double[] Coords
            {
                get
                {
                    return _Coords;
                }
                set
                {
                    if (value.Length != _Dimensions)
                    {
                        throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                    }

                    _Coords = value;
                    InvalidateSolution();
                }
            }
            public double[] Force
            {
                get
                {
                    return _Force;
                }
                set
                {
                    if (value.Length != _Dimensions)
                    {
                        throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                    }

                    _Force = value;
                    InvalidateSolution();
                }
            }
            public int[] Fixity
            {
                get
                {
                    return _Fixity;
                }
                set
                {
                    if (value.Length != _Dimensions)
                    {
                        throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                    }

                    _Fixity = value;
                    InvalidateSolution();
                }
            }
            public double ForceMagnitude
            {
                get
                {
                    Vector3 output;

                    if (_Dimensions == 1)
                    {
                        output = new Vector3(_Force[0], 0, 0);
                    }

                    else if (_Dimensions == 2)
                    {
                        output = new Vector3(_Force[0], _Force[1], 0);
                    }

                    else // dimensions = 3 or 6
                    {
                        output = new Vector3(_Force[0], _Force[1], _Force[2]);

                    }

                    return output.Length;
                }
            } // will eventually need moment functions too
            public double[] ForceDirection
            {
                get
                {
                    Vector3 output;

                    if (_Dimensions == 1)
                    {
                        output = new Vector3(_Force[0], 0, 0);
                        output.Normalize();
                        return new[] { output.X };
                    }

                    else if (_Dimensions == 2)
                    {
                        output = new Vector3(_Force[0], _Force[1], 0);
                        output.Normalize();
                        return new[] { output.X, output.Y };
                    }

                    else // dimensions = 3 or 6
                    {
                        output = new Vector3(_Force[0], _Force[1], _Force[2]);
                        output.Normalize();
                        return new[] { output.X, output.Y, output.Z };

                    }
                }
            }
            public double[] Displacement
            {
                get
                {
                    return _Disp;
                }
            }
            public double[] ReactionForce
            {
                get
                {
                    return _ReactionForce;
                }
            }
            public double[] FinalPos
            {
                get
                {
                    var output = new double[_Dimensions + 1];

                    for (int i = 0, loopTo = _Dimensions - 1; i <= loopTo; i++)
                        output[i] = _Coords[i] + _Disp[i]; // add disp to each coord

                    return output;
                }
            }
            public int Dimension
            {
                get
                {
                    return _Dimensions;
                }
            }
            public int ID
            {
                get
                {
                    return _ID;
                }
            }

            public Node(double[] NewCoords, int[] NewFixity, int NewID, int Dimensions)
            {

                if (_ValidDimensions.Contains(Dimensions) == false)
                {
                    throw new Exception("Attempted to create element, ID <" + NewID.ToString() + "> with invalid number of dimensions: " + Dimensions.ToString());
                }

                if (NewCoords.Length != Dimensions | NewFixity.Length != Dimensions)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                }

                _Coords = NewCoords;
                _Fixity = NewFixity;
                _ID = NewID;
                _Dimensions = Dimensions;
            }
            public void Solve(double[] Disp, double[] R)
            {

                if (Disp.Length != _Dimensions | Disp.Length != _Dimensions)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                }

                _Disp = Disp;
                _ReactionForce = R;
                _SolutionValid = true;
            }
            public void DrawNode(double[] N_mm)
            {

                if (N_mm.Length != _Dimensions)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                }

                DrawNode(N_mm, _Color);
                DrawForce(N_mm, _Force, _ForceColor);
                DrawFixity(N_mm, _Fixity, _FixityColor);
            } // draw always has mm input
            public void DrawReaction(double[] N_mm)
            {

                if (N_mm.Length != _Dimensions)
                {
                    throw new Exception("Attempted to execute sub <" + MethodBase.GetCurrentMethod().Name + "> for node ID <" + _ID.ToString() + "> with input params having different dimensions than specified node dimension.");
                }

                DrawForce(N_mm, _ReactionForce, _ReactionColor);
            } // draw always has mm input


            public double[] CalcDispIncrementPos_mm(double Percentage, double ScaleFactor)
            {
                var output = new double[_Dimensions + 1];

                for (int i = 0, loopTo = _Coords.Length - 1; i <= loopTo; i++)
                    output[i] = (_Coords[i] + _Disp[i] * Percentage * ScaleFactor) * 1000.0d; // convert to mm

                return output;
            }


            // ---------- Draw is not yet setup for 6D nodes ---- need to be able to display rotation displacement

            private void DrawNode(double[] N, Color Color)
            {

                double[] tmp = null;

                if (_Dimensions == 1)
                {
                    tmp = new[] { N[0], 0d, 0d };
                }

                else if (_Dimensions == 2)
                {
                    tmp = new[] { N[0], N[1], 0d };
                }

                else // dimensions 3 or 6
                {
                    tmp = N;
                }

                GL.Color3(Color);
                GL.Begin(PrimitiveType.Quads);

                if (_Dimensions == 1 | _Dimensions == 2)
                {
                    GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2]);
                    GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2]);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2]);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2]);
                }

                else
                {
                    GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] - 1d);
                    GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] - 1d);

                    GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] - 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] - 1d);

                    GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] + 1d);

                    GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] - 1d);
                    GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] - 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] - 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] - 1d);

                    GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] + 1d, tmp[2] - 1d);
                    GL.Vertex3(tmp[0] + 1d, tmp[1] + 1d, tmp[2] - 1d);

                    GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] + 1d);
                    GL.Vertex3(tmp[0] - 1d, tmp[1] - 1d, tmp[2] - 1d);
                    GL.Vertex3(tmp[0] + 1d, tmp[1] - 1d, tmp[2] - 1d);

                }
                GL.End();
            } // draw always has mm input
            private void DrawForce(double[] N, double[] F, Color Color)
            {
                if (Array.TrueForAll(F, ValEqualsZero) == false) // dont draw a force if its zero
                {

                    double[] tmp = null;
                    double forcelength = 10.0d;
                    Vector3 vect = default;

                    if (_Dimensions == 1)
                    {
                        tmp = new[] { N[0], 0d, 0d };
                        vect = new Vector3(F[0], 0, 0);
                    }

                    else if (_Dimensions == 2)
                    {
                        tmp = new[] { N[0], N[1], 0d };
                        vect = new Vector3(F[0], F[1], 0);
                    }

                    else // dimensions 3 or 6
                    {
                        tmp = N;
                        vect = new Vector3(F[0], F[1], F[2]);
                    }

                    vect.Normalize();

                    // ------------------- create normal vectors to use for drawing arrows -----------------
                    var X = new Vector3(1, 0, 0);
                    Vector3 normal1 = Vector3.Cross(vect, X);
                    normal1 = Vector3.Multiply(normal1, forcelength * 0.2d);

                    var Z = new Vector3(0, 0, 1);
                    Vector3 normal2 = Vector3.Cross(vect, Z);
                    normal2 = Vector3.Multiply(normal2, forcelength * 0.2d);

                    // ---------------- set arrow to proper length after calculating normals ---------
                    vect = Vector3.Multiply(vect, forcelength);

                    GL.Color3(Color);
                    GL.Begin(PrimitiveType.Lines);

                    GL.Vertex3(tmp[0], tmp[1], tmp[2]);
                    GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);

                    GL.End();

                    GL.Begin(PrimitiveType.Triangles);
                    Vector3 endPt = Vector3.Multiply(vect, 0.8d);
                    GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);
                    GL.Vertex3(tmp[0] + endPt.X + normal1.X, tmp[1] + endPt.Y + normal1.Y, tmp[2] + endPt.Z + normal1.Z);
                    GL.Vertex3(tmp[0] + endPt.X + normal2.X, tmp[1] + endPt.Y + normal2.Y, tmp[2] + endPt.Z + normal2.Z);

                    GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);
                    GL.Vertex3(tmp[0] + endPt.X - normal1.X, tmp[1] + endPt.Y - normal1.Y, tmp[2] + endPt.Z - normal1.Z);
                    GL.Vertex3(tmp[0] + endPt.X - normal2.X, tmp[1] + endPt.Y - normal2.Y, tmp[2] + endPt.Z - normal2.Z);

                    GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);
                    GL.Vertex3(tmp[0] + endPt.X + normal1.X, tmp[1] + endPt.Y + normal1.Y, tmp[2] + endPt.Z + normal1.Z);
                    GL.Vertex3(tmp[0] + endPt.X - normal2.X, tmp[1] + endPt.Y - normal2.Y, tmp[2] + endPt.Z - normal2.Z);

                    GL.Vertex3(tmp[0] + vect.X, tmp[1] + vect.Y, tmp[2] + vect.Z);
                    GL.Vertex3(tmp[0] + endPt.X - normal1.X, tmp[1] + endPt.Y - normal1.Y, tmp[2] + endPt.Z - normal1.Z);
                    GL.Vertex3(tmp[0] + endPt.X + normal2.X, tmp[1] + endPt.Y + normal2.Y, tmp[2] + endPt.Z + normal2.Z);

                    GL.End();
                }
            } // draw always has mm input
            private void DrawFixity(double[] N, int[] Fix, Color Color)
            {
                double squareoffset = 1.5d;
                double[] tmp = null;

                if (_Dimensions == 1)
                {
                    tmp = new[] { N[0], 0d, 0d };
                }

                else if (_Dimensions == 2)
                {
                    tmp = new[] { N[0], N[1], 0d };
                }

                else // dimensions 3 or 6
                {
                    tmp = N;
                }

                GL.Color3(Color); // set drawing color


                if (Fix[0] == 1) // X Axis
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex3(tmp[0], tmp[1] + squareoffset, tmp[2] + squareoffset);
                    GL.Vertex3(tmp[0], tmp[1] - squareoffset, tmp[2] + squareoffset);
                    GL.Vertex3(tmp[0], tmp[1] - squareoffset, tmp[2] - squareoffset);
                    GL.Vertex3(tmp[0], tmp[1] + squareoffset, tmp[2] - squareoffset);
                    GL.End();
                }

                if (_Dimensions > 1) // or else will error when searching for invalid value
                {
                    if (Fix[1] == 1) // Y Axis
                    {
                        GL.Begin(PrimitiveType.Quads);
                        GL.Vertex3(tmp[0] + squareoffset, tmp[1], tmp[2] + squareoffset);
                        GL.Vertex3(tmp[0] - squareoffset, tmp[1], tmp[2] + squareoffset);
                        GL.Vertex3(tmp[0] - squareoffset, tmp[1], tmp[2] - squareoffset);
                        GL.Vertex3(tmp[0] + squareoffset, tmp[1], tmp[2] - squareoffset);
                        GL.End();
                    }
                }

                if (_Dimensions > 2) // or else will error when searching for invalid value
                {
                    if (Fix[2] == 1) // Z Axis
                    {
                        GL.Begin(PrimitiveType.Quads);
                        GL.Vertex3(tmp[0] + squareoffset, tmp[1] + squareoffset, tmp[2]);
                        GL.Vertex3(tmp[0] - squareoffset, tmp[1] + squareoffset, tmp[2]);
                        GL.Vertex3(tmp[0] - squareoffset, tmp[1] - squareoffset, tmp[2]);
                        GL.Vertex3(tmp[0] + squareoffset, tmp[1] - squareoffset, tmp[2]);
                        GL.End();
                    }
                }
            } // draw always has mm input
            private bool ValEqualsZero(double value)
            {
                if (value == 0d)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }  // used to check if all force values are 0 for drawing


            private void InvalidateSolution()
            {
                _SolutionValid = false;
                SolutionInvalidated?.Invoke(_ID);
            }

        }
    }

    public partial class ElementMgr
    {

        private static Color _SelectedColor = Color.Yellow; // the default color of selected objects for the program

        private Dictionary<int, IElement> _Bar1Elements = new Dictionary<int, IElement>(); // reference elements by ID

        public event ElementListChangedEventHandler ElementListChanged;

        public delegate void ElementListChangedEventHandler(Dictionary<int, IElement> ElemList); // Length of Elementlist has changed
        public event ElementChangedEventHandler ElementChanged;

        public delegate void ElementChangedEventHandler(int ID); // Element has changed such that list needs to be updated & screen redrawn
        public event ElementChanged_RedrawOnlyEventHandler ElementChanged_RedrawOnly;

        public delegate void ElementChanged_RedrawOnlyEventHandler(); // Element has changed such that screen only needs to be redrawn
        public event ElementAddedEventHandler ElementAdded;

        public delegate void ElementAddedEventHandler(int ElemID, List<int> NodeIDs, Type Type); // dont use for redrawing lists or screen
        public event ElementDeletedEventHandler ElementDeleted;

        public delegate void ElementDeletedEventHandler(int ElemID, IElement Type); // dont use for redrawing lists or screen

        public static int get_NumOfNodes(Type ElemType)
        {
            switch (ElemType)
            {
                case var @case when @case == typeof(Element_Bar_Linear):
                    {
                        return 2;
                        break;
                    }

                default:
                    {
                        return default;
                    }
            }
        }
        public static int get_NodeDOFs(Type ElemType)
        {
            switch (ElemType)
            {
                case var @case when @case == typeof(Element_Bar_Linear):
                    {
                        return 1;
                        break;
                    }

                default:
                    {
                        return default;
                    }
            }
        }
        public static string get_Name(Type ElemType)
        {
            switch (ElemType)
            {
                case var @case when @case == typeof(Element_Bar_Linear):
                    {
                        return "Bar_Linear";
                        break;
                    }

                default:
                    {
                        return null;
                    }
            }
        }
        public static int get_ElemDOFs(Type ElemType)
        {
            return get_NumOfNodes(ElemType) * get_NodeDOFs(ElemType);
        }
        public static Dictionary<string, Units.DataUnitType> get_ElementArgs(Type ElemType)
        {
            var output = new Dictionary<string, Units.DataUnitType>();

            switch (ElemType)
            {

                case var @case when @case == typeof(Element_Bar_Linear):
                    {
                        output.Add("Area", Units.DataUnitType.Area);
                        break;
                    }

                default:
                    {
                        return null;
                    }

            }

            return output;
        }


        public List<IElement> Elemlist
        {
            get
            {
                return _Bar1Elements.Values.ToList();
            }
        }
        public List<int> AllIDs
        {
            get
            {
                var output = _Bar1Elements.Keys.ToList();
                output.Sort();
                return output;
            }
        }
        public IElement get_ElemObj(string ElemID)
        {
            return _Bar1Elements[Conversions.ToInteger(ElemID)];
        }
        public Dictionary<int, DenseMatrix> get_K_matricies(Dictionary<int, List<int>> ConnectMatrix, Dictionary<int, double[]> NodeCoords, Dictionary<int, double> E)
        {
            var output = new Dictionary<int, DenseMatrix>();

            foreach (int E_ID in ConnectMatrix.Keys) // iterate through each element
            {
                var ElemNodeCoords = new List<double[]>();

                foreach (int NodeID in ConnectMatrix[E_ID]) // get the coordinates of each node in the element
                    ElemNodeCoords.Add(NodeCoords[NodeID]);

                int MatID = _Bar1Elements[E_ID].Material;

                output.Add(E_ID, _Bar1Elements[E_ID].get_K_mtrx(ElemNodeCoords, E[MatID]));
            }

            return output;
        }


        public void Add(Type Type, List<int> NodeIDs, double[] ElementArgs, int Mat = -1)
        {
            IElement newElem = null;
            int newElemID = CreateId();

            // ------------------ Determine type of element ----------------------

            if (ReferenceEquals(Type, typeof(Element_Bar_Linear))) // linear bar element --------------------------------
            {

                if (ElementArgs[0] > 0d)
                {
                    newElem = new Element_Bar_Linear(ElementArgs[0], newElemID, Mat);
                }

                else
                {
                    throw new Exception("Tried to add element <" + Type.ToString() + ">, ID <" + newElemID + "> with invalid area:" + ElementArgs[0].ToString());
                }
            }

            else
            {
                throw new Exception("Tried to add element with invalid type.");
                return;
            }  // dont want to add anything to the list or raise events

            // -------------------- check for errors and if valid add then raise events -----------------

            if (NodeIDs.Count != get_NumOfNodes(newElem.GetType())) // check if the right number of nodes are listed
            {
                throw new Exception("Tried to add element of type <" + Type.ToString() + "> with " + NodeIDs.Count.ToString() + " nodes. Should have " + get_NumOfNodes(newElem.GetType()) + " nodes.");
            }

            if (newElem is not null) // more error checking
            {
                _Bar1Elements.Add(newElem.ID, newElem);
                ElementAdded?.Invoke(newElem.ID, NodeIDs, newElem.GetType());
                ElementListChanged?.Invoke(_Bar1Elements);
            }

        } // nodeIDs only used to raise event about generation
        public void SelectElems(int[] IDs, bool selected)
        {
            foreach (int item in IDs)
                _Bar1Elements[item].Selected = selected;
            ElementChanged_RedrawOnly?.Invoke();
        }
        public void Delete(List<int> IDs)
        {
            for (int i = 0, loopTo = IDs.Count - 1; i <= loopTo; i++)
            {
                var tmp = _Bar1Elements[IDs[i]]; // save temporarily so we can raise event after deletion
                _Bar1Elements.Remove(IDs[i]);
                ElementDeleted?.Invoke(tmp.ID, (IElement)tmp.GetType());
            }

            if (IDs.Count > 0)
            {
                ElementListChanged?.Invoke(_Bar1Elements);
            }
        }

        private int CreateId()
        {
            int NewID = 1;
            bool IDUnique = false;

            while (_Bar1Elements.Keys.Contains(NewID))
                NewID += 1;

            return NewID;
        }


        // -------------------------- Classes & interfaces --------------------------------

        public partial class Element_Bar_Linear : Element, IElement
        {

            private double _Area = 0d; // x-section area in m^2
            private double _BodyForce = 0d; // Body force in N/m^3
            private double _TractionForce = 0d; // Traction force in N/m

            public override Type MyType
            {
                get
                {
                    return GetType();
                }
            }
            private DenseMatrix get_N_mtrx(double[] IntrinsicCoords)
            {
                if (IntrinsicCoords.Length != 1)
                {
                    throw new Exception("Wrong number of coords input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                double eta = IntrinsicCoords[0];

                var N = new DenseMatrix(1, NodeDOFs * NumOfNodes); // u = Nq - size based off total number of element DOFs
                N(0, 0) = (1d - eta) / 2.0d;
                N(0, 1) = (1d + eta) / 2.0d;

                return N;
            }
            public void get_Interpolated_Displacement(double[] IntrinsicCoords, DenseMatrix GblNodeQ)
            {
                if (GblNodeQ.Values.Count != ElemDOFs)
                {
                    throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                return get_N_mtrx(IntrinsicCoords) * GblNodeQ;
            } // can interpolate either position or displacement
            public DenseMatrix get_B_mtrx(List<double[]> GblNodeCoords)
            {
                if (GblNodeCoords.Count != NumOfNodes)
                {
                    throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                var B_out = new DenseMatrix(1, NodeDOFs * NumOfNodes); // based from total DOFs
                B_out(0, 0) = -1.0d;
                B_out(0, 1) = 1.0d;

                B_out = B_out * (1d / get_Length(GblNodeCoords));  // B = [-1 1]*1/(x2-x1)

                return B_out;
            } // needs to be given with local node 1 in first spot on list
            public double get_Length(List<double[]> GblNodeCoords)
            {
                if (GblNodeCoords.Count != NumOfNodes)
                {
                    throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                double output = GblNodeCoords[1][0] - GblNodeCoords[0][0];

                if (output < 0d)
                {
                    throw new Exception("Nodes given in wrong order to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                return output;
            }
            public DenseMatrix get_Stress_mtrx(List<double[]> GblNodeCoords, DenseMatrix GblNodeQ, double E, double[] IntrinsicCoords = null)
            {
                if (GblNodeCoords.Count != NumOfNodes | GblNodeQ.Values.Count != ElemDOFs)
                {
                    throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                DenseMatrix output = E * get_B_mtrx(GblNodeCoords) * GblNodeQ;
                return output;
            } // node 1 displacement comes first in disp input, followed by second
            public DenseMatrix get_K_mtrx(List<double[]> GblNodeCoords, double E)
            {
                if (GblNodeCoords.Count != NumOfNodes)
                {
                    throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                var output = new DenseMatrix(ElemDOFs, ElemDOFs);
                output(0, 0) = 1;
                output(1, 0) = -1;
                output(0, 1) = -1;
                output(1, 1) = 1;

                output = output * E * _Area / get_Length(GblNodeCoords);

                return output;
            } // node 1 displacement comes first in disp input, followed by second
            public DenseMatrix get_BodyForce_mtrx(List<double[]> GblNodeCoords)
            {
                if (GblNodeCoords.Count != ElemDOFs)
                {
                    throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                var output = new DenseMatrix(ElemDOFs, 1);
                output(0, 0) = 1;
                output(1, 0) = 1;

                output = output * _Area * get_Length(GblNodeCoords) * _BodyForce * 0.5d;

                return output;
            } // node 1 displacement comes first in disp input, followed by second
            public DenseMatrix get_TractionForce_mtrx(List<double[]> GblNodeCoords)
            {
                if (GblNodeCoords.Count != NumOfNodes)
                {
                    throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                var output = new DenseMatrix(ElemDOFs, 1);
                output(0, 0) = 1;
                output(1, 0) = 1;

                output = output * get_Length(GblNodeCoords) * _TractionForce * 0.5d;

                return output;
            }

            public Element_Bar_Linear(double Area, int ID, int Mat = -1) : base(System.Drawing.Color.Green, ID, Mat)
            {

                _Area = Area;
            }

            public void SortNodeOrder(ref List<int> NodeIDs, List<double[]> NodeCoords)
            {
                if (NodeIDs.Count != NumOfNodes | NodeCoords.Count != NumOfNodes) // error handling
                {
                    throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ".");
                }

                var SortedIdList = new List<int>();

                if (NodeCoords[0][0] < NodeCoords[1][0]) // larger x-coord is local element 2
                {
                    SortedIdList.Add(NodeIDs[0]);
                    SortedIdList.Add(NodeIDs[1]);
                }
                else
                {
                    SortedIdList.Add(NodeIDs[1]);
                    SortedIdList.Add(NodeIDs[0]);
                }

                NodeIDs = SortedIdList;
            }
            public void SetBodyForce(DenseMatrix ForcePerVol)
            {
                if (ForcePerVol.Values.Count != NodeDOFs) // can only have forces in directions of DOFs
                {
                    throw new Exception("Wrong number of Forces input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                _BodyForce = ForcePerVol(0, 0);
                InvalidateSolution();
            }
            public void SetTractionForce(DenseMatrix ForcePerLength)
            {
                if (ForcePerLength.Values.Count != NodeDOFs) // can only have forces in directions of DOFs
                {
                    throw new Exception("Wrong number of Forces input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                _BodyForce = ForcePerLength(0, 0);
                InvalidateSolution();
            }
            public void Draw(List<double[]> GblNodeCoords)
            {

                if (GblNodeCoords.Count != NumOfNodes)
                {
                    throw new Exception("Wrong number of Nodes input to " + MethodBase.GetCurrentMethod().Name + ". Node: " + ID.ToString());
                }

                GL.LineWidth(2);

                GL.Begin(PrimitiveType.Lines);

                if (NodeDOFs == 1)
                {
                    for (int i = 0, loopTo = NumOfNodes - 1; i <= loopTo; i++)
                    {
                        GL.Color3(_Color[i]);
                        GL.Vertex3(GblNodeCoords[i][0], 0, 0);
                    }
                }
                else if (NodeDOFs == 2)
                {
                    for (int i = 0, loopTo2 = NumOfNodes - 1; i <= loopTo2; i++)
                    {
                        GL.Color3(_Color[i]);
                        GL.Vertex3(GblNodeCoords[i][0], GblNodeCoords[i][1], 0);
                    }
                }
                else
                {
                    for (int i = 0, loopTo1 = NumOfNodes - 1; i <= loopTo1; i++)
                    {
                        GL.Color3(_Color[i]);
                        GL.Vertex3(GblNodeCoords[i][0], GblNodeCoords[i][1], GblNodeCoords[i][2]);
                    }
                }

                GL.End();

                GL.LineWidth(1);
            }

        }
        public abstract partial class Element : IBaseElement
        {

            private int _NumOfNodes; // holds value only for internal usage
            private int _NodeDOFs; // holds value only for internal usage

            private int _ID = -1;
            private int _Material; // holds material ID
            private Color _DefaultColor;
            protected Color[] _Color = null; // want to be able to set a color for each endpoint of the element

            private bool _ReadyToSolve = false; // is true if the nodes of the element are set up properly
            protected bool _SolutionValid = false; // is true if the solution for the element is correct

            public event SolutionInvalidatedEventHandler SolutionInvalidated;

            public delegate void SolutionInvalidatedEventHandler(int ElemID);

            public abstract Type MyType { get; }

            protected int NumOfNodes
            {
                get
                {
                    return _NumOfNodes;
                }
            }
            protected int NodeDOFs
            {
                get
                {
                    return _NodeDOFs;
                }
            }
            protected int ElemDOFs
            {
                get
                {
                    return _NodeDOFs * _NumOfNodes;
                }
            }


            public int ID
            {
                get
                {
                    return _ID;
                }
            }

            public Color SelectColor
            {
                get
                {
                    return _SelectedColor;
                }
            }
            public Color[] ElemColor
            {
                get
                {
                    return _Color;
                }
            }
            public Color get_CornerColor(int LocalNodeID)
            {
                return _Color[LocalNodeID];
            }
            public bool AllCornersSameColor
            {
                get
                {
                    var tmp = _Color[0];

                    foreach (Color c in _Color)
                    {
                        if (c != tmp)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            } // checks if all corners are the same color
            public bool get_AllCornersEqualColor(Color C_in)
            {
                foreach (Color c in _Color)
                {
                    if (c != C_in)
                    {
                        return false;
                    }
                }
                return true;
            } // true if all colors are input color

            public bool Selected
            {
                get
                {
                    if (AllCornersEqualColor(_SelectedColor))
                    {
                        return true;
                    }
                    return false;
                }
                set
                {
                    if (value)
                    {
                        SetColor(_SelectedColor);
                    }
                    else if (AllCornersEqualColor(_SelectedColor)) // check if the object is actually selected
                    {
                        SetColor(_DefaultColor);
                    }
                }
            } // changes the color if selected
            public int Material
            {
                get
                {
                    return _Material;
                }
                set
                {
                    _Material = value;
                    InvalidateSolution();
                }
            } // flags the solution invalid if set

            public Element(Color inColor, int ID, int Mat = -1)
            {
                _NumOfNodes = get_NumOfNodes(MyType);
                _NodeDOFs = get_NodeDOFs(MyType);

                _DefaultColor = inColor;

                _Color = new Color[_NumOfNodes]; // need to have a color for each node in the element
                for (int i = 0, loopTo = _Color.Length - 1; i <= loopTo; i++) // initially set all corners to the default color
                    _Color[i] = inColor;

                _Material = Mat;
                _ID = ID;

            }

            public void SetColor(Color C)
            {
                foreach (Color Val in _Color)
                    Val = C;
            } // sets all endpoints to the specified color
            public void SetCornerColor(Color C, int LocalNodeID)
            {
                _Color[LocalNodeID] = C;
            }


            protected void InvalidateSolution()
            {
                _SolutionValid = false;
                SolutionInvalidated?.Invoke(_ID);
            }

        } // base element subclass - common between all types of elements



        public partial interface IElement : IBaseElement
        {

            Type MyType { get; }
            object get_Interpolated_Displacement(double[] IntrinsicCoords, DenseMatrix GblNodeQ); // can interpolate either position or displacement
            DenseMatrix get_B_mtrx(List<double[]> GblNodeCoords); // needs to be given with local node 1 in first spot on list
            double get_Length(List<double[]> GblNodeCoords);
            DenseMatrix get_Stress_mtrx(List<double[]> GblNodeCoords, DenseMatrix GblNodeQ, double E, double[] IntrinsicCoords = null);

            DenseMatrix get_K_mtrx(List<double[]> GblNodeCoords, double E); // node 1 displacement comes first in disp input, followed by second
            DenseMatrix get_BodyForce_mtrx(List<double[]> GblNodeCoords); // node 1 displacement comes first in disp input, followed by second
            DenseMatrix get_TractionForce_mtrx(List<double[]> GblNodeCoords);


            void SortNodeOrder(ref List<int> NodeIDs, List<double[]> NodeCoords);
            void SetBodyForce(DenseMatrix ForcePerVol);
            void SetTractionForce(DenseMatrix ForcePerLength);
            void Draw(List<double[]> GblNodeCoords);

        }
        public partial interface IBaseElement
        {

            // ------------------ From Element Baseclass -----------------
            // ReadOnly Property NumOfNodes As Integer
            // ReadOnly Property NodeDOFs As Integer
            // ReadOnly Property ElemDOFs As Integer


            int ID { get; }
            Color SelectColor { get; }
            Color[] ElemColor { get; }
            Color get_CornerColor(int LocalNodeID);
            bool AllCornersSameColor { get; }
            bool get_AllCornersEqualColor(Color C_in);

            bool Selected { get; set; }
            int Material { get; set; }
            void SetColor(Color C);
            void SetCornerColor(Color C, int LocalNodeID);

        } // interface for base element subclass

    }


    public partial class CoordinateSystem
    {

        private double originX = 0d;
        private double originY = 0d;
        private double originZ = 0d;
        private int _scale = 1;
        private Color SysColor = Color.Teal;

        public CoordinateSystem(double Scale)
        {
            _scale = (int)Math.Round(Scale);
        }

        public void Draw(bool ThreeDimensional)
        {

            Matrix4 xform = Matrix4.CreateTranslation(new Vector3(originX / _scale, originY / _scale, originZ / _scale));
            xform = Matrix4.Mult(xform, Matrix4.CreateScale(_scale, _scale, _scale));
            GL.MultMatrix(xform);


            // X Axis
            GL.Color3(SysColor);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(0.9d, 0.1d, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(0.9d, -0.1d, 0);

            // letter x
            GL.Vertex3(0.9d, 0.25d, 0);
            GL.Vertex3(1, 0.35d, 0);
            GL.Vertex3(1, 0.15d, 0);
            GL.Vertex3(0.8d, 0.35d, 0);
            GL.Vertex3(0.9d, 0.25d, 0);
            GL.Vertex3(0.8d, 0.15d, 0);

            // Y Axis

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(0.1d, 0.9d, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(-0.1d, 0.9d, 0);

            // letter y
            GL.Vertex3(0.15d, 1, 0);
            GL.Vertex3(0.25d, 0.9d, 0);
            GL.Vertex3(0.25d, 0.9d, 0);
            GL.Vertex3(0.35d, 1, 0);
            GL.Vertex3(0.25d, 0.9d, 0);
            GL.Vertex3(0.25d, 0.8d, 0);

            if (ThreeDimensional)
            {
                // Z Axis
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, 1);
                GL.Vertex3(0, 0, 1);
                GL.Vertex3(0.1d, 0, 0.9d);
                GL.Vertex3(0, 0, 1);
                GL.Vertex3(-0.1d, 0, 0.9d);

                // letter Z
                GL.Vertex3(0.15d, 0, 1);
                GL.Vertex3(0.35d, 0, 1);
                GL.Vertex3(0.15d, 0, 1);
                GL.Vertex3(0.35d, 0, 0.8d);
                GL.Vertex3(0.35d, 0, 0.8d);
                GL.Vertex3(0.15d, 0, 0.8d);
            }

            GL.End();

            xform = Matrix4.Identity;
            xform = Matrix4.Mult(xform, Matrix4.CreateScale(1d / _scale, 1d / _scale, 1d / _scale));
            xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(new Vector3(-originX / _scale, -originY / _scale, -originZ / _scale)));
            GL.MultMatrix(xform);


        }


    }

    public partial class Units
    {

        public partial class Length
        {
            private static double get_ConversionFactor(int LengthUnit)
            {
                switch (LengthUnit)
                {

                    case (int)LengthUnits.m:
                        {
                            return 1.0d;
                        }
                    case (int)LengthUnits.mm:
                        {
                            return 0.001d;
                        }
                    case (int)LengthUnits.cm:
                        {
                            return 0.01d;
                        }
                    case (int)LengthUnits.inch:
                        {
                            return 0.0254d;
                        }
                    case (int)LengthUnits.ft:
                        {
                            return 0.3048d;
                        }

                    default:
                        {
                            return (double)default;
                        }
                }
            }

            public static int DefaultUnit
            {
                get
                {
                    return (int)LengthUnits.m;
                }
            }
            public static string[] get_UnitStrings(int LengthUnit)
            {
                switch (LengthUnit)
                {

                    case (int)LengthUnits.m:
                        {
                            return new[] { "m" };
                        }
                    case (int)LengthUnits.mm:
                        {
                            return new[] { "mm" };
                        }
                    case (int)LengthUnits.cm:
                        {
                            return new[] { "cm" };
                        }
                    case (int)LengthUnits.inch:
                        {
                            return new[] { "in", "inch", "\"" };
                        }
                    case (int)LengthUnits.ft:
                        {
                            return new[] { "ft", "feet", "'" };
                        }

                    default:
                        {
                            return null;
                        }
                }
            }
            public static int get_UnitEnums(string UnitString)
            {
                for (int I = 0, loopTo = Enum.GetNames(typeof(LengthUnits)).Count() - 1; I <= loopTo; I++)
                {
                    if (get_UnitStrings(I).Contains(UnitString))
                    {
                        return I;
                    }
                }
                return -1; // if could not find
            }

            public static double Convert(int InputUnit, double Data, int OutputUnit)
            {
                return Data * get_ConversionFactor(InputUnit) / get_ConversionFactor(OutputUnit);
            }

            public enum LengthUnits
            {
                m = 0,
                mm = 1,
                cm = 2,
                inch = 3,
                ft = 4
            }

        }

        public static Dictionary<string, double> get_ConversionFactors(DataUnitType UnitType)
        {
            var output = new Dictionary<string, double>();
            switch (UnitType)
            {
                case DataUnitType.Length:
                    {
                        output.Add("m", 1d);
                        output.Add("mm", 0.001d);
                        output.Add("cm", 0.01d);
                        output.Add("in", 0.0254d);
                        output.Add("ft", 0.3048d);
                        return output;
                    }

                case DataUnitType.Area:
                    {
                        output.Add("m^2", 1d);
                        output.Add("mm^2", 0.001d * 0.001d);
                        output.Add("cm^2", 0.01d * 0.01d);
                        output.Add("in^2", 0.0254d * 0.0254d);
                        output.Add("ft^2", 0.3048d * 0.3048d);
                        return output;
                    }

                case DataUnitType.Force:
                    {
                        output.Add("N", 1d);
                        output.Add("lb", 4.44822d);
                        output.Add("lbs", 4.44822d);
                        return output;
                    }

                case DataUnitType.Pressure:
                    {
                        output.Add("Pa", 1d);
                        output.Add("pa", 1d);
                        output.Add("kpa", 1000d);
                        output.Add("Kpa", 1000d);
                        output.Add("Mpa", 1000 * 1000);
                        output.Add("mpa", 1000 * 1000);
                        output.Add("bar", 100000d);
                        output.Add("psi", 6894.76d);
                        return output;
                    }
            }

            return null;
        }

        // ----------------- UPDATE THESE IF NEW UNITS ARE ADDED -------------------
        private static double get_ConversionFactor(int Unit)
        {
            switch (Unit)
            {
                // ---------------- Length ------------------
                case (int)AllUnits.m:
                    {
                        return 1.0d;
                    }
                case (int)AllUnits.mm:
                    {
                        return 0.001d;
                    }
                case (int)AllUnits.cm:
                    {
                        return 0.01d;
                    }
                case (int)AllUnits.inch:
                    {
                        return 0.0254d;
                    }
                case (int)AllUnits.ft:
                    {
                        return 0.3048d;
                    }

                // ---------------------------Area----------------------
                case (int)AllUnits.m_squared:
                    {
                        return get_ConversionFactor((int)AllUnits.m) * get_ConversionFactor((int)AllUnits.m);
                    }
                case (int)AllUnits.mm_squared:
                    {
                        return get_ConversionFactor((int)AllUnits.mm) * get_ConversionFactor((int)AllUnits.mm);
                    }
                case (int)AllUnits.cm_squared:
                    {
                        return get_ConversionFactor((int)AllUnits.cm) * get_ConversionFactor((int)AllUnits.cm);
                    }
                case (int)AllUnits.in_squared:
                    {
                        return get_ConversionFactor((int)AllUnits.inch) * get_ConversionFactor((int)AllUnits.inch);
                    }
                case (int)AllUnits.ft_squared:
                    {
                        return get_ConversionFactor((int)AllUnits.ft) * get_ConversionFactor((int)AllUnits.ft);
                    }

                // -------------------- Force ---------------------------

                case (int)AllUnits.N:
                    {
                        return 1d;
                    }
                case (int)AllUnits.lb:
                    {
                        return 4.44822d;
                    }

                // ----------------- Pressure ----------------------
                case (int)AllUnits.Pa:
                    {
                        return 1d;
                    }
                case (int)AllUnits.KPa:
                    {
                        return 1000d;
                    }
                case (int)AllUnits.MPa:
                    {
                        return 1000 * 1000;
                    }
                case (int)AllUnits.GPa:
                    {
                        return 1000 * 1000 * 1000;
                    }
                case (int)AllUnits.Psi:
                    {
                        return 6894.76d;
                    }
                case (int)AllUnits.Bar:
                    {
                        return 100000d;
                    }

                default:
                    {
                        return (double)default;
                    }
            }
        }
        public static string[] get_UnitStrings(AllUnits Unit)
        {
            switch (Unit)
            {
                // -------------------- Length -------------------
                case AllUnits.m:
                    {
                        return new[] { "m" };
                    }
                case AllUnits.mm:
                    {
                        return new[] { "mm" };
                    }
                case AllUnits.cm:
                    {
                        return new[] { "cm" };
                    }
                case AllUnits.inch:
                    {
                        return new[] { "in", "inch", "\"" };
                    }
                case AllUnits.ft:
                    {
                        return new[] { "ft", "feet", "'" };
                    }

                // ---------------------------Area----------------------

                case AllUnits.m_squared:
                    {
                        return new[] { "m^2" };
                    }
                case AllUnits.mm_squared:
                    {
                        return new[] { "mm^2" };
                    }
                case AllUnits.cm_squared:
                    {
                        return new[] { "cm^2" };
                    }
                case AllUnits.in_squared:
                    {
                        return new[] { "in^2", "sqin" };
                    }
                case AllUnits.ft_squared:
                    {
                        return new[] { "ft^2", "sqft" };
                    }

                // -------------------- Force ---------------------------

                case AllUnits.N:
                    {
                        return new[] { "N" };
                    }
                case AllUnits.lb:
                    {
                        return new[] { "lb", "lbs" };
                    }

                // ----------------- Pressure ----------------------
                case AllUnits.Pa:
                    {
                        return new[] { "Pa", "pa" };
                    }
                case AllUnits.KPa:
                    {
                        return new[] { "KPa", "kpa", "Kpa" };
                    }
                case AllUnits.MPa:
                    {
                        return new[] { "MPa", "mpa", "Mpa" };
                    }
                case AllUnits.GPa:
                    {
                        return new[] { "GPa", "gpa", "Gpa" };
                    }
                case AllUnits.Psi:
                    {
                        return new[] { "psi", "Psi" };
                    }
                case AllUnits.Bar:
                    {
                        return new[] { "bar", "Bar" };
                    }

                default:
                    {
                        return null;
                    }
            }
        }
        public static AllUnits get_DefaultUnit(DataUnitType UnitType)
        {
            switch (UnitType)
            {
                case DataUnitType.Length:
                    {
                        return AllUnits.m;
                    }

                case DataUnitType.Area:
                    {
                        return AllUnits.m_squared;
                    }

                case DataUnitType.Force:
                    {
                        return AllUnits.N;
                    }

                case DataUnitType.Pressure:
                    {
                        return AllUnits.Pa;
                    }

                default:
                    {
                        return default;
                    }
            }
        }
        public static int[] get_UnitTypeRange(DataUnitType UnitType)
        {
            switch (UnitType)
            {
                case DataUnitType.Length:
                    {
                        return new[] { 0, 4 };
                    }

                case DataUnitType.Area:
                    {
                        return new[] { 5, 9 };
                    }

                case DataUnitType.Force:
                    {
                        return new[] { 10, 11 };
                    }

                case DataUnitType.Pressure:
                    {
                        return new[] { 12, 17 };
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        // ----------------------------------------------------------------------------
        public static List<string> get_TypeUnitStrings(DataUnitType UnitType)
        {
            int[] range = get_UnitTypeRange(UnitType);

            var output = new List<string>();

            for (int i = range[0], loopTo = range[1]; i <= loopTo; i++) // search through the enum range for that type
            {
                foreach (string S in get_UnitStrings((AllUnits)i)) // get all the strings for each enum and add to output
                    output.Add(S);
            }

            return output;
        }
        public static AllUnits get_UnitEnums(string UnitString)
        {
            for (int I = 0, loopTo = Enum.GetNames(typeof(AllUnits)).Count() - 1; I <= loopTo; I++)
            {
                if (get_UnitStrings((AllUnits)I).Contains(UnitString))
                {
                    return (AllUnits)I;
                }
            }
            return (AllUnits)(-1); // if could not find
        }
        public static double Convert(AllUnits InputUnit, double Data, AllUnits OutputUnit)
        {
            return Data * get_ConversionFactor((int)InputUnit) / get_ConversionFactor((int)OutputUnit);
        }



        public enum DataUnitType
        {
            Length = 0, // m
            Area = 1, // m^2
            Force = 2, // N
            Pressure = 3 // Pa
        }
        public enum AllUnits
        {
            // --------------- Length -------------------
            mm,
            cm,
            m,
            inch,
            ft,
            // ------------- Area -----------------------
            mm_squared,
            cm_squared,
            m_squared,
            in_squared,
            ft_squared,
            // ------------- Force -----------------------
            N,
            lb,
            // ------------- Pressure ---------------
            KPa,
            MPa,
            GPa,
            Pa,
            Psi,
            Bar

        }
    }
}
