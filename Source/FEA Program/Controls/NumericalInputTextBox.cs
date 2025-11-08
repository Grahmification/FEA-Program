using FEA_Program.Models;

namespace FEA_Program.Controls
{
    internal class NumericalInputTextBox : TextBox
    {
        private Units.DataUnitType _unitType;
        private Units.AllUnits _DefaultInputUnit;

        public double Value
        {
            get
            {
                if (CheckInput() == false)
                {
                    this.SelectAll();
                    throw new Exception("Incorrect value <" + this.Text + "> entered into textbox.");
                }

                return ConvertInput();
            }
            set
            {
                Text = ConvertValueTo(value);
            }
        }

        public NumericalInputTextBox(int Width, Point Location, Units.DataUnitType unitType, Units.AllUnits DefaultInputUnit)
        {
            _unitType = unitType;
            _DefaultInputUnit = DefaultInputUnit;

            Value = 0;


            this.Width = Width;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            this.Location = Location;
            base.TextChanged += OnTextChanged;
            base.Click += TextBox_MouseClick;

        }

        public NumericalInputTextBox()
        {
            base.TextChanged += OnTextChanged;
            base.Click += TextBox_MouseClick;

        }

        private bool ValidateText()
        {
            if (CheckInput() == true)
            {
                this.BackColor = Color.White;
                return true;
            }
            else
            {
                this.BackColor = Color.IndianRed;
                return false;
            }
        }

        private void OnTextChanged(object? sender, EventArgs e)
        {
            ValidateText();
        }

        private void TextBox_MouseClick(object? sender, EventArgs e)
        {
            if(sender is not null)
            {
                TextBox sendtxt = (TextBox)sender;
                sendtxt.SelectAll();
            }
        }

        private bool CheckInput()
        {
            try
            {
                string mytext = this.Text;
                string unitIdentifier = "";

                if ((int)_unitType != -1 & (int)_DefaultInputUnit != -1)
                {
                    List<string> allowedUnitStrings = Units.TypeUnitStrings(_unitType);

                    for (int i = 0, loopTo = allowedUnitStrings.Count - 1; i <= loopTo; i++)
                    {
                        if (mytext.Contains(allowedUnitStrings[i]))
                        {
                            unitIdentifier = allowedUnitStrings[i];
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(unitIdentifier))
                    {
                        mytext = mytext.Replace(unitIdentifier, "");
                    }
                }

                double inputData_dbl = double.Parse(mytext);

                // Me.Text = CStr(Math.Round(Units.Convert(Units.UnitEnums(unitIdentifier), inputData_dbl, _DefaultInputUnit), 5)) & Units.UnitStrings(_DefaultInputUnit)(0)

                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }
        private double ConvertInput()
        {
            string mytext = this.Text;
            string unitIdentifier = "";
            double output;

            if ((int)_unitType == -1 | (int)_DefaultInputUnit == -1)
            {
                output = double.Parse(Text);
            }
            else
            {

                List<string> allowedUnitStrings = Units.TypeUnitStrings(_unitType);

                for (int i = 0, loopTo = allowedUnitStrings.Count - 1; i <= loopTo; i++)
                {
                    if (mytext.Contains(allowedUnitStrings[i]))
                    {
                        unitIdentifier = allowedUnitStrings[i];
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(unitIdentifier))
                {
                    mytext = mytext.Replace(unitIdentifier, "");
                    output = Units.Convert(Units.UnitEnums(unitIdentifier), double.Parse(mytext), Units.DefaultUnit(_unitType)); // convert to the default type
                }
                else
                {
                    output = Units.Convert(_DefaultInputUnit, double.Parse(mytext), Units.DefaultUnit(_unitType));
                } // if no specific type specified need to convert from the default input type
            }
            return output;
        }
        private string ConvertValueTo(double value)
        {       
            // Unit is invalid
            if ((int)_DefaultInputUnit == -1 | (int)_unitType == -1)
            {
                return $"{value:F3}";
            }
            else
            {
                value = Units.Convert(Units.DefaultUnit(_unitType), value, _DefaultInputUnit);
                return $"{value:F3}{Units.UnitStrings(_DefaultInputUnit)[0]}";
            }
        }

    }
}
