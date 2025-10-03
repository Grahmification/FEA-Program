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
        /// <param name="elements">The list of elements</param>
        /// <returns>[Element ID, Element K Matrix]</returns>
        public static Dictionary<int, DenseMatrix> Get_K_Matricies(List<IElement> elements)
        {
            // Sort from smallest to largest IDs. Not required, but good practice
            var output = new SortedDictionary<int, DenseMatrix>();

            foreach (var element in elements) // iterate through each element
            {
                output.Add(element.ID, element.K_Matrix());
            }

            return output.ToDictionary();
        }

    }
}
