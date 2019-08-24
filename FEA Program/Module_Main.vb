Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports MathNet.Numerics.LinearAlgebra.Double
Imports System.IO
Imports System.Xml.Serialization

Public Module Module_Main

    Public WithEvents Nodes As New NodeManager
    Public WithEvents Elements As New ElementManager
    Public Materials As New List(Of Material)
    Public Loadedform As Mainform = Nothing

    Public Solve As New Solver

    Private Sub ListRedrawNeeded() Handles Nodes.NodeListChanged, Nodes.NodeChanged, Elements.ElementListChanged, Elements.ElementChanged
        Loadedform.RedrawLists()
        Loadedform.RedrawScreen()
    End Sub
    Private Sub ScreenRedrawOnlyNeeded() Handles Nodes.NodeChanged_RedrawOnly, Elements.ElementChanged_RedrawOnly
        Loadedform.RedrawScreen()
    End Sub
    Private Sub HangingElements(ByVal IDs As List(Of Integer)) Handles Nodes.InvalidElement
        Elements.Delete(IDs, Nodes.ElementNodes(IDs))
    End Sub 'deletes elements if a node is deleted and leaves one hanging


    Public Class Material
        Public Name As String = ""
        Public E As Double = 0 ' youngs modulus in Pa

        Public ReadOnly Property E_GPa As Double
            Get
                Return (E / (1000 * 1000 * 1000)) 'convert to GPa
            End Get
        End Property
        Public Sub New(ByVal _Name As String, ByVal E_GPa As Double)
            Name = _Name
            E = E_GPa * 1000 * 1000 * 1000 'convert to Pa
        End Sub
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
        Public Sub Draw()

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
            GL.End()

        End Sub


    End Class

    Public Class Conectivity

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

    End Class

    Public Class Solver

        Public Function Assemble_K_Mtx(ByVal ElemList As List(Of ElementManager.Element_Bar1), ByVal Nodelist As List(Of NodeManager.Node)) As SparseMatrix
            Dim allNodeIDs As New List(Of Integer)
            Dim allElemIDs As List(Of Integer) = Elements.AllIDs
            Dim output As New SparseMatrix(Nodelist.Count * 3, Nodelist.Count * 3)
            Dim All_K_locals As New Dictionary(Of Integer, DenseMatrix)


            For Each N As NodeManager.Node In Nodelist 'get all node IDs and sort from smallest to largest
                allNodeIDs.Add(N.ID)
            Next
            allNodeIDs.Sort()

            Dim NodeMatrixIndicies As New Dictionary(Of Integer, Integer()) 'determines where each displacement will go on the assembled matrix

            For i = 0 To allNodeIDs.Count - 1
                NodeMatrixIndicies.Add(allNodeIDs(i), {i * 3, i * 3 + 1, i * 3 + 2}) 'allocate 3 spots for each node (x,y,z)
            Next

            For Each ID As Integer In allElemIDs
                All_K_locals.Add(ID, Elements.ElemObj(ID).K_matrix(Nodes.ElementNodes(ID))) 'add all K matrixes to array so they dont have to be recalculated.
            Next


            output.Clear() 'set all values to zero before writing

            For i As Integer = 0 To allNodeIDs.Count - 1 'iterate through all nodes in correct order by ID
                Dim attachedElems As List(Of Integer) = Nodes.NodeObj(allNodeIDs(i)).AttachedElements 'get element ID's attached to each node

                For Each EID As Integer In attachedElems 'iterate through each 

                Next
            Next

            For Each EID As Integer In allElemIDs 'iterate through each element
                Dim attachedNodes As List(Of Integer) = Nodes.ElementNodeIDs(EID)
                attachedNodes.Sort() 'sort from smallest ID to largest ID

                Dim Node1Indicies As Integer() = NodeMatrixIndicies(attachedNodes(0)) 'get indicies of first node (smallest ID)
                Dim Node2Indicies As Integer() = NodeMatrixIndicies(attachedNodes(1)) 'get inticies of second node

                '------------------------------- TOP LEFT CORNER OF LOCAL MATRIX ------------------------------
                For i As Integer = 0 To 2 'iterate through rows
                    For j As Integer = 0 To 2 'iterate through columns
                        output.Item(Node1Indicies(i), Node1Indicies(j)) += All_K_locals(EID).Item(i, j)
                    Next
                Next

                '------------------------------- TOP Right CORNER OF LOCAL MATRIX ------------------------------

                For i As Integer = 0 To 2 'iterate through rows
                    For j As Integer = 0 To 2 'iterate through columns
                        output.Item(Node1Indicies(i), Node2Indicies(j)) += All_K_locals(EID).Item(i, j + 3)
                    Next
                Next

                '------------------------------- Bottom LEFT CORNER OF LOCAL MATRIX ------------------------------

                For i As Integer = 0 To 2 'iterate through rows
                    For j As Integer = 0 To 2 'iterate through columns
                        output.Item(Node2Indicies(i), Node1Indicies(j)) += All_K_locals(EID).Item(i + 3, j)
                    Next
                Next

                '------------------------------- Bottom right CORNER OF LOCAL MATRIX ------------------------------

                For i As Integer = 0 To 2 'iterate through rows
                    For j As Integer = 0 To 2 'iterate through columns
                        output.Item(Node2Indicies(i), Node2Indicies(j)) += All_K_locals(EID).Item(i + 3, j + 3)
                    Next
                Next
            Next

            Return output
        End Function
        Public Function Assemble_K_Mtx2(ByVal Connectivity As Dictionary(Of Integer, List(Of Integer)), ByVal K_matricies As Dictionary(Of Integer, DenseMatrix), ByVal AllNodeIDs As List(Of Integer)) As SparseMatrix

            AllNodeIDs.Sort()

            Dim NodeMatrixIndicies As New Dictionary(Of Integer, Integer()) 'determines where each displacement will go on the assembled matrix
            For i = 0 To allNodeIDs.Count - 1
                NodeMatrixIndicies.Add(allNodeIDs(i), {i * 3, i * 3 + 1, i * 3 + 2}) 'allocate 3 spots for each node (x,y,z)
            Next

            Dim output As New SparseMatrix(AllNodeIDs.Count * 3, AllNodeIDs.Count * 3)
            output.Clear() 'set all values to zero before writing
            Return Nothing
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

    End Class
    Public Class NodeManager

        Private _Nodes As New Dictionary(Of Integer, Node) 'reference nodes by ID

        Public Event NodeListChanged(ByVal NodeList As Dictionary(Of Integer, Node)) 'Length of nodelist has changed
        Public Event NodeChanged(ByVal IDs As List(Of Integer)) 'Node has changed such that list needs to be updated & screen redrawn
        Public Event NodeChanged_RedrawOnly() 'Node has changed such that screen only needs to be redrawn
        Public Event InvalidElement(ByVal IDs As List(Of Integer)) 'if a node is delete and leaves a dangling element

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
        End Property
        Public ReadOnly Property ElementNodes(ByVal ElemIDs As List(Of Integer)) As List(Of Node())
            Get
                Dim output As New List(Of Node())

                For i As Integer = 0 To ElemIDs.Count - 1
                    Dim unsortedNodes As New List(Of Node)

                    For Each item As Node In _Nodes.Values
                        If item.AttachedElements.Contains(ElemIDs(i)) Then 'if element is attached to node then add
                            unsortedNodes.Add(item)
                        End If

                        If output.Count = 2 Then 'each element only has 2 nodes, no need to keep searching if they are found
                            Exit For
                        End If
                    Next


                    If unsortedNodes.Count = 2 Then 'Each element should always have 2 nodes
                        Dim sortedoutput As New List(Of Node)

                        If unsortedNodes(0).Coords(0) > unsortedNodes(1).Coords(0) Then 'make sure node with larger positive value always comes first
                            sortedoutput.Add(unsortedNodes(1))
                            sortedoutput.Add(unsortedNodes(0))
                        Else
                            If unsortedNodes(0).Coords(1) > unsortedNodes(1).Coords(1) Then 'check Y in case X vals equal
                                sortedoutput.Add(unsortedNodes(1))
                                sortedoutput.Add(unsortedNodes(0))
                            Else
                                If unsortedNodes(0).Coords(2) > unsortedNodes(1).Coords(2) Then 'check Z in case X,Y vals equal
                                    sortedoutput.Add(unsortedNodes(1))
                                    sortedoutput.Add(unsortedNodes(0))
                                Else
                                    sortedoutput.Add(unsortedNodes(0))
                                    sortedoutput.Add(unsortedNodes(1))
                                End If
                            End If
                        End If

                        output.Add(sortedoutput.ToArray)
                    Else
                        Return Nothing
                    End If

                Next
                Return output
            End Get
        End Property
        Public ReadOnly Property ElementNodes(ByVal ElemID As Integer) As Node()
            Get
                Dim inputList As New List(Of Integer) 'only need a list for the input type
                inputList.Add(ElemID)

                Dim output As List(Of Node()) = ElementNodes(inputList)

                Return output(0)
            End Get
        End Property
        Public ReadOnly Property ElementNodeIDs(ByVal ElemID As Integer) As List(Of Integer)
            Get
                Dim output As New List(Of Integer)

                For Each ND As Node In ElementNodes(ElemID) 'returns the actual node object - need to separate out the IDs
                    output.Add(ND.ID) 'add each ID to the list
                Next

                Return output
            End Get
        End Property
        Public ReadOnly Property Connectivity(ByVal ElemIDs As List(Of Integer)) As Dictionary(Of Integer, List(Of Integer))
            Get
                Dim output As New Dictionary(Of Integer, List(Of Integer))

                For Each ElemID As Integer In ElemIDs
                    Dim nodeIDs As New List(Of Integer)

                    For Each ND As Node In ElementNodes(ElemID) 'returns the actual node object - need to separate out the IDs
                        nodeIDs.Add(ND.ID) 'add each ID to the list
                    Next

                    output.Add(ElemID, nodeIDs)
                Next

                Return output
            End Get
        End Property 'returns the node IDs connected to each input element ID, indexed by element ID
        Public ReadOnly Property NodeObj(ByVal ID As Integer) As Node
            Get
                Return _Nodes(ID)
            End Get
        End Property
        Public ReadOnly Property F_Mtx() As DenseMatrix
            Get
                Dim output As New DenseMatrix(_Nodes.Values.Count * 3, 1)
                Dim allforcevalues As New List(Of Double)
                Dim ids As List(Of Integer) = AllIDs
                ids.Sort()

                For i As Integer = 0 To ids.Count - 1
                    allforcevalues.Add(_Nodes(ids(i)).Force(0))
                    allforcevalues.Add(_Nodes(ids(i)).Force(1))
                    allforcevalues.Add(_Nodes(ids(i)).Force(2))
                Next

                For i As Integer = 0 To allforcevalues.Count - 1
                    output(i, 0) = allforcevalues(i)
                Next

                Return output
            End Get
        End Property
        Public ReadOnly Property Q_Mtx() As DenseMatrix
            Get
                Dim output As New DenseMatrix(_Nodes.Values.Count * 3, 1)
                Dim ids As List(Of Integer) = AllIDs
                ids.Sort()

                Dim allQvalues As New List(Of Double) 'list to hold temporarily

                For i As Integer = 0 To ids.Count - 1
                    allQvalues.Add(_Nodes(ids(i)).Fixity(0))
                    allQvalues.Add(_Nodes(ids(i)).Fixity(1))
                    allQvalues.Add(_Nodes(ids(i)).Fixity(2))
                Next

                For i As Integer = 0 To allQvalues.Count - 1
                    output(i, 0) = allQvalues(i)
                Next

                Return output
            End Get
        End Property

        Public Sub SelectNodes(ByVal IDs As Integer(), ByVal selected As Boolean)
            For Each item As Integer In IDs
                _Nodes(item).Selected = selected
            Next
            RaiseEvent NodeChanged_RedrawOnly()
        End Sub
        Public Function AddNodes(ByVal Coords As List(Of Double()), ByVal Fixity As List(Of Double())) As List(Of Integer)
            Dim output As New List(Of Integer) 'want to return created ID's

            For i As Integer = 0 To Coords.Count - 1
                Dim newnode As New Node(Coords(i), Fixity(i), CreateNodeId)
                _Nodes.Add(newnode.ID, newnode)
                output.Add(newnode.ID) 'make sure to return ID's
            Next

            RaiseEvent NodeListChanged(_Nodes)
            Return output
        End Function
        Public Sub EditNode(ByVal Coords As List(Of Double()), ByVal fixity As List(Of Double()), ByVal IDs As List(Of Integer))
            For i As Integer = 0 To IDs.Count - 1
                _Nodes(IDs(i)).Coords = Coords(i)
                _Nodes(IDs(i)).Fixity = fixity(i)
            Next
            
            RaiseEvent NodeListChanged(_Nodes)
        End Sub
        Public Sub Delete(ByVal IDs As List(Of Integer))
            Dim HangingElemIDs As New List(Of Integer)

            For Each NodeID As Integer In IDs
                For Each ElemID As Integer In _Nodes(NodeID).AttachedElements
                    HangingElemIDs.Add(ElemID)
                Next
            Next

            If HangingElemIDs.Count > 0 Then 'elements need to be deleted - need to delete these before deleting nodes so element can unattach itself
                RaiseEvent InvalidElement(HangingElemIDs)
            End If

            For Each NodeID As Integer In IDs 'remove node from list
                _Nodes.Remove(NodeID)
            Next


            If HangingElemIDs.Count = 0 & IDs.Count > 0 Then 'if any are actually set to be deleted - also dont need to refresh if elements were deleted as they will anyway
                RaiseEvent NodeListChanged(_Nodes)
            End If

        End Sub
        Public Sub SetSolution(ByVal Q As DenseMatrix, ByVal R As DenseMatrix)
            Dim Ids As List(Of Integer) = AllIDs

            For i As Integer = 0 To AllIDs.Count - 1
                Dim reactions As New List(Of Double)
                Dim displacements As New List(Of Double)

                For j As Integer = 0 To 2
                    reactions.Add(R(3 * i + j, 0))
                    displacements.Add(Q(3 * i + j, 0))
                Next

                _Nodes(Ids(i)).Solve(displacements.ToArray, reactions.ToArray)
            Next
            RaiseEvent NodeChanged(Ids)
        End Sub
        Public Sub Addforce(ByVal force As List(Of Double()), ByVal IDs As List(Of Integer))
            For i As Integer = 0 To IDs.Count - 1
                _Nodes(IDs(i)).Force = force(i)
            Next

            RaiseEvent NodeChanged(IDs)
        End Sub


        Private Function CreateNodeId() As Integer
            Dim NewID As Integer = 1
            Dim IDUnique As Boolean = False

            While _Nodes.Keys.Contains(NewID)
                NewID += 1
            End While

            Return NewID
        End Function

        <Serializable>
        Public Class Node
            Private _Coords As Double() = {0, 0, 0} 'coordinates in m
            Private _Disp As Double() = {0, 0, 0} 'displacement in m
            Private _ID As Integer = 0

            Private _Force As Double() = {0, 0, 0} 'force in N
            Private _ReactionForce As Double() = {0, 0, 0} 'reaction force in N
            Private _Fixity As Double() = {0, 0, 0} ' 0 = floating, 1 = fixed

            Private _DefaultColor As Color = Color.Blue
            Private _DefaultForceColor As Color = Color.Purple
            Private _DefaultFixityColor As Color = Color.Red
            Private _FixityColor As Color = Color.Red
            Private _Color As Color = Color.Blue
            Private _ForceColor As Color = Color.Purple
            Private _SelectedColor As Color = Color.Yellow
            Private _ReactionColor As Color = Color.Green

            Private _AttachedElements As New List(Of Integer)

            Private _SolutionValid As Boolean = False

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
                    Dim output As Double() = {_Coords(0) * 1000, _Coords(1) * 1000, _Coords(2) * 1000}
                    Return output
                End Get
            End Property
            Public Property Coords As Double()
                Get
                    Return _Coords
                End Get
                Set(value As Double())
                    _Coords = value
                    _SolutionValid = False
                End Set
            End Property

            Public Property Force As Double()
                Get
                    Return _Force
                End Get
                Set(value As Double())
                    _Force = value
                    _SolutionValid = False
                End Set
            End Property
            Public Property Fixity As Double()
                Get
                    Return _Fixity
                End Get
                Set(value As Double())
                    _Fixity = value
                    _SolutionValid = False
                End Set
            End Property
            Public ReadOnly Property ForceMagnitude As Double
                Get
                    Dim output As Vector3 = New Vector3(_Force(0), _Force(1), _Force(2))
                    Return output.Length
                End Get
            End Property
            Public ReadOnly Property ForceDirection As Double()
                Get
                    Dim output As Vector3 = New Vector3(_Force(0), _Force(1), _Force(2))
                    output.Normalize()
                    Return {output.X, output.Y, output.Z}
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
                    Return {_Coords(0) + _Disp(0), _Coords(1) + _Disp(1), _Coords(2) + _Disp(2)}
                End Get
            End Property
            Public ReadOnly Property ID As Integer
                Get
                    Return _ID
                End Get
            End Property
            Public ReadOnly Property AttachedElements As List(Of Integer)
                Get
                    Return _AttachedElements
                End Get
            End Property

            Public Sub New(ByVal Coords As Double(), ByVal Fixity As Double(), ByVal ID As Integer)
                _SolutionValid = False
                _Coords = Coords
                _Fixity = Fixity
                _ID = ID
            End Sub

            Public Sub Solve(ByVal Disp As Double(), ByVal R As Double())
                _Disp = Disp
                _ReactionForce = R
                _SolutionValid = True
            End Sub
            Public Sub Draw()

                DrawNode(Coords_mm, _Color)
                DrawForce(Coords_mm, _Force, _ForceColor)
                DrawFixity(Coords_mm, _Fixity, _FixityColor)

            End Sub 'draw always has mm input
            Public Sub Draw_Manual(ByVal N_mm As Double())
                DrawNode(N_mm, _Color)
                DrawForce(N_mm, _Force, _ForceColor)
                DrawFixity(N_mm, _Fixity, _FixityColor)
            End Sub 'draw always has mm input
            Public Sub DrawReaction(ByVal N_mm As Double())
                DrawForce(N_mm, _ReactionForce, _ReactionColor)
            End Sub 'draw always has mm input
            Public Function CalcDispIncrementPos_mm(ByVal Percentage As Double, ByVal ScaleFactor As Double) As Double()
                Return {(_Coords(0) + _Disp(0) * Percentage * ScaleFactor) * 1000, (_Coords(1) + _Disp(1) * Percentage * ScaleFactor) * 1000, (_Coords(2) + _Disp(2) * Percentage * ScaleFactor) * 1000}
            End Function
            Public Sub AddtoElement(ByVal ID As Integer)
                _AttachedElements.Add(ID)
            End Sub
            Public Sub RemoveFromElement(ByVal ID As Integer)
                _AttachedElements.Remove(ID)
            End Sub

            Private Sub DrawNode(ByVal N As Double(), ByVal Color As Color)

                Dim tmp As Double() = N

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
                If F(0) <> 0 Or F(1) <> 0 Or F(2) <> 0 Then 'cant draw a force if its zero

                    Dim tmp As Double() = N

                    Dim forcelength As Double = 10.0

                    Dim vect As Vector3 = New Vector3(F(0), F(1), F(2))
                    vect.Normalize()

                    Dim X As Vector3 = New Vector3(1, 0, 0)
                    Dim normal1 As Vector3 = Vector3.Cross(vect, X)
                    normal1 = Vector3.Multiply(normal1, forcelength * 0.2)
                    Dim Y As Vector3 = New Vector3(0, 1, 0)
                    Dim normal2 As Vector3 = Vector3.Cross(vect, Y)
                    normal2 = Vector3.Multiply(normal2, forcelength * 0.2)

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
                GL.Color3(Color)

                Dim tmp As Double() = N

                If Fix(0) = 1 Then 'X Axis
                    GL.Begin(PrimitiveType.Quads)
                    GL.Vertex3(tmp(0), tmp(1) + squareoffset, tmp(2) + squareoffset)
                    GL.Vertex3(tmp(0), tmp(1) - squareoffset, tmp(2) + squareoffset)
                    GL.Vertex3(tmp(0), tmp(1) - squareoffset, tmp(2) - squareoffset)
                    GL.Vertex3(tmp(0), tmp(1) + squareoffset, tmp(2) - squareoffset)
                    GL.End()
                End If

                If Fix(1) = 1 Then 'Y Axis
                    GL.Begin(PrimitiveType.Quads)
                    GL.Vertex3(tmp(0) + squareoffset, tmp(1), tmp(2) + squareoffset)
                    GL.Vertex3(tmp(0) - squareoffset, tmp(1), tmp(2) + squareoffset)
                    GL.Vertex3(tmp(0) - squareoffset, tmp(1), tmp(2) - squareoffset)
                    GL.Vertex3(tmp(0) + squareoffset, tmp(1), tmp(2) - squareoffset)
                    GL.End()
                End If

                If Fix(2) = 1 Then 'Z Axis
                    GL.Begin(PrimitiveType.Quads)
                    GL.Vertex3(tmp(0) + squareoffset, tmp(1) + squareoffset, tmp(2))
                    GL.Vertex3(tmp(0) - squareoffset, tmp(1) + squareoffset, tmp(2))
                    GL.Vertex3(tmp(0) - squareoffset, tmp(1) - squareoffset, tmp(2))
                    GL.Vertex3(tmp(0) + squareoffset, tmp(1) - squareoffset, tmp(2))
                    GL.End()
                End If
            End Sub 'draw always has mm input

        End Class
    End Class
    Public Class ElementManager

        Private _Bar1Elements As New Dictionary(Of Integer, Element_Bar1) 'reference elements by ID

        Public Event ElementListChanged(ByVal ElemList As Dictionary(Of Integer, Element_Bar1)) 'Length of Elementlist has changed
        Public Event ElementChanged(ByVal ID As Integer) 'Element has changed such that list needs to be updated & screen redrawn
        Public Event ElementChanged_RedrawOnly() 'Element has changed such that screen only needs to be redrawn

        Public ReadOnly Property Elemlist As List(Of Element_Bar1)
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
        Public ReadOnly Property ElemObj(ByVal ElemID As String) As Element_Bar1
            Get
                Return _Bar1Elements(ElemID)
            End Get
        End Property
        Public ReadOnly Property K_matricies(ByVal ELemIDs As List(Of Integer), ElemNodes As List(Of NodeManager.Node())) As Dictionary(Of Integer, DenseMatrix)
            Get
                Dim output As New Dictionary(Of Integer, DenseMatrix)

                For i As Integer = 0 To ELemIDs.Count - 1
                    output.Add(ELemIDs(i), _Bar1Elements(ELemIDs(i)).K_matrix(ElemNodes(i)))
                Next

                Return output
            End Get
        End Property


        Public Function Add(ByRef Node1 As NodeManager.Node, ByRef Node2 As NodeManager.Node, ByVal Area As Double, ByVal E As Double) As Integer
            Dim newElem As New Element_Bar1(Node1, Node2, Area, E, CreateId())
            _Bar1Elements.Add(newElem.ID, newElem)
            RaiseEvent ElementListChanged(_Bar1Elements)
            Return newElem.ID() 'return the created ID
        End Function
        Public Sub SelectElems(ByVal IDs As Integer(), ByVal selected As Boolean)
            For Each item As Integer In IDs
                _Bar1Elements(item).Selected = selected
            Next
            RaiseEvent ElementChanged_RedrawOnly()
        End Sub
        Public Sub Delete(ByVal IDs As List(Of Integer), ByRef ElemNodelist As List(Of NodeManager.Node()))
            For i As Integer = 0 To IDs.Count - 1
                _Bar1Elements.Remove(IDs(i))
                ElemNodelist(i)(0).RemoveFromElement(IDs(i))
                ElemNodelist(i)(1).RemoveFromElement(IDs(i))
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


        <Serializable>
        Public Class Element_Bar1
            Inherits Element

            Private _Area As Double
            Private _YoungsMod As Double

            Public ReadOnly Property E As Double
                Get
                    Return _YoungsMod
                End Get
            End Property 'youngs modulus in Pa
            Public ReadOnly Property A As Double
                Get
                    Return _Area
                End Get
            End Property 'x-section area in m^2


            Public ReadOnly Property Length(ByVal N As NodeManager.Node()) As Double
                Get
                    Dim _node1 As NodeManager.Node = N(0)
                    Dim _node2 As NodeManager.Node = N(1)
                    Return Math.Sqrt(Math.Pow(_node2.Coords(0) - _node1.Coords(0), 2) + Math.Pow(_node2.Coords(1) - _node1.Coords(1), 2) + Math.Pow(_node2.Coords(2) - _node1.Coords(2), 2))
                End Get
            End Property 'element length in m
            Public ReadOnly Property K_matrix(ByVal Nodes As NodeManager.Node()) As DenseMatrix
                Get
                    Dim L_tmp As Double = L(Nodes) 'perform once to save resources
                    Dim M_tmp As Double = M(Nodes)
                    Dim N_tmp As Double = N(Nodes)
                    Dim Length_tmp As Double = Length(Nodes)

                    Dim L_Mtx As New DenseMatrix(2, 6) 'set up transformation matrix
                    L_Mtx.Item(0, 0) = L_tmp
                    L_Mtx.Item(0, 1) = M_tmp
                    L_Mtx.Item(0, 2) = N_tmp
                    L_Mtx.Item(0, 3) = 0
                    L_Mtx.Item(0, 4) = 0
                    L_Mtx.Item(0, 5) = 0
                    L_Mtx.Item(1, 0) = 0
                    L_Mtx.Item(1, 1) = 0
                    L_Mtx.Item(1, 2) = 0
                    L_Mtx.Item(1, 3) = L_tmp
                    L_Mtx.Item(1, 4) = M_tmp
                    L_Mtx.Item(1, 5) = N_tmp

                    Dim K_Mtx_local As New DenseMatrix(2, 2) 'set up local element matrix
                    K_Mtx_local.Item(0, 0) = 1
                    K_Mtx_local.Item(0, 1) = -1
                    K_Mtx_local.Item(1, 0) = -1
                    K_Mtx_local.Item(1, 1) = 1
                    K_Mtx_local = K_Mtx_local * (E * A / Length_tmp)

                    Dim K_mtx_global As DenseMatrix = L_Mtx.Transpose * K_Mtx_local * L_Mtx  'K_global = L(transpose)*K_local*L
                    Return K_mtx_global
                End Get
            End Property 'global stiffness matrix (N/m)
            Public ReadOnly Property Stress(ByVal NDS As NodeManager.Node()) As Double
                Get
                    Dim _node1 As NodeManager.Node = NDS(0)
                    Dim _node2 As NodeManager.Node = NDS(1)

                    Dim output As Double = (((_node2.Displacement(0) - _node1.Displacement(0)) * L(NDS)) + ((_node2.Displacement(1) - _node1.Displacement(1)) * M(NDS)) + ((_node2.Displacement(2) - _node1.Displacement(2)) * N(NDS))) * E / L(NDS)

                    Return output
                End Get
            End Property  'elemental stress in Pa
            Private ReadOnly Property L(ByVal N As NodeManager.Node()) As Double
                Get
                    Dim _node1 As NodeManager.Node = N(0)
                    Dim _node2 As NodeManager.Node = N(1)
                    Return (_node2.Coords(0) - _node1.Coords(0)) / Length(N)
                End Get
            End Property
            Private ReadOnly Property M(ByVal N As NodeManager.Node()) As Double
                Get
                    Dim _node1 As NodeManager.Node = N(0)
                    Dim _node2 As NodeManager.Node = N(1)
                    Return (_node2.Coords(1) - _node1.Coords(1)) / Length(N)
                End Get
            End Property
            Private ReadOnly Property N(ByVal ND As NodeManager.Node()) As Double
                Get
                    Dim _node1 As NodeManager.Node = ND(0)
                    Dim _node2 As NodeManager.Node = ND(1)
                    Return (_node2.Coords(2) - _node1.Coords(2)) / Length(ND)
                End Get
            End Property



            Public Sub New(ByRef Node1 As NodeManager.Node, ByRef Node2 As NodeManager.Node, ByVal Area As Double, ByVal E_Pa As Double, ByVal ID As Integer)
                MyBase.New(ElementType.Bar1, 2, Drawing.Color.Green, ID)

                Node1.AddtoElement(ID)
                Node2.AddtoElement(ID)

                _Area = Area
                _YoungsMod = E_Pa

            End Sub
            Public Sub Draw(ByVal N As NodeManager.Node())
                Dim _node1 As NodeManager.Node = N(0)
                Dim _node2 As NodeManager.Node = N(1)

                GL.Color3(_Color)
                GL.Begin(PrimitiveType.Lines)
                GL.Vertex3(_node1.Coords_mm(0), _node1.Coords_mm(1), _node1.Coords_mm(2)) 'always draw in mm
                GL.Vertex3(_node2.Coords_mm(0), _node2.Coords_mm(1), _node2.Coords_mm(2)) 'always draw in mm
                GL.End()
            End Sub 'draw always has mm input
            Public Sub Draw_manual(ByVal N1_mm As Double(), ByVal N2_mm As Double())

                GL.Color3(_Color)
                GL.Begin(PrimitiveType.Lines)
                GL.Vertex3(N1_mm(0), N1_mm(1), N1_mm(2))
                GL.Vertex3(N2_mm(0), N2_mm(1), N2_mm(2))
                GL.End()
            End Sub 'draw always has mm input

        End Class
        <Serializable> Class Element
            Private _ID As Integer
            Private _DefaultColor As Color
            Protected _Color As Color
            Private _SelectedColor As Color = Color.Yellow
            Private _Type As ElementType
            Private _NumOfNodes As Integer

            Public ReadOnly Property ID As Integer
                Get
                    Return _ID
                End Get
            End Property
            Public ReadOnly Property Color As Color
                Get
                    Return _Color
                End Get
            End Property
            Public ReadOnly Property SelectedColor As Color
                Get
                    Return _SelectedColor
                End Get
            End Property
            Public ReadOnly Property Type As ElementType
                Get
                    Return _Type
                End Get
            End Property
            Public ReadOnly Property NumOfNodes As Integer
                Get
                    Return _NumOfNodes
                End Get
            End Property
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
                    Else
                        If _Color = _SelectedColor Then
                            _Color = _DefaultColor
                        End If
                    End If
                End Set
            End Property

            Public Sub New(ByVal Type As ElementType, ByVal NumNodes As Integer, ByVal Color As Color, ByVal ID As Integer)
                _Type = Type
                _DefaultColor = Color
                _Color = Color
                _NumOfNodes = NumNodes
                _ID = ID
            End Sub

        End Class
        Public Enum ElementType
            Bar1 = 0
        End Enum

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


    Public Function ExportData(ByVal NodeList As List(Of NodeManager.Node), ByVal ElementList As List(Of ElementManager.Element_Bar1)) As List(Of String)

        Dim stringdata As New List(Of String)

        For Each Node As NodeManager.Node In NodeList
            stringdata.Add("<NODE> " & CStr(Node.ID) & "|" & String.Join(",", Node.Coords) & "|" & String.Join(",", Node.Fixity) & "|" & String.Join(",", Node.Force) & "|" & String.Join(",", Node.AttachedElements))
        Next

        For Each Element As ElementManager.Element_Bar1 In ElementList
            stringdata.Add("<ELEMENT> " & CStr(Element.ID) & "|" & Element.A & "|" & Element.E & "|" & Element.NumOfNodes & "|" & Element.Type.ToString)
        Next

        Return stringdata
    End Function
    Public Sub ImportData(ByVal data As List(Of String))

        Nodes.Delete(Nodes.AllIDs) 'this will delete all nodes and subsequently elements

        Dim _coordsList As New List(Of Double())
        Dim _fixitylist As New List(Of Double())
        Dim _forceList As New List(Of Double())
        Dim _attachedElements As New List(Of Double())

        Dim _elemIDlist As New List(Of Integer)
        Dim _AreaList As New List(Of Double)
        Dim _YoungsList As New List(Of Double)

        For Each line As String In data
            If line.Split(" ").First = "<NODE>" Then
                Dim NodeData As String() = line.Split(" ").Last.Split("|")

                _coordsList.Add(StringArrToDoubleArr(NodeData(1).Split(",")))
                _fixitylist.Add(StringArrToDoubleArr(NodeData(2).Split(",")))
                _forceList.Add(StringArrToDoubleArr(NodeData(3).Split(",")))
                _attachedElements.Add(StringArrToDoubleArr(NodeData(4).Split(",")))

                ' Nodes.AddNodes(_coordsList, _fixitylist)

            ElseIf line.Split(" ").First = "<ELEMENT>" Then
                Dim ElementData As String() = line.Split(" ").Last.Split("|")

                _elemIDlist.Add(CInt(ElementData(0)))
                _AreaList.Add(CDbl(ElementData(1)))
                _YoungsList.Add(CDbl(ElementData(2)))

            End If
        Next


        Dim NodeIDlist As List(Of Integer) = Nodes.AddNodes(_coordsList, _fixitylist) 'need the new IDs to add elements
        Nodes.Addforce(_forceList, NodeIDlist)

        For i As Integer = 0 To _elemIDlist.Count - 1
            Dim attachednodeIDs As New List(Of Integer)

            For j = 0 To _attachedElements.Count - 1
                If _attachedElements(j).Contains(_elemIDlist(i)) Then
                    attachednodeIDs.Add(NodeIDlist(j))
                End If
            Next

            Elements.Add(Nodes.NodeObj(attachednodeIDs(0)), Nodes.NodeObj(attachednodeIDs(1)), _AreaList(i), _YoungsList(i))

        Next
    End Sub
    Public Function TextFileToStringList(ByVal FilePath As String) As List(Of String)

        Dim fileReader As System.IO.StreamReader
        fileReader = My.Computer.FileSystem.OpenTextFileReader(FilePath)

        Dim data As New List(Of String)

        While True
            Dim tmp As String = fileReader.ReadLine

            If tmp <> "" Then
                data.Add(tmp)
            Else
                Exit While
            End If

        End While

        fileReader.Close()
        Return data
    End Function
    Public Sub StringListToTextFile(ByVal Data As List(Of String), ByVal SavePath As String)

        If File.Exists(SavePath) Then 'check if file exists
            Dim result2 As DialogResult = MessageBox.Show("This file already exists. Overwrite?", "Overwrite file?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
            If result2 = DialogResult.No Then
                Return
            End If
        End If

        'if user wants to overwrite then continue

        Dim STwriter As System.IO.StreamWriter
        STwriter = My.Computer.FileSystem.OpenTextFileWriter(SavePath, False)

        For Each s As String In Data
            STwriter.WriteLine(s)
        Next

        STwriter.Close()
    End Sub

    Public Sub STLtoVertexes(ByVal FilePath As String)
        Dim fileReader As System.IO.StreamReader
        fileReader = My.Computer.FileSystem.OpenTextFileReader(FilePath)

        Dim _coordslist As New List(Of Double())
        Dim _fixitylist As New List(Of Double())


        While True
            Dim tmp As String = fileReader.ReadLine

            If tmp <> "endsolid" Then

                Dim tmp2 As String() = tmp.Split(" ")

                If tmp2.Length >= 13 Then
                    If tmp2(9) = "vertex" Then
                        Dim coords As Double() = {CDbl(tmp2(10)) / 1000.0, CDbl(tmp2(11)) / 1000.0, CDbl(tmp2(12)) / 1000.0}
                        _coordslist.Add(coords)
                        _fixitylist.Add({1, 1, 1})
                    End If
                End If

            Else
                Exit While
            End If

        End While
        MessageBox.Show("Done Parse")
        Nodes.AddNodes(_coordslist, _fixitylist)

        fileReader.Close()





    End Sub



    Public Sub ObjectToXML(ByVal SaveData As Object, ByVal SavePath As String, Optional ByVal FileName As String = Nothing)
        If FileName <> Nothing Then
            SavePath = SavePath & "\" & FileName
        End If

        Dim ser As New XmlSerializer(SaveData.GetType)
        Dim fs As New FileStream(SavePath, FileMode.Create)
        ser.Serialize(fs, SaveData)
        fs.Close()
    End Sub
    Public Sub XMLToObject(ByVal LoadPath As String, ByRef LoadData As Object)
        If File.Exists(LoadPath) Then
            Dim ser As New XmlSerializer(LoadData.GetType)
            Dim fs As New FileStream(LoadPath, FileMode.OpenOrCreate)
            LoadData = DirectCast(ser.Deserialize(fs), Object)
            fs.Close()
        End If
    End Sub

    Public Function StringArrToDoubleArr(ByVal input As String()) As Double()
        Dim output As New List(Of Double)

        For Each Val As String In input
            output.Add(CDbl(Val))
        Next

        Return output.ToArray
    End Function


End Module
