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

        public Element(int ID, int Mat = -1)
        {
            _NumOfNodes = ElementMgr.NumOfNodes(MyType);
            _NodeDOFs = ElementMgr.NodeDOFs(MyType);

            _Material = Mat;
            _ID = ID;
        }

        protected void InvalidateSolution()
        {
            _SolutionValid = false;
            SolutionInvalidated?.Invoke(_ID);
        }
    }
}
