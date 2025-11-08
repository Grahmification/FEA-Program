using SharpDX;
using Color = System.Windows.Media.Color;

namespace FEA_Program.Views.Helix
{
    /// <summary>
    /// Helper methods
    /// </summary>
    internal class Utils
    {
        /// <summary>
        /// Convert a System.Windows.Media.Color to a SharpDX.Color4
        /// </summary>
        /// <param name="color">The color to convert</param>
        /// <returns></returns>
        public static Color4 ToColor4(Color color)
        {
            return new Color4(color.ScR, color.ScG, color.ScB, color.ScA);
        }
    }
}
