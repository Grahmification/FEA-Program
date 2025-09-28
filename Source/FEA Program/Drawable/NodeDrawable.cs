using FEA_Program.Models;
using OpenTK.Graphics.OpenGL;

namespace FEA_Program.Drawable
{
    // Note: Draw is not yet setup for 6D nodes - need to be able to display rotation displacement

    internal class NodeDrawable(double[] coords, int[] fixity, int id, int dimension) : Node(coords, fixity, id, dimension)
    {
        private Color _DefaultColor = Color.Blue;
        private Color _DefaultForceColor = Color.Purple;
        private Color _ReactionForceColor = Color.Green;
        private Color _DefaultFixityColor = Color.Red;
        private Color _SelectedColor = Color.Yellow;

        public bool Selected { get; set; } = false;
        public void Draw()
        {
            var color = Selected ? _SelectedColor : _DefaultColor;
            var forceColor = Selected ? _SelectedColor : _DefaultForceColor;
            var fixityColor = Selected ? _SelectedColor : _DefaultFixityColor;

            DrawNode(this, color);
            DrawNodeForce(this, forceColor, false);
            DrawNodeFixity(this, fixityColor);
        }
        public void DrawReaction()
        {
            var forceColor = Selected ? _SelectedColor : _ReactionForceColor;
            DrawNodeForce(this, forceColor, true);
        }

        public static void DrawNode(Node node, Color color)
        {
            var nodeCoords = node.Coords_mm; // Always draw as mm input
            double[] coords = [0, 0, 0];

            // nodeCoords may have less than 3 items 
            for(int i = 0; i < Math.Min(nodeCoords.Length, 3); i++)
            {
                coords[i] = nodeCoords[i];
            }

            if (node.Dimension == 1 | node.Dimension == 2)
            {
                GL.Color3(color);
                GL.Begin(PrimitiveType.Quads);

                GL.Vertex3(coords[0] + 1d, coords[1] + 1d, coords[2]);
                GL.Vertex3(coords[0] + 1d, coords[1] - 1d, coords[2]);
                GL.Vertex3(coords[0] - 1d, coords[1] - 1d, coords[2]);
                GL.Vertex3(coords[0] - 1d, coords[1] + 1d, coords[2]);

                GL.End();
            }
            else // dimensions 3 or 6
            {
                Primitives.Cube(color, coords, 2);
            }
        }
        public static void DrawNodeForce(Node node, Color color, bool reaction = false)
        {
            var nodeCoords = node.Coords_mm; // Always draw as mm input
            var nodeForce = reaction ? node.ReactionForce : node.Force;
            double[] coords = [0, 0, 0];
            double[] force = [0, 0, 0];

            // nodeCoords may have less than 3 items 
            for (int i = 0; i < Math.Min(nodeCoords.Length, 3); i++)
            {
                coords[i] = nodeCoords[i];
                force[i] = nodeForce[i];
            }

            double forcelength = 10.0d;

            if (node.Dimension == 1)
            {
                coords[1] = 0;
                coords[2] = 0;
                force[1] = 0;
                force[2] = 0;
            }
            else if (node.Dimension == 1)
            {
                coords[2] = 0;
                force[2] = 0;
            }

            Primitives.Arrow(color, coords, force, forcelength);
        }
        public static void DrawNodeFixity(Node node, Color color)
        {
            double squareoffset = 1.5d;


            var nodeCoords = node.Coords_mm; // Always draw as mm input
            var nodeFixity = node.Fixity;
            double[] coords = [0, 0, 0];
            int[] fixity = [0, 0, 0];

            // nodeCoords may have less than 3 items 
            for (int i = 0; i < Math.Min(nodeCoords.Length, 3); i++)
            {
                coords[i] = nodeCoords[i];
                fixity[i] = nodeFixity[i];
            }

            GL.Color4(color); // set drawing color

            if (fixity[0] == 1) // X Axis
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Vertex3(coords[0], coords[1] + squareoffset, coords[2] + squareoffset);
                GL.Vertex3(coords[0], coords[1] - squareoffset, coords[2] + squareoffset);
                GL.Vertex3(coords[0], coords[1] - squareoffset, coords[2] - squareoffset);
                GL.Vertex3(coords[0], coords[1] + squareoffset, coords[2] - squareoffset);
                GL.End();
            }

            if (node.Dimension > 1) // or else will error when searching for invalid value
            {
                if (fixity[1] == 1) // Y Axis
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex3(coords[0] + squareoffset, coords[1], coords[2] + squareoffset);
                    GL.Vertex3(coords[0] - squareoffset, coords[1], coords[2] + squareoffset);
                    GL.Vertex3(coords[0] - squareoffset, coords[1], coords[2] - squareoffset);
                    GL.Vertex3(coords[0] + squareoffset, coords[1], coords[2] - squareoffset);
                    GL.End();
                }
            }

            if (node.Dimension > 2) // or else will error when searching for invalid value
            {
                if (fixity[2] == 1) // Z Axis
                {
                    GL.Begin(PrimitiveType.Quads);
                    GL.Vertex3(coords[0] + squareoffset, coords[1] + squareoffset, coords[2]);
                    GL.Vertex3(coords[0] - squareoffset, coords[1] + squareoffset, coords[2]);
                    GL.Vertex3(coords[0] - squareoffset, coords[1] - squareoffset, coords[2]);
                    GL.Vertex3(coords[0] + squareoffset, coords[1] - squareoffset, coords[2]);
                    GL.End();
                }
            }
        }
    }
}
