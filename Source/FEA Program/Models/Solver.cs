using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;

namespace FEA_Program.Models
{
    /// <summary>
    /// For solving FEA problems
    /// </summary>
    internal class Solver
    {
        /// <summary>
        /// Fires when the solver indicates problem matricies have been computed
        /// </summary>
        public event EventHandler<(Matrix, Matrix, Matrix)>? SolutionStarted;

        /// <summary>
        /// Called when the solver indicates problem matricies with fixed displacements removed have been calculated
        /// </summary>
        public event EventHandler<(Matrix, Matrix)>? PartiallyReducedCalculated;

        /// <summary>
        /// Called when the solver indicates problem matricies in their final solving form have been calculated
        /// </summary>
        public event EventHandler<(Matrix, Matrix)>? FullyReducedCalculated;

        // ---------------------- Methods ----------------------

        /// <summary>
        /// Solves the FEA Problem
        /// </summary>
        /// <param name="K">The global stiffness matrix</param>
        /// <param name="F">The global force vector</param>
        /// <param name="Q">The global fixity vector</param>
        /// <returns>The global [Displacement Vector, Reaction Force Vector]</returns>
        public DenseVector[] Solve(SparseMatrix K, DenseVector F, DenseVector Q)
        {
            SolutionStarted?.Invoke(this, new(K, SparseMatrix.OfColumnVectors(Q), SparseMatrix.OfColumnVectors(F)));
            
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

            PartiallyReducedCalculated?.Invoke(this, (K, Fm));

            var Q_Solved = SolveReduced(DenseMatrix.OfMatrix(K), DenseMatrix.OfMatrix(Fm));  // solve displacements
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
        private DenseVector SolveReduced(DenseMatrix K, DenseMatrix F)
        {
            int problemSize = F.RowCount;

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

            FullyReducedCalculated?.Invoke(this, (K, F));

            // -------------------- Step 3 - Solve reduced solution -------------------------

            DenseVector Q = (DenseVector)K.Solve(F).Column(0); // solve displacements

            // -------------------- Step 4 - Re-add duplicate displacement values -------------------------

            DenseVector Q_output = new(problemSize);

            // Re add unique values to the solution
            var uniqueRowIndices = Enumerable.Range(0, problemSize).Except(allDuplicateRows).ToList();

            for (int i = 0; i < Q.Count; i++)
            {
                Q_output[uniqueRowIndices[i]] = Q[i];
            }

            // Re add duplicate values to the solution
            foreach (var kvp in duplicateRowIndexes)
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
    }
}
