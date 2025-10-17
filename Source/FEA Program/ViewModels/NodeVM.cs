using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using SharpDX;
using System.ComponentModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class NodeVM: ObservableObject
    {
        // ---------------------- Events ----------------------

        public event EventHandler? EditRequest;
        public event EventHandler? DeleteRequest;

        // ---------------------- Properties ----------------------
        public Node Model { get; private set; } = Node.DummyNode();

        /// <summary>
        /// Get the node coordinates in user units
        /// </summary>
        public double[] Coordinates_mm => Model.Coordinates.Select(coord => coord * 1000.0).ToArray();

        public Vector3 DrawPosition => new(
            (float)(Coordinates_mm.Length > 0 ? Coordinates_mm[0] : 0.0),
            (float)(Coordinates_mm.Length > 1 ? Coordinates_mm[1] : 0.0),
            (float)(Coordinates_mm.Length > 2 ? Coordinates_mm[2] : 0.0));

        public bool FixedX => Model.Fixity[0] == 1;
        public bool FixedY => Model.Fixity.Length > 1 && Model.Fixity[1] == 1;
        public bool FixedZ => Model.Fixity.Length > 2 && Model.Fixity[2] == 1;

        public Vector3 Force => new(
            (float)(Model.Force.Length > 0 ? Model.Force[0] : 0.0),
            (float)(Model.Force.Length > 1 ? Model.Force[1] : 0.0),
            (float)(Model.Force.Length > 2 ? Model.Force[2] : 0.0));

        public bool HasForce => Model.ForceMagnitude > 0;


        // ---------------------- Commands ----------------------
        public ICommand? EditCommand { get; }
        public ICommand? DeleteCommand { get; }

        public NodeVM() { }

        public NodeVM(Node model)
        {
            Model = model;
            Model.PropertyChanged += OnModelPropertyChanged;
            EditCommand = new RelayCommand(() => EditRequest?.Invoke(this, EventArgs.Empty));
            DeleteCommand = new RelayCommand(() => DeleteRequest?.Invoke(this, EventArgs.Empty));
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is Node node)
            {
                if (e.PropertyName == nameof(Node.Coordinates))
                {
                    OnPropertyChanged(nameof(Coordinates_mm));
                    OnPropertyChanged(nameof(DrawPosition));
                }
                else if(e.PropertyName == nameof(Node.Force))
                {
                    OnPropertyChanged(nameof(Force));
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
    }
}
