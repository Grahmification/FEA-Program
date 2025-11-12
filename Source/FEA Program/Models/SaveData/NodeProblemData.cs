namespace FEA_Program.Models.SaveData
{
    /// <summary>
    /// Save file data for a node with no results included
    /// </summary>
    internal class NodeProblemData
    {
        public int ID { get; set; } = IDClass.InvalidID;

        /// <summary>
        /// Dimension of the node
        /// </summary>
        public Dimensions Dimension { get; set; } = Dimensions.Invalid;

        /// <summary>
        /// True if the node includes rotary DOFs and moments
        /// </summary>
        public bool HasRotation { get; set; } = false;

        /// <summary>
        /// The node coordinates in program units (m)
        /// </summary>
        public double[] Position { get; set; } = [0];

        /// <summary>
        /// The fixity for each node dimension
        /// </summary>
        public int[] Fixity { get; set; } = [0];

        /// <summary>
        /// Whether each rotary dimension of the node is fixed. 0 = floating, 1 = fixed.
        /// </summary>
        public int[] RotationFixity { get; set; } = []; // Default to empty for rotational parameters

        /// <summary>
        /// The node force in program units (m)
        /// </summary>
        public double[] Force { get; set; } = [0];

        /// <summary>
        /// The node moment in program units [Nm].
        /// </summary>
        public double[] Moment { get; set; } = []; // Default to empty for rotational parameters

    }
}
