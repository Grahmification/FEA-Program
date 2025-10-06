using System.Diagnostics;

namespace FEA_Program.UI
{
    internal class ProgramExtensions
    {
        /// <summary>
        /// Indicate if the executable has been generated in debug mode.
        /// </summary>
        static public bool IsDebugExecutable
        {
            get
            {
                bool isDebug = false;
                CheckDebugExecutable(ref isDebug);
                return isDebug;
            }
        }

        /// <summary>
        /// Indicate if the executable has been generated in debug mode.
        /// </summary>
        /// <param name="isDebug"></param>
        [Conditional("DEBUG")]
        static private void CheckDebugExecutable(ref bool isDebug) => isDebug = true;

        /// <summary>
        /// Gets the version (ex. 1.0.1) of the assembly which calls this method
        /// </summary>
        /// <returns></returns>
        static public string GetAssemblyVersion()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetCallingAssembly();
                var versInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return $"{versInfo.FileMajorPart}.{versInfo.FileMinorPart}.{versInfo.FileBuildPart}";
            }
            catch
            {
                return "X.X.X";
            }
        }
    }
}
