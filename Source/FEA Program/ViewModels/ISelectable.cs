using System.ComponentModel;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Generic definition for an object which can be selected
    /// </summary>
    internal interface ISelectable: INotifyPropertyChanged
    {
        /// <summary>
        /// Whether the object is selected
        /// </summary>
        public bool Selected { get; set; }
    }
}
