namespace FEA_Program.Models
{
    internal class MaterialManager
    {
        private readonly Dictionary<int, Material> _Materials = []; // reference by ID

        // ---------------------- Events ----------------------------

        public event EventHandler<Dictionary<int, Material>>? MaterialListChanged;

        // ---------------------- Public Properties ----------------------------

        public List<Material> MaterialList => _Materials.Values.ToList();

        // ---------------------- Public Methods ----------------------------

        // TODO: Update so material deletions or edits affect and elements with the material

        public void Add(string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, MaterialType subtype)
        {
            int ID = CreateMatlId();
            _Materials.Add(ID, new Material(Name, E_GPa, ID, subtype)
            {
                V = V,
                E = E_GPa * Math.Pow(1000, 3),
                Sy = Sy_MPa * Math.Pow(1000, 2),
                Sut = Sut_MPa * Math.Pow(1000, 2),
            });

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

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="materials"></param>
        public void ImportMaterials(List<Material> materials)
        {
            _Materials.Clear();
            foreach (var material in materials)
            {
                _Materials[material.ID] = material;
            }

            MaterialListChanged?.Invoke(this, _Materials);
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
