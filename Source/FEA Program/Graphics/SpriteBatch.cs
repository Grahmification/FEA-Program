using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace FEA_Program.Graphics
{
    internal class SpriteBatch
    {
        public static void DrawArrow(double length, Vector3 direction, Vector3 position, bool threeDimensional, Color color, int linethickness = 1)
        {

            LoadOrientation(position, VectorToRotationMtx(direction), (float)length);

            GL.LineWidth(linethickness);

            GL.Color3(color);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 1);
            GL.End();

            GL.Begin(PrimitiveType.Triangles);
            if (threeDimensional)
            {

                GL.Vertex3(0, 0, 1);
                GL.Vertex3(0.1d, 0.1d, 0.8d);
                GL.Vertex3(0.1d, -0.1d, 0.8d);

                GL.Vertex3(0, 0, 1);
                GL.Vertex3(-0.1d, 0.1d, 0.8d);
                GL.Vertex3(-0.1d, -0.1d, 0.8d);

                GL.Vertex3(0, 0, 1);
                GL.Vertex3(0.1d, -0.1d, 0.8d);
                GL.Vertex3(-0.1d, -0.1d, 0.8d);

                GL.Vertex3(0, 0, 1);
                GL.Vertex3(0.1d, 0.1d, 0.8d);
                GL.Vertex3(-0.1d, 0.1d, 0.8d);
            }

            else
            {
                GL.Vertex3(0, 0, 1);
                GL.Vertex3(0.1d, 0, 0.8d);
                GL.Vertex3(-0.1d, 0, 0.8d);
            }
            GL.End();

            GL.LineWidth(1);

            RevertOrientation(position, VectorToRotationMtx(direction), (float)length);
        }
        public static float[] MatrixToArray(Matrix4 xform)
        {
            // 1. Create a float array of 16 elements.
            float[] matrixArray = new float[16];

            // 2. Manually copy the data from the Matrix4 into the array.
            //    OpenTK Matrix4 is column-major, meaning you fill the array 
            //    by going down the columns first (M11, M21, M31, M41, then M12, M22, etc.).

            // Column 1 (M11, M21, M31, M41)
            matrixArray[0] = xform.M11;
            matrixArray[1] = xform.M21;
            matrixArray[2] = xform.M31;
            matrixArray[3] = xform.M41;

            // Column 2 (M12, M22, M32, M42)
            matrixArray[4] = xform.M12;
            matrixArray[5] = xform.M22;
            matrixArray[6] = xform.M32;
            matrixArray[7] = xform.M42;

            // Column 3 (M13, M23, M33, M43)
            matrixArray[8] = xform.M13;
            matrixArray[9] = xform.M23;
            matrixArray[10] = xform.M33;
            matrixArray[11] = xform.M43;

            // Column 4 (M14, M24, M34, M44)
            matrixArray[12] = xform.M14;
            matrixArray[13] = xform.M24;
            matrixArray[14] = xform.M34;
            matrixArray[15] = xform.M44;

            return matrixArray;
        }

        private static void LoadOrientation(Vector3 Trans, Matrix4 Rot, float zoom)
        {
            Matrix4 xform = Matrix4.Identity;
            xform = Matrix4.Mult(xform, Rot);
            xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(Trans / zoom));
            xform = Matrix4.Mult(xform, Matrix4.CreateScale((float)zoom));
            GL.MultMatrix(MatrixToArray(xform));
        }
        private static void RevertOrientation(Vector3 Trans, Matrix4 Rot, float zoom)
        {
            Matrix4 xform = Matrix4.Identity;
            xform = Matrix4.Mult(xform, Matrix4.CreateScale((float)(1.0 / zoom)));
            xform = Matrix4.Mult(xform, Matrix4.CreateTranslation(-1 * Trans / zoom));
            xform = Matrix4.Mult(xform, Rot.Inverted());
            GL.MultMatrix(MatrixToArray(xform));
        }


        private static Matrix4 VectorToRotationMtx(Vector3 V)
        {
            double angle = Math.Acos(Vector3.Dot(V, Vector3.UnitZ) / V.Length);

            return Matrix4.CreateFromAxisAngle(new Vector3(V.Y, -V.X, 0), (float)-angle);
        }
    }
}
