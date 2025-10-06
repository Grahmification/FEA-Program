using System.Windows.Input;

namespace FEA_Program.ViewModels.Base
{
    internal class RelayCommand<T> : ICommand where T : new()
    {
        /// <summary>
        /// The action to run
        /// </summary>
        private readonly Action<T> mAction;

        private readonly Func<bool>? canExecuteEvaluator;

        /// <summary>
        /// The event that's fired when CanExecute value has changed
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<T> action, Func<bool>? canExecute = null)
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
        public void Execute(object? parameter = null)
        {
            parameter ??= new T(); // Initialize to new if the object is null
            mAction((T)parameter);
        }

    }
}
