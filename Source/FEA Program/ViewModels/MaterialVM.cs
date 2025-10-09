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
        /// Youngs modulus in GPa
        /// </summary>
        public double E_GPa { get => Model.E / Math.Pow(1000.0, 3); set => Model.E = value * Math.Pow(1000.0, 3); }

        /// <summary>
        /// Yield strength in MPa
        /// </summary>
        public double Sy_MPa { get => Model.Sy / Math.Pow(1000.0, 2); set => Model.Sy = value * Math.Pow(1000.0, 2); }

        /// <summary>
        /// Ultimate strength in Pa
        /// </summary>
        public double Sut_MPa { get => Model.Sut / Math.Pow(1000.0, 2); set => Model.Sut = value * Math.Pow(1000.0, 2); }

        // ---------------------- Commands ----------------------
        public ICommand? EditCommand { get; }
        public ICommand? DeleteCommand { get; }

        public MaterialVM() { }

        public MaterialVM(Material model)
        {
            Model = model;
            EditCommand = new RelayCommand(() => EditRequest?.Invoke(this, EventArgs.Empty));
            DeleteCommand = new RelayCommand(() => DeleteRequest?.Invoke(this, EventArgs.Empty));
        }
    }
}
