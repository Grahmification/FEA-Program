namespace FEA_Program.UserControls
{
    partial class ProblemTreeControl
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
            components = new System.ComponentModel.Container();
            TreeNode treeNode1 = new TreeNode("Nodes");
            TreeNode treeNode2 = new TreeNode("Elements");
            TreeNode treeNode3 = new TreeNode("Forces");
            TreeNode treeNode4 = new TreeNode("Materials");
            treeView_main = new TreeView();
            contextMenuStrip_Edit = new ContextMenuStrip(components);
            toolStripMenuItem_nodeEdit = new ToolStripMenuItem();
            toolStripMenuItem_NodeDelete = new ToolStripMenuItem();
            contextMenuStrip_Edit.SuspendLayout();
            SuspendLayout();
            // 
            // treeView_main
            // 
            treeView_main.BackColor = Color.LightGray;
            treeView_main.BorderStyle = BorderStyle.None;
            treeView_main.Dock = DockStyle.Fill;
            treeView_main.FullRowSelect = true;
            treeView_main.Location = new Point(0, 0);
            treeView_main.Name = "treeView_main";
            treeNode1.Name = "Node0";
            treeNode1.Text = "Nodes";
            treeNode2.Name = "Node1";
            treeNode2.Text = "Elements";
            treeNode3.Name = "Node2";
            treeNode3.Text = "Forces";
            treeNode4.Name = "Node3";
            treeNode4.Text = "Materials";
            treeView_main.Nodes.AddRange(new TreeNode[] { treeNode1, treeNode2, treeNode3, treeNode4 });
            treeView_main.Size = new Size(212, 415);
            treeView_main.TabIndex = 0;
            treeView_main.NodeMouseClick += TreeView_Main_NodeMouseClick;
            // 
            // contextMenuStrip_Edit
            // 
            contextMenuStrip_Edit.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_nodeEdit, toolStripMenuItem_NodeDelete });
            contextMenuStrip_Edit.Name = "contextMenuStrip_Nodes";
            contextMenuStrip_Edit.Size = new Size(108, 48);
            // 
            // toolStripMenuItem_nodeEdit
            // 
            toolStripMenuItem_nodeEdit.Name = "toolStripMenuItem_nodeEdit";
            toolStripMenuItem_nodeEdit.Size = new Size(107, 22);
            toolStripMenuItem_nodeEdit.Text = "Edit";
            // 
            // toolStripMenuItem_NodeDelete
            // 
            toolStripMenuItem_NodeDelete.Name = "toolStripMenuItem_NodeDelete";
            toolStripMenuItem_NodeDelete.Size = new Size(107, 22);
            toolStripMenuItem_NodeDelete.Text = "Delete";
            toolStripMenuItem_NodeDelete.Click += toolStripMenuItem_Delete_Click;
            // 
            // ProblemTreeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(treeView_main);
            Name = "ProblemTreeControl";
            Size = new Size(212, 415);
            contextMenuStrip_Edit.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeView_main;
        private ContextMenuStrip contextMenuStrip_Edit;
        private ToolStripMenuItem toolStripMenuItem_nodeEdit;
        private ToolStripMenuItem toolStripMenuItem_NodeDelete;
    }
}
