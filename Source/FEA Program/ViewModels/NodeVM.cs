using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class NodeVM: ObservableObject
    {
        // ---------------------- Events ----------------------

        public event EventHandler? EditRequest;
        public event EventHandler? DeleteRequest;

        // ---------------------- Properties ----------------------
        public Node Model { get; private set; } = Node.DummyNode();

        /// <summary>
        /// Get the node coordinates in user units
        /// </summary>
        public double[] Coordinates_mm => Model.Coordinates.Select(coord => coord * 1000.0).ToArray();

        // ---------------------- Commands ----------------------
        public ICommand? EditCommand { get; }
        public ICommand? DeleteCommand { get; }

        public NodeVM() { }

        public NodeVM(Node model)
        {
            Model = model;
            EditCommand = new RelayCommand(() => EditRequest?.Invoke(this, EventArgs.Empty));
            DeleteCommand = new RelayCommand(() => DeleteRequest?.Invoke(this, EventArgs.Empty));
        }
    }
}
