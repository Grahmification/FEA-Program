using System.ComponentModel;

namespace FEA_Program.ViewModels.Base
{
    /// <summary>
    /// A property that is computed from another property
    /// </summary>
    internal class DependentProperty<T> : ObservableObject
    {
        private record struct ParentProperty(INotifyPropertyChanged Parent, string PropertyName);

        private readonly List<ParentProperty> _dependencies = [];

        private readonly Func<T> _getter = () => throw new NotImplementedException();
        private readonly Action<T> _setter = (_) => throw new NotImplementedException();

        public T Value
        {
            get => _getter();
            set => _setter(value);
        }

        public DependentProperty(INotifyPropertyChanged parent, string propertyName, Func<T> getter, Action<T>? setter = null)
        {
            _getter = getter;

            if(setter != null)
                _setter = setter;

            AddDependency(parent, propertyName);

        }

        public DependentProperty(Func<T> getter, Action<T>? setter = null)
        {
            _getter = getter;

            if (setter != null)
                _setter = setter;
        }

        public void AddDependency(INotifyPropertyChanged parent, string propertyName)
        {
            _dependencies.Add(new ParentProperty(parent, propertyName));
            parent.PropertyChanged += OnParentPropertyChanged;
        }

        /// <summary>
        /// Fires when a property of the parent items has been changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnParentPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not INotifyPropertyChanged changedParent)
                return;

            // If the parent and property name match, the value has changed
            if (_dependencies.Any(d => d.Parent == changedParent && d.PropertyName == e.PropertyName))
            {
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
