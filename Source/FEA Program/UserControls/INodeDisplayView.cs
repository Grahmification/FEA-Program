using FEA_Program.Drawable;

namespace FEA_Program.UserControls
{
    /// <summary>
    /// Generic definition for a form which displays nodes
    /// </summary>
    internal interface INodeDisplayView
    {
        public event EventHandler? NodeAddRequest;
        public event EventHandler<int>? NodeEditRequest;
        public event EventHandler<int>? NodeDeleteRequest;

        public void DisplayNodes(List<NodeDrawable> nodes);
    }
}
