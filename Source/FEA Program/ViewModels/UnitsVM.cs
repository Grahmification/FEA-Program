using FEA_Program.ViewModels.Base;
using FEA_Program.Models;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel representing all units in the program
    /// </summary>
    internal class UnitsVM: ObservableObject
    {
        /// <summary>
        /// Length/distance units
        /// </summary>
        public UnitVM Length { get; private set; } = new(UnitType.Length, Unit.mm);

        /// <summary>
        /// Area units
        /// </summary>
        public UnitVM Area { get; private set; } = new(UnitType.Area, Unit.mm_squared);

        /// <summary>
        /// Force units
        /// </summary>
        public UnitVM Force { get; private set; } = new(UnitType.Force, Unit.N);

        /// <summary>
        /// Stress units
        /// </summary>
        public UnitVM Stress { get; private set; } = new(UnitType.Pressure, Unit.MPa);

        /// <summary>
        /// Pressure used for Young's modulus calculations
        /// </summary>
        public UnitVM Modulus { get; private set; } = new(UnitType.Pressure, Unit.GPa);

    }
}
