using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace FEA_Program.Drawable
{
    internal class CoordinateSystem
    {
        public double OriginX { get; set; } = 0d;
        public double OriginY { get; set; } = 0d;
        public double OriginZ { get; set; } = 0d;
        private double Scale { get; set; } = 1;
        public Color Color { get; set; } = Color.Teal;

        public CoordinateSystem(double[] position, double scale)
        {
            OriginX = position[0];
            OriginY = position[1];
            OriginZ = position[2];
            Scale = scale;
        }

        public void Draw(bool threeDimensional)
        {
            Matrix4 xform = Matrix4.CreateTranslation(Vector3.One * (float)(OriginX / Scale));
            xform = Matrix4.Mult(xform, Matrix4.CreateScale((float)Scale));
            GL.MultMatrix(ref xform);

            // X Axis
            GL.Color3(Color);
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

            if (threeDimensional)
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
            xform = Matrix4.Mult(xform, Matrix4.CreateScale((float)(1f / Scale)));
            xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(Vector3.One * (float)(-OriginX / Scale)));
            GL.MultMatrix(ref xform);
        }
    }
}
