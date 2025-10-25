using FEA_Program.ViewModels.Base;
using SharpDX;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace FEA_Program.ViewModels
{
    internal class NodeDrawVM: ObservableObject
    {
        public static Color SelectedColor = Colors.Yellow;
        public static Color DefaultNodeColor = Colors.LightGreen;
        public static Color DefaultFixityColor = Colors.Blue;

        // ---------------------- Properties ----------------------
        public NodeVM Node { get; private set; } = new();

        public Vector3 DrawPosition => ArrayToVector(GetScaledDisplacement_mm());
        public Vector3 Force => ArrayToVector(Node.Model.Force);
        public double ForceLength => ScaleForceMagnitude(Node.Model.ForceMagnitude);

        public Color NodeColor => Node.Selected ? SelectedColor : DefaultNodeColor;
        public Color FixityColor => Node.Selected ? SelectedColor : DefaultFixityColor;
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
                    OnPropertyChanged(nameof(ForceLength));
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
        /// Scales an input value (0 to 100) to a linear output range.
        /// </summary>
        /// <param name="inputValue">The input force value</param>
        /// <returns>The scaled value between the specified limits</returns>
        public static double ScaleForceMagnitude(double inputValue)
        {
            if (inputValue == 0)
                return 0; // Hide the force if the value is zero
            
            // Define the input and output ranges (These are fixed constants)
            const double inputMin = 0.0;
            const double inputMax = 100.0;

            const double outputMin = 1;
            const double outputMax = 3;

            // Ensure the input value is clamped within the expected range (0 to 100)
            // This prevents the output from going outside the 0.5 to 2.0 range if invalid input is provided.
            double clampedValue = Math.Clamp(inputValue, inputMin, inputMax);

            // Calculate the ratio of the input value within its range (0.0 to 1.0)
            double ratio = (clampedValue - inputMin) / (inputMax - inputMin);

            // Calculate the corresponding position in the output range
            return outputMin + ratio * (outputMax - outputMin);
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
