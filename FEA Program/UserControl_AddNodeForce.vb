Public Class UserControl_AddNodeForce

    Private _DOFs As Integer = -1
    Private _DOFNames As String() = {"X", "Y", "Z"}
    Private _Nodes As List(Of NodeMgr.Node)

    Private _YIncrement As Integer = 50
    Private _FirstLabelPos As Integer() = {4, 46}
    Private _FirstTextBoxPos As Integer() = {6, 66}

    Private _Labels As New List(Of Label)
    Private _TxtBoxes As New List(Of NumericalInputTextBox)


    Public Event NodeForceAddFormSuccess(ByVal sender As UserControl_AddNodeForce, ByVal Forces As List(Of Double()), ByVal NodeIDs As List(Of Integer))
    Public Event NodeSelectionUpdated(ByVal sender As Object, ByVal SelectedNodeIDs As List(Of Integer))

    Public Sub New(ByVal NodeDOFs As Integer, ByVal Nodes As List(Of NodeMgr.Node))

        InitializeComponent()

        _DOFs = NodeDOFs
        _Nodes = Nodes


        '-------------------- add force component textboxes ------------------

        For I As Integer = 0 To NodeDOFs - 1
            '-------------------- add label ------------------
            Dim LB As New Label

            If I < _DOFNames.Length Then
                LB.Text = _DOFNames(I) & " Component (N)"
            Else
                LB.Text = "#" 'handles potential error case where names run out
            End If

            LB.Width = 100
            LB.Height = 13
            LB.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            LB.Location = New Point(_FirstLabelPos(0), _FirstLabelPos(1) + _YIncrement * I)

            _Labels.Add(LB)
            Me.Controls.Add(LB)

            '-------------------- add textbox ------------------

            Dim txt As New NumericalInputTextBox(100, New Point(_FirstTextBoxPos(0), _FirstTextBoxPos(1) + _YIncrement * I), Units.DataUnitType.Force, Units.AllUnits.N)

            txt.Tag = I
            _TxtBoxes.Add(txt)
            Me.Controls.Add(txt)

        Next

        '----------------- Move Checklist down -----------------------

        CheckedListBox_ApplyNodes.Location = New Point(CheckedListBox_ApplyNodes.Location.X, CheckedListBox_ApplyNodes.Location.Y + _YIncrement * _DOFs)
        CheckedListBox_ApplyNodes.Height = CheckedListBox_ApplyNodes.Height - _YIncrement * _DOFs

        Label2.Location = New Point(Label2.Location.X, Label2.Location.Y + _YIncrement * _DOFs)


        '------------------ populate node checklist --------------------

        For Each Node As NodeMgr.Node In _Nodes
            Dim text As String = CStr(Node.ID) & " - (" & String.Join(",", Node.Coords_mm) & ")"

            CheckedListBox_ApplyNodes.Items.Add(text)

        Next

        ValidateEntry(Nothing, Nothing)
    End Sub 'sets up input forms
    Private Sub ButtonAccept_Click(sender As Object, e As EventArgs) Handles Button_Accept.Click, Button_Cancel.Click
        Try
            Dim sendbtn As Button = sender

            If sendbtn Is Button_Accept Then

                Dim SelectedNodeIDs As List(Of Integer) = GetCheckedListBoxNodeIds() 'get the node IDs

                Dim ForceComponents As New List(Of Double)
                For i As Integer = 0 To _DOFs - 1
                    ForceComponents.Add(_TxtBoxes(i).Value) 'get the force components depending how many DOFs are available
                Next

                Dim CopiedForceComponents As New List(Of Double()) 'need to copy the force for each node ID to get the right input format
                For i As Integer = 0 To SelectedNodeIDs.Count - 1
                    CopiedForceComponents.Add(ForceComponents.ToArray)
                Next

                RaiseEvent NodeSelectionUpdated(Me, New List(Of Integer)) 'deselect all nodes
                RaiseEvent NodeForceAddFormSuccess(Me, CopiedForceComponents, SelectedNodeIDs)
            End If

            Me.Dispose()
        Catch ex As Exception
            MessageBox.Show("Error adding force: " & ex.Message)
        End Try
    End Sub
    Private Sub ValidateEntry(sender As Object, e As EventArgs) Handles CheckedListBox_ApplyNodes.MouseUp
        Try
            If CheckedListBox_ApplyNodes.CheckedItems.Count = 0 Then 'need to select at least 1 node to add
                Throw New Exception
            End If

            Button_Accept.Enabled = True 'if this works for all then everything is ok
        Catch ex As Exception
            Button_Accept.Enabled = False
        End Try
    End Sub

    Private Sub CheckedListBox_ApplyNodes_Click(sender As Object, e As EventArgs) Handles CheckedListBox_ApplyNodes.MouseUp
        Try
            Dim SelectedNodeIDs As List(Of Integer) = GetCheckedListBoxNodeIds()

            RaiseEvent NodeSelectionUpdated(Me, SelectedNodeIDs)
        Catch ex As Exception
            MessageBox.Show("Error occurred when attempting to highlight selected node.")
        End Try
    End Sub
    Private Function GetCheckedListBoxNodeIds() As List(Of Integer)
        Dim SelectedNodeIDs As New List(Of Integer)

        For Each TextValue As String In CheckedListBox_ApplyNodes.CheckedItems
            If TextValue IsNot Nothing Then 'no selection in combobox
                Dim NODEID As Integer = CInt(TextValue.Split(" ").First) 'split based on previous formatting to get node id
                SelectedNodeIDs.Add(NODEID)
            End If
        Next

        Return SelectedNodeIDs
    End Function

    Private Sub UserControl_AddElement_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyData = Keys.Enter & Button_Accept.Enabled Then
            Button_Accept.PerformClick()
        End If
    End Sub

End Class


  




  



