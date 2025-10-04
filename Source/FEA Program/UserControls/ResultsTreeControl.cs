using FEA_Program.Controllers;
using FEA_Program.Models;

namespace FEA_Program.UserControls
{
    internal partial class ResultsTreeControl : UserControl
    {
        public ResultsTreeControl()
        {
            InitializeComponent();
        }

        public void DrawSolution(ProblemManager P)
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

                var stress = element.StressMatrix();
                var endDispl = element.Interpolated_Displacement([1]);

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
