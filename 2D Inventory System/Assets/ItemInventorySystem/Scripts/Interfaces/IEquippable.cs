namespace DIVH_InventorySystem
{
    /// <summary>
    /// Interface for items that can be equipped by a player.
    /// </summary>
    public interface IEquippable
    {
        /// <summary>
        /// Equips the item to the specified player.
        /// </summary>
        /// <param name="player">The player that will equip the item.</param>
        public void Equip(Player player);
    }
}