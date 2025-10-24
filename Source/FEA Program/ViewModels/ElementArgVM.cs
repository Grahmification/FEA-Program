using FEA_Program.Models;
using FEA_Program.ViewModels.Base;


namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for parameters which can vary between different elements
    /// </summary>
    internal class ElementArgVM : ObservableObject
    {
        private double _value = 0;

        /// <summary>
        /// Function to check whether the value is valid
        /// </summary>
        private readonly Func<double, bool> _ValidatorMethod = (x) => true;

        // ---------------------- Events ----------------------

        public event EventHandler? ValueChanged;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The list index for the linked value in <see cref="IElement.ElementArgs"/>
        /// </summary>
        public int Index { get; private set; } = -1;
        public string Name { get; private set; } = "";
        public double UserValue { get => Units.ToUser(Value); set => Value = Units.FromUser(value); }
        public double Value { get => _value; set { _value = value; ValidateValue(); ValueChanged?.Invoke(this, EventArgs.Empty); } }
        public UnitVM Units { get; private set; } = new();
        public bool ValueValid { get; private set; } = true;


        public ElementArgVM() { }

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

        private void ValidateValue()
        {
            ValueValid = _ValidatorMethod(Value);
        }

        public static double[] GetArgumentsArray(IEnumerable<ElementArgVM> arguments)
        {
            var output = new double[arguments.Count()];

            foreach (var arg in arguments)
                output[arg.Index] = arg.Value;

            return output;
        }
    }
}
