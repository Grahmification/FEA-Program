using FEA_Program.Drawable;

namespace FEA_Program.UserControls
{
    /// <summary>
    /// Generic definition for a form which can control adding or editing nodes
    /// </summary>
    internal interface INodeEditView
    {

        public event EventHandler? NodeAddRequest;
        public event EventHandler<(NodeDrawable, bool)>? NodeEditConfirmed;

        public void ShowNodeEditView(NodeDrawable node, bool edit);
    }
}
