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

        private string _value = "";

        // ------------------ Modifiable properties ----------------------
        public string Name { get; set; } = "";
        public UnitVM? Unit { get; set; } = null;
        public bool UnitsAfterValue { get; set; } = true;
        public bool Selected { get; set; } = false;

        // ------------------ View specific properties ----------------------
        public string DisplayName => !UnitsAfterValue && Unit != null ? $"{Name} [{Unit.UnitString}]" : Name;
        public string DisplayValue => UnitsAfterValue && Unit != null ? $"{_value} {Unit.UnitString}" : _value;

        public TreePropertyVM(INotifyPropertyChanged parent, string propertyName, Func<string> valueGetter)
        {
            _propertyName = propertyName;
            _valueGetter = valueGetter;

            _parent = parent;
            _parent.PropertyChanged += OnParentPropertyChanged;
            UpdateValue(); // Get the starting value
        }

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
        private void UpdateValue()
        {
            _value = _valueGetter();
            OnPropertyChanged(nameof(DisplayValue));
        }
    }
}
