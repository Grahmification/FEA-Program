using FEA_Program.Drawable;

namespace FEA_Program.UserControls
{
    /// <summary>
    /// Generic definition for the main program view
    /// </summary>
    internal interface IMainView
    {
        public event EventHandler? NodeAddRequest;

        public event EventHandler<int>? NodeEditRequest;

        public INodeEditView ShowNodeEditView(NodeDrawable node, bool edit);
    }
}
