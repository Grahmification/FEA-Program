using FEA_Program.UI;

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
        public string ProgramVersion { get; set; } = "X.X.X";
        public string Status { get; private set; } = "";

        public void SetStatus(string status)
        {
            Status = status;
        }
        public void ClearStatus()
        {
            Status = "";
        }

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
        public void DisplayMessage(string message, string title = "")
        {
            FormattedMessageBox.DisplayMessage(message, title);
        }

    }
}
