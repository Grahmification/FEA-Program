using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    /// <summary>
    /// Manages the lookup table connecting elements and nodes
    /// </summary>
    internal class Connectivity
    {
        private Dictionary<int, List<int>> _ConnectMatrix = []; // dict key is global element ID, list index is local node ID, list value at index is global node ID

        public Dictionary<int, List<int>> ConnectMatrix => _ConnectMatrix;

        /// <summary>
        /// Gets global node IDs that a element is using
        /// </summary>
        /// <param name="elementID">The element ID</param>
        /// <returns>IDs of nodes contained in the element</returns>
        public List<int> ElementNodes(int elementID) => _ConnectMatrix[elementID];

        /// <summary>
        /// Returns all of the element ID's attached to a global node ID
        /// </summary>
        /// <param name="nodeID">The global node ID</param>
        /// <returns>IDs of any elements using the node</returns>
        public List<int> NodeElements(int nodeID)
        {
            var output = new List<int>();

            foreach (KeyValuePair<int, List<int>> KVP in _ConnectMatrix)
            {
                if (KVP.Value.Contains(nodeID)) // check if the nodeID is used in the element
                {
                    output.Add(KVP.Key); // if so add the element ID to the output
                }
            }

            output.Sort(); // sort the output for good measure (lowest element ID comes first)
            return output;
        }

        public void AddConnection(int elementID, List<int> nodeIDs)
        {
            _ConnectMatrix.Add(elementID, nodeIDs);
        } // nodeIDs need to be sorted in the correct local order for the element
        public void RemoveConnection(int elementID)
        {
            _ConnectMatrix.Remove(elementID);
        }

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="connectivityMatrix"></param>
        public void ImportMatrix(Dictionary<int, List<int>> connectivityMatrix)
        {
            _ConnectMatrix = connectivityMatrix;
        }

        /// <summary>
        /// Assembles the global stiffness matrix
        /// </summary>
        /// <param name="K_Matricies">Key = Element ID, Value = Element's K matrix</param>
        /// <param name="nodeDOFs">Key = NodeID, Value = Node DOFs for the given ID</param>
        /// <returns>The global stiffness matrix, sorted from the lowest node ID to highest [N1x, N1y, N2x, N2y,....]</returns>
        public SparseMatrix Assemble_K_Mtx(Dictionary<int, DenseMatrix> K_Matricies, Dictionary<int, int> nodeDOFs)
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
                var elementNodeIDs = ElementNodes(elementID_and_K.Key);

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

    } // need to call functions in here from element/node events upon creation/deletion
}
