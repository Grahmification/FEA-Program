using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    /// <summary>
    /// Extra functionality for working with elements
    /// </summary>
    internal class ElementExtensions
    {
        /// <summary>
        /// Assembles the matrix defining links between elements and nodes
        /// </summary>
        /// <param name="elements">The list of elements to assemble for</param>
        /// <returns>The connectivity matrix. Key is the element ID. Array index is local node ID within the element. Array value at index is global node ID.</returns>
        public static Dictionary<int, int[]> Get_Connectivity_Matrix(List<IElement> elements)
        {
            return elements.ToDictionary(
                element => element.ID,                                  // Key Selector: Use the element's ID
                element => element.Nodes.Select(n => n.ID).ToArray()    // Value Selector: Use LINQ to convert the list of nodes to an array of node IDs
            );
        }

        /// <summary>
        /// Assembles the global stiffness matrix
        /// </summary>
        /// <param name="K_Matricies">Key = Element ID, Value = Element's K matrix</param>
        /// <param name="nodeDOFs">Key = NodeID, Value = Node DOFs for the given ID</param>
        /// <param name="connectivity">Key = Element ID, Value = Node IDs linked to the element</param>
        /// <returns>The global stiffness matrix, sorted from the lowest node ID to highest [N1x, N1y, N2x, N2y,....]</returns>
        public static SparseMatrix Assemble_K_Matrix(Dictionary<int, DenseMatrix> K_Matricies, Dictionary<int, int> nodeDOFs, Dictionary<int, int[]> connectivity)
        {
            // ---------------------- Get Total Size of the Problem --------------------------

            int problemSize = nodeDOFs.Values.Sum(); // All the DOFs added together

            // ----------------------- Setup Output Matrix -----------------------------------

            var output = new SparseMatrix(problemSize, problemSize);

            // ------------------------Iterate Through Each Node ID and allocate regions of large matrix --------------------------

            var nodeIDs = nodeDOFs.Keys.ToList();
            nodeIDs.Sort(); // Smallest ID comes first

            var nodeMatrixIndicies = new Dictionary<int, int[]>(); // determines where each displacement will go on the assembled matrix, sorted by node matrix
            int indexCounter = 0; // to count how many places have been used up in the matrix

            foreach (int ID in nodeIDs)
            {
                int DOF = nodeDOFs[ID]; // get the number of DOFs for the node
                var allocatedIndicies = new int[DOF]; // create array to hold allocated indicies

                for (int i = 0; i < DOF; i++) // allocate a value for each DOF incrementally based on the number of DOFs
                {
                    allocatedIndicies[i] = indexCounter;
                    indexCounter += 1;
                }

                nodeMatrixIndicies.Add(ID, allocatedIndicies);
            }

            // ------------------------Iterate Through Each Element --------------------------

            foreach (KeyValuePair<int, DenseMatrix> elementID_and_K in K_Matricies)
            {
                // 1. get node ID's - assume in correct order
                var elementNodeIDs = connectivity[elementID_and_K.Key];

                // 2. Allocate Regions of the K Matrix for Each Element
                var nodeKmtxIndicies = new Dictionary<int, int[]>(); // holds which regions of the k matrix are claimed by each node
                indexCounter = 0; // to count how many places have been used up in the matrix

                foreach (int ID in elementNodeIDs)
                {
                    int DOF = nodeDOFs[ID]; // get the number of DOFs for the node
                    var allocatedIndicies = new int[DOF]; // create array to hold allocated indicies

                    for (int i = 0; i < DOF; i++) // allocate a value for each DOF incrementally based on the number of DOFs
                    {
                        allocatedIndicies[i] = indexCounter;
                        indexCounter += 1;
                    }

                    nodeKmtxIndicies.Add(ID, allocatedIndicies);
                }

                // 3. Move the value range for each node from the local K matrix to the global
                foreach (int i in elementNodeIDs)
                {
                    foreach (int j in elementNodeIDs)
                    {
                        for (int row = 0; row < nodeDOFs[i]; row++)
                        {
                            for (int col = 0; col < nodeDOFs[j]; col++)
                            {

                                int[] assembled_K_nodeRegion_i = nodeMatrixIndicies[i];
                                int[] assembled_K_nodeRegion_j = nodeMatrixIndicies[j];

                                int[] local_K_nodeRegion_i = nodeKmtxIndicies[i];
                                int[] local_K_nodeRegion_j = nodeKmtxIndicies[j];

                                output[assembled_K_nodeRegion_i[row], assembled_K_nodeRegion_j[col]] += elementID_and_K.Value[local_K_nodeRegion_i[row], local_K_nodeRegion_j[col]];
                            }
                        }
                    }
                }
            }

            return output;
        }

    }
}
