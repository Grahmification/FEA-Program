namespace FEA_Program.Models
{
    internal class MaterialManager
    {
        private readonly Dictionary<int, Material> _Materials = []; // reference by ID

        // ---------------------- Events ----------------------------

        public event EventHandler<Dictionary<int, Material>>? MaterialListChanged;

        // ---------------------- Public Properties ----------------------------

        public List<Material> MaterialList => _Materials.Values.ToList();
        public Dictionary<int, double> All_E => _Materials.Values.ToDictionary(
            mat => mat.ID,  // Key selector: The ID of the Material object
            mat => mat.E    // Value selector: The E property of the Material object
        );

        // ---------------------- Public Methods ----------------------------

        public void Add(string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, MaterialType subtype)
        {

            int ID = CreateMatlId();
            var mat = new Material(Name, E_GPa, V, Sy_MPa, Sut_MPa, ID, subtype);
            _Materials.Add(ID, mat);

            MaterialListChanged?.Invoke(this, _Materials);

        }
        public void Delete(int ID)
        {
            _Materials.Remove(ID);
            MaterialListChanged?.Invoke(this, _Materials);
        }
        public Material GetMaterial(int id) => _Materials[id];
        public void AddDefaultMaterials()
        {
            Add("Aluminum 6061-T6", 69, 0.3, 276, 210, MaterialType.Aluminum_Alloy);
            Add("1045 Steel", 200, 0.29, 565, 310, MaterialType.Steel_Alloy);
        }

        // ---------------------- Private Helpers ----------------------------

        private int CreateMatlId()
        {
            int NewID = 1;
            bool IDUnique = false;

            while (_Materials.Keys.Contains(NewID))
                NewID += 1;

            return NewID;
        }
    }
}
