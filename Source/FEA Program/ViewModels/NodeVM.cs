using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.ComponentModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class NodeVM: ObservableObject
    {
        // ---------------------- Events ----------------------

        public event EventHandler? EditRequest;
        public event EventHandler? DeleteRequest;
        public event EventHandler? EditForceRequest;

        // ---------------------- Properties ----------------------
        public Node Model { get; private set; } = Node.DummyNode();

        /// <summary>
        /// Get the node coordinates in user units
        /// </summary>
        public double[] Coordinates_mm => Model.Coordinates.Select(coord => coord * 1000.0).ToArray();
        public bool Selected { get; set; } = false;

        public bool FixedX => Model.Fixity[0] == 1;
        public bool FixedY => Model.Fixity.Length > 1 && Model.Fixity[1] == 1;
        public bool FixedZ => Model.Fixity.Length > 2 && Model.Fixity[2] == 1;

        public bool HasForce => Model.ForceMagnitude > 0;

        /// <summary>
        /// Display the node ID and the coordinates nicely in a text field
        /// </summary>
        public string IDCoordsDisplayText => $"{Model.ID} - ({string.Join(", ", Coordinates_mm.Select(c => c.ToString("F1")).ToArray() ?? [])})";


        // ---------------------- Commands ----------------------
        public ICommand? EditCommand { get; }
        public ICommand? DeleteCommand { get; }
        public ICommand? EditForceCommand { get; }
        public ICommand? DeleteForceCommand { get; }

        public NodeVM() { }

        public NodeVM(Node model)
        {
            Model = model;
            Model.PropertyChanged += OnModelPropertyChanged;
            EditCommand = new RelayCommand(() => EditRequest?.Invoke(this, EventArgs.Empty));
            DeleteCommand = new RelayCommand(() => DeleteRequest?.Invoke(this, EventArgs.Empty));
            EditForceCommand = new RelayCommand(() => EditForceRequest?.Invoke(this, EventArgs.Empty));
            DeleteForceCommand = new RelayCommand(DeleteForce);
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is Node node)
            {
                if (e.PropertyName == nameof(Node.Coordinates))
                {
                    OnPropertyChanged(nameof(Coordinates_mm));
                }
                else if(e.PropertyName == nameof(Node.Force))
                {
                    OnPropertyChanged(nameof(HasForce));
                }
                else if (e.PropertyName == nameof(Node.Fixity))
                {
                    OnPropertyChanged(nameof(FixedX));
                    OnPropertyChanged(nameof(FixedY));
                    OnPropertyChanged(nameof(FixedZ));
                }

            }
        }

        // ---------------------- Private Helpers ----------------------
        private void DeleteForce()
        {
            // Set all force components to zero
            Model.Force = new double[Model.Dimension];
        }
    }
}
