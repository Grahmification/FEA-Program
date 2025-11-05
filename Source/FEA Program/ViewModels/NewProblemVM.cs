using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using FEA_Program.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for a control to set up a new FEA problem
    /// </summary>
    internal class NewProblemVM : ObservableObject
    {
        /// <summary>
        /// The attached window
        /// </summary>
        private NewProblemWindow? _window = null;

        // ---------------------- Events ----------------------

        /// <summary>
        /// Raised when the new problem is accepted
        /// </summary>
        public event EventHandler<ProblemTypes>? Accepted;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The list of problem types to choose from
        /// </summary>
        public ObservableCollection<ProblemTypes> AvailableProblemTypes { get; private set; } = [];

        /// <summary>
        /// The currently selected problem type for the new problem
        /// </summary>
        public ProblemTypes? SelectedProblemType { get; set; } = null;

        // ---------------------- Sub VMs ----------------------

        /// <summary>
        /// Base VM for handling errors and status
        /// </summary>
        public BaseVM Base { get; set; } = new();

        // ---------------------- Commands ----------------------

        /// <summary>
        /// Relaycommand to create the editor window
        /// </summary>
        public ICommand CreateCommand { get; }

        /// <summary>
        /// Relay command to accept the new problem
        /// </summary>
        public ICommand AcceptCommand { get; }

        /// <summary>
        /// Relay command to cancel the new problem
        /// </summary>
        public ICommand CancelCommand { get; }

        // ---------------------- Methods ----------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        public NewProblemVM()
        {
            CreateCommand = new RelayCommand(Create);
            AcceptCommand = new RelayCommand(Accept, () => SelectedProblemType != null);
            CancelCommand = new RelayCommand(Cancel);
        }

        /// <summary>
        /// Displays the window to setup the new problem
        /// </summary>
        /// <param name="problemTypes">The list of problem types to choose from</param>
        public void DislayWindow(List<ProblemTypes> problemTypes)
        {
            SelectedProblemType = null;
            AvailableProblemTypes = new(problemTypes);

            if (AvailableProblemTypes.Count > 0)
                SelectedProblemType = AvailableProblemTypes[0];

            _window = new NewProblemWindow
            {
                DataContext = this
            };

            _window.Show();
            _window.BringIntoView();
        }

        /// <summary>
        /// Creates a new instance of the editor window
        /// </summary>
        private void Create()
        {
            try
            {
                DislayWindow([ProblemTypes.Beam_1D, ProblemTypes.Truss_3D]);
            }
            catch(Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }

        /// <summary>
        /// Accepts the new problem
        /// </summary>
        private void Accept()
        {
            try
            {
                if (SelectedProblemType != null)
                    Accepted?.Invoke(this, SelectedProblemType.Value);
            }
            // Handle silently if the user cancels due to a warning message
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }

            _window?.Close();
        }

        /// <summary>
        /// Cancels the new problem
        /// </summary>
        private void Cancel()
        {
            try
            {
                _window?.Close();
            }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }
        }
    }
}
