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
        public double[] UserCoordinates => Model.Coordinates.Select(coord => App.Units.Length.ToUser(coord)).ToArray();
        public double[] UserDisplacement => Model.Displacement.Select(coord => App.Units.Length.ToUser(coord)).ToArray();
        public double[] UserFinalPos => UserCoordinates.Zip(UserDisplacement, (coord, disp) => coord + disp).ToArray();
        public double ForceMagnitude => Geometry.Magnitude(Model.Force.Take(3).ToArray()); // Compute based on the first 3 items
        public bool DisplacementIsValid => ArrayHasValidValues(Model.Displacement);

        public bool Selected { get; set; } = false;

        public bool FixedX => Model.Fixity[0] == 1;
        public bool FixedY => Model.Fixity.Length > 1 && Model.Fixity[1] == 1;
        public bool FixedZ => Model.Fixity.Length > 2 && Model.Fixity[2] == 1;

        public bool HasForce => ForceMagnitude > 0;

        /// <summary>
        /// Display the node ID and the coordinates nicely in a text field
        /// </summary>
        public string IDCoordsDisplayText => $"{Model.ID} - ({string.Join(", ", UserCoordinates.Select(c => c.ToString("F1")).ToArray() ?? [])})";


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
                    OnPropertyChanged(nameof(UserCoordinates));
                    OnPropertyChanged(nameof(UserFinalPos));
                }
                else if (e.PropertyName == nameof(Node.Displacement))
                {
                    OnPropertyChanged(nameof(UserDisplacement));
                    OnPropertyChanged(nameof(UserFinalPos));
                    OnPropertyChanged(nameof(DisplacementIsValid));
                }
                else if(e.PropertyName == nameof(Node.Force))
                {
                    OnPropertyChanged(nameof(HasForce));
                    OnPropertyChanged(nameof(ForceMagnitude));
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

        /// <summary>
        /// Check an array for invalid values
        /// </summary>
        /// <param name="values"></param>
        /// <returns>True if all values are valid</returns>
        public static bool ArrayHasValidValues(double[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                // Handle erroneous values
                if (double.IsNaN(values[i]) || double.IsInfinity(values[i]))
                    return false;
            }

            return true;
        }
    }
}
