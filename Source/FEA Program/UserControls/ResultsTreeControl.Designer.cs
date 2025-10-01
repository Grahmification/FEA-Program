namespace FEA_Program.UserControls
{
    partial class ResultsTreeControl
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
            TreeNode treeNode1 = new TreeNode("Nodes");
            TreeNode treeNode2 = new TreeNode("Elements");
            treeView_main = new TreeView();
            SuspendLayout();
            // 
            // treeView_main
            // 
            treeView_main.BackColor = Color.LightGray;
            treeView_main.BorderStyle = BorderStyle.None;
            treeView_main.Dock = DockStyle.Fill;
            treeView_main.Location = new Point(0, 0);
            treeView_main.Name = "treeView_main";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Nodes";
            treeNode2.Name = "Node1";
            treeNode2.Text = "Elements";
            treeView_main.Nodes.AddRange(new TreeNode[] { treeNode1, treeNode2 });
            treeView_main.Size = new Size(212, 415);
            treeView_main.TabIndex = 0;
            // 
            // ResultsTreeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(treeView_main);
            Name = "ResultsTreeControl";
            Size = new Size(212, 415);
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeView_main;
    }
}
