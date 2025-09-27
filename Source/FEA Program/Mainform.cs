using FEA_Program.Controls;
using FEA_Program.Graphics;
using FEA_Program.Models;
using FEA_Program.UserControls;
using MathNet.Numerics.LinearAlgebra.Double;
using OpenTK.Mathematics;

namespace FEA_Program
{
    internal partial class Mainform : Form
    {
        private StressProblem P = default;

        public CoordinateSystem Coord = new CoordinateSystem(50);
        public CoordinateSystem Coord2 = new CoordinateSystem(25);
        public GLControlDraggable GlCont;

        public Mainform()
        {
            Load += Mainform_Load;
            
            InitializeComponent();

            GlCont = new(glControl_main, Color.Black, true);
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            try
            {
                PopulateProblemTypeBox(ref ToolStripComboBox_ProblemMode, typeof(ProblemTypes));

                var tmp = new CodeInputComboBox(P.CommandList);
                SplitContainer_Main.Panel2.Controls.Add(tmp);
                tmp.Show();
                tmp.Dock = DockStyle.Bottom;

                this.GlCont.DrawStuffAfterOrientation += DrawStuff;
                this.GlCont.DrawStuffBeforeOrientation += DrawStuffBefore;
                this.GlCont.ViewUpdated += ViewUpdated;
            }

            catch (Exception ex)
            {

            }
        }

        public void DrawStuff(bool ThreeDimensional)
        {

            Coord.Draw(ThreeDimensional);

            foreach (Node N in P.Nodes.Nodelist)
                N.DrawNode(N.Coords_mm);

            foreach (IElement E in P.Elements.Elemlist)
            {
                var nodeCoords = new List<double[]>();
                foreach (int NodeID in P.Connect.ElementNodes(E.ID))
                    nodeCoords.Add(P.Nodes.NodeObj(NodeID).Coords_mm);

                E.Draw(nodeCoords);
            }

            // SpriteBatch.DrawArrow(10, New Vector3(1, 1, 1), New Vector3(1, 1, 1), True, Color.Green, 3)
            // GL.LoadMatrix(GlCont.Orientation)
            // Coord2.Draw(ThreeDimensional)
        }

        public void DrawStuffBefore(bool ThreeDimensional)
        {

        }

        public void ViewUpdated(Vector3 Trans, Matrix4 rot, Vector3 zoom, bool ThreeDimensional)
        {

            var rotationAngles = new Vector3((float)Math.Atan2(rot.M23, rot.M33), (float)Math.Atan2(-1 * rot.M31, Math.Sqrt(rot.M23 * rot.M23 + rot.M33 * rot.M33)), (float)Math.Atan2(rot.M21, rot.M11));
            rotationAngles *= (float)(180f / Math.PI);

            if (ThreeDimensional)
            {
                ToolStripStatusLabel_Trans.Text = "(X: " + Math.Round(Trans.X, 1) + ", Y:" + Math.Round(Trans.Y, 1) + ", Z:" + Math.Round(Trans.Z, 1) + ")";
                ToolStripStatusLabel_Rot.Text = "(RX: " + Math.Round(rotationAngles.X, 1) + ", RY:" + Math.Round(rotationAngles.Y, 1) + ", RZ:" + Math.Round(rotationAngles.Z, 1) + ")";
                ToolStripStatusLabel_Zoom.Text = "(Zoom: " + Math.Round(zoom.X, 1) + ")";
            }
            else
            {
                ToolStripStatusLabel_Trans.Text = "(X: " + Math.Round(Trans.X, 1) + ", Y:" + Math.Round(Trans.Y, 1) + ")";
                ToolStripStatusLabel_Rot.Text = "";
                ToolStripStatusLabel_Zoom.Text = "(Zoom: " + Math.Round(zoom.X, 1) + ")";
            }
        }


        public void ReDrawLists()
        {
            TreeView_Main.Nodes.Clear();

            // ---------------------- Add base level ------------------------

            string[] Baselevel = new[] { "Nodes", "Elements", "Forces", "Materials" };


            for (int i = 0, loopTo = Baselevel.Length - 1; i <= loopTo; i++)
            {
                var N = new TreeNode(Baselevel[i]);
                TreeView_Main.Nodes.Add(N);
            }

            // ------------------ Add nodes --------------------

            for (int i = 0, loopTo1 = P.Nodes.Nodelist.Count - 1; i <= loopTo1; i++)
            {
                Node tmpNode = P.Nodes.Nodelist[i];
                var N = new TreeNode("Node " + tmpNode.ID);
                N.Tag = tmpNode.ID;
                N.ContextMenuStrip = ContextMenuStrip_TreeView;

                TreeView_Main.Nodes[0].Nodes.Add(N);
                TreeView_Main.Nodes[0].Nodes[i].Nodes.Add(new TreeNode("Pos: " + string.Join(",", tmpNode.Coords_mm.ToArray())));
                TreeView_Main.Nodes[0].Nodes[i].Nodes.Add(new TreeNode("Fixity: " + string.Join(",", tmpNode.Fixity.ToArray())));
            }

            // ------------------ Add Materials --------------------

            for (int i = 0, loopTo2 = P.Materials.AllIDs.Count - 1; i <= loopTo2; i++)
            {
                MaterialClass mat = P.Materials.MatObj(P.Materials.AllIDs[i]);
                var N = new TreeNode(mat.Name);
                N.Tag = mat.ID;

                TreeView_Main.Nodes[3].Nodes.Add(N);
                TreeView_Main.Nodes[3].Nodes[i].Nodes.Add(new TreeNode("E (GPa): " + (string)mat.E_GPa.ToString()));
                TreeView_Main.Nodes[3].Nodes[i].Nodes.Add(new TreeNode("V : " + (string)mat.V.ToString()));
                TreeView_Main.Nodes[3].Nodes[i].Nodes.Add(new TreeNode("Sy (MPa) : " + (string)mat.Sy_MPa.ToString()));
                TreeView_Main.Nodes[3].Nodes[i].Nodes.Add(new TreeNode("Sut (MPa) : " + (string)mat.Sut_MPa.ToString()));
                TreeView_Main.Nodes[3].Nodes[i].Nodes.Add(new TreeNode("Type : " + Enum.GetName(typeof(MaterialType), mat.Subtype)));
            }

            // ------------------ Add Elements --------------------

            for (int i = 0, loopTo3 = P.Elements.Elemlist.Count - 1; i <= loopTo3; i++)
            {
                IElement tmpElem = P.Elements.Elemlist[i];
                var N = new TreeNode("Element " + tmpElem.ID);
                N.Tag = tmpElem.ID;

                TreeView_Main.Nodes[1].Nodes.Add(N);
                TreeView_Main.Nodes[1].Nodes[i].Nodes.Add(new TreeNode("Type: " + ElementMgr.Name(tmpElem.MyType)));
                // TreeView_Main.Nodes(1).Nodes(i).Nodes.Add(New TreeNode("Area: " & CStr(Units.Convert(Units.AllUnits.m_squared, tmpElem.a, Units.AllUnits.mm_squared)) & Units.UnitStrings(Units.AllUnits.mm_squared)(0)))
                TreeView_Main.Nodes[1].Nodes[i].Nodes.Add(new TreeNode("Material: " + P.Materials.MatObj(tmpElem.Material).Name));

                var nodeCoords = new List<double[]>();
                foreach (int NodeID in P.Connect.ElementNodes(tmpElem.ID))
                    nodeCoords.Add(P.Nodes.NodeObj(NodeID).Coords);

                TreeView_Main.Nodes[1].Nodes[i].Nodes.Add(new TreeNode("Length: " + tmpElem.Length(nodeCoords)));
            }

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
                P = new StressProblem(this, (ProblemTypes)TSCB.SelectedIndex);
                GlCont.ThreeDimensional = P.ThreeDimensional;

                ReDrawLists();
            }
            catch (Exception ex)
            {

            }
        }

        private void ToolStripButton_Addnode_Click(object sender, EventArgs e)
        {
            try
            {
                var uc = new AddNodeControl(P.AvailableNodeDOFs);
                uc.NodeAddFormSuccess += NodeAddFormSuccess;

                AddUserControlToSplitCont(uc, SplitContainer_Main, 1);
            }
            catch (Exception ex)
            {

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

                AddUserControlToSplitCont(UC, SplitContainer_Main, 1);
            }
            catch (Exception ex)
            {

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
                    ElementArgsList.Add(ElementMgr.ElementArgs(ElemType));


                var UC = new AddElementControl(P.AvailableElements, ElementArgsList, P.Materials.MatList, P.Nodes.Nodelist);
                UC.ElementAddFormSuccess += ElemAddFormSuccess;
                UC.NodeSelectionUpdated += FeatureAddFormNodeSelectionChanged;
                AddUserControlToSplitCont(UC, SplitContainer_Main, 1);
            }
            catch (Exception ex)
            {

            }
        }
        private void ElemAddFormSuccess(AddElementControl sender, Type Type, List<int> NodeIDs, double[] ElementArgs, int Mat)
        {

            sender.ElementAddFormSuccess -= ElemAddFormSuccess;
            sender.NodeSelectionUpdated -= FeatureAddFormNodeSelectionChanged;

            P.Elements.Add(Type, NodeIDs, ElementArgs, Mat);

        }

        private void ToolStripButton_AddNodeForce_Click(object sender, EventArgs e)
        {
            try
            {

                var UC = new AddNodeForceControl(P.AvailableNodeDOFs, P.Nodes.Nodelist);
                UC.NodeForceAddFormSuccess += NodeForceAddFormSuccess;
                UC.NodeSelectionUpdated += FeatureAddFormNodeSelectionChanged;
                AddUserControlToSplitCont(UC, SplitContainer_Main, 1);
            }
            catch (Exception ex)
            {

            }
        }
        private void NodeForceAddFormSuccess(AddNodeForceControl sender, List<double[]> Forces, List<int> NodeIDs)
        {

            sender.NodeForceAddFormSuccess -= NodeForceAddFormSuccess;
            sender.NodeSelectionUpdated -= FeatureAddFormNodeSelectionChanged;

            P.Nodes.Addforce(Forces, NodeIDs);
        }


        private void FeatureAddFormNodeSelectionChanged(object sender, List<int> SelectedNodeIDs)
        {
            P.Nodes.SelectNodes(P.Nodes.AllIDs.ToArray(), false);
            if (SelectedNodeIDs.Count > 0)
            {
                P.Nodes.SelectNodes(SelectedNodeIDs.ToArray(), true);
            }
        }

        private void AddUserControlToSplitCont(UserControl UC, SplitContainer SCont, int SPanel)
        {

            if (SPanel == 1)
            {
                SCont.Panel1.Controls.Add(UC);
                SCont.Panel1MinSize = UC.Width;
            }
            else // panel 2
            {
                SCont.Panel2.Controls.Add(UC);
                SCont.Panel2MinSize = UC.Width;
            }

            UC.BringToFront();
            UC.Dock = DockStyle.Fill;
        }

        private void TreeView_Main_NodeMouseClick(object sender, TreeViewEventArgs e)
        {
            TreeView Tree = (TreeView)sender;

            if (Input.buttonPress(System.Windows.Forms.MouseButtons.Left))
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

                P.Nodes.SelectNodes(P.Nodes.AllIDs.ToArray(), false);
                P.Elements.SelectElems(P.Elements.AllIDs.ToArray(), true);


                P.Nodes.SelectNodes(NodeIDs.ToArray(), true);
                P.Elements.SelectElems(ElemIDs.ToArray(), true);
            }

            // End If
            else if (Input.buttonDown(System.Windows.Forms.MouseButtons.Right))
            {
            }

        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            TreeNode Node = (TreeNode)sender;

            MessageBox.Show(Node.FullPath);
        }


        private void ToolStripButton1_Click(object sender, EventArgs e)
        {

            var NodeDOFS = new Dictionary<int, int>();
            foreach (int NodeID in P.Nodes.AllIDs)
                NodeDOFS.Add(NodeID, P.Nodes.NodeObj(NodeID).Dimension);

            SparseMatrix K_assembled = P.Connect.Assemble_K_Mtx(P.Elements.get_K_matricies(P.Connect.ConnectMatrix, P.Nodes.NodeCoords, P.Materials.All_E), NodeDOFS);
            DenseMatrix[] output = P.Connect.Solve(K_assembled, P.Nodes.F_Mtx, P.Nodes.Q_Mtx);


            var outputstr1 = new List<string>();
            var outputstr2 = new List<string>();

            for (int i = 0, loopTo = output[0].RowCount - 1; i <= loopTo; i++)
            {
                for (int j = 0, loopTo1 = output[0].ColumnCount - 1; j <= loopTo1; j++)
                    outputstr1.Add(output[0][i, j].ToString());
            }

            for (int i = 0, loopTo2 = output[0].RowCount - 1; i <= loopTo2; i++)
            {
                for (int j = 0, loopTo3 = output[0].ColumnCount - 1; j <= loopTo3; j++)
                    outputstr2.Add(output[1][i, j].ToString());
            }

            MessageBox.Show(string.Join(",", outputstr1));
            MessageBox.Show(string.Join(",", outputstr2));
        }
    }
}
