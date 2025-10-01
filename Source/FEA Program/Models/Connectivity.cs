using FEA_Program.Forms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;

namespace FEA_Program.Models
{
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
            SparseMatrix Fm = SparseMatrix.OfColumnVectors(F);

            int problemSize = Q.Count;

            // get indicies of each fixed displacement
            // If Q[i] == 1 then fixed
            var fixedDisplacementIndicies = Enumerable.Range(0, problemSize).Where(i => Q[i] == 1).ToList();

            // Remove unused directions (no fixity, no force, no K)
            // If K has a row that's all zeros and corresponding F is also Zero, we should treat it as fixed too
            var zeroKandFIndicies = GetZeroRows(K)
                            .Where(index => F[index] == 0); // Only keep if F[index] is 0

            // Combine, ensure uniqueness, and sort in descending order (largest to smallest)
            // Sort because rows must be removed from the matrix starting with the highest index
            var fixedIndicies = fixedDisplacementIndicies
                .Union(zeroKandFIndicies)  // Combine and enforce uniqueness
                .OrderByDescending(i => i) // Sort largest to smallest
                .ToList();

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
                Fm = (SparseMatrix)Fm.RemoveRow(index);
            }

            var Q_Solved = SolveReduced(DenseMatrix.OfMatrix(K), DenseMatrix.OfMatrix(Fm), debugging);  // solve displacements
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

        /// <summary>
        /// Solves the FEA solution to get displacements after the fixed indicies have been removed
        /// </summary>
        /// <param name="K">The global stiffness matrix with fixed rows/columns removed</param>
        /// <param name="F">The global force matrix with fixed rows removed</param>
        /// <returns>The displacement vector</returns>
        private static DenseVector SolveReduced(DenseMatrix K, DenseMatrix F, bool debugging)
        {
            int problemSize = F.RowCount;
            
            if (debugging)  // Display matricies for the user
            {
                var form = new MatrixViewerForm([K, F], ["K Matrix Partially Reduced", "F Matrix Partially Reduced"]);
            }

            // Check for errors. If we still a row of K[i] = [0....] and F[i] = 0, we have a force pointing in an un-constrained direction
            if (GetZeroRows(SparseMatrix.OfMatrix(K)).Where(index => F[index, 0] != 0).Any())
            {
                throw new ArithmeticException("Unable to solve. There is a force pointing in a direction that is unconstrained, resulting in infinite displacement.");
            }

            // -------------------- Step 1 - Find any duplicate equations in the solution -------------------------

            // Get indexes of and duplicate rows, indexed by the first occurrence
            SortedDictionary<int, List<int>> duplicateRowIndexes = GetDuplicateEquationIndexes(K, F);
            var allDuplicateRows = duplicateRowIndexes.Values.SelectMany(list => list).OrderByDescending(row => row);

            // -------------------- Step 2 - Remove all duplicate rows/columns to fully reduce solution -------------------------

            foreach (int duplicateRow in allDuplicateRows)
            {
                K = (DenseMatrix)K.RemoveRow(duplicateRow);
                K = (DenseMatrix)K.RemoveColumn(duplicateRow);
                F = (DenseMatrix)F.RemoveRow(duplicateRow);
            }

            // -------------------- Step 3 - Solve reduced solution -------------------------

            if (debugging)  // Display matricies for the user
            {
                var form = new MatrixViewerForm([K, F], ["K Matrix Fully Reduced", "F Matrix Fully Reduced"]);
            }

            DenseVector Q = (DenseVector)K.Solve(F).Column(0); // solve displacements

            // -------------------- Step 4 - Re-add duplicate displacement values -------------------------

            DenseVector Q_output = new(problemSize);

            // Re add unique values to the solution
            var uniqueRowIndices = Enumerable.Range(0, problemSize).Except(allDuplicateRows).ToList();

            for(int i = 0; i < Q.Count; i++)
            {
                Q_output[uniqueRowIndices[i]] = Q[i];
            }

            // Re add duplicate values to the solution
            foreach(var kvp in duplicateRowIndexes)
            {
                // If there are duplicate equations the displacement will be over-estimated by the number of duplicates
                Q_output[kvp.Key] /= (kvp.Value.Count + 1);

                foreach (int duplicateIndex in kvp.Value)
                {
                    Q_output[duplicateIndex] = Q_output[kvp.Key];
                }
            }

            return Q_output;
        }

        /// <summary>
        /// Gets duplicate equation indexes in the reduced solution
        /// </summary>
        /// <param name="K">The global stiffness matrix with fixed rows/columns removed</param>
        /// <param name="F">The global force matrix with fixed rows removed</param>
        /// <returns>[first index of duplicate, indexes of corresponding duplicates]</returns>
        private static SortedDictionary<int, List<int>> GetDuplicateEquationIndexes(DenseMatrix K, DenseMatrix F)
        {
            // Append F onto the last column of K
            DenseMatrix equationRows = DenseMatrix.OfColumnVectors(K.EnumerateColumns().Append(F.Column(0)));

            // Get indexes of and duplicate rows, indexed by the first occurrence
            SortedDictionary<int, List<int>> duplicateRowIndexes = [];

            // Stores the first occurrence of each unique row vector.
            // Key: The unique row vector itself.
            // Value: The row index of its first occurrence.
            var uniqueRowsMap = new Dictionary<Vector<double>, int>();

            for (int i = 0; i < equationRows.RowCount; i++)
            {
                Vector<double> currentRow = equationRows.Row(i);

                // Try to get the index of the first occurrence of this row.
                if (uniqueRowsMap.TryGetValue(currentRow, out int firstIndex))
                {
                    // Case 1: Duplicate Found (currentRow already exists in the dictionary)
                    // Ensure the list for the first occurrence index exists in the output dictionary.
                    if (!duplicateRowIndexes.ContainsKey(firstIndex))
                    {
                        duplicateRowIndexes[firstIndex] = [];
                    }

                    // Add the current index 'i' to the list of duplicates for 'firstIndex'.
                    duplicateRowIndexes[firstIndex].Add(i);
                }
                else
                {
                    // Case 2: Unique Row (first time seeing this row)
                    // Store the current row vector and its index as the first occurrence.
                    uniqueRowsMap.Add(currentRow, i);
                }
            }

            return duplicateRowIndexes;
        }

        /// <summary>
        /// Get all the row indicies where every value in the row is 0
        /// </summary>
        /// <param name="matrix">The matrix to evaluate</param>
        /// <returns></returns>
        public static List<int> GetZeroRows(SparseMatrix matrix)
        {
            // 1. Safely cast the matrix's internal storage to the SparseCompressedRow format.
            var storage = matrix.Storage as SparseCompressedRowMatrixStorage<double>;

            // Safety check, though for a SparseMatrix it should succeed
            if (storage == null)
            {
                // Fallback to iterating rows if the storage type is unexpected (less efficient)
                return Enumerable.Range(0, matrix.RowCount)
                                 .Where(r => matrix.Row(r).All(v => v == 0.0))
                                 .ToList();
            }

            // Access the CSR RowPointers array, which stores the start index of each row's 
            // non-zero elements in the Values array.
            var rowPointers = storage.RowPointers;

            // The number of non-zero elements in row 'i' is: rowPointers[i+1] - rowPointers[i].
            // If this difference is 0, the row contains only zeros (explicitly or implicitly).
            var zeroRowIndices = new List<int>();
            for (int i = 0; i < matrix.RowCount; i++)
            {
                // Check if the start index for the current row is the same 
                // as the start index for the next row.
                if (rowPointers[i + 1] - rowPointers[i] == 0)
                {
                    zeroRowIndices.Add(i);
                }
            }

            return zeroRowIndices;
        }
    } // need to call functions in here from element/node events upon creation/deletion
}
