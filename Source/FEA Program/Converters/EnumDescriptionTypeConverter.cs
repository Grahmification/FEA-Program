using FEA_Program.Utils;
using System.ComponentModel;

namespace FEA_Program.Converters
{
    /// <summary>
    /// Allows displaying enums based on their description field
    /// </summary>
    /// <param name="type"></param>
    internal class EnumDescriptionTypeConverter(Type type) : EnumConverter(type)
    {
        public override object ConvertTo(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return Attributes.GetDescription(value);
            }

            return base.ConvertTo(context, culture, value, destinationType) ?? "";
        }

    }
}
