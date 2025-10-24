using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class MaterialVM: ObservableObject
    {
        // ---------------------- Events ----------------------

        public event EventHandler? EditRequest;
        public event EventHandler? DeleteRequest;

        // ---------------------- Properties ----------------------
        public Material Model { get; private set; } = Material.DummyMaterial();

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
        public ICommand? EditCommand { get; }
        public ICommand? DeleteCommand { get; }

        public MaterialVM() { }

        public MaterialVM(Material model)
        {
            Model = model;
            Model.PropertyChanged += OnModelPropertyChanged;
            EditCommand = new RelayCommand(() => EditRequest?.Invoke(this, EventArgs.Empty));
            DeleteCommand = new RelayCommand(() => DeleteRequest?.Invoke(this, EventArgs.Empty));
        }

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
