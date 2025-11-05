using FEA_Program.Models;
using FEA_Program.Utils;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for a material in the FEA program
    /// </summary>
    internal class MaterialVM: ObservableObject
    {
        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when the material requests that it be edited
        /// </summary>
        public event EventHandler? EditRequest;

        /// <summary>
        /// Fires when the material requests that it be deleted
        /// </summary>
        public event EventHandler? DeleteRequest;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The material's underlying model
        /// </summary>
        public Material Model { get; private set; } = Material.DummyMaterial();

        /// <summary>
        /// Properties for treeview display
        /// </summary>
        public ObservableCollection<TreePropertyVM> Properties { get; private set; } = [];

        /// <summary>
        /// Youngs modulus in user units
        /// </summary>
        public double E { get => App.Units.Modulus.ToUser(Model.E); set => Model.E = App.Units.Modulus.FromUser(value); }

        /// <summary>
        /// Yield strength in user units
        /// </summary>
        public double Sy { get => App.Units.Stress.ToUser(Model.Sy); set => Model.Sy = App.Units.Stress.FromUser(value); }

        /// <summary>
        /// Ultimate strength in user units
        /// </summary>
        public double Sut { get => App.Units.Stress.ToUser(Model.Sut); set => Model.Sut = App.Units.Stress.FromUser(value); }

        // ---------------------- Commands ----------------------

        /// <summary>
        /// Relay command for editing the material
        /// </summary>
        public ICommand? EditCommand { get; }

        /// <summary>
        /// Relay command for deleting the material
        /// </summary>
        public ICommand? DeleteCommand { get; }

        // ---------------------- Methods ----------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        public MaterialVM() { }

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="model">The material's underlying model</param>
        public MaterialVM(Material model)
        {
            Model = model;
            Model.PropertyChanged += OnModelPropertyChanged;
            EditCommand = new RelayCommand(() => EditRequest?.Invoke(this, EventArgs.Empty));
            DeleteCommand = new RelayCommand(() => DeleteRequest?.Invoke(this, EventArgs.Empty));

            // Create tree properties
            Properties.Add(new TreePropertyVM(this, nameof(E), () => E.ToString("F1")) { Name = "E", Unit = App.Units.Modulus});
            Properties.Add(new TreePropertyVM(Model, nameof(Material.V), () => Model.V.ToString("F2")) { Name = "V" });
            Properties.Add(new TreePropertyVM(this, nameof(Sy), () => Sy.ToString("F1")) { Name = "Sy", Unit = App.Units.Stress });
            Properties.Add(new TreePropertyVM(this, nameof(Sut), () => Sut.ToString("F1")) { Name = "Sut", Unit = App.Units.Stress });
            Properties.Add(new TreePropertyVM(Model, nameof(Material.Subtype), () => Attributes.GetDescription(Model.Subtype)) { Name = "Type" });
        }

        // ---------------------- Event Handlers ----------------------

        /// <summary>
        /// Called when a property in the model has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(sender is Material m)
            {
                if(e.PropertyName == nameof(Material.E))
                {
                    OnPropertyChanged(nameof(E));
                }
                else if (e.PropertyName == nameof(Material.Sy))
                {
                    OnPropertyChanged(nameof(Sy));
                }
                else if (e.PropertyName == nameof(Material.Sut))
                {
                    OnPropertyChanged(nameof(Sut));
                }
            }
        }
    }
}
