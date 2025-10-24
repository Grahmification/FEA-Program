using FEA_Program.Models;
using FEA_Program.ViewModels.Base;
using System.ComponentModel;
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
        public ElementArgVM[] Arguments { get; private set; } = [];
        public bool Selected { get; set; } = false;

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

            Model.PropertyChanged += OnModelPropertyChanged;

            // Setup Arguments
            Arguments = [.. ElementArgs(model.ElementType)];

            foreach(var arg in Arguments)
            {
                arg.ValueChanged += OnArgumentValueUpdated;
            }

            // Arguments should only be changed from these VMs, so synchronization only needs to be called at startup
            SynchronizeArguments();
        }

        // ---------------------- Event Handlers ----------------------
        private void OnArgumentValueUpdated(object? sender, EventArgs e)
        {
            if (sender is ElementArgVM vm && Model is not null)
            {
                Model.ElementArgs[vm.Index] = vm.Value;
            }
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is IElement)
            {
                if (e.PropertyName == nameof(IElement.SolutionValid))
                {
                    OnPropertyChanged(nameof(IElement.MaxStress));
                }
            }
        }

        // ---------------------- Private Helpers ----------------------

        /// <summary>
        /// Synchronizes Argument values with the model
        /// </summary>
        private void SynchronizeArguments()
        {
            foreach (var arg in Arguments)
            {
                if (Model is not null)
                    arg.Value = Model.ElementArgs[arg.Index];
            }
        }

        // ---------------------- Static Methods ----------------------

        /// <summary>
        /// Get element arguments associated with various element types
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public static List<ElementArgVM> ElementArgs(ElementTypes elementType) => elementType switch
        {
            // Case for ElementBarLinear
            ElementTypes.BarLinear => new()
            {
                new ElementArgVM(0, "Area", App.Units.Area, validatorMethod: (x) => x > 0)
            },
            // Default case: return an empty list
            _ => []
        };

        /// <summary>
        /// Get the number of nodes for a given element type
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public static int NumOfNodes(ElementTypes elementType)
        {
            return elementType switch
            {
                ElementTypes.BarLinear => new ElementBarLinear(1, 0, [Node.DummyNode(), Node.DummyNode()], Models.Material.DummyMaterial()).NumOfNodes,
                _ => 0,
            };
        }
    }
}
