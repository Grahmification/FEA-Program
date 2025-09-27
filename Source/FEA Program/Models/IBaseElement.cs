namespace FEA_Program.Models
{
    internal interface IBaseElement
    {
        // ------------------ From Element Baseclass -----------------
        // ReadOnly Property NumOfNodes As Integer
        // ReadOnly Property NodeDOFs As Integer
        // ReadOnly Property ElemDOFs As Integer

        int ID { get; }
        Color SelectColor { get; }
        Color[] ElemColor { get; }
        Color CornerColor(int LocalNodeID);
        bool AllCornersSameColor { get; }
        bool AllCornersEqualColor(Color C_in);

        bool Selected { get; set; }
        int Material { get; set; }
        void SetColor(Color C);
        void SetCornerColor(Color C, int LocalNodeID);
    }
}
