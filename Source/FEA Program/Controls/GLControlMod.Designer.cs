using OpenTK.GLControl;
using OpenTK.Windowing.Common;

namespace FEA_Program.Controls
{
    partial class GLControlMod
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            glControl1 = new GLControl();
            SuspendLayout();
            // 
            // glControl1
            // 
            glControl1.API = ContextAPI.OpenGL;
            glControl1.APIVersion = new Version(3, 3, 0, 0);
            glControl1.Flags = ContextFlags.Default;
            glControl1.IsEventDriven = true;
            glControl1.Name = "glControl1";
            glControl1.Profile = ContextProfile.Core;
            glControl1.SharedContext = null;
            glControl1.Size = new Size(0, 0);
            glControl1.TabIndex = 0;
            ResumeLayout(false);
        }

        #endregion

        private GLControl glControl1;
    }
}
