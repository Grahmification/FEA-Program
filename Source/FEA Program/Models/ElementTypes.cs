using FEA_Program.Converters;
using System.ComponentModel;

namespace FEA_Program.Models
{
    /// <summary>
    /// Different types of elements
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ElementTypes
    {
        [Description("Linear Truss")]
        TrussLinear,

        [Description("Linear Beam")]
        BeamLinear
    }
}
