using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal class Material(string name, double E_GPa, double v, double sy_MPa, double sut_MPa, int id, MaterialType subtype = MaterialType.Other)
    {
        private double _Sy = sy_MPa * 1000d * 1000d; // yield strength in Pa
        private double _Sut = sut_MPa * 1000d * 1000d; // ultimate strength in Pa

        public int ID { get; private set; } = id;
        public string Name { get; private set; } = name;

        /// <summary>
        /// Youngs modulus in Pa
        /// </summary>
        public double E { get; private set; } = E_GPa * 1000d * 1000d * 1000d; // convert to Pa

        /// <summary>
        /// Poissons ratio
        /// </summary>
        public double V { get; private set; } = v;
        public MaterialType Subtype { get; private set; } = subtype;

        /// <summary>
        /// Yield strength in MPa
        /// </summary>
        public double Sy_MPa => _Sy / Math.Pow(1000.0, 2);

        /// <summary>
        /// Ultimate strength in Pa
        /// </summary>
        public double Sut_MPa => _Sut / Math.Pow(1000.0, 2);

        /// <summary>
        /// Youngs modulus in GPa
        /// </summary>
        public double E_GPa => E / Math.Pow(1000.0, 3);
        public DenseMatrix D_Matrix_2D
        {
            get
            {
                var output = new DenseMatrix(3, 3);
                output.Clear(); // set all vals to 0

                output[0, 0] = 1;
                output[0, 1] = V;
                output[1, 0] = V;
                output[1, 1] = 1;
                output[2, 2] = (1 - V) / 2.0;

                return (DenseMatrix)(output * E / (1.0 - V * V));
            }
        }
    }

    public enum MaterialType
    {
        Steel_Alloy,
        Aluminum_Alloy,
        Other
    }
}
