using FEA_Program.Models;
using FEA_Program.Utils;
using FEA_Program.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// Viewmodel for an element in the FEA problem
    /// </summary>
    internal class ElementVM: ObservableObject, ISelectable
    {
        // ---------------------- Events ----------------------

        /// <summary>
        /// Fires when the element requests that it be edited
        /// </summary>
        public event EventHandler? EditRequest;

        /// <summary>
        /// Fires when the element requests that it be deleted
        /// </summary>
        public event EventHandler? DeleteRequest;

        // ---------------------- Properties ----------------------

        /// <summary>
        /// The element's material
        /// </summary>
        public MaterialVM Material { get; private set; } = new();

        /// <summary>
        /// The nodes contained in the element
        /// </summary>
        public NodeVM[] Nodes { get; private set; } = [];
        
        /// <summary>
        /// The element's underlying model
        /// </summary>
        public IElement? Model { get; private set; } = null;

        /// <summary>
        /// The list of IDs for nodes contained in the element
        /// </summary>
        public int[] NodeIds => Nodes.Select(n => n.Model.ID).ToArray();

        /// <summary>
        /// The arguments that depend on the type of element
        /// </summary>
        public ElementArgVM[] Arguments { get; private set; } = [];

        /// <summary>
        /// Properties for treeview display
        /// </summary>
        public ObservableCollection<TreePropertyVM> Properties { get; private set; } = [];

        /// <summary>
        /// Properties for treeview resultsdisplay
        /// </summary>
        public ObservableCollection<TreePropertyVM> ResultProperties { get; private set; } = [];

        /// <summary>
        /// Whether the item is selected
        /// </summary>
        public bool Selected { get; set; } = false;

        /// <summary>
        /// The element's max stress in user units
        /// </summary>
        public double MaxStress => App.Units.Stress.ToUser(Model?.MaxStress ?? 0);

        /// <summary>
        /// The element's yield strength safety factor
        /// </summary>
        public double SafetyFactorYield => Model?.SafetyFactorYield ?? 0;

        /// <summary>
        /// The element's ultimate strength safety factor
        /// </summary>
        public double SafetyFactorUltimate => Model?.SafetyFactorUltimate ?? 0;

        /// <summary>
        /// True if the element stress is a valid number
        /// </summary>
        public bool StressIsValid => !double.IsNaN(MaxStress) && !double.IsInfinity(MaxStress);

        // ---------------------- Commands ----------------------
        
        /// <summary>
        /// Relay command for editing the element
        /// </summary>
        public ICommand? EditCommand { get; }

        /// <summary>
        /// Relay command for deleting the element
        /// </summary>
        public ICommand? DeleteCommand { get; }

        // ---------------------- Methods ----------------------

        /// <summary>
        /// Default constructor
        /// </summary>
        public ElementVM() { }

        /// <summary>
        /// Primary constructor
        /// </summary>
        /// <param name="model">The element model</param>
        /// <param name="nodes">All nodes contained in the element</param>
        /// <param name="material">The element's material</param>
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

            // Create results tree properties
            ResultProperties.Add(new TreePropertyVM(this, nameof(MaxStress), () => MaxStress.ToString("F2")) { Name = "Stress", Unit = App.Units.Stress });
            ResultProperties.Add(new TreePropertyVM(this, nameof(SafetyFactorYield), () => SafetyFactorYield.ToString("F1")) { Name = "Safety Factor, Yield"});
            ResultProperties.Add(new TreePropertyVM(this, nameof(SafetyFactorUltimate), () => SafetyFactorUltimate.ToString("F1")) { Name = "Safety Factor, Ultimate" });

            foreach (var prop in ResultProperties)
                prop.PropertyChanged += OnTreePropertyPropertyChanged;
        }

        // ---------------------- Event Handlers ----------------------
        
        /// <summary>
        /// Called when one of the element argument's value has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnArgumentValueUpdated(object? sender, EventArgs e)
        {
            if (sender is ElementArgVM vm && Model is not null)
            {
                // Propagate changes to the model
                Model.ElementArgs[vm.Index] = vm.Value;
            }
        }

        /// <summary>
        /// Called when a property in <see cref="ResultProperties"/> or <see cref="Properties"/> changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Called then a property in the model has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is IElement)
            {
                if (e.PropertyName == nameof(IElement.SolutionValid))
                {
                    OnPropertyChanged(nameof(SafetyFactorYield));
                    OnPropertyChanged(nameof(SafetyFactorUltimate));
                    OnPropertyChanged(nameof(MaxStress));
                    OnPropertyChanged(nameof(StressIsValid));
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
            ElementTypes.TrussLinear => new()
            {
                new ElementArgVM(0, "Area", App.Units.Area, validatorMethod: (x) => x > 0) {UserValue = 1}
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
                ElementTypes.TrussLinear => new ElementTrussLinear(1, 0, [Node.DummyNode(), Node.DummyNode()], Models.Material.DummyMaterial()).NumOfNodes,
                _ => 0,
            };
        }
    }
}
