using FEA_Program.Models;

namespace FEA_Program.Drawable
{
    internal interface IElementDrawable: IElement
    {
        string Name { get; }

        public bool Selected { get; set; }

        public void Draw(List<double[]> nodeCoords);
    }
}
