using FEA_Program.Models;

namespace FEA_Program.SaveData
{
    /// <summary>
    /// Save file data for a node with no results included
    /// </summary>
    internal class NodeProblemData
    {
        public int Dimension { get; set; } = 1;
        public int ID { get; set; } = IDClass.InvalidID;
        public double[] Coords { get; set; } = [0];
        public int[] Fixity { get; set; } = [0];
        public double[] Force { get; set; } = [0];
    }
}
