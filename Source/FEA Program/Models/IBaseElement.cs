namespace FEA_Program.Models
{
    internal interface IBaseElement
    {
        // ------------------ From Element Baseclass -----------------
        // ReadOnly Property NumOfNodes As Integer
        // ReadOnly Property NodeDOFs As Integer
        // ReadOnly Property ElemDOFs As Integer

        int ID { get; }

        int Material { get; set; }
    }
}
