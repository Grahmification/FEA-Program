using FEA_Program.Models;
using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.UserControls
{
    internal partial class ResultsTreeControl : UserControl
    {
        public ResultsTreeControl()
        {
            InitializeComponent();
        }

        public void DrawSolution(StressProblem P)
        {
            var tree = treeView_main;
            tree.Nodes.Clear();

            // ---------------------- Add base level ------------------------

            string[] baseLevel = ["Nodes", "Elements"];

            for (int i = 0; i < baseLevel.Length; i++)
            {
                tree.Nodes.Add(new TreeNode(baseLevel[i]));
            }

            // ------------------ Add nodes --------------------

            var baseNode = tree.Nodes[0];

            foreach (Node node in P.Nodes.Nodelist)
            {
                var newNode = new TreeNode($"Node {node.ID}")
                {
                    Tag = node.ID,
                };

                newNode.Nodes.Add(new TreeNode($"Displacement: {string.Join(",", node.Displacement)}"));
                newNode.Nodes.Add(new TreeNode($"Position: {string.Join(",", node.FinalPos)}"));
                newNode.Nodes.Add(new TreeNode($"Reaction Force [N]: {string.Join(",", node.ReactionForce)}"));
                baseNode.Nodes.Add(newNode);
            }

            // ------------------ Add elements --------------------

            baseNode = tree.Nodes[1];

            foreach (IElement element in P.Elements.Elemlist)
            {
                var newNode = new TreeNode($"Element {element.ID}")
                {
                    Tag = element.ID,
                };

                // Get node coordinates and displacements associated with the element
                var nodeIDs = P.Connect.GetElementNodes(element.ID);
                var nodes = nodeIDs.Select(id => (Node)P.Nodes.GetNode(id)).ToList();
                var nodesCoords = nodes.Select(n => n.Coordinates).ToList();
                DenseVector nodeDisplacement = Node.BuildVector(nodes, n => n.Displacement);

                var stress = element.StressMatrix(nodesCoords, nodeDisplacement);
                var endDispl = element.Interpolated_Displacement([1], nodeDisplacement);

                newNode.Nodes.Add(new TreeNode($"Stress: {string.Join(",", stress.Values)}"));
                newNode.Nodes.Add(new TreeNode($"Interpolated end displ:  {string.Join(",", endDispl.Values)}"));
                baseNode.Nodes.Add(newNode);
            }

            // Expand base level nodes
            foreach (TreeNode node in tree.Nodes)
                node.Expand();
        }
    }
}
