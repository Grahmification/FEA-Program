using HelixToolkit.Wpf.SharpDX.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Manages item selection in the program
    /// </summary>
    internal class SelectionVM: ObservableObject
    {
        private bool _AllowMultiSelect = false;

        /// <summary>
        /// True if multiple items can be selected simultaneously
        /// </summary>
        public bool AllowMultiSelect { 
            get => _AllowMultiSelect; 
            set { 
                _AllowMultiSelect = value; 

                // Clear excess list items
                if (!value && SelectedItems.Count > 1) 
                {
                    // Start from the second-to-last item (Count - 2) and go down to index 0
                    for (int i = SelectedItems.Count - 2; i >= 0; i--)
                    {
                        SelectedItems[i].Selected = false;  // This will remove it from the list
                    }
                } 
            } 
        }

        /// <summary>
        /// Items which are actively selected
        /// </summary>
        public ObservableCollection<ISelectable> SelectedItems { get; private set; } = [];

        /// <summary>
        /// All items which are being monitored for selection
        /// </summary>
        public ObservableCollection<ISelectable> AllItems { get; private set; } = [];

        // ---------------- Public Methods ------------------------

        /// <summary>
        /// Adds an item to the selection manager
        /// </summary>
        /// <param name="item">The item to be managed</param>
        public void AddItem(ISelectable item)
        {
            AllItems.Add(item);
            item.PropertyChanged += OnItemSelectionChanged;
        }

        /// <summary>
        /// Removes an item to the selection manager
        /// </summary>
        /// <param name="item">The item being managed</param>
        public void RemoveItem(ISelectable item)
        {
            item.Selected = false; // This will remove it from the selected list
            item.PropertyChanged -= OnItemSelectionChanged;
            AllItems.Remove(item);
            
            
        }

        /// <summary>
        /// Deselects all items being manages
        /// </summary>
        public void DeselectAll()
        {
            foreach (var item in AllItems)
            {
                item.Selected = false;
            }
        }

        // ---------------- Event Methods ------------------------

        /// <summary>
        /// Called when the <see cref="ISelectable.Selected"/> property changes for an item being managed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemSelectionChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ISelectable item && e.PropertyName == nameof(ISelectable.Selected))
            {
                if (item.Selected)
                {
                    if (!AllowMultiSelect)
                    {
                        for (int i = 0; i < SelectedItems.Count; i++)
                            SelectedItems[i].Selected = false;  // This will remove it from the list

                        SelectedItems.Clear();  // Good measure
                    }

                    SelectedItems.Add(item);
                }
                else
                {
                    SelectedItems.Remove(item);
                }
            }
        }
    }
}
