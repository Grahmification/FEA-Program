using FEA_Program.Models;
using FEA_Program.ViewModels.Base;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for parameters which can vary between different elements
    /// </summary>
    internal class ElementArgVM : ObservableObject
    {
        private double _value = 0;

        // ---------------------- Events ----------------------

        public event EventHandler? ValueChanged;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The list index for the linked value in <see cref="IElement.ElementArgs"/>
        /// </summary>
        public int Index { get; private set; } = -1;
        public string Name { get; private set; } = "";
        public double Value { get => _value; set { _value = value; ValueChanged?.Invoke(this, EventArgs.Empty); } }
        public Units.DataUnitType UnitType { get; private set; } = Units.DataUnitType.Unitless;
        public string UnitName => Units.UnitStrings(Units.DefaultUnit(UnitType))[0];


        public ElementArgVM() { }

        public ElementArgVM(int index, string name, Units.DataUnitType unitType, double initialValue = 0)
        {
            Index = index;
            _value = initialValue;
            Name = name;
            UnitType = unitType;
        }
    }
}
