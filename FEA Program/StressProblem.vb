Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports MathNet.Numerics.LinearAlgebra.Double
Imports System.IO
Imports System.Xml.Serialization
Imports System.Reflection
Public Class StressProblem

    Public WithEvents Nodes As New NodeMgr
    Public WithEvents Elements As New ElementMgr
    Public WithEvents Materials As New MaterialMgr
    Public WithEvents Connect As New Connectivity
    Public Loadedform As Mainform = Nothing

    Private _Type As ProblemTypes

    Public ReadOnly Property AvailableElements As Type()
        Get
            Select Case _Type
                Case ProblemTypes.Bar_1D
                    Return {GetType(ElementMgr.Element_Bar_Linear)}
                    Exit Select

                Case Else
                    Return Nothing
            End Select
        End Get
    End Property 'which elements are available depending on problem type
    Public ReadOnly Property AvailableNodeDOFs As Integer
        Get

            Select Case _Type
                Case ProblemTypes.Bar_1D
                    Return 1
                    Exit Select

                Case ProblemTypes.Beam_1D
                    Return 1
                    Exit Select
                Case ProblemTypes.Truss_3D
                    Return 3
                    Exit Select
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property 'which node DOF should be used for given problem type
    Public ReadOnly Property ThreeDimensional As Boolean
        Get
            Select Case _Type
                Case ProblemTypes.Bar_1D
                    Return False

                Case ProblemTypes.Beam_1D
                    Return False

                Case ProblemTypes.Truss_3D
                    Return True

                Case Else
                    Return Nothing
            End Select
        End Get
    End Property 'whether the screen should be 3D based on the problem type
    Public ReadOnly Property CommandList As Dictionary(Of String, List(Of Type))
        Get
            Dim output As New Dictionary(Of String, List(Of Type))

            '------------- Add Node Command-------------
            Dim tmp As New List(Of Type)

            For i As Integer = 1 To AvailableNodeDOFs
                tmp.Add(GetType(Double)) 'node position
            Next

            For i As Integer = 1 To AvailableNodeDOFs
                tmp.Add(GetType(Boolean)) 'node fixity
            Next

            output.Add("AddNode", tmp)

            '------------- Add Material Command-------------
            Dim tmp2 As New List(Of Type)

            tmp2.Add(GetType(String))
            tmp2.Add(GetType(Double))
            tmp2.Add(GetType(Double))
            tmp2.Add(GetType(Double))
            tmp2.Add(GetType(Double))
            tmp2.Add(GetType(Integer))

            output.Add("AddMaterial", tmp)

            '------------- Add Element Command-------------

            Return output
        End Get
    End Property

    Public Sub New(ByVal form As Mainform, ByVal Type As ProblemTypes)
        Me.Loadedform = form
        _Type = Type
    End Sub

    Private Sub ListRedrawNeeded() Handles Nodes.NodeListChanged, Nodes.NodeChanged, Elements.ElementListChanged, Elements.ElementChanged, Materials.MatlListChanged
        Loadedform.ReDrawLists()
        Loadedform.GlCont.Invalidate()
    End Sub
    Private Sub ScreenRedrawOnlyNeeded() Handles Nodes.NodeChanged_RedrawOnly, Elements.ElementChanged_RedrawOnly
        Loadedform.GlCont.Invalidate()
    End Sub

    Private Sub HangingElements(ByVal NodeID As Integer, ByVal Dimension As Integer) Handles Nodes.NodeDeleted

        Dim ElementsToDelete As List(Of Integer) = Connect.NodeElements(NodeID)
        Elements.Delete(ElementsToDelete)

    End Sub 'deletes elements if a node is deleted and leaves one hanging
    Private Sub ElementCreation(ByVal ElemID As Integer, ByVal NodeIDs As List(Of Integer), ByVal Type As Type) Handles Elements.ElementAdded

        '---------------------- Get Coords of all of the nodes in the element and then sort Ids

        Dim NodeCoords As New List(Of Double())

        For Each ID As Integer In NodeIDs
            NodeCoords.Add(Nodes.NodeObj(ID).Coords())
        Next

        Elements.ElemObj(ElemID).SortNodeOrder(NodeIDs, NodeCoords) 'nodeIDs is passed byref

        Connect.AddConnection(ElemID, NodeIDs)
    End Sub
    Private Sub ElementDeletion(ByVal ElemID As Integer, ByVal Type As ElementMgr.IElement) Handles Elements.ElementDeleted
        Connect.RemoveConnection(ElemID)
    End Sub


    Public Enum ProblemTypes
        Bar_1D
        Beam_1D
        Truss_3D
    End Enum

End Class


Public Class Solver

End Class


Public Class Connectivity

    Private _ConnectMatrix As New Dictionary(Of Integer, List(Of Integer)) 'dict key is global element ID, list index is local node ID, list value at index is global node ID

    Public ReadOnly Property ConnectMatrix As Dictionary(Of Integer, List(Of Integer))
        Get
            Return _ConnectMatrix
        End Get
    End Property

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


    Private Sub SortNodeLocalIDs(ByVal NodeCoords As Dictionary(Of Integer, Dictionary(Of Integer, Double())))

    End Sub
    Public Function Assemble_K_Mtx(ByVal K_matricies As Dictionary(Of Integer, DenseMatrix), ByVal NodeDOFs As Dictionary(Of Integer, Integer)) As SparseMatrix

        '---------------------- Get Total Size of the Problem --------------------------

        Dim problemSize As Integer = 0
        For Each DOF As Integer In NodeDOFs.Values
            problemSize += DOF
        Next

        '----------------------- Setup Output Matrix -----------------------------------

        Dim output As New SparseMatrix(problemSize, problemSize)

        '------------------------Iterate Through Each Node ID and allocate regions of large matrix --------------------------

        Dim NodeIDs As List(Of Integer) = NodeDOFs.Keys.ToList
        NodeIDs.Sort() 'sort so smallest ID comes first

        Dim NodeMatrixIndicies As New Dictionary(Of Integer, Integer()) 'determines where each displacement will go on the assembled matrix, sorted by node matrix
        Dim IndexCounter As Integer = 0 'to count how many places have been used up in the matrix

        For Each ID As Integer In NodeIDs
            Dim DOF As Integer = NodeDOFs(ID) 'get the number of DOFs for the node
            Dim allocatedIndicies(DOF - 1) As Integer 'create array to hold allocated indicies

            For i As Integer = 0 To DOF - 1 'allocate a value for each DOF incrementally based on the number of DOFs
                allocatedIndicies(i) = IndexCounter
                IndexCounter += 1
            Next

            NodeMatrixIndicies.Add(ID, allocatedIndicies)
        Next

        '------------------------Iterate Through Each Element --------------------------

        For Each ElemID_and_K As KeyValuePair(Of Integer, DenseMatrix) In K_matricies
            '1. get node ID's - assume in correct order
            Dim E_nodeIDs As List(Of Integer) = Me.ElementNodes(ElemID_and_K.Key)

            '2. Allocate Regions of the K Matrix for Each Element

            Dim NodeKmtxIndicies As New Dictionary(Of Integer, Integer()) 'holds which regions of the k matrix are claimed by each node
            IndexCounter = 0 'to count how many places have been used up in the matrix

            For Each ID As Integer In E_nodeIDs
                Dim DOF As Integer = NodeDOFs(ID) 'get the number of DOFs for the node
                Dim allocatedIndicies(DOF - 1) As Integer 'create array to hold allocated indicies

                For i As Integer = 0 To DOF - 1 'allocate a value for each DOF incrementally based on the number of DOFs
                    allocatedIndicies(i) = IndexCounter
                    IndexCounter += 1
                Next

                NodeKmtxIndicies.Add(ID, allocatedIndicies)
            Next

            '3. Move the value range for each node from the local K matrix to the global

            For Each i As Integer In E_nodeIDs
                For Each j As Integer In E_nodeIDs

                    For row As Integer = 0 To NodeDOFs(i) - 1
                        For col As Integer = 0 To NodeDOFs(j) - 1

                            Dim assembled_K_nodeRegion_i As Integer() = NodeMatrixIndicies(i)
                            Dim assembled_K_nodeRegion_j As Integer() = NodeMatrixIndicies(j)

                            Dim local_K_nodeRegion_i As Integer() = NodeKmtxIndicies(i)
                            Dim local_K_nodeRegion_j As Integer() = NodeKmtxIndicies(j)


                            output(assembled_K_nodeRegion_i(row), assembled_K_nodeRegion_j(col)) = ElemID_and_K.Value(local_K_nodeRegion_i(row), local_K_nodeRegion_j(col))
                        Next
                    Next

                Next
            Next

        Next

        Return output
    End Function
    Public Function Solve(ByVal K As SparseMatrix, ByVal F As DenseMatrix, ByVal Q As DenseMatrix) As DenseMatrix()

        Dim FixedIndicies As New List(Of Integer)
        Dim ProblemSize As Integer = Q.RowCount

        For i As Integer = 0 To ProblemSize - 1 'get indicies of each fixed displacement
            If Q(i, 0) = 1 Then
                FixedIndicies.Add(i)
            End If
        Next

        FixedIndicies.Reverse() 'need to remove rows with highest index first

        For Each Val As Integer In FixedIndicies 'first remove columns - they will be multiplied by 0 anyway - rows are needed later for reaction forces
            K = K.RemoveColumn(Val)
        Next

        Dim Reaction_K_Mtx As New DenseMatrix(FixedIndicies.Count, K.ColumnCount) 'need to keep removed values to calculate reaction forces
        Dim Reaction_F_Mtx As New DenseMatrix(FixedIndicies.Count, 1)

        For i As Integer = 0 To FixedIndicies.Count - 1
            Reaction_K_Mtx.SetRow(FixedIndicies.Count - i - 1, K.Row(FixedIndicies(i))) 'save values which are going to be removed from K
            Reaction_F_Mtx.Item(FixedIndicies.Count - i - 1, 0) -= F.Item(FixedIndicies(i), 0) 'adds any forces that are pointed against the direction of reaction forces
        Next

        For Each Val As Integer In FixedIndicies 'remove rows not needed for solving displacements
            K = K.RemoveRow(Val)
            F = F.RemoveRow(Val)
        Next

        Dim Q_Solved As SparseMatrix = K.Solve(F) 'solve displacements
        Reaction_F_Mtx += Reaction_K_Mtx.Multiply(Q_Solved) 'add reactions due to displacements

        Dim Q_output As New DenseMatrix(ProblemSize, 1)
        Dim R_output As New DenseMatrix(ProblemSize, 1)

        Dim k_int As Integer = 0
        Dim j As Integer = 0
        For i As Integer = 0 To ProblemSize - 1
            If FixedIndicies.Contains(i) Then 'this row has been fixed
                Q_output.Item(i, 0) = 0
                R_output.Item(i, 0) = Reaction_F_Mtx(k_int, 0)
                k_int += 1
            Else 'floating row
                Q_output.Item(i, 0) = Q_Solved(j, 0)
                R_output.Item(i, 0) = 0 'reaction must be 0 for a floating displacement
                j += 1
            End If
        Next

        Return {Q_output, R_output}
    End Function


End Class 'need to call functions in here from element/node events upon creation/deletion
Public Class MaterialMgr

    Private _Materials As New Dictionary(Of Integer, MaterialClass) 'reference nodes by ID

    Public Event MatlListChanged(ByVal MatlList As Dictionary(Of Integer, MaterialClass)) 'Length of Matllist has changed

    Public ReadOnly Property MatObj(ByVal ID As Integer) As MaterialClass
        Get
            Return _Materials(ID)
        End Get
    End Property
    Public ReadOnly Property AllIDs As List(Of Integer)
        Get
            Return _Materials.Keys.ToList()
        End Get
    End Property
    Public ReadOnly Property MatList As List(Of MaterialClass)
        Get
            Return _Materials.Values.ToList
        End Get
    End Property
    Public ReadOnly Property All_E As Dictionary(Of Integer, Double)
        Get
            Dim output As New Dictionary(Of Integer, Double)

            For Each Mat As MaterialClass In _Materials.Values
                output.Add(Mat.ID, Mat.E)
            Next
            Return output
        End Get
    End Property


    Public Sub Add(ByVal Name As String, ByVal E_GPa As Double, ByVal V As Double, ByVal Sy_MPa As Double, ByVal Sut_MPa As Double, ByVal subtype As MaterialType)

        Dim ID As Integer = CreateMatlId()
        Dim mat As New MaterialClass(Name, E_GPa, V, Sy_MPa, Sut_MPa, subtype, ID)
        _Materials.Add(ID, mat)

        RaiseEvent MatlListChanged(_Materials)

    End Sub
    Public Sub Delete(ByVal ID As Integer)
        _Materials.Remove(ID)

        RaiseEvent MatlListChanged(_Materials)
    End Sub

    Private Function CreateMatlId() As Integer
        Dim NewID As Integer = 1
        Dim IDUnique As Boolean = False

        While _Materials.Keys.Contains(NewID)
            NewID += 1
        End While

        Return NewID
    End Function


    Public Class MaterialClass
        Private _Name As String = ""
        Private _E As Double = 0 ' youngs modulus in Pa
        Private _V As Double = 0 ' poissons ratio
        Private _Sy As Double = 0 ' yield strength in Pa
        Private _Sut As Double = 0 ' ultimate strength in Pa
        Private _subtype As MaterialType
        Private _ID As Integer = -1


        Public ReadOnly Property ID As Integer
            Get
                Return _ID
            End Get
        End Property
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

        Public Sub New(ByVal Name As String, ByVal E_GPa As Double, ByVal V As Double, ByVal Sy_MPa As Double, ByVal Sut_MPa As Double, ByVal subtype As MaterialType, ByVal InputID As Integer)

            _Name = Name
            _E = E_GPa * 1000 * 1000 * 1000 'convert to Pa
            _V = V
            _Sy = Sy_MPa * 1000 * 1000 'convert to Pa
            _Sut = Sut_MPa * 1000 * 1000 'convert to Pa
            _subtype = subtype
            _ID = InputID
        End Sub


    End Class
    Public Enum MaterialType
        Steel_Alloy
        Aluminum_Alloy
    End Enum

End Class
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

    Public ReadOnly Property NodeCoords As Dictionary(Of Integer, Double()) 'gets coords of all nodes sorted by ID
        Get
            Dim output As New Dictionary(Of Integer, Double())

            For Each N As Node In _Nodes.Values
                output.Add(N.ID, N.Coords)
            Next

            Return output
        End Get
    End Property 'gets all coords referenced by ID
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
    Public Sub AddNodes(ByVal Coords As List(Of Double()), ByVal Fixity As List(Of Integer()), ByVal Dimensions As List(Of Integer))
        If Coords.Count <> Fixity.Count Or Coords.Count <> Dimensions.Count Then
            Throw New Exception("Tried to run sub <" & MethodInfo.GetCurrentMethod.Name & "> with unmatched lengths of input values.")
        End If

        For i As Integer = 0 To Coords.Count - 1
            If ExistsAtLocation(Coords(i)) Then 'dont want to create node where one already is
                Throw New Exception("Tried to create node at location where one already exists. Nodes cannot be in identical locations.")
            End If

            Dim newnode As New Node(Coords(i), Fixity(i), CreateNodeId, Dimensions(i))
            _Nodes.Add(newnode.ID, newnode)
            RaiseEvent NodeAdded(newnode.ID, newnode.Dimension)
        Next

        RaiseEvent NodeListChanged(_Nodes) 'this will redraw so leave it until all have been updated
    End Sub
    Public Sub EditNode(ByVal Coords As List(Of Double()), ByVal fixity As List(Of Integer()), ByVal IDs As List(Of Integer))
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
            If N.Dimension = 1 Then
                If N.Coords(0) = Coords(0) Then 'node already exists at this location
                    Return True
                End If
            ElseIf N.Dimension = 2 Then
                If N.Coords(0) = Coords(0) And N.Coords(1) = Coords(1) Then 'node already exists at this location
                    Return True
                End If
            Else '3 or 6 DOFs
                If N.Coords(0) = Coords(0) And N.Coords(1) = Coords(1) And N.Coords(2) = Coords(2) Then 'node already exists at this location
                    Return True
                End If
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
        Private _Fixity As Integer() = {0, 0, 0, 0, 0, 0} ' 0 = floating, 1 = fixed

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
                Dim output(_Dimensions - 1) As Double

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
        Public Property Fixity As Integer()
            Get
                Return _Fixity
            End Get
            Set(value As Integer())
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

        Public Sub New(ByVal NewCoords As Double(), ByVal NewFixity As Integer(), ByVal NewID As Integer, ByVal Dimensions As Integer)

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

            If _Dimensions = 1 Or _Dimensions = 2 Then
                GL.Vertex3(tmp(0) + 1, tmp(1) + 1, tmp(2))
                GL.Vertex3(tmp(0) + 1, tmp(1) - 1, tmp(2))
                GL.Vertex3(tmp(0) - 1, tmp(1) - 1, tmp(2))
                GL.Vertex3(tmp(0) - 1, tmp(1) + 1, tmp(2))

            Else
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

            End If
            GL.End()
        End Sub 'draw always has mm input
        Private Sub DrawForce(ByVal N As Double(), ByVal F As Double(), ByVal Color As Color)
            If Array.TrueForAll(F, AddressOf ValEqualsZero) = False Then 'dont draw a force if its zero

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

                Dim Z As Vector3 = New Vector3(0, 0, 1)
                Dim normal2 As Vector3 = Vector3.Cross(vect, Z)
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
        Private Sub DrawFixity(ByVal N As Double(), ByVal Fix As Integer(), ByVal Color As Color)
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

    Shared _SelectedColor As Color = Color.Yellow ' the default color of selected objects for the program

    Private _Bar1Elements As New Dictionary(Of Integer, IElement) 'reference elements by ID

    Public Event ElementListChanged(ByVal ElemList As Dictionary(Of Integer, IElement)) 'Length of Elementlist has changed
    Public Event ElementChanged(ByVal ID As Integer) 'Element has changed such that list needs to be updated & screen redrawn
    Public Event ElementChanged_RedrawOnly() 'Element has changed such that screen only needs to be redrawn
    Public Event ElementAdded(ByVal ElemID As Integer, ByVal NodeIDs As List(Of Integer), ByVal Type As Type) 'dont use for redrawing lists or screen
    Public Event ElementDeleted(ByVal ElemID As Integer, ByVal Type As IElement) 'dont use for redrawing lists or screen

    Public Shared ReadOnly Property NumOfNodes(ByVal ElemType As Type) As Integer
        Get
            Select Case ElemType
                Case GetType(Element_Bar_Linear)
                    Return 2
                    Exit Select
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property
    Public Shared ReadOnly Property NodeDOFs(ByVal ElemType As Type) As Integer
        Get
            Select Case ElemType
                Case GetType(Element_Bar_Linear)
                    Return 1
                    Exit Select
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property
    Public Shared ReadOnly Property Name(ByVal ElemType As Type) As String
        Get
            Select Case ElemType
                Case GetType(Element_Bar_Linear)
                    Return "Bar_Linear"
                    Exit Select
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property
    Public Shared ReadOnly Property ElemDOFs(ByVal ElemType As Type) As Integer
        Get
            Return NumOfNodes(ElemType) * NodeDOFs(ElemType)
        End Get
    End Property
    Public Shared ReadOnly Property ElementArgs(ByVal ElemType As Type) As Dictionary(Of String, Units.DataUnitType)
        Get
            Dim output As New Dictionary(Of String, Units.DataUnitType)

            Select Case ElemType

                Case GetType(Element_Bar_Linear)
                    output.Add("Area", Units.DataUnitType.Area)

                Case Else
                    Return Nothing

            End Select

            Return output
        End Get
    End Property


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
    Public ReadOnly Property K_matricies(ConnectMatrix As Dictionary(Of Integer, List(Of Integer)), ByVal NodeCoords As Dictionary(Of Integer, Double()), ByVal E As Dictionary(Of Integer, Double)) As Dictionary(Of Integer, DenseMatrix)
        Get
            Dim output As New Dictionary(Of Integer, DenseMatrix)

            For Each E_ID As Integer In ConnectMatrix.Keys 'iterate through each element
                Dim ElemNodeCoords As New List(Of Double())

                For Each NodeID As Integer In ConnectMatrix(E_ID) 'get the coordinates of each node in the element
                    ElemNodeCoords.Add(NodeCoords(NodeID))
                Next

                Dim MatID As Integer = _Bar1Elements(E_ID).Material 'get the material ID

                output.Add(E_ID, _Bar1Elements(E_ID).K_mtrx(ElemNodeCoords, E(MatID)))
            Next

            Return output
        End Get
    End Property


    Public Sub Add(ByVal Type As Type, ByVal NodeIDs As List(Of Integer), ByVal ElementArgs As Double(), Optional ByVal Mat As Integer = -1)
        Dim newElem As IElement = Nothing
        Dim newElemID As Integer = CreateId()

        '------------------ Determine type of element ----------------------

        If Type Is GetType(Element_Bar_Linear) Then 'linear bar element --------------------------------

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

        If NodeIDs.Count <> ElementMgr.NumOfNodes(newElem.GetType()) Then 'check if the right number of nodes are listed
            Throw New Exception("Tried to add element of type <" & Type.ToString & "> with " & CStr(NodeIDs.Count) & " nodes. Should have " & ElementMgr.NumOfNodes(newElem.GetType()) & " nodes.")
        End If

        If newElem IsNot Nothing Then 'more error checking
            _Bar1Elements.Add(newElem.ID, newElem)
            RaiseEvent ElementAdded(newElem.ID, NodeIDs, newElem.GetType)
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
            RaiseEvent ElementDeleted(tmp.ID, tmp.GetType)
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

        Public Overrides ReadOnly Property MyType As Type Implements IElement.MyType
            Get
                Return Me.GetType()
            End Get
        End Property
        Private ReadOnly Property N_mtrx(ByVal IntrinsicCoords As Double()) As DenseMatrix
            Get
                If IntrinsicCoords.Length <> 1 Then
                    Throw New Exception("Wrong number of coords input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                End If

                Dim eta As Double = IntrinsicCoords(0)

                Dim N As New DenseMatrix(1, Me.NodeDOFs * Me.NumOfNodes) 'u = Nq - size based off total number of element DOFs
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

                Dim B_out As New DenseMatrix(1, Me.NodeDOFs * Me.NumOfNodes) 'based from total DOFs
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
        Public ReadOnly Property Stress_mtrx(ByVal GblNodeCoords As List(Of Double()), ByVal GblNodeQ As DenseMatrix, ByVal E As Double, Optional ByVal IntrinsicCoords As Double() = Nothing) As DenseMatrix Implements IElement.Stress_mtrx
            Get
                If GblNodeCoords.Count <> Me.NumOfNodes Or GblNodeQ.Values.Count <> Me.ElemDOFs Then
                    Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                End If

                Dim output As DenseMatrix = E * Me.B_mtrx(GblNodeCoords) * GblNodeQ
                Return output
            End Get
        End Property 'node 1 displacement comes first in disp input, followed by second
        Public ReadOnly Property K_mtrx(ByVal GblNodeCoords As List(Of Double()), ByVal E As Double) As DenseMatrix Implements IElement.K_mtrx
            Get
                If GblNodeCoords.Count <> Me.NumOfNodes Then
                    Throw New Exception("Wrong number of Nodes input to " & MethodInfo.GetCurrentMethod.Name & ". Node: " & CStr(Me.ID))
                End If

                Dim output As New DenseMatrix(Me.ElemDOFs, Me.ElemDOFs)
                output(0, 0) = 1
                output(1, 0) = -1
                output(0, 1) = -1
                output(1, 1) = 1

                output = output * E * _Area / Me.Length(GblNodeCoords)

                Return output
            End Get
        End Property 'node 1 displacement comes first in disp input, followed by second
        Public ReadOnly Property BodyForce_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix Implements IElement.BodyForce_mtrx
            Get
                If GblNodeCoords.Count <> Me.ElemDOFs Then
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

        Public Sub New(ByVal Area As Double, ByVal ID As Integer, Optional ByVal Mat As Integer = -1)
            MyBase.New(Drawing.Color.Green, ID, Mat)

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

            GL.LineWidth(2)

            GL.Begin(PrimitiveType.Lines)

            If Me.NodeDOFs = 1 Then
                For i As Integer = 0 To Me.NumOfNodes - 1
                    GL.Color3(_Color(i))
                    GL.Vertex3(GblNodeCoords(i)(0), 0, 0)
                Next
            ElseIf Me.NodeDOFs = 2 Then
                For i As Integer = 0 To Me.NumOfNodes - 1
                    GL.Color3(_Color(i))
                    GL.Vertex3(GblNodeCoords(i)(0), GblNodeCoords(i)(1), 0)
                Next
            Else
                For i As Integer = 0 To Me.NumOfNodes - 1
                    GL.Color3(_Color(i))
                    GL.Vertex3(GblNodeCoords(i)(0), GblNodeCoords(i)(1), GblNodeCoords(i)(2))
                Next
            End If

            GL.End()

            GL.LineWidth(1)
        End Sub

    End Class
    Public MustInherit Class Element
        Implements IBaseElement

        Private _NumOfNodes As Integer 'holds value only for internal usage
        Private _NodeDOFs As Integer 'holds value only for internal usage

        Private _ID As Integer = -1    
        Private _Material As Integer 'holds material ID
        Private _DefaultColor As Color
        Protected _Color As Color() = Nothing 'want to be able to set a color for each endpoint of the element

        Private _ReadyToSolve As Boolean = False 'is true if the nodes of the element are set up properly
        Protected _SolutionValid As Boolean = False 'is true if the solution for the element is correct

        Public Event SolutionInvalidated(ByVal ElemID As Integer)

        Public MustOverride ReadOnly Property MyType As Type

        Protected ReadOnly Property NumOfNodes As Integer
            Get
                Return _NumOfNodes
            End Get
        End Property
        Protected ReadOnly Property NodeDOFs As Integer
            Get
                Return _NodeDOFs
            End Get
        End Property
        Protected ReadOnly Property ElemDOFs As Integer
            Get
                Return _NodeDOFs * _NumOfNodes
            End Get
        End Property


        Public ReadOnly Property ID As Integer Implements IBaseElement.ID
            Get
                Return _ID
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
        Public Property Material As Integer Implements IBaseElement.Material
            Get
                Return _Material
            End Get
            Set(value As Integer)
                _Material = value
                InvalidateSolution()
            End Set
        End Property 'flags the solution invalid if set

        Public Sub New(inColor As Color, ByVal ID As Integer, Optional ByVal Mat As Integer = -1)
            _NumOfNodes = ElementMgr.NumOfNodes(MyType)
            _NodeDOFs = ElementMgr.NodeDOFs(MyType)

            _DefaultColor = inColor

            ReDim _Color(_NumOfNodes - 1) 'need to have a color for each node in the element
            For i As Integer = 0 To _Color.Length - 1 'initially set all corners to the default color
                _Color(i) = inColor
            Next

            _Material = Mat
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

  

    Public Interface IElement
        Inherits IBaseElement

        ReadOnly Property MyType As Type
        ReadOnly Property Interpolated_Displacement(ByVal IntrinsicCoords As Double(), ByVal GblNodeQ As DenseMatrix) 'can interpolate either position or displacement
        ReadOnly Property B_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix 'needs to be given with local node 1 in first spot on list
        ReadOnly Property Length(ByVal GblNodeCoords As List(Of Double())) As Double
        ReadOnly Property Stress_mtrx(ByVal GblNodeCoords As List(Of Double()), ByVal GblNodeQ As DenseMatrix, ByVal E As Double, Optional ByVal IntrinsicCoords As Double() = Nothing) As DenseMatrix

        ReadOnly Property K_mtrx(ByVal GblNodeCoords As List(Of Double()), ByVal E As Double) As DenseMatrix 'node 1 displacement comes first in disp input, followed by second
        ReadOnly Property BodyForce_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix 'node 1 displacement comes first in disp input, followed by second
        ReadOnly Property TractionForce_mtrx(ByVal GblNodeCoords As List(Of Double())) As DenseMatrix


        Sub SortNodeOrder(ByRef NodeIDs As List(Of Integer), ByVal NodeCoords As List(Of Double()))
        Sub SetBodyForce(ByVal ForcePerVol As DenseMatrix)
        Sub SetTractionForce(ByVal ForcePerLength As DenseMatrix)
        Sub Draw(ByVal GblNodeCoords As List(Of Double()))

    End Interface
    Public Interface IBaseElement

        '------------------ From Element Baseclass -----------------
        'ReadOnly Property NumOfNodes As Integer
        'ReadOnly Property NodeDOFs As Integer
        'ReadOnly Property ElemDOFs As Integer


        ReadOnly Property ID As Integer
        ReadOnly Property SelectColor As Color
        ReadOnly Property ElemColor As Color()
        ReadOnly Property CornerColor(ByVal LocalNodeID As Integer) As Color
        ReadOnly Property AllCornersSameColor As Boolean
        ReadOnly Property AllCornersEqualColor(ByVal C_in As Color) As Boolean

        Property Selected As Boolean
        Property Material As Integer
        Sub SetColor(ByVal C As Color)
        Sub SetCornerColor(ByVal C As Color, ByVal LocalNodeID As Integer)

    End Interface 'interface for base element subclass

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

    Public Sub Draw(ByVal ThreeDimensional As Boolean)

        Dim xform As Matrix4 = Matrix4.CreateTranslation(New Vector3(originX / _scale, originY / _scale, originZ / _scale))
        xform = Matrix4.Mult(xform, Matrix4.CreateScale(_scale, _scale, _scale))
        GL.MultMatrix(xform)


        'X Axis
        GL.Color3(SysColor)
        GL.Begin(PrimitiveType.Lines)
        GL.Vertex3(0, 0, 0)
        GL.Vertex3(1, 0, 0)
        GL.Vertex3(1, 0, 0)
        GL.Vertex3(0.9, 0.1, 0)
        GL.Vertex3(1, 0, 0)
        GL.Vertex3(0.9, -0.1, 0)

        'letter x
        GL.Vertex3(0.9, 0.25, 0)
        GL.Vertex3(1, 0.35, 0)
        GL.Vertex3(1, 0.15, 0)
        GL.Vertex3(0.8, 0.35, 0)
        GL.Vertex3(0.9, 0.25, 0)
        GL.Vertex3(0.8, 0.15, 0)

        'Y Axis

        GL.Vertex3(0, 0, 0)
        GL.Vertex3(0, 1, 0)
        GL.Vertex3(0, 1, 0)
        GL.Vertex3(0.1, 0.9, 0)
        GL.Vertex3(0, 1, 0)
        GL.Vertex3(-0.1, 0.9, 0)

        'letter y
        GL.Vertex3(0.15, 1, 0)
        GL.Vertex3(0.25, 0.9, 0)
        GL.Vertex3(0.25, 0.9, 0)
        GL.Vertex3(0.35, 1, 0)
        GL.Vertex3(0.25, 0.9, 0)
        GL.Vertex3(0.25, 0.8, 0)

        If ThreeDimensional Then
            'Z Axis
            GL.Vertex3(0, 0, 0)
            GL.Vertex3(0, 0, 1)
            GL.Vertex3(0, 0, 1)
            GL.Vertex3(0.1, 0, 0.9)
            GL.Vertex3(0, 0, 1)
            GL.Vertex3(-0.1, 0, 0.9)

            'letter Z
            GL.Vertex3(0.15, 0, 1)
            GL.Vertex3(0.35, 0, 1)
            GL.Vertex3(0.15, 0, 1)
            GL.Vertex3(0.35, 0, 0.8)
            GL.Vertex3(0.35, 0, 0.8)
            GL.Vertex3(0.15, 0, 0.8)
        End If

        GL.End()

        xform = Matrix4.Identity
        xform = Matrix4.Mult(xform, Matrix4.CreateScale(1 / _scale, 1 / _scale, 1 / _scale))
        xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(New Vector3(-originX / _scale, -originY / _scale, -originZ / _scale)))
        GL.MultMatrix(xform)


    End Sub


End Class

Public Class Units

    Public Class Length
        Private Shared ReadOnly Property ConversionFactor(ByVal LengthUnit As Integer) As Double
            Get
                Select Case LengthUnit

                    Case LengthUnits.m
                        Return 1.0
                    Case LengthUnits.mm
                        Return 0.001
                    Case LengthUnits.cm
                        Return 0.01
                    Case LengthUnits.inch
                        Return 0.0254
                    Case LengthUnits.ft
                        Return 0.3048
                    Case Else
                        Return Nothing
                End Select
            End Get
        End Property

        Public Shared ReadOnly Property DefaultUnit As Integer
            Get
                Return LengthUnits.m
            End Get
        End Property
        Public Shared ReadOnly Property UnitStrings(ByVal LengthUnit As Integer) As String()
            Get
                Select Case LengthUnit

                    Case LengthUnits.m
                        Return {"m"}
                    Case LengthUnits.mm
                        Return {"mm"}
                    Case LengthUnits.cm
                        Return {"cm"}
                    Case LengthUnits.inch
                        Return {"in", "inch", """"}
                    Case LengthUnits.ft
                        Return {"ft", "feet", "'"}
                    Case Else
                        Return Nothing
                End Select
            End Get
        End Property
        Public Shared ReadOnly Property UnitEnums(ByVal UnitString As String) As Integer
            Get
                For I As Integer = 0 To [Enum].GetNames(GetType(LengthUnits)).Count - 1
                    If UnitStrings(I).Contains(UnitString) Then
                        Return I
                    End If
                Next
                Return -1 'if could not find
            End Get
        End Property

        Public Shared Function Convert(ByVal InputUnit As Integer, ByVal Data As Double, ByVal OutputUnit As Integer) As Double
            Return Data * ConversionFactor(InputUnit) / ConversionFactor(OutputUnit)
        End Function

        Public Enum LengthUnits
            m = 0
            mm = 1
            cm = 2
            inch = 3
            ft = 4
        End Enum

    End Class

    Public Shared ReadOnly Property ConversionFactors(ByVal UnitType As DataUnitType) As Dictionary(Of String, Double)
        Get
            Dim output As New Dictionary(Of String, Double)
            Select Case UnitType
                Case DataUnitType.Length
                    output.Add("m", 1)
                    output.Add("mm", 0.001)
                    output.Add("cm", 0.01)
                    output.Add("in", 0.0254)
                    output.Add("ft", 0.3048)
                    Return output

                Case DataUnitType.Area
                    output.Add("m^2", 1)
                    output.Add("mm^2", 0.001 * 0.001)
                    output.Add("cm^2", 0.01 * 0.01)
                    output.Add("in^2", 0.0254 * 0.0254)
                    output.Add("ft^2", 0.3048 * 0.3048)
                    Return output

                Case DataUnitType.Force
                    output.Add("N", 1)
                    output.Add("lb", 4.44822)
                    output.Add("lbs", 4.44822)
                    Return output

                Case DataUnitType.Pressure
                    output.Add("Pa", 1)
                    output.Add("pa", 1)
                    output.Add("kpa", 1000)
                    output.Add("Kpa", 1000)
                    output.Add("Mpa", 1000 * 1000)
                    output.Add("mpa", 1000 * 1000)
                    output.Add("bar", 100000)
                    output.Add("psi", 6894.76)
                    Return output
            End Select

            Return Nothing
        End Get
    End Property

    '----------------- UPDATE THESE IF NEW UNITS ARE ADDED -------------------
    Private Shared ReadOnly Property ConversionFactor(ByVal Unit As Integer) As Double
        Get
            Select Case Unit
                '---------------- Length ------------------
                Case AllUnits.m
                    Return 1.0
                Case AllUnits.mm
                    Return 0.001
                Case AllUnits.cm
                    Return 0.01
                Case AllUnits.inch
                    Return 0.0254
                Case AllUnits.ft
                    Return 0.3048

                    '---------------------------Area----------------------
                Case AllUnits.m_squared
                    Return ConversionFactor(AllUnits.m) * ConversionFactor(AllUnits.m)
                Case AllUnits.mm_squared
                    Return ConversionFactor(AllUnits.mm) * ConversionFactor(AllUnits.mm)
                Case AllUnits.cm_squared
                    Return ConversionFactor(AllUnits.cm) * ConversionFactor(AllUnits.cm)
                Case AllUnits.in_squared
                    Return ConversionFactor(AllUnits.inch) * ConversionFactor(AllUnits.inch)
                Case AllUnits.ft_squared
                    Return ConversionFactor(AllUnits.ft) * ConversionFactor(AllUnits.ft)

                    '-------------------- Force ---------------------------

                Case AllUnits.N
                    Return 1
                Case AllUnits.lb
                    Return 4.44822

                    '----------------- Pressure ----------------------
                Case AllUnits.Pa
                    Return 1
                Case AllUnits.KPa
                    Return 1000
                Case AllUnits.MPa
                    Return 1000 * 1000
                Case AllUnits.GPa
                    Return 1000 * 1000 * 1000
                Case AllUnits.Psi
                    Return 6894.76
                Case AllUnits.Bar
                    Return 100000
      
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property
    Public Shared ReadOnly Property UnitStrings(ByVal Unit As AllUnits) As String()
        Get
            Select Case Unit
                '-------------------- Length -------------------
                Case AllUnits.m
                    Return {"m"}
                Case AllUnits.mm
                    Return {"mm"}
                Case AllUnits.cm
                    Return {"cm"}
                Case AllUnits.inch
                    Return {"in", "inch", """"}
                Case AllUnits.ft
                    Return {"ft", "feet", "'"}

                    '---------------------------Area----------------------

                Case AllUnits.m_squared
                    Return {"m^2"}
                Case AllUnits.mm_squared
                    Return {"mm^2"}
                Case AllUnits.cm_squared
                    Return {"cm^2"}
                Case AllUnits.in_squared
                    Return {"in^2", "sqin"}
                Case AllUnits.ft_squared
                    Return {"ft^2", "sqft"}

                    '-------------------- Force ---------------------------

                Case AllUnits.N
                    Return {"N"}
                Case AllUnits.lb
                    Return {"lb", "lbs"}

                    '----------------- Pressure ----------------------
                Case AllUnits.Pa
                    Return {"Pa", "pa"}
                Case AllUnits.KPa
                    Return {"KPa", "kpa", "Kpa"}
                Case AllUnits.MPa
                    Return {"MPa", "mpa", "Mpa"}
                Case AllUnits.GPa
                    Return {"GPa", "gpa", "Gpa"}
                Case AllUnits.Psi
                    Return {"psi", "Psi"}
                Case AllUnits.Bar
                    Return {"bar", "Bar"}

                Case Else
                    Return Nothing
            End Select
        End Get
    End Property
    Public Shared ReadOnly Property DefaultUnit(ByVal UnitType As DataUnitType) As AllUnits
        Get
            Select Case UnitType
                Case DataUnitType.Length
                    Return AllUnits.m

                Case DataUnitType.Area
                    Return AllUnits.m_squared

                Case DataUnitType.Force
                    Return AllUnits.N

                Case DataUnitType.Pressure
                    Return AllUnits.Pa
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property
    Public Shared ReadOnly Property UnitTypeRange(ByVal UnitType As DataUnitType) As Integer()
        Get
            Select Case UnitType
                Case DataUnitType.Length
                    Return {0, 4}

                Case DataUnitType.Area
                    Return {5, 9}

                Case DataUnitType.Force
                    Return {10, 11}

                Case DataUnitType.Pressure
                    Return {12, 17}
                Case Else
                    Return Nothing
            End Select
        End Get
    End Property

    '----------------------------------------------------------------------------
    Public Shared ReadOnly Property TypeUnitStrings(ByVal UnitType As DataUnitType) As List(Of String)
        Get
            Dim range As Integer() = UnitTypeRange(UnitType)

            Dim output As New List(Of String)

            For i As Integer = range(0) To range(1) 'search through the enum range for that type
                For Each S As String In UnitStrings(i) 'get all the strings for each enum and add to output
                    output.Add(S)
                Next
            Next

                Return output
        End Get
    End Property
    Public Shared ReadOnly Property UnitEnums(ByVal UnitString As String) As AllUnits
        Get
            For I As Integer = 0 To [Enum].GetNames(GetType(AllUnits)).Count - 1
                If UnitStrings(I).Contains(UnitString) Then
                    Return I
                End If
            Next
            Return -1 'if could not find
        End Get
    End Property
    Public Shared Function Convert(ByVal InputUnit As AllUnits, ByVal Data As Double, ByVal OutputUnit As AllUnits) As Double
        Return Data * ConversionFactor(InputUnit) / ConversionFactor(OutputUnit)
    End Function



    Public Enum DataUnitType
        Length = 0 'm
        Area = 1 'm^2
        Force = 2 'N
        Pressure = 3 'Pa
    End Enum
    Public Enum AllUnits
        '--------------- Length -------------------
        mm
        cm
        m
        inch
        ft
        '------------- Area -----------------------
        mm_squared
        cm_squared
        m_squared
        in_squared
        ft_squared
        '------------- Force -----------------------
        N
        lb
        '------------- Pressure ---------------
        KPa
        MPa
        GPa
        Pa
        Psi
        Bar

    End Enum


End Class
