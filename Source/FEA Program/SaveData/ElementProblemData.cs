using FEA_Program.Models;

namespace FEA_Program.SaveData
{
    /// <summary>
    /// Save file data for an element with no results included
    /// </summary>
    internal class ElementProblemData
    {
        public int ID { get; set; } = Constants.InvalidID;
        public int MaterialID { get; set; } = Constants.InvalidID;
        public int NodeDOFs { get; set; } = 1;
        public ElementTypes ElementType { get; set; } = ElementTypes.BarLinear;

        public double[] BodyForce { get; set; } = [0, 0, 0];
        public double[] TractionForce { get; set; } = [0, 0, 0];
        public double[] ElementArgs { get; set; } = [];
    }
}
