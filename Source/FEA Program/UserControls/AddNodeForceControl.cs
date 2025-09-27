using FEA_Program.Controls;
using FEA_Program.Models;

namespace FEA_Program.UserControls
{
    internal partial class AddNodeForceControl : UserControl
    {
        private int _DOFs = -1;
        private string[] _DOFNames = new[] { "X", "Y", "Z" };
        private List<Node> _Nodes;

        private int _YIncrement = 50;
        private int[] _FirstLabelPos = new[] { 4, 46 };
        private int[] _FirstTextBoxPos = new[] { 6, 66 };

        private List<Label> _Labels = new List<Label>();
        private List<NumericalInputTextBox> _TxtBoxes = new List<NumericalInputTextBox>();


        public event NodeForceAddFormSuccessEventHandler? NodeForceAddFormSuccess;

        public delegate void NodeForceAddFormSuccessEventHandler(AddNodeForceControl sender, List<double[]> Forces, List<int> NodeIDs);
        public event NodeSelectionUpdatedEventHandler? NodeSelectionUpdated;

        public delegate void NodeSelectionUpdatedEventHandler(object sender, List<int> SelectedNodeIDs);


        public AddNodeForceControl(int NodeDOFs, List<Node> Nodes)
        {
            InitializeComponent();

            _DOFs = NodeDOFs;
            _Nodes = Nodes;


            // -------------------- add force component textboxes ------------------

            for (int I = 0, loopTo = NodeDOFs - 1; I <= loopTo; I++)
            {
                // -------------------- add label ------------------
                var LB = new Label();

                if (I < _DOFNames.Length)
                {
                    LB.Text = _DOFNames[I] + " Component (N)";
                }
                else
                {
                    LB.Text = "#";
                } // handles potential error case where names run out

                LB.Width = 100;
                LB.Height = 13;
                LB.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                LB.Location = new Point(_FirstLabelPos[0], _FirstLabelPos[1] + _YIncrement * I);

                _Labels.Add(LB);
                this.Controls.Add(LB);

                // -------------------- add textbox ------------------

                var txt = new NumericalInputTextBox(100, new Point(_FirstTextBoxPos[0], _FirstTextBoxPos[1] + _YIncrement * I), Units.DataUnitType.Force, Units.AllUnits.N);

                txt.Tag = I;
                _TxtBoxes.Add(txt);
                this.Controls.Add(txt);
            }
        }

        private void ButtonAccept_Click(object sender, EventArgs e)
        {
            try
            {
                Button sendbtn = (Button)sender;

                if (object.ReferenceEquals(sendbtn, Button_Accept))
                {

                    var SelectedNodeIDs = GetCheckedListBoxNodeIds(); // get the node IDs

                    var ForceComponents = new List<double>();
                    for (int i = 0, loopTo = _DOFs - 1; i <= loopTo; i++)
                        ForceComponents.Add(_TxtBoxes[i].Value); // get the force components depending how many DOFs are available

                    var CopiedForceComponents = new List<double[]>(); // need to copy the force for each node ID to get the right input format
                    for (int i = 0, loopTo1 = SelectedNodeIDs.Count - 1; i <= loopTo1; i++)
                        CopiedForceComponents.Add(ForceComponents.ToArray());

                    NodeSelectionUpdated?.Invoke(this, new List<int>()); // deselect all nodes
                    NodeForceAddFormSuccess?.Invoke(this, CopiedForceComponents, SelectedNodeIDs);
                }

                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding force: " + ex.Message);
            }
        }
        private void ValidateEntry(object sender, EventArgs e)
        {
            try
            {
                if (CheckedListBox_ApplyNodes.CheckedItems.Count == 0) // need to select at least 1 node to add
                {
                    throw new Exception();
                }

                Button_Accept.Enabled = true; // if this works for all then everything is ok
            }
            catch (Exception)
            {
                Button_Accept.Enabled = false;
            }
        }

        private void CheckedListBox_ApplyNodes_Click(object sender, EventArgs e)
        {
            try
            {
                var SelectedNodeIDs = GetCheckedListBoxNodeIds();

                NodeSelectionUpdated?.Invoke(this, SelectedNodeIDs);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred when attempting to highlight selected node.");
            }
        }
        private List<int> GetCheckedListBoxNodeIds()
        {
            var SelectedNodeIDs = new List<int>();

            foreach (string TextValue in CheckedListBox_ApplyNodes.CheckedItems)
            {
                if (TextValue is not null) // no selection in combobox
                {
                    int NODEID = int.Parse(TextValue.Split(" ").First()); // split based on previous formatting to get node id
                    SelectedNodeIDs.Add(NODEID);
                }
            }

            return SelectedNodeIDs;
        }

        private void UserControl_AddElement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter & Button_Accept.Enabled)
            {
                Button_Accept.PerformClick();
            }
        }
    }
}
