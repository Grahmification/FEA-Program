namespace FEA_Program.Models.SaveData
{
    /// <summary>
    /// Save file data for an FEA problem with no results included
    /// </summary>
    internal class ProblemData
    {
        public List<NodeProblemData> Nodes { get; set; } = [];
        public List<ElementProblemData> Elements { get; set; } = [];
        public List<MaterialSaveData> Materials { get; set; } = [];
        public Dictionary<int, int[]> ConnectivityMatrix { get; set; } = [];

        public ProblemTypes ProblemType { get; set; } = ProblemTypes.Truss_1D;
    }
}
