using FEA_Program.Drawable;

namespace FEA_Program.Models
{
    /// <summary>
    /// Handles drawing the nodes in the 3D display
    /// </summary>
    internal class NodeDrawManager
    {
        private bool _DrawDisplaced = false;
        public double _DisplacePercentage = 0;
        public double _DisplaceScaling = 1;

        /// <summary>
        /// Node has changed such that screen needs to be redrawn
        /// </summary>
        public event EventHandler? RedrawRequired;


        public List<NodeDrawable> Nodes { get; set; } = [];
        public bool DrawDisplaced { get => _DrawDisplaced; set { _DrawDisplaced = value; UpdateScaling(); } }
        public double DisplacePercentage { get => _DisplacePercentage; set { _DisplacePercentage = value; UpdateScaling(); } }
        public double DisplaceScaling { get => _DisplaceScaling; set { _DisplaceScaling = value; UpdateScaling(); } }


        public void DrawNodes()
        {
            foreach (NodeDrawable N in Nodes)
                N.Draw();
        }

        /// <summary>
        /// Update internal value of displace scaling on all the nodes
        /// </summary>
        private void UpdateScaling()
        {
            double nodeDrawScaling = 0;

            if (DrawDisplaced)
            {
                nodeDrawScaling = (DisplacePercentage / 100.0) * DisplaceScaling;
            }

            foreach (NodeDrawable N in Nodes)
                N.DisplacementScalingFactor = nodeDrawScaling;

            RedrawRequired?.Invoke(this, new());
        }
    }
}
