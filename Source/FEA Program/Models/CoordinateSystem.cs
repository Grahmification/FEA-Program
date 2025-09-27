using FEA_Program.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FEA_Program.Models
{
    internal class CoordinateSystem
    {
        private double originX = 0d;
        private double originY = 0d;
        private double originZ = 0d;
        private int _scale = 1;
        private Color SysColor = Color.Teal;

        public CoordinateSystem(double Scale)
        {
            _scale = (int)Math.Round(Scale);
        }

        public void Draw(bool ThreeDimensional)
        {
            Matrix4 xform = Matrix4.CreateTranslation(new Vector3((float)(originX / _scale), (float)(originY / _scale), (float)(originZ / _scale)));
            xform = Matrix4.Mult(xform, Matrix4.CreateScale(_scale, _scale, _scale));
            GL.MultMatrix(SpriteBatch.MatrixToArray(xform));

            // X Axis
            GL.Color3(SysColor);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(0.9d, 0.1d, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(0.9d, -0.1d, 0);

            // letter x
            GL.Vertex3(0.9d, 0.25d, 0);
            GL.Vertex3(1, 0.35d, 0);
            GL.Vertex3(1, 0.15d, 0);
            GL.Vertex3(0.8d, 0.35d, 0);
            GL.Vertex3(0.9d, 0.25d, 0);
            GL.Vertex3(0.8d, 0.15d, 0);

            // Y Axis

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(0.1d, 0.9d, 0);
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(-0.1d, 0.9d, 0);

            // letter y
            GL.Vertex3(0.15d, 1, 0);
            GL.Vertex3(0.25d, 0.9d, 0);
            GL.Vertex3(0.25d, 0.9d, 0);
            GL.Vertex3(0.35d, 1, 0);
            GL.Vertex3(0.25d, 0.9d, 0);
            GL.Vertex3(0.25d, 0.8d, 0);

            if (ThreeDimensional)
            {
                // Z Axis
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(0, 0, 1);
                GL.Vertex3(0, 0, 1);
                GL.Vertex3(0.1d, 0, 0.9d);
                GL.Vertex3(0, 0, 1);
                GL.Vertex3(-0.1d, 0, 0.9d);

                // letter Z
                GL.Vertex3(0.15d, 0, 1);
                GL.Vertex3(0.35d, 0, 1);
                GL.Vertex3(0.15d, 0, 1);
                GL.Vertex3(0.35d, 0, 0.8d);
                GL.Vertex3(0.35d, 0, 0.8d);
                GL.Vertex3(0.15d, 0, 0.8d);
            }

            GL.End();

            xform = Matrix4.Identity;
            xform = Matrix4.Mult(xform, Matrix4.CreateScale(1f / _scale, 1f / _scale, 1f / _scale));
            xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(new Vector3((float)(-originX / _scale), (float)(-originY / _scale), (float)(-originZ / _scale))));
            GL.MultMatrix(SpriteBatch.MatrixToArray(xform));
        }
    }
}
