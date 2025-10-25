using FEA_Program.Converters;
using System.ComponentModel;


namespace FEA_Program.Models
{
    /// <summary>
    /// Different material classifications
    /// </summary>
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum MaterialType
    {
        [Description("Steel Alloy")]
        Steel_Alloy,

        [Description("Aluminum Alloy")]
        Aluminum_Alloy,

        [Description("Other")]
        Other
    }
}
