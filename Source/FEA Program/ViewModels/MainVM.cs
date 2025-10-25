using FEA_Program.Models;
using FEA_Program.UI;
using FEA_Program.ViewModels.Base;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Top level program ViewModel
    /// </summary>
    internal class MainVM: ObservableObject
    {
        /// <summary>
        /// Set to true to enable debugging messages and other functionality
        /// </summary>
        private readonly bool _debugging = ProgramExtensions.IsDebugExecutable;

        // Sub VMs
        public BaseVM Base { get; private set; } = new();
        public ProjectVM Project { get; private set; } = new();
        public ViewPortVM ViewPort { get; private set; } = new();


        public MainVM()
        {
            try
            {
                Base.Debugging = _debugging;
                Base.ProgramVersion = ProgramExtensions.GetAssemblyVersion();

                Project.SetBase(Base);
                Project.ProblemReset += OnProblemReset;

                ViewPort.Base = Base;
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        // ---------------------- Event Methods ----------------------
        private void OnProblemReset(object? sender, ProblemTypes e)
        {
            if(sender is ProjectVM vm)
            {
                ViewPort.Is3Dimensional = vm.ThreeDimensional;
            }
        }
    }
}
