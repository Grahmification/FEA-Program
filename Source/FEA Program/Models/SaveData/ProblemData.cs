namespace FEA_Program.Models.SaveData
{
    /// <summary>
    /// Save file data for an FEA problem with no results included
    /// </summary>
    internal class ProblemData
    {
        /// <summary>
        /// All the nodes in the FEA problem
        /// </summary>
        public List<NodeProblemData> Nodes { get; set; } = [];

        /// <summary>
        /// Aall the elements in the FEA problem
        /// </summary>
        public List<ElementProblemData> Elements { get; set; } = [];

        /// <summary>
        /// All the materials in the FEA problem
        /// </summary>
        public List<MaterialSaveData> Materials { get; set; } = [];

        /// <summary>
        /// The matrix connecting elements and nodes
        /// </summary>
        public Dictionary<int, int[]> ConnectivityMatrix { get; set; } = [];

        /// <summary>
        /// The FEA problem type
        /// </summary>
        public ProblemTypes ProblemType { get; set; } = ProblemTypes.Truss_1D;
    }
}
