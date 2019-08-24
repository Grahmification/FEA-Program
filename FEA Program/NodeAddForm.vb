Public Class NodeAddForm

    Private _editID As Integer

    Public Sub New(Optional ByVal EditNodeID As Integer = -1)
        InitializeComponent()
        Me.Show()

        _editID = EditNodeID

        If _editID <> -1 Then
            Me.Text = "Edit Node " & CStr(_editID)
            Dim N As NodeManager.Node = Nodes.NodeObj(_editID)
            TextBox1.Text = N.Coords_mm(0)
            TextBox2.Text = N.Coords_mm(1)
            TextBox3.Text = N.Coords_mm(2)

            If N.Fixity(0) = 1 Then
                CheckBox_FixX.Checked = True
            End If
            If N.Fixity(1) = 1 Then
                CheckBox_FixY.Checked = True
            End If
            If N.Fixity(2) = 1 Then
                CheckBox_FixZ.Checked = True
            End If
        End If
    End Sub

    Private Sub Button_Click(sender As Object, e As EventArgs) Handles Button_OK.Click, Button_Cancel.Click
        Dim sendbtn As Button = sender

        If sendbtn Is Button_OK Then

            Dim coords As New List(Of Double())
            Dim fixity As New List(Of Double())

            coords.Add({CDbl(TextBox1.Text) / 1000.0, CDbl(TextBox2.Text) / 1000.0, CDbl(TextBox3.Text) / 1000.0}) 'convert to m
            fixity.Add({CInt(CheckBox_FixX.Checked) * -1, CInt(CheckBox_FixY.Checked) * -1, CInt(CheckBox_FixZ.Checked) * -1})

            If _editID = -1 Then
                Nodes.AddNodes(coords, fixity)
            Else
                Dim editIDs As New List(Of Integer) 'convert to list form for output
                editIDs.Add(_editID)

                Nodes.EditNode(coords, fixity, editIDs)
            End If
            
        End If

        Me.Close()
    End Sub

    Private Sub TextBox_MouseClick(sender As Object, e As MouseEventArgs) Handles TextBox1.MouseClick, TextBox2.MouseClick, TextBox3.MouseClick
        Dim sendtxt As TextBox = sender
        sendtxt.SelectAll()
    End Sub
    Private Sub ValidateEntry(sender As Object, e As EventArgs) Handles TextBox1.TextChanged, TextBox2.TextChanged, TextBox3.TextChanged
        Try
            Dim tmp As Double = CDbl(TextBox1.Text)
            tmp += CDbl(TextBox2.Text)
            tmp += CDbl(TextBox3.Text)
            Button_OK.Enabled = True
        Catch ex As Exception
            Button_OK.Enabled = False
        End Try
    End Sub
    Private Sub Button_FixFloat_Click(sender As Object, e As EventArgs) Handles Button_FixAll.Click, Button_FloatAll.Click
        Dim btn As Button = sender

        If sender Is Button_FixAll Then
            CheckBox_FixX.Checked = True
            CheckBox_FixY.Checked = True
            CheckBox_FixZ.Checked = True
        Else
            CheckBox_FixX.Checked = False
            CheckBox_FixY.Checked = False
            CheckBox_FixZ.Checked = False
        End If

    End Sub



 
 
End Class