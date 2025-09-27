using FEA_Program.Controls;
using FEA_Program.Models;

namespace FEA_Program.UserControls
{
    internal partial class AddMaterialControl : UserControl
    {
        private List<int> _EnumValues = new List<int>();
        private List<NumericalInputTextBox> _TxtBoxes = new List<NumericalInputTextBox>();

        private int _YIncrement = 50;
        private int[] _FirstLabelPos = new[] { 6, 96 };
        private int[] _FirstTextBoxPos = new[] { 5, 115 };

        public event MatlAddFormSuccessEventHandler MatlAddFormSuccess;
        public delegate void MatlAddFormSuccessEventHandler(AddMaterialControl sender, string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, int subtype);

        public AddMaterialControl(Type MatlSubTypes)
        {
            InitializeComponent();

            // -------------------- Set Up Combobox -----------------------

            foreach (int Val in Enum.GetValues(MatlSubTypes))
                _EnumValues.Add(Val);

            foreach (string Val in Enum.GetNames(MatlSubTypes))
                ComboBox_SubTypes.Items.Add(Val);

            if (ComboBox_SubTypes.Items.Count > 0)
            {
                ComboBox_SubTypes.SelectedIndex = 0;
            }

            // ------------------- Set up textboxes ----------------------

            string[] labelText = new[] { "E (GPa)", "V", "Sy (MPa)", "Sut (MPa)" };

            foreach (string S in labelText)
            {
                var LB = new Label();
                LB.Text = S;
                LB.Width = 80;
                LB.Height = 16;
                LB.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                LB.Location = new Point(_FirstLabelPos[0], _FirstLabelPos[1]);
                this.Controls.Add(LB);


                int unitType = (int)Units.DataUnitType.Pressure;
                int DefaultInput = (int)Units.AllUnits.MPa;

                if (S == "V")
                {
                    unitType = -1;
                    DefaultInput = -1;
                }
                else if (S == "E (GPa)")
                {
                    DefaultInput = (int)Units.AllUnits.GPa;
                }

                var txt = new NumericalInputTextBox(185, new Point(_FirstTextBoxPos[0], _FirstTextBoxPos[1]), (Units.DataUnitType)unitType, (Units.AllUnits)DefaultInput);
                _TxtBoxes.Add(txt);
                this.Controls.Add(txt);

                _FirstTextBoxPos[1] += _YIncrement;
                _FirstLabelPos[1] += _YIncrement;
            }

            this.KeyDown += UserControl_AddElement_KeyDown;

        }

        private void ButtonAccept_Click(object? sender, EventArgs e)
        {
            try
            {
                Button sendbtn = (Button)sender;

                if (object.ReferenceEquals(sendbtn, Button_Accept))
                {

                    string _name = TextBox_Name.Text;
                    double _E = _TxtBoxes[0].Value;
                    double _V = _TxtBoxes[1].Value;
                    double _Sy = _TxtBoxes[2].Value;
                    double _Sut = _TxtBoxes[3].Value;
                    int _Type = this._EnumValues[ComboBox_SubTypes.SelectedIndex];

                    MatlAddFormSuccess?.Invoke(this, _name, _E, _V, _Sy, _Sut, _Type);
                }

                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding material: " + ex.Message);
            }
        }
        private void ValidateEntry(object sender, EventArgs e)
        {
            try
            {
                if (TextBox_Name.Text == "")
                {
                    throw new Exception("Invalid Name");
                }

                if (ComboBox_SubTypes.Items.Count == 0)
                {
                    throw new Exception("No Subtypes");
                }

                Button_Accept.Enabled = true; // if this works for all then everything is ok
            }
            catch (Exception ex)
            {
                Button_Accept.Enabled = false;
            }
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
