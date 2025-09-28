using FEA_Program.Models;

namespace FEA_Program.Drawable
{
    internal interface IElementDrawable: IElement
    {
        public bool Selected { get; set; }

        public void Draw(List<double[]> nodeCoords);
    }
}
