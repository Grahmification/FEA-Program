using FEA_Program.Forms;
using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal class Connectivity
    {
        private readonly Dictionary<int, List<int>> _ConnectMatrix = []; // dict key is global element ID, list index is local node ID, list value at index is global node ID

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


        private void SortNodeLocalIDs(Dictionary<int, Dictionary<int, double[]>> NodeCoords)
        {

        }
        
        /// <summary>
        /// Assembles the global stiffness matrix
        /// </summary>
        /// <param name="K_Matricies">Key = Element ID, Value = Element's K matrix</param>
        /// <param name="nodeDOFs">Key = NodeID, Value = Node DOFs for the given ID</param>
        /// <returns>The global stiffness matrix</returns>
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
        
        /// <summary>
        /// Solves the FEA Problem
        /// </summary>
        /// <param name="K">The global stiffness matrix</param>
        /// <param name="F">The global force vector</param>
        /// <param name="Q">The global fixity vector</param>
        /// <returns>The global [Displacement Vector, Reaction Force Vector]</returns>
        public static DenseVector[] Solve(SparseMatrix K, DenseVector F, DenseVector Q, bool debugging = false)
        {
            if (debugging)  // Display matricies for the user
            {
                var form = new MatrixViewerForm([K, SparseMatrix.OfColumnVectors(Q), SparseMatrix.OfColumnVectors(F)], 
                    ["K Matrix", "Q Matrix", "F Matrix"]);
            }
            
            // Convert to matrix so we can use .RemoveRow
            DenseMatrix Fm = (DenseMatrix)F.ToColumnMatrix();

            int problemSize = Q.Count;

            // get indicies of each fixed displacement
            // If Q[i] == 1 then fixed
            var fixedIndicies = Enumerable.Range(0, problemSize).Where(i => Q[i] == 1).ToList();
            fixedIndicies.Reverse(); // need to remove rows with highest index first

            // first remove columns - they will be multiplied by 0 anyway - rows are still needed for reaction forces
            foreach (int index in fixedIndicies) 
                K = (SparseMatrix)K.RemoveColumn(index);

            // Keep removed rows to calculate reaction forces
            var Reaction_K_Mtx = new DenseMatrix(fixedIndicies.Count, K.ColumnCount); 
            var Reaction_F_Mtx = new DenseVector(fixedIndicies.Count);

            for (int i = 0; i < fixedIndicies.Count; i++)
            {
                Reaction_K_Mtx.SetRow(fixedIndicies.Count - i - 1, K.Row(fixedIndicies[i])); // save values which are going to be removed from K
                Reaction_F_Mtx[fixedIndicies.Count - i - 1] -= Fm[fixedIndicies[i], 0]; // adds any forces that are pointed against the direction of reaction forces
            }

            // Now that we've stored the rows, remove rows not needed for solving displacements
            foreach (int index in fixedIndicies)
            {
                K = (SparseMatrix)K.RemoveRow(index);
                Fm = (DenseMatrix)Fm.RemoveRow(index);
            }

            if (debugging)  // Display matricies for the user
            {
                //var form = new MatrixViewerForm(Fm, "F Matrix Reduced");
                var form = new MatrixViewerForm([K, Fm], ["K Matrix Reduced", "F Matrix Reduced"]);
            }

            SparseVector Q_Solved = (SparseVector)K.Solve(Fm).Column(0); // solve displacements
            Reaction_F_Mtx += (DenseVector)Reaction_K_Mtx.Multiply(Q_Solved); // add reactions due to displacements

            var Q_output = new DenseVector(problemSize);
            var R_output = new DenseVector(problemSize);

            // Tranfer separated fixed/floating results into the singular outputs
            int fixedRow = 0;
            int floatingRow = 0;
            for (int i = 0; i < problemSize; i++)
            {
                if (fixedIndicies.Contains(i)) // this row has been fixed
                {
                    Q_output[i] = 0;
                    R_output[i] = Reaction_F_Mtx[fixedRow];
                    fixedRow += 1;
                }
                else // floating row
                {
                    Q_output[i] = Q_Solved[floatingRow];
                    R_output[i] = 0; // Reaction must be 0 for a floating displacement
                    floatingRow += 1;
                }
            }

            return [Q_output, R_output];
        }


    } // need to call functions in here from element/node events upon creation/deletion
}
