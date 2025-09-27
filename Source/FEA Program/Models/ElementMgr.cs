using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal class ElementMgr
    {
        private Dictionary<int, IElement> _Bar1Elements = new Dictionary<int, IElement>(); // reference elements by ID

        public event ElementListChangedEventHandler? ElementListChanged;

        public delegate void ElementListChangedEventHandler(Dictionary<int, IElement> ElemList); // Length of Elementlist has changed
        public event ElementChangedEventHandler? ElementChanged;

        public delegate void ElementChangedEventHandler(int ID); // Element has changed such that list needs to be updated & screen redrawn
        public event ElementChanged_RedrawOnlyEventHandler? ElementChanged_RedrawOnly;

        public delegate void ElementChanged_RedrawOnlyEventHandler(); // Element has changed such that screen only needs to be redrawn
        public event ElementAddedEventHandler? ElementAdded;

        public delegate void ElementAddedEventHandler(int ElemID, List<int> NodeIDs, Type Type); // dont use for redrawing lists or screen
        public event ElementDeletedEventHandler? ElementDeleted;

        public delegate void ElementDeletedEventHandler(int ElemID, IElement Type); // dont use for redrawing lists or screen

        public static int NumOfNodes(Type ElemType)
        {
            switch (ElemType)
            {
                case var @case when @case == typeof(ElementBarLinear):
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
        public static int NodeDOFs(Type ElemType)
        {
            switch (ElemType)
            {
                case var @case when @case == typeof(ElementBarLinear):
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
        public static string Name(Type ElemType)
        {
            switch (ElemType)
            {
                case var @case when @case == typeof(ElementBarLinear):
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
        public static int ElemDOFs(Type ElemType)
        {
            return NumOfNodes(ElemType) * NodeDOFs(ElemType);
        }
        public static Dictionary<string, Units.DataUnitType> ElementArgs(Type ElemType)
        {
            var output = new Dictionary<string, Units.DataUnitType>();

            switch (ElemType)
            {

                case var @case when @case == typeof(ElementBarLinear):
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


        public List<IElement> Elemlist => _Bar1Elements.Values.ToList();
        public List<int> AllIDs
        {
            get
            {
                var output = _Bar1Elements.Keys.ToList();
                output.Sort();
                return output;
            }
        }
        public IElement ElemObj(string ElemID)
        {
            return _Bar1Elements[int.Parse(ElemID)];
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

                output.Add(E_ID, _Bar1Elements[E_ID].K_mtrx(ElemNodeCoords, E[MatID]));
            }

            return output;
        }


        public void Add(Type Type, List<int> NodeIDs, double[] ElementArgs, int Mat = -1)
        {
            IElement newElem = null;
            int newElemID = CreateId();

            // ------------------ Determine type of element ----------------------

            if (ReferenceEquals(Type, typeof(ElementBarLinear))) // linear bar element --------------------------------
            {

                if (ElementArgs[0] > 0d)
                {
                    newElem = new ElementBarLinear(ElementArgs[0], newElemID, Mat);
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

            if (NodeIDs.Count != NumOfNodes(newElem.GetType())) // check if the right number of nodes are listed
            {
                throw new Exception("Tried to add element of type <" + Type.ToString() + "> with " + NodeIDs.Count.ToString() + " nodes. Should have " + NumOfNodes(newElem.GetType()) + " nodes.");
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
    }
}
