namespace UltimateItemSystem
{
    /// <summary>
    /// An interface for objects that can be used by the player.
    /// </summary>
    public interface IUseable
    {
        /// <summary>
        /// Uses the object and returns whether it was successful.
        /// </summary>
        /// <param name="player">The player using the object.</param>
        /// <returns>True if the object was successfully used, false otherwise.</returns>
        public bool Use(Player player);
    }
}
