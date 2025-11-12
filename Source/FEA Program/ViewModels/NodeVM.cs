using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for a node in the FEA program
    /// </summary>
    internal class NodeVM: ObservableObject, ISelectable
    {
        /// <summary>
        /// Names of each noode coordinate, sorted by index
        /// </summary>
        public static string[] CoordinateNames = ["X", "Y", "Z"];

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when the node requests that it be edited
        /// </summary>
        public event EventHandler? EditRequest;

        /// <summary>
        /// Fires when the node requests that it be deleted
        /// </summary>
        public event EventHandler? DeleteRequest;

        /// <summary>
        /// Fires when the node requests that its force be edited
        /// </summary>
        public event EventHandler? EditForceRequest;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The node's underlying model
        /// </summary>
        public Node Model { get; private set; } = Node.DummyNode();

        /// <summary>
        /// Properties for treeview display
        /// </summary>
        public ObservableCollection<TreePropertyVM> Properties { get; private set; } = [];

        /// <summary>
        /// Force properties for treeview display
        /// </summary>
        public ObservableCollection<TreePropertyVM> ForceProperties { get; private set; } = [];

        /// <summary>
        /// Properties for results treeview display
        /// </summary>
        public ObservableCollection<TreePropertyVM> ResultProperties { get; private set; } = [];

        /// <summary>
        /// Get the node position in user units
        /// </summary>
        public double[] UserCoordinates => Model.Position.Select(coord => App.Units.Length.ToUser(coord)).ToArray();

        /// <summary>
        /// Get the node displacement in user units
        /// </summary>
        public double[] UserDisplacement => Model.Displacement.Select(coord => App.Units.Length.ToUser(coord)).ToArray();

        /// <summary>
        /// Get the final displaced position in user units
        /// </summary>
        public double[] UserFinalPos => UserCoordinates.Zip(UserDisplacement, (coord, disp) => coord + disp).ToArray();

        /// <summary>
        /// The node force in user units
        /// </summary>
        public double[] Force => Model.Force.Take(3).Select(f => App.Units.Force.ToUser(f)).ToArray();  // Compute based on the first 3 items

        /// <summary>
        /// The node reaction force in user units
        /// </summary>
        public double[] ReactionForce => Model.ReactionForce.Select(f => App.Units.Force.ToUser(f)).ToArray();

        /// <summary>
        /// The node force magnitude in user units
        /// </summary>
        public double ForceMagnitude => Geometry.Magnitude(Force);

        /// <summary>
        /// True if the displacement is a valid number
        /// </summary>
        public bool DisplacementIsValid => ArrayHasValidValues(Model.Displacement);

        /// <summary>
        /// Whether the item is selected
        /// </summary>
        public bool Selected { get; set; } = false;

        /// <summary>
        /// True if the node X coordinate is fixed
        /// </summary>
        public bool FixedX => Model.Fixity[0] == 1;

        /// <summary>
        /// True if the node Y coordinate is fixed
        /// </summary>
        public bool FixedY => Model.Fixity.Length > 1 && Model.Fixity[1] == 1;

        /// <summary>
        /// True if the node Z coordinate is fixed
        /// </summary>
        public bool FixedZ => Model.Fixity.Length > 2 && Model.Fixity[2] == 1;

        /// <summary>
        /// True if the node has a non-zero force
        /// </summary>
        public bool HasForce => ForceMagnitude > 0;

        /// <summary>
        /// Display the node ID and the coordinates nicely in a text field
        /// </summary>
        public string IDCoordsDisplayText => $"{Model.ID} - ({string.Join(", ", UserCoordinates.Select(c => c.ToString("F1")).ToArray() ?? [])})";

        // ---------------------- Commands ----------------------

        /// <summary>
        /// Relay command for editing the node
        /// </summary>
        public ICommand? EditCommand { get; }

        /// <summary>
        /// Relay command for deleting the node
        /// </summary>
        public ICommand? DeleteCommand { get; }

        /// <summary>
        /// Relay command for editing the node's force
        /// </summary>
        public ICommand? EditForceCommand { get; }

        /// <summary>
        /// Relay command for deleting the node's force
        /// </summary>
        public ICommand? DeleteForceCommand { get; }

        // ---------------------- Methods ----------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        public NodeVM() { }

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="model">The node's underlying model</param>
        public NodeVM(Node model)
        {
            Model = model;
            Model.PropertyChanged += OnModelPropertyChanged;
            EditCommand = new RelayCommand(() => EditRequest?.Invoke(this, EventArgs.Empty));
            DeleteCommand = new RelayCommand(() => DeleteRequest?.Invoke(this, EventArgs.Empty));
            EditForceCommand = new RelayCommand(() => EditForceRequest?.Invoke(this, EventArgs.Empty));
            DeleteForceCommand = new RelayCommand(DeleteForce);

            // Create tree properties

            // Need to hardcode indexes or can end up with undefined functionality
            Func<string>[] valueFunctions = [
                () => $"{UserCoordinates[0]:F2}" + (Model.Fixity[0] == 1 ? " (Fixed)" : ""),
                () => $"{UserCoordinates[1]:F2}" + (Model.Fixity[1] == 1 ? " (Fixed)" : ""),
                () => $"{UserCoordinates[2]:F2}" + (Model.Fixity[2] == 1 ? " (Fixed)" : "")
             ];

            for(int i = 0; i < (int)Model.Dimension; i++)
            {
                var vm = new TreePropertyVM(this, nameof(UserCoordinates), valueFunctions[i])
                {
                    Name = CoordinateNames[i],
                    Unit = App.Units.Length,
                    UnitsAfterValue = false
                };

                vm.PropertyChanged += OnTreePropertyPropertyChanged;
                Properties.Add(vm);
            }

            // Create force tree properties
            ForceProperties.Add(new TreePropertyVM(this, nameof(ForceMagnitude), () => ForceMagnitude.ToString("F2")) { Name = "Magnitude", Unit = App.Units.Force});
            ForceProperties.Add(new TreePropertyVM(this, nameof(Force), () => string.Join(", ", Force.Select(f => f.ToString("F1")))) { Name = "Components", Unit = App.Units.Force, UnitsAfterValue = false });

            foreach (var vm in ForceProperties)
                vm.PropertyChanged += OnTreePropertyPropertyChanged;

            // Create result tree properties
            ResultProperties.Add(new TreePropertyVM(this, nameof(UserFinalPos), () => string.Join(", ", UserFinalPos.Select(f => f.ToString("G2")))) { Name = "Position", Unit = App.Units.Length, UnitsAfterValue = false });
            ResultProperties.Add(new TreePropertyVM(this, nameof(UserDisplacement), () => Geometry.Magnitude(UserDisplacement).ToString("G3")) { Name = "Displacement", Unit = App.Units.Length, UnitsAfterValue = true });
            ResultProperties.Add(new TreePropertyVM(this, nameof(UserDisplacement), () => string.Join(", ", UserDisplacement.Select(f => f.ToString("G3")))) { Name = "Components", Unit = App.Units.Length, UnitsAfterValue = false });
            ResultProperties.Add(new TreePropertyVM(this, nameof(ReactionForce), () => Geometry.Magnitude(ReactionForce).ToString("G2")) { Name = "Reaction Force", Unit = App.Units.Force, UnitsAfterValue = true });
            ResultProperties.Add(new TreePropertyVM(this, nameof(ReactionForce), () => string.Join(", ", ReactionForce.Select(f => f.ToString("G2")))) { Name = "Components", Unit = App.Units.Force, UnitsAfterValue = false });

            foreach (var vm in ResultProperties)
                vm.PropertyChanged += OnTreePropertyPropertyChanged;
        }

        // ---------------------- Event Handlers ----------------------

        /// <summary>
        /// Called then a property in the model has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is Node node)
            {
                if (e.PropertyName == nameof(Node.Position))
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
                else if (e.PropertyName == nameof(Node.ReactionForce))
                {
                    OnPropertyChanged(nameof(ReactionForce));
                }
                else if(e.PropertyName == nameof(Node.Force))
                {
                    OnPropertyChanged(nameof(HasForce));
                    OnPropertyChanged(nameof(ForceMagnitude));
                    OnPropertyChanged(nameof(Force));
                }
                else if (e.PropertyName == nameof(Node.Fixity))
                {
                    OnPropertyChanged(nameof(FixedX));
                    OnPropertyChanged(nameof(FixedY));
                    OnPropertyChanged(nameof(FixedZ));
                }
            }
        }

        /// <summary>
        /// Called when a property in <see cref="Properties"/> changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreePropertyPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is TreePropertyVM vm)
            {
                if (e.PropertyName == nameof(TreePropertyVM.Selected))
                {
                    // Propagate selections to this VM
                    Selected = vm.Selected;
                }
            }
        }

        // ---------------------- Private Helpers ----------------------

        /// <summary>
        /// Deletes the force attached to the node
        /// </summary>
        private void DeleteForce()
        {
            // Set all force components to zero
            Model.Force = new double[(int)Model.Dimension];
        }

        // ---------------------- Static Methods ----------------------

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
