Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports MathNet.Numerics.LinearAlgebra.Double
Imports System.IO
Imports System.Xml.Serialization
Imports System.Reflection
Public Class Development

    Shared _SelectedColor As Color = Color.Yellow ' the default color of selected objects for the program

    Public WithEvents Nodes As New NodeMgr
    Public WithEvents Elements As New ElementMgr
    Public WithEvents Problem As New Connectivity
    Public Materials As New List(Of Material)
    Public Loadedform As Mainform = Nothing
    'Public Solve As New Solver

    Private Sub ListRedrawNeeded() Handles Nodes.NodeListChanged, Nodes.NodeChanged, Elements.ElementListChanged, Elements.ElementChanged
        Loadedform.RedrawLists()
        Loadedform.RedrawScreen()
    End Sub
    Private Sub ScreenRedrawOnlyNeeded() Handles Nodes.NodeChanged_RedrawOnly, Elements.ElementChanged_RedrawOnly
        Loadedform.RedrawScreen()
    End Sub

    Private Sub HangingElements(ByVal NodeID As Integer, ByVal Dimension As Integer) Handles Nodes.NodeDeleted

        Dim ElementsToDelete As List(Of Integer) = Problem.NodeElements(NodeID)
        Elements.Delete(ElementsToDelete)

    End Sub 'deletes elements if a node is deleted and leaves one hanging
    Private Sub ElementCreation(ByVal ElemID As Integer, ByVal NodeIDs As List(Of Integer), ByVal Type As ElementMgr.ElementType) Handles Elements.ElementAdded

        '---------------------- Get Coords of all of the nodes in the element and then sort Ids

        Dim NodeCoords As New List(Of Double())

        For Each ID As Integer In NodeIDs
            NodeCoords.Add(Nodes.NodeObj(ID).Coords())
        Next

        Elements.ElemObj(ElemID).SortNodeOrder(NodeIDs, NodeCoords) 'nodeIDs is passed byref

        Problem.AddConnection(ElemID, NodeIDs)
    End Sub
    Private Sub ElementDeletion(ByVal ElemID As Integer, ByVal Type As ElementMgr.ElementType) Handles Elements.ElementDeleted
        Problem.RemoveConnection(ElemID)
    End Sub



    Public Class MaterialClass
        Private _Name As String = ""
        Private _E As Double = 0 ' youngs modulus in Pa
        Private _V As Double = 0 ' poissons ratio
        Private _Sy As Double = 0 ' yield strength in Pa
        Private _Sut As Double = 0 ' ultimate strength in Pa
        Private _subtype As MaterialType

        Public ReadOnly Property Name As String
            Get
                Return _Name
            End Get
        End Property
        Public ReadOnly Property Sy_MPa As Double
            Get
                Return _Sy / (1000.0 * 1000.0)
            End Get
        End Property
        Public ReadOnly Property Sut_MPa As Double
            Get
                Return _Sut / (1000.0 * 1000.0)
            End Get
        End Property
        Public ReadOnly Property E_GPa As Double
            Get
                Return (_E / (1000 * 1000 * 1000)) 'convert to GPa
            End Get
        End Property
        Public ReadOnly Property E As Double
            Get
                Return _E
            End Get
        End Property
        Public ReadOnly Property V As Double
            Get
                Return _V
            End Get
        End Property
        Public ReadOnly Property Subtype As MaterialType
            Get
                Return _subtype
            End Get
        End Property
        Public ReadOnly Property D_matrix_2D() As DenseMatrix
            Get
                Dim output As New DenseMatrix(3, 3)
                output.Clear() ' set all vals to 0

                output(0, 0) = 1
                output(0, 1) = _V
                output(1, 0) = _V
                output(1, 1) = 1
                output(2, 2) = (1 - _V) / 2

                output = output * _E / (1 - _V * _V)

                Return output
            End Get
        End Property

        Public Sub New(ByVal Name As String, ByVal E_GPa As Double, ByVal V As Double, ByVal Sy_MPa As Double, ByVal Sut_MPa As Double, ByVal subtype As MaterialType)

            Name = _Name
            _E = E_GPa * 1000 * 1000 * 1000 'convert to Pa
            _V = V
            _Sy = Sy_MPa * 1000 * 1000 'convert to Pa
            _Sut = Sut_MPa * 1000 * 1000 'convert to Pa
            _subtype = subtype
        End Sub

        Public Enum MaterialType
            Steel_Alloy
            Aluminum_Alloy
        End Enum

    End Class
    Public Class CoordinateSystem

        Private originX As Double = 0
        Private originY As Double = 0
        Private originZ As Double = 0
        Private _scale As Integer = 1
        Private SysColor As Color = Color.Teal

        Public Sub New(ByVal Scale As Double)
            Me._scale = Scale
        End Sub
        Public Sub Draw(ByVal Two_Dimensional As Boolean)

            'X Axis
            GL.Color3(SysColor)
            GL.Begin(PrimitiveType.Lines)
            GL.Vertex3(originX, originY, originZ)
            GL.Vertex3(originX + _scale, originY, originZ)
            GL.Vertex3(originX + _scale, originY, originZ)
            GL.Vertex3(originX + _scale * 0.9, originY + _scale * 0.1, originZ)
            GL.Vertex3(originX + _scale, originY, originZ)
            GL.Vertex3(originX + _scale * 0.9, originY + _scale * -0.1, originZ)

            'letter x
            GL.Vertex3(originX + _scale * 0.9, originY + _scale * 0.25, originZ)
            GL.Vertex3(originX + _scale, originY + _scale * 0.35, originZ)
            GL.Vertex3(originX + _scale, originY + _scale * 0.15, originZ)
            GL.Vertex3(originX + _scale * 0.8, originY + _scale * 0.35, originZ)
            GL.Vertex3(originX + _scale * 0.9, originY + _scale * 0.25, originZ)
            GL.Vertex3(originX + _scale * 0.8, originY + _scale * 0.15, originZ)

            'Y Axis

            GL.Vertex3(originX, originY, originZ)
            GL.Vertex3(originX, originY + _scale, originZ)
            GL.Vertex3(originX, originY + _scale, originZ)
            GL.Vertex3(originX + _scale * 0.1, originY + _scale * 0.9, originZ)
            GL.Vertex3(originX, originY + _scale, originZ)
            GL.Vertex3(originX + _scale * -0.1, originY + _scale * 0.9, originZ)

            'letter y
            GL.Vertex3(originX + _scale * 0.15, originY + _scale, originZ)
            GL.Vertex3(originX + _scale * 0.25, originY + _scale * 0.9, originZ)
            GL.Vertex3(originX + _scale * 0.25, originY + _scale * 0.9, originZ)
            GL.Vertex3(originX + _scale * 0.35, originY + _scale, originZ)
            GL.Vertex3(originX + _scale * 0.25, originY + _scale * 0.9, originZ)
            GL.Vertex3(originX + _scale * 0.25, originY + _scale * 0.8, originZ)

            If Two_Dimensional = False Then 'don't draw Z-axis if 2D
                'Z Axis
                GL.Vertex3(originX, originY, originZ)
                GL.Vertex3(originX, originY, originZ + _scale)
                GL.Vertex3(originX, originY, originZ + _scale)
                GL.Vertex3(originX + _scale * 0.1, originY, originZ + _scale * 0.9)
                GL.Vertex3(originX, originY, originZ + _scale)
                GL.Vertex3(originX + _scale * -0.1, originY, originZ + _scale * 0.9)

                'letter Z
                GL.Vertex3(originX + _scale * 0.15, originY, originZ + _scale)
                GL.Vertex3(originX + _scale * 0.35, originY, originZ + _scale)
                GL.Vertex3(originX + _scale * 0.15, originY, originZ + _scale)
                GL.Vertex3(originX + _scale * 0.35, originY, originZ + _scale * 0.8)
                GL.Vertex3(originX + _scale * 0.35, originY, originZ + _scale * 0.8)
                GL.Vertex3(originX + _scale * 0.15, originY, originZ + _scale * 0.8)

            End If

            GL.End()
        End Sub


    End Class

    Public Class Connectivity

        Private _ConnectMatrix As New Dictionary(Of Integer, List(Of Integer)) 'dict key is global element ID, list index is local node ID, list value at index is global node ID

        Public ReadOnly Property ElementNodes(ByVal ElementID As Integer) As List(Of Integer)
            Get
                Return _ConnectMatrix(ElementID)
            End Get
        End Property 'returns global Node ID's utilized in input element
        Public ReadOnly Property NodeElements(ByVal NodeID As Integer) As List(Of Integer)
            Get
                Dim output As New List(Of Integer)

                For Each KVP As KeyValuePair(Of Integer, List(Of Integer)) In _ConnectMatrix
                    If KVP.Value.Contains(NodeID) Then 'check if the nodeID is used in the element
                        output.Add(KVP.Key) 'if so add the element ID to the output
                    End If
                Next

                output.Sort() 'sort the output for good measure (lowest element ID comes first)
                Return output
            End Get
        End Property 'returns all of the element ID's attached to a global node ID

        Public Sub AddConnection(ByVal ElementID As Integer, ByVal NodeIDs As List(Of Integer))
            _ConnectMatrix.Add(ElementID, NodeIDs)
        End Sub 'nodeIDs need to be sorted in the correct local order for the element
        Public Sub RemoveConnection(ByVal ElementID As Integer)
            _ConnectMatrix.Remove(ElementID)
        End Sub

    End Class 'need to call functions in here from element/node events upon creation/deletion

    Public Class NodeMgr

        Private _Nodes As New Dictionary(Of Integer, Node) 'reference nodes by ID

        Public Event NodeListChanged(ByVal NodeList As Dictionary(Of Integer, Node)) 'Length of nodelist has changed
        Public Event NodeChanged(ByVal IDs As List(Of Integer)) 'Node has changed such that list needs to be updated & screen redrawn
        Public Event NodeChanged_RedrawOnly() 'Node has changed such that screen only needs to be redrawn
        Public Event NodeAdded(ByVal NodeID As Integer, ByVal Dimension As Integer) 'dont use for redrawing lists or screen
        Public Event NodeDeleted(ByVal NodeID As Integer, ByVal Dimension As Integer) 'dont use for redrawing lists or screen

        Public ReadOnly Property Nodelist As List(Of Node)
            Get
                Return _Nodes.Values.ToList
            End Get
        End Property
        Public ReadOnly Property AllIDs As List(Of Integer)
            Get
                Dim output As List(Of Integer) = _Nodes.Keys.ToList
                output.Sort()
                Return output
            End Get
        End Property 'all node ids
        Public ReadOnly Property NodeObj(ByVal ID As Integer) As Node
            Get
                Return _Nodes(ID)
            End Get
        End Property
        Public ReadOnly Property ProblemSize As Integer
            Get
                Dim output As Integer = 0

                For Each Val As Node In _Nodes.Values
                    output += Val.Dimension
                Next

                Return output
            End Get
        End Property 'overall number of node dimensions in the list
        Public ReadOnly Property F_Mtx() As DenseMatrix
            Get
                Dim output As New DenseMatrix(ProblemSize, 1)
                Dim ids As List(Of Integer) = AllIDs
                ids.Sort()

                Dim currentrow As Integer = 0

                For i As Integer = 0 To ids.Count - 1 'iterate through each node
                    For j As Integer = 0 To _Nodes(ids(i)).Dimension - 1 'iterate through each dimension of the node

                        output(currentrow, 0) = _Nodes(ids(i)).Force(j)
                        currentrow += 1
                    Next

                    currentrow += 1
                Next

                Return output
            End Get
        End Property 'output sorted by node ID
        Public ReadOnly Property Q_Mtx() As DenseMatrix
            Get
                Dim output As New DenseMatrix(ProblemSize, 1)
                Dim ids As List(Of Integer) = AllIDs
                ids.Sort()

                Dim currentrow As Integer = 0

                For i As Integer = 0 To ids.Count - 1 'iterate through each node
                    For j As Integer = 0 To _Nodes(ids(i)).Dimension - 1 'iterate through each dimension of the node

                        output(currentrow, 0) = _Nodes(ids(i)).Fixity(j)
                        currentrow += 1
                    Next

                    currentrow += 1
                Next

                Return output
            End Get
        End Property 'output sorted by node ID

        Public Sub SelectNodes(ByVal IDs As Integer(), ByVal selected As Boolean)
            For Each item As Integer In IDs
                _Nodes(item).Selected = selected
            Next
            RaiseEvent NodeChanged_RedrawOnly()
        End Sub
        Public Sub AddNodes(ByVal Coords As List(Of Double()), ByVal Fixity As List(Of Double()), ByVal Dimensions As List(Of Integer))
            If Coords.Count <> Fixity.Count Or Coords.Count <> Dimensions.Count Then
                Throw New Exception("Tried to run sub <" & MethodInfo.GetCurrentMethod.Name & "> with unmatched lengths of input values.")
            End If

            For i As Integer = 0 To Coords.Count - 1
                If ExistsAtLocation(Coords(i)) Then 'dont want to create node where one already is
                    Throw New Exception("Tried to create node at location <" & CStr(Coords(i)(0)) & "," & CStr(Coords(i)(1)) & "," & CStr(Coords(i)(2)) & ">. Node already exists here.")
                End If

                Dim newnode As New Node(Coords(i), Fixity(i), CreateNodeId, Dimensions(i))
                _Nodes.Add(newnode.ID, newnode)
                RaiseEvent NodeAdded(newnode.ID, newnode.Dimension)
            Next

            RaiseEvent NodeListChanged(_Nodes) 'this will redraw so leave it until all have been updated
        End Sub
        Public Sub EditNode(ByVal Coords As List(Of Double()), ByVal fixity As List(Of Double()), ByVal IDs As List(Of Integer))
            If Coords.Count <> fixity.Count Or Coords.Count <> IDs.Count Then
                Throw New Exception("Tried to run sub <" & MethodInfo.GetCurrentMethod.Name & "> with unmatched lengths of input values.")
            End If

            For i As Integer = 0 To IDs.Count - 1
                _Nodes(IDs(i)).Coords = Coords(i)
                _Nodes(IDs(i)).Fixity = fixity(i)
            Next

            RaiseEvent NodeChanged(IDs)
        End Sub
        Public Sub Delete(ByVal IDs As List(Of Integer))

            For Each NodeID As Integer In IDs 'remove node from list
                Dim tmp As Node = _Nodes(NodeID) 'needed to raise event
                _Nodes.Remove(NodeID)

                RaiseEvent NodeDeleted(NodeID, tmp.Dimension)
            Next

            If IDs.Count > 0 Then
                RaiseEvent NodeListChanged(_Nodes)
            End If
        End Sub
        Public Sub SetSolution(ByVal Q As DenseMatrix, ByVal R As DenseMatrix)
            Dim Ids As List(Of Integer) = AllIDs
            Dim currentRow As Integer = 0 'tracks the current row being used from the input matrix

            For i As Integer = 0 To AllIDs.Count - 1 'iterate through each node
                Dim reactions As New List(Of Double)
                Dim displacements As New List(Of Double)

                For j As Integer = 0 To _Nodes(Ids(i)).Dimension - 1 'iterate through each dimension
                    reactions.Add(R(currentRow, 0))
                    displacements.Add(Q(currentRow, 0))

                    currentRow += 1
                Next

                _Nodes(Ids(i)).Solve(displacements.ToArray, reactions.ToArray)
                currentRow += 1
            Next
            RaiseEvent NodeChanged(Ids)
        End Sub
        Public Sub Addforce(ByVal force As List(Of Double()), ByVal IDs As List(Of Integer))
            If force.Count <> IDs.Count Then
                Throw New Exception("Tried to run sub <" & MethodInfo.GetCurrentMethod.Name & "> with unmatched lengths of input values.")
            End If

            For i As Integer = 0 To IDs.Count - 1
                _Nodes(IDs(i)).Force = force(i)
            Next

            RaiseEvent NodeChanged(IDs)
        End Sub
        Public Function ExistsAtLocation(ByVal Coords As Double()) As Boolean
            For Each N As Node In _Nodes.Values
                If N.Coords(0) = Coords(0) And N.Coords(1) = Coords(1) And N.Coords(2) = Coords(2) Then 'node already exists at this location
                    Return True
                End If
            Next

            Return False
        End Function

        Private Function CreateNodeId() As Integer
            Dim NewID As Integer = 1
            Dim IDUnique As Boolean = False

            While _Nodes.Keys.Contains(NewID)
                NewID += 1
            End While

            Return NewID
        End Function

        Public Class Node

            '---------------------- ALL MEMBERS HERE SHOWN FOR 6D NODE, MEMBERS ARE SHORTENED ACCORDINGLY BY DIMENSION

            Private _Coords As Double() = {0, 0, 0, 0, 0, 0} 'coordinates in m
            Private _Disp As Double() = {0, 0, 0, 0, 0, 0} 'displacement in m
            Private _Fixity As Double() = {0, 0, 0, 0, 0, 0} ' 0 = floating, 1 = fixed

            Private _Force As Double() = {0, 0, 0, 0, 0, 0} 'first 3 items  = force [N], last 3 = moments [Nm]
            Private _ReactionForce As Double() = {0, 0, 0, 0, 0, 0} 'reaction force in [N], reaction moments in [Nm]

            Private _ID As Integer = -1
            Private _Dimensions As Integer = 0 '1 = 1D, 2 = 2D, 3 = 3D, 6 = 6D
            Private _ValidDimensions As Integer() = {1, 2, 3, 6} 'provides a list of available dimsensions for error checking

            Private _DefaultColor As Color = Color.Blue
            Private _DefaultForceColor As Color = Color.Purple
            Private _DefaultFixityColor As Color = Color.Red
            Private _FixityColor As Color = Color.Red
            Private _Color As Color = Color.Blue
            Private _ForceColor As Color = Color.Purple
            Private _SelectedColor As Color = Color.Yellow
            Private _ReactionColor As Color = Color.Green

            Private _SolutionValid As Boolean = False

            Public Event SolutionInvalidated(ByVal NodeID As Integer)

            Public Property Selected As Boolean
                Get
                    If _Color = _SelectedColor Then
                        Return True
                    End If
                    Return False
                End Get
                Set(value As Boolean)
                    If value Then
                        _Color = _SelectedColor
                        _ForceColor = _SelectedColor
                        _FixityColor = _SelectedColor
                    Else
                        If _Color = _SelectedColor Then
                            _Color = _DefaultColor
                            _ForceColor = _DefaultForceColor
                            _FixityColor = _DefaultFixityColor
                        End If
                    End If
                End Set
            End Property
            Public ReadOnly Property Coords_mm As Double()
                Get
                    Dim output(_Dimensions) As Double

                    For i As Integer = 0 To _Coords.Length - 1
                        output(i) = _Coords(i) * 1000.0 'convert to mm
                    Next

                    Return output
                End Get
            End Property
            Public Property Coords As Double()
                Get
                    Return _Coords
                End Get
                Set(value As Double())
                    If value.Length <> _Dimensions Then
                        Throw New Exception("Attempted to execute sub <" & MethodInfo.GetCurrentMethod.Name & "> for node ID <" & CStr(_ID) & "> with input params having different dimensions than specified node dimension.")
                    End If

                    _Coords = value
                    InvalidateSolution()
                End Set
            End Property
            Public Property Force As Double()
                Get
                    Return _Force
                End Get
                Set(value As Double())
                    If value.Length <> _Dimensions Then
                        Throw New Exception("Attempted to execute sub <" & MethodInfo.GetCurrentMethod.Name & "> for node ID <" & CStr(_ID) & "> with input params having different dimensions than specified node dimension.")
                    End If

                    _Force = value
                    InvalidateSolution()
                End Set
            End Property
            Public Property Fixity As Double()
                Get
                    Return _Fixity
                End Get
                Set(value As Double())
                    If value.Length <> _Dimensions Then
                        Throw New Exception("Attempted to execute sub <" & MethodInfo.GetCurrentMethod.Name & "> for node ID <" & CStr(_ID) & "> with input params having different dimensions than specified node dimension.")
                    End If

                    _Fixity = value
                    InvalidateSolution()
                End Set
            End Property
            Public ReadOnly Property ForceMagnitude As Double
                Get
                    Dim output As Vector3

                    If _Dimensions = 1 Then
                        output = New Vector3(_Force(0), 0, 0)

                    ElseIf _Dimensions = 2 Then
                        output = New Vector3(_Force(0), _Force(1), 0)

                    Else 'dimensions = 3 or 6
                        output = New Vector3(_Force(0), _Force(1), _Force(2))

                    End If

                    Return output.Length
                End Get
            End Property 'will eventually need moment functions too
            Public ReadOnly Property ForceDirection As Double()
                Get
                    Dim output As Vector3

                    If _Dimensions = 1 Then
                        output = New Vector3(_Force(0), 0, 0)
                        output.Normalize()
                        Return {output.X}

                    ElseIf _Dimensions = 2 Then
                        output = New Vector3(_Force(0), _Force(1), 0)
                        output.Normalize()
                        Return {output.X, output.Y}

                    Else 'dimensions = 3 or 6
                        output = New Vector3(_Force(0), _Force(1), _Force(2))
                        output.Normalize()
                        Return {output.X, output.Y, output.Z}

                    End If
                End Get
            End Property
            Public ReadOnly Property Displacement As Double()
                Get
                    Return _Disp
                End Get
            End Property
            Public ReadOnly Property ReactionForce As Double()
                Get
                    Return _ReactionForce
                End Get
            End Property
            Public ReadOnly Property FinalPos As Double()
                Get
                    Dim output(_Dimensions) As Double

                    For i As Integer = 0 To _Dimensions - 1
                        output(i) = _Coords(i) + _Disp(i) 'add disp to each coord
                    Next

                    Return output
                End Get
            End Property
            Public ReadOnly Property Dimension As Integer
                Get
                    Return _Dimensions
                End Get
            End Property
            Public ReadOnly Property ID As Integer
                Get
                    Return _ID
                End Get
            End Property

            Public Sub New(ByVal NewCoords As Double(), ByVal NewFixity As Double(), ByVal NewID As Integer, ByVal Dimensions As Integer)

                If _ValidDimensions.Contains(Dimensions) = False Then
                    Throw New Exception("Attempted to create element, ID <" & CStr(NewID) & "> with invalid number of dimensions: " & CStr(Dimensions))
                End If

                If NewCoords.Length <> Dimensions Or NewFixity.Length <> Dimensions Then
                    Throw New Exception("Attempted to execute sub <" & MethodInfo.GetCurrentMethod.Name & "> for node ID <" & CStr(_ID) & "> with input params having different dimensions than specified node dimension.")
                End If

                _Coords = NewCoords
                _Fixity = NewFixity
                _ID = NewID
                _Dimensions = Dimensions
            End Sub
            Public Sub Solve(ByVal Disp As Double(), ByVal R As Double())

                If Disp.Length <> _Dimensions Or Disp.Length <> _Dimensions Then
                    Throw New Exception("Attempted to execute sub <" & MethodInfo.GetCurrentMethod.Name & "> for node ID <" & CStr(_ID) & "> with input params having different dimensions than specified node dimension.")
                End If

                _Disp = Disp
                _ReactionForce = R
                _SolutionValid = True
            End Sub
            Public Sub DrawNode(ByVal N_mm As Double())

                If N_mm.Length <> _Dimensions Then
                    Throw New Exception("Attempted to execute sub <" & MethodInfo.GetCurrentMethod.Name & "> for node ID <" & CStr(_ID) & "> with input params having different dimensions than specified node dimension.")
                End If

                DrawNode(N_mm, _Color)
                DrawForce(N_mm, _Force, _ForceColor)
                DrawFixity(N_mm, _Fixity, _FixityColor)
            End Sub 'draw always has mm input
            Public Sub DrawReaction(ByVal N_mm As Double())

                If N_mm.Length <> _Dimensions Then
                    Throw New Exception("Attempted to execute sub <" & MethodInfo.GetCurrentMethod.Name & "> for node ID <" & CStr(_ID) & "> with input params having different dimensions than specified node dimension.")
                End If

                DrawForce(N_mm, _ReactionForce, _ReactionColor)
            End Sub 'draw always has mm input


            Public Function CalcDispIncrementPos_mm(ByVal Percentage As Double, ByVal ScaleFactor As Double) As Double()
                Dim output(_Dimensions) As Double

                For i As Integer = 0 To _Coords.Length - 1
                    output(i) = (_Coords(i) + _Disp(i) * Percentage * ScaleFactor) * 1000.0 'convert to mm
                Next

                Return output
            End Function


            '---------- Draw is not yet setup for 6D nodes ---- need to be able to display rotation displacement

            Private Sub DrawNode(ByVal N As Double(), ByVal Color As Color)

                Dim tmp As Double() = Nothing

                If _Dimensions = 1 Then
                    tmp = {N(0), 0, 0}

                ElseIf _Dimensions = 2 Then
                    tmp = {N(0), N(1), 0}

                Else 'dimensions 3 or 6
                    tmp = N
                End If

                GL.Color3(Color)
                GL.Begin(PrimitiveType.Quads)
                GL.Vertex3(tmp(0) + 1, tmp(1) + 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) + 1, tmp(1) - 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) + 1, tmp(1) - 1, tmp(2) - 1)
                GL.Vertex3(tmp(0) + 1, tmp(1) + 1, tmp(2) - 1)

                GL.Vertex3(tmp(0) - 1, tmp(1) + 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) - 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) - 1, tmp(2) - 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) + 1, tmp(2) - 1)

                GL.Vertex3(tmp(0) + 1, tmp(1) + 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) + 1, tmp(1) - 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) - 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) + 1, tmp(2) + 1)

                GL.Vertex3(tmp(0) + 1, tmp(1) + 1, tmp(2) - 1)
                GL.Vertex3(tmp(0) + 1, tmp(1) - 1, tmp(2) - 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) - 1, tmp(2) - 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) + 1, tmp(2) - 1)

                GL.Vertex3(tmp(0) + 1, tmp(1) + 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) + 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) + 1, tmp(2) - 1)
                GL.Vertex3(tmp(0) + 1, tmp(1) + 1, tmp(2) - 1)

                GL.Vertex3(tmp(0) + 1, tmp(1) - 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) - 1, tmp(2) + 1)
                GL.Vertex3(tmp(0) - 1, tmp(1) - 1, tmp(2) - 1)
                GL.Vertex3(tmp(0) + 1, tmp(1) - 1, tmp(2) - 1)

                GL.End()
            End Sub 'draw always has mm input
            Private Sub DrawForce(ByVal N As Double(), ByVal F As Double(), ByVal Color As Color)
                If Array.TrueForAll(F, AddressOf ValEqualsZero) Then 'dont draw a force if its zero

                    Dim tmp As Double() = Nothing
                    Dim forcelength As Double = 10.0
                    Dim vect As Vector3 = Nothing

                    If _Dimensions = 1 Then
                        tmp = {N(0), 0, 0}
                        vect = New Vector3(F(0), 0, 0)

                    ElseIf _Dimensions = 2 Then
                        tmp = {N(0), N(1), 0}
                        vect = New Vector3(F(0), F(1), 0)

                    Else 'dimensions 3 or 6
                        tmp = N
                        vect = New Vector3(F(0), F(1), F(2))
                    End If

                    vect.Normalize()

                    '------------------- create normal vectors to use for drawing arrows -----------------
                    Dim X As Vector3 = New Vector3(1, 0, 0)
                    Dim normal1 As Vector3 = Vector3.Cross(vect, X)
                    normal1 = Vector3.Multiply(normal1, forcelength * 0.2)

                    Dim Y As Vector3 = New Vector3(0, 1, 0)
                    Dim normal2 As Vector3 = Vector3.Cross(vect, Y)
                    normal2 = Vector3.Multiply(normal2, forcelength * 0.2)

                    '---------------- set arrow to proper length after calculating normals ---------
                    vect = Vector3.Multiply(vect, forcelength)

                    GL.Color3(Color)
                    GL.Begin(PrimitiveType.Lines)

                    GL.Vertex3(tmp(0), tmp(1), tmp(2))
                    GL.Vertex3(tmp(0) + vect.X, tmp(1) + vect.Y, tmp(2) + vect.Z)

                    GL.End()

                    GL.Begin(PrimitiveType.Triangles)
                    Dim endPt As Vector3 = Vector3.Multiply(vect, 0.8)
                    GL.Vertex3(tmp(0) + vect.X, tmp(1) + vect.Y, tmp(2) + vect.Z)
                    GL.Vertex3(tmp(0) + endPt.X + normal1.X, tmp(1) + endPt.Y + normal1.Y, tmp(2) + endPt.Z + normal1.Z)
                    GL.Vertex3(tmp(0) + endPt.X + normal2.X, tmp(1) + endPt.Y + normal2.Y, tmp(2) + endPt.Z + normal2.Z)

                    GL.Vertex3(tmp(0) + vect.X, tmp(1) + vect.Y, tmp(2) + vect.Z)
                    GL.Vertex3(tmp(0) + endPt.X - normal1.X, tmp(1) + endPt.Y - normal1.Y, tmp(2) + endPt.Z - normal1.Z)
                    GL.Vertex3(tmp(0) + endPt.X - normal2.X, tmp(1) + endPt.Y - normal2.Y, tmp(2) + endPt.Z - normal2.Z)

                    GL.Vertex3(tmp(0) + vect.X, tmp(1) + vect.Y, tmp(2) + vect.Z)
                    GL.Vertex3(tmp(0) + endPt.X + normal1.X, tmp(1) + endPt.Y + normal1.Y, tmp(2) + endPt.Z + normal1.Z)
                    GL.Vertex3(tmp(0) + endPt.X - normal2.X, tmp(1) + endPt.Y - normal2.Y, tmp(2) + endPt.Z - normal2.Z)

                    GL.Vertex3(tmp(0) + vect.X, tmp(1) + vect.Y, tmp(2) + vect.Z)
                    GL.Vertex3(tmp(0) + endPt.X - normal1.X, tmp(1) + endPt.Y - normal1.Y, tmp(2) + endPt.Z - normal1.Z)
                    GL.Vertex3(tmp(0) + endPt.X + normal2.X, tmp(1) + endPt.Y + normal2.Y, tmp(2) + endPt.Z + normal2.Z)

                    GL.End()
                End If
            End Sub 'draw always has mm input
            Private Sub DrawFixity(ByVal N As Double(), ByVal Fix As Double(), ByVal Color As Color)
                Dim squareoffset As Double = 1.5
                Dim tmp As Double() = Nothing

                If _Dimensions = 1 Then
                    tmp = {N(0), 0, 0}

                ElseIf _Dimensions = 2 Then
                    tmp = {N(0), N(1), 0}

                Else 'dimensions 3 or 6
                    tmp = N
                End If

                GL.Color3(Color) 'set drawing color


                If Fix(0) = 1 Then 'X Axis
                    GL.Begin(PrimitiveType.Quads)
                    GL.Vertex3(tmp(0), tmp(1) + squareoffset, tmp(2) + squareoffset)
                    GL.Vertex3(tmp(0), tmp(1) - squareoffset, tmp(2) + squareoffset)
                    GL.Vertex3(tmp(0), tmp(1) - squareoffset, tmp(2) - squareoffset)
                    GL.Vertex3(tmp(0), tmp(1) + squareoffset, tmp(2) - squareoffset)
                    GL.End()
                End If

                If _Dimensions > 1 Then 'or else will error when searching for invalid value
                    If Fix(1) = 1 Then 'Y Axis
                        GL.Begin(PrimitiveType.Quads)
                        GL.Vertex3(tmp(0) + squareoffset, tmp(1), tmp(2) + squareoffset)
                        GL.Vertex3(tmp(0) - squareoffset, tmp(1), tmp(2) + squareoffset)
                        GL.Vertex3(tmp(0) - squareoffset, tmp(1), tmp(2) - squareoffset)
                        GL.Vertex3(tmp(0) + squareoffset, tmp(1), tmp(2) - squareoffset)
                        GL.End()
                    End If
                End If

                If _Dimensions > 2 Then 'or else will error when searching for invalid value
                    If Fix(2) = 1 Then 'Z Axis
                        GL.Begin(PrimitiveType.Quads)
                        GL.Vertex3(tmp(0) + squareoffset, tmp(1) + squareoffset, tmp(2))
                        GL.Vertex3(tmp(0) - squareoffset, tmp(1) + squareoffset, tmp(2))
                        GL.Vertex3(tmp(0) - squareoffset, tmp(1) - squareoffset, tmp(2))
                        GL.Vertex3(tmp(0) + squareoffset, tmp(1) - squareoffset, tmp(2))
                        GL.End()
                    End If
                End If
            End Sub 'draw always has mm input
            Private Function ValEqualsZero(ByVal value As Double) As Boolean
                If value = 0 Then
                    Return True
                Else
                    Return False
                End If
            End Function  'used to check if all force values are 0 for drawing


            Private Sub InvalidateSolution()
                _SolutionValid = False
                RaiseEvent SolutionInvalidated(_ID)
            End Sub

        End Class
    End Class
    Public Class ElementMgr

        Private _Bar1Elements As New Dictionary(Of Integer, IElement) 'reference elements by ID

        Public Event ElementListChanged(ByVal ElemList As Dictionary(Of Integer, IElement)) 'Length of Elementlist has changed
        Public Event ElementChanged(ByVal ID As Integer) 'Element has changed such that list needs to be updated & screen redrawn
        Public Event ElementChanged_RedrawOnly() 'Element has changed such that screen only needs to be redrawn
        Public Event ElementAdded(ByVal ElemID As Integer, ByVal NodeIDs As List(Of Integer), ByVal Type As ElementType) 'dont use for redrawing lists or screen
        Public Event ElementDeleted(ByVal ElemID As Integer, ByVal Type As ElementType) 'dont use for redrawing lists or screen

        Public ReadOnly Property Elemlist As List(Of IElement)
            Get
                Return _Bar1Elements.Values.ToList
            End Get
        End Property
        Public ReadOnly Property AllIDs As List(Of Integer)
            Get
                Dim output As List(Of Integer) = _Bar1Elements.Keys.ToList
                output.Sort()
                Return output
            End Get
        End Property
        Public ReadOnly Property ElemObj(ByVal ElemID As String) As IElement
            Get
                Return _Bar1Elements(ElemID)
            End Get
        End Property
        Public ReadOnly Property K_matricies(ByVal ELemIDs As List(Of Integer), ElemNodes As List(Of NodeManager.Node())) As Dictionary(Of Integer, DenseMatrix)
            Get
                Dim output As New Dictionary(Of Integer, DenseMatrix)

                For i As Integer = 0 To ELemIDs.Count - 1
                    'output.Add(ELemIDs(i), _Bar1Elements(ELemIDs(i)).K_mtrx(ElemNodes(i)))
                Next

                Return output
            End Get
        End Property


        Public Sub Add(ByVal Type As ElementType, ByVal NodeIDs As List(Of Integer), ByVal ElementArgs As Double(), Optional ByVal Mat As MaterialClass = Nothing)
            Dim newElem As IElement = Nothing
            Dim newElemID As Integer = CreateId()

            '------------------ Determine type of element

            If Type = ElementType.Bar_linear Then 'linear bar element --------------------------------

                If ElementArgs(0) > 0 Then
                    newElem = New Element_Bar_Linear(ElementArgs(0), newElemID, Mat)
                Else
                    Throw New Exception("Tried to add element <" & Type.ToString & ">, ID <" & newElemID & "> with invalid area:" & CStr(ElementArgs(0)))
                End If

            Else
                Throw New Exception("Tried to add element with invalid type.")
                Return  'dont want to add anything to the list or raise events
            End If

            '-------------------- check for errors and if valid add then raise events -----------------

            If NodeIDs.Count <> newElem.NumOfNodes Then 'check if the right number of nodes are listed
                Throw New Exception("Tried to add element of type <" & Type.ToString & "> with " & CStr(NodeIDs.Count) & " nodes. Should have " & newElem.NumOfNodes & " nodes.")
            End If

            If newElem IsNot Nothing Then 'more error checking
                _Bar1Elements.Add(newElem.ID, newElem)
                RaiseEvent ElementAdded(newElem.ID, NodeIDs, Type)
                RaiseEvent ElementListChanged(_Bar1Elements)
            End If

        End Sub 'nodeIDs only used to raise event about generation
        Public Sub SelectElems(ByVal IDs As Integer(), ByVal selected As Boolean)
            For Each item As Integer In IDs
                _Bar1Elements(item).Selected = selected
            Next
            RaiseEvent ElementChanged_RedrawOnly()
        End Sub
        Public Sub Delete(ByVal IDs As List(Of Integer))
            For i As Integer = 0 To IDs.Count - 1
                Dim tmp As IElement = _Bar1Elements(IDs(i)) 'save temporarily so we can raise event after deletion
                _Bar1Elements.Remove(IDs(i))
                RaiseEvent ElementDeleted(tmp.ID, tmp.Type)
            Next

            If IDs.Count > 0 Then
                RaiseEvent ElementListChanged(_Bar1Elements)
            End If
        End Sub

        Private Function CreateId() As Integer
            Dim NewID As Integer = 1
            Dim IDUnique As Boolean = False

            While _Bar1Elements.Keys.Contains(NewID)
                NewID += 1
            End While

            Return NewID
        End Function


        '-------------------------- Classes & interfaces --------------------------------

        Public Class Element_Bar_Linear
            Inherits Element
            Implements IElement

            Private _Area As Double = 0 'x-section area in m^2
            Private _BodyForce As Double = 0 'Body force in N/m^3
            Private _TractionForce As Double = 0 'Traction force in N/m

            Private ReadOnly Property N_mtrx(ByVal IntrinsicCoords As Double()) As DenseMatrix
                Get
                    If IntrinsicCoords.Length <> 1 Then
                        Throw New Exception("Wrong number of coords input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                    End If

                    Dim eta As Double = IntrinsicCoords(0)

                    Dim N As New DenseMatrix(1, NodeDOFs * NumOfNodes) 'u = Nq - size based off total number of element DOFs
                    N(0, 0) = (1 - eta) / 2.0
                    N(0, 1) = (1 + eta) / 2.0

                    Return N
                End Get
            End Property
            Public ReadOnly Property Interpolated_Displacement(ByVal IntrinsicCoords As Double(), ByVal GblNodeQ As DenseMatrix) Implements IElement.Interpolated_Displacement
                Get
                    If GblNodeQ.Values.Count <> Me.ElemDOFs Then
                        Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                    End If

                    Return Me.N_mtrx(IntrinsicCoords) * GblNodeQ
                End Get
            End Property 'can interpolate either position or displacement
            Public ReadOnly Property B_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix Implements IElement.B_mtrx
                Get
                    If GblNodeCoords.Count <> Me.NumOfNodes Then
                        Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                    End If

                    Dim B_out As New DenseMatrix(1, NodeDOFs * NumOfNodes) 'based from total DOFs
                    B_out(0, 0) = -1.0
                    B_out(0, 1) = 1.0

                    B_out = B_out * (1 / Me.Length(GblNodeCoords))  'B = [-1 1]*1/(x2-x1)

                    Return B_out
                End Get
            End Property 'needs to be given with local node 1 in first spot on list
            Public ReadOnly Property Length(ByVal GblNodeCoords As List(Of Double())) As Double Implements IElement.Length
                Get
                    If GblNodeCoords.Count <> Me.NumOfNodes Then
                        Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                    End If

                    Dim output As Double = GblNodeCoords(1)(0) - GblNodeCoords(0)(0)

                    If output < 0 Then
                        Throw New Exception("Nodes given in wrong order to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                    End If

                    Return output
                End Get
            End Property
            Public ReadOnly Property Stress_mtrx(ByVal GblNodeCoords As List(Of Double()), ByVal GblNodeQ As DenseMatrix, Optional ByVal IntrinsicCoords As Double() = Nothing) As DenseMatrix Implements IElement.Stress_mtrx
                Get
                    If GblNodeCoords.Count <> Me.NumOfNodes Or GblNodeQ.Values.Count <> Me.ElemDOFs Then
                        Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                    End If

                    Dim output As DenseMatrix = Me.Material.E * Me.B_mtrx(GblNodeCoords) * GblNodeQ
                    Return output
                End Get
            End Property 'node 1 displacement comes first in disp input, followed by second
            Public ReadOnly Property K_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix Implements IElement.K_mtrx
                Get
                    If GblNodeCoords.Count <> Me.NumOfNodes Then
                        Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                    End If

                    Dim output As New DenseMatrix(Me.ElemDOFs, Me.ElemDOFs)
                    output(0, 0) = 1
                    output(1, 0) = -1
                    output(0, 1) = -1
                    output(1, 1) = 1

                    output = output * Me.Material.E * _Area / Me.Length(GblNodeCoords)

                    Return output
                End Get
            End Property 'node 1 displacement comes first in disp input, followed by second
            Public ReadOnly Property BodyForce_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix Implements IElement.BodyForce_mtrx
                Get
                    If GblNodeCoords.Count <> Me.NumOfNodes Then
                        Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                    End If

                    Dim output As New DenseMatrix(Me.ElemDOFs, 1)
                    output(0, 0) = 1
                    output(1, 0) = 1

                    output = output * _Area * Me.Length(GblNodeCoords) * _BodyForce * 0.5

                    Return output
                End Get
            End Property 'node 1 displacement comes first in disp input, followed by second
            Public ReadOnly Property TractionForce_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix Implements IElement.TractionForce_mtrx
                Get
                    If GblNodeCoords.Count <> Me.NumOfNodes Then
                        Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                    End If

                    Dim output As New DenseMatrix(Me.ElemDOFs, 1)
                    output(0, 0) = 1
                    output(1, 0) = 1

                    output = output * Me.Length(GblNodeCoords) * _TractionForce * 0.5

                    Return output
                End Get
            End Property

            Public Sub New(ByVal Area As Double, ByVal ID As Integer, Optional ByVal Mat As MaterialClass = Nothing)
                MyBase.New(ElementType.Bar_linear, 2, 1, Drawing.Color.Green, ID, Mat)

                _Area = Area
            End Sub

            Public Sub SortNodeOrder(ByRef NodeIDs As List(Of Integer), ByVal NodeCoords As List(Of Double())) Implements IElement.SortNodeOrder
                If NodeIDs.Count <> Me.NumOfNodes Or NodeCoords.Count <> Me.NumOfNodes Then 'error handling
                    Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ".")
                End If

                Dim SortedIdList As New List(Of Integer)

                If NodeCoords(0)(0) < NodeCoords(1)(0) Then 'larger x-coord is local element 2
                    SortedIdList.Add(NodeIDs(0))
                    SortedIdList.Add(NodeIDs(1))
                Else
                    SortedIdList.Add(NodeIDs(1))
                    SortedIdList.Add(NodeIDs(0))
                End If

                NodeIDs = SortedIdList
            End Sub
            Public Sub SetBodyForce(ByVal ForcePerVol As DenseMatrix) Implements IElement.SetBodyForce
                If ForcePerVol.Values.Count <> Me.NodeDOFs Then 'can only have forces in directions of DOFs
                    Throw New Exception("Wrong number of Forces input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                End If

                _BodyForce = ForcePerVol(0, 0)
                InvalidateSolution()
            End Sub
            Public Sub SetTractionForce(ByVal ForcePerLength As DenseMatrix) Implements IElement.SetTractionForce
                If ForcePerLength.Values.Count <> Me.NodeDOFs Then 'can only have forces in directions of DOFs
                    Throw New Exception("Wrong number of Forces input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                End If

                _BodyForce = ForcePerLength(0, 0)
                InvalidateSolution()
            End Sub
            Public Sub Draw(ByVal GblNodeCoords As List(Of Double())) Implements IElement.Draw

                If GblNodeCoords.Count <> Me.NumOfNodes Then
                    Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                End If

                GL.Begin(PrimitiveType.Lines)

                For i As Integer = 0 To Me.NumOfNodes - 1
                    GL.Color3(_Color(i))
                    GL.Vertex3(GblNodeCoords(i)(0), GblNodeCoords(i)(1), GblNodeCoords(i)(2))
                Next

                GL.End()
            End Sub

        End Class
        Public Class Element
            Implements IBaseElement

            Private _ID As Integer = -1
            Private _Type As ElementType 'type for referencing
            Private _NumOfNodes As Integer 'the number of nodes each element needs to contain
            Private _NodeDOFs As Integer 'the number of DOFs each node in the element will have
            Private _Material As MaterialClass 'contains all material properties

            Private _DefaultColor As Color
            Protected _Color As Color() = Nothing 'want to be able to set a color for each endpoint of the element


            Private _ReadyToSolve As Boolean = False 'is true if the nodes of the element are set up properly
            Protected _SolutionValid As Boolean = False 'is true if the solution for the element is correct

            Public Event SolutionInvalidated(ByVal ElemID As Integer)

            Public ReadOnly Property ID As Integer Implements IBaseElement.ID
                Get
                    Return _ID
                End Get
            End Property
            Public ReadOnly Property Type As ElementType Implements IBaseElement.Type
                Get
                    Return _Type
                End Get
            End Property
            Public ReadOnly Property NumOfNodes As Integer Implements IBaseElement.NumOfNodes
                Get
                    Return _NumOfNodes
                End Get
            End Property
            Public ReadOnly Property NodeDOFs As Integer Implements IBaseElement.NodeDOFs
                Get
                    Return _NodeDOFs
                End Get
            End Property
            Public ReadOnly Property ElemDOFs As Integer Implements IBaseElement.ElemDOFs
                Get
                    Return Me.NumOfNodes * Me.NodeDOFs
                End Get
            End Property

            Public ReadOnly Property SelectColor As Color Implements IBaseElement.SelectColor
                Get
                    Return _SelectedColor
                End Get
            End Property
            Public ReadOnly Property ElemColor As Color() Implements IBaseElement.ElemColor
                Get
                    Return _Color
                End Get
            End Property
            Public ReadOnly Property CornerColor(ByVal LocalNodeID As Integer) As Color Implements IBaseElement.CornerColor
                Get
                    Return _Color(LocalNodeID)
                End Get
            End Property
            Public ReadOnly Property AllCornersSameColor As Boolean Implements IBaseElement.AllCornersSameColor
                Get
                    Dim tmp As Color = _Color(0)

                    For Each c As Color In _Color
                        If c <> tmp Then
                            Return False
                        End If
                    Next
                    Return True
                End Get
            End Property 'checks if all corners are the same color
            Public ReadOnly Property AllCornersEqualColor(ByVal C_in As Color) As Boolean Implements IBaseElement.AllCornersEqualColor
                Get
                    For Each c As Color In _Color
                        If c <> C_in Then
                            Return False
                        End If
                    Next
                    Return True
                End Get
            End Property 'true if all colors are input color

            Public Property Selected As Boolean Implements IBaseElement.Selected
                Get
                    If AllCornersEqualColor(_SelectedColor) Then
                        Return True
                    End If
                    Return False
                End Get
                Set(value As Boolean)
                    If value Then
                        SetColor(_SelectedColor)
                    Else
                        If AllCornersEqualColor(_SelectedColor) Then 'check if the object is actually selected
                            SetColor(_DefaultColor)
                        End If
                    End If
                End Set
            End Property 'changes the color if selected
            Public Property Material As MaterialClass Implements IBaseElement.Material
                Get
                    Return _Material
                End Get
                Set(value As MaterialClass)
                    _Material = value
                    InvalidateSolution()
                End Set
            End Property 'flags the solution invalid if set

            Public Sub New(ByVal Type As ElementType, ByVal NumNodes As Integer, ByVal NodeDOFs As Integer, ByVal Color As Color, ByVal ID As Integer, Optional ByVal Mat As MaterialClass = Nothing)
                _Type = Type
                _DefaultColor = Color

                ReDim _Color(NumNodes) 'need to have a color for each node in the element
                For Each Val As Color In _Color 'initially set all corners to the default color
                    Val = Color
                Next

                _Material = Mat
                _NodeDOFs = NodeDOFs
                _NumOfNodes = NumNodes
                _ID = ID
            End Sub

            Public Sub SetColor(ByVal C As Color) Implements IBaseElement.SetColor
                For Each Val As Color In _Color
                    Val = C
                Next
            End Sub 'sets all endpoints to the specified color
            Public Sub SetCornerColor(ByVal C As Color, ByVal LocalNodeID As Integer) Implements IBaseElement.SetCornerColor
                _Color(LocalNodeID) = C
            End Sub


            Protected Sub InvalidateSolution()
                _SolutionValid = False
                RaiseEvent SolutionInvalidated(_ID)
            End Sub

        End Class 'base element subclass - common between all types of elements

        Public Enum ElementType
            Bar_linear = 0
        End Enum
        Public Interface IElement
            Inherits IBaseElement

            ReadOnly Property Interpolated_Displacement(ByVal IntrinsicCoords As Double(), ByVal GblNodeQ As DenseMatrix) 'can interpolate either position or displacement
            ReadOnly Property B_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix 'needs to be given with local node 1 in first spot on list
            ReadOnly Property Length(ByVal GblNodeCoords As List(Of Double())) As Double
            ReadOnly Property Stress_mtrx(ByVal GblNodeCoords As List(Of Double()), ByVal GblNodeQ As DenseMatrix, Optional ByVal IntrinsicCoords As Double() = Nothing) As DenseMatrix

            ReadOnly Property K_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix 'node 1 displacement comes first in disp input, followed by second
            ReadOnly Property BodyForce_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix 'node 1 displacement comes first in disp input, followed by second
            ReadOnly Property TractionForce_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix


            Sub SortNodeOrder(ByRef NodeIDs As List(Of Integer), ByVal NodeCoords As List(Of Double()))
            Sub SetBodyForce(ByVal ForcePerVol As DenseMatrix)
            Sub SetTractionForce(ByVal ForcePerLength As DenseMatrix)
            Sub Draw(ByVal GblNodeCoords As List(Of Double()))

        End Interface
        Public Interface IBaseElement

            '------------------ From Element Baseclass -----------------

            ReadOnly Property ID As Integer
            ReadOnly Property Type As ElementType
            ReadOnly Property NumOfNodes As Integer
            ReadOnly Property NodeDOFs As Integer
            ReadOnly Property ElemDOFs As Integer
            ReadOnly Property SelectColor As Color
            ReadOnly Property ElemColor As Color()
            ReadOnly Property CornerColor(ByVal LocalNodeID As Integer) As Color
            ReadOnly Property AllCornersSameColor As Boolean
            ReadOnly Property AllCornersEqualColor(ByVal C_in As Color) As Boolean

            Property Selected As Boolean
            Property Material As MaterialClass

            Sub SetColor(ByVal C As Color)
            Sub SetCornerColor(ByVal C As Color, ByVal LocalNodeID As Integer)


        End Interface 'interface for base element subclass

    End Class

    Public Class View

        Private _currentTrans As Double() = {0, 0, 0}
        Private _finalTrans As Double() = {0, 0, 0}
        Private _currentRot As Double() = {0, 0}
        Private _finalRot As Double() = {0, 0}
        Private _finalZoom As Double = 1.0

        Private _mousepressed As Boolean = False
        Private _mousedelta As Double = 0

        Private _mouseDownX As Double = 0
        Private _mouseDownY As Double = 0
        Private _currentMouseX As Double = 0
        Private _currentMouseY As Double = 0

        Private _RotMultiplier As Double = 1
        Private _TransMultiplier As Double = -0.1
        Private _ZoomMultiplier As Double = 0.02

        Public Event ViewUpdated(ByVal Trans As Double(), ByVal Rot As Double(), ByVal zoom As Double)

        Public Sub Dragstarted(ByRef C As GLControl, X As Double, Y As Double)
            If MouseIsOverControl(C) Then
                _mousepressed = True
                _mouseDownX = X
                _mouseDownY = Y
            End If
        End Sub
        Public Sub Dragged(ByRef C As GLControl, X As Double, Y As Double)
            If _mousepressed Then
                _currentMouseX = X
                _currentMouseY = Y
                SetOrientation_Drag(C)
            End If
        End Sub
        Public Sub EndDrag()
            _mousepressed = False
            _finalTrans(0) += _currentTrans(0)
            _finalTrans(1) += _currentTrans(1)
            _finalTrans(2) += _currentTrans(2)
            _finalRot(0) += _currentRot(0)
            _finalRot(1) += _currentRot(1)
        End Sub
        Public Sub MouseWheeled(ByRef C As GLControl, Z As Double)
            If MouseIsOverControl(C) Then
                _mousedelta = Z
                PrepCamera(C)
                SetOrientation_Drag(C)
            End If
        End Sub

        Public Sub Refresh(ByVal C As GLControl)
            PrepCamera(C)
            SetOrientation_Manual(C, _finalTrans, _finalRot, _finalZoom)
        End Sub
        Public Sub SetOrientation_Manual(ByVal C As GLControl, ByVal Trans As Double(), ByVal Rot As Double(), ByVal Zoom As Double)
            'PrepCamera(C)

            _finalTrans(0) = Trans(0)
            _finalTrans(1) = Trans(1)

            _finalRot(0) = Rot(0)
            _finalRot(1) = Rot(1)

            _finalZoom = Zoom

            GL.Translate(0, _finalTrans(1), _finalTrans(0))
            GL.Rotate(_finalRot(0), 0, 1, 0)
            GL.Rotate(_finalRot(1), 0, 0, 1)
            GL.Scale(_finalZoom, _finalZoom, _finalZoom)

            'prepare output event
            Dim transOut As Double() = {_finalTrans(0), _finalTrans(1), _finalTrans(2)}
            Dim rotOut As Double() = {_finalRot(0), _finalRot(1)}
            RaiseEvent ViewUpdated(transOut, rotOut, _finalZoom)
        End Sub
        Private Sub SetOrientation_Drag(ByVal C As GLControl)
            PrepCamera(C)

            _currentRot(0) = 0
            _currentRot(1) = 0
            _currentTrans(0) = 0
            _currentTrans(1) = 0
            _currentTrans(2) = 0


            Dim _RotXcomp As Double = 0 'component of mouse movement in X direction
            Dim _RotYcomp As Double = 0 'component of mouse movement in Y direction

            '------------- handle mousewheel ---------------

            If _mousedelta <> 0 Then
                If _mousedelta > 0 Then
                    _finalZoom += _ZoomMultiplier
                ElseIf _mousedelta < 0 & _finalZoom <> _ZoomMultiplier Then
                    _finalZoom -= _ZoomMultiplier
                End If
                _mousedelta = 0 'actual zoom value updated below
            End If

            '------------- handle mousebutton ---------------

            If _mousepressed = True Then

                Dim deltax As Double = _currentMouseX - _mouseDownX
                Dim deltaY As Double = _currentMouseY - _mouseDownY

                If My.Computer.Keyboard.ShiftKeyDown Then 'translate mode
                    _currentTrans(0) = deltax * _TransMultiplier
                    _currentTrans(1) = deltaY * _TransMultiplier

                Else 'rotation mode
                    _currentRot(0) = deltax * _RotMultiplier
                    _currentRot(1) = deltaY * _RotMultiplier

                End If
            End If

            'perform actual orientation.. if nothing is updated stays at last position

            GL.Translate(0, (_finalTrans(1) + _currentTrans(1)), (_finalTrans(0) + _currentTrans(0)))
            GL.Rotate(_finalRot(0) + _currentRot(0), 0, 1, 0)
            GL.Rotate(_finalRot(1) + _currentRot(1), 0, 0, 1)
            GL.Scale(_finalZoom, _finalZoom, _finalZoom)

            'prepare output event
            Dim trans As Double() = {_finalTrans(0) + _currentTrans(0), _finalTrans(1) + _currentTrans(1), _finalTrans(2) + _currentTrans(2)}
            Dim rot As Double() = {_finalRot(0) + _currentRot(0), _finalRot(1) + _currentRot(1)}
            RaiseEvent ViewUpdated(trans, rot, _finalZoom)
        End Sub

        Private Sub PrepCamera(ByVal C As GLControl)
            'First Clear Buffers
            GL.Clear(ClearBufferMask.ColorBufferBit)
            GL.Clear(ClearBufferMask.DepthBufferBit)

            'Basic Setup for viewing
            Dim perspective As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(1.1, 4 / 3, 1, 10000) 'Setup Perspective
            GL.MatrixMode(MatrixMode.Projection) 'Load Perspective
            GL.LoadIdentity()
            GL.LoadMatrix(perspective)

            Dim lookat As Matrix4 = Matrix4.LookAt(100, 0, 0, 0, 0, 0, 0, 1, 0) 'Setup camera
            GL.MatrixMode(MatrixMode.Modelview) 'Load Camera
            GL.LoadIdentity()
            GL.LoadMatrix(lookat)

            GL.Viewport(0, 0, C.Width, C.Height) 'Size of window
            GL.Enable(EnableCap.DepthTest) 'Enable correct Z Drawings
            GL.DepthFunc(DepthFunction.Less) 'Enable correct Z Drawings

        End Sub


        Private Function MouseIsOverControl(ByRef c As GLControl) As Boolean
            Return c.ClientRectangle.Contains(c.PointToClient(Control.MousePosition))
        End Function
    End Class


End Class



