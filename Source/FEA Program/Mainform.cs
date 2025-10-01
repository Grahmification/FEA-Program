using FEA_Program.Controls;
using FEA_Program.Drawable;
using FEA_Program.Graphics;
using FEA_Program.Models;
using FEA_Program.SaveData;
using FEA_Program.UI;
using FEA_Program.UserControls;
using MathNet.Numerics.LinearAlgebra.Double;
using OpenTK.Mathematics;

namespace FEA_Program
{
    internal partial class Mainform : Form
    {
        private StressProblem P;
        private NodeDrawManager _DrawManager = new();

        public CoordinateSystem Coord = new([0, 0, 0], 10);
        public GLControlDraggable GlCont;

        public Mainform()
        {
            Load += Mainform_Load;

            InitializeComponent();

            // Setup draw manager
            _DrawManager.DisplaceScaling = 10000;
            _DrawManager.RedrawRequired += (s, e) => GlCont?.SubControl.Invalidate();
            resultDisplaySettingsControl_main.SetDrawManager(_DrawManager);
            
            ResetProblem(ProblemTypes.Bar_1D);
        }
        private void Mainform_Load(object sender, EventArgs e)
        {
            try
            {
                // Must initialize after class is created, or will get a context error
                GlCont = new(glControl_main, true);
                GlCont.DrawStuff += OnViewDrawStuff;
                GlCont.ViewUpdated += ViewUpdated;

                PopulateProblemTypeBox(ref ToolStripComboBox_ProblemMode, typeof(ProblemTypes));

                var tmp = new CodeInputComboBox(P.CommandList);
                SplitContainer_Main.Panel2.Controls.Add(tmp);
                tmp.Show();
                tmp.Dock = DockStyle.Bottom;

                P.Materials.AddDefaultMaterials();
            }

            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }


        private void ResetProblem(ProblemTypes problemType)
        {
            if (P is null)
            {
                P = new(this, problemType);
            }
            else
            {
                var materials = P.Materials; // Save materials so they don't change
                P = new(this, problemType, materials);
            }

            // Make sure node changes get updated in the draw manager
            _DrawManager.Nodes = P.Nodes.Nodelist;
            P.Nodes.NodeListChanged += OnNodeListChanged;
        }
        private void OnNodeListChanged(object? sender, SortedDictionary<int, NodeDrawable> e)
        {
            _DrawManager.Nodes = [.. e.Values];
        }


        public void OnViewDrawStuff(object? sender, bool threeDimensional)
        {
            Coord.Draw(threeDimensional);

            _DrawManager.DrawNodes();

            foreach (IElementDrawable E in P.Elements.Elemlist)
            {
                var nodeCoords = new List<double[]>();
                foreach (int NodeID in P.Connect.ElementNodes(E.ID))
                    nodeCoords.Add(P.Nodes.GetNode(NodeID).DrawCoordinates);

                E.Draw(nodeCoords);
            }
        }
        public void ViewUpdated(object? sender, GLControlViewUpdatedEventArgs e)
        {

            var rot = e.Rotation;
            var rotationAngles = new Vector3((float)Math.Atan2(rot.M23, rot.M33), (float)Math.Atan2(-1 * rot.M31, Math.Sqrt(rot.M23 * rot.M23 + rot.M33 * rot.M33)), (float)Math.Atan2(rot.M21, rot.M11));
            rotationAngles *= (float)(180f / Math.PI);

            if (e.ThreeDimensional)
            {
                ToolStripStatusLabel_Trans.Text = $"(X: {e.Translation.X:F1}, Y: {e.Translation.Y:F1}, Z: {e.Translation.Z:F1})";
                ToolStripStatusLabel_Rot.Text = $"(RX: {rotationAngles.X:F1}, RY: {rotationAngles.Y:F1}, RZ: {rotationAngles.Z:F1})";
            }
            else
            {
                ToolStripStatusLabel_Trans.Text = $"(X: {e.Translation.X:F1}, Y: {e.Translation.Y:F1})";
                ToolStripStatusLabel_Rot.Text = "";
            }

            ToolStripStatusLabel_Zoom.Text = $"(Zoom: {e.Zoom:F1})";
        }


        public void ReDrawLists()
        {
            TreeView_Main.Nodes.Clear();

            // ---------------------- Add base level ------------------------

            string[] baseLevel = ["Nodes", "Elements", "Forces", "Materials"];

            for (int i = 0; i < baseLevel.Length; i++)
            {
                TreeView_Main.Nodes.Add(new TreeNode(baseLevel[i]));
            }

            // ------------------ Add nodes --------------------

            var baseNode = TreeView_Main.Nodes[0];

            foreach (Node node in P.Nodes.Nodelist)
            {
                var newNode = new TreeNode($"Node {node.ID}")
                {
                    Tag = node.ID,
                    ContextMenuStrip = ContextMenuStrip_TreeView
                };

                newNode.Nodes.Add(new TreeNode($"Pos: {string.Join(",", node.Coords_mm)}"));
                newNode.Nodes.Add(new TreeNode($"Fixity: {string.Join(",", node.Fixity)}"));
                baseNode.Nodes.Add(newNode);
            }

            // ------------------ Add Elements --------------------

            baseNode = TreeView_Main.Nodes[1];

            foreach (IElement element in P.Elements.Elemlist)
            {
                var nodeCoords = P.Connect.ElementNodes(element.ID).Select(NodeID => P.Nodes.GetNode(NodeID).Coords).ToList();

                var newNode = new TreeNode($"Element {element.ID}")
                {
                    Tag = element.ID
                };

                newNode.Nodes.Add(new TreeNode($"Type: {element.Name}"));
                //newNode.Nodes.Add(new TreeNode("Area: " & (Units.Convert(Units.AllUnits.m_squared, element.Ar, Units.AllUnits.mm_squared)) & Units.UnitStrings(Units.AllUnits.mm_squared)(0)))
                newNode.Nodes.Add(new TreeNode($"Material: {element.Material.Name}"));
                newNode.Nodes.Add(new TreeNode($"Length: {element.Length(nodeCoords)}"));
                baseNode.Nodes.Add(newNode);
            }

            // ------------------ Add forces --------------------

            baseNode = TreeView_Main.Nodes[2];

            foreach (Node node in P.Nodes.Nodelist)
            {
                if (node.ForceMagnitude > 0)
                {
                    var newNode = new TreeNode($"Force (Node {node.ID})")
                    {
                        Tag = node.ID,
                        ContextMenuStrip = ContextMenuStrip_TreeView
                    };

                    newNode.Nodes.Add(new TreeNode($"Magnitude [N]: {node.ForceMagnitude}"));
                    newNode.Nodes.Add(new TreeNode($"Components [N]: {string.Join(",", node.Force)}"));
                    baseNode.Nodes.Add(newNode);
                }
            }

            // ------------------ Add Materials --------------------

            baseNode = TreeView_Main.Nodes[3];

            foreach (Material mat in P.Materials.MaterialList)
            {
                var newNode = new TreeNode(mat.Name)
                {
                    Tag = mat.ID
                };

                newNode.Nodes.Add(new TreeNode($"E (GPa): {mat.E_GPa}"));
                newNode.Nodes.Add(new TreeNode($"V : {mat.V}"));
                newNode.Nodes.Add(new TreeNode($"Sy (MPa) : {mat.Sy_MPa}"));
                newNode.Nodes.Add(new TreeNode($"Sut (MPa) {mat.Sut_MPa}: "));
                newNode.Nodes.Add(new TreeNode($"Type : {Enum.GetName(typeof(MaterialType), mat.Subtype)}"));
                baseNode.Nodes.Add(newNode);
            }

            // Expand base level nodes
            foreach (TreeNode node in TreeView_Main.Nodes)
                node.Expand();
        }

        // -------------------- Upper Toolstrip --------------------------
        private void PopulateProblemTypeBox(ref ToolStripComboBox TSCB, Type T)
        {
            TSCB.Items.Clear();

            foreach (string Val in Enum.GetNames(T))
                TSCB.Items.Add(Val);

            TSCB.SelectedIndex = 0;
        }
        private void ToolStripComboBox_ProblemMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ToolStripComboBox TSCB = (ToolStripComboBox)sender;
                var materials = P.Materials; // Save materials - they don't change
                ResetProblem((ProblemTypes)TSCB.SelectedIndex);
                GlCont.ThreeDimensional = P.ThreeDimensional;

                ReDrawLists();
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }

        private void ToolStripButtonSolve_Click(object sender, EventArgs e)
        {
            try
            {
                var nodeDOFS = new Dictionary<int, int>();
                foreach (INode node in P.Nodes.Nodelist)
                    nodeDOFS.Add(node.ID, node.Dimension);

                SparseMatrix K_assembled = P.Connect.Assemble_K_Mtx(P.Elements.Get_K_Matricies(P.Connect.ConnectMatrix, P.Nodes.NodeCoordinates), nodeDOFS);
                DenseVector[] output = Connectivity.Solve(K_assembled, P.Nodes.F_Mtx, P.Nodes.Q_Mtx, true);

                var displacements = output[0].Values;
                var reactionForces = output[1].Values;

                MessageBox.Show(string.Join(",", displacements), "Displacements");
                MessageBox.Show(string.Join(",", reactionForces), "Reaction Forces");

                // TODO: Ensure sorting between nodes and results is correct!!!
                P.Nodes.SetSolution(output[0], output[1]);

                resultsTreeControl_main.DrawSolution(P);
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }

        private void ToolStripButton_Addnode_Click(object sender, EventArgs e)
        {
            try
            {
                var uc = new AddNodeControl(P.AvailableNodeDOFs);
                uc.NodeAddFormSuccess += NodeAddFormSuccess;

                DisplaySideBarMenuControl(uc);
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
        private void NodeAddFormSuccess(AddNodeControl sender, List<double[]> Coords, List<int[]> Fixity, List<int> Dimensions)
        {
            sender.NodeAddFormSuccess -= NodeAddFormSuccess;
            P.Nodes.AddNodes(Coords, Fixity, Dimensions);
        }

        private void ToolStripButton_AddMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                var UC = new AddMaterialControl(typeof(MaterialType));
                UC.MatlAddFormSuccess += MatlAddFormSuccess;

                DisplaySideBarMenuControl(UC);
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
        private void MatlAddFormSuccess(AddMaterialControl sender, string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, int subtype)
        {
            sender.MatlAddFormSuccess -= MatlAddFormSuccess;
            P.Materials.Add(Name, E_GPa, V, Sy_MPa, Sut_MPa, (MaterialType)subtype);
        }

        private void ToolStripButton_AddElement_Click(object sender, EventArgs e)
        {
            try
            {
                var ElementArgsList = new List<Dictionary<string, Units.DataUnitType>>();

                foreach (Type ElemType in P.AvailableElements)
                    ElementArgsList.Add(ElementManager.ElementArgs(ElemType));

                var UC = new AddElementControl(P.AvailableElements, ElementArgsList, P.Materials.MaterialList, P.Nodes.BaseNodelist);
                UC.ElementAddFormSuccess += ElemAddFormSuccess;
                UC.NodeSelectionUpdated += FeatureAddFormNodeSelectionChanged;
                DisplaySideBarMenuControl(UC);
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
        private void ElemAddFormSuccess(AddElementControl sender, Type Type, List<int> NodeIDs, double[] ElementArgs, int Mat)
        {

            sender.ElementAddFormSuccess -= ElemAddFormSuccess;
            sender.NodeSelectionUpdated -= FeatureAddFormNodeSelectionChanged;

            var material = P.Materials.GetMaterial(Mat);
            int nodeDOFs = P.AvailableNodeDOFs;
            P.Elements.Add(nodeDOFs, Type, NodeIDs, ElementArgs, material);

        }

        private void ToolStripButton_AddNodeForce_Click(object sender, EventArgs e)
        {
            try
            {
                var UC = new AddNodeForceControl(P.AvailableNodeDOFs, P.Nodes.BaseNodelist);
                UC.NodeForceAddFormSuccess += NodeForceAddFormSuccess;
                UC.NodeSelectionUpdated += FeatureAddFormNodeSelectionChanged;
                DisplaySideBarMenuControl(UC);
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
        private void NodeForceAddFormSuccess(AddNodeForceControl sender, List<double[]> Forces, List<int> NodeIDs)
        {

            sender.NodeForceAddFormSuccess -= NodeForceAddFormSuccess;
            sender.NodeSelectionUpdated -= FeatureAddFormNodeSelectionChanged;

            P.Nodes.SetNodeForces(Forces, NodeIDs);
        }
        private void FeatureAddFormNodeSelectionChanged(object sender, List<int> selectedNodeIDs)
        {
            P.Nodes.SelectNodes(false); // Deselect all
            if (selectedNodeIDs.Count > 0)
            {
                P.Nodes.SelectNodes(true, selectedNodeIDs.ToArray());
            }
        }

        /// <summary>
        /// Displays a user control on the sidebar overlaying everything else
        /// </summary>
        /// <param name="uc"></param>
        private void DisplaySideBarMenuControl(UserControl uc)
        {

            SplitContainer_Main.Panel1.Controls.Add(uc);
            SplitContainer_Main.Panel1MinSize = uc.Width;
            uc.BringToFront();
            uc.Dock = DockStyle.Fill;
        }

        // ------------------------------ Load/Save ---------------------------------
        private async void toolStripMenuItem_Save_Click(object sender, EventArgs e)
        {
            try
            {
                var filePath = IODialogs.DisplaySaveFileDialog([IOFileTypes.JSON], "FEA Problem 1");

                if (filePath != null && filePath != "")
                {
                    var saveData = P.GetSaveData();

                    await JsonSerializer.SerializeToJsonFile(saveData, filePath);
                }
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }

        private async void toolStripMenuItem_Load_Click(object sender, EventArgs e)
        {
            try
            {
                var filePath = IODialogs.DisplayOpenFileDialog([IOFileTypes.JSON]);

                if (filePath != null && filePath != "")
                {
                    var saveData = await JsonSerializer.DeserializeFromJsonFile<ProblemData>(filePath);

                    if (saveData != null)
                    {
                        // This this before loading data so changing it doesn't reset the problem
                        ToolStripComboBox_ProblemMode.SelectedIndex = (int)saveData.ProblemType;

                        P.LoadData(saveData);
                    }
                }
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }

        // ------------------------------ Misc Events ---------------------------------

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

                    P.Nodes.SelectNodes(false); // De-Select all
                    P.Elements.SelectElements(false); // De-Select all

                    P.Nodes.SelectNodes(true, [.. NodeIDs]);
                    P.Elements.SelectElements(true, [.. ElemIDs]);
                }

                // End If
                else if (InputManager.IsButtonDown(MouseButtons.Right))
                {
                }
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
    }
}
