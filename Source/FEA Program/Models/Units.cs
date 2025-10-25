namespace FEA_Program.Models
{
    internal class Units
    {
        // ----------------- UPDATE THESE IF NEW UNITS ARE ADDED -------------------
        private static double ConversionFactor(Unit unit) => unit switch
        {
            // ---------------- Unitless ------------------
            Unit.Unitless => 1.0,
            // ---------------- Length ------------------
            Unit.m => 1.0,
            Unit.mm => 0.001,
            Unit.cm => 0.01,
            Unit.inch => 0.0254,
            Unit.ft => 0.3048d,
            // ---------------- Area ------------------
            Unit.m_squared => ConversionFactor(Unit.m) * ConversionFactor(Unit.m),
            Unit.mm_squared => ConversionFactor(Unit.mm) * ConversionFactor(Unit.mm),
            Unit.cm_squared => ConversionFactor(Unit.cm) * ConversionFactor(Unit.cm),
            Unit.in_squared => ConversionFactor(Unit.inch) * ConversionFactor(Unit.inch),
            Unit.ft_squared => ConversionFactor(Unit.ft) * ConversionFactor(Unit.ft),
            // -------------------- Force ---------------------------
            Unit.N => 1.0,
            Unit.lb => 4.44822,
            // ----------------- Pressure ----------------------
            Unit.Pa => 1,
            Unit.KPa => 1000,
            Unit.MPa => 1000 * 1000,
            Unit.GPa => 1000 * 1000 * 1000,
            Unit.Psi => 6894.76,
            Unit.Bar => 100000,
            //Default
            _ => 1.0
        };

        public static string[] UnitStrings(Unit unit) => unit switch
        {
            // ---------------- Unitless ------------------
            Unit.Unitless => ["-"],
            // ---------------- Length ------------------
            Unit.m => ["m"],
            Unit.mm => ["mm"],
            Unit.cm => ["cm"],
            Unit.inch => ["in", "inch", "\""],
            Unit.ft => ["ft", "feet", "'"],
            // ---------------- Area ------------------
            Unit.m_squared => ["m^2"],
            Unit.mm_squared => ["mm^2"],
            Unit.cm_squared => ["cm^2"],
            Unit.in_squared => ["in^2", "sqin"],
            Unit.ft_squared => ["ft^2", "sqft"],
            // -------------------- Force ---------------------------
            Unit.N => ["N"],
            Unit.lb => ["lb", "lbs"],
            // ----------------- Pressure ----------------------
            Unit.Pa => ["Pa", "pa"],
            Unit.KPa => ["KPa", "kpa", "Kpa"],
            Unit.MPa => ["MPa", "mpa", "Mpa"],
            Unit.GPa => ["GPa", "gpa", "Gpa"],
            Unit.Psi => ["psi", "Psi"],
            Unit.Bar => ["bar", "Bar"],
            //Default
            _ => ["-"]
        };

        public static Unit DefaultUnit(UnitType unitType) => unitType switch
        {
            UnitType.Length => Unit.m,
            UnitType.Area => Unit.m_squared,
            UnitType.Force => Unit.N,
            UnitType.Pressure => Unit.Pa,
            UnitType.Unitless => Unit.Unitless,
            _ => Unit.Unitless
        };

        public static int[] UnitTypeRange(UnitType unitType) => unitType switch
        {
            UnitType.Length => [0, 4],
            UnitType.Area => [5, 9],
            UnitType.Force => [10, 11],
            UnitType.Pressure => [12, 17],
            UnitType.Unitless => [18, 18],
            _ => [18, 18]
        };

        // ----------------------------------------------------------------------------
        public static List<string> TypeUnitStrings(UnitType unitType)
        {
            // Get the range of enum values (e.g., [start_index, end_index])
            int[] range = UnitTypeRange(unitType);
            int startIndex = range[0];
            int endIndex = range[1];

            // Use LINQ to generate the sequence of enum values, map them to strings,
            // and flatten the resulting lists into a single List<string>.
            return Enumerable.Range(startIndex, endIndex - startIndex + 1)
                .Cast<Unit>()
                .SelectMany(unit => UnitStrings(unit))
                .ToList();
        }

        public static double Convert(Unit inputUnit, double data, Unit outputUnit)
        {
            return data * ConversionFactor(inputUnit) / ConversionFactor(outputUnit);
        }
    }
}
