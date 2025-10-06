using FEA_Program.Models;
using FEA_Program.ViewModels.Base;

namespace FEA_Program.ViewModels
{
    internal class MaterialVM: ObservableObject
    {
        public Material Model { get; private set; } = Material.DummyMaterial();


        /// <summary>
        /// Youngs modulus in GPa
        /// </summary>
        public double E_GPa => Model.E / Math.Pow(1000.0, 3);

        /// <summary>
        /// Yield strength in MPa
        /// </summary>
        public double Sy_MPa => Model.Sy / Math.Pow(1000.0, 2);

        /// <summary>
        /// Ultimate strength in Pa
        /// </summary>
        public double Sut_MPa => Model.Sut / Math.Pow(1000.0, 2);

        public MaterialVM(Material model)
        {
            Model = model;
        }
    }
}
