using SharpDX;
using System.Globalization;
using System.Windows.Data;

namespace FEA_Program.Converters
{
    /// <summary>
    /// Converts a vector to it's length
    /// </summary>
    public class VectorToLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Vector3 vect)
                return vect.Length();
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(); // One-way only
        }
    }
}
