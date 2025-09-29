namespace FEA_Program.Models
{
    internal interface INode
    {
        public int ID { get; }
        public int Dimension { get; }
        public double[] Coords { get; set; }
    }
}
