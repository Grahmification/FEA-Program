using FEA_Program.ViewModels.Base;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for an X, Y, or Z coordinate
    /// </summary>
    internal class CoordinateVM : ObservableObject
    {
        private double _value = 0;
        private bool _fixed = false;
        private readonly int _index = -1;

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when the value has changed, with the index as an argument
        /// </summary>
        public event EventHandler<int>? ValueChanged;

        // ---------------------- Properties ----------------------
        
        /// <summary>
        /// The display label
        /// </summary>
        public string Label => NodeVM.CoordinateNames[_index];

        /// <summary>
        /// The coordinate value
        /// </summary>
        public double Value { get => _value; set { _value = value; ValueChanged?.Invoke(this, _index); } }

        /// <summary>
        /// Whether the coordinate is fixed
        /// </summary>
        public bool Fixed { get => _fixed; set { _fixed = value; ValueChanged?.Invoke(this, _index); } }

        public CoordinateVM() { }

        /// <summary>
        /// Construct the VM
        /// </summary>
        /// <param name="index">Index of the coordinate</param>
        /// <param name="initialValue">The starting value</param>
        /// <param name="isFixed">True if the coordinate is fixed</param>
        public CoordinateVM(int index, double initialValue, bool isFixed)
        {
            _index = index;
            _value = initialValue;
            _fixed = isFixed;
        }
    }
}
