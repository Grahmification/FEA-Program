using System.Runtime.CompilerServices;

namespace FEA_Program.Models
{
    internal class StressProblem
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
            Loadedform.GlCont.SubControl.Invalidate();
        }
        private void ScreenRedrawOnlyNeeded()
        {
            Loadedform.GlCont.SubControl.Invalidate();
        }

        private void HangingElements(int NodeID, int Dimension)
        {

            var ElementsToDelete = Connect.NodeElements(NodeID);
            Elements.Delete(ElementsToDelete);

        } // deletes elements if a node is deleted and leaves one hanging
        private void ElementCreation(int ElemID, List<int> NodeIDs, Type Type)
        {

            // ---------------------- Get Coords of all of the nodes in the element and then sort Ids

            var NodeCoords = new List<double[]>();

            foreach (int ID in NodeIDs)
                NodeCoords.Add(Nodes.NodeObj(ID).Coords);

            Elements.ElemObj(ElemID.ToString()).SortNodeOrder(ref NodeIDs, NodeCoords); // nodeIDs is passed byref

            Connect.AddConnection(ElemID, NodeIDs);
        }
        private void ElementDeletion(int ElemID, IElement Type)
        {
            Connect.RemoveConnection(ElemID);
        }
    }

    public enum ProblemTypes
    {
        Bar_1D,
        Beam_1D,
        Truss_3D
    }
}
