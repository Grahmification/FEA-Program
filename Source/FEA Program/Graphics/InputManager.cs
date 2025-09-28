using OpenTK.GLControl;
using OpenTK.Mathematics;

namespace FEA_Program.Graphics
{
    /// <summary>
    /// Handles mouse/keyboard inputs, allowing multiple button pressed to work simultaneously press/releases to be tracked
    /// </summary>
    internal class InputManager
    {
        private static List<Keys> keysDown = [];
        private static List<Keys> keysDownLast = [];
        private static List<MouseButtons> buttonsDown = [];
        private static List<MouseButtons> buttonsDownLast = [];

        private static Vector2 _mouseDownCoords = Vector2.Zero; // point where the mouse was when clicked
        private static Vector2 _mouseLastCoords = Vector2.Zero; // point where the mouse was when last updated

        public static void Initialize(GLControl control)
        {
            control.KeyDown += OnKeydown;
            control.KeyUp += OnKeyUp;
            control.MouseDown += OnMouseDown;
            control.MouseUp += OnMouseUp;
        }

        private static void OnKeydown(object? sender, KeyEventArgs e)
        {
            keysDown.Add(e.KeyData);
        }
        private static void OnKeyUp(object? sender, KeyEventArgs e)
        {
            while (keysDown.Contains(e.KeyData))
                keysDown.Remove(e.KeyData);
        }
        private static void OnMouseDown(object? sender, MouseEventArgs e)
        {
            buttonsDown.Add(e.Button);
            _mouseDownCoords = new Vector2(e.X, e.Y);
        }
        private static void OnMouseUp(object? sender, MouseEventArgs e)
        {
            while (buttonsDown.Contains(e.Button))
                buttonsDown.Remove(e.Button);
        }

        public static void Update(int mouseX, int mouseY)
        {
            keysDownLast = new List<Keys>(keysDown);
            buttonsDownLast = buttonsDown;
            _mouseLastCoords = new Vector2(mouseX, mouseY);
        }

        public static Vector2 MouseLastVector(int CurrentX, int CurrentY)
        {
            return new Vector2(CurrentX, CurrentY) - _mouseLastCoords;
        }
        public static Vector2 MouseLastRotationVector(int CurrentX, int CurrentY)
        {
            var currentpos = new Vector2(CurrentX, CurrentY);
            currentpos -= _mouseLastCoords;
            currentpos.Normalize();
            return new Vector2(currentpos.X, currentpos.Y);
        }
        public static Vector2 MouseDownVector(int CurrentX, int CurrentY)
        {
            return new Vector2(CurrentX, CurrentY) - _mouseDownCoords;
        }

        public static bool KeyPressOccurred(Keys key)
        {
            return keysDown.Contains(key) & !keysDownLast.Contains(key);
        }
        public static bool KeyReleaseOccurred(Keys key)
        {
            return !keysDown.Contains(key) & keysDownLast.Contains(key);
        }
        public static bool IsKeyDown(Keys key)
        {
            return keysDown.Contains(key);
        }

        public static bool ButtonPressOccurred(MouseButtons button)
        {
            return buttonsDown.Contains(button) & !buttonsDownLast.Contains(button);
        }
        public static bool ButtonReleaseOccurred(MouseButtons button)
        {
            return !buttonsDown.Contains(button) & buttonsDownLast.Contains(button);
        }
        public static bool IsButtonDown(MouseButtons button)
        {
            return buttonsDown.Contains(button);
        }
    }
}
