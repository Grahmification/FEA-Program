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
            TreeNode treeNode5 = new TreeNode("Nodes");
            TreeNode treeNode6 = new TreeNode("Elements");
            TreeNode treeNode7 = new TreeNode("Forces");
            TreeNode treeNode8 = new TreeNode("Materials");
            treeView_main = new TreeView();
            contextMenuStrip_Nodes = new ContextMenuStrip(components);
            contextMenuStrip_Elements = new ContextMenuStrip(components);
            contextMenuStrip_Forces = new ContextMenuStrip(components);
            contextMenuStrip_Materials = new ContextMenuStrip(components);
            toolStripMenuItem_nodeEdit = new ToolStripMenuItem();
            toolStripMenuItem_NodeDelete = new ToolStripMenuItem();
            contextMenuStrip_Nodes.SuspendLayout();
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
            treeNode5.Name = "Node0";
            treeNode5.Text = "Nodes";
            treeNode6.Name = "Node1";
            treeNode6.Text = "Elements";
            treeNode7.Name = "Node2";
            treeNode7.Text = "Forces";
            treeNode8.Name = "Node3";
            treeNode8.Text = "Materials";
            treeView_main.Nodes.AddRange(new TreeNode[] { treeNode5, treeNode6, treeNode7, treeNode8 });
            treeView_main.Size = new Size(212, 415);
            treeView_main.TabIndex = 0;
            treeView_main.NodeMouseClick += TreeView_Main_NodeMouseClick;
            // 
            // contextMenuStrip_Nodes
            // 
            contextMenuStrip_Nodes.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_nodeEdit, toolStripMenuItem_NodeDelete });
            contextMenuStrip_Nodes.Name = "contextMenuStrip_Nodes";
            contextMenuStrip_Nodes.Size = new Size(181, 70);
            // 
            // contextMenuStrip_Elements
            // 
            contextMenuStrip_Elements.Name = "contextMenuStrip_Elements";
            contextMenuStrip_Elements.Size = new Size(61, 4);
            // 
            // contextMenuStrip_Forces
            // 
            contextMenuStrip_Forces.Name = "contextMenuStrip_Forces";
            contextMenuStrip_Forces.Size = new Size(61, 4);
            // 
            // contextMenuStrip_Materials
            // 
            contextMenuStrip_Materials.Name = "contextMenuStrip_Materials";
            contextMenuStrip_Materials.Size = new Size(61, 4);
            // 
            // toolStripMenuItem_nodeEdit
            // 
            toolStripMenuItem_nodeEdit.Name = "toolStripMenuItem_nodeEdit";
            toolStripMenuItem_nodeEdit.Size = new Size(180, 22);
            toolStripMenuItem_nodeEdit.Text = "Edit";
            // 
            // toolStripMenuItem_NodeDelete
            // 
            toolStripMenuItem_NodeDelete.Name = "toolStripMenuItem_NodeDelete";
            toolStripMenuItem_NodeDelete.Size = new Size(180, 22);
            toolStripMenuItem_NodeDelete.Text = "Delete";
            // 
            // ProblemTreeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(treeView_main);
            Name = "ProblemTreeControl";
            Size = new Size(212, 415);
            contextMenuStrip_Nodes.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeView_main;
        private ContextMenuStrip contextMenuStrip_Nodes;
        private ToolStripMenuItem toolStripMenuItem_nodeEdit;
        private ToolStripMenuItem toolStripMenuItem_NodeDelete;
        private ContextMenuStrip contextMenuStrip_Elements;
        private ContextMenuStrip contextMenuStrip_Forces;
        private ContextMenuStrip contextMenuStrip_Materials;
    }
}
