using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    /// <summary>
    /// Extra functionality for working with nodes
    /// </summary>
    internal class NodeExtensions
    {
        /// <summary>
        /// Gets the global force vector, sorted from smallest to largest node ID
        /// </summary>
        public static DenseVector F_Matrix(List<Node> nodes)
        {
            // Sort nodes from smallest to largest ID
            nodes = [.. nodes.OrderBy(node => node.ID)];
            return BuildVector(nodes, n => n.Force);
        }

        /// <summary>
        /// Gets the global fixity vector, sorted from smallest to largest node ID
        /// </summary>
        public static DenseVector Q_Matrix(List<Node> nodes)
        {
            // Sort nodes from smallest to largest ID
            nodes = [.. nodes.OrderBy(node => node.ID)];
            return BuildVector(nodes, n => Array.ConvertAll(n.Fixity, x => (double)x));
        }

        /// <summary>
        /// Sets the solution for all nodes in the list
        /// </summary>
        /// <param name="nodes">The list of nodes to set the FEA solution for</param>
        /// <param name="Q">The global displacement vector</param>
        /// <param name="R">The global reaction force vector</param>
        public static void ApplySolution(List<Node> nodes, DenseVector Q, DenseVector R)
        {
            // Sort nodes from smallest to largest ID - this solution will always be in this form
            nodes = [.. nodes.OrderBy(node => node.ID)];

            int index = 0;

            foreach (Node node in nodes)
            {
                var nodeDisplacements = new double[node.Dimension];
                var nodeReactions = new double[node.Dimension];

                for (int i = 0; i < node.Dimension; i++)
                {
                    nodeDisplacements[i] = Q[index];
                    nodeReactions[i] = R[index];
                    index++;
                }

                node.Solve(nodeDisplacements, nodeReactions);
            }
        }


        /// <summary>
        /// Builds a sequenctial vector from a collection of nodes
        /// </summary>
        /// <param name="nodes">The nodes</param>
        /// <param name="selector">The node property to get</param>
        /// <returns>The vector with each node sequentially appended</returns>
        public static DenseVector BuildVector(List<Node> nodes, Func<Node, double[]> selector)
        {
            int outputSize = nodes.Sum(node => node.Dimension);
            var output = new DenseVector(outputSize);
            int currentRow = 0;

            foreach (var node in nodes)
            {
                var values = selector(node);
                for (int i = 0; i < node.Dimension; i++)
                {
                    output[currentRow++] = values[i];
                }
            }

            return output;
        }

    }
}
