<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NodeAddForm
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.Button_OK = New System.Windows.Forms.Button()
        Me.Button_Cancel = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.CheckBox_FixX = New System.Windows.Forms.CheckBox()
        Me.CheckBox_FixY = New System.Windows.Forms.CheckBox()
        Me.CheckBox_FixZ = New System.Windows.Forms.CheckBox()
        Me.Button_FixAll = New System.Windows.Forms.Button()
        Me.Button_FloatAll = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 33)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(96, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "X Coordinate (mm):"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 55)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(96, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Y Coordinate (mm):"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 78)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(96, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Z Coordinate (mm):"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(114, 30)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(100, 20)
        Me.TextBox1.TabIndex = 3
        Me.TextBox1.Text = "0.0"
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(114, 52)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(100, 20)
        Me.TextBox2.TabIndex = 4
        Me.TextBox2.Text = "0.0"
        '
        'TextBox3
        '
        Me.TextBox3.Location = New System.Drawing.Point(114, 78)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(100, 20)
        Me.TextBox3.TabIndex = 5
        Me.TextBox3.Text = "0.0"
        '
        'Button_OK
        '
        Me.Button_OK.Location = New System.Drawing.Point(107, 120)
        Me.Button_OK.Name = "Button_OK"
        Me.Button_OK.Size = New System.Drawing.Size(75, 23)
        Me.Button_OK.TabIndex = 6
        Me.Button_OK.Text = "Accept"
        Me.Button_OK.UseVisualStyleBackColor = True
        '
        'Button_Cancel
        '
        Me.Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button_Cancel.Location = New System.Drawing.Point(206, 120)
        Me.Button_Cancel.Name = "Button_Cancel"
        Me.Button_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.Button_Cancel.TabIndex = 7
        Me.Button_Cancel.Text = "Cancel"
        Me.Button_Cancel.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(258, 9)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(92, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Fix Displacements"
        '
        'CheckBox_FixX
        '
        Me.CheckBox_FixX.AutoSize = True
        Me.CheckBox_FixX.Location = New System.Drawing.Point(248, 32)
        Me.CheckBox_FixX.Name = "CheckBox_FixX"
        Me.CheckBox_FixX.Size = New System.Drawing.Size(33, 17)
        Me.CheckBox_FixX.TabIndex = 9
        Me.CheckBox_FixX.Text = "X"
        Me.CheckBox_FixX.UseVisualStyleBackColor = True
        '
        'CheckBox_FixY
        '
        Me.CheckBox_FixY.AutoSize = True
        Me.CheckBox_FixY.Location = New System.Drawing.Point(248, 54)
        Me.CheckBox_FixY.Name = "CheckBox_FixY"
        Me.CheckBox_FixY.Size = New System.Drawing.Size(33, 17)
        Me.CheckBox_FixY.TabIndex = 10
        Me.CheckBox_FixY.Text = "Y"
        Me.CheckBox_FixY.UseVisualStyleBackColor = True
        '
        'CheckBox_FixZ
        '
        Me.CheckBox_FixZ.AutoSize = True
        Me.CheckBox_FixZ.Location = New System.Drawing.Point(248, 77)
        Me.CheckBox_FixZ.Name = "CheckBox_FixZ"
        Me.CheckBox_FixZ.Size = New System.Drawing.Size(33, 17)
        Me.CheckBox_FixZ.TabIndex = 11
        Me.CheckBox_FixZ.Text = "Z"
        Me.CheckBox_FixZ.UseVisualStyleBackColor = True
        '
        'Button_FixAll
        '
        Me.Button_FixAll.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button_FixAll.Location = New System.Drawing.Point(287, 36)
        Me.Button_FixAll.Name = "Button_FixAll"
        Me.Button_FixAll.Size = New System.Drawing.Size(75, 23)
        Me.Button_FixAll.TabIndex = 12
        Me.Button_FixAll.Text = "Fix All"
        Me.Button_FixAll.UseVisualStyleBackColor = True
        '
        'Button_FloatAll
        '
        Me.Button_FloatAll.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button_FloatAll.Location = New System.Drawing.Point(287, 65)
        Me.Button_FloatAll.Name = "Button_FloatAll"
        Me.Button_FloatAll.Size = New System.Drawing.Size(75, 23)
        Me.Button_FloatAll.TabIndex = 13
        Me.Button_FloatAll.Text = "Float All"
        Me.Button_FloatAll.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(138, 9)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(44, 13)
        Me.Label5.TabIndex = 14
        Me.Label5.Text = "Position"
        '
        'NodeAddForm
        '
        Me.AcceptButton = Me.Button_OK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Button_Cancel
        Me.ClientSize = New System.Drawing.Size(377, 153)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Button_FloatAll)
        Me.Controls.Add(Me.Button_FixAll)
        Me.Controls.Add(Me.CheckBox_FixZ)
        Me.Controls.Add(Me.CheckBox_FixY)
        Me.Controls.Add(Me.CheckBox_FixX)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Button_Cancel)
        Me.Controls.Add(Me.Button_OK)
        Me.Controls.Add(Me.TextBox3)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Name = "NodeAddForm"
        Me.Text = "Add Node"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents Button_OK As System.Windows.Forms.Button
    Friend WithEvents Button_Cancel As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents CheckBox_FixX As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox_FixY As System.Windows.Forms.CheckBox
    Friend WithEvents CheckBox_FixZ As System.Windows.Forms.CheckBox
    Friend WithEvents Button_FixAll As System.Windows.Forms.Button
    Friend WithEvents Button_FloatAll As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
End Class
