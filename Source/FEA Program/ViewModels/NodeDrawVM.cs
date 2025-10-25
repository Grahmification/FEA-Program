using FEA_Program.ViewModels.Base;
using SharpDX;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FEA_Program.ViewModels
{
    internal class NodeDrawVM: ObservableObject
    {
        public static Color4 SelectedColor = new(1, 1, 0, 1);
        public static Color4 DefaultNodeColor = new(0, 1, 0, 1);
        public static Color4 DefaultFixityColor = new(0, 0, 1, 1);

        // ---------------------- Properties ----------------------
        public NodeVM Node { get; private set; } = new();

        public Vector3 DrawPosition => ArrayToVector(GetScaledDisplacement_mm());
        public Vector3 Force => ArrayToVector(Node.Model.Force);
        public Color4 NodeColor => Node.Selected ? SelectedColor : DefaultNodeColor;
        public Color4 FixityColor => Node.Selected ? SelectedColor : DefaultFixityColor;
        public ObservableCollection<Vector3> ReactionForces { get; private set; } = [];

        /// <summary>
        /// How much to scale the result displacement by. 0 = show at original position, 1 = show at displaced position
        /// </summary>
        public double DisplacementScalingFactor { get; set; } = 0;

        public NodeDrawVM() { }

        public NodeDrawVM(NodeVM node)
        {
            Node = node;
            Node.PropertyChanged += OnNodePropertyChanged;
            PropertyChanged += OnThisPropertyChanged;
        }

        private void OnNodePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is NodeVM node)
            {
                if (e.PropertyName == nameof(NodeVM.UserCoordinates))
                {
                    OnPropertyChanged(nameof(DrawPosition));
                }
                else if (e.PropertyName == nameof(NodeVM.HasForce))
                {
                    OnPropertyChanged(nameof(Force));
                }
                else if (e.PropertyName == (nameof(NodeVM.Selected)))
                {
                    OnPropertyChanged(nameof(NodeColor));
                    OnPropertyChanged(nameof(FixityColor));
                }
            }
        }
        private void OnThisPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is NodeDrawVM)
            {
                if (e.PropertyName == nameof(DisplacementScalingFactor))
                {
                    OnPropertyChanged(nameof(DrawPosition));
                }
            }
        }
        private double[] GetScaledDisplacement_mm()
        {
            var scaleFactor = DisplacementScalingFactor;

            if (scaleFactor < 0)
                scaleFactor = 0;
            
            var output = new double[Node.Model.Dimension];

            for (int i = 0; i < Node.Model.Coordinates.Length; i++)
                output[i] = (Node.Model.Coordinates[i] + Node.Model.Displacement[i] * scaleFactor) * 1000.0; // convert to mm

            return output;
        }


        /// <summary>
        /// Convert an XYZ array to vector, with non-supplied values being zero
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Vector3 ArrayToVector(double[] array)
        {
            return new(
            (float)(array.Length > 0 ? array[0] : 0.0),
            (float)(array.Length > 1 ? array[1] : 0.0),
            (float)(array.Length > 2 ? array[2] : 0.0));
        }

        /// <summary>
        /// Applies reaction forces to each node in the specified list, normalizing the forces between the specified bounds
        /// </summary>
        /// <param name="nodes">The list of nodes to apply forces to</param>
        /// <param name="maxLength">The maxiumum force size to display for any component</param>
        /// <param name="minLength">The minimum force size to display for any component</param>
        public static void DrawReactionForces(IEnumerable<NodeDrawVM> nodes, double maxLength=2, double minLength=0.6)
        {
            if (nodes == null || !nodes.Any()) return;

            // Remove existing forces
            foreach (var node in nodes)
                node.ReactionForces.Clear();

            // Get all non-zero components
            List<double> forceComponents = nodes
                .SelectMany(node => node.Node.Model.ReactionForce)  // Get the ReactionForce array from every node and flatten it
                .Where(component => component != 0)  // Keep only the components that are not equal to zero.
                .ToList();

            if (forceComponents.Count == 0)
                return;

            // Compute the global max/min
            double minMag = forceComponents.Where(v => v != 0).Select(Math.Abs).Min();
            double maxMag = forceComponents.Select(Math.Abs).Max();

            // Add forces to each node
            foreach (var node in nodes)
            {
                var reaction = node.Node.Model.ReactionForce;
       
                for (int i = 0; i < reaction.Length; i++)
                {
                    // If a force component is non-zero, add it's normalized value
                    if (reaction[i] != 0)
                    {
                        double sign = Math.Sign(reaction[i]);
                        double mag = Math.Abs(reaction[i]);

                        // Linear interpolation between [minLength and MaxLength]
                        double normalizedMag = minLength + (mag - minMag) / (maxMag - minMag) * (maxLength - minLength);

                        var vector = new Vector3(0);
                        vector[i] = (float)(sign * normalizedMag);
                        node.ReactionForces.Add(vector);
                    }
                }
            }
        }
    }
}
