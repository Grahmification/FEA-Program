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
        private bool _rotationFixed = false;
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

        /// <summary>
        /// Whether the coordinate rotation is fixed
        /// </summary>
        public bool RotationFixed { get => _rotationFixed; set { _rotationFixed = value; ValueChanged?.Invoke(this, _index); } }

        /// <summary>
        /// Whether to display the control to fix/unfix the rotation
        /// </summary>
        public bool DisplayRotation { get; private set; }

        public CoordinateVM() { }

        /// <summary>
        /// Construct the VM
        /// </summary>
        /// <param name="index">Index of the coordinate</param>
        /// <param name="initialValue">The starting value</param>
        /// <param name="isFixed">True if the coordinate is fixed</param>
        /// <param name="hasRotation">Whether the coordinate has a rotation assocated with it</param>
        /// <param name="rotationFixed">Whether the rotation is fixed</param>
        public CoordinateVM(int index, double initialValue, bool isFixed, bool hasRotation = false, bool rotationFixed = false)
        {
            _index = index;
            _value = initialValue;
            _fixed = isFixed;
            RotationFixed = rotationFixed;
            DisplayRotation = hasRotation;
        }
    }
}
