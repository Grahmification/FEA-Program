using FEA_Program.ViewModels;
using System.Windows;
using System.Windows.Threading;

namespace FEA_Program
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Global program units
        /// </summary>
        internal static UnitsVM Units { get; } = new UnitsVM();


        /// <summary>
        /// Fires when an unhandled exception occurs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }

}
