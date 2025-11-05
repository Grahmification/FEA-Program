using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    /// <summary>
    /// A material in the FEA problem
    /// </summary>
    /// <param name="name">The material's name</param>
    /// <param name="e">Young's modulus in Pa</param>
    /// <param name="id">Unique identifier for the class</param>
    /// <param name="subtype">Optional material's subtype</param>
    internal class Material(string name, double e, int id = IDClass.InvalidID, MaterialType subtype = MaterialType.Other) : IDClass(id), ICloneable
    {
        public string Name { get; set; } = name;
        public MaterialType Subtype { get; set; } = subtype;

        /// <summary>
        /// Youngs modulus in Pa
        /// </summary>
        public double E { get; set; } = e;

        /// <summary>
        /// Poissons ratio
        /// </summary>
        public double V { get; set; } = 0;

        /// <summary>
        /// Yield strength in Pa
        /// </summary>
        public double Sy { get; set; } = 0;

        /// <summary>
        /// Ultimate strength in Pa
        /// </summary>
        public double Sut { get; set; } = 0;
        
        /// <summary>
        /// Gets the material constutive matrix for a 2D material in program units
        /// </summary>
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

        // ---------------------- Public Methods ----------------------

        /// <summary>
        /// Creates a material with generic parameters for a alternative null placeholder
        /// </summary>
        /// <returns></returns>
        public static Material DummyMaterial() => new("Dummy material", 0);

        /// <summary>
        /// Clone the class
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Material(Name, E, ID, Subtype)
            {
                V = this.V,
                Sy = this.Sy,
                Sut = this.Sut
            };
        }

        /// <summary>
        /// Import parameters from another matieral, while retaining ID.
        /// </summary>
        /// <param name="other"></param>
        public void ImportParameters(Material other)
        {
            Name = other.Name;
            Subtype = other.Subtype;
            E = other.E;
            V = other.V;
            Sy = other.Sy;
            Sut = other.Sut;
        }
    }
}
