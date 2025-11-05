using FEA_Program.Utils;

namespace FEA_Program.ViewModels.Base
{
    /// <summary>
    /// Base level VM to control general program status and error display
    /// </summary>
    internal class BaseVM : ObservableObject
    {
        /// <summary>
        /// If true, will enable debugging messages. Use this to enable debugging for other VMs
        /// </summary>
        public bool Debugging { get; set; } = false;

        /// <summary>
        /// The program version string to display
        /// </summary>
        public string ProgramVersion { get; set; } = "X.X.X";

        /// <summary>
        /// The program status string to display
        /// </summary>
        public string Status { get; private set; } = "";

        /// <summary>
        /// Sets the program display status
        /// </summary>
        /// <param name="status">The string to display</param>
        public void SetStatus(string status)
        {
            Status = status;
        }

        /// <summary>
        /// Clears the program status string
        /// </summary>
        public void ClearStatus()
        {
            Status = "";
        }

        /// <summary>
        /// Logs an exception and displays it to the user
        /// </summary>
        /// <param name="ex">The exception to display</param>
        public void LogAndDisplayException(Exception ex)
        {
            if (Debugging)
            {
                DisplayError(ex.ToString());
            }
            else
            {
                DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Displays an error to the user
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="blocking">True if this call should block until the user accepts the message</param>
        public void DisplayError(string message, bool blocking = false)
        {
            if (blocking)
            {
                FormattedMessageBox.DisplayError(message);
            }
            else
            {
                new Thread(() => FormattedMessageBox.DisplayError(message)).Start();
            }
        }

        /// <summary>
        /// Displays a message to the user
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="title">The messagebox header</param>
        public void DisplayMessage(string message, string title = "")
        {
            FormattedMessageBox.DisplayMessage(message, title);
        }

    }
}
