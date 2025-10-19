using FEA_Program.ViewModels.Base;
using SharpDX;
using System.ComponentModel;

namespace FEA_Program.ViewModels
{
    internal class NodeDrawVM: ObservableObject
    {
        // ---------------------- Properties ----------------------
        public NodeVM Node { get; private set; } = new();

        public Vector3 DrawPosition => ArrayToVector(Node.Coordinates_mm);
        public Vector3 Force => ArrayToVector(Node.Model.Force);

        public NodeDrawVM() { }

        public NodeDrawVM(NodeVM node)
        {
            Node = node;
            Node.PropertyChanged += OnNodePropertyChanged;
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
            }
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
