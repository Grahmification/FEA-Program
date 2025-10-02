using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    /// <summary>
    /// Manages the lookup table connecting elements and nodes
    /// </summary>
    internal class Connectivity
    {
        /// <summary>
        /// The connectivity matrix.
        /// Key is the element ID.
        /// Array index is local node ID within the element
        /// Array value at index is global node ID.
        /// </summary>
        public Dictionary<int, int[]> ConnectivityMatrix { get; private set; } = [];


        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectivityMatrix">Optionally specify a starting matrix</param>
        public Connectivity(Dictionary<int, int[]>? connectivityMatrix = null)
        {
            ConnectivityMatrix = connectivityMatrix ?? [];
        }

        /// <summary>
        /// Gets global node IDs that a element is using
        /// </summary>
        /// <param name="elementID">The element ID</param>
        /// <returns>IDs of nodes contained in the element</returns>
        public int[] GetElementNodes(int elementID) => ConnectivityMatrix[elementID];

        /// <summary>
        /// Returns all of the element ID's attached to a global node ID
        /// </summary>
        /// <param name="nodeID">The global node ID</param>
        /// <returns>IDs of any elements using the node</returns>
        public List<int> GetNodeElements(int nodeID)
        {
            // Use LINQ to select the Key (Element ID) 
            // for every KeyValuePair where the Value (int[]) contains the nodeID.
            return ConnectivityMatrix
                .Where(kvp => kvp.Value.Contains(nodeID))
                .Select(kvp => kvp.Key)
                .OrderBy(key => key) // Sorts the element IDs for good measure (lowest element ID comes first)
                .ToList();
        }

        /// <summary>
        /// Adds a connection to the connectivity matrix.
        ///  NodeIDs need to be sorted in the correct local order for the element
        /// </summary>
        /// <param name="elementID">The element</param>
        /// <param name="nodeIDs">The node IDs in the element</param>
        public void AddConnection(int elementID, int[] nodeIDs)
        {
            ConnectivityMatrix.Add(elementID, nodeIDs);
        }

        /// <summary>
        /// Removes a connection from the connectivity matrix
        /// </summary>
        /// <param name="elementID">The element to remove</param>
        /// <returns></returns>
        public bool RemoveConnection(int elementID)
        {
            return ConnectivityMatrix.Remove(elementID);
        }

        /// <summary>
        /// Assembles the global stiffness matrix
        /// </summary>
        /// <param name="K_Matricies">Key = Element ID, Value = Element's K matrix</param>
        /// <param name="nodeDOFs">Key = NodeID, Value = Node DOFs for the given ID</param>
        /// <returns>The global stiffness matrix, sorted from the lowest node ID to highest [N1x, N1y, N2x, N2y,....]</returns>
        public SparseMatrix Assemble_K_Matrix(Dictionary<int, DenseMatrix> K_Matricies, Dictionary<int, int> nodeDOFs)
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
                var elementNodeIDs = GetElementNodes(elementID_and_K.Key);

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
