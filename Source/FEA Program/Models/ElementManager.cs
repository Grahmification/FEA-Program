using FEA_Program.Drawable;

namespace FEA_Program.Models
{
    internal class ElementManager
    {
        private readonly Dictionary<int, IElementDrawable> _Bar1Elements = []; // reference elements by ID

        public event EventHandler<Dictionary<int, IElementDrawable>>? ElementListChanged;  // Length of Elementlist has changed
        public event EventHandler<int>? ElementChanged; // Element has changed such that list needs to be updated & screen redrawn
        public event EventHandler? ElementChanged_RedrawOnly; // Element has changed such that screen only needs to be redrawn

        // ---------------------- Public Properties ----------------------------
        public StressProblem Problem { get; set; } = new();

        public List<IElementDrawable> Elemlist => _Bar1Elements.Values.ToList();

        // ---------------------- Public Methods ----------------------------

        public void Add(int nodeDOFs, Type elementType, List<NodeDrawable> nodes, double[] elementArgs, Material material)
        {
            IElementDrawable? newElement = null;
            int newElemID = IDClass.CreateUniqueId(Problem.Elements.Cast<IHasID>().ToList());

            // ------------------ Determine type of element ----------------------

            if (ReferenceEquals(elementType, typeof(ElementBarLinear))) // linear bar element
            {
                newElement = new ElementBarLinearDrawable(elementArgs[0], newElemID, nodes, material, nodeDOFs);
            }
            else
            {
                // dont want to add anything to the list or raise events
                throw new Exception("Tried to add element with unsupported type.");
            }  

            if (newElement is not null) // more error checking
            {
                _Bar1Elements.Add(newElement.ID, newElement);
                Problem.AddElement(newElement);
                ElementListChanged?.Invoke(this, _Bar1Elements);
            }
        } // nodeIDs only used to raise event about generation
        public void Delete(List<int> elementIDs)
        {
            foreach (int id in elementIDs)
            {
                _Bar1Elements.Remove(id);
                Problem.RemoveElement(id);
            }

            if (elementIDs.Count > 0)
            {
                ElementListChanged?.Invoke(this, _Bar1Elements);
            }
        }
        public void SelectElements(bool selected, int[]? ids = null)
        {
            // If IDs isn't specified, select all elements
            ids ??= [.. _Bar1Elements.Keys.OrderBy(id => id)];

            foreach (int item in ids)
                _Bar1Elements[item].Selected = selected;
            ElementChanged_RedrawOnly?.Invoke(this, new());
        }

        public void Reset()
        {
            _Bar1Elements.Clear();
            ElementListChanged?.Invoke(this, _Bar1Elements);
        }

        /// <summary>
        /// Import a dataset, usually when loading from a file
        /// </summary>
        /// <param name="elements"></param>
        public void ImportElements(List<IElementDrawable> elements)
        {
            _Bar1Elements.Clear();
            foreach (var element in elements)
            {
                _Bar1Elements[element.ID] = element;
                Problem.AddElement(element);
            }

            ElementListChanged?.Invoke(this, _Bar1Elements);
        }

        // ---------------------- Static Methods ----------------------------

        public static int NumOfNodes(Type elementType)
        {
            return elementType switch
            {
                Type t when t == typeof(ElementBarLinear) || t == typeof(ElementBarLinearDrawable) => new ElementBarLinear(1, 0, [Node.DummyNode(), Node.DummyNode()], Material.DummyMaterial()).NumOfNodes,
                _ => 0,
            };
        }
        public static string Name(Type elementType)
        {
            return elementType switch
            {
                Type t when t == typeof(ElementBarLinear) || t == typeof(ElementBarLinearDrawable) => new ElementBarLinearDrawable(1, 0, [NodeDrawable.DummyNode(), NodeDrawable.DummyNode()],Material.DummyMaterial()).Name,
                _ => "",
            };
        }
        public static Dictionary<string, Units.DataUnitType> ElementArgs(Type elementType) => elementType switch
        {
            // Case for ElementBarLinear
            Type t when t == typeof(ElementBarLinear) => new Dictionary<string, Units.DataUnitType>
            {
                { "Area", Units.DataUnitType.Area }
            },
            // Default case: return an empty dictionary
            _ => []
        };
    }
}
