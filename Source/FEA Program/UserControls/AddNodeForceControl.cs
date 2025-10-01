using FEA_Program.Controls;
using FEA_Program.Models;
using FEA_Program.UI;

namespace FEA_Program.UserControls
{
    internal partial class AddNodeForceControl : UserControl
    {
        private int _DOFs = -1;
        private string[] _DOFNames = ["X", "Y", "Z"];
        private List<Node> _Nodes;

        private const int _YIncrement = 40;
        private const int _YStart = 46;
        private int[] _FirstLabelPos = [4, _YStart];
        private int[] _FirstTextBoxPos = [6, _YStart + 20];

        private List<Label> _Labels = [];
        private List<NumericalInputTextBox> _TxtBoxes = [];


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

            for (int i = 0; i <= NodeDOFs - 1; i++)
            {
                // -------------------- add label ------------------
                var LB = new Label();

                // handles potential error case where names run out
                LB.Text = (i < _DOFNames.Length) ? $"{_DOFNames[i]} Component (N)" : "#";
                LB.Width = 100;
                LB.Height = 13;
                LB.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                LB.Location = new Point(_FirstLabelPos[0], _FirstLabelPos[1] + _YIncrement * i);

                _Labels.Add(LB);
                this.Controls.Add(LB);

                // -------------------- add textbox ------------------

                var txt = new NumericalInputTextBox(100, new Point(_FirstTextBoxPos[0], _FirstTextBoxPos[1] + _YIncrement * i), Units.DataUnitType.Force, Units.AllUnits.N);

                txt.Tag = i;
                _TxtBoxes.Add(txt);
                this.Controls.Add(txt);
            }

            // ----------------- Move Checklist down -----------------------

            CheckedListBox_ApplyNodes.Location = new Point(CheckedListBox_ApplyNodes.Location.X, CheckedListBox_ApplyNodes.Location.Y + _YIncrement * _DOFs);
            CheckedListBox_ApplyNodes.Height -= _YIncrement * _DOFs;

            Label2.Location = new Point(Label2.Location.X, Label2.Location.Y + _YIncrement * _DOFs);

            // ------------------ populate node checklist --------------------

            foreach (Node Node in _Nodes)
            {
                string text = $"{Node.ID} - ({string.Join(",", Node.Coords_mm)})";
                CheckedListBox_ApplyNodes.Items.Add(text);
            }

            ValidateEntry();
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
        private void UserControl_AddElement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter & Button_Accept.Enabled)
            {
                Button_Accept.PerformClick();
            }
        }
        private void CheckedListBox_ApplyNodes_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                var SelectedNodeIDs = GetCheckedListBoxNodeIds();

                NodeSelectionUpdated?.Invoke(this, SelectedNodeIDs);

                ValidateEntry();
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
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
        private void ValidateEntry()
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
    }
}
