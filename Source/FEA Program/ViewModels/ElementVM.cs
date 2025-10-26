using FEA_Program.Models;
using FEA_Program.UI;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// Properties for treeview display
        /// </summary>
        public ObservableCollection<TreePropertyVM> Properties { get; private set; } = [];

        public bool Selected { get; set; } = false;
        public double MaxStress => App.Units.Stress.ToUser(Model?.MaxStress ?? 0);
        public double SafetyFactorYield => Model?.SafetyFactorYield ?? 0;
        public double SafetyFactorUltimate => Model?.SafetyFactorUltimate ?? 0;

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

            // Setup tree properties
            Properties.Add(new TreePropertyVM(Model, nameof(IElement.ElementType), () => Attributes.GetDescription(Model.ElementType)) { Name = "Type" });
            Properties.Add(new TreePropertyVM(this, nameof(Material), () => Material.Model.Name) { Name = "Material" });
            Properties.Add(new TreePropertyVM(this, nameof(Nodes), () => string.Join(", ", Nodes.Select(n => n.Model.ID.ToString()))) { Name = "Nodes" });

            // Create a tree item for each argument
            foreach (var arg in Arguments)
            {
                Properties.Add(new TreePropertyVM(arg, nameof(ElementArgVM.Value), arg.UserValue.ToString) { Name = arg.Name, Unit = arg.Units });
            }

            foreach(var prop in Properties)
                prop.PropertyChanged += OnTreePropertyPropertyChanged;
        }

        // ---------------------- Event Handlers ----------------------
        private void OnArgumentValueUpdated(object? sender, EventArgs e)
        {
            if (sender is ElementArgVM vm && Model is not null)
            {
                Model.ElementArgs[vm.Index] = vm.Value;
            }
        }
        private void OnTreePropertyPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is TreePropertyVM vm)
            {
                if (e.PropertyName == nameof(TreePropertyVM.Selected))
                {
                    // Propagate selections to this VM
                    Selected = vm.Selected;
                }
            }
        }

        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is IElement)
            {
                if (e.PropertyName == nameof(IElement.SolutionValid))
                {
                    OnPropertyChanged(nameof(SafetyFactorYield));
                    OnPropertyChanged(nameof(SafetyFactorUltimate));
                    OnPropertyChanged(nameof(MaxStress));
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
