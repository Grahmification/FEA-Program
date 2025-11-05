using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FEA_Program.Views
{
    /// <summary>
    /// Interaction logic for EditNodeControl.xaml
    /// </summary>
    public partial class EditNodeControl : UserControl
    {
        public EditNodeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when a key is pressed on one of the number entry textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Force the textbox to validate when the enter key is pressed
            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;
                var binding = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);
                binding?.UpdateSource();
            }
        }
    }
}
