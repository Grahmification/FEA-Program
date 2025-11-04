namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Generic definition for a viewmodel which can display a control on the sidebar
    /// </summary>
    internal interface ISideBarEditor
    {
        /// <summary>
        /// Fires when the sidebar control is opening
        /// </summary>
        public event EventHandler Opening;

        /// <summary>
        /// Fires when the sidebar control has closed
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Cancels editing on the sidebar control, hiding it
        /// </summary>
        public void CancelEdit();
    }
    
    /// <summary>
    /// Viewmodel managing different editors on the sidebar
    /// </summary>
    internal class SidebarVM
    {
        private readonly List<ISideBarEditor> _editors = [];

        /// <summary>
        /// Returns true if an editor is open
        /// </summary>
        public bool EditorIsOpen => ActiveEditor != null;

        /// <summary>
        /// The currently open editor on the sidebar
        /// </summary>
        public ISideBarEditor? ActiveEditor { get; private set; } = null;

        /// <summary>
        /// Adds an editor so it can be managed by the viewmodel
        /// </summary>
        /// <param name="editor">The editor to manage</param>
        public void AddEditor(ISideBarEditor editor)
        {
            _editors.Add(editor);
            editor.Opening += OnEditorOpening;
            editor.Closed += OnEditorClosed;
        }

        /// <summary>
        /// Resets the VM, clearing all editors from the list
        /// </summary>
        public void Reset()
        {
            foreach (var editor in _editors)
            {
                editor.Opening -= OnEditorOpening;
                editor.Closed -= OnEditorClosed;
            }

            _editors.Clear();
            ActiveEditor = null;
        }

        // ---------------------- Event Handlers ----------------------
        private void OnEditorOpening(object? sender, EventArgs e)
        {
            if(sender is ISideBarEditor editor)
            {
                // Hide the currently displayed editor prior to showing the other one
                ActiveEditor?.CancelEdit();
                ActiveEditor = editor;
            }
        }
        private void OnEditorClosed(object? sender, EventArgs e)
        {
            ActiveEditor = null;
        }

    }
}
