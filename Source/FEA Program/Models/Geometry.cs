using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal static class Geometry
    {
        /// <summary>
        /// Computes the absolute length between two coordinates with arbitrary dimensions
        /// </summary>
        /// <param name="c2">Coordinate 2 [X,...]</param>
        /// <param name="c1">Coordinate 1 [X,...]</param>
        /// <returns></returns>
        public static double Length(double[]c2, double[] c1)
        {
            var dimensions = Math.Min(c1.Length, c2.Length);

            DenseVector lengthVector = new(dimensions);

            for (int i = 0; i < dimensions; i++)
            {
                lengthVector[i] = c2[i] - c1[i];
            }

            var length = lengthVector.L2Norm(); // Compute length
            return length;
        }

        /// <summary>
        /// Computes the direction cosines (l, m, n) between two coordinates
        /// </summary>
        /// <param name="c2">Coordinate 2 [X,...]</param>
        /// <param name="c1">Coordinate 1 [X,...]</param>
        /// <returns>Tuple (l, m, n)</returns>
        public static (double l, double m, double n) ComputeDirectionCosines(double[] c2, double[] c1)
        {
            var dimensions = Math.Min(c1.Length, c2.Length);
            double L = Length(c2, c1); // Element length

            if (L == 0)
                throw new ArgumentException("Coordinates are identical — zero length element.");

            if (dimensions == 0)
                throw new ArgumentException("Cannot compute angle based on zero length coordinate");

            // C2 - C1 = Vector from node1 → node2
            double l = (c2[0] - c1[0]) / L;  // dx / L
            double m = 0;
            double n = 0;

            if (dimensions > 1)
                m = (c2[1] - c1[1]) / L; // dy / L

            if (dimensions > 2)
                n = (c2[2] - c1[2]) / L;  // dz / L

            return (l, m, n);
        }
    }
}
