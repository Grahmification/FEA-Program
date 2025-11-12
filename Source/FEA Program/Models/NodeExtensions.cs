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
            return BuildVector(nodes, n => n.ForceVector);
        }

        /// <summary>
        /// Gets the global fixity vector, sorted from smallest to largest node ID
        /// </summary>
        public static DenseVector Q_Matrix(List<Node> nodes)
        {
            // Sort nodes from smallest to largest ID
            nodes = [.. nodes.OrderBy(node => node.ID)];
            return BuildVector(nodes, n => Array.ConvertAll(n.FixityVector, x => (double)x));
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
                var nodeDisplacements = new double[node.DOFs];
                var nodeReactions = new double[node.DOFs];

                for (int i = 0; i < node.DOFs; i++)
                {
                    nodeDisplacements[i] = Q[index];
                    nodeReactions[i] = R[index];
                    index++;
                }

                node.Solve(nodeDisplacements, nodeReactions);
            }
        }


        /// <summary>
        /// Builds a sequential vector from a collection of nodes by concatenating the double[] 
        /// returned by the selector for each node.
        /// </summary>
        /// <typeparam name="TNode">The type of the node, must implement INode.</typeparam>
        /// <param name="nodes">The collection of nodes.</param>
        /// <param name="selector">The node property (double[]) to get.</param>
        /// <returns>A DenseVector with all selected values sequentially appended.</returns>
        public static DenseVector BuildVector<TNode>(IEnumerable<TNode> nodes, Func<TNode, double[]> selector) where TNode : INode
        {
            // Use LINQ's SelectMany to flatten the arrays returned by the selector 
            // into a single sequence of double values.
            double[] vectorData = nodes
                .SelectMany(selector) // Applies selector to each node and flattens the results
                .ToArray();           // Converts the resulting sequence into a single array

            // Create the DenseVector from the combined array
            return new DenseVector(vectorData);
        }


        /// <summary>
        /// Returns true if a node already exists at the given location
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool NodeExistsAtLocation(double[] position, IEnumerable<Node> existingNodes)
        {
            foreach (Node node in existingNodes)
            {
                // This should work regardless of dimension
                if (node.Position.Take((int)node.Dimension).SequenceEqual(position.Take((int)node.Dimension)))
                    return true;
            }

            return false;
        }

    }
}
