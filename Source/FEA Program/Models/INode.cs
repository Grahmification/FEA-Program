namespace FEA_Program.Models
{
    internal interface INode
    {
        public event EventHandler<int>? SolutionInvalidated;

        public int ID { get; }
        public int Dimension { get; }
        public double[] Coordinates { get; set; }
        public double[] Displacement { get; }
    }
}
