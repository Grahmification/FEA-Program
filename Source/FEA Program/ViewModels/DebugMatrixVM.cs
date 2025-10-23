using FEA_Program.UI;
using FEA_Program.ViewModels.Base;
using FEA_Program.Windows;
using MathNet.Numerics.LinearAlgebra;
using System.Windows.Input;

namespace FEA_Program.ViewModels
{
    /// <summary>
    /// ViewModel for displaying debugging solver matricies
    /// </summary>
    internal class DebugMatrixVM: ObservableObject
    {
        // --------------------- Input Matricies -------------------------------------
        public string K_Matrix_Text => Get_K_Matrix_Test(K_Matrix);
        public Matrix<double> K_Matrix { get; set; } = Matrix<double>.Build.Dense(0, 0);
        public Matrix<double> F_Matrix { get; set; } = Matrix<double>.Build.Dense(0, 0);
        public Matrix<double> Q_Matrix { get; set; } = Matrix<double>.Build.Dense(0, 0);

        // --------------------- Partially Reduced Matricies -------------------------------------
        public string K_Matrix_Text_Reduced => Get_K_Matrix_Test(K_Matrix_Reduced);
        public Matrix<double> K_Matrix_Reduced { get; set; } = Matrix<double>.Build.Dense(0, 0);
        public Matrix<double> F_Matrix_Reduced { get; set; } = Matrix<double>.Build.Dense(0, 0);

        // --------------------- Fully Reduced Matricies -------------------------------------
        public string K_Matrix_Text_Fully_Reduced => Get_K_Matrix_Test(K_Matrix_Fully_Reduced);
        public Matrix<double> K_Matrix_Fully_Reduced { get; set; } = Matrix<double>.Build.Dense(0, 0);
        public Matrix<double> F_Matrix_Fully_Reduced { get; set; } = Matrix<double>.Build.Dense(0, 0);

        // --------------------- Commands -------------------------------------
        public ICommand ShowWindowCommand { get; }

        // --------------------- Public Methods -------------------------------------
        public DebugMatrixVM() 
        { 
            ShowWindowCommand = new RelayCommand(ShowWindow);
        }
        public void ShowWindow()
        {
            if(K_Matrix.ColumnCount == 0)
            {
                FormattedMessageBox.DisplayMessage("The problem matricies are currently empty. Solve the problem to compute the matricies.", "Empty Matricies");
            }
            
            var view = new DebuggingMatrixWindow
            {
                DataContext = this
            };
            view.Activate();
            view.Show();
        }
        public void ResetMatricies()
        {
            List<Matrix<double>> matricies = [K_Matrix, F_Matrix, Q_Matrix, 
                K_Matrix_Reduced, F_Matrix_Reduced, 
                K_Matrix_Fully_Reduced, F_Matrix_Fully_Reduced];

            for (int i=0; i<matricies.Count; i++)
            {
                matricies[i] = Matrix<double>.Build.Dense(0, 0);
            }
        }

        private static string Get_K_Matrix_Test(Matrix<double> matrix)
        {
            return $"K Matrix ({matrix.RowCount} x {matrix.ColumnCount})";
        }
    }
}
