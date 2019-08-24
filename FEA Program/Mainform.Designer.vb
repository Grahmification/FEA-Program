<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Mainform
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Mainform))
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Nodes")
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Elements")
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Forces")
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Materials")
        Me.ToolStrip_Upper = New System.Windows.Forms.ToolStrip()
        Me.ToolStripDropDownButton_File = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ToolStripButton_Addnode = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripComboBox_ProblemMode = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripButton_AddMaterial = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton_AddElement = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton_AddNodeForce = New System.Windows.Forms.ToolStripButton()
        Me.StatusStrip_Lower = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel_Trans = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel_Rot = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel_Zoom = New System.Windows.Forms.ToolStripStatusLabel()
        Me.SplitContainer_Main = New System.Windows.Forms.SplitContainer()
        Me.TabControl_Main = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TreeView_Main = New System.Windows.Forms.TreeView()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.ContextMenuStrip_TreeView = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStrip_Upper.SuspendLayout()
        Me.StatusStrip_Lower.SuspendLayout()
        CType(Me.SplitContainer_Main, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer_Main.Panel1.SuspendLayout()
        Me.SplitContainer_Main.SuspendLayout()
        Me.TabControl_Main.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.ContextMenuStrip_TreeView.SuspendLayout()
        Me.SuspendLayout()
        '
        'ToolStrip_Upper
        '
        Me.ToolStrip_Upper.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripDropDownButton_File, Me.ToolStripButton_Addnode, Me.ToolStripComboBox_ProblemMode, Me.ToolStripButton_AddMaterial, Me.ToolStripButton_AddElement, Me.ToolStripButton_AddNodeForce, Me.ToolStripButton1})
        Me.ToolStrip_Upper.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip_Upper.Name = "ToolStrip_Upper"
        Me.ToolStrip_Upper.Size = New System.Drawing.Size(830, 25)
        Me.ToolStrip_Upper.TabIndex = 0
        Me.ToolStrip_Upper.Text = "ToolStrip1"
        '
        'ToolStripDropDownButton_File
        '
        Me.ToolStripDropDownButton_File.Image = CType(resources.GetObject("ToolStripDropDownButton_File.Image"), System.Drawing.Image)
        Me.ToolStripDropDownButton_File.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton_File.Name = "ToolStripDropDownButton_File"
        Me.ToolStripDropDownButton_File.Size = New System.Drawing.Size(54, 22)
        Me.ToolStripDropDownButton_File.Text = "File"
        '
        'ToolStripButton_Addnode
        '
        Me.ToolStripButton_Addnode.Image = CType(resources.GetObject("ToolStripButton_Addnode.Image"), System.Drawing.Image)
        Me.ToolStripButton_Addnode.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton_Addnode.Name = "ToolStripButton_Addnode"
        Me.ToolStripButton_Addnode.Size = New System.Drawing.Size(81, 22)
        Me.ToolStripButton_Addnode.Text = "Add Node"
        '
        'ToolStripComboBox_ProblemMode
        '
        Me.ToolStripComboBox_ProblemMode.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripComboBox_ProblemMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ToolStripComboBox_ProblemMode.Name = "ToolStripComboBox_ProblemMode"
        Me.ToolStripComboBox_ProblemMode.Size = New System.Drawing.Size(121, 25)
        '
        'ToolStripButton_AddMaterial
        '
        Me.ToolStripButton_AddMaterial.Image = CType(resources.GetObject("ToolStripButton_AddMaterial.Image"), System.Drawing.Image)
        Me.ToolStripButton_AddMaterial.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton_AddMaterial.Name = "ToolStripButton_AddMaterial"
        Me.ToolStripButton_AddMaterial.Size = New System.Drawing.Size(95, 22)
        Me.ToolStripButton_AddMaterial.Text = "Add Material"
        '
        'ToolStripButton_AddElement
        '
        Me.ToolStripButton_AddElement.Image = CType(resources.GetObject("ToolStripButton_AddElement.Image"), System.Drawing.Image)
        Me.ToolStripButton_AddElement.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton_AddElement.Name = "ToolStripButton_AddElement"
        Me.ToolStripButton_AddElement.Size = New System.Drawing.Size(95, 22)
        Me.ToolStripButton_AddElement.Text = "Add Element"
        '
        'ToolStripButton_AddNodeForce
        '
        Me.ToolStripButton_AddNodeForce.Image = CType(resources.GetObject("ToolStripButton_AddNodeForce.Image"), System.Drawing.Image)
        Me.ToolStripButton_AddNodeForce.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton_AddNodeForce.Name = "ToolStripButton_AddNodeForce"
        Me.ToolStripButton_AddNodeForce.Size = New System.Drawing.Size(81, 22)
        Me.ToolStripButton_AddNodeForce.Text = "Add Force"
        '
        'StatusStrip_Lower
        '
        Me.StatusStrip_Lower.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel_Trans, Me.ToolStripStatusLabel_Rot, Me.ToolStripStatusLabel_Zoom})
        Me.StatusStrip_Lower.Location = New System.Drawing.Point(0, 375)
        Me.StatusStrip_Lower.Name = "StatusStrip_Lower"
        Me.StatusStrip_Lower.Size = New System.Drawing.Size(830, 22)
        Me.StatusStrip_Lower.TabIndex = 1
        Me.StatusStrip_Lower.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel_Trans
        '
        Me.ToolStripStatusLabel_Trans.Name = "ToolStripStatusLabel_Trans"
        Me.ToolStripStatusLabel_Trans.Size = New System.Drawing.Size(121, 17)
        Me.ToolStripStatusLabel_Trans.Text = "ToolStripStatusLabel1"
        '
        'ToolStripStatusLabel_Rot
        '
        Me.ToolStripStatusLabel_Rot.Name = "ToolStripStatusLabel_Rot"
        Me.ToolStripStatusLabel_Rot.Size = New System.Drawing.Size(121, 17)
        Me.ToolStripStatusLabel_Rot.Text = "ToolStripStatusLabel2"
        '
        'ToolStripStatusLabel_Zoom
        '
        Me.ToolStripStatusLabel_Zoom.Name = "ToolStripStatusLabel_Zoom"
        Me.ToolStripStatusLabel_Zoom.Size = New System.Drawing.Size(121, 17)
        Me.ToolStripStatusLabel_Zoom.Text = "ToolStripStatusLabel3"
        '
        'SplitContainer_Main
        '
        Me.SplitContainer_Main.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer_Main.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer_Main.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer_Main.Name = "SplitContainer_Main"
        '
        'SplitContainer_Main.Panel1
        '
        Me.SplitContainer_Main.Panel1.Controls.Add(Me.TabControl_Main)
        Me.SplitContainer_Main.Size = New System.Drawing.Size(830, 350)
        Me.SplitContainer_Main.SplitterDistance = 160
        Me.SplitContainer_Main.TabIndex = 2
        Me.SplitContainer_Main.TabStop = False
        '
        'TabControl_Main
        '
        Me.TabControl_Main.Controls.Add(Me.TabPage1)
        Me.TabControl_Main.Controls.Add(Me.TabPage2)
        Me.TabControl_Main.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl_Main.Location = New System.Drawing.Point(0, 0)
        Me.TabControl_Main.Name = "TabControl_Main"
        Me.TabControl_Main.SelectedIndex = 0
        Me.TabControl_Main.Size = New System.Drawing.Size(156, 346)
        Me.TabControl_Main.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.TreeView_Main)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Size = New System.Drawing.Size(148, 320)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Problem"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TreeView_Main
        '
        Me.TreeView_Main.BackColor = System.Drawing.Color.LightGray
        Me.TreeView_Main.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeView_Main.FullRowSelect = True
        Me.TreeView_Main.Location = New System.Drawing.Point(0, 0)
        Me.TreeView_Main.Name = "TreeView_Main"
        TreeNode1.Name = "Nodes"
        TreeNode1.Text = "Nodes"
        TreeNode2.Name = "Elements"
        TreeNode2.Text = "Elements"
        TreeNode3.Name = "Forces"
        TreeNode3.Text = "Forces"
        TreeNode4.Name = "Materials"
        TreeNode4.Text = "Materials"
        Me.TreeView_Main.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode2, TreeNode3, TreeNode4})
        Me.TreeView_Main.Size = New System.Drawing.Size(148, 320)
        Me.TreeView_Main.TabIndex = 1
        '
        'TabPage2
        '
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(148, 320)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'ContextMenuStrip_TreeView
        '
        Me.ContextMenuStrip_TreeView.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.ToolStripMenuItem2})
        Me.ContextMenuStrip_TreeView.Name = "ContextMenuStrip_TreeView"
        Me.ContextMenuStrip_TreeView.Size = New System.Drawing.Size(108, 48)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(107, 22)
        Me.ToolStripMenuItem1.Text = "Edit"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(107, 22)
        Me.ToolStripMenuItem2.Text = "Delete"
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "ToolStripButton1"
        '
        'Mainform
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(830, 397)
        Me.Controls.Add(Me.SplitContainer_Main)
        Me.Controls.Add(Me.StatusStrip_Lower)
        Me.Controls.Add(Me.ToolStrip_Upper)
        Me.Name = "Mainform"
        Me.Text = "FEA"
        Me.ToolStrip_Upper.ResumeLayout(False)
        Me.ToolStrip_Upper.PerformLayout()
        Me.StatusStrip_Lower.ResumeLayout(False)
        Me.StatusStrip_Lower.PerformLayout()
        Me.SplitContainer_Main.Panel1.ResumeLayout(False)
        CType(Me.SplitContainer_Main, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer_Main.ResumeLayout(False)
        Me.TabControl_Main.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.ContextMenuStrip_TreeView.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolStrip_Upper As System.Windows.Forms.ToolStrip
    Friend WithEvents StatusStrip_Lower As System.Windows.Forms.StatusStrip
    Friend WithEvents SplitContainer_Main As System.Windows.Forms.SplitContainer
    Friend WithEvents ToolStripStatusLabel_Trans As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel_Rot As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel_Zoom As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripDropDownButton_File As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents TreeView_Main As System.Windows.Forms.TreeView
    Friend WithEvents TabControl_Main As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents ToolStripButton_Addnode As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripComboBox_ProblemMode As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents ToolStripButton_AddMaterial As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton_AddElement As System.Windows.Forms.ToolStripButton
    Friend WithEvents ContextMenuStrip_TreeView As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripButton_AddNodeForce As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton1 As System.Windows.Forms.ToolStripButton

End Class
