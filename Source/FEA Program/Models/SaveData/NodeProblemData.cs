namespace FEA_Program.Models.SaveData
{
    /// <summary>
    /// Save file data for a node with no results included
    /// </summary>
    internal class NodeProblemData
    {
        public int ID { get; set; } = IDClass.InvalidID;

        /// <summary>
        /// Number of DOFs in the node
        /// </summary>
        public int Dimension { get; set; } = 1;
        
        /// <summary>
        /// The node coordinates in program units (m)
        /// </summary>
        public double[] Coords { get; set; } = [0];

        /// <summary>
        /// The fixity for each node dimension
        /// </summary>
        public int[] Fixity { get; set; } = [0];

        /// <summary>
        /// The node force in program units (m)
        /// </summary>
        public double[] Force { get; set; } = [0];
    }
}
