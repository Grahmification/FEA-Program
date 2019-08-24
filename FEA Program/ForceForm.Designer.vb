<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ForceForm
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
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ComboBox_Node = New System.Windows.Forms.ComboBox()
        Me.Panel_MagDir = New System.Windows.Forms.Panel()
        Me.Btn_Yneg = New System.Windows.Forms.Button()
        Me.Btn_Ypos = New System.Windows.Forms.Button()
        Me.Btn_Zpos = New System.Windows.Forms.Button()
        Me.Btn_Zneg = New System.Windows.Forms.Button()
        Me.Btn_Xneg = New System.Windows.Forms.Button()
        Me.Btn_Xpos = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBox_Zdir = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBox_Ydir = New System.Windows.Forms.TextBox()
        Me.TextBox_Xdir = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox_Magnitude = New System.Windows.Forms.TextBox()
        Me.RadioButton_MagDir = New System.Windows.Forms.RadioButton()
        Me.RadioButton_Components = New System.Windows.Forms.RadioButton()
        Me.Panel_ForceComponents = New System.Windows.Forms.Panel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TextBox_Zcomp = New System.Windows.Forms.TextBox()
        Me.TextBox_Ycomp = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TextBox_XComp = New System.Windows.Forms.TextBox()
        Me.Btn_Accept = New System.Windows.Forms.Button()
        Me.Btn_Cancel = New System.Windows.Forms.Button()
        Me.Panel_MagDir.SuspendLayout()
        Me.Panel_ForceComponents.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(117, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(93, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Node to add force"
        '
        'ComboBox_Node
        '
        Me.ComboBox_Node.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.ComboBox_Node.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
        Me.ComboBox_Node.FormattingEnabled = True
        Me.ComboBox_Node.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.ComboBox_Node.Location = New System.Drawing.Point(57, 25)
        Me.ComboBox_Node.MaxDropDownItems = 100
        Me.ComboBox_Node.Name = "ComboBox_Node"
        Me.ComboBox_Node.Size = New System.Drawing.Size(212, 21)
        Me.ComboBox_Node.TabIndex = 5
        '
        'Panel_MagDir
        '
        Me.Panel_MagDir.BackColor = System.Drawing.SystemColors.ControlDark
        Me.Panel_MagDir.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel_MagDir.Controls.Add(Me.Btn_Yneg)
        Me.Panel_MagDir.Controls.Add(Me.Btn_Ypos)
        Me.Panel_MagDir.Controls.Add(Me.Btn_Zpos)
        Me.Panel_MagDir.Controls.Add(Me.Btn_Zneg)
        Me.Panel_MagDir.Controls.Add(Me.Btn_Xneg)
        Me.Panel_MagDir.Controls.Add(Me.Btn_Xpos)
        Me.Panel_MagDir.Controls.Add(Me.Label6)
        Me.Panel_MagDir.Controls.Add(Me.Label5)
        Me.Panel_MagDir.Controls.Add(Me.TextBox_Zdir)
        Me.Panel_MagDir.Controls.Add(Me.Label4)
        Me.Panel_MagDir.Controls.Add(Me.TextBox_Ydir)
        Me.Panel_MagDir.Controls.Add(Me.TextBox_Xdir)
        Me.Panel_MagDir.Controls.Add(Me.Label3)
        Me.Panel_MagDir.Controls.Add(Me.Label1)
        Me.Panel_MagDir.Controls.Add(Me.TextBox_Magnitude)
        Me.Panel_MagDir.Location = New System.Drawing.Point(12, 84)
        Me.Panel_MagDir.Name = "Panel_MagDir"
        Me.Panel_MagDir.Size = New System.Drawing.Size(310, 114)
        Me.Panel_MagDir.TabIndex = 6
        '
        'Btn_Yneg
        '
        Me.Btn_Yneg.Location = New System.Drawing.Point(253, 55)
        Me.Btn_Yneg.Name = "Btn_Yneg"
        Me.Btn_Yneg.Size = New System.Drawing.Size(29, 23)
        Me.Btn_Yneg.TabIndex = 21
        Me.Btn_Yneg.Text = "Y-"
        Me.Btn_Yneg.UseVisualStyleBackColor = True
        '
        'Btn_Ypos
        '
        Me.Btn_Ypos.Location = New System.Drawing.Point(218, 55)
        Me.Btn_Ypos.Name = "Btn_Ypos"
        Me.Btn_Ypos.Size = New System.Drawing.Size(29, 23)
        Me.Btn_Ypos.TabIndex = 20
        Me.Btn_Ypos.Text = "Y+"
        Me.Btn_Ypos.UseVisualStyleBackColor = True
        '
        'Btn_Zpos
        '
        Me.Btn_Zpos.Location = New System.Drawing.Point(218, 81)
        Me.Btn_Zpos.Name = "Btn_Zpos"
        Me.Btn_Zpos.Size = New System.Drawing.Size(29, 23)
        Me.Btn_Zpos.TabIndex = 19
        Me.Btn_Zpos.Text = "Z+"
        Me.Btn_Zpos.UseVisualStyleBackColor = True
        '
        'Btn_Zneg
        '
        Me.Btn_Zneg.Location = New System.Drawing.Point(253, 81)
        Me.Btn_Zneg.Name = "Btn_Zneg"
        Me.Btn_Zneg.Size = New System.Drawing.Size(29, 23)
        Me.Btn_Zneg.TabIndex = 18
        Me.Btn_Zneg.Text = "Z-"
        Me.Btn_Zneg.UseVisualStyleBackColor = True
        '
        'Btn_Xneg
        '
        Me.Btn_Xneg.Location = New System.Drawing.Point(253, 29)
        Me.Btn_Xneg.Name = "Btn_Xneg"
        Me.Btn_Xneg.Size = New System.Drawing.Size(29, 23)
        Me.Btn_Xneg.TabIndex = 17
        Me.Btn_Xneg.Text = "X-"
        Me.Btn_Xneg.UseVisualStyleBackColor = True
        '
        'Btn_Xpos
        '
        Me.Btn_Xpos.Location = New System.Drawing.Point(218, 29)
        Me.Btn_Xpos.Name = "Btn_Xpos"
        Me.Btn_Xpos.Size = New System.Drawing.Size(29, 23)
        Me.Btn_Xpos.TabIndex = 16
        Me.Btn_Xpos.Text = "X+"
        Me.Btn_Xpos.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(204, 12)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(87, 13)
        Me.Label6.TabIndex = 15
        Me.Label6.Text = "Preset Directions"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 86)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(76, 13)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Z Vector Dir [ ]"
        '
        'TextBox_Zdir
        '
        Me.TextBox_Zdir.Location = New System.Drawing.Point(88, 83)
        Me.TextBox_Zdir.Name = "TextBox_Zdir"
        Me.TextBox_Zdir.Size = New System.Drawing.Size(110, 20)
        Me.TextBox_Zdir.TabIndex = 13
        Me.TextBox_Zdir.Text = "0.00000"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 60)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(76, 13)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "Y Vector Dir [ ]"
        '
        'TextBox_Ydir
        '
        Me.TextBox_Ydir.Location = New System.Drawing.Point(88, 57)
        Me.TextBox_Ydir.Name = "TextBox_Ydir"
        Me.TextBox_Ydir.Size = New System.Drawing.Size(110, 20)
        Me.TextBox_Ydir.TabIndex = 11
        Me.TextBox_Ydir.Text = "0.00000"
        '
        'TextBox_Xdir
        '
        Me.TextBox_Xdir.Location = New System.Drawing.Point(88, 31)
        Me.TextBox_Xdir.Name = "TextBox_Xdir"
        Me.TextBox_Xdir.Size = New System.Drawing.Size(110, 20)
        Me.TextBox_Xdir.TabIndex = 10
        Me.TextBox_Xdir.Text = "1.00000"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 34)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(76, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "X Vector Dir [ ]"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Magnitude [N]"
        '
        'TextBox_Magnitude
        '
        Me.TextBox_Magnitude.Location = New System.Drawing.Point(88, 5)
        Me.TextBox_Magnitude.Name = "TextBox_Magnitude"
        Me.TextBox_Magnitude.Size = New System.Drawing.Size(110, 20)
        Me.TextBox_Magnitude.TabIndex = 0
        Me.TextBox_Magnitude.Text = "1.00000"
        '
        'RadioButton_MagDir
        '
        Me.RadioButton_MagDir.AutoSize = True
        Me.RadioButton_MagDir.Checked = True
        Me.RadioButton_MagDir.Location = New System.Drawing.Point(12, 61)
        Me.RadioButton_MagDir.Name = "RadioButton_MagDir"
        Me.RadioButton_MagDir.Size = New System.Drawing.Size(141, 17)
        Me.RadioButton_MagDir.TabIndex = 7
        Me.RadioButton_MagDir.TabStop = True
        Me.RadioButton_MagDir.Text = "Magnitude and Direction"
        Me.RadioButton_MagDir.UseVisualStyleBackColor = True
        '
        'RadioButton_Components
        '
        Me.RadioButton_Components.AutoSize = True
        Me.RadioButton_Components.Location = New System.Drawing.Point(12, 204)
        Me.RadioButton_Components.Name = "RadioButton_Components"
        Me.RadioButton_Components.Size = New System.Drawing.Size(114, 17)
        Me.RadioButton_Components.TabIndex = 8
        Me.RadioButton_Components.Text = "Force Components"
        Me.RadioButton_Components.UseVisualStyleBackColor = True
        '
        'Panel_ForceComponents
        '
        Me.Panel_ForceComponents.BackColor = System.Drawing.SystemColors.ControlDark
        Me.Panel_ForceComponents.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Panel_ForceComponents.Controls.Add(Me.Label8)
        Me.Panel_ForceComponents.Controls.Add(Me.Label7)
        Me.Panel_ForceComponents.Controls.Add(Me.TextBox_Zcomp)
        Me.Panel_ForceComponents.Controls.Add(Me.TextBox_Ycomp)
        Me.Panel_ForceComponents.Controls.Add(Me.Label11)
        Me.Panel_ForceComponents.Controls.Add(Me.TextBox_XComp)
        Me.Panel_ForceComponents.Enabled = False
        Me.Panel_ForceComponents.Location = New System.Drawing.Point(12, 227)
        Me.Panel_ForceComponents.Name = "Panel_ForceComponents"
        Me.Panel_ForceComponents.Size = New System.Drawing.Size(310, 86)
        Me.Panel_ForceComponents.TabIndex = 9
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(8, 60)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(88, 13)
        Me.Label8.TabIndex = 13
        Me.Label8.Text = "Z Component [N]"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(8, 34)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(88, 13)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "Y Component [N]"
        '
        'TextBox_Zcomp
        '
        Me.TextBox_Zcomp.Location = New System.Drawing.Point(102, 57)
        Me.TextBox_Zcomp.Name = "TextBox_Zcomp"
        Me.TextBox_Zcomp.Size = New System.Drawing.Size(110, 20)
        Me.TextBox_Zcomp.TabIndex = 11
        Me.TextBox_Zcomp.Text = "0.00000"
        '
        'TextBox_Ycomp
        '
        Me.TextBox_Ycomp.Location = New System.Drawing.Point(102, 31)
        Me.TextBox_Ycomp.Name = "TextBox_Ycomp"
        Me.TextBox_Ycomp.Size = New System.Drawing.Size(110, 20)
        Me.TextBox_Ycomp.TabIndex = 10
        Me.TextBox_Ycomp.Text = "0.00000"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(8, 8)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(88, 13)
        Me.Label11.TabIndex = 8
        Me.Label11.Text = "X Component [N]"
        '
        'TextBox_XComp
        '
        Me.TextBox_XComp.Location = New System.Drawing.Point(102, 5)
        Me.TextBox_XComp.Name = "TextBox_XComp"
        Me.TextBox_XComp.Size = New System.Drawing.Size(110, 20)
        Me.TextBox_XComp.TabIndex = 0
        Me.TextBox_XComp.Text = "1.00000"
        '
        'Btn_Accept
        '
        Me.Btn_Accept.Location = New System.Drawing.Point(78, 332)
        Me.Btn_Accept.Name = "Btn_Accept"
        Me.Btn_Accept.Size = New System.Drawing.Size(75, 23)
        Me.Btn_Accept.TabIndex = 10
        Me.Btn_Accept.Text = "Create"
        Me.Btn_Accept.UseVisualStyleBackColor = True
        '
        'Btn_Cancel
        '
        Me.Btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Btn_Cancel.Location = New System.Drawing.Point(172, 332)
        Me.Btn_Cancel.Name = "Btn_Cancel"
        Me.Btn_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.Btn_Cancel.TabIndex = 11
        Me.Btn_Cancel.Text = "Cancel"
        Me.Btn_Cancel.UseVisualStyleBackColor = True
        '
        'ForceForm
        '
        Me.AcceptButton = Me.Btn_Accept
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Btn_Cancel
        Me.ClientSize = New System.Drawing.Size(335, 371)
        Me.ControlBox = False
        Me.Controls.Add(Me.Btn_Cancel)
        Me.Controls.Add(Me.Btn_Accept)
        Me.Controls.Add(Me.Panel_ForceComponents)
        Me.Controls.Add(Me.RadioButton_Components)
        Me.Controls.Add(Me.RadioButton_MagDir)
        Me.Controls.Add(Me.Panel_MagDir)
        Me.Controls.Add(Me.ComboBox_Node)
        Me.Controls.Add(Me.Label2)
        Me.Name = "ForceForm"
        Me.Text = "Add Force"
        Me.Panel_MagDir.ResumeLayout(False)
        Me.Panel_MagDir.PerformLayout()
        Me.Panel_ForceComponents.ResumeLayout(False)
        Me.Panel_ForceComponents.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents ComboBox_Node As System.Windows.Forms.ComboBox
    Friend WithEvents Panel_MagDir As System.Windows.Forms.Panel
    Friend WithEvents Btn_Yneg As System.Windows.Forms.Button
    Friend WithEvents Btn_Ypos As System.Windows.Forms.Button
    Friend WithEvents Btn_Zpos As System.Windows.Forms.Button
    Friend WithEvents Btn_Zneg As System.Windows.Forms.Button
    Friend WithEvents Btn_Xneg As System.Windows.Forms.Button
    Friend WithEvents Btn_Xpos As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Zdir As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Ydir As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Xdir As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Magnitude As System.Windows.Forms.TextBox
    Friend WithEvents RadioButton_MagDir As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButton_Components As System.Windows.Forms.RadioButton
    Friend WithEvents Panel_ForceComponents As System.Windows.Forms.Panel
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Zcomp As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Ycomp As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents TextBox_XComp As System.Windows.Forms.TextBox
    Friend WithEvents Btn_Accept As System.Windows.Forms.Button
    Friend WithEvents Btn_Cancel As System.Windows.Forms.Button
End Class
