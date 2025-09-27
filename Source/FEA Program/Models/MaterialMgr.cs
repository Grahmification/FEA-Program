namespace FEA_Program.Models
{
    internal class MaterialMgr
    {

        private Dictionary<int, MaterialClass> _Materials = []; // reference nodes by ID

        public event MatlListChangedEventHandler MatlListChanged;

        public delegate void MatlListChangedEventHandler(Dictionary<int, MaterialClass> MatlList); // Length of Matllist has changed

        public MaterialClass MatObj(int ID)
        {
            return _Materials[ID];
        }
        public List<int> AllIDs
        {
            get
            {
                return _Materials.Keys.ToList();
            }
        }
        public List<MaterialClass> MatList
        {
            get
            {
                return _Materials.Values.ToList();
            }
        }
        public Dictionary<int, double> All_E
        {
            get
            {
                var output = new Dictionary<int, double>();

                foreach (MaterialClass Mat in _Materials.Values)
                    output.Add(Mat.ID, Mat.E);
                return output;
            }
        }


        public void Add(string Name, double E_GPa, double V, double Sy_MPa, double Sut_MPa, MaterialType subtype)
        {

            int ID = CreateMatlId();
            var mat = new MaterialClass(Name, E_GPa, V, Sy_MPa, Sut_MPa, subtype, ID);
            _Materials.Add(ID, mat);

            MatlListChanged?.Invoke(_Materials);

        }
        public void Delete(int ID)
        {
            _Materials.Remove(ID);

            MatlListChanged?.Invoke(_Materials);
        }

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
