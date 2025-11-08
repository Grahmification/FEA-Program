using FEA_Program.Drawable;

namespace FEA_Program.UserControls
{
    /// <summary>
    /// Generic definition a view that displays editors
    /// </summary>
    internal interface IEditorDisplayView
    {
        public event EventHandler? NodeAddRequest;

        public INodeEditView ShowNodeEditView(NodeDrawable node, bool edit);
    }
}
