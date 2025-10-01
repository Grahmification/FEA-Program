using FEA_Program.Controls;
using FEA_Program.Models;
using FEA_Program.UI;

namespace FEA_Program.UserControls
{
    internal partial class AddElementControl : UserControl
    {
        private Type[] _AvailableElemTypes;
        private List<int> _MatIDs = new List<int>();
        private List<Node> _Nodes;
        private List<Dictionary<string, Units.DataUnitType>> _ElementArgs;

        private int _YIncrement = 50;
        private int[] _FirstLabelPos;
        private int[] _FirstComboBoxPos;
        private int[] _FirstTextBoxPos;

        private List<ComboBox> _ComboBoxes = new List<ComboBox>();
        private List<Label> _Labels = new List<Label>();
        private List<NumericalInputTextBox> _TxtBoxes = new List<NumericalInputTextBox>();

        private AutoCompleteStringCollection nodeCollection = new AutoCompleteStringCollection();

        public event ElementAddFormSuccessEventHandler ElementAddFormSuccess;

        public delegate void ElementAddFormSuccessEventHandler(AddElementControl sender, Type Type, List<int> NodeIDs, double[] ElementArgs, int Mat);
        public event NodeSelectionUpdatedEventHandler NodeSelectionUpdated;

        public delegate void NodeSelectionUpdatedEventHandler(object sender, List<int> SelectedNodeIDs);

        public AddElementControl(Type[] AvailableElemTypes, List<Dictionary<string, Units.DataUnitType>> ElementArgs, List<Material> Mats, List<Node> Nodes)
        {
            int startingY = 120;
            _FirstLabelPos = new[] { 8, startingY + _YIncrement };
            _FirstComboBoxPos = new[] { 7, 20 + startingY + _YIncrement };
            _FirstTextBoxPos = new[] { 7, 20 + startingY + _YIncrement };

            InitializeComponent();

            _AvailableElemTypes = AvailableElemTypes;
            _ElementArgs = ElementArgs;
            _Nodes = Nodes;

            // -------------------- add element types ------------------

            {
                var withBlock = _ComboBox_ElemType;
                foreach (Type i in AvailableElemTypes)
                    withBlock.Items.Add(ElementManager.Name(i));

                if (withBlock.Items.Count > 0)
                {
                    withBlock.SelectedIndex = 0;
                }
            }

            // --------------- add materials --------------------

            {
                var withBlock1 = _ComboBox_Material;
                foreach (Material m in Mats)
                {
                    withBlock1.Items.Add(m.Name);
                    _MatIDs.Add(m.ID);
                }

                if (withBlock1.Items.Count > 0)
                {
                    withBlock1.SelectedIndex = 0;
                }
            }

            ValidateEntry();
        }

        private void ButtonAccept_Click(object? sender, EventArgs e)
        {
            try
            {
                Button sendbtn = (Button)sender;

                if (object.ReferenceEquals(sendbtn, _Button_Accept))
                {

                    var ElemType = _AvailableElemTypes[_ComboBox_ElemType.SelectedIndex];


                    var NodeIDs = new List<int>();
                    foreach (ComboBox CBX in _ComboBoxes)
                        NodeIDs.Add(int.Parse(CBX.Text.Split(" ").First()));


                    var ElementArgs = new List<double>();
                    foreach (NumericalInputTextBox txt in _TxtBoxes)
                        ElementArgs.Add(txt.Value);


                    int MatID = this._MatIDs[_ComboBox_Material.SelectedIndex];

                    ElementAddFormSuccess?.Invoke(this, ElemType, NodeIDs, ElementArgs.ToArray(), MatID);
                }

                this.Dispose();
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError("Error adding element: " + ex.Message);
            }
        }

        private void ComboBoxSelectionChanged(object? sender, EventArgs e)
        {
            try
            {
                ValidateEntry();
            }
            catch(Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }

        private void ValidateEntry()
        {
            try
            {
                if (_ComboBox_ElemType.Items.Count == 0 | _ComboBox_Material.Items.Count == 0 | _Nodes.Count == 0)
                {
                    throw new Exception();
                }

                var NodeCBoxIndexes = new List<int>();

                foreach (ComboBox Cbox in _ComboBoxes)
                {
                    NodeCBoxIndexes.Add(Cbox.SelectedIndex);

                    if (Cbox.SelectedItem == null)
                        throw new Exception();
                }
                    

                if (AllIndexesDifferent(NodeCBoxIndexes) == false)
                {
                    throw new Exception();
                }


                _Button_Accept.Enabled = true; // if this works for all then everything is ok
            }
            catch (Exception)
            {
                _Button_Accept.Enabled = false;
            }
        }



        private void ComboBox_ElemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox CBX = (ComboBox)sender;

                // ------------------ First remove any previous boxes --------------------------

                foreach (Label LB in _Labels)
                    LB.Dispose();
                _Labels.Clear();

                foreach (ComboBox CB in _ComboBoxes)
                {
                    CB.SelectionChangeCommitted -= ComboBoxSelectionChanged;
                    CB.SelectionChangeCommitted -= NodeComboBoxSelectionChanged; // for node selection event
                    CB.Dispose();
                }
                _ComboBoxes.Clear();

                foreach (TextBox txt in _TxtBoxes)
                    txt.Dispose();
                _TxtBoxes.Clear();

                // ------------------ Add new boxes for each node ----------------

                for (int I = 0, loopTo = ElementManager.NumOfNodes(_AvailableElemTypes[CBX.SelectedIndex]) - 1; I <= loopTo; I++)
                {

                    var LB = new Label();
                    LB.Text = "Node " + (I + 1).ToString();
                    LB.Width = 80;
                    LB.Height = 16;
                    LB.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    LB.Location = new Point(_FirstLabelPos[0], _FirstLabelPos[1]);
                    _Labels.Add(LB);
                    this.Controls.Add(LB);

                    var CBox = new ComboBox();
                    CBox.Width = 196;
                    CBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                    CBox.Location = new Point(_FirstComboBoxPos[0], _FirstComboBoxPos[1]);
                    CBox.Tag = I;
                    CBox.AutoCompleteCustomSource = nodeCollection;
                    CBox.SelectionChangeCommitted += ComboBoxSelectionChanged;

                    _ComboBoxes.Add(CBox);
                    this.Controls.Add(CBox);

                    _FirstLabelPos[1] += _YIncrement;
                    _FirstComboBoxPos[1] += _YIncrement;
                    _FirstTextBoxPos[1] += _YIncrement;
                }

                // --------------------- Populate Node Comboboxes -------------------------

                foreach (Node Node in _Nodes)
                {
                    string text = (string)Node.ID.ToString() + " - (" + string.Join(",", Node.Coords_mm) + ")";

                    nodeCollection.Add(text);

                    foreach (ComboBox CB in _ComboBoxes)
                        CB.Items.Add(text);
                }

                // ------------------------------------Add handler for node combobox highlight selection event -------------
                // do after population so not firing over and over

                foreach (ComboBox Cbox in _ComboBoxes)
                    Cbox.SelectedIndexChanged += NodeComboBoxSelectionChanged; // for node selection event

                // -------------------- Add TextBoxes for Element Arguments -------------

                foreach (KeyValuePair<string, Units.DataUnitType> Arg in this._ElementArgs[CBX.SelectedIndex])
                {
                    var LB = new Label();
                    LB.Text = Arg.Key;
                    LB.Width = 80;
                    LB.Height = 16;
                    LB.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    LB.Location = new Point(_FirstLabelPos[0], _FirstLabelPos[1]);
                    _Labels.Add(LB);
                    this.Controls.Add(LB);

                    var txt = new NumericalInputTextBox(196, new Point(_FirstTextBoxPos[0], _FirstTextBoxPos[1]), Arg.Value, Units.AllUnits.mm_squared);
                    _TxtBoxes.Add(txt);
                    this.Controls.Add(txt);

                    _FirstTextBoxPos[1] += _YIncrement;
                    _FirstLabelPos[1] += _YIncrement;
                }
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }

        private bool AllIndexesDifferent(List<int> Indexes)
        {

            for (int i = 0, loopTo = Indexes.Count - 1; i <= loopTo; i++)
            {
                for (int j = 0, loopTo1 = Indexes.Count - 1; j <= loopTo1; j++)
                {
                    if (i != j & Indexes[i] == Indexes[j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void UserControl_AddElement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter & _Button_Accept.Enabled)
            {
                _Button_Accept.PerformClick();
            }
        }

        private void NodeComboBoxSelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var SelectedNodeIDs = new List<int>();

                foreach (ComboBox CB in _ComboBoxes)
                {
                    string? Data = CB.SelectedItem?.ToString();

                    if (Data is not null) // no selection in combobox
                    {
                        int NODEID = int.Parse(Data.Split(" ").First()); // split based on previous formatting to get node id
                        SelectedNodeIDs.Add(NODEID);
                    }
                }

                NodeSelectionUpdated?.Invoke(this, SelectedNodeIDs);
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        } // raises selection event to highlight nodes
    }
}
