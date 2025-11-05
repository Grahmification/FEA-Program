using FEA_Program.ViewModels.Base;
using FEA_Program.Models;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel representing a single unit
    /// </summary>
    internal class UnitVM: ObservableObject
    {
        /// <summary>
        /// The unit being displayed
        /// </summary>
        private Unit _Unit;

        /// <summary>
        /// Fires when the Unit's value has changed
        /// </summary>
        public event EventHandler? UnitChanged;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The high level type of unit
        /// </summary>
        public UnitType UnitType { get; private set; }

        /// <summary>
        /// The actual unit
        /// </summary>
        public Unit Unit { get => _Unit; set { _Unit = value; UnitChanged?.Invoke(this, EventArgs.Empty); } }

        /// <summary>
        /// String prefix for the unit
        /// </summary>
        public string UnitString => Units.UnitStrings(Unit)[0];

        // ---------------------- Methods ----------------------

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="unitType">The high level type of unit</param>
        /// <param name="unit">The actual unit</param>
        public UnitVM(UnitType? unitType = null, Unit? unit = null) 
        {
            UnitType = unitType ?? UnitType.Unitless;
            _Unit = unit ?? Units.DefaultUnit(UnitType);
        }

        /// <summary>
        /// Convert from program units to user units
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public double ToUser(double value)
        {
            return Units.Convert(Units.DefaultUnit(UnitType), value, Unit);
        }

        /// <summary>
        /// Convert from user units to program units
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public double FromUser(double value)
        {
            return Units.Convert(Unit, value, Units.DefaultUnit(UnitType));
        }

    }
}
