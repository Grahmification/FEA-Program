namespace FEA_Program.Models
{
    /// <summary>
    /// Types of various units
    /// </summary>
    internal enum UnitType
    {
        Length = 0, // m
        Area = 1, // m^2
        Force = 2, // N
        Pressure = 3, // Pa
        Unitless = 4 // [-]
    }

    /// <summary>
    /// Values for various units
    /// </summary>
    internal enum Unit
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
