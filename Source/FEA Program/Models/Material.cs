using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal class Material(string name, double E_GPa, double v, double sy_MPa, double sut_MPa, int id, MaterialType subtype = MaterialType.Other)
    {
        public int ID { get; private set; } = id;
        public string Name { get; private set; } = name;
        public MaterialType Subtype { get; set; } = subtype;

        /// <summary>
        /// Youngs modulus in Pa
        /// </summary>
        public double E { get; set; } = E_GPa * 1000 * 1000 * 1000; // convert to Pa

        /// <summary>
        /// Poissons ratio
        /// </summary>
        public double V { get; set; } = v;

        /// <summary>
        /// Yield strength in Pa
        /// </summary>
        public double Sy { get; set; } = sy_MPa * 1000 * 1000;

        /// <summary>
        /// Ultimate strength in Pa
        /// </summary>
        public double Sut { get; set; } = sut_MPa * 1000 * 1000;


        /// <summary>
        /// Youngs modulus in GPa
        /// </summary>
        public double E_GPa => E / Math.Pow(1000.0, 3);

        /// <summary>
        /// Yield strength in MPa
        /// </summary>
        public double Sy_MPa => Sy / Math.Pow(1000.0, 2);

        /// <summary>
        /// Ultimate strength in Pa
        /// </summary>
        public double Sut_MPa => Sut / Math.Pow(1000.0, 2);

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

        public static Material DummyMaterial() => new("Dummy matieral", 1, 0, 1, 1, -1);
    }

    public enum MaterialType
    {
        Steel_Alloy,
        Aluminum_Alloy,
        Other
    }
}
