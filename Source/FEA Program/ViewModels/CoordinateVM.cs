using FEA_Program.ViewModels.Base;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for an X, Y, or Z coordinate
    /// </summary>
    internal class CoordinateVM : ObservableObject
    {
        public static string[] Labels = ["X", "Y", "Z"];

        private double _value = 0;
        private bool _fixed = false;
        private int _index = -1;

        // ---------------------- Events ----------------------

        public event EventHandler<int>? ValueChanged;

        // ---------------------- Properties ----------------------
        public string Label => Labels[_index];
        public double Value { get => _value; set { _value = value; ValueChanged?.Invoke(this, _index); } }
        public bool Fixed { get => _fixed; set { _fixed = value; ValueChanged?.Invoke(this, _index); } }

        public CoordinateVM() { }

        public CoordinateVM(int index, double initialValue, bool isFixed)
        {
            _index = index;
            _value = initialValue;
            _fixed = isFixed;
        }
    }
}
