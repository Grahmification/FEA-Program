Public Class UserControl_AddNode

    Private _DOFs As Integer = -1
    Private _DOFNames As String() = {"X", "Y", "Z"}

    Private _YIncrement As Integer = 25
    Private _FirstLabelPos As Integer() = {6, 36 + _YIncrement}
    Private _FirstTextBoxPos As Integer() = {30, 34 + _YIncrement}
    Private _FirstCheckBoxPos As Integer() = {145, 32 + _YIncrement}

    Private _TxtBoxes As New List(Of NumericalInputTextBox)
    Private _ChkBoxes As New List(Of CheckBox)

    Public Event NodeAddFormSuccess(ByVal sender As UserControl_AddNode, ByVal Coords As List(Of Double()), ByVal Fixity As List(Of Integer()), ByVal Dimensions As List(Of Integer))

    Public Sub New(ByVal NodeDOFs As Integer)
        InitializeComponent()

        _DOFs = NodeDOFs

        For I As Integer = 0 To NodeDOFs - 1
            '-------------------- add label ------------------
            Dim LB As New Label

            If I < _DOFNames.Length Then
                LB.Text = _DOFNames(I)
            Else
                LB.Text = "#"
            End If

            LB.Width = 10
            LB.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            LB.Location = New Point(_FirstLabelPos(0), _FirstLabelPos(1) + _YIncrement * I)
            Me.Controls.Add(LB)


            '---------------- add textbox ---------------------
            Dim txt As New NumericalInputTextBox(100, New Point(_FirstTextBoxPos(0), _FirstTextBoxPos(1) + _YIncrement * I), Units.DataUnitType.Length, Units.AllUnits.mm)
            'txt.Text = "0.000"
            'txt.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right
            'txt.Location = New Point(_FirstTextBoxPos(0), _FirstTextBoxPos(1) + _YIncrement * I)
            Me.Controls.Add(txt)
            txt.Tag = I
            _TxtBoxes.Add(txt)

            'AddHandler txt.TextChanged, AddressOf ValidateEntry
            'AddHandler txt.MouseClick, AddressOf TextBox_MouseClick

            '-------------- add checkbox -------------------

            Dim CK As New CheckBox
            CK.Checked = False
            CK.Anchor = AnchorStyles.Top Or AnchorStyles.Right
            CK.Location = New Point(_FirstCheckBoxPos(0), _FirstCheckBoxPos(1) + _YIncrement * I)
            Me.Controls.Add(CK)
            CK.Tag = I
            _ChkBoxes.Add(CK)

            '------------ move buttons

            Button_FixAll.Location = New Point(Button_FixAll.Location.X, Button_FixAll.Location.Y + _YIncrement)
            Button_UnfixAll.Location = New Point(Button_UnfixAll.Location.X, Button_UnfixAll.Location.Y + _YIncrement)

        Next

    End Sub 'sets up input forms


    Private Sub ButtonAccept_Click(sender As Object, e As EventArgs) Handles Button_Accept.Click, Button_Cancel.Click
        Try
            Dim sendbtn As Button = sender

            If sendbtn Is Button_Accept Then
                Dim coords(_DOFs - 1) As Double
                Dim fixity(_DOFs - 1) As Integer

                For i As Integer = 0 To _DOFs - 1
                    coords(i) = _TxtBoxes(i).Value

                    If _ChkBoxes(i).Checked Then
                        fixity(i) = 1
                    Else
                        fixity(i) = 0
                    End If
                Next

                Dim tmpCoords As New List(Of Double())
                Dim tmpfixity As New List(Of Integer())
                Dim tmpDim As New List(Of Integer)

                tmpCoords.Add(coords)
                tmpfixity.Add(fixity)
                tmpDim.Add(_DOFs)

                RaiseEvent NodeAddFormSuccess(Me, tmpCoords, tmpfixity, tmpDim)
            End If

            Me.Dispose()
        Catch ex As Exception
            MessageBox.Show("Error adding node: " & ex.Message)
        End Try
    End Sub
    Private Sub ButtonAccept_Click_OLD(sender As Object, e As EventArgs)
        Try
            Dim sendbtn As Button = sender

            If sendbtn Is Button_Accept Then
                Dim coords(_DOFs - 1) As Double
                Dim fixity(_DOFs - 1) As Integer

                For i As Integer = 0 To _DOFs - 1
                    coords(i) = CDbl(_TxtBoxes(i).Text) / 1000.0 'convert to m

                    If _ChkBoxes(i).Checked Then
                        fixity(i) = 1
                    Else
                        fixity(i) = 0
                    End If
                Next

                Dim tmpCoords As New List(Of Double())
                Dim tmpfixity As New List(Of Integer())
                Dim tmpDim As New List(Of Integer)

                tmpCoords.Add(coords)
                tmpfixity.Add(fixity)
                tmpDim.Add(_DOFs)

                RaiseEvent NodeAddFormSuccess(Me, tmpCoords, tmpfixity, tmpDim)
            End If
        Catch ex As Exception
            MessageBox.Show("Error adding node:" & ex.Message)
        End Try
        Me.Dispose()
    End Sub

    Private Sub Button_FixFloat_Click(sender As Object, e As EventArgs) Handles Button_FixAll.Click, Button_UnfixAll.Click
        Try
            Dim btn As Button = sender

            If sender Is Button_FixAll Then
                For Each Ck As CheckBox In _ChkBoxes
                    Ck.Checked = True
                Next
            Else
                For Each Ck As CheckBox In _ChkBoxes
                    Ck.Checked = False
                Next
            End If
        Catch ex As Exception

        End Try
    End Sub

    'Private Sub TextBox_MouseClick(sender As Object, e As MouseEventArgs)
    'Dim sendtxt As TextBox = sender
    'sendtxt.SelectAll()
    'End Sub

    Private Sub ValidateEntry(sender As Object, e As EventArgs)
        Try
            For Each txt As TextBox In _TxtBoxes
                Dim tmp As Double = CDbl(txt.Text)
            Next

            Button_Accept.Enabled = True 'if this works for all then everything is ok
        Catch ex As Exception
            Button_Accept.Enabled = False
        End Try
    End Sub

 

End Class
