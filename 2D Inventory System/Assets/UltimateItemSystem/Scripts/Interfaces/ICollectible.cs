namespace UltimateItemSystem
{
    /// <summary>
    /// Interface for collectible objects that can be picked up by a player.
    /// </summary>
    public interface ICollectible
    {

        /// <summary>
        /// Collects the item and adds it to the player's inventory.
        /// </summary>
        /// <param name="player">The player that collects the item.</param>
        public void Collect(Player player);
    }

}