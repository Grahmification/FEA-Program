using FEA_Program.Drawable;
using MathNet.Numerics.LinearAlgebra.Double;

namespace FEA_Program.Models
{
    internal class ElementManager
    {
        private readonly Dictionary<int, IElementDrawable> _Bar1Elements = []; // reference elements by ID

        public event EventHandler<Dictionary<int, IElementDrawable>>? ElementListChanged;  // Length of Elementlist has changed
        public event EventHandler<int>? ElementChanged; // Element has changed such that list needs to be updated & screen redrawn
        public event EventHandler? ElementChanged_RedrawOnly; // Element has changed such that screen only needs to be redrawn
        public event ElementAddedEventHandler? ElementAdded;
        public delegate void ElementAddedEventHandler(int ElemID, List<int> NodeIDs); // dont use for redrawing lists or screen
        public event EventHandler<IElementDrawable>? ElementDeleted;  // dont use for redrawing lists or screen

        // ---------------------- Public Properties ----------------------------

        public List<IElementDrawable> Elemlist => _Bar1Elements.Values.ToList();

        // ---------------------- Public Methods ----------------------------

        public void Add(int nodeDOFs, Type elementType, List<int> nodeIDs, double[] elementArgs, Material material)
        {
            IElementDrawable? newElement = null;
            int newElemID = CreateUniqueId();

            // ------------------ Determine type of element ----------------------

            if (ReferenceEquals(elementType, typeof(ElementBarLinear))) // linear bar element
            {
                newElement = new ElementBarLinearDrawable(elementArgs[0], newElemID, material, nodeDOFs);
            }
            else
            {
                // dont want to add anything to the list or raise events
                throw new Exception("Tried to add element with unsupported type.");
            }  

            // -------------------- check for errors and if valid add then raise events -----------------

            if (nodeIDs.Count != newElement.NumOfNodes) // check if the right number of nodes are listed
            {
                throw new ArgumentException($"Tried to add element of type <{elementType}> with {nodeIDs.Count} nodes. Should have {NumOfNodes(newElement.GetType())} nodes.");
            }

            if (newElement is not null) // more error checking
            {
                _Bar1Elements.Add(newElement.ID, newElement);
                ElementAdded?.Invoke(newElement.ID, nodeIDs);
                ElementListChanged?.Invoke(this, _Bar1Elements);
            }
        } // nodeIDs only used to raise event about generation
        public void Delete(List<int> elementIDs)
        {
            foreach (int id in elementIDs)
            {
                var deleteElement = _Bar1Elements[id]; // save temporarily so we can raise event after deletion
                _Bar1Elements.Remove(id);
                ElementDeleted?.Invoke(this, deleteElement);
            }

            if (elementIDs.Count > 0)
            {
                ElementListChanged?.Invoke(this, _Bar1Elements);
            }
        }
        public IElement GetElement(int id) => _Bar1Elements[id];
        public void SelectElements(bool selected, int[]? ids = null)
        {
            // If IDs isn't specified, select all elements
            ids ??= [.. _Bar1Elements.Keys.OrderBy(id => id)];

            foreach (int item in ids)
                _Bar1Elements[item].Selected = selected;
            ElementChanged_RedrawOnly?.Invoke(this, new());
        }

        /// <summary>
        /// Gets K matricies for all elements
        /// </summary>
        /// <param name="connectionMatrix">Global connectivity matrix [Element ID, Node IDs]</param>
        /// <param name="nodeCoordinates">Node coordinates [Node ID, coordinates]</param>
        /// <returns>[Element ID, Element K Matrix]</returns>
        public Dictionary<int, DenseMatrix> Get_K_Matricies(Dictionary<int, int[]> connectionMatrix, Dictionary<int, double[]> nodeCoordinates)
        {
            var output = new Dictionary<int, DenseMatrix>();

            foreach (int elementID in connectionMatrix.Keys) // iterate through each element
            {
                var elementNodeCoords = new List<double[]>();

                foreach (int NodeID in connectionMatrix[elementID]) // get the coordinates of each node in the element
                    elementNodeCoords.Add(nodeCoordinates[NodeID]);

                output.Add(elementID, _Bar1Elements[elementID].K_Matrix(elementNodeCoords));
            }

            return output;
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
            }

            ElementListChanged?.Invoke(this, _Bar1Elements);
        }


        // ---------------------- Private Helpers ----------------------------

        private int CreateUniqueId()
        {
            int newID = 1;

            while (_Bar1Elements.ContainsKey(newID))
                newID += 1;

            return newID;
        }

        // ---------------------- Static Methods ----------------------------

        public static int NumOfNodes(Type elementType)
        {
            return elementType switch
            {
                Type t when t == typeof(ElementBarLinear) || t == typeof(ElementBarLinearDrawable) => new ElementBarLinear(1, 0, Material.DummyMaterial()).NumOfNodes,
                _ => 0,
            };
        }
        public static string Name(Type elementType)
        {
            return elementType switch
            {
                Type t when t == typeof(ElementBarLinear) || t == typeof(ElementBarLinearDrawable) => new ElementBarLinear(1, 0, Material.DummyMaterial()).Name,
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
