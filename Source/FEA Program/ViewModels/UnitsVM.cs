using FEA_Program.ViewModels.Base;
using static FEA_Program.Models.Units;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel representing a single unit
    /// </summary>
    internal class UnitsVM: ObservableObject
    {
        public UnitVM Length { get; private set; } = new(DataUnitType.Length, AllUnits.mm);
        public UnitVM Area { get; private set; } = new(DataUnitType.Area, AllUnits.mm_squared);
        public UnitVM Stress { get; private set; } = new(DataUnitType.Pressure, AllUnits.MPa);

    }
}
