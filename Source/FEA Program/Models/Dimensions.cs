using FEA_Program.Converters;
using System.ComponentModel;

namespace FEA_Program.Models
{
    /// <summary>
    /// Different dimensions of problems
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Dimensions
    {
        [Description("-")]
        Invalid = 0,

        [Description("1D")]
        One = 1,

        [Description("2D")]
        Two = 2,

        [Description("3D")]
        Three = 3
    }
}
