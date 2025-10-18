using System.Windows.Data;
using System.Windows.Input;
using UserControl = System.Windows.Controls.UserControl;
using TextBox = System.Windows.Controls.TextBox;

namespace FEA_Program.Views
{
    /// <summary>
    /// Interaction logic for AddElementControl.xaml
    /// </summary>
    public partial class AddElementControl : UserControl
    {
        public AddElementControl()
        {
            InitializeComponent();
        }


        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                if (textBox != null)
                {
                    // Get the binding expression for the Text property
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);

                    // Manually push the current TextBox text value to the source property (Value)
                    binding?.UpdateSource();

                    // Optional: Move focus away after pressing Enter (improves UX)
                    // This line requires System.Windows.Input;
                    Keyboard.ClearFocus();
                }
            }
        }
    }
}
