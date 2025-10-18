using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    internal class ElementVM: ObservableObject
    {
        // ---------------------- Events ----------------------

        public event EventHandler? EditRequest;
        public event EventHandler? DeleteRequest;

        // ---------------------- Properties ----------------------
        public MaterialVM Material { get; private set; } = new();
        public NodeVM[] Nodes { get; private set; } = [];
        public IElement? Model { get; private set; } = null;
        public int[] NodeIds => Nodes.Select(n => n.Model.ID).ToArray();

        // ---------------------- Commands ----------------------
        public ICommand? EditCommand { get; }
        public ICommand? DeleteCommand { get; }

        public ElementVM() { }

        public ElementVM(IElement model, NodeVM[] nodes, MaterialVM material)
        {
            Model = model;
            Nodes = nodes;
            Material = material;
            EditCommand = new RelayCommand(() => EditRequest?.Invoke(this, EventArgs.Empty));
            DeleteCommand = new RelayCommand(() => DeleteRequest?.Invoke(this, EventArgs.Empty));
        }
    }
}
