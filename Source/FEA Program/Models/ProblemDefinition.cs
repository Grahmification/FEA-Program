namespace FEA_Program.Models
{
    /// <summary>
    /// Basic properties of a <see cref="StressProblem"/> based on it's <see cref="ProblemTypes"/>
    /// </summary>
    internal class ProblemDefinition(ProblemTypes problemType)
    {
        private readonly ProblemTypes _problemType = problemType;
        
        /// <summary>
        /// which elements are available in the problem
        /// </summary>
        public ElementTypes[] AvailableElements => _problemType switch
        {
            ProblemTypes.Truss_1D => [ElementTypes.TrussLinear],
            ProblemTypes.Truss_3D => [ElementTypes.TrussLinear],
            ProblemTypes.Beam_1D => [ElementTypes.BeamLinear],

            // Default case: return empty
            _ => []
        };

        /// <summary>
        /// The dimension of the current problem
        /// </summary>
        public int Dimension => _problemType switch
        {
            // Case for 1 DOFs
            ProblemTypes.Truss_1D or ProblemTypes.Beam_1D => 1,

            // Case for 3 DOFs
            ProblemTypes.Truss_3D => 3,

            // Default case (return 0)
            _ => 0
        };

        /// <summary>
        /// Whether nodes have rotation in the problem
        /// </summary>
        public bool NodesHaveRotation => _problemType switch
        {
            // Trusses - no rotation
            ProblemTypes.Truss_1D or ProblemTypes.Truss_3D => false,

            // Beams - rotation
            ProblemTypes.Beam_1D => true,

            // Default
            _ => false
        };

        /// <summary>
        /// The number of node DOFs in the current problem
        /// </summary>
        public int NodeDOFs => Node.NumberOfDOFs(Dimension, NodesHaveRotation);
    }
}
