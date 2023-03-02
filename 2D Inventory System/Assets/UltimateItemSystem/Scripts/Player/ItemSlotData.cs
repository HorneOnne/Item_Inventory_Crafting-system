namespace UltimateItemSystem
{
    /// <summary>
    /// Represents data for an item slot, including the type of storage and the index of the slot.
    /// </summary>
    public struct ItemSlotData
    {
        /// <summary>
        /// The type of storage in which the item slot is stored (e.g. player inventory, container, etc.).
        /// </summary>
        public StoredType slotStoredType;

        /// <summary>
        /// The index of the item slot.
        /// </summary>
        public int slotIndex;
    }
}