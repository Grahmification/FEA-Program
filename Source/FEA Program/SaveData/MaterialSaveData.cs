using FEA_Program.Models;

namespace FEA_Program.SaveData
{
    /// <summary>
    /// Save file data for a material
    /// </summary>
    internal class MaterialSaveData
    {
        public int ID { get; set; } = Constants.InvalidID;
        public string Name { get; set; } = "";
        public MaterialType Subtype { get; set; } = MaterialType.Other;

        /// <summary>
        /// Youngs modulus in Pa
        /// </summary>
        public double E { get; set; } = 0;

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
    }
}
