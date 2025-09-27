namespace FEA_Program
{
    partial class Mainform
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            TreeNode treeNode5 = new TreeNode("Nodes");
            TreeNode treeNode6 = new TreeNode("Elements");
            TreeNode treeNode7 = new TreeNode("Forces");
            TreeNode treeNode8 = new TreeNode("Materials");
            ToolStrip_Upper = new ToolStrip();
            ToolStripDropDownButton_File = new ToolStripDropDownButton();
            ToolStripButton_Addnode = new ToolStripButton();
            ToolStripComboBox_ProblemMode = new ToolStripComboBox();
            ToolStripButton_AddMaterial = new ToolStripButton();
            ToolStripButton_AddElement = new ToolStripButton();
            ToolStripButton_AddNodeForce = new ToolStripButton();
            ToolStripButton1 = new ToolStripButton();
            StatusStrip_Lower = new StatusStrip();
            ToolStripStatusLabel_Trans = new ToolStripStatusLabel();
            ToolStripStatusLabel_Rot = new ToolStripStatusLabel();
            ToolStripStatusLabel_Zoom = new ToolStripStatusLabel();
            SplitContainer_Main = new SplitContainer();
            TabControl_Main = new TabControl();
            TabPage1 = new TabPage();
            TreeView_Main = new TreeView();
            TabPage2 = new TabPage();
            ContextMenuStrip_TreeView = new ContextMenuStrip(components);
            ToolStripMenuItem1 = new ToolStripMenuItem();
            ToolStripMenuItem2 = new ToolStripMenuItem();
            glControl_main = new OpenTK.GLControl.GLControl();
            ToolStrip_Upper.SuspendLayout();
            StatusStrip_Lower.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SplitContainer_Main).BeginInit();
            SplitContainer_Main.Panel1.SuspendLayout();
            SplitContainer_Main.Panel2.SuspendLayout();
            SplitContainer_Main.SuspendLayout();
            TabControl_Main.SuspendLayout();
            TabPage1.SuspendLayout();
            ContextMenuStrip_TreeView.SuspendLayout();
            SuspendLayout();
            // 
            // ToolStrip_Upper
            // 
            ToolStrip_Upper.Items.AddRange(new ToolStripItem[] { ToolStripDropDownButton_File, ToolStripButton_Addnode, ToolStripComboBox_ProblemMode, ToolStripButton_AddMaterial, ToolStripButton_AddElement, ToolStripButton_AddNodeForce, ToolStripButton1 });
            ToolStrip_Upper.Location = new Point(0, 0);
            ToolStrip_Upper.Name = "ToolStrip_Upper";
            ToolStrip_Upper.Size = new Size(968, 25);
            ToolStrip_Upper.TabIndex = 0;
            ToolStrip_Upper.Text = "ToolStrip1";
            // 
            // ToolStripDropDownButton_File
            // 
            ToolStripDropDownButton_File.ImageTransparentColor = Color.Magenta;
            ToolStripDropDownButton_File.Name = "ToolStripDropDownButton_File";
            ToolStripDropDownButton_File.Size = new Size(38, 22);
            ToolStripDropDownButton_File.Text = "File";
            // 
            // ToolStripButton_Addnode
            // 
            ToolStripButton_Addnode.ImageTransparentColor = Color.Magenta;
            ToolStripButton_Addnode.Name = "ToolStripButton_Addnode";
            ToolStripButton_Addnode.Size = new Size(65, 22);
            ToolStripButton_Addnode.Text = "Add Node";
            // 
            // ToolStripComboBox_ProblemMode
            // 
            ToolStripComboBox_ProblemMode.Alignment = ToolStripItemAlignment.Right;
            ToolStripComboBox_ProblemMode.DropDownStyle = ComboBoxStyle.DropDownList;
            ToolStripComboBox_ProblemMode.Name = "ToolStripComboBox_ProblemMode";
            ToolStripComboBox_ProblemMode.Size = new Size(140, 25);
            // 
            // ToolStripButton_AddMaterial
            // 
            ToolStripButton_AddMaterial.ImageTransparentColor = Color.Magenta;
            ToolStripButton_AddMaterial.Name = "ToolStripButton_AddMaterial";
            ToolStripButton_AddMaterial.Size = new Size(79, 22);
            ToolStripButton_AddMaterial.Text = "Add Material";
            // 
            // ToolStripButton_AddElement
            // 
            ToolStripButton_AddElement.ImageTransparentColor = Color.Magenta;
            ToolStripButton_AddElement.Name = "ToolStripButton_AddElement";
            ToolStripButton_AddElement.Size = new Size(79, 22);
            ToolStripButton_AddElement.Text = "Add Element";
            // 
            // ToolStripButton_AddNodeForce
            // 
            ToolStripButton_AddNodeForce.ImageTransparentColor = Color.Magenta;
            ToolStripButton_AddNodeForce.Name = "ToolStripButton_AddNodeForce";
            ToolStripButton_AddNodeForce.Size = new Size(65, 22);
            ToolStripButton_AddNodeForce.Text = "Add Force";
            // 
            // ToolStripButton1
            // 
            ToolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ToolStripButton1.ImageTransparentColor = Color.Magenta;
            ToolStripButton1.Name = "ToolStripButton1";
            ToolStripButton1.Size = new Size(23, 22);
            ToolStripButton1.Text = "ToolStripButton1";
            // 
            // StatusStrip_Lower
            // 
            StatusStrip_Lower.Items.AddRange(new ToolStripItem[] { ToolStripStatusLabel_Trans, ToolStripStatusLabel_Rot, ToolStripStatusLabel_Zoom });
            StatusStrip_Lower.Location = new Point(0, 436);
            StatusStrip_Lower.Name = "StatusStrip_Lower";
            StatusStrip_Lower.Padding = new Padding(1, 0, 16, 0);
            StatusStrip_Lower.Size = new Size(968, 22);
            StatusStrip_Lower.TabIndex = 1;
            StatusStrip_Lower.Text = "StatusStrip1";
            // 
            // ToolStripStatusLabel_Trans
            // 
            ToolStripStatusLabel_Trans.Name = "ToolStripStatusLabel_Trans";
            ToolStripStatusLabel_Trans.Size = new Size(120, 17);
            ToolStripStatusLabel_Trans.Text = "ToolStripStatusLabel1";
            // 
            // ToolStripStatusLabel_Rot
            // 
            ToolStripStatusLabel_Rot.Name = "ToolStripStatusLabel_Rot";
            ToolStripStatusLabel_Rot.Size = new Size(120, 17);
            ToolStripStatusLabel_Rot.Text = "ToolStripStatusLabel2";
            // 
            // ToolStripStatusLabel_Zoom
            // 
            ToolStripStatusLabel_Zoom.Name = "ToolStripStatusLabel_Zoom";
            ToolStripStatusLabel_Zoom.Size = new Size(120, 17);
            ToolStripStatusLabel_Zoom.Text = "ToolStripStatusLabel3";
            // 
            // SplitContainer_Main
            // 
            SplitContainer_Main.BorderStyle = BorderStyle.Fixed3D;
            SplitContainer_Main.Dock = DockStyle.Fill;
            SplitContainer_Main.Location = new Point(0, 25);
            SplitContainer_Main.Margin = new Padding(4, 3, 4, 3);
            SplitContainer_Main.Name = "SplitContainer_Main";
            // 
            // SplitContainer_Main.Panel1
            // 
            SplitContainer_Main.Panel1.Controls.Add(TabControl_Main);
            // 
            // SplitContainer_Main.Panel2
            // 
            SplitContainer_Main.Panel2.Controls.Add(glControl_main);
            SplitContainer_Main.Size = new Size(968, 411);
            SplitContainer_Main.SplitterDistance = 186;
            SplitContainer_Main.SplitterWidth = 5;
            SplitContainer_Main.TabIndex = 2;
            SplitContainer_Main.TabStop = false;
            // 
            // TabControl_Main
            // 
            TabControl_Main.Controls.Add(TabPage1);
            TabControl_Main.Controls.Add(TabPage2);
            TabControl_Main.Dock = DockStyle.Fill;
            TabControl_Main.Location = new Point(0, 0);
            TabControl_Main.Margin = new Padding(4, 3, 4, 3);
            TabControl_Main.Name = "TabControl_Main";
            TabControl_Main.SelectedIndex = 0;
            TabControl_Main.Size = new Size(182, 407);
            TabControl_Main.TabIndex = 0;
            // 
            // TabPage1
            // 
            TabPage1.Controls.Add(TreeView_Main);
            TabPage1.Location = new Point(4, 24);
            TabPage1.Margin = new Padding(0);
            TabPage1.Name = "TabPage1";
            TabPage1.Size = new Size(174, 379);
            TabPage1.TabIndex = 0;
            TabPage1.Text = "Problem";
            TabPage1.UseVisualStyleBackColor = true;
            // 
            // TreeView_Main
            // 
            TreeView_Main.BackColor = Color.LightGray;
            TreeView_Main.Dock = DockStyle.Fill;
            TreeView_Main.FullRowSelect = true;
            TreeView_Main.Location = new Point(0, 0);
            TreeView_Main.Margin = new Padding(4, 3, 4, 3);
            TreeView_Main.Name = "TreeView_Main";
            treeNode5.Name = "Nodes";
            treeNode5.Text = "Nodes";
            treeNode6.Name = "Elements";
            treeNode6.Text = "Elements";
            treeNode7.Name = "Forces";
            treeNode7.Text = "Forces";
            treeNode8.Name = "Materials";
            treeNode8.Text = "Materials";
            TreeView_Main.Nodes.AddRange(new TreeNode[] { treeNode5, treeNode6, treeNode7, treeNode8 });
            TreeView_Main.Size = new Size(174, 379);
            TreeView_Main.TabIndex = 1;
            // 
            // TabPage2
            // 
            TabPage2.Location = new Point(4, 24);
            TabPage2.Margin = new Padding(4, 3, 4, 3);
            TabPage2.Name = "TabPage2";
            TabPage2.Padding = new Padding(4, 3, 4, 3);
            TabPage2.Size = new Size(174, 379);
            TabPage2.TabIndex = 1;
            TabPage2.Text = "TabPage2";
            TabPage2.UseVisualStyleBackColor = true;
            // 
            // ContextMenuStrip_TreeView
            // 
            ContextMenuStrip_TreeView.Items.AddRange(new ToolStripItem[] { ToolStripMenuItem1, ToolStripMenuItem2 });
            ContextMenuStrip_TreeView.Name = "ContextMenuStrip_TreeView";
            ContextMenuStrip_TreeView.Size = new Size(108, 48);
            // 
            // ToolStripMenuItem1
            // 
            ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            ToolStripMenuItem1.Size = new Size(107, 22);
            ToolStripMenuItem1.Text = "Edit";
            // 
            // ToolStripMenuItem2
            // 
            ToolStripMenuItem2.Name = "ToolStripMenuItem2";
            ToolStripMenuItem2.Size = new Size(107, 22);
            ToolStripMenuItem2.Text = "Delete";
            // 
            // glControl_main
            // 
            glControl_main.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl_main.APIVersion = new Version(2, 1, 0, 0);
            glControl_main.Dock = DockStyle.Fill;
            glControl_main.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl_main.IsEventDriven = true;
            glControl_main.Location = new Point(0, 0);
            glControl_main.Name = "glControl_main";
            glControl_main.Profile = OpenTK.Windowing.Common.ContextProfile.Any;
            glControl_main.SharedContext = null;
            glControl_main.Size = new Size(773, 407);
            glControl_main.TabIndex = 0;
            // 
            // Mainform
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(968, 458);
            Controls.Add(SplitContainer_Main);
            Controls.Add(StatusStrip_Lower);
            Controls.Add(ToolStrip_Upper);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Mainform";
            Text = "FEA";
            ToolStrip_Upper.ResumeLayout(false);
            ToolStrip_Upper.PerformLayout();
            StatusStrip_Lower.ResumeLayout(false);
            StatusStrip_Lower.PerformLayout();
            SplitContainer_Main.Panel1.ResumeLayout(false);
            SplitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SplitContainer_Main).EndInit();
            SplitContainer_Main.ResumeLayout(false);
            TabControl_Main.ResumeLayout(false);
            TabPage1.ResumeLayout(false);
            ContextMenuStrip_TreeView.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        internal ToolStrip ToolStrip_Upper;
        internal StatusStrip StatusStrip_Lower;
        internal SplitContainer SplitContainer_Main;
        internal ToolStripStatusLabel ToolStripStatusLabel_Trans;
        internal ToolStripStatusLabel ToolStripStatusLabel_Rot;
        internal ToolStripStatusLabel ToolStripStatusLabel_Zoom;
        internal ToolStripDropDownButton ToolStripDropDownButton_File;
        internal TreeView TreeView_Main;
        internal TabControl TabControl_Main;
        internal TabPage TabPage1;
        internal TabPage TabPage2;
        internal ToolStripButton ToolStripButton_Addnode;
        internal ToolStripComboBox ToolStripComboBox_ProblemMode;
        internal ToolStripButton ToolStripButton_AddMaterial;
        internal ToolStripButton ToolStripButton_AddElement;
        internal ContextMenuStrip ContextMenuStrip_TreeView;
        internal ToolStripMenuItem ToolStripMenuItem1;
        internal ToolStripMenuItem ToolStripMenuItem2;
        internal ToolStripButton ToolStripButton_AddNodeForce;
        internal ToolStripButton ToolStripButton1;
        private OpenTK.GLControl.GLControl glControl_main;
    }
}
