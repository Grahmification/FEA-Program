namespace FEA_Program.Controls
{
    internal class CodeInputComboBox : ComboBox
    {

        private AutoCompleteStringCollection CommandCollection = new AutoCompleteStringCollection();
        private Dictionary<string, List<Type>> _CommandList = new Dictionary<string, List<Type>>(StringComparer.OrdinalIgnoreCase); // case insensitivity

        public event CommandEnteredEventHandler CommandEntered;

        public delegate void CommandEnteredEventHandler(string CommandName, List<object> Arguments);

        public CodeInputComboBox(Dictionary<string, List<Type>> CommandList) // string is command name, type list holds argument types
        {

            foreach (KeyValuePair<string, List<Type>> KVP in CommandList) // copy so case insensitivity doesn't get wrecked by outside
                _CommandList.Add(KVP.Key, KVP.Value);

            foreach (KeyValuePair<string, List<Type>> CMD in CommandList)
            {
                string SB = CMD.Key + "(";

                if (CMD.Value is not null)
                {
                    for (int i = 1, loopTo = CMD.Value.Count - 1; i <= loopTo; i++)
                        SB = SB + ",";
                }

                SB = SB + ")";

                CommandCollection.Add(SB);
            }

            this.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.AutoCompleteCustomSource = CommandCollection;
            this.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;

            UserLostFocus(this, new());
            base.KeyDown += KeyPressed;
            base.LostFocus += UserLostFocus;
            base.GotFocus += UserGotFocus;
        }

        private void KeyPressed(object? sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string data = this.Text;
                    data = RemoveWhitespace(data); // remove any spaces
                    string commandName = data.Split("(").First();

                    if (_CommandList.Keys.Contains(commandName, StringComparer.CurrentCultureIgnoreCase) == false) // check if command is in command list
                    {
                        throw new Exception();
                    }

                    string[] Arguments = data.Split("(").Last().Split(")").First().Split(",");
                    var outputArgs = new List<object>();

                    if (_CommandList[commandName] is not null) // only need to do if we have arguments
                    {
                        if (Arguments.Count() != _CommandList[commandName].Count) // check for correct number of arguments
                        {
                            throw new Exception();
                        }

                        for (int i = 0, loopTo = Arguments.Count() - 1; i <= loopTo; i++) // check args are correct type
                        {
                            switch (_CommandList[commandName][i])
                            {

                                case var @case when @case == typeof(int):
                                    {
                                        int tmp = int.Parse(Arguments[i]);
                                        break;
                                    }
                                case var case1 when case1 == typeof(double):
                                    {
                                        double tmp = double.Parse(Arguments[i]);
                                        break;
                                    }
                                case var case2 when case2 == typeof(string):
                                    {
                                        string tmp = Arguments[i];
                                        break;
                                    }
                                case var case3 when case3 == typeof(bool):
                                    {
                                        bool tmp = bool.Parse(Arguments[i]);
                                        break;
                                    }
                            }

                            outputArgs.Add(Arguments[i]);
                        }
                    }

                    this.BackColor = Color.White;
                    this.Items.Add(this.Text);
                    CommandEntered?.Invoke(commandName, outputArgs);
                }
            }
            catch (Exception ex)
            {
                this.BackColor = Color.IndianRed;
            }
        }

        private void UserLostFocus(object? sender, EventArgs e)
        {
            if (this.Text == "")
            {
                this.Text = "Type a command...";
            }
        }
        private void UserGotFocus(object? sender, EventArgs e)
        {
            if (this.Text == "Type a command...")
            {
                this.Text = "";
            }
        }


        private string RemoveWhitespace(string fullString)
        {
            return new string(fullString.Where(x => !char.IsWhiteSpace(x)).ToArray());
        } // removes any spaces from string

    }
}
