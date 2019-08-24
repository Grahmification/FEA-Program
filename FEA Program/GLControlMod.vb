Imports OpenTK
Imports OpenTK.Graphics
Imports OpenTK.Graphics.OpenGL
Imports OpenTK.Input

Public Class GLControlMod
    Inherits GLControl

    Private _backcolor As Color
    Private _view As New View
    Private _Orientation As Matrix4 = Matrix4.Identity

    Public Event DrawStuffBeforeOrientation(ByVal ThreeDimensional As Boolean)
    Public Event ViewUpdated(ByVal Trans As Vector3, ByVal rot As Matrix4, ByVal zoom As Vector3, ByVal ThreeDimensional As Boolean)
    Public Event DrawStuffAfterOrientation(ByVal ThreeDimensional As Boolean)

    Private _MouseRotMultiplier As Double = 0.05
    Private _MouseTransMultiplier As Double = 0.3
    Private _MouseZoomMultiplier As Double = 0.05

    Private _ThreeDimensional As Boolean = False 'if false sets viewing to 2D style

    Public Property ThreeDimensional As Boolean
        Get
            Return _ThreeDimensional
        End Get
        Set(value As Boolean)
            _ThreeDimensional = value
            _view = New View 'reset the view
            Me.Invalidate()
        End Set
    End Property

    Public Sub New(ByVal BackColor As Color, ByVal ThreeDimensional As Boolean)
        _backcolor = BackColor
        _ThreeDimensional = ThreeDimensional
        Input.Initialize(Me)
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        Me.OnPaint(Nothing)
    End Sub
    Protected Overrides Sub OnMouseMove(e As Windows.Forms.MouseEventArgs)
        MyBase.OnMouseMove(e)

        If Input.buttonDown(Windows.Forms.MouseButtons.Left) Then 'screen translation
            Dim mousevector As Vector2 = Input.MouseLastVector(e.X, e.Y)
            _view.SetOrientation_Relative(New Vector3(mousevector.X * _MouseTransMultiplier, mousevector.Y * -_MouseTransMultiplier, 0), Matrix4.Identity, Vector3.Zero, True)

            Me.Invalidate()

        ElseIf Input.buttonDown(Windows.Forms.MouseButtons.Right) And _ThreeDimensional Then 'only rotate in 3D mode
            Dim MouseRotVector As Vector2 = Input.MouseLastRotationVector(e.X, e.Y)
            Dim RotInput As Matrix4 = Matrix4.CreateFromAxisAngle(New Vector3(MouseRotVector.Y, MouseRotVector.X, 0), _MouseRotMultiplier)
            _view.SetOrientation_Relative(Vector3.Zero, RotInput, Vector3.Zero, True)

            Me.Invalidate()
        End If

        Input.update(e.X, e.Y)
    End Sub
    Protected Overrides Sub OnMouseWheel(e As Windows.Forms.MouseEventArgs)
        MyBase.OnMouseWheel(e)

        If e.Delta > 0 Then
            _view.SetOrientation_Relative(Vector3.Zero, Matrix4.Identity, New Vector3(_MouseZoomMultiplier, _MouseZoomMultiplier, _MouseZoomMultiplier), True)
        Else
            _view.SetOrientation_Relative(Vector3.Zero, Matrix4.Identity, New Vector3(-_MouseZoomMultiplier, -_MouseZoomMultiplier, -_MouseZoomMultiplier), True)
        End If

        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        _view.PrepCamera(Me.Width, Me.Height, _backcolor, _ThreeDimensional)
        RaiseEvent DrawStuffBeforeOrientation(_ThreeDimensional)

        _Orientation = _view.ApplyView()
        RaiseEvent ViewUpdated(_view.CurrentTransLation, _view.CurrentRotation, _view.CurrentZoom, _ThreeDimensional)
        RaiseEvent DrawStuffAfterOrientation(_ThreeDimensional)

        GraphicsContext.CurrentContext.SwapInterval = True
        Me.SwapBuffers()

    End Sub


End Class

Public Class View

    Private _Trans_Current As Vector3 = Vector3.Zero
    Private _Trans_GoTo As Vector3 = Vector3.Zero
    Private _Rot_Current As Matrix4 = Matrix4.CreateRotationX(0) 'default to zero rotation
    Private _Rot_GoTo As Matrix4 = Matrix4.CreateRotationX(0) 'default to zero rotation
    Private _Zoom_Current As Vector3 = New Vector3(1, 1, 1)
    Private _Zoom_GoTo As Vector3 = New Vector3(1, 1, 1)

    Public ReadOnly Property CurrentTransLation As Vector3
        Get
            Return _Trans_Current
        End Get
    End Property
    Public ReadOnly Property CurrentRotation As Matrix4
        Get
            Return _Rot_Current
        End Get
    End Property
    Public ReadOnly Property CurrentZoom As Vector3
        Get
            Return _Zoom_Current
        End Get
    End Property


    Public Sub PrepCamera(ByVal Screenwidth As Integer, ByVal ScreenHeight As Integer, ByVal _backcolor As Color, ByVal ThreeDimensional As Boolean)
        'First Clear Buffers
        GL.Clear(ClearBufferMask.ColorBufferBit)
        GL.Clear(ClearBufferMask.DepthBufferBit)


        'Basic Setup for viewing

        Dim perspective As Matrix4 = Matrix4.CreateOrthographic(Screenwidth, ScreenHeight, 0.001, 1000) 'Setup 2D Perspective
        If ThreeDimensional Then
            perspective = Matrix4.CreatePerspectiveFieldOfView(1.1, 4 / 3, 1, 10000) 'Change to 3D perspective if desired
        End If

        GL.MatrixMode(MatrixMode.Projection) 'Load Perspective
        GL.LoadIdentity()
        GL.LoadMatrix(perspective)

        Dim lookat As Matrix4 = Matrix4.LookAt(0, 0, 100, 0, 0, 0, 0, 1, 0) 'Setup camera
        GL.MatrixMode(MatrixMode.Modelview) 'Load Camera
        GL.LoadIdentity()
        GL.LoadMatrix(lookat)

        GL.ClearColor(_backcolor) 'set background color

        GL.Viewport(0, 0, Screenwidth, ScreenHeight) 'Size of window
        GL.Enable(EnableCap.DepthTest) 'Enable correct Z Drawings
        GL.DepthFunc(DepthFunction.Less) 'Enable correct Z Drawings

    End Sub

    Public Sub SetOrientation_ABS(ByVal Trans As Vector3, ByVal Rot As Matrix4, ByVal Zoom As Vector3)

        _Trans_GoTo = Trans
        _Rot_GoTo = Rot
        _Zoom_GoTo = Zoom

    End Sub
    Public Sub SetOrientation_Relative(ByVal Trans As Vector3, ByVal Rot As Matrix4, ByVal Zoom As Vector3, ByVal rotRelativeToScreen As Boolean)

        _Trans_GoTo = _Trans_Current + Trans

        If rotRelativeToScreen Then
            _Rot_GoTo = _Rot_Current * Rot
        Else
            _Rot_GoTo = Rot * _Rot_Current
        End If

        If (_Zoom_Current.X + Zoom.X) > 0 And (_Zoom_Current.Y + Zoom.Y) > 0 And (_Zoom_Current.Z + Zoom.Z) > 0 Then 'don't allow zoom to go negative
            _Zoom_GoTo = _Zoom_Current + Zoom
        End If

    End Sub
    Public Function ApplyView() As Matrix4
        Dim xform As Matrix4 = Matrix4.Identity

        xform = Matrix4.Mult(xform, _Rot_GoTo)
        xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(_Trans_GoTo.X, _Trans_GoTo.Y, _Trans_GoTo.Z))
        xform = Matrix4.Mult(xform, Matrix4.CreateScale(_Zoom_Current.X, _Zoom_Current.Y, _Zoom_Current.Z))

        GL.MultMatrix(xform)

        _Trans_Current = _Trans_GoTo
        _Rot_Current = _Rot_GoTo
        _Zoom_Current = _Zoom_GoTo

        Return xform
    End Function

End Class


Public Class SpriteBatch


    Public Shared Sub DrawArrow(ByVal length As Double, ByVal direction As Vector3, ByVal position As Vector3, ByVal threeDimensional As Boolean, ByVal color As Color, Optional ByVal linethickness As Integer = 1)

        LoadOrientation(position, VectorToRotationMtx(direction), length)

        GL.LineWidth(linethickness)

        GL.Color3(color)
        GL.Begin(PrimitiveType.Lines)
        GL.Vertex3(0, 0, 0)
        GL.Vertex3(0, 0, 1)
        GL.End()

        GL.Begin(PrimitiveType.Triangles)
        If threeDimensional Then

            GL.Vertex3(0, 0, 1)
            GL.Vertex3(0.1, 0.1, 0.8)
            GL.Vertex3(0.1, -0.1, 0.8)

            GL.Vertex3(0, 0, 1)
            GL.Vertex3(-0.1, 0.1, 0.8)
            GL.Vertex3(-0.1, -0.1, 0.8)

            GL.Vertex3(0, 0, 1)
            GL.Vertex3(0.1, -0.1, 0.8)
            GL.Vertex3(-0.1, -0.1, 0.8)

            GL.Vertex3(0, 0, 1)
            GL.Vertex3(0.1, 0.1, 0.8)
            GL.Vertex3(-0.1, 0.1, 0.8)

        Else
            GL.Vertex3(0, 0, 1)
            GL.Vertex3(0.1, 0, 0.8)
            GL.Vertex3(-0.1, 0, 0.8)
        End If
        GL.End()

        GL.LineWidth(1)

        RevertOrientation(position, VectorToRotationMtx(direction), length)

    End Sub


    Private Shared Sub LoadOrientation(ByVal Trans As Vector3, ByVal Rot As Matrix4, ByVal zoom As Double)
        Dim xform As Matrix4 = Matrix4.Identity
        xform = Matrix4.Mult(xform, Rot)
        xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(Trans / zoom))
        xform = Matrix4.Mult(xform, Matrix4.CreateScale(zoom))
        GL.MultMatrix(xform)
    End Sub
    Private Shared Sub RevertOrientation(ByVal Trans As Vector3, ByVal Rot As Matrix4, ByVal zoom As Double)
        Dim xform As Matrix4 = Matrix4.Identity
        xform = Matrix4.Mult(xform, Matrix4.CreateScale(1 / zoom))
        xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(-1 * Trans / zoom))
        xform = Matrix4.Mult(xform, Rot.Inverted())
        GL.MultMatrix(xform)
    End Sub

    Private Shared Function VectorToRotationMtx(ByVal V As Vector3) As Matrix4

        Dim angle As Double = Math.Acos(Vector3.Dot(V, Vector3.UnitZ) / V.Length)

        Return Matrix4.CreateFromAxisAngle(New Vector3(V.Y, -V.X, 0), -angle)
    End Function



End Class

Public Class Input

    Private Shared keysDown As List(Of Key) = New List(Of Key)
    Private Shared keysDownLast As List(Of Key) = New List(Of Key)
    Private Shared buttonsDown As List(Of MouseButtons) = New List(Of MouseButtons)
    Private Shared buttonsDownLast As List(Of MouseButtons) = New List(Of MouseButtons)

    Private Shared MouseDown As Vector2 = Vector2.Zero 'point where the mouse was when clicked
    Private Shared MouseLast As Vector2 = Vector2.Zero 'point where the mouse was when last updated

    Public Shared Sub Initialize(ByVal game As GLControl)
        AddHandler game.KeyDown, AddressOf game_keydown
        AddHandler game.KeyUp, AddressOf game_keyUp
        AddHandler game.MouseDown, AddressOf game_mouseDown
        AddHandler game.MouseUp, AddressOf game_mouseUp
    End Sub

    Private Shared Sub game_keydown(sender As Object, e As KeyEventArgs)
        keysDown.Add(e.KeyData)
    End Sub
    Private Shared Sub game_keyUp(sender As Object, e As KeyEventArgs)
        While keysDown.Contains(e.KeyData)
            keysDown.Remove(e.KeyData)
        End While
    End Sub
    Private Shared Sub game_mouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs)
        buttonsDown.Add(e.Button)
        MouseDown = New Vector2(e.X, e.Y)
    End Sub
    Private Shared Sub game_mouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs)
        While buttonsDown.Contains(e.Button)
            buttonsDown.Remove(e.Button)
        End While
    End Sub

    Public Shared Sub update(ByVal X As Integer, ByVal Y As Integer)
        keysDownLast = New List(Of Key)(keysDown)
        buttonsDownLast = buttonsDown
        MouseLast = New Vector2(X, Y)
    End Sub

    Public Shared Function MouseLastVector(ByVal CurrentX As Integer, ByVal CurrentY As Integer) As Vector2
        Return New Vector2(CurrentX, CurrentY) - MouseLast
    End Function
    Public Shared Function MouseLastRotationVector(ByVal CurrentX As Integer, ByVal CurrentY As Integer) As Vector2
        Dim currentpos As Vector2 = New Vector2(CurrentX, CurrentY)
        currentpos -= MouseLast
        currentpos.Normalize()
        Return New Vector2(currentpos.X, currentpos.Y)
    End Function
    Public Shared Function MouseDownVector(ByVal CurrentX As Integer, ByVal CurrentY As Integer) As Vector2
        Return New Vector2(CurrentX, CurrentY) - MouseDown
    End Function

    Public Shared Function keyPress(ByVal key As Key) As Boolean
        Return (keysDown.Contains(key) And Not keysDownLast.Contains(key))
    End Function
    Public Shared Function keyRelease(ByVal key As Key) As Boolean
        Return (Not keysDown.Contains(key) And keysDownLast.Contains(key))
    End Function
    Public Shared Function keyDown(ByVal key As Key) As Boolean
        Return (keysDown.Contains(key))
    End Function

    Public Shared Function buttonPress(ByVal button As MouseButtons) As Boolean
        Return (buttonsDown.Contains(button) And Not buttonsDownLast.Contains(button))
    End Function
    Public Shared Function buttonRelease(ByVal button As MouseButtons) As Boolean
        Return (Not buttonsDown.Contains(button) And buttonsDownLast.Contains(button))
    End Function
    Public Shared Function buttonDown(ByVal button As MouseButtons) As Boolean
        Return (buttonsDown.Contains(button))
    End Function

End Class 'keyboard/mouse inputs