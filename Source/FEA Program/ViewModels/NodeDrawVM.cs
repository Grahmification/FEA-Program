using FEA_Program.ViewModels.Base;
using SharpDX;
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
                if (e.PropertyName == nameof(NodeVM.Coordinates_mm))
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
    }
}
