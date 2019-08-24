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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Mainform))
        Me.StatusStrip_MainBottom = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel3 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStrip_MainTop = New System.Windows.Forms.ToolStrip()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ToolStripMenuItem_save = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem_load = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripButton_Solve = New System.Windows.Forms.ToolStripButton()
        Me.SplitContainer_MainHoriz = New System.Windows.Forms.SplitContainer()
        Me.GlControl_Main = New OpenTK.GLControl()
        Me.TabControl_Bottom = New System.Windows.Forms.TabControl()
        Me.TabPage_Nodes = New System.Windows.Forms.TabPage()
        Me.ListView_Nodes = New System.Windows.Forms.ListView()
        Me.ID = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Coords = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Util = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Fixity = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Displacement = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Reactions = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ToolStrip_Nodes = New System.Windows.Forms.ToolStrip()
        Me.TSButton_AddNode = New System.Windows.Forms.ToolStripButton()
        Me.TSButton_EditNode = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton_deleteNode = New System.Windows.Forms.ToolStripButton()
        Me.TabPage_Elements = New System.Windows.Forms.TabPage()
        Me.ListView_Elements = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ToolStrip_Elements = New System.Windows.Forms.ToolStrip()
        Me.ToolStripButton_AddElem = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton_DeleteElem = New System.Windows.Forms.ToolStripButton()
        Me.TabPage_Forces = New System.Windows.Forms.TabPage()
        Me.ListView_Forces = New System.Windows.Forms.ListView()
        Me.Node = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Magnitue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Direction = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Component = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ToolStrip_Force = New System.Windows.Forms.ToolStrip()
        Me.ToolStripButton_AddForce = New System.Windows.Forms.ToolStripButton()
        Me.Toolstripbutton_deleteforce = New System.Windows.Forms.ToolStripButton()
        Me.CheckBox_DrawReactions = New System.Windows.Forms.CheckBox()
        Me.TrackBar_AnimateDisplacement = New System.Windows.Forms.TrackBar()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip_MainBottom.SuspendLayout()
        Me.ToolStrip_MainTop.SuspendLayout()
        CType(Me.SplitContainer_MainHoriz, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer_MainHoriz.Panel1.SuspendLayout()
        Me.SplitContainer_MainHoriz.Panel2.SuspendLayout()
        Me.SplitContainer_MainHoriz.SuspendLayout()
        Me.TabControl_Bottom.SuspendLayout()
        Me.TabPage_Nodes.SuspendLayout()
        Me.ToolStrip_Nodes.SuspendLayout()
        Me.TabPage_Elements.SuspendLayout()
        Me.ToolStrip_Elements.SuspendLayout()
        Me.TabPage_Forces.SuspendLayout()
        Me.ToolStrip_Force.SuspendLayout()
        CType(Me.TrackBar_AnimateDisplacement, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip_MainBottom
        '
        Me.StatusStrip_MainBottom.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripStatusLabel2, Me.ToolStripStatusLabel3})
        Me.StatusStrip_MainBottom.Location = New System.Drawing.Point(0, 681)
        Me.StatusStrip_MainBottom.Name = "StatusStrip_MainBottom"
        Me.StatusStrip_MainBottom.Size = New System.Drawing.Size(789, 22)
        Me.StatusStrip_MainBottom.TabIndex = 0
        Me.StatusStrip_MainBottom.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(121, 17)
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(121, 17)
        Me.ToolStripStatusLabel2.Text = "ToolStripStatusLabel2"
        '
        'ToolStripStatusLabel3
        '
        Me.ToolStripStatusLabel3.Name = "ToolStripStatusLabel3"
        Me.ToolStripStatusLabel3.Size = New System.Drawing.Size(121, 17)
        Me.ToolStripStatusLabel3.Text = "ToolStripStatusLabel3"
        '
        'ToolStrip_MainTop
        '
        Me.ToolStrip_MainTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripDropDownButton1, Me.ToolStripButton_Solve})
        Me.ToolStrip_MainTop.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip_MainTop.Name = "ToolStrip_MainTop"
        Me.ToolStrip_MainTop.Size = New System.Drawing.Size(789, 25)
        Me.ToolStrip_MainTop.TabIndex = 1
        Me.ToolStrip_MainTop.Text = "ToolStrip1"
        '
        'ToolStripDropDownButton1
        '
        Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem_save, Me.ToolStripMenuItem_load, Me.ToolStripMenuItem1})
        Me.ToolStripDropDownButton1.Image = CType(resources.GetObject("ToolStripDropDownButton1.Image"), System.Drawing.Image)
        Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(54, 22)
        Me.ToolStripDropDownButton1.Text = "File"
        '
        'ToolStripMenuItem_save
        '
        Me.ToolStripMenuItem_save.Name = "ToolStripMenuItem_save"
        Me.ToolStripMenuItem_save.Size = New System.Drawing.Size(183, 22)
        Me.ToolStripMenuItem_save.Text = "Save to File"
        '
        'ToolStripMenuItem_load
        '
        Me.ToolStripMenuItem_load.Name = "ToolStripMenuItem_load"
        Me.ToolStripMenuItem_load.Size = New System.Drawing.Size(183, 22)
        Me.ToolStripMenuItem_load.Text = "Load From File"
        '
        'ToolStripButton_Solve
        '
        Me.ToolStripButton_Solve.Image = CType(resources.GetObject("ToolStripButton_Solve.Image"), System.Drawing.Image)
        Me.ToolStripButton_Solve.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton_Solve.Name = "ToolStripButton_Solve"
        Me.ToolStripButton_Solve.Size = New System.Drawing.Size(55, 22)
        Me.ToolStripButton_Solve.Text = "Solve"
        '
        'SplitContainer_MainHoriz
        '
        Me.SplitContainer_MainHoriz.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.SplitContainer_MainHoriz.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer_MainHoriz.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer_MainHoriz.Name = "SplitContainer_MainHoriz"
        Me.SplitContainer_MainHoriz.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer_MainHoriz.Panel1
        '
        Me.SplitContainer_MainHoriz.Panel1.Controls.Add(Me.GlControl_Main)
        '
        'SplitContainer_MainHoriz.Panel2
        '
        Me.SplitContainer_MainHoriz.Panel2.Controls.Add(Me.TabControl_Bottom)
        Me.SplitContainer_MainHoriz.Size = New System.Drawing.Size(789, 656)
        Me.SplitContainer_MainHoriz.SplitterDistance = 529
        Me.SplitContainer_MainHoriz.TabIndex = 2
        '
        'GlControl_Main
        '
        Me.GlControl_Main.AutoSize = True
        Me.GlControl_Main.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.GlControl_Main.BackColor = System.Drawing.Color.Black
        Me.GlControl_Main.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GlControl_Main.Location = New System.Drawing.Point(0, 0)
        Me.GlControl_Main.Name = "GlControl_Main"
        Me.GlControl_Main.Size = New System.Drawing.Size(787, 527)
        Me.GlControl_Main.TabIndex = 1
        Me.GlControl_Main.VSync = True
        '
        'TabControl_Bottom
        '
        Me.TabControl_Bottom.Controls.Add(Me.TabPage_Nodes)
        Me.TabControl_Bottom.Controls.Add(Me.TabPage_Elements)
        Me.TabControl_Bottom.Controls.Add(Me.TabPage_Forces)
        Me.TabControl_Bottom.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl_Bottom.Location = New System.Drawing.Point(0, 0)
        Me.TabControl_Bottom.Name = "TabControl_Bottom"
        Me.TabControl_Bottom.SelectedIndex = 0
        Me.TabControl_Bottom.Size = New System.Drawing.Size(787, 121)
        Me.TabControl_Bottom.TabIndex = 0
        '
        'TabPage_Nodes
        '
        Me.TabPage_Nodes.Controls.Add(Me.ListView_Nodes)
        Me.TabPage_Nodes.Controls.Add(Me.ToolStrip_Nodes)
        Me.TabPage_Nodes.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_Nodes.Name = "TabPage_Nodes"
        Me.TabPage_Nodes.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage_Nodes.Size = New System.Drawing.Size(779, 95)
        Me.TabPage_Nodes.TabIndex = 0
        Me.TabPage_Nodes.Text = "Nodes"
        Me.TabPage_Nodes.UseVisualStyleBackColor = True
        '
        'ListView_Nodes
        '
        Me.ListView_Nodes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ID, Me.Coords, Me.Util, Me.Fixity, Me.Displacement, Me.Reactions})
        Me.ListView_Nodes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView_Nodes.FullRowSelect = True
        Me.ListView_Nodes.GridLines = True
        Me.ListView_Nodes.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.ListView_Nodes.Location = New System.Drawing.Point(3, 28)
        Me.ListView_Nodes.Name = "ListView_Nodes"
        Me.ListView_Nodes.Size = New System.Drawing.Size(773, 64)
        Me.ListView_Nodes.TabIndex = 1
        Me.ListView_Nodes.UseCompatibleStateImageBehavior = False
        Me.ListView_Nodes.View = System.Windows.Forms.View.Details
        '
        'ID
        '
        Me.ID.Text = "Node ID"
        '
        'Coords
        '
        Me.Coords.Text = "Coordinates (mm)"
        Me.Coords.Width = 150
        '
        'Util
        '
        Me.Util.Text = "Linked Elements"
        Me.Util.Width = 100
        '
        'Fixity
        '
        Me.Fixity.Text = "Fixed Displacements"
        Me.Fixity.Width = 120
        '
        'Displacement
        '
        Me.Displacement.Text = "Displacements (mm)"
        Me.Displacement.Width = 150
        '
        'Reactions
        '
        Me.Reactions.Text = "Reaction Forces (N)"
        Me.Reactions.Width = 120
        '
        'ToolStrip_Nodes
        '
        Me.ToolStrip_Nodes.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSButton_AddNode, Me.TSButton_EditNode, Me.ToolStripButton_deleteNode})
        Me.ToolStrip_Nodes.Location = New System.Drawing.Point(3, 3)
        Me.ToolStrip_Nodes.Name = "ToolStrip_Nodes"
        Me.ToolStrip_Nodes.Size = New System.Drawing.Size(773, 25)
        Me.ToolStrip_Nodes.TabIndex = 0
        Me.ToolStrip_Nodes.Text = "ToolStrip1"
        '
        'TSButton_AddNode
        '
        Me.TSButton_AddNode.Image = CType(resources.GetObject("TSButton_AddNode.Image"), System.Drawing.Image)
        Me.TSButton_AddNode.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.TSButton_AddNode.Name = "TSButton_AddNode"
        Me.TSButton_AddNode.Size = New System.Drawing.Size(51, 22)
        Me.TSButton_AddNode.Text = "New"
        '
        'TSButton_EditNode
        '
        Me.TSButton_EditNode.Image = CType(resources.GetObject("TSButton_EditNode.Image"), System.Drawing.Image)
        Me.TSButton_EditNode.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.TSButton_EditNode.Name = "TSButton_EditNode"
        Me.TSButton_EditNode.Size = New System.Drawing.Size(50, 22)
        Me.TSButton_EditNode.Text = "Edit "
        '
        'ToolStripButton_deleteNode
        '
        Me.ToolStripButton_deleteNode.Image = CType(resources.GetObject("ToolStripButton_deleteNode.Image"), System.Drawing.Image)
        Me.ToolStripButton_deleteNode.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton_deleteNode.Name = "ToolStripButton_deleteNode"
        Me.ToolStripButton_deleteNode.Size = New System.Drawing.Size(60, 22)
        Me.ToolStripButton_deleteNode.Text = "Delete"
        '
        'TabPage_Elements
        '
        Me.TabPage_Elements.Controls.Add(Me.ListView_Elements)
        Me.TabPage_Elements.Controls.Add(Me.ToolStrip_Elements)
        Me.TabPage_Elements.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_Elements.Name = "TabPage_Elements"
        Me.TabPage_Elements.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage_Elements.Size = New System.Drawing.Size(779, 95)
        Me.TabPage_Elements.TabIndex = 1
        Me.TabPage_Elements.Text = "Elements"
        Me.TabPage_Elements.UseVisualStyleBackColor = True
        '
        'ListView_Elements
        '
        Me.ListView_Elements.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6})
        Me.ListView_Elements.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView_Elements.FullRowSelect = True
        Me.ListView_Elements.GridLines = True
        Me.ListView_Elements.Location = New System.Drawing.Point(3, 28)
        Me.ListView_Elements.Name = "ListView_Elements"
        Me.ListView_Elements.Size = New System.Drawing.Size(773, 64)
        Me.ListView_Elements.TabIndex = 2
        Me.ListView_Elements.UseCompatibleStateImageBehavior = False
        Me.ListView_Elements.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Element ID"
        Me.ColumnHeader1.Width = 80
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Length (mm)"
        Me.ColumnHeader2.Width = 100
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Area (mm^2)"
        Me.ColumnHeader3.Width = 80
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "E (GPa)"
        Me.ColumnHeader4.Width = 80
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Node ID's"
        Me.ColumnHeader5.Width = 100
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Stress (MPa)"
        Me.ColumnHeader6.Width = 80
        '
        'ToolStrip_Elements
        '
        Me.ToolStrip_Elements.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton_AddElem, Me.ToolStripButton_DeleteElem})
        Me.ToolStrip_Elements.Location = New System.Drawing.Point(3, 3)
        Me.ToolStrip_Elements.Name = "ToolStrip_Elements"
        Me.ToolStrip_Elements.Size = New System.Drawing.Size(773, 25)
        Me.ToolStrip_Elements.TabIndex = 0
        Me.ToolStrip_Elements.Text = "ToolStrip1"
        '
        'ToolStripButton_AddElem
        '
        Me.ToolStripButton_AddElem.Image = CType(resources.GetObject("ToolStripButton_AddElem.Image"), System.Drawing.Image)
        Me.ToolStripButton_AddElem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton_AddElem.Name = "ToolStripButton_AddElem"
        Me.ToolStripButton_AddElem.Size = New System.Drawing.Size(51, 22)
        Me.ToolStripButton_AddElem.Text = "New"
        '
        'ToolStripButton_DeleteElem
        '
        Me.ToolStripButton_DeleteElem.Image = CType(resources.GetObject("ToolStripButton_DeleteElem.Image"), System.Drawing.Image)
        Me.ToolStripButton_DeleteElem.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton_DeleteElem.Name = "ToolStripButton_DeleteElem"
        Me.ToolStripButton_DeleteElem.Size = New System.Drawing.Size(60, 22)
        Me.ToolStripButton_DeleteElem.Text = "Delete"
        '
        'TabPage_Forces
        '
        Me.TabPage_Forces.Controls.Add(Me.ListView_Forces)
        Me.TabPage_Forces.Controls.Add(Me.ToolStrip_Force)
        Me.TabPage_Forces.Location = New System.Drawing.Point(4, 22)
        Me.TabPage_Forces.Name = "TabPage_Forces"
        Me.TabPage_Forces.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage_Forces.Size = New System.Drawing.Size(779, 95)
        Me.TabPage_Forces.TabIndex = 2
        Me.TabPage_Forces.Text = "Forces"
        Me.TabPage_Forces.UseVisualStyleBackColor = True
        '
        'ListView_Forces
        '
        Me.ListView_Forces.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Node, Me.Magnitue, Me.Direction, Me.Component})
        Me.ListView_Forces.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView_Forces.FullRowSelect = True
        Me.ListView_Forces.GridLines = True
        Me.ListView_Forces.Location = New System.Drawing.Point(3, 28)
        Me.ListView_Forces.Name = "ListView_Forces"
        Me.ListView_Forces.Size = New System.Drawing.Size(773, 64)
        Me.ListView_Forces.TabIndex = 1
        Me.ListView_Forces.UseCompatibleStateImageBehavior = False
        Me.ListView_Forces.View = System.Windows.Forms.View.Details
        '
        'Node
        '
        Me.Node.Text = "Attached Node"
        Me.Node.Width = 100
        '
        'Magnitue
        '
        Me.Magnitue.Text = "Magnitude (N)"
        Me.Magnitue.Width = 80
        '
        'Direction
        '
        Me.Direction.Text = "Direction"
        Me.Direction.Width = 150
        '
        'Component
        '
        Me.Component.Text = "Components (N)"
        Me.Component.Width = 150
        '
        'ToolStrip_Force
        '
        Me.ToolStrip_Force.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton_AddForce, Me.Toolstripbutton_deleteforce})
        Me.ToolStrip_Force.Location = New System.Drawing.Point(3, 3)
        Me.ToolStrip_Force.Name = "ToolStrip_Force"
        Me.ToolStrip_Force.Size = New System.Drawing.Size(773, 25)
        Me.ToolStrip_Force.TabIndex = 0
        Me.ToolStrip_Force.Text = "ToolStrip_Force"
        '
        'ToolStripButton_AddForce
        '
        Me.ToolStripButton_AddForce.Image = CType(resources.GetObject("ToolStripButton_AddForce.Image"), System.Drawing.Image)
        Me.ToolStripButton_AddForce.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton_AddForce.Name = "ToolStripButton_AddForce"
        Me.ToolStripButton_AddForce.Size = New System.Drawing.Size(51, 22)
        Me.ToolStripButton_AddForce.Text = "New"
        '
        'Toolstripbutton_deleteforce
        '
        Me.Toolstripbutton_deleteforce.Image = CType(resources.GetObject("Toolstripbutton_deleteforce.Image"), System.Drawing.Image)
        Me.Toolstripbutton_deleteforce.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.Toolstripbutton_deleteforce.Name = "Toolstripbutton_deleteforce"
        Me.Toolstripbutton_deleteforce.Size = New System.Drawing.Size(60, 22)
        Me.Toolstripbutton_deleteforce.Text = "Delete"
        '
        'CheckBox_DrawReactions
        '
        Me.CheckBox_DrawReactions.AutoSize = True
        Me.CheckBox_DrawReactions.Location = New System.Drawing.Point(266, 5)
        Me.CheckBox_DrawReactions.Name = "CheckBox_DrawReactions"
        Me.CheckBox_DrawReactions.Size = New System.Drawing.Size(102, 17)
        Me.CheckBox_DrawReactions.TabIndex = 4
        Me.CheckBox_DrawReactions.Text = "Draw Reactions"
        Me.CheckBox_DrawReactions.UseVisualStyleBackColor = True
        '
        'TrackBar_AnimateDisplacement
        '
        Me.TrackBar_AnimateDisplacement.AutoSize = False
        Me.TrackBar_AnimateDisplacement.LargeChange = 1
        Me.TrackBar_AnimateDisplacement.Location = New System.Drawing.Point(156, 3)
        Me.TrackBar_AnimateDisplacement.Maximum = 100
        Me.TrackBar_AnimateDisplacement.Name = "TrackBar_AnimateDisplacement"
        Me.TrackBar_AnimateDisplacement.Size = New System.Drawing.Size(104, 20)
        Me.TrackBar_AnimateDisplacement.TabIndex = 5
        Me.TrackBar_AnimateDisplacement.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(183, 22)
        Me.ToolStripMenuItem1.Text = "ToolStripMenuItem1"
        '
        'Mainform
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(789, 703)
        Me.Controls.Add(Me.TrackBar_AnimateDisplacement)
        Me.Controls.Add(Me.CheckBox_DrawReactions)
        Me.Controls.Add(Me.SplitContainer_MainHoriz)
        Me.Controls.Add(Me.ToolStrip_MainTop)
        Me.Controls.Add(Me.StatusStrip_MainBottom)
        Me.Name = "Mainform"
        Me.Text = "FEA"
        Me.StatusStrip_MainBottom.ResumeLayout(False)
        Me.StatusStrip_MainBottom.PerformLayout()
        Me.ToolStrip_MainTop.ResumeLayout(False)
        Me.ToolStrip_MainTop.PerformLayout()
        Me.SplitContainer_MainHoriz.Panel1.ResumeLayout(False)
        Me.SplitContainer_MainHoriz.Panel1.PerformLayout()
        Me.SplitContainer_MainHoriz.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer_MainHoriz, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer_MainHoriz.ResumeLayout(False)
        Me.TabControl_Bottom.ResumeLayout(False)
        Me.TabPage_Nodes.ResumeLayout(False)
        Me.TabPage_Nodes.PerformLayout()
        Me.ToolStrip_Nodes.ResumeLayout(False)
        Me.ToolStrip_Nodes.PerformLayout()
        Me.TabPage_Elements.ResumeLayout(False)
        Me.TabPage_Elements.PerformLayout()
        Me.ToolStrip_Elements.ResumeLayout(False)
        Me.ToolStrip_Elements.PerformLayout()
        Me.TabPage_Forces.ResumeLayout(False)
        Me.TabPage_Forces.PerformLayout()
        Me.ToolStrip_Force.ResumeLayout(False)
        Me.ToolStrip_Force.PerformLayout()
        CType(Me.TrackBar_AnimateDisplacement, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip_MainBottom As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStrip_MainTop As System.Windows.Forms.ToolStrip
    Friend WithEvents SplitContainer_MainHoriz As System.Windows.Forms.SplitContainer
    Friend WithEvents GlControl_Main As OpenTK.GLControl
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel3 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents TabControl_Bottom As System.Windows.Forms.TabControl
    Friend WithEvents TabPage_Nodes As System.Windows.Forms.TabPage
    Friend WithEvents TabPage_Elements As System.Windows.Forms.TabPage
    Friend WithEvents TabPage_Forces As System.Windows.Forms.TabPage
    Friend WithEvents ToolStrip_Nodes As System.Windows.Forms.ToolStrip
    Friend WithEvents TSButton_AddNode As System.Windows.Forms.ToolStripButton
    Friend WithEvents ListView_Nodes As System.Windows.Forms.ListView
    Friend WithEvents ID As System.Windows.Forms.ColumnHeader
    Friend WithEvents Coords As System.Windows.Forms.ColumnHeader
    Friend WithEvents Util As System.Windows.Forms.ColumnHeader
    Friend WithEvents Displacement As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToolStrip_Elements As System.Windows.Forms.ToolStrip
    Friend WithEvents ToolStripButton_AddElem As System.Windows.Forms.ToolStripButton
    Friend WithEvents ListView_Elements As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader5 As System.Windows.Forms.ColumnHeader
    Friend WithEvents TSButton_EditNode As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStrip_Force As System.Windows.Forms.ToolStrip
    Friend WithEvents ToolStripButton_AddForce As System.Windows.Forms.ToolStripButton
    Friend WithEvents Fixity As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToolStripButton_DeleteElem As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton_deleteNode As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripButton_Solve As System.Windows.Forms.ToolStripButton
    Friend WithEvents Reactions As System.Windows.Forms.ColumnHeader
    Friend WithEvents CheckBox_DrawReactions As System.Windows.Forms.CheckBox
    Friend WithEvents ToolStripDropDownButton1 As System.Windows.Forms.ToolStripDropDownButton
    Friend WithEvents ToolStripMenuItem_save As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem_load As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ListView_Forces As System.Windows.Forms.ListView
    Friend WithEvents Node As System.Windows.Forms.ColumnHeader
    Friend WithEvents Magnitue As System.Windows.Forms.ColumnHeader
    Friend WithEvents Direction As System.Windows.Forms.ColumnHeader
    Friend WithEvents Component As System.Windows.Forms.ColumnHeader
    Friend WithEvents Toolstripbutton_deleteforce As System.Windows.Forms.ToolStripButton
    Friend WithEvents TrackBar_AnimateDisplacement As System.Windows.Forms.TrackBar
    Friend WithEvents ColumnHeader6 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem

End Class
