Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input
Imports MathNet.Numerics.LinearAlgebra.Double
Public Class Mainform

    Public Coord As New CoordinateSystem(50)
    Public Coord2 As New CoordinateSystem(25)
    Public GlCont As New GLControlMod(Color.Black, True)


    Private P As StressProblem = Nothing

    Private Sub Mainform_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            PopulateProblemTypeBox(ToolStripComboBox_ProblemMode, GetType(StressProblem.ProblemTypes))

            Dim tmp As New CodeInputComboBox(P.CommandList)
            SplitContainer_Main.Panel2.Controls.Add(tmp)
            tmp.Show()
            tmp.Dock = DockStyle.Bottom


            GlCont.Dock = DockStyle.Fill
            SplitContainer_Main.Panel2.Controls.Add(GlCont)

            AddHandler GlCont.DrawStuffAfterOrientation, AddressOf DrawStuff
            AddHandler GlCont.DrawStuffBeforeOrientation, AddressOf DrawStuffBefore
            AddHandler GlCont.ViewUpdated, AddressOf ViewUpdated

        Catch ex As Exception

        End Try
    End Sub

    Public Sub DrawStuff(ByVal ThreeDimensional As Boolean)

        Coord.Draw(ThreeDimensional)

        For Each N As NodeMgr.Node In P.Nodes.Nodelist
            N.DrawNode(N.Coords_mm)
        Next

        For Each E As ElementMgr.IElement In P.Elements.Elemlist
            Dim nodeCoords As New List(Of Double())
            For Each NodeID As Integer In P.Connect.ElementNodes(E.ID)
                nodeCoords.Add(P.Nodes.NodeObj(NodeID).Coords_mm)
            Next

            E.Draw(nodeCoords)
        Next

        'SpriteBatch.DrawArrow(10, New Vector3(1, 1, 1), New Vector3(1, 1, 1), True, Color.Green, 3)
        'GL.LoadMatrix(GlCont.Orientation)
        'Coord2.Draw(ThreeDimensional)
    End Sub
    Public Sub DrawStuffBefore(ByVal ThreeDimensional As Boolean)

    End Sub
    Public Sub ViewUpdated(ByVal Trans As Vector3, ByVal rot As Matrix4, ByVal zoom As Vector3, ByVal ThreeDimensional As Boolean)

        Dim rotationAngles As Vector3 = New Vector3(Math.Atan2(rot.M23, rot.M33), Math.Atan2(-1 * rot.M31, Math.Sqrt(rot.M23 * rot.M23 + rot.M33 * rot.M33)), Math.Atan2(rot.M21, rot.M11))
        rotationAngles *= 180 / Math.PI

        If ThreeDimensional Then
            ToolStripStatusLabel_Trans.Text = "(X: " & Math.Round(Trans.X, 1) & ", Y:" & Math.Round(Trans.Y, 1) & ", Z:" & Math.Round(Trans.Z, 1) & ")"
            ToolStripStatusLabel_Rot.Text = "(RX: " & Math.Round(rotationAngles.X, 1) & ", RY:" & Math.Round(rotationAngles.Y, 1) & ", RZ:" & Math.Round(rotationAngles.Z, 1) & ")"
            ToolStripStatusLabel_Zoom.Text = "(Zoom: " & Math.Round(zoom.X, 1) & ")"
        Else
            ToolStripStatusLabel_Trans.Text = "(X: " & Math.Round(Trans.X, 1) & ", Y:" & Math.Round(Trans.Y, 1) & ")"
            ToolStripStatusLabel_Rot.Text = ""
            ToolStripStatusLabel_Zoom.Text = "(Zoom: " & Math.Round(zoom.X, 1) & ")"
        End If

        

    End Sub


    Public Sub ReDrawLists()
        TreeView_Main.Nodes.Clear()

        '---------------------- Add base level ------------------------

        Dim Baselevel As String() = {"Nodes", "Elements", "Forces", "Materials"}


        For i As Integer = 0 To Baselevel.Length - 1
            Dim N As New TreeNode(Baselevel(i))
            TreeView_Main.Nodes.Add(N)
        Next

        '------------------ Add nodes --------------------

        For i As Integer = 0 To P.Nodes.Nodelist.Count - 1
            Dim tmpNode As NodeMgr.Node = P.Nodes.Nodelist(i)
            Dim N As New TreeNode("Node " & tmpNode.ID)
            N.Tag = tmpNode.ID
            N.ContextMenuStrip = ContextMenuStrip_Treeview

            TreeView_Main.Nodes(0).Nodes.Add(N)
            TreeView_Main.Nodes(0).Nodes(i).Nodes.Add(New TreeNode("Pos: " & String.Join(",", tmpNode.Coords_mm.ToArray)))
            TreeView_Main.Nodes(0).Nodes(i).Nodes.Add(New TreeNode("Fixity: " & String.Join(",", tmpNode.Fixity.ToArray)))
        Next

        '------------------ Add Materials --------------------

        For i As Integer = 0 To P.Materials.AllIDs.Count - 1
            Dim mat As MaterialMgr.MaterialClass = P.Materials.MatObj(P.Materials.AllIDs(i))
            Dim N As New TreeNode(mat.Name)
            N.Tag = mat.ID

            TreeView_Main.Nodes(3).Nodes.Add(N)
            TreeView_Main.Nodes(3).Nodes(i).Nodes.Add(New TreeNode("E (GPa): " & CStr(mat.E_GPa)))
            TreeView_Main.Nodes(3).Nodes(i).Nodes.Add(New TreeNode("V : " & CStr(mat.V)))
            TreeView_Main.Nodes(3).Nodes(i).Nodes.Add(New TreeNode("Sy (MPa) : " & CStr(mat.Sy_MPa)))
            TreeView_Main.Nodes(3).Nodes(i).Nodes.Add(New TreeNode("Sut (MPa) : " & CStr(mat.Sut_MPa)))
            TreeView_Main.Nodes(3).Nodes(i).Nodes.Add(New TreeNode("Type : " & CStr([Enum].GetName(GetType(MaterialMgr.MaterialType), mat.Subtype))))
        Next

        '------------------ Add Elements --------------------

        For i As Integer = 0 To P.Elements.Elemlist.Count - 1
            Dim tmpElem As ElementMgr.IElement = P.Elements.Elemlist(i)
            Dim N As New TreeNode("Element " & tmpElem.ID)
            N.Tag = tmpElem.ID

            TreeView_Main.Nodes(1).Nodes.Add(N)
            TreeView_Main.Nodes(1).Nodes(i).Nodes.Add(New TreeNode("Type: " & ElementMgr.Name(tmpElem.MyType)))
            'TreeView_Main.Nodes(1).Nodes(i).Nodes.Add(New TreeNode("Area: " & CStr(Units.Convert(Units.AllUnits.m_squared, tmpElem.a, Units.AllUnits.mm_squared)) & Units.UnitStrings(Units.AllUnits.mm_squared)(0)))
            TreeView_Main.Nodes(1).Nodes(i).Nodes.Add(New TreeNode("Material: " & P.Materials.MatObj(tmpElem.Material).Name))

            Dim nodeCoords As New List(Of Double())
            For Each NodeID As Integer In P.Connect.ElementNodes(tmpElem.ID)
                nodeCoords.Add(P.Nodes.NodeObj(NodeID).Coords)
            Next

            TreeView_Main.Nodes(1).Nodes(i).Nodes.Add(New TreeNode("Length: " & tmpElem.Length(nodeCoords)))

        Next

    End Sub



    '-------------------- Upper Toolstrip --------------------------
    Private Sub PopulateProblemTypeBox(ByRef TSCB As ToolStripComboBox, ByVal T As Type)
        TSCB.Items.Clear()

        For Each Val As String In System.Enum.GetNames(T)
            TSCB.Items.Add(Val)
        Next

        TSCB.SelectedIndex = 0
    End Sub
    Private Sub ToolStripComboBox_ProblemMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox_ProblemMode.SelectedIndexChanged
        Try
            Dim TSCB As ToolStripComboBox = sender
            P = New StressProblem(Me, TSCB.SelectedIndex)
            GlCont.ThreeDimensional = P.ThreeDimensional

            ReDrawLists()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ToolStripButton_Addnode_Click(sender As Object, e As EventArgs) Handles ToolStripButton_Addnode.Click
        Try
            Dim uc As New UserControl_AddNode(P.AvailableNodeDOFs)
            AddHandler uc.NodeAddFormSuccess, AddressOf NodeAddFormSuccess

            AddUserControlToSplitCont(uc, SplitContainer_Main, 1)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub NodeAddFormSuccess(ByVal sender As UserControl_AddNode, ByVal Coords As List(Of Double()), ByVal Fixity As List(Of Integer()), ByVal Dimensions As List(Of Integer))
        RemoveHandler sender.NodeAddFormSuccess, AddressOf NodeAddFormSuccess
        P.Nodes.AddNodes(Coords, Fixity, Dimensions)
    End Sub

    Private Sub ToolStripButton_AddMaterial_Click(sender As Object, e As EventArgs) Handles ToolStripButton_AddMaterial.Click
        Try
            Dim UC As New UserControl_AddMaterial(GetType(MaterialMgr.MaterialType))
            AddHandler UC.MatlAddFormSuccess, AddressOf MatlAddFormSuccess

            AddUserControlToSplitCont(UC, SplitContainer_Main, 1)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub MatlAddFormSuccess(ByVal sender As UserControl_AddMaterial, Name As String, ByVal E_GPa As Double, ByVal V As Double, ByVal Sy_MPa As Double, ByVal Sut_MPa As Double, ByVal subtype As Integer)
        RemoveHandler sender.MatlAddFormSuccess, AddressOf MatlAddFormSuccess
        P.Materials.Add(Name, E_GPa, V, Sy_MPa, Sut_MPa, subtype)
    End Sub

    Private Sub ToolStripButton_AddElement_Click(sender As Object, e As EventArgs) Handles ToolStripButton_AddElement.Click
        Try
            Dim ElementArgsList As New List(Of Dictionary(Of String, Units.DataUnitType))

            For Each ElemType As Type In P.AvailableElements
                ElementArgsList.Add(ElementMgr.ElementArgs(ElemType))
            Next


            Dim UC As New UserControl_AddElement(P.AvailableElements, ElementArgsList, P.Materials.MatList, P.Nodes.Nodelist)
            AddHandler UC.ElementAddFormSuccess, AddressOf ElemAddFormSuccess
            AddHandler UC.NodeSelectionUpdated, AddressOf FeatureAddFormNodeSelectionChanged
            AddUserControlToSplitCont(UC, SplitContainer_Main, 1)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ElemAddFormSuccess(ByVal sender As UserControl_AddElement, ByVal Type As Type, ByVal NodeIDs As List(Of Integer), ByVal ElementArgs As Double(), ByVal Mat As Integer)

        RemoveHandler sender.ElementAddFormSuccess, AddressOf ElemAddFormSuccess
        RemoveHandler sender.NodeSelectionUpdated, AddressOf FeatureAddFormNodeSelectionChanged

        P.Elements.Add(Type, NodeIDs, ElementArgs, Mat)

    End Sub

    Private Sub ToolStripButton_AddNodeForce_Click(sender As Object, e As EventArgs) Handles ToolStripButton_AddNodeForce.Click
        Try

            Dim UC As New UserControl_AddNodeForce(P.AvailableNodeDOFs, P.Nodes.Nodelist)
            AddHandler UC.NodeForceAddFormSuccess, AddressOf NodeForceAddFormSuccess
            AddHandler UC.NodeSelectionUpdated, AddressOf FeatureAddFormNodeSelectionChanged
            AddUserControlToSplitCont(UC, SplitContainer_Main, 1)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub NodeForceAddFormSuccess(ByVal sender As UserControl_AddNodeForce, ByVal Forces As List(Of Double()), ByVal NodeIDs As List(Of Integer))

        RemoveHandler sender.NodeForceAddFormSuccess, AddressOf NodeForceAddFormSuccess
        RemoveHandler sender.NodeSelectionUpdated, AddressOf FeatureAddFormNodeSelectionChanged

        P.Nodes.Addforce(Forces, NodeIDs)
    End Sub


    Private Sub FeatureAddFormNodeSelectionChanged(ByVal sender As Object, ByVal SelectedNodeIDs As List(Of Integer))
        P.Nodes.SelectNodes(P.Nodes.AllIDs.ToArray, False)
        If SelectedNodeIDs.Count > 0 Then
            P.Nodes.SelectNodes(SelectedNodeIDs.ToArray, True)
        End If
    End Sub

    Private Sub AddUserControlToSplitCont(ByVal UC As UserControl, ByVal SCont As SplitContainer, ByVal SPanel As Integer)

        If SPanel = 1 Then
            SCont.Panel1.Controls.Add(UC)
            SCont.Panel1MinSize = UC.Width
        Else 'panel 2
            SCont.Panel2.Controls.Add(UC)
            SCont.Panel2MinSize = UC.Width
        End If

        UC.BringToFront()
        UC.Dock = DockStyle.Fill
    End Sub

    Private Sub TreeView_Main_NodeMouseClick(sender As Object, e As TreeViewEventArgs) Handles TreeView_Main.AfterSelect
        Dim Tree As TreeView = sender

        If Input.buttonPress(Windows.Forms.MouseButtons.Left) Then
            'If e.Node.Level = 1 Then
            ' Dim FirstLevel As String = e.Node.FullPath.Split("\").First()
            Dim NodeIDs As New List(Of Integer)
            Dim ElemIDs As New List(Of Integer)

            For Each N As TreeNode In Tree.Nodes(0).Nodes
                If N.IsSelected Then
                    NodeIDs.Add(N.Tag)
                End If
            Next

            For Each N As TreeNode In Tree.Nodes(1).Nodes
                If N.IsSelected Then
                    ElemIDs.Add(N.Tag)
                End If
            Next

            P.Nodes.SelectNodes(P.Nodes.AllIDs.ToArray, False)
            P.Elements.SelectElems(P.Elements.AllIDs.ToArray, True)


            P.Nodes.SelectNodes(NodeIDs.ToArray, True)
            P.Elements.SelectElems(ElemIDs.ToArray, True)

            'End If
        ElseIf Input.buttonDown(Windows.Forms.MouseButtons.Right) Then
        End If

    End Sub

    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        Dim Node As TreeNode = sender

        MessageBox.Show(Node.FullPath)


    End Sub


    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click

        Dim NodeDOFS As New Dictionary(Of Integer, Integer)
        For Each NodeID As Integer In P.Nodes.AllIDs
            NodeDOFS.Add(NodeID, P.Nodes.NodeObj(NodeID).Dimension)
        Next

        Dim K_assembled As sparsematrix = P.Connect.Assemble_K_Mtx(P.Elements.K_matricies(P.Connect.ConnectMatrix, P.Nodes.NodeCoords, P.Materials.All_E), NodeDOFS)
        Dim output As DenseMatrix() = P.Connect.Solve(K_assembled, P.Nodes.F_Mtx, P.Nodes.Q_Mtx)


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



        MessageBox.Show(String.Join(",", outputstr1))
        MessageBox.Show(String.Join(",", outputstr2))
    End Sub
End Class


Public Class CodeInputComboBox
    Inherits ComboBox

    Private CommandCollection As New AutoCompleteStringCollection
    Private _CommandList As New Dictionary(Of String, List(Of Type))(StringComparer.OrdinalIgnoreCase) 'case insensitivity

    Public Event CommandEntered(ByVal CommandName As String, ByVal Arguments As List(Of Object))

    Public Sub New(ByVal CommandList As Dictionary(Of String, List(Of Type))) 'string is command name, type list holds argument types

        For Each KVP As KeyValuePair(Of String, List(Of Type)) In CommandList 'copy so case insensitivity doesn't get wrecked by outside
            _CommandList.Add(KVP.Key, KVP.Value)
        Next

        For Each CMD As KeyValuePair(Of String, List(Of Type)) In CommandList
            Dim SB As String = CMD.Key & "("

            If CMD.Value IsNot Nothing Then
                For i As Integer = 1 To CMD.Value.Count - 1
                    SB = SB & ","
                Next
            End If

            SB = SB & ")"

            CommandCollection.Add(SB)
        Next

        Me.AutoCompleteSource = Windows.Forms.AutoCompleteSource.CustomSource
        Me.AutoCompleteCustomSource = CommandCollection
        Me.AutoCompleteMode = Windows.Forms.AutoCompleteMode.SuggestAppend

        UserLostFocus()
    End Sub

    Private Sub KeyPressed(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Try
            If e.KeyCode = Keys.Enter Then
                Dim data As String = Me.Text
                data = RemoveWhitespace(data) 'remove any spaces
                Dim commandName As String = data.Split("(").First

                If _CommandList.Keys.Contains(commandName, StringComparer.CurrentCultureIgnoreCase) = False Then 'check if command is in command list
                    Throw New Exception
                End If

                Dim Arguments As String() = data.Split("(").Last.Split(")").First.Split(",")
                Dim outputArgs As New List(Of Object)

                If _CommandList(commandName) IsNot Nothing Then 'only need to do if we have arguments
                    If Arguments.Count <> _CommandList(commandName).Count Then 'check for correct number of arguments
                        Throw New Exception
                    End If

                    For i As Integer = 0 To Arguments.Count - 1 'check args are correct type
                        Select Case _CommandList(commandName)(i)

                            Case GetType(Integer)
                                Dim tmp As Integer = CInt(Arguments(i))
                            Case GetType(Double)
                                Dim tmp As Double = CDbl(Arguments(i))
                            Case GetType(String)
                                Dim tmp As String = CStr(Arguments(i))
                            Case GetType(Boolean)
                                Dim tmp As Boolean = CBool(Arguments(i))
                        End Select

                        outputArgs.Add(Arguments(i))
                    Next
                End If

                Me.BackColor = Color.White
                Me.Items.Add(Me.Text)
                RaiseEvent CommandEntered(commandName, outputArgs)
            End If
        Catch ex As Exception
            Me.BackColor = Color.IndianRed
        End Try
    End Sub

    Private Sub UserLostFocus() Handles MyBase.LostFocus
        If Me.Text = "" Then
            Me.Text = "Type a command..."
        End If
    End Sub
    Private Sub UserGotFocus() Handles MyBase.GotFocus
        If Me.Text = "Type a command..." Then
            Me.Text = ""
        End If
    End Sub


    Private Function RemoveWhitespace(fullString As String) As String
        Return New String(fullString.Where(Function(x) Not Char.IsWhiteSpace(x)).ToArray())
    End Function 'removes any spaces from string

End Class
Public Class NumericalInputTextBox
    Inherits TextBox

    Private _unitType As Units.DataUnitType
    Private _DefaultInputUnit As Units.AllUnits

    Public ReadOnly Property Value As Double
        Get
            If CheckInput() = False Then
                Me.SelectAll()
                Throw New Exception("Incorrect value <" & Me.Text & "> entered into textbox.")
            End If

            Return ConvertInput()
        End Get
    End Property

    Public Sub New(ByVal Width As Integer, ByVal Location As Point, ByVal unitType As Units.DataUnitType, ByVal DefaultInputUnit As Units.AllUnits)
        _unitType = unitType
        _DefaultInputUnit = DefaultInputUnit

        If DefaultInputUnit = -1 Or unitType = -1 Then
            Me.Text = "0.000"
        Else
            Me.Text = "0.000" & Units.UnitStrings(DefaultInputUnit)(0)
        End If

        Me.Width = Width
        Me.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right
        Me.Location = Location

    End Sub

    Public Sub New()

    End Sub

    Private Function ValidateText() As Boolean Handles MyBase.TextChanged
        If CheckInput() = True Then
            Me.BackColor = Color.White
            Return True
        Else
            Me.BackColor = Color.IndianRed
            Return False
        End If
    End Function

    Private Sub TextBox_MouseClick(sender As Object, e As EventArgs) Handles MyBase.Click
        Dim sendtxt As TextBox = sender
        sendtxt.SelectAll()
    End Sub

    Private Function CheckInput() As Boolean
        Try
            Dim mytext As String = Me.Text
            Dim unitIdentifier As String = ""

            If _unitType <> -1 And _DefaultInputUnit <> -1 Then
                Dim allowedUnitStrings As List(Of String) = Units.TypeUnitStrings(_unitType)

                For i As Integer = 0 To allowedUnitStrings.Count - 1
                    If mytext.Contains(allowedUnitStrings(i)) Then
                        unitIdentifier = allowedUnitStrings(i)
                        Exit For
                    End If
                Next

                If unitIdentifier <> "" Then
                    mytext = mytext.Replace(unitIdentifier, "")
                End If
            End If
           
            Dim inputData_dbl As Double = CDbl(mytext)

            'Me.Text = CStr(Math.Round(Units.Convert(Units.UnitEnums(unitIdentifier), inputData_dbl, _DefaultInputUnit), 5)) & Units.UnitStrings(_DefaultInputUnit)(0)

            Return True

        Catch ex As Exception
            Return False
        End Try
    End Function
    Private Function ConvertInput() As Double
        Dim mytext As String = Me.Text
        Dim unitIdentifier As String = ""
        Dim output As Double

        If _unitType = -1 Or _DefaultInputUnit = -1 Then
            output = CDbl(Me.Text)
        Else

            Dim allowedUnitStrings As List(Of String) = Units.TypeUnitStrings(_unitType)

            For i As Integer = 0 To allowedUnitStrings.Count - 1
                If mytext.Contains(allowedUnitStrings(i)) Then
                    unitIdentifier = allowedUnitStrings(i)
                    Exit For
                End If
            Next

            If unitIdentifier <> "" Then
                mytext = mytext.Replace(unitIdentifier, "")
                output = Units.Convert(Units.UnitEnums(unitIdentifier), CDbl(mytext), Units.DefaultUnit(_unitType)) 'convert to the default type
            Else
                output = Units.Convert(_DefaultInputUnit, CDbl(mytext), Units.DefaultUnit(_unitType)) 'if no specific type specified need to convert from the default input type
            End If
        End If

        Return output
    End Function

End Class