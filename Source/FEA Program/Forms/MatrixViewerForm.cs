using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Forms
{
    /// <summary>
    /// A simple Windows Form application to display MathNet.Numerics matrices 
    /// (DenseMatrix and SparseMatrix) in a DataGridView.
    /// </summary>
    public partial class MatrixViewerForm : Form
    {
        private List<DataGridView> _dataGridViews = [];
        private List<Label> _matrixTypeLabels = [];

        public MatrixViewerForm(List<Matrix<double>> matricies, List<string> titles)
        {
            InitializeComponent();

            // --- Form Setup ---
            Text = "MathNet Matrix Viewer";
            ClientSize = new Size(800, 600);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.Padding = new Padding(10);
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- Layout Panel ---
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = matricies.Count,
                RowStyles =
            {
                new RowStyle(SizeType.Absolute, 40), // For label
                new RowStyle(SizeType.Percent, 100)  // For DataGridView
            }
            };

            Controls.Add(mainLayout);

            double totalColumns = matricies.Sum(m => m.ColumnCount) + matricies.Count; // Extra column for each row indicator

            for (int i = 0; i < matricies.Count; i++)
            {
                mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (int)(100.0 * (matricies[i].ColumnCount + 1) / totalColumns)));
                
                // --- Matrix Type Label (Row 0) ---
                var label = new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0),
                };
                mainLayout.Controls.Add(label, i, 0);
                _matrixTypeLabels.Add(label);

                // --- Data Grid View (Row 1) ---
                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToResizeRows = false,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders,
                    BackgroundColor = SystemColors.ControlLightLight,
                    BorderStyle = BorderStyle.None,
                    GridColor = Color.LightGray
                };
                mainLayout.Controls.Add(grid, i, 1);
                _dataGridViews.Add(grid);
            }

            for (int i = 0; i < matricies.Count; i++)
            {
                if (i == titles.Count)
                    titles.Add("");
                
                DisplayMatrix(i, matricies[i], titles[i]);
            }

            Show(); // Display the form
            BringToFront();
        }

        /// <summary>
        /// Initializes sample Dense and Sparse matrices for demonstration.
        /// </summary>
        public static SparseMatrix GetSampleMatrix()
        {
            // 5x5 Sparse Matrix (Mostly zeros)
            var _sparseMatrix = SparseMatrix.Create(5, 5, 0.0);
            _sparseMatrix[0, 0] = 99.0;
            _sparseMatrix[1, 4] = -12.345;
            _sparseMatrix[4, 2] = 0.001;
            _sparseMatrix[3, 1] = 500.0;
            return _sparseMatrix;
        }

        /// <summary>
        /// Populates the DataGridView with the elements of the provided MathNet matrix.
        /// </summary>
        /// <param name="matrix">The MathNet matrix to display.</param>
        /// <param name="title">The title for the matrix type label.</param>
        private void DisplayMatrix(int index, Matrix<double> matrix, string title = "")
        {         
            // Update the title label
            title = title == "" ? "Matrix" : title;
            _matrixTypeLabels[index].Text = $"{title} ({matrix.RowCount} x {matrix.ColumnCount})";

            var _dataGridView = _dataGridViews[index];

            // Clear existing content
            _dataGridView.Columns.Clear();
            _dataGridView.Rows.Clear();

            int rows = matrix.RowCount;
            int cols = matrix.ColumnCount;

            // 1. Setup Columns (Header: C0, C1, C2...)
            for (int j = 0; j < cols; j++)
            {
                _dataGridView.Columns.Add($"C{j}", $"Col {j}");
                _dataGridView.Columns[j].Width = 70; // Fixed width for clean look
                _dataGridView.Columns[j].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            // 2. Set the number of rows
            _dataGridView.RowCount = rows;

            // 3. Populate cells and set row headers (Header: R0, R1, R2...)
            for (int i = 0; i < rows; i++)
            {
                // Set the row header
                _dataGridView.Rows[i].HeaderCell.Value = $"Row {i}";

                // Populate the cell values
                for (int j = 0; j < cols; j++)
                {
                    // Format the number to 4 decimal places for consistency
                    double value = matrix[i, j];
                    _dataGridView.Rows[i].Cells[j].Value = value.ToString("F4");

                    // Highlight non-zero elements in the Sparse Matrix display
                    if (matrix is SparseMatrix && Math.Abs(value) > 1e-9)
                    {
                        _dataGridView.Rows[i].Cells[j].Style.BackColor = Color.FromArgb(220, 240, 255); // Light Blue Tint
                        _dataGridView.Rows[i].Cells[j].Style.Font = new Font(_dataGridView.Font, FontStyle.Bold);
                    }
                    else
                    {
                        _dataGridView.Rows[i].Cells[j].Style.BackColor = Color.White;
                        _dataGridView.Rows[i].Cells[j].Style.Font = new Font(_dataGridView.Font, FontStyle.Regular);
                    }
                }
            }

            // Auto-size the row headers (R0, R1, etc.) after setting values
            _dataGridView.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
        }
    }
}
