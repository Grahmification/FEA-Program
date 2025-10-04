using FEA_Program.Controllers;
using FEA_Program.Drawable;
using FEA_Program.Graphics;
using FEA_Program.Models;
using FEA_Program.UI;

namespace FEA_Program.UserControls
{
    internal partial class ProblemTreeControl : UserControl
    {
        private List<TreeNode> _nodeItems = [];
        private List<TreeNode> _elementItems = [];
        private List<TreeNode> _forceItems = [];
        private List<TreeNode> _materialItems = [];


        public event EventHandler<int>? NodeDeleteRequest;
        public event EventHandler<int>? ElementDeleteRequest;


        public ProblemTreeControl()
        {
            InitializeComponent();
        }

        public void DrawTree(ProblemManager P)
        {
            var tree = treeView_main;
            tree.Nodes.Clear();
            _forceItems.Clear();
            _elementItems.Clear();
            _materialItems.Clear();
            _nodeItems.Clear();

            // ---------------------- Add base level ------------------------

            string[] baseLevel = ["Nodes", "Elements", "Forces", "Materials"];

            for (int i = 0; i < baseLevel.Length; i++)
            {
                tree.Nodes.Add(new TreeNode(baseLevel[i]));
            }

            // ------------------ Add nodes --------------------

            var baseNode = tree.Nodes[0];

            foreach (NodeDrawable node in P.Nodes.Nodelist)
            {
                var newNode = new TreeNode($"Node {node.Node.ID}")
                {
                    Tag = node.Node.ID,
                };

                newNode.Nodes.Add(new TreeNode($"Pos: {string.Join(",", node.Coordinates_mm)}"));
                newNode.Nodes.Add(new TreeNode($"Fixity: {string.Join(",", node.Node.Fixity)}"));
                baseNode.Nodes.Add(newNode);
                _nodeItems.Add(newNode);
            }

            // ------------------ Add Elements --------------------

            baseNode = tree.Nodes[1];

            foreach (IElementDrawable element in P.Elements.Elemlist)
            {
                var newNode = new TreeNode($"Element {element.ID}")
                {
                    Tag = element.ID
                };

                newNode.Nodes.Add(new TreeNode($"Type: {element.Name}"));
                //newNode.Nodes.Add(new TreeNode("Area: " & (Units.Convert(Units.AllUnits.m_squared, element.Ar, Units.AllUnits.mm_squared)) & Units.UnitStrings(Units.AllUnits.mm_squared)(0)))
                newNode.Nodes.Add(new TreeNode($"Material: {element.Material.Name}"));
                newNode.Nodes.Add(new TreeNode($"Length: {element.Length()}"));
                baseNode.Nodes.Add(newNode);
                _elementItems.Add(newNode);
            }

            // ------------------ Add forces --------------------

            baseNode = tree.Nodes[2];

            foreach (Node node in P.Nodes.Nodelist.Select(n => n.Node))
            {
                if (node.ForceMagnitude > 0)
                {
                    var newNode = new TreeNode($"Force (Node {node.ID})")
                    {
                        Tag = node.ID,
                    };

                    newNode.Nodes.Add(new TreeNode($"Magnitude [N]: {node.ForceMagnitude}"));
                    newNode.Nodes.Add(new TreeNode($"Components [N]: {string.Join(",", node.Force)}"));
                    baseNode.Nodes.Add(newNode);
                    _forceItems.Add(newNode);
                }
            }

            // ------------------ Add Materials --------------------

            baseNode = tree.Nodes[3];

            foreach (Material mat in P.Materials.MaterialList)
            {
                var newNode = new TreeNode(mat.Name)
                {
                    Tag = mat.ID,
                };

                newNode.Nodes.Add(new TreeNode($"E (GPa): {mat.E_GPa}"));
                newNode.Nodes.Add(new TreeNode($"V : {mat.V}"));
                newNode.Nodes.Add(new TreeNode($"Sy (MPa) : {mat.Sy_MPa}"));
                newNode.Nodes.Add(new TreeNode($"Sut (MPa) {mat.Sut_MPa}: "));
                newNode.Nodes.Add(new TreeNode($"Type : {Enum.GetName(typeof(MaterialType), mat.Subtype)}"));
                baseNode.Nodes.Add(newNode);
                _materialItems.Add(newNode);
            }

            // Expand base level nodes
            foreach (TreeNode node in tree.Nodes)
                node.Expand();
        }

        private void TreeView_Main_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                TreeView Tree = (TreeView)sender;

                if (InputManager.ButtonPressOccurred(MouseButtons.Left))
                {
                    // If e.Node.Level = 1 Then
                    // Dim FirstLevel As String = e.Node.FullPath.Split("\").First()
                    var NodeIDs = new List<int>();
                    var ElemIDs = new List<int>();

                    foreach (TreeNode N in Tree.Nodes[0].Nodes)
                    {
                        if (N.IsSelected)
                        {
                            NodeIDs.Add((int)N.Tag);
                        }
                    }

                    foreach (TreeNode N in Tree.Nodes[1].Nodes)
                    {
                        if (N.IsSelected)
                        {
                            ElemIDs.Add((int)N.Tag);
                        }
                    }

                    //P.Nodes.SelectNodes(false); // De-Select all
                    //P.Elements.SelectElements(false); // De-Select all

                    //P.Nodes.SelectNodes(true, [.. NodeIDs]);
                    //P.Elements.SelectElements(true, [.. ElemIDs]);
                }

                // End If
                else if (e.Button == MouseButtons.Right)
                {
                    HandleRightClick(Tree, e);
                }
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }

        private void HandleRightClick(TreeView tree, TreeNodeMouseClickEventArgs e)
        {
            tree.SelectedNode = e.Node;

            // A node was selected
            if (_nodeItems.Contains(e.Node) || _elementItems.Contains(e.Node))
            {
                // Store the node in the ContextMenuStrip's Tag property
                contextMenuStrip_Edit.Tag = e.Node;
                contextMenuStrip_Edit.Show(tree, e.Location);
            }
        }

        private void toolStripMenuItem_Delete_Click(object sender, EventArgs e)
        {
            // The ToolStripItem is the sender, its Parent is the ContextMenuStrip
            ContextMenuStrip? menu = ((ToolStripMenuItem)sender).Owner as ContextMenuStrip;

            if (menu != null && menu.Tag is TreeNode clickedNode)
            {
                // A node was selected
                if (_nodeItems.Contains(clickedNode))
                {
                    NodeDeleteRequest?.Invoke(this, (int)clickedNode.Tag);
                }
                // An element was selected
                else if (_elementItems.Contains(clickedNode))
                {
                    ElementDeleteRequest?.Invoke(this, (int)clickedNode.Tag);
                }
            }
        }
    }
}
