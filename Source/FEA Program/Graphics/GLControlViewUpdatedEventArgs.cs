using OpenTK.Mathematics;

namespace FEA_Program.Graphics
{
    /// <summary>
    /// Arguments for when a <see cref="GLControlDraggable"/> view is updated
    /// </summary>
    /// <param name="translation">Translation of the view [X,Y,Z]</param>
    /// <param name="rotation">Rotation of the view [Z,X]</param>
    /// <param name="zoom">Zoom of the view</param>
    internal class GLControlViewUpdatedEventArgs(Vector3 translation, Matrix4 rotation, double zoom, bool threeDimensional)
    {
        /// <summary>
        /// Translation of the view [X,Y,Z]
        /// </summary>
        public Vector3 Translation { get; private set; } = translation;

        /// <summary>
        /// Rotation of the view [Z,X]
        /// </summary>
        public Matrix4 Rotation { get; private set; } = rotation;

        /// <summary>
        /// Zoom of the view
        /// </summary>
        public double Zoom { get; private set; } = zoom;

        /// <summary>
        /// Zoom of the view
        /// </summary>
        public bool ThreeDimensional { get; private set; } = threeDimensional;
    }
}
