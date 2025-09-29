using System.Runtime.CompilerServices;

namespace FEA_Program.Models
{
    internal class StressProblem
    {
        private NodeManager _Nodes;

        public virtual NodeManager Nodes
        {
            get { return _Nodes; }
            set
            {
                if (_Nodes != null)
                {
                    _Nodes.NodeListChanged -= (s, e) => OnListRedrawNeeded();
                    _Nodes.NodesChanged -= (s, e) => OnListRedrawNeeded();
                    _Nodes.NodeChanged_RedrawOnly -= OnScreenRedrawOnlyNeeded;
                    _Nodes.NodeDeleted -= OnNodeDeletion;
                }

                _Nodes = value;
                if (_Nodes != null)
                {
                    _Nodes.NodeListChanged += (s, e) => OnListRedrawNeeded();
                    _Nodes.NodesChanged += (s, e) => OnListRedrawNeeded();
                    _Nodes.NodeChanged_RedrawOnly += OnScreenRedrawOnlyNeeded;
                    _Nodes.NodeDeleted += OnNodeDeletion;
                }
            }
        }
        private ElementManager _Elements;

        public virtual ElementManager Elements
        {
            get { return _Elements; }
            set
            {
                if (_Elements != null)
                {
                    _Elements.ElementListChanged -= (s, e) => OnListRedrawNeeded();
                    _Elements.ElementChanged -= (s, e) => OnListRedrawNeeded();
                    _Elements.ElementChanged_RedrawOnly -= OnScreenRedrawOnlyNeeded;
                    _Elements.ElementAdded -= OnElementCreation;
                    _Elements.ElementDeleted -= OnElementDeletion;
                }

                _Elements = value;
                if (_Elements != null)
                {
                    _Elements.ElementListChanged += (s, e) => OnListRedrawNeeded();
                    _Elements.ElementChanged += (s, e) => OnListRedrawNeeded();
                    _Elements.ElementChanged_RedrawOnly += OnScreenRedrawOnlyNeeded;
                    _Elements.ElementAdded += OnElementCreation;
                    _Elements.ElementDeleted += OnElementDeletion;
                }
            }
        }
        private MaterialManager _Materials;

        public virtual MaterialManager Materials
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
                    _Materials.MaterialListChanged -= (s, e) => OnListRedrawNeeded();
                }

                _Materials = value;
                if (_Materials != null)
                {
                    _Materials.MaterialListChanged += (s, e) => OnListRedrawNeeded();
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
                            return new[] { typeof(ElementBarLinear) };
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
            Nodes = new NodeManager();
            Elements = new ElementManager();
            Materials = new MaterialManager();
            Connect = new Connectivity();
            Loadedform = form;
            _Type = Type;
        }

        private void OnListRedrawNeeded()
        {
            Loadedform.ReDrawLists();
            Loadedform.GlCont.SubControl.Invalidate();
        }
        private void OnScreenRedrawOnlyNeeded(object? sender, EventArgs e)
        {
            Loadedform.GlCont.SubControl.Invalidate();
        }


        private void OnElementCreation(int ElemID, List<int> NodeIDs)
        {

            // Sort the node IDs accordingly for the given element type
            var nodes = new List<INode>();

            foreach (int ID in NodeIDs)
                nodes.Add(Nodes.GetNode(ID));

            Elements.GetElement(ElemID).SortNodeOrder(ref nodes);
            var sortedIDs = nodes.Select(node => node.ID).ToList();

            Connect.AddConnection(ElemID, sortedIDs);
        }
        private void OnElementDeletion(object? sender, IElement e)
        {
            Connect.RemoveConnection(e.ID);
        }
        private void OnNodeDeletion(object? sender, INode e)
        {
            RemoveHangingElements(e.ID);
        }

        /// <summary>
        /// Deletes elements if a node is deleted and leaves one hanging
        /// </summary>
        /// <param name="nodeID"></param>
        private void RemoveHangingElements(int nodeID)
        {
            var ElementsToDelete = Connect.NodeElements(nodeID);
            Elements.Delete(ElementsToDelete);
        }


    }

    public enum ProblemTypes
    {
        Bar_1D,
        Beam_1D,
        Truss_3D
    }
}
