namespace FEA_Program.Models
{
    /// <summary>
    /// Base element subclass - common between all types of elements
    /// </summary>
    internal abstract class Element : IBaseElement
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
                return Constants.SelectedColor;
            }
        }
        public Color[] ElemColor
        {
            get
            {
                return _Color;
            }
        }
        public Color CornerColor(int LocalNodeID)
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
        public bool AllCornersEqualColor(Color C_in)
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
                if (AllCornersEqualColor(Constants.SelectedColor))
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    SetColor(Constants.SelectedColor);
                }
                else if (AllCornersEqualColor(Constants.SelectedColor)) // check if the object is actually selected
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
            _NumOfNodes = ElementMgr.NumOfNodes(MyType);
            _NodeDOFs = ElementMgr.NodeDOFs(MyType);

            _DefaultColor = inColor;

            _Color = new Color[_NumOfNodes]; // need to have a color for each node in the element
            for (int i = 0, loopTo = _Color.Length - 1; i <= loopTo; i++) // initially set all corners to the default color
                _Color[i] = inColor;

            _Material = Mat;
            _ID = ID;

        }

        public void SetColor(Color C)
        {
            for(int i = 0; i< _Color.Length; i++)
            {
                _Color[i] = C;
            }
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

    }
}
