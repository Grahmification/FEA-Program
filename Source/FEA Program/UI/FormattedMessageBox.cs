using System.Windows;

namespace FEA_Program.UI
{
    /// <summary>
    /// Class providing a global entry point for displaying message boxes
    /// </summary>
    public static class FormattedMessageBox
    {
        /// <summary>
        /// Displays a user message in a <see cref="MessageBox"/>
        /// </summary>
        /// <param name="message">The user message</param>
        /// <param name="header">Optional header for the messagebox</param>
        /// <returns>Result from the message box button</returns>
        public static MessageBoxResult DisplayMessage(string message, string header = "")
        {
            if (header == "")
                header = "Info";

            return MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Displays an error message to the user in a <see cref="MessageBox"/>
        /// </summary>
        /// <param name="message">The error message</param>
        /// <returns>Result from the message box button</returns>
        public static MessageBoxResult DisplayError(string message)
        {
            return MessageBox.Show($"An error occurred: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Displays an error message to the user in a <see cref="MessageBox"/>
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns>Result from the message box button</returns>
        public static MessageBoxResult DisplayError(Exception ex)
        {
            return MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Displays a user message in a <see cref="MessageBox"/>
        /// </summary>
        /// <param name="message">The user message</param>
        /// <param name="header">Optional header for the messagebox</param>
        /// <returns>Result from the message box button</returns>
        public static MessageBoxResult DisplayYesNoQuestion(string message, string header = "")
        {
            if (header == "")
                header = "Info";

            return MessageBox.Show(message, header, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
    }
}
