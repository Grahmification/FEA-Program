namespace FEA_Program.Models
{
    internal class Units
    {
        // ----------------- UPDATE THESE IF NEW UNITS ARE ADDED -------------------
        private static double ConversionFactor(int Unit)
        {
            switch (Unit)
            {
                // ---------------- Unitless ------------------
                case (int)AllUnits.Unitless:
                    {
                        return 1.0;
                    }

                // ---------------- Length ------------------
                case (int)AllUnits.m:
                    {
                        return 1.0d;
                    }
                case (int)AllUnits.mm:
                    {
                        return 0.001d;
                    }
                case (int)AllUnits.cm:
                    {
                        return 0.01d;
                    }
                case (int)AllUnits.inch:
                    {
                        return 0.0254d;
                    }
                case (int)AllUnits.ft:
                    {
                        return 0.3048d;
                    }

                // ---------------------------Area----------------------
                case (int)AllUnits.m_squared:
                    {
                        return ConversionFactor((int)AllUnits.m) * ConversionFactor((int)AllUnits.m);
                    }
                case (int)AllUnits.mm_squared:
                    {
                        return ConversionFactor((int)AllUnits.mm) * ConversionFactor((int)AllUnits.mm);
                    }
                case (int)AllUnits.cm_squared:
                    {
                        return ConversionFactor((int)AllUnits.cm) * ConversionFactor((int)AllUnits.cm);
                    }
                case (int)AllUnits.in_squared:
                    {
                        return ConversionFactor((int)AllUnits.inch) * ConversionFactor((int)AllUnits.inch);
                    }
                case (int)AllUnits.ft_squared:
                    {
                        return ConversionFactor((int)AllUnits.ft) * ConversionFactor((int)AllUnits.ft);
                    }

                // -------------------- Force ---------------------------

                case (int)AllUnits.N:
                    {
                        return 1d;
                    }
                case (int)AllUnits.lb:
                    {
                        return 4.44822d;
                    }

                // ----------------- Pressure ----------------------
                case (int)AllUnits.Pa:
                    {
                        return 1d;
                    }
                case (int)AllUnits.KPa:
                    {
                        return 1000d;
                    }
                case (int)AllUnits.MPa:
                    {
                        return 1000 * 1000;
                    }
                case (int)AllUnits.GPa:
                    {
                        return 1000 * 1000 * 1000;
                    }
                case (int)AllUnits.Psi:
                    {
                        return 6894.76d;
                    }
                case (int)AllUnits.Bar:
                    {
                        return 100000d;
                    }

                default:
                    {
                        return (double)default;
                    }
            }
        }
        public static string[] UnitStrings(AllUnits Unit)
        {
            switch (Unit)
            {
                // ---------------- Unitless ------------------
                case AllUnits.Unitless:
                    {
                        return ["-"];
                    }

                // -------------------- Length -------------------
                case AllUnits.m:
                    {
                        return new[] { "m" };
                    }
                case AllUnits.mm:
                    {
                        return new[] { "mm" };
                    }
                case AllUnits.cm:
                    {
                        return new[] { "cm" };
                    }
                case AllUnits.inch:
                    {
                        return new[] { "in", "inch", "\"" };
                    }
                case AllUnits.ft:
                    {
                        return new[] { "ft", "feet", "'" };
                    }

                // ---------------------------Area----------------------

                case AllUnits.m_squared:
                    {
                        return new[] { "m^2" };
                    }
                case AllUnits.mm_squared:
                    {
                        return new[] { "mm^2" };
                    }
                case AllUnits.cm_squared:
                    {
                        return new[] { "cm^2" };
                    }
                case AllUnits.in_squared:
                    {
                        return new[] { "in^2", "sqin" };
                    }
                case AllUnits.ft_squared:
                    {
                        return new[] { "ft^2", "sqft" };
                    }

                // -------------------- Force ---------------------------

                case AllUnits.N:
                    {
                        return new[] { "N" };
                    }
                case AllUnits.lb:
                    {
                        return new[] { "lb", "lbs" };
                    }

                // ----------------- Pressure ----------------------
                case AllUnits.Pa:
                    {
                        return new[] { "Pa", "pa" };
                    }
                case AllUnits.KPa:
                    {
                        return new[] { "KPa", "kpa", "Kpa" };
                    }
                case AllUnits.MPa:
                    {
                        return new[] { "MPa", "mpa", "Mpa" };
                    }
                case AllUnits.GPa:
                    {
                        return new[] { "GPa", "gpa", "Gpa" };
                    }
                case AllUnits.Psi:
                    {
                        return new[] { "psi", "Psi" };
                    }
                case AllUnits.Bar:
                    {
                        return new[] { "bar", "Bar" };
                    }

                default:
                    {
                        return null;
                    }
            }
        }
        public static AllUnits DefaultUnit(DataUnitType UnitType)
        {
            switch (UnitType)
            {
                case DataUnitType.Unitless:
                    {
                        return AllUnits.Unitless;
                    }

                case DataUnitType.Length:
                    {
                        return AllUnits.m;
                    }

                case DataUnitType.Area:
                    {
                        return AllUnits.m_squared;
                    }

                case DataUnitType.Force:
                    {
                        return AllUnits.N;
                    }

                case DataUnitType.Pressure:
                    {
                        return AllUnits.Pa;
                    }

                default:
                    {
                        return default;
                    }
            }
        }
        public static int[] UnitTypeRange(DataUnitType UnitType)
        {
            switch (UnitType)
            {
                case DataUnitType.Length:
                    {
                        return new[] { 0, 4 };
                    }

                case DataUnitType.Area:
                    {
                        return new[] { 5, 9 };
                    }

                case DataUnitType.Force:
                    {
                        return new[] { 10, 11 };
                    }

                case DataUnitType.Pressure:
                    {
                        return new[] { 12, 17 };
                    }

                case DataUnitType.Unitless:
                    {
                        return [18, 18];
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        // ----------------------------------------------------------------------------
        public static List<string> TypeUnitStrings(DataUnitType UnitType)
        {
            int[] range = UnitTypeRange(UnitType);

            var output = new List<string>();

            for (int i = range[0], loopTo = range[1]; i <= loopTo; i++) // search through the enum range for that type
            {
                foreach (string S in UnitStrings((AllUnits)i)) // get all the strings for each enum and add to output
                    output.Add(S);
            }

            return output;
        }
        public static AllUnits UnitEnums(string UnitString)
        {
            for (int I = 0, loopTo = Enum.GetNames(typeof(AllUnits)).Count() - 1; I <= loopTo; I++)
            {
                if (UnitStrings((AllUnits)I).Contains(UnitString))
                {
                    return (AllUnits)I;
                }
            }
            return (AllUnits)(-1); // if could not find
        }
        public static double Convert(AllUnits InputUnit, double Data, AllUnits OutputUnit)
        {
            return Data * ConversionFactor((int)InputUnit) / ConversionFactor((int)OutputUnit);
        }



        public enum DataUnitType
        {
            Length = 0, // m
            Area = 1, // m^2
            Force = 2, // N
            Pressure = 3, // Pa
            Unitless = 4 // [-]
        }
        public enum AllUnits
        {
            // --------------- Length -------------------
            mm,
            cm,
            m,
            inch,
            ft,
            // ------------- Area -----------------------
            mm_squared,
            cm_squared,
            m_squared,
            in_squared,
            ft_squared,
            // ------------- Force -----------------------
            N,
            lb,
            // ------------- Pressure ---------------
            KPa,
            MPa,
            GPa,
            Pa,
            Psi,
            Bar,
            // ------------- Unitless ---------------
            Unitless,
        }
    }
}
