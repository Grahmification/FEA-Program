using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FEA_Program.ViewModels.Base
{
    /// <summary>
    /// An ObservableCollection that is linked to another parent collection, and will change when it changes
    /// </summary>
    /// <typeparam name="T">The type in this collection</typeparam>
    /// <typeparam name="PT">The type in the parent collection</typeparam>
    internal class DependentObservableCollection<T, PT>
    {
        private readonly ObservableCollection<PT> _parentCollection = [];
        private readonly Func<PT, T> _addFunction;
        private readonly Func<PT, T> _removeFunction;
        private readonly Action<PT> _replaceFunction = (e) => { return; };


        public ObservableCollection<T> Collection { get; private set; } = [];

        public DependentObservableCollection(ObservableCollection<PT> parentCollection, Func<PT, T> addFunction, Func<PT, T> removeFunction, Action<PT>? replaceFunction = null)
        {
            _parentCollection = parentCollection;
            _addFunction = addFunction;
            _removeFunction = removeFunction;

            if (replaceFunction != null)
                _replaceFunction = replaceFunction;

            // Add all existing items
            foreach(var item in parentCollection)
            {
                Collection.Add(_addFunction(item));
            }

            _parentCollection.CollectionChanged += OnParentCollectionChanged;
        }

        private void OnParentCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // The 'Action' property tells you what happened to the collection
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // An item was added.
                    // e.NewItems holds the item(s) that were added.
                    // e.NewStartingIndex is the index where the new item(s) start.
                    if (e.NewItems != null)
                    {
                        foreach(var item in e.NewItems)
                        {
                            Collection.Add(_addFunction((PT)item));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    // An item was removed.
                    // e.OldItems holds the item(s) that were removed.
                    // e.OldStartingIndex is the index where the removed item(s) were located.
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            Collection.Remove(_removeFunction((PT)item));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // An item was replaced.
                    // e.NewItems holds the new item(s).
                    // e.OldItems holds the item(s) that were replaced.
                    if (e.NewItems != null && e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            _replaceFunction((PT)item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Collection.Clear();
                    break;
            }
        }
    }
}
