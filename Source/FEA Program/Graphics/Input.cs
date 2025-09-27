using OpenTK.GLControl;
using OpenTK.Mathematics;

namespace FEA_Program.Graphics
{
    /// <summary>
    /// Handles GLControl mouse/keyboard inputs
    /// </summary>
    internal class Input
    {
        private static List<Keys> keysDown = new List<Keys>();
        private static List<Keys> keysDownLast = new List<Keys>();
        private static List<MouseButtons> buttonsDown = new List<MouseButtons>();
        private static List<MouseButtons> buttonsDownLast = new List<MouseButtons>();

        private static Vector2 MouseDown = Vector2.Zero; // point where the mouse was when clicked
        private static Vector2 MouseLast = Vector2.Zero; // point where the mouse was when last updated

        public static void Initialize(GLControl game)
        {
            game.KeyDown += game_keydown;
            game.KeyUp += game_keyUp;
            game.MouseDown += game_mouseDown;
            game.MouseUp += game_mouseUp;
        }

        private static void game_keydown(object sender, KeyEventArgs e)
        {
            keysDown.Add(e.KeyData);
        }
        private static void game_keyUp(object sender, KeyEventArgs e)
        {
            while (keysDown.Contains(e.KeyData))
                keysDown.Remove(e.KeyData);
        }
        private static void game_mouseDown(object sender, MouseEventArgs e)
        {
            buttonsDown.Add(e.Button);
            MouseDown = new Vector2(e.X, e.Y);
        }
        private static void game_mouseUp(object sender, MouseEventArgs e)
        {
            while (buttonsDown.Contains(e.Button))
                buttonsDown.Remove(e.Button);
        }

        public static void update(int X, int Y)
        {
            keysDownLast = new List<Keys>(keysDown);
            buttonsDownLast = buttonsDown;
            MouseLast = new Vector2(X, Y);
        }

        public static Vector2 MouseLastVector(int CurrentX, int CurrentY)
        {
            return new Vector2(CurrentX, CurrentY) - MouseLast;
        }
        public static Vector2 MouseLastRotationVector(int CurrentX, int CurrentY)
        {
            var currentpos = new Vector2(CurrentX, CurrentY);
            currentpos -= MouseLast;
            currentpos.Normalize();
            return new Vector2(currentpos.X, currentpos.Y);
        }
        public static Vector2 MouseDownVector(int CurrentX, int CurrentY)
        {
            return new Vector2(CurrentX, CurrentY) - MouseDown;
        }

        public static bool keyPress(Keys key)
        {
            return keysDown.Contains(key) & !keysDownLast.Contains(key);
        }
        public static bool keyRelease(Keys key)
        {
            return !keysDown.Contains(key) & keysDownLast.Contains(key);
        }
        public static bool keyDown(Keys key)
        {
            return keysDown.Contains(key);
        }

        public static bool buttonPress(MouseButtons button)
        {
            return buttonsDown.Contains(button) & !buttonsDownLast.Contains(button);
        }
        public static bool buttonRelease(MouseButtons button)
        {
            return !buttonsDown.Contains(button) & buttonsDownLast.Contains(button);
        }
        public static bool buttonDown(MouseButtons button)
        {
            return buttonsDown.Contains(button);
        }
    }
}
