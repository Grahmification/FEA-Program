using FEA_Program.Models;
using OpenTK.Graphics.OpenGL;

namespace FEA_Program.Drawable
{
    internal class ElementBarLinearDrawable : ElementBarLinear, IElementDrawable
    {
        private readonly Color _DefaultColor = Color.Green;
        private readonly Color _SelectedColor = Constants.SelectedColor;

        private readonly NodeDrawable[] _nodes;

        public string Name => "Bar Linear";

        public bool Selected { get; set; } = false;

        /// <summary>
        /// Allows setting a color for each corner of the element
        /// </summary>
        public Color[] Colors { get; private set; } = [];

        public ElementBarLinearDrawable(double area, int ID, List<NodeDrawable> nodes, Material material, int nodeDOFs = 1) : base(area, ID, nodes.Select(n => (INode)n.Node).ToList(), material, nodeDOFs)
        {
            // Check that sorting of the nodes is correct
            _nodes = [.. nodes];
            var baseNodes = nodes.Select(n => (INode)n.Node).ToList();
            SortNodeOrder(ref baseNodes);

            // Reverse the nodes
            if (baseNodes[0].ID != _nodes[0].Node.ID)
            {
                _nodes[0] = nodes[1];
                _nodes[1] = nodes[0];
            }

            Colors = new Color[NumOfNodes]; // Need to have a color for each node in the element
            SetColor(_DefaultColor); // Initially set all corners to the default color
        }

        public void Draw()
        {
            Color[] drawColors = Colors;

            if (Selected)
            {
                for(int i = 0; i < drawColors.Length; i++)
                {
                    drawColors[i] = _SelectedColor;
                }
            }

            GL.LineWidth(2);

            GL.Begin(PrimitiveType.Lines);

            if (NodeDOFs == 1)
            {
                for (int i = 0, loopTo = NumOfNodes - 1; i <= loopTo; i++)
                {
                    GL.Color4(drawColors[i]);
                    GL.Vertex3(_nodes[i].DrawCoordinates[0], 0, 0);
                }
            }
            else if (NodeDOFs == 2)
            {
                for (int i = 0, loopTo2 = NumOfNodes - 1; i <= loopTo2; i++)
                {
                    GL.Color4(drawColors[i]);
                    GL.Vertex3(_nodes[i].DrawCoordinates[0], _nodes[i].DrawCoordinates[1], 0);
                }
            }
            else
            {
                for (int i = 0, loopTo1 = NumOfNodes - 1; i <= loopTo1; i++)
                {
                    GL.Color4(drawColors[i]);
                    GL.Vertex3(_nodes[i].DrawCoordinates[0], _nodes[i].DrawCoordinates[1], _nodes[i].DrawCoordinates[2]);
                }
            }

            GL.End();
            GL.LineWidth(1);
        }

        /// <summary>
        /// Sets all endpoints to the specified color
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            for (int i = 0; i < Colors.Length; i++)
            {
                Colors[i] = color;
            }
        }
        public void SetCornerColor(Color color, int nodeIndex)
        {
            Colors[nodeIndex] = color;
        }
    }
}
