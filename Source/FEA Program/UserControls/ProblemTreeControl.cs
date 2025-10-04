using FEA_Program.Drawable;
using FEA_Program.Graphics;
using FEA_Program.Models;
using FEA_Program.UI;

namespace FEA_Program.UserControls
{
    internal partial class ProblemTreeControl : UserControl, INodeDisplayView
    {
        private readonly TreeNode _nodeRootItem;
        private readonly TreeNode _elementRootItem;
        private readonly TreeNode _forceRootItem;
        private readonly TreeNode _materialRootItem;

        public event EventHandler? NodeAddRequest;
        public event EventHandler<int>? NodeEditRequest;
        public event EventHandler<int>? NodeDeleteRequest;
        public event EventHandler<int>? ElementDeleteRequest;


        public ProblemTreeControl()
        {
            InitializeComponent();

            var tree = treeView_main;
            tree.Nodes.Clear();

            _nodeRootItem = new TreeNode("Nodes");
            _elementRootItem = new TreeNode("Elements");
            _forceRootItem = new TreeNode("Forces");
            _materialRootItem = new TreeNode("Material");

            tree.Nodes.Add(_nodeRootItem);
            tree.Nodes.Add(_elementRootItem);
            tree.Nodes.Add(_forceRootItem);
            tree.Nodes.Add(_materialRootItem);
        }
        public void DisplayNodes(List<NodeDrawable> nodes)
        {
            // ------------------ Add nodes --------------------
            var baseNode = _nodeRootItem;
            baseNode.Nodes.Clear();

            foreach (NodeDrawable node in nodes)
            {
                var newNode = new TreeNode($"Node {node.Node.ID}")
                {
                    Tag = node.Node.ID,
                };

                newNode.Nodes.Add(new TreeNode($"Pos: {string.Join(",", node.Coordinates_mm)}"));
                newNode.Nodes.Add(new TreeNode($"Fixity: {string.Join(",", node.Node.Fixity)}"));
                baseNode.Nodes.Add(newNode);
            }

            // ------------------ Add forces --------------------
            baseNode = _forceRootItem;
            baseNode.Nodes.Clear();

            foreach (Node node in nodes.Select(n => n.Node))
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
                }
            }

            baseNode.Expand();
        }
        public void DisplayElements(List<IElementDrawable> elements)
        {
            // ------------------ Add Elements --------------------
            var baseNode = _elementRootItem;
            baseNode.Nodes.Clear();

            foreach (IElementDrawable element in elements)
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
            }

            baseNode.Expand();
        }
        public void DisplayMaterials(List<Material> materials)
        {
            // ------------------ Add Materials --------------------
            var baseNode = _materialRootItem;
            baseNode.Nodes.Clear();

            foreach (Material mat in materials)
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
            }

            baseNode.Expand();
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
            if (_nodeRootItem.Nodes.Contains(e.Node) || _elementRootItem.Nodes.Contains(e.Node))
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
                if (_nodeRootItem.Nodes.Contains(clickedNode))
                {
                    NodeDeleteRequest?.Invoke(this, (int)clickedNode.Tag);
                }
                // An element was selected
                else if (_elementRootItem.Nodes.Contains(clickedNode))
                {
                    ElementDeleteRequest?.Invoke(this, (int)clickedNode.Tag);
                }
            }
        }
    }
}
