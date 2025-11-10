namespace FEA_Program.Models
{
    // Note: Unclear if this interface is needed. The program may stay with one type of node
    
    /// <summary>
    /// Generic definition for a node
    /// </summary>
    internal interface INode
    {
        /// <summary>
        /// Fires when the solve state changes from true to false
        /// </summary>
        public event EventHandler<int>? SolutionInvalidated;

        /// <summary>
        /// Unique identifier
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Number of DOFs in the node.
        /// </summary>
        public int DOFs { get; }

        /// <summary>
        /// Coordinates of the node center in program units (m). Length depends on DOFs.
        /// </summary>
        public double[] Coordinates { get; set; }

        /// <summary>
        /// Displacement of the node center in program units (m). Length depends on DOFs.
        /// </summary>
        public double[] Displacement { get; }
    }
}
