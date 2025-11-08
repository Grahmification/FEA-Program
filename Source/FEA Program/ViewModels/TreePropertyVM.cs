using FEA_Program.ViewModels.Base;
using System.ComponentModel;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// A viewmodel for displaying a class property in a treeview
    /// </summary>
    internal class TreePropertyVM: ObservableObject
    {
        /// <summary>
        /// The parent who this VM's value is linked to
        /// </summary>
        private readonly INotifyPropertyChanged _parent;

        /// <summary>
        /// The name of the property in the parent that this value is linked to
        /// </summary>
        private readonly string _propertyName;

        /// <summary>
        /// Function to get the value from the parent
        /// </summary>
        private readonly Func<string> _valueGetter = () => "";

        /// <summary>
        /// The value to display in string format
        /// </summary>
        private string _value = "";

        // ------------------ Modifiable properties ----------------------

        /// <summary>
        /// The display name
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The display unit
        /// </summary>
        public UnitVM? Unit { get; set; } = null;

        /// <summary>
        /// True if the unit name should be displayed after the value
        /// </summary>
        public bool UnitsAfterValue { get; set; } = true;

        /// <summary>
        /// Whether the item is selected
        /// </summary>
        public bool Selected { get; set; } = false;

        // ------------------ View specific properties ----------------------

        /// <summary>
        /// The name text to display in the treeview
        /// </summary>
        public string DisplayName => !UnitsAfterValue && Unit != null ? $"{Name} [{Unit.UnitString}]" : Name;

        /// <summary>
        /// The value text to display in the treeview
        /// </summary>
        public string DisplayValue => UnitsAfterValue && Unit != null ? $"{_value} {Unit.UnitString}" : _value;

        // ------------------ Methods ----------------------

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="parent">The parent item who's value the VM is displaying</param>
        /// <param name="propertyName">The parent item's property name</param>
        /// <param name="valueGetter">Function to get the value from the value</param>
        public TreePropertyVM(INotifyPropertyChanged parent, string propertyName, Func<string> valueGetter)
        {
            _propertyName = propertyName;
            _valueGetter = valueGetter;

            _parent = parent;
            _parent.PropertyChanged += OnParentPropertyChanged;
            UpdateValue(); // Get the starting value
        }

        /// <summary>
        /// Called when one of the parent's properties has changed value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnParentPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not null)
            {
                if (e.PropertyName == _propertyName)
                {
                    UpdateValue();
                }
            }
        }

        /// <summary>
        /// Updates the value internally
        /// </summary>
        private void UpdateValue()
        {
            _value = _valueGetter();
            OnPropertyChanged(nameof(DisplayValue));
        }
    }
}
