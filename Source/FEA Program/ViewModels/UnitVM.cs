using FEA_Program.ViewModels.Base;
using FEA_Program.Models;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel representing a single unit
    /// </summary>
    internal class UnitVM: ObservableObject
    {
        private Unit _Unit;

        public event EventHandler? UnitChanged;
        
        public UnitType UnitType { get; private set; }
        public Unit Unit { get => _Unit; set { _Unit = value; UnitChanged?.Invoke(this, EventArgs.Empty); } }
        public string UnitString => Units.UnitStrings(Unit)[0];

 
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
