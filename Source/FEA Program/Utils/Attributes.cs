using System.ComponentModel;
using System.Reflection;

namespace FEA_Program.Utils
{
    internal class Attributes
    {
        /// <summary>
        /// Gets the first description attribute from the object
        /// </summary>
        /// <param name="value">The object to get the description from</param>
        /// <returns>The description string</returns>
        public static string GetDescription(object? value)
        {
            // Tutorial: https://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/
            if (value != null)
            {
                FieldInfo? fi = value.GetType().GetField(value?.ToString() ?? "");
                if (fi != null)
                {
                    var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    return attributes.Length > 0 && !string.IsNullOrEmpty(attributes[0].Description) ? attributes[0].Description : value?.ToString() ?? "";
                }
            }

            return string.Empty;
        }
    }
}
