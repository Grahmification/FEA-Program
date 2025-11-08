using FEA_Program.Converters;
using System.ComponentModel;


namespace FEA_Program.Models
{
    /// <summary>
    /// Different categories of FEA problems
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ProblemTypes
    {
        [Description("Truss 1D")]
        Truss_1D,

        [Description("Beam 1D")]
        Beam_1D,

        [Description("Truss 3D")]
        Truss_3D
    }
}
