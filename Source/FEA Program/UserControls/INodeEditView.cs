using FEA_Program.Drawable;

namespace FEA_Program.UserControls
{
    /// <summary>
    /// Generic definition for a form which can control adding or editing nodes
    /// </summary>
    internal interface INodeEditView
    {
        public event EventHandler<NodeDrawable>? NodeEditConfirmed;

        public bool Editing { get; }
    }
}
