Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports MathNet.Numerics.LinearAlgebra.Double
Public Class Mainform

    Dim WithEvents view As New View()
    Dim coord As CoordinateSystem = New CoordinateSystem(50)
    Dim tmp As List(Of NodeManager.Node)

    Private animationPercent As Double = -1
    Private drawReactions As Boolean = False

    Private Sub GlControl_Main_Load(sender As Object, e As EventArgs) Handles GlControl_Main.Load
        Loadedform = Me
        GL.ClearColor(Color.Black)
        view.Refresh(GlControl_Main)



        Materials.Add(New Material("Steel", 200))
        Materials.Add(New Material("Aluminum", 69))
    End Sub
    Private Sub GlControl_Main_MouseDown(sender As Object, e As MouseEventArgs) Handles GlControl_Main.MouseDown
        view.Dragstarted(GlControl_Main, MousePosition.X, MousePosition.Y)
    End Sub
    Private Sub GlControl_Main_MouseMove(sender As Object, e As MouseEventArgs) Handles GlControl_Main.MouseMove
        If e.Button = Windows.Forms.MouseButtons.Left Then
            view.Dragged(GlControl_Main, MousePosition.X, MousePosition.Y)
        End If
    End Sub
    Private Sub GlControl_Main_MouseUp(sender As Object, e As MouseEventArgs) Handles GlControl_Main.MouseUp
        view.EndDrag()
    End Sub
    Private Sub GlControl_Main_MouseWheel(sender As Object, e As MouseEventArgs) Handles GlControl_Main.MouseWheel
        view.MouseWheeled(GlControl_Main, e.Delta)
    End Sub

   

    Private Sub TSButton_ModifyFeature_Click(sender As Object, e As EventArgs) Handles TSButton_AddNode.Click, ToolStripButton_AddElem.Click, TSButton_EditNode.Click, ToolStripButton_AddForce.Click, ToolStripButton_DeleteElem.Click, ToolStripButton_deleteNode.Click, Toolstripbutton_deleteforce.Click
        Dim TSB As ToolStripButton = sender

        If TSB Is ToolStripButton_AddElem Then
            Dim tmp As New ElemAddForm
            tmp.Show()

        ElseIf TSB Is ToolStripButton_DeleteElem Then
            Dim selectIDs As New List(Of Integer)
            For i As Integer = 0 To ListView_Elements.SelectedItems.Count - 1
                selectIDs.Add(CInt(ListView_Elements.SelectedItems(i).Text))
            Next
            Elements.Delete(selectIDs, Nodes.ElementNodes(selectIDs))

        ElseIf TSB Is ToolStripButton_deleteNode Then
            Dim selectIDs As New List(Of Integer)
            For i As Integer = 0 To ListView_Nodes.SelectedItems.Count - 1
                selectIDs.Add(CInt(ListView_Nodes.SelectedItems(i).Text))
            Next
            Nodes.Delete(selectIDs)

        ElseIf TSB Is Toolstripbutton_deleteforce Then
            Dim selectIDs As New List(Of Integer)
            Dim ZeroForces As New List(Of Double()) 'holds zero values to reset each force to 0
            For i As Integer = 0 To ListView_Forces.SelectedItems.Count - 1
                selectIDs.Add(CInt(ListView_Forces.SelectedItems(i).Text))
                ZeroForces.Add({0, 0, 0})
            Next
            Nodes.Addforce(ZeroForces, selectIDs)


        ElseIf TSB Is TSButton_AddNode Then
            Dim tmp As New NodeAddForm

        ElseIf TSB Is TSButton_EditNode Then
            If ListView_Nodes.SelectedItems.Count > 0 Then
                Dim tmp As New NodeAddForm(CInt(ListView_Nodes.SelectedItems(0).Text))
            End If

        ElseIf TSB Is ToolStripButton_AddForce Then
            Dim tmp As New ForceForm
        End If
    End Sub

    Private Sub PopulateNodeList()
        ListView_Nodes.Items.Clear()

        If Nodes.Nodelist IsNot Nothing Then
            For Each Node As NodeManager.Node In Nodes.Nodelist
                Dim li As New ListViewItem
                li.Text = CStr(Node.ID)
                li.SubItems.Add("X: " & CStr(Math.Round(Node.Coords_mm(0), 3)) & ", Y: " & CStr(Math.Round(Node.Coords_mm(1), 3)) & ", Z: " & CStr(Math.Round(Node.Coords_mm(1), 3)))
                li.SubItems.Add(String.Join(",", Node.AttachedElements))

                Dim tmp As New List(Of String)
                If Node.Fixity(0) = 1 Then
                    tmp.Add("X")
                End If
                If Node.Fixity(1) = 1 Then
                    tmp.Add("Y")
                End If
                If Node.Fixity(2) = 1 Then
                    tmp.Add("Z")
                End If
                li.SubItems.Add(String.Join(",", tmp))

                li.SubItems.Add("U: " & CStr(Math.Round(Node.Displacement(0) * 1000, 5)) & ", V: " & CStr(Math.Round(Node.Displacement(1) * 1000, 5)) & ", W: " & CStr(Math.Round(Node.Displacement(2) * 1000, 5))) 'convert to mm
                li.SubItems.Add("X: " & CStr(Math.Round(Node.ReactionForce(0), 3)) & ", Y: " & CStr(Math.Round(Node.ReactionForce(1), 3)) & ", Z: " & CStr(Math.Round(Node.ReactionForce(2), 3)))
                ListView_Nodes.Items.Add(li)
            Next
        End If
    End Sub
    Private Sub PopulateElemList()
        ListView_Elements.Items.Clear()

        If Elements.Elemlist IsNot Nothing Then
            For Each Elem As ElementManager.Element_Bar1 In Elements.Elemlist

                Dim elemnodes As NodeManager.Node() = Nodes.ElementNodes(Elem.ID)

                Dim li As New ListViewItem
                li.Text = CStr(Elem.ID)
                li.SubItems.Add(CStr(Math.Round(Elem.Length(elemnodes) * 1000, 3))) 'convert to mm
                li.SubItems.Add(CStr(Math.Round(Elem.A * 1000 * 1000, 3))) 'convert to mm^2
                li.SubItems.Add(CStr(Math.Round((Elem.E / (1000.0 * 1000 * 1000)), 3))) 'convert to GPa
                li.SubItems.Add(CStr(elemnodes(0).ID) & "," & CStr(elemnodes(1).ID))
                li.SubItems.Add(CStr(Math.Round(Elem.Stress(elemnodes) / (1000 * 1000), 3))) 'convert to MPa
                ListView_Elements.Items.Add(li)
            Next
        End If
    End Sub
    Private Sub PopulateForceList()
        ListView_Forces.Items.Clear()

        If Nodes.Nodelist IsNot Nothing Then
            For Each Node As NodeManager.Node In Nodes.Nodelist
                If Node.Force(0) <> 0 Or Node.Force(1) <> 0 Or Node.Force(2) <> 0 Then 'dont show nodes with no force
                    Dim li As New ListViewItem
                    li.Text = CStr(Node.ID)
                    li.SubItems.Add(CStr(Math.Round(Node.ForceMagnitude, 5)))
                    li.SubItems.Add("X: " & CStr(Math.Round(Node.ForceDirection(0), 3)) & ", Y: " & CStr(Math.Round(Node.ForceDirection(1), 3)) & ", Z: " & CStr(Math.Round(Node.ForceDirection(1), 3)))
                    li.SubItems.Add("X: " & CStr(Math.Round(Node.Force(0), 3)) & ", Y: " & CStr(Math.Round(Node.Force(1), 3)) & ", Z: " & CStr(Math.Round(Node.Force(1), 3)))

                    ListView_Forces.Items.Add(li)
                End If
            Next
        End If

    End Sub
    Public Sub RedrawLists()
        PopulateNodeList()
        PopulateElemList()
        PopulateForceList()
    End Sub

    Public Sub RedrawScreen()
        view.Refresh(GlControl_Main)
    End Sub
    Private Sub OrienationUpdated(ByVal Trans As Double(), ByVal Rot As Double(), ByVal zoom As Double) Handles view.ViewUpdated
        ToolStripStatusLabel1.Text = "(X: " & Trans(0) & ", Y:" & Trans(1) & ", Z:" & Trans(2) & ")"
        ToolStripStatusLabel2.Text = "(RX: " & Rot(0) & ", RY:" & Rot(1) & ")"
        ToolStripStatusLabel3.Text = "(Zoom: " & zoom & ")"

        'draw any objects
        coord.Draw()

        If animationPercent = -1 Then
            For Each node As NodeManager.Node In Nodes.Nodelist
                node.Draw()

                If drawReactions Then
                    node.DrawReaction(node.Coords_mm)
                End If
            Next

            For Each Elem As ElementManager.Element_Bar1 In Elements.Elemlist
                Elem.Draw(Nodes.ElementNodes(Elem.ID))
            Next
        Else
            For Each node As NodeManager.Node In Nodes.Nodelist
                node.Draw_Manual(node.CalcDispIncrementPos_mm(animationPercent, 1000))
                If drawReactions Then
                    node.DrawReaction(node.CalcDispIncrementPos_mm(animationPercent, 1000))
                End If
            Next

            For Each Elem As ElementManager.Element_Bar1 In Elements.Elemlist
                Elem.Draw_manual(Nodes.ElementNodes(Elem.ID)(0).CalcDispIncrementPos_mm(animationPercent, 1000), Nodes.ElementNodes(Elem.ID)(1).CalcDispIncrementPos_mm(animationPercent, 1000))
            Next
        End If
        'Finally...
        GraphicsContext.CurrentContext.SwapInterval = True
        GlControl_Main.SwapBuffers() 'Takes from the 'GL' and puts into control  
    End Sub


    Private Sub ListView_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListView_Nodes.ItemSelectionChanged, ListView_Elements.ItemSelectionChanged, ListView_Forces.ItemSelectionChanged
        Nodes.SelectNodes(Nodes.AllIDs.ToArray, False) 'first assume none have been selected
        Elements.SelectElems(Elements.AllIDs.ToArray, False)

        '------------------------ NODES & Forces--------------------

        If ListView_Nodes.SelectedItems.Count > 0 Or ListView_Forces.SelectedItems.Count > 0 Then 'if some have been selected then get ids to list and select
            Dim selectIDs As New List(Of Integer)
            For i As Integer = 0 To ListView_Nodes.SelectedItems.Count - 1 'handle nodes
                selectIDs.Add(CInt(ListView_Nodes.SelectedItems(i).Text))
            Next

            For i As Integer = 0 To ListView_Forces.SelectedItems.Count - 1 'handle forces
                selectIDs.Add(CInt(ListView_Forces.SelectedItems(i).Text))
            Next
            Nodes.SelectNodes(selectIDs.ToArray, True)
        End If

        '------------------------ Elements--------------------

        If ListView_Elements.SelectedItems.Count > 0 Then 'if some have been selected then get ids to list and select
            Dim selectIDs As New List(Of Integer)
            For i As Integer = 0 To ListView_Elements.SelectedItems.Count - 1
                selectIDs.Add(CInt(ListView_Elements.SelectedItems(i).Text))
            Next
            Elements.SelectElems(selectIDs.ToArray, True)
        End If

        '------------------------ Finish--------------------

    End Sub
    Private Sub ListView_LostFocus(sender As Object, e As EventArgs) Handles ListView_Nodes.LostFocus, ListView_Elements.LostFocus, ListView_Forces.LostFocus
        Dim LV As ListView = sender
        LV.SelectedItems.Clear()
    End Sub



    Private Sub ToolStripButton_Solve_Click(sender As Object, e As EventArgs) Handles ToolStripButton_Solve.Click

        Dim output As DenseMatrix() = Solve.Solve(Solve.Assemble_K_Mtx(Elements.Elemlist, Nodes.Nodelist), Nodes.F_Mtx, Nodes.Q_Mtx)

        Dim outputstr1 As New List(Of String)
        Dim outputstr2 As New List(Of String)

        For i As Integer = 0 To output(0).RowCount - 1
            For j As Integer = 0 To output(0).ColumnCount - 1
                outputstr1.Add(CStr(output(0).Item(i, j)))
            Next
        Next

        For i As Integer = 0 To output(0).RowCount - 1
            For j As Integer = 0 To output(0).ColumnCount - 1
                outputstr2.Add(CStr(output(1).Item(i, j)))
            Next
        Next

        Nodes.SetSolution(output(0), output(1))

        'MessageBox.Show(String.Join(",", outputstr1))
        'MessageBox.Show(String.Join(",", outputstr2))
    End Sub
    Private Sub CheckBox_DrawReactions_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_DrawReactions.CheckedChanged
        If CheckBox_DrawReactions.Checked Then
            drawReactions = True
        Else
            drawReactions = False
        End If
        RedrawScreen()
    End Sub
    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar_AnimateDisplacement.Scroll
        animationPercent = TrackBar_AnimateDisplacement.Value / 100.0

        If TrackBar_AnimateDisplacement.Value = 0 Then
            animationPercent = -1
        End If
        RedrawScreen()
    End Sub

    Private Sub ToolStripMenuItem_save_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem_save.Click
        Dim fd As New SaveFileDialog
        fd.Filter = "Txt File|*.txt"
        fd.FileName = "Study1.txt"

        If fd.ShowDialog = Windows.Forms.DialogResult.OK Then
            StringListToTextFile(ExportData(Nodes.Nodelist, Elements.Elemlist), fd.FileName)
        End If
    End Sub
    Private Sub ToolStripMenuItem_load_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem_load.Click
        Dim fb As New OpenFileDialog
        fb.Filter = "Txt File|*.txt"
        fb.FileName = "Study1"

        If fb.ShowDialog = Windows.Forms.DialogResult.OK Then
            ImportData(TextFileToStringList(fb.FileName))
        End If
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Dim fb As New OpenFileDialog
        fb.Filter = "Txt File|*.txt"
        fb.FileName = "Study1"

        If fb.ShowDialog = Windows.Forms.DialogResult.OK Then
            STLtoVertexes(fb.FileName)
        End If
    End Sub
End Class
