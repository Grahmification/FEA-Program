using FEA_Program.Controls;
using FEA_Program.Models;

namespace FEA_Program.UserControls
{
    internal partial class AddNodeControl : UserControl
    {
        private int _DOFs = -1;
        private string[] _DOFNames = ["X", "Y", "Z"];

        private int _YIncrement = 25;
        private int[] _FirstLabelPos;
        private int[] _FirstTextBoxPos;
        private int[] _FirstCheckBoxPos;

        private List<NumericalInputTextBox> _TxtBoxes = [];
        private List<CheckBox> _ChkBoxes = [];

        public event NodeAddFormSuccessEventHandler? NodeAddFormSuccess;

        public delegate void NodeAddFormSuccessEventHandler(AddNodeControl sender, List<double[]> Coords, List<int[]> Fixity, List<int> Dimensions);

        public AddNodeControl(int NodeDOFs)
        {
            _FirstLabelPos = new[] { 6, 36 + _YIncrement };
            _FirstTextBoxPos = new[] { 30, 34 + _YIncrement };
            _FirstCheckBoxPos = new[] { 145, 32 + _YIncrement };

            InitializeComponent();

            _DOFs = NodeDOFs;

            for (int I = 0, loopTo = NodeDOFs - 1; I <= loopTo; I++)
            {
                // -------------------- add label ------------------
                var LB = new Label();

                if (I < _DOFNames.Length)
                {
                    LB.Text = _DOFNames[I];
                }
                else
                {
                    LB.Text = "#";
                }

                LB.Width = 10;
                LB.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                LB.Location = new Point(_FirstLabelPos[0], _FirstLabelPos[1] + _YIncrement * I);
                this.Controls.Add(LB);


                // ---------------- add textbox ---------------------
                var txt = new NumericalInputTextBox(100, new Point(_FirstTextBoxPos[0], _FirstTextBoxPos[1] + _YIncrement * I), Units.DataUnitType.Length, Units.AllUnits.mm);
                this.Controls.Add(txt);
                txt.Tag = I;
                txt.TextChanged += OnTextFieldChanged;
                _TxtBoxes.Add(txt);


                // AddHandler txt.TextChanged, AddressOf ValidateEntry
                // AddHandler txt.MouseClick, AddressOf TextBox_MouseClick

                // -------------- add checkbox -------------------

                var CK = new CheckBox();
                CK.Checked = false;
                CK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                CK.Location = new Point(_FirstCheckBoxPos[0], _FirstCheckBoxPos[1] + _YIncrement * I);
                this.Controls.Add(CK);
                CK.Tag = I;
                _ChkBoxes.Add(CK);

                // ------------ move buttons

                Button_FixAll.Location = new Point(Button_FixAll.Location.X, Button_FixAll.Location.Y + _YIncrement);
                Button_UnfixAll.Location = new Point(Button_UnfixAll.Location.X, Button_UnfixAll.Location.Y + _YIncrement);

                ValidateEntry();
            }
        }

        private void ButtonAccept_Click(object sender, EventArgs e)
        {
            try
            {
                Button sendbtn = (Button)sender;

                if (object.ReferenceEquals(sendbtn, Button_Accept))
                {
                    var coords = new double[_DOFs];
                    var fixity = new int[_DOFs];

                    for (int i = 0, loopTo = _DOFs - 1; i <= loopTo; i++)
                    {
                        coords[i] = _TxtBoxes[i].Value;

                        if (_ChkBoxes[i].Checked)
                        {
                            fixity[i] = 1;
                        }
                        else
                        {
                            fixity[i] = 0;
                        }
                    }

                    var tmpCoords = new List<double[]>();
                    var tmpfixity = new List<int[]>();
                    var tmpDim = new List<int>();

                    tmpCoords.Add(coords);
                    tmpfixity.Add(fixity);
                    tmpDim.Add(_DOFs);

                    NodeAddFormSuccess?.Invoke(this, tmpCoords, tmpfixity, tmpDim);
                }

                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding node: " + ex.Message);
            }
        }
        private void ButtonAccept_Click_OLD(object sender, EventArgs e)
        {
            try
            {
                Button sendbtn = (Button)sender;

                if (object.ReferenceEquals(sendbtn, Button_Accept))
                {
                    var coords = new double[_DOFs];
                    var fixity = new int[_DOFs];

                    for (int i = 0, loopTo = _DOFs - 1; i <= loopTo; i++)
                    {
                        coords[i] = double.Parse(_TxtBoxes[i].Text) / 1000.0; // convert to m

                        if (_ChkBoxes[i].Checked)
                        {
                            fixity[i] = 1;
                        }
                        else
                        {
                            fixity[i] = 0;
                        }
                    }

                    var tmpCoords = new List<double[]>();
                    var tmpfixity = new List<int[]>();
                    var tmpDim = new List<int>();

                    tmpCoords.Add(coords);
                    tmpfixity.Add(fixity);
                    tmpDim.Add(_DOFs);

                    NodeAddFormSuccess?.Invoke(this, tmpCoords, tmpfixity, tmpDim);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding node:" + ex.Message);
            }
            this.Dispose();
        }
        private void Button_FixFloat_Click(object sender, EventArgs e)
        {
            try
            {
                // The boolean condition determines the value to assign to Ck.Checked.
                bool isFixAll = object.ReferenceEquals((Button)sender, Button_FixAll);

                foreach (CheckBox Ck in _ChkBoxes)
                {
                    Ck.Checked = isFixAll;
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        private void OnTextFieldChanged(object? sender, EventArgs e)
        {
            ValidateEntry();
        }

        private void ValidateEntry()
        {
            try
            {
                foreach (NumericalInputTextBox txt in _TxtBoxes)
                {
                    double value = txt.Value;
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
