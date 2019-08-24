Public Class UserControl_AddMaterial

    Private _EnumValues As New List(Of Integer)
    Private _TxtBoxes As New List(Of NumericalInputTextBox)

    Private _YIncrement As Integer = 50
    Private _FirstLabelPos As Integer() = {6, 96}
    Private _FirstTextBoxPos As Integer() = {5, 115}

    Public Event MatlAddFormSuccess(ByVal sender As UserControl_AddMaterial, Name As String, ByVal E_GPa As Double, ByVal V As Double, ByVal Sy_MPa As Double, ByVal Sut_MPa As Double, ByVal subtype As Integer)

    Public Sub New(ByVal MatlSubTypes As Type)
        InitializeComponent()

        '-------------------- Set Up Combobox -----------------------

        For Each Val As Integer In System.Enum.GetValues(MatlSubTypes)
            _EnumValues.Add(Val)
        Next

        For Each Val As String In System.Enum.GetNames(MatlSubTypes)
            ComboBox_SubTypes.Items.Add(Val)
        Next

        If ComboBox_SubTypes.Items.Count > 0 Then
            ComboBox_SubTypes.SelectedIndex = 0
        End If

        '------------------- Set up textboxes ----------------------

        Dim labelText As String() = {"E (GPa)", "V", "Sy (MPa)", "Sut (MPa)"}

        For Each S As String In labelText
            Dim LB As New Label
            LB.Text = S
            LB.Width = 80
            LB.Height = 16
            LB.Anchor = AnchorStyles.Left Or AnchorStyles.Top
            LB.Location = New Point(_FirstLabelPos(0), _FirstLabelPos(1))
            Me.Controls.Add(LB)


            Dim unitType As Integer = Units.DataUnitType.Pressure
            Dim DefaultInput As Integer = Units.AllUnits.MPa

            If S = "V" Then
                unitType = -1
                DefaultInput = -1
            ElseIf S = "E (GPa)" Then
                DefaultInput = Units.AllUnits.GPa
            End If

            Dim txt As New NumericalInputTextBox(185, New Point(_FirstTextBoxPos(0), _FirstTextBoxPos(1)), unitType, DefaultInput)
            _TxtBoxes.Add(txt)
            Me.Controls.Add(txt)

            _FirstTextBoxPos(1) += _YIncrement
            _FirstLabelPos(1) += _YIncrement
        Next

    End Sub 'sets up input forms
    Private Sub ButtonAccept_Click(sender As Object, e As EventArgs) Handles Button_Accept.Click, Button_Cancel.Click
        Try
            Dim sendbtn As Button = sender

            If sendbtn Is Button_Accept Then

                Dim _name As String = TextBox_Name.Text
                Dim _E As Double = _TxtBoxes(0).Value
                Dim _V As Double = _TxtBoxes(1).Value
                Dim _Sy As Double = _TxtBoxes(2).Value
                Dim _Sut As Double = _TxtBoxes(3).Value
                Dim _Type As Integer = _EnumValues(ComboBox_SubTypes.SelectedIndex)

                RaiseEvent MatlAddFormSuccess(Me, _name, _E, _V, _Sy, _Sut, _Type)
            End If

            Me.Dispose()
        Catch ex As Exception
            MessageBox.Show("Error adding material: " & ex.Message)
        End Try
    End Sub
    Private Sub ValidateEntry(sender As Object, e As EventArgs) Handles TextBox_Name.TextChanged
        Try
            If TextBox_Name.Text = "" Then
                Throw New Exception("Invalid Name")
            End If

            If ComboBox_SubTypes.Items.Count = 0 Then
                Throw New Exception("No Subtypes")
            End If

            Button_Accept.Enabled = True 'if this works for all then everything is ok
        Catch ex As Exception
            Button_Accept.Enabled = False
        End Try
    End Sub
    Private Sub UserControl_AddElement_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyData = Keys.Enter & Button_Accept.Enabled Then
            Button_Accept.PerformClick()
        End If
    End Sub



End Class
