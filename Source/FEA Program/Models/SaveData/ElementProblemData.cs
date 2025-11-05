namespace FEA_Program.Models.SaveData
{
    /// <summary>
    /// Save file data for an element with no results included
    /// </summary>
    internal class ElementProblemData
    {
        public int ID { get; set; } = IDClass.InvalidID;

        /// <summary>
        /// ID of the element's material
        /// </summary>
        public int MaterialID { get; set; } = IDClass.InvalidID;

        /// <summary>
        /// IDs of all nodes contained in the element
        /// </summary>
        public int[] NodeIDs { get; set; } = [];

        /// <summary>
        /// The number of DOFs for nodes in the element
        /// </summary>
        public int NodeDOFs { get; set; } = 1;
        public ElementTypes ElementType { get; set; } = ElementTypes.TrussLinear;

        /// <summary>
        /// The element's body force in program units (N/m^3) [X, Y, Z]
        /// </summary>
        public double[] BodyForce { get; set; } = [0, 0, 0];

        /// <summary>
        /// The element's traction force in program units (N/m) [X, Y, Z]
        /// </summary>
        public double[] TractionForce { get; set; } = [0, 0, 0];

        /// <summary>
        /// Array of element type specific arguments in program units
        /// </summary>
        public double[] ElementArgs { get; set; } = [];
    }
}
