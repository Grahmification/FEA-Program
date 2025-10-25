using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using FEA_Program.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class NewProblemVM : ObservableObject
    {
        private NewProblemWindow? _window = null;

        public event EventHandler<ProblemTypes>? Accepted;

        // ---------------------- Properties ----------------------
        public ObservableCollection<ProblemTypes> AvailableProblemTypes { get; private set; } = [];

        public ProblemTypes? SelectedProblemType { get; set; } = null;

        // ---------------------- Sub VMs ----------------------
        public BaseVM Base { get; set; } = new();

        // ---------------------- Commands ----------------------
        public ICommand CreateCommand { get; }
        public ICommand AcceptCommand { get; }
        public ICommand CancelCommand { get; }

        public NewProblemVM()
        {
            CreateCommand = new RelayCommand(Create);
            AcceptCommand = new RelayCommand(Accept, () => SelectedProblemType != null);
            CancelCommand = new RelayCommand(Cancel);
        }
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
        private void Accept()
        {
            try
            {
                if (SelectedProblemType != null)
                    Accepted?.Invoke(this, SelectedProblemType.Value);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Base.LogAndDisplayException(ex);
            }

            _window?.Close();
        }
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
