using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal class Connectivity
    {

        private Dictionary<int, List<int>> _ConnectMatrix = new Dictionary<int, List<int>>(); // dict key is global element ID, list index is local node ID, list value at index is global node ID

        public Dictionary<int, List<int>> ConnectMatrix
        {
            get
            {
                return _ConnectMatrix;
            }
        }

        public List<int> ElementNodes(int ElementID)
        {
            return _ConnectMatrix[ElementID];
        } // returns global Node ID's utilized in input element
        public List<int> NodeElements(int NodeID)
        {
            var output = new List<int>();

            foreach (KeyValuePair<int, List<int>> KVP in _ConnectMatrix)
            {
                if (KVP.Value.Contains(NodeID)) // check if the nodeID is used in the element
                {
                    output.Add(KVP.Key); // if so add the element ID to the output
                }
            }

            output.Sort(); // sort the output for good measure (lowest element ID comes first)
            return output;
        } // returns all of the element ID's attached to a global node ID

        public void AddConnection(int ElementID, List<int> NodeIDs)
        {
            _ConnectMatrix.Add(ElementID, NodeIDs);
        } // nodeIDs need to be sorted in the correct local order for the element
        public void RemoveConnection(int ElementID)
        {
            _ConnectMatrix.Remove(ElementID);
        }


        private void SortNodeLocalIDs(Dictionary<int, Dictionary<int, double[]>> NodeCoords)
        {

        }
        public SparseMatrix Assemble_K_Mtx(Dictionary<int, DenseMatrix> K_matricies, Dictionary<int, int> NodeDOFs)
        {

            // ---------------------- Get Total Size of the Problem --------------------------

            int problemSize = 0;
            foreach (int DOF in NodeDOFs.Values)
                problemSize += DOF;

            // ----------------------- Setup Output Matrix -----------------------------------

            var output = new SparseMatrix(problemSize, problemSize);

            // ------------------------Iterate Through Each Node ID and allocate regions of large matrix --------------------------

            var NodeIDs = NodeDOFs.Keys.ToList();
            NodeIDs.Sort(); // sort so smallest ID comes first

            var NodeMatrixIndicies = new Dictionary<int, int[]>(); // determines where each displacement will go on the assembled matrix, sorted by node matrix
            int IndexCounter = 0; // to count how many places have been used up in the matrix

            foreach (int ID in NodeIDs)
            {
                int DOF = NodeDOFs[ID]; // get the number of DOFs for the node
                var allocatedIndicies = new int[DOF]; // create array to hold allocated indicies

                for (int i = 0, loopTo = DOF - 1; i <= loopTo; i++) // allocate a value for each DOF incrementally based on the number of DOFs
                {
                    allocatedIndicies[i] = IndexCounter;
                    IndexCounter += 1;
                }

                NodeMatrixIndicies.Add(ID, allocatedIndicies);
            }

            // ------------------------Iterate Through Each Element --------------------------

            foreach (KeyValuePair<int, DenseMatrix> ElemID_and_K in K_matricies)
            {
                // 1. get node ID's - assume in correct order
                var E_nodeIDs = ElementNodes(ElemID_and_K.Key);

                // 2. Allocate Regions of the K Matrix for Each Element

                var NodeKmtxIndicies = new Dictionary<int, int[]>(); // holds which regions of the k matrix are claimed by each node
                IndexCounter = 0; // to count how many places have been used up in the matrix

                foreach (int ID in E_nodeIDs)
                {
                    int DOF = NodeDOFs[ID]; // get the number of DOFs for the node
                    var allocatedIndicies = new int[DOF]; // create array to hold allocated indicies

                    for (int i = 0, loopTo1 = DOF - 1; i <= loopTo1; i++) // allocate a value for each DOF incrementally based on the number of DOFs
                    {
                        allocatedIndicies[i] = IndexCounter;
                        IndexCounter += 1;
                    }

                    NodeKmtxIndicies.Add(ID, allocatedIndicies);
                }

                // 3. Move the value range for each node from the local K matrix to the global

                foreach (int i in E_nodeIDs)
                {
                    foreach (int j in E_nodeIDs)
                    {

                        for (int row = 0, loopTo2 = NodeDOFs[i] - 1; row <= loopTo2; row++)
                        {
                            for (int col = 0, loopTo3 = NodeDOFs[j] - 1; col <= loopTo3; col++)
                            {

                                int[] assembled_K_nodeRegion_i = NodeMatrixIndicies[i];
                                int[] assembled_K_nodeRegion_j = NodeMatrixIndicies[j];

                                int[] local_K_nodeRegion_i = NodeKmtxIndicies[i];
                                int[] local_K_nodeRegion_j = NodeKmtxIndicies[j];

                                output[assembled_K_nodeRegion_i[row], assembled_K_nodeRegion_j[col]] = ElemID_and_K.Value[local_K_nodeRegion_i[row], local_K_nodeRegion_j[col]];
                            }
                        }

                    }
                }

            }

            return output;
        }
        public DenseMatrix[] Solve(SparseMatrix K, DenseMatrix F, DenseMatrix Q)
        {

            var FixedIndicies = new List<int>();
            int ProblemSize = Q.RowCount;

            for (int i = 0, loopTo = ProblemSize - 1; i <= loopTo; i++) // get indicies of each fixed displacement
            {
                if (Q[i, 0] == 1)
                {
                    FixedIndicies.Add(i);
                }
            }

            FixedIndicies.Reverse(); // need to remove rows with highest index first

            foreach (int Val in FixedIndicies) // first remove columns - they will be multiplied by 0 anyway - rows are needed later for reaction forces
                K = (SparseMatrix)K.RemoveColumn(Val);

            var Reaction_K_Mtx = new DenseMatrix(FixedIndicies.Count, K.ColumnCount); // need to keep removed values to calculate reaction forces
            var Reaction_F_Mtx = new DenseMatrix(FixedIndicies.Count, 1);

            for (int i = 0, loopTo1 = FixedIndicies.Count - 1; i <= loopTo1; i++)
            {
                Reaction_K_Mtx.SetRow(FixedIndicies.Count - i - 1, K.Row(FixedIndicies[i])); // save values which are going to be removed from K
                Reaction_F_Mtx[FixedIndicies.Count - i - 1, 0] -= F[FixedIndicies[i], 0]; // adds any forces that are pointed against the direction of reaction forces
            }

            foreach (int Val in FixedIndicies) // remove rows not needed for solving displacements
            {
                K = (SparseMatrix)K.RemoveRow(Val);
                F = (DenseMatrix)F.RemoveRow(Val);
            }

            SparseMatrix Q_Solved = (SparseMatrix)K.Solve(F); // solve displacements
            Reaction_F_Mtx += (DenseMatrix)Reaction_K_Mtx.Multiply(Q_Solved); // add reactions due to displacements

            var Q_output = new DenseMatrix(ProblemSize, 1);
            var R_output = new DenseMatrix(ProblemSize, 1);

            int k_int = 0;
            int j = 0;
            for (int i = 0, loopTo2 = ProblemSize - 1; i <= loopTo2; i++)
            {
                if (FixedIndicies.Contains(i)) // this row has been fixed
                {
                    Q_output[i, 0] = 0;
                    R_output[i, 0] = Reaction_F_Mtx[k_int, 0];
                    k_int += 1;
                }
                else // floating row
                {
                    Q_output[i, 0] = Q_Solved[j, 0];
                    R_output[i, 0] = 0; // reaction must be 0 for a floating displacement
                    j += 1;
                }
            }

            return new[] { Q_output, R_output };
        }


    } // need to call functions in here from element/node events upon creation/deletion
}
