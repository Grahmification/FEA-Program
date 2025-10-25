using FEA_Program.ViewModels.Base;

namespace FEA_Program.Models
{
    /// <summary>
    /// Generic definition for a class that has an ID
    /// </summary>
    internal interface IHasID
    {
        /// <summary>
        /// The idenfier for the class
        /// </summary>
        public int ID { get; }
    }

    /// <summary>
    /// Base definition for a class that has an ID
    /// </summary>
    internal abstract class IDClass(int ID = IDClass.InvalidID) : ObservableObject, IHasID, IComparable<IHasID>
    {
        /// <summary>
        /// Number used to indicate an invalid ID
        /// </summary>
        public const int InvalidID = -1;

        /// <summary>
        /// The idenfier for the class
        /// </summary>
        public int ID { get; private set; } = ID;

        /// <summary>
        /// This method is required by the IComparable<T> interface
        /// </summary>
        /// <param name="other">The class to compare</param>
        /// <returns></returns>
        public int CompareTo(IHasID? other)
        {
            // Handle the case where 'other' is null
            if (other is null)
                return 1; // Any object is greater than null

            // Compare the IDs
            // This leverages the built-in comparison for 'int'
            return ID.CompareTo(other.ID);
        }

        public static int CreateUniqueId(List<IHasID> existingItems)
        {
            var existingIDs = existingItems.Select(i => i.ID);
            int newID = 1;

            while (existingIDs.Contains(newID))
                newID += 1;

            return newID;
        }
    }


}
