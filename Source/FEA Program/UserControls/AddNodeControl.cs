using FEA_Program.Controls;
using FEA_Program.Models;
using FEA_Program.UI;

namespace FEA_Program.UserControls
{
    /// <summary>
    /// Menu for adding new nodes. Will resize for the node DOFs
    /// </summary>
    internal partial class AddNodeControl : UserControl
    {
        private readonly int _DOFs = 0;
        private readonly string[] _DOFNames = ["X", "Y", "Z"];

        private readonly int _labelX = 6;
        private readonly int _labelY = 60;
        private readonly int _YIncrement = 25;

        private readonly List<NumericalInputTextBox> _TextBoxes = [];
        private readonly List<CheckBox> _CheckBoxes = [];

        public event NodeAddFormSuccessEventHandler? NodeAddFormSuccess;
        public delegate void NodeAddFormSuccessEventHandler(AddNodeControl sender, List<double[]> Coords, List<int[]> Fixity, List<int> Dimensions);

        public AddNodeControl(int nodeDOFs)
        {
            int[] _FirstLabelPos = [_labelX, _labelY];
            int[] _FirstTextBoxPos = [_labelX + 26, _labelY - 2];
            int[] _FirstCheckBoxPos = [_labelX + 140, _labelY - 4];

            InitializeComponent();

            _DOFs = nodeDOFs;

            for (int i = 0; i < nodeDOFs; i++)
            {
                // -------------------- add label ------------------
                var LB = new Label
                {
                    Text = i < _DOFNames.Length ? _DOFNames[i] : "#",
                    Width = 10,
                    Anchor = AnchorStyles.Left | AnchorStyles.Top,
                    Location = new Point(_FirstLabelPos[0], _FirstLabelPos[1] + _YIncrement * i)
                };
                Controls.Add(LB);

                // ---------------- add textbox ---------------------
                var txt = new NumericalInputTextBox(100, new Point(_FirstTextBoxPos[0], _FirstTextBoxPos[1] + _YIncrement * i), Units.DataUnitType.Length, Units.AllUnits.mm)
                {
                    Tag = i
                };
                txt.TextChanged += OnTextFieldChanged;
                _TextBoxes.Add(txt);
                Controls.Add(txt);

                // -------------- add checkbox -------------------
                var CK = new CheckBox
                {
                    Checked = false,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Location = new Point(_FirstCheckBoxPos[0], _FirstCheckBoxPos[1] + _YIncrement * i),
                    Tag = i
                };
                _CheckBoxes.Add(CK);
                Controls.Add(CK);

                // ------------ move buttons down ----------------
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

                if (ReferenceEquals(sendbtn, Button_Accept))
                {
                    var coords = new double[_DOFs];
                    var fixity = new int[_DOFs];

                    for (int i = 0; i < _DOFs; i++)
                    {
                        coords[i] = _TextBoxes[i].Value;
                        fixity[i] = _CheckBoxes[i].Checked ? 1 : 0;
                    }

                    NodeAddFormSuccess?.Invoke(this, [coords], [fixity], [_DOFs]);
                }

                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding node: " + ex.Message);
            }
        }    
        private void Button_FixFloat_Click(object sender, EventArgs e)
        {
            try
            {
                // The boolean condition determines the value to assign to Ck.Checked.
                bool isFixAll = ReferenceEquals((Button)sender, Button_FixAll);

                foreach (CheckBox Ck in _CheckBoxes)
                {
                    Ck.Checked = isFixAll;
                }
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }
        private void OnTextFieldChanged(object? sender, EventArgs e)
        {
            try
            {
                ValidateEntry();
            }
            catch (Exception ex)
            {
                FormattedMessageBox.DisplayError(ex);
            }
        }

        private void ValidateEntry()
        {
            try
            {
                foreach (NumericalInputTextBox txt in _TextBoxes)
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
