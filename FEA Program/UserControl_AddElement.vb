Public Class UserControl_AddElement

    Private _AvailableElemTypes As Type()
    Private _MatIDs As New List(Of Integer)
    Private _Nodes As List(Of NodeMgr.Node)
    Private _ElementArgs As List(Of Dictionary(Of String, Units.DataUnitType))

    Private _YIncrement As Integer = 50
    Private _FirstLabelPos As Integer() = {8, 96 + _YIncrement}
    Private _FirstComboBoxPos As Integer() = {7, 115 + _YIncrement}
    Private _FirstTextBoxPos As Integer() = {7, 115 + _YIncrement}

    Private _ComboBoxes As New List(Of ComboBox)
    Private _Labels As New List(Of Label)
    Private _TxtBoxes As New List(Of NumericalInputTextBox)

    Private nodeCollection As New AutoCompleteStringCollection

    Public Event ElementAddFormSuccess(ByVal sender As UserControl_AddElement, Type As Type, ByVal NodeIDs As List(Of Integer), ByVal ElementArgs As Double(), ByVal Mat As Integer)
    Public Event NodeSelectionUpdated(ByVal sender As Object, ByVal SelectedNodeIDs As List(Of Integer))

    Public Sub New(AvailableElemTypes As Type(), ByVal ElementArgs As List(Of Dictionary(Of String, Units.DataUnitType)), ByVal Mats As List(Of MaterialMgr.MaterialClass), ByVal Nodes As List(Of NodeMgr.Node))

        InitializeComponent()

        _AvailableElemTypes = AvailableElemTypes
        _ElementArgs = ElementArgs
        _Nodes = Nodes


        '-------------------- add element types ------------------

        With ComboBox_ElemType
            For Each i As Type In AvailableElemTypes
                .Items.Add(ElementMgr.Name(i))
            Next

            If .Items.Count > 0 Then
                .SelectedIndex = 0
            End If
        End With

        '--------------- add materials --------------------

        With ComboBox_Material
            For Each m As MaterialMgr.MaterialClass In Mats
                .Items.Add(m.Name)
                _MatIDs.Add(m.ID)
            Next

            If .Items.Count > 0 Then
                .SelectedIndex = 0
            End If
        End With

    End Sub 'sets up input forms
    Private Sub ButtonAccept_Click(sender As Object, e As EventArgs) Handles Button_Accept.Click, Button_Cancel.Click
        Try
            Dim sendbtn As Button = sender

            If sendbtn Is Button_Accept Then

                Dim ElemType As Type = _AvailableElemTypes(ComboBox_ElemType.SelectedIndex)


                Dim NodeIDs As New List(Of Integer)
                For Each CBX As ComboBox In _ComboBoxes
                    NodeIDs.Add(CInt(CBX.Text.Split(" ").First))
                Next


                Dim ElementArgs As New List(Of Double)
                For Each txt As NumericalInputTextBox In _TxtBoxes
                    ElementArgs.Add(txt.Value)
                Next


                Dim MatID As Integer = _MatIDs(ComboBox_Material.SelectedIndex)

                RaiseEvent ElementAddFormSuccess(Me, ElemType, NodeIDs, ElementArgs.ToArray, MatID)
            End If

            Me.Dispose()
        Catch ex As Exception
            MessageBox.Show("Error adding element: " & ex.Message)
        End Try
    End Sub
    Private Sub ValidateEntry(sender As Object, e As EventArgs) Handles ComboBox_ElemType.SelectedIndexChanged, ComboBox_Material.SelectedIndexChanged
        Try
            If ComboBox_ElemType.Items.Count = 0 Or ComboBox_Material.Items.Count = 0 Or _Nodes.Count = 0 Then
                Throw New Exception
            End If

            Dim NodeCBoxIndexes As New List(Of Integer)

            For Each Cbox As ComboBox In _ComboBoxes
                NodeCBoxIndexes.Add(Cbox.SelectedIndex)
            Next

            If AllIndexesDifferent(NodeCBoxIndexes) = False Then
                Throw New Exception
            End If


            Button_Accept.Enabled = True 'if this works for all then everything is ok
        Catch ex As Exception
            Button_Accept.Enabled = False
        End Try
    End Sub



    Private Sub ComboBox_ElemType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_ElemType.SelectedIndexChanged
        Dim CBX As ComboBox = sender

        '------------------ First remove any previous boxes --------------------------

        For Each LB As Label In _Labels
            LB.Dispose()
        Next
        _Labels.Clear()

        For Each CB As ComboBox In _ComboBoxes
            RemoveHandler CB.SelectedIndexChanged, AddressOf ValidateEntry
            RemoveHandler CB.SelectedIndexChanged, AddressOf NodeComboBoxSelectionChanged 'for node selection event
            CB.Dispose()
        Next
        _ComboBoxes.Clear()

        For Each txt As TextBox In _TxtBoxes
            txt.Dispose()
        Next
        _TxtBoxes.Clear()

        '------------------ Add new boxes for each node ----------------

        For I As Integer = 0 To ElementMgr.NumOfNodes(_AvailableElemTypes(CBX.SelectedIndex)) - 1

            Dim LB As New Label
            LB.Text = "Node " & CStr(I + 1)
            LB.Width = 80
            LB.Height = 16
            LB.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            LB.Location = New Point(_FirstLabelPos(0), _FirstLabelPos(1))
            _Labels.Add(LB)
            Me.Controls.Add(LB)

            Dim CBox As New ComboBox
            CBox.Width = 196
            CBox.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right
            CBox.Location = New Point(_FirstComboBoxPos(0), _FirstComboBoxPos(1))
            CBox.Tag = I
            CBox.AutoCompleteCustomSource = nodeCollection
            AddHandler CBox.SelectedIndexChanged, AddressOf ValidateEntry

            _ComboBoxes.Add(CBox)
            Me.Controls.Add(CBox)

            _FirstLabelPos(1) += _YIncrement
            _FirstComboBoxPos(1) += _YIncrement
            _FirstTextBoxPos(1) += _YIncrement
        Next

        '--------------------- Populate Node Comboboxes -------------------------

        For Each Node As NodeMgr.Node In _Nodes
            Dim text As String = CStr(Node.ID) & " - (" & String.Join(",", Node.Coords_mm) & ")"

            nodeCollection.Add(text)

            For Each CB As ComboBox In _ComboBoxes
                CB.Items.Add(text)
            Next
        Next

        '------------------------------------Add handler for node combobox highlight selection event -------------
        'do after population so not firing over and over

        For Each Cbox As ComboBox In _ComboBoxes
            AddHandler Cbox.SelectedIndexChanged, AddressOf NodeComboBoxSelectionChanged 'for node selection event
        Next

        '-------------------- Add TextBoxes for Element Arguments -------------

        For Each Arg As KeyValuePair(Of String, Units.DataUnitType) In _ElementArgs(CBX.SelectedIndex)
            Dim LB As New Label
            LB.Text = Arg.Key
            LB.Width = 80
            LB.Height = 16
            LB.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            LB.Location = New Point(_FirstLabelPos(0), _FirstLabelPos(1))
            _Labels.Add(LB)
            Me.Controls.Add(LB)

            Dim txt As New NumericalInputTextBox(196, New Point(_FirstTextBoxPos(0), _FirstTextBoxPos(1)), Arg.Value, Units.AllUnits.mm_squared)
            _TxtBoxes.Add(txt)
            Me.Controls.Add(txt)

            _FirstTextBoxPos(1) += _YIncrement
            _FirstLabelPos(1) += _YIncrement
        Next

    End Sub

    Private Function AllIndexesDifferent(ByVal Indexes As List(Of Integer)) As Boolean

        For i As Integer = 0 To Indexes.Count - 1
            For j As Integer = 0 To Indexes.Count - 1
                If i <> j And Indexes(i) = Indexes(j) Then
                    Return False
                End If
            Next
        Next

        Return True
    End Function

    Private Sub UserControl_AddElement_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyData = Keys.Enter & Button_Accept.Enabled Then
            Button_Accept.PerformClick()
        End If
    End Sub

    Private Sub NodeComboBoxSelectionChanged(sender As Object, e As EventArgs)
        Try
            Dim SelectedNodeIDs As New List(Of Integer)

            For Each CB As ComboBox In _ComboBoxes
                Dim Data As String = CB.SelectedItem

                If Data IsNot Nothing Then 'no selection in combobox
                    Dim NODEID As Integer = CInt(Data.Split(" ").First) 'split based on previous formatting to get node id
                    SelectedNodeIDs.Add(NODEID)
                End If
            Next

            RaiseEvent NodeSelectionUpdated(Me, SelectedNodeIDs)
        Catch ex As Exception
            MessageBox.Show("Error occurred when attempting to highlight selected node.")
        End Try
    End Sub 'raises selection event to highlight nodes


End Class
