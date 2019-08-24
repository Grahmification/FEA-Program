Public Class ElemAddForm

    Dim nodeCollection As New AutoCompleteStringCollection

    Private Sub ElemAddForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each Node As NodeManager.Node In Nodes.Nodelist
            Dim text As String = CStr(Node.ID) & " - (X" & CStr(Node.Coords_mm(0)) & ", Y" & CStr(Node.Coords_mm(1)) & ", Z" & CStr(Node.Coords_mm(2)) & ")" 'convert to mm
            ComboBox_Node1.Items.Add(text)
            ComboBox_Node2.Items.Add(text)
            nodeCollection.Add(text)
        Next

        ComboBox_Node1.AutoCompleteCustomSource = nodeCollection
        ComboBox_Node2.AutoCompleteCustomSource = nodeCollection

        For Each Mat As Material In Materials
            Dim text As String = Mat.Name & " (" & CStr(Mat.E_GPa) & " GPa)"
            ComboBox1.Items.Add(text)
        Next

        If ComboBox_Node1.Items.Count > 0 Then
            ComboBox_Node1.SelectedIndex = 0
        End If

        If ComboBox_Node2.Items.Count > 1 Then
            ComboBox_Node2.SelectedIndex = 1
        End If

        If ComboBox1.Items.Count > 0 Then
            ComboBox1.SelectedIndex = 0
        End If
    End Sub
    Private Sub TextBox_MouseClick(sender As Object, e As MouseEventArgs) Handles TextBox1.MouseClick
        Dim sendtxt As TextBox = sender
        sendtxt.SelectAll()
    End Sub

    Private Sub ValidateOK(sender As Object, e As EventArgs) Handles TextBox1.TextChanged, ComboBox_Node1.TextChanged, ComboBox_Node2.TextChanged, ComboBox1.TextChanged
        Try
   
            '--------------- Validate for correct inputs -----------------

            Dim tmp As Double = CDbl(TextBox1.Text) 'text that value is a number
            tmp += 5.0

            If ComboBox1.Items.Count > 0 And nodeCollection.Count >= 2 And nodeCollection.Contains(ComboBox_Node1.Text) And nodeCollection.Contains(ComboBox_Node2.Text) And ComboBox_Node1.Text <> ComboBox_Node2.Text Then
                Button_OK.Enabled = True
            Else
                Button_OK.Enabled = False
            End If
        Catch ex As Exception
            Button_OK.Enabled = False
        End Try
    End Sub

    Private Sub Button_Click(sender As Object, e As EventArgs) Handles Button_OK.Click, Button_Cancel.Click
        Dim sendbtn As Button = sender
        Nodes.SelectNodes(Nodes.AllIDs.ToArray, False)

        If sendbtn Is Button_OK Then
            Dim node1ID As Integer = CInt(ComboBox_Node1.Text.Split(" ").First)
            Dim node2ID As Integer = CInt(ComboBox_Node2.Text.Split(" ").First)
            Dim area As Double = CDbl(TextBox1.Text) / (1000 * 1000.0) 'covert to m^2
            Dim Estring1 As String = ComboBox1.Text.Split("(").Last
            Dim YoungsMod As Double = CDbl(Estring1.Split(" ").First) * 1000 * 1000 * 1000 'Convert to Pa
            Elements.Add(Nodes.NodeObj(node1ID), Nodes.NodeObj(node2ID), area, YoungsMod)
        End If

        Me.Close()
    End Sub


    Private Sub ComboBox_Node1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_Node1.TextChanged, ComboBox_Node2.TextChanged
        '----------- Highlight nodes on screen -------------------
            Nodes.SelectNodes(Nodes.AllIDs.ToArray, False)

        Dim selectIDs As New List(Of Integer)

        If nodeCollection.Contains(ComboBox_Node1.Text) Then
            selectIDs.Add(CInt(ComboBox_Node1.Text.Split(" ").First))
        End If

        If nodeCollection.Contains(ComboBox_Node2.Text) Then
            selectIDs.Add(CInt(ComboBox_Node2.Text.Split(" ").First))
        End If

        If selectIDs.Count >= 1 Then
            Nodes.SelectNodes(selectIDs.ToArray, True)
        End If
    End Sub


End Class