using FEA_Program.ViewModels.Base;
using static FEA_Program.Models.Units;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel representing a single unit
    /// </summary>
    internal class UnitVM: ObservableObject
    {
        private AllUnits _Unit;

        public event EventHandler? UnitChanged;
        
        public DataUnitType UnitType { get; private set; }
        public AllUnits Unit { get => _Unit; set { _Unit = value; UnitChanged?.Invoke(this, EventArgs.Empty); } }
        public string UnitString => UnitStrings(Unit)[0];

 
        public UnitVM(DataUnitType? unitType = null, AllUnits? unit = null) 
        {
            UnitType = unitType ?? DataUnitType.Unitless;
            _Unit = unit ?? DefaultUnit(UnitType);
        }

        /// <summary>
        /// Convert from program units to user units
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public double ToUser(double value)
        {
            return Convert(DefaultUnit(UnitType), value, Unit);
        }

        /// <summary>
        /// Convert from user units to program units
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public double FromUser(double value)
        {
            return Convert(Unit, value, DefaultUnit(UnitType));
        }

    }
}
