using FEA_Program.ViewModels.Base;
using FEA_Program.Models;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel representing a single unit
    /// </summary>
    internal class UnitsVM: ObservableObject
    {
        public UnitVM Length { get; private set; } = new(UnitType.Length, Unit.mm);
        public UnitVM Area { get; private set; } = new(UnitType.Area, Unit.mm_squared);
        public UnitVM Stress { get; private set; } = new(UnitType.Pressure, Unit.MPa);

        /// <summary>
        /// Pressure used for Young's modulus calculations
        /// </summary>
        public UnitVM Modulus { get; private set; } = new(UnitType.Pressure, Unit.GPa);

        public UnitVM Force { get; private set; } = new(UnitType.Force, Unit.N);

    }
}
