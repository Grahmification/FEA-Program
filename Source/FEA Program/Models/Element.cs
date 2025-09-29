namespace FEA_Program.Models
{
    /// <summary>
    /// Base element subclass - common between all types of elements
    /// </summary>
    internal abstract class Element
    {
        private int _Material; // holds material ID
        private bool _ReadyToSolve = false; // is true if the nodes of the element are set up properly
        
        public event EventHandler<int>? SolutionInvalidated;

        //public abstract Type MyType { get; }

        public int ID { get; private set; }
        public int Material
        {
            get { return _Material; }
            set { _Material = value; InvalidateSolution(); }
        } // flags the solution invalid if set
        public bool SolutionValid { get; protected set; } = false; // is true if the solution for the element is correct
        public abstract string Name { get; }
        public abstract int NumOfNodes { get; }
        public abstract int NodeDOFs { get; }
        public int ElementDOFs => NumOfNodes * NodeDOFs;

        public Element(int id, int material = -1)
        {
            _Material = material;
            ID = id;
        }

        protected void InvalidateSolution()
        {
            SolutionValid = false;
            SolutionInvalidated?.Invoke(this, ID);
        }

        protected void ValidateLength<T>(IReadOnlyCollection<T> coordinates, int targetLength, string? methodName)
        {
            methodName ??= "Unknown";

            if (coordinates.Count != targetLength)
            {
                throw new ArgumentOutOfRangeException($"Wrong number of array values input to <{methodName}> for Element ID <{ID}>. Element has {NodeDOFs} node DOFs.");
            }
        }
    }
}
