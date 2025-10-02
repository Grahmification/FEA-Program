using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    /// <summary>
    /// Additional functionality for working with elements
    /// </summary>
    internal class ElementExtensions
    {
        /// <summary>
        /// Gets K matricies a list of elements
        /// </summary>
        /// <param name="connectionMatrix">Global connectivity matrix [Element ID, Node IDs]</param>
        /// <param name="nodeCoordinates">Node coordinates [Node ID, coordinates]</param>
        /// <returns>[Element ID, Element K Matrix]</returns>
        public static Dictionary<int, DenseMatrix> Get_K_Matricies(List<IElement> elements, Dictionary<int, int[]> connectionMatrix, Dictionary<int, double[]> nodeCoordinates)
        {
            // Sort from smallest to largest IDs. Not required, but good practice
            var output = new SortedDictionary<int, DenseMatrix>();

            foreach (var element in elements) // iterate through each element
            {
                // Get the coordinates of each node in the element
                // Use LINQ to project the list of NodeIDs into a list of coordinate arrays.
                List<double[]> elementNodeCoords = connectionMatrix[element.ID]
                    .Select(nodeId => nodeCoordinates[nodeId])
                    .ToList();

                output.Add(element.ID, element.K_Matrix(elementNodeCoords));
            }

            return output.ToDictionary();
        }

    }
}
