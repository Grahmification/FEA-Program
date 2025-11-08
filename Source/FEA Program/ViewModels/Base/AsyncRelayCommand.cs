using System.Windows.Input;
/*
* https://stackoverflow.com/questions/42712848/async-icommand-implementation
*/

namespace FEA_Program.ViewModels.Base
{
    /// <summary>
    /// A <see cref="RelayCommand"/> for async applications
    /// </summary>
    internal class AsyncRelayCommand : ICommand
    {
        /// <summary>
        /// The action to run
        /// </summary>
        private readonly Func<Task> mAction;

        private readonly Func<bool>? canExecuteEvaluator;

        /// <summary>
        /// The event that's fired when CanExecute value has changed
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public AsyncRelayCommand(Func<Task> action, Func<bool>? canExecute = null)
        {
            mAction = action;
            canExecuteEvaluator = canExecute;
        }

        /// <summary>
        /// A relay command can always execute (will cause button to be greyed out if false, etc.)
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object? parameter)
        {
            if (canExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                bool result = canExecuteEvaluator.Invoke();
                return result;
            }
        }

        /// <summary>
        /// Runs the action
        /// </summary>
        /// <param name="parameter"></param>
        public async void Execute(object? parameter = null)
        {
            await mAction();
        }
    }
}
