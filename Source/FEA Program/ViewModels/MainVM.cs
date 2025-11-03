using FEA_Program.Models;
using FEA_Program.Utils;
using FEA_Program.ViewModels.Base;
using System.Windows.Input;

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
        public SidebarVM SidebarManager { get; private set; } = new();

        // ---------------------- Commands ----------------------
        /// <summary>
        /// RelayCommand for <see cref="CloseApplication"/>
        /// </summary>
        public ICommand ExitCommand { get; private set; }


        public MainVM()
        {
            ExitCommand = new RelayCommand(CloseApplication);

            try
            {
                Base.Debugging = _debugging;
                Base.ProgramVersion = ProgramExtensions.GetAssemblyVersion();

                Project.SetBase(Base);
                Project.ProblemReset += OnProblemReset;

                // Fire this manually - we won't get the first Reset when the ProjectVM is first created
                OnProblemReset(Project, Project.Problem.ProblemType);

                ViewPort.Base = Base;
                ViewPort.ResetCamera();
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

            // Setup the sidebar
            SidebarManager.Reset();
            SidebarManager.AddEditor(Project.Nodes.Editor);
            SidebarManager.AddEditor(Project.Nodes.ForceEditor);
            SidebarManager.AddEditor(Project.Elements.AddEditor);
            SidebarManager.AddEditor(Project.Materials.Editor);
        }

        // ------------------ Private Helpers ---------------------

        /// <summary>
        /// Closes the application
        /// </summary>
        private void CloseApplication()
        {
            App.Current.Shutdown();
        }
    }
}
