Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Public Class ForceForm

    Dim nodeCollection As New AutoCompleteStringCollection

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        Me.Show()
        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub ForceForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each Node As NodeManager.Node In Nodes.Nodelist
            Dim text As String = CStr(Node.ID) & " - (X" & CStr(Node.Coords_mm(0)) & ", Y" & CStr(Node.Coords_mm(1)) & ", Z" & CStr(Node.Coords_mm(2)) & ")"
            ComboBox_Node.Items.Add(text)
            nodeCollection.Add(text)
        Next

        If nodeCollection.Count > 0 Then
            ComboBox_Node.SelectedIndex = 0
        End If

        ComboBox_Node.AutoCompleteCustomSource = nodeCollection
    End Sub
    Private Sub TextBox_MouseClick(sender As Object, e As MouseEventArgs) Handles TextBox_Magnitude.MouseClick, TextBox_XComp.MouseClick, TextBox_Xdir.MouseClick, TextBox_Ycomp.MouseClick, TextBox_Ydir.MouseClick, TextBox_Zcomp.MouseClick, TextBox_Zdir.MouseClick
        Dim sendtxt As TextBox = sender
        sendtxt.SelectAll()
    End Sub
    Private Sub ComboBox_Node_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_Node.TextChanged
        '----------- Highlight nodes on screen -------------------
        Nodes.SelectNodes(Nodes.AllIDs.ToArray, False)

        Dim selectIDs As New List(Of Integer)

        If nodeCollection.Contains(ComboBox_Node.Text) Then
            selectIDs.Add(CInt(ComboBox_Node.Text.Split(" ").First))
            Nodes.SelectNodes(selectIDs.ToArray, True)
        End If
    End Sub
    Private Sub ValidateOK(sender As Object, e As EventArgs) Handles TextBox_Magnitude.MouseClick, TextBox_XComp.MouseClick, TextBox_Xdir.MouseClick, TextBox_Ycomp.MouseClick, TextBox_Ydir.MouseClick, TextBox_Zcomp.MouseClick, TextBox_Zdir.MouseClick, ComboBox_Node.TextChanged
        Try

            '---------------Check Magnitude box  -----------------

            If Panel_MagDir.Enabled Then
                Dim tmp As Double = CDbl(TextBox_Magnitude.Text) 'text that value is a number
                tmp += CDbl(TextBox_Xdir.Text)
                tmp += CDbl(TextBox_Ydir.Text)
                tmp += CDbl(TextBox_Zdir.Text)

                If CDbl(TextBox_Xdir.Text) = 0 And CDbl(TextBox_Ydir.Text) = 0 And CDbl(TextBox_Zdir.Text) = 0 Then
                    Throw New Exception
                End If

            ElseIf Panel_ForceComponents.Enabled Then
                Dim tmp As Double = CDbl(TextBox_XComp.Text) 'text that value is a number
                tmp += CDbl(TextBox_Ycomp.Text)
                tmp += CDbl(TextBox_Zcomp.Text)

                If CDbl(TextBox_XComp.Text) = 0 And CDbl(TextBox_Ycomp.Text) = 0 And CDbl(TextBox_Zcomp.Text) = 0 Then
                    Throw New Exception
                End If

            Else
                Throw New Exception
            End If

            '--------------- Check Combobox -----------------

            If ComboBox_Node.Items.Count > 0 And nodeCollection.Contains(ComboBox_Node.Text) Then
                Btn_Accept.Enabled = True
            Else
                Throw New Exception
            End If
        Catch ex As Exception
            Btn_Accept.Enabled = False
        End Try
    End Sub

    Private Sub Button_Click(sender As Object, e As EventArgs) Handles Btn_Cancel.Click, Btn_Accept.Click
        Try
            Dim sendbtn As Button = sender
            Nodes.SelectNodes(Nodes.AllIDs.ToArray, False)



            If sendbtn Is Btn_Accept Then
                Dim outputForce As New List(Of Double()) 'need in list format
                Dim vect As Vector3 = Vector3.Zero

                Dim nodeID As New List(Of Integer)
                nodeID.Add(CInt(ComboBox_Node.Text.Split(" ").First)) 'need in list format

                If Panel_MagDir.Enabled Then
                    vect.X = CDbl(TextBox_Xdir.Text)
                    vect.Y = CDbl(TextBox_Ydir.Text)
                    vect.Z = CDbl(TextBox_Zdir.Text)
                    vect.Normalize()
                    vect = Vector3.Multiply(vect, CDbl(TextBox_Magnitude.Text))
                    outputForce.Add({vect.X, vect.Y, vect.Z})

                    Nodes.Addforce(outputForce, nodeID)

                ElseIf Panel_ForceComponents.Enabled Then
                    vect.X = CDbl(TextBox_XComp.Text)
                    vect.Y = CDbl(TextBox_Ycomp.Text)
                    vect.Z = CDbl(TextBox_Zcomp.Text)
                    outputForce.Add({vect.X, vect.Y, vect.Z})

                    Nodes.Addforce(outputForce, nodeID)

                End If
            End If

            Me.Close()

        Catch ex As Exception
        End Try
    End Sub

    Private Sub RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton_MagDir.CheckedChanged, RadioButton_MagDir.CheckedChanged


        If RadioButton_MagDir.Checked Then
            Panel_MagDir.Enabled = True
            Panel_ForceComponents.Enabled = False

        ElseIf RadioButton_Components.Checked Then
            Panel_MagDir.Enabled = False
            Panel_ForceComponents.Enabled = True

        Else
            Panel_MagDir.Enabled = False
            Panel_ForceComponents.Enabled = False
        End If
    End Sub

    Private Sub PresetDirections(sender As Object, e As EventArgs) Handles Btn_Xpos.Click, Btn_Ypos.Click, Btn_Zpos.Click, Btn_Xneg.Click, Btn_Yneg.Click, Btn_Zneg.Click
        Dim btn As Button = sender

        If btn Is Btn_Xpos Or btn Is Btn_Xneg Then
            TextBox_Ydir.Text = "0.00000"
            TextBox_Zdir.Text = "0.00000"
            If btn Is Btn_Xpos Then
                TextBox_Xdir.Text = "1.00000"
            Else
                TextBox_Xdir.Text = "-1.00000"
            End If
        End If

        If btn Is Btn_Ypos Or btn Is Btn_Yneg Then
            TextBox_Xdir.Text = "0.00000"
            TextBox_Zdir.Text = "0.00000"
            If btn Is Btn_Ypos Then
                TextBox_Ydir.Text = "1.00000"
            Else
                TextBox_Ydir.Text = "-1.00000"
            End If
        End If

        If btn Is Btn_Zpos Or btn Is Btn_Zneg Then
            TextBox_Xdir.Text = "0.00000"
            TextBox_Ydir.Text = "0.00000"
            If btn Is Btn_Zpos Then
                TextBox_Zdir.Text = "1.00000"
            Else
                TextBox_Zdir.Text = "-1.00000"
            End If
        End If
    End Sub
End Class