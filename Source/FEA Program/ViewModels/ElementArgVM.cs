using FEA_Program.Models;
using FEA_Program.ViewModels.Base;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for any parameter which can vary between different elements
    /// </summary>
    internal class ElementArgVM : ObservableObject
    {
        private double _value = 0;

        /// <summary>
        /// Function to check whether the value is valid
        /// </summary>
        private readonly Func<double, bool> _ValidatorMethod = (x) => true;

        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when the argument value has changed
        /// </summary>
        public event EventHandler? ValueChanged;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The list index for the linked value in <see cref="IElement.ElementArgs"/>
        /// </summary>
        public int Index { get; private set; } = -1;

        /// <summary>
        /// Name of the argument
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Value of the argument in user units
        /// </summary>
        public double UserValue { get => Units.ToUser(Value); set => Value = Units.FromUser(value); }

        /// <summary>
        /// Value of the argument in program units
        /// </summary>
        public double Value { get => _value; set { _value = value; ValidateValue(); ValueChanged?.Invoke(this, EventArgs.Empty); } }

        /// <summary>
        /// Unit type for the value
        /// </summary>
        public UnitVM Units { get; private set; } = new();

        /// <summary>
        /// True if the value is a valid entry. <see cref="_ValidatorMethod"/>
        /// </summary>
        public bool ValueValid { get; private set; } = true;

        // ---------------------- Methods ----------------------

        public ElementArgVM() { }

        /// <summary>
        /// Create the viewmodel
        /// </summary>
        /// <param name="index">The list index for the linked value in <see cref="IElement.ElementArgs"/></param>
        /// <param name="name">Name of the argument</param>
        /// <param name="units">Unit type for the value</param>
        /// <param name="initialValue">Starting value in program units</param>
        /// <param name="validatorMethod">Function to check whether the value is valid</param>
        public ElementArgVM(int index, string name, UnitVM? units = null, double initialValue = 0, Func<double, bool>? validatorMethod = null)
        {
            Index = index;
            _value = initialValue;
            Name = name;
            Units = units ?? new UnitVM();

            if (validatorMethod != null)
            {
                _ValidatorMethod = validatorMethod;
            }

            ValidateValue();
        }

        /// <summary>
        /// Checks whether the value is valid.
        /// </summary>
        private void ValidateValue()
        {
            ValueValid = _ValidatorMethod(Value);
        }

        /// <summary>
        /// Gets the full elements argument array from a list of ViewModels.
        /// </summary>
        /// <param name="arguments">The list of arguments for the element</param>
        /// <returns>An array compatible with <see cref="IElement.ElementArgs"/></returns>
        public static double[] GetArgumentsArray(IEnumerable<ElementArgVM> arguments)
        {
            var output = new double[arguments.Count()];

            foreach (var arg in arguments)
                output[arg.Index] = arg.Value;

            return output;
        }
    }
}
