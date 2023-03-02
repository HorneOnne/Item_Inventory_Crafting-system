using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// Interface for items that can be dropped by a player.
    /// </summary>
    public interface IDroppable
    {
        /// <summary>
        /// Drops the item at the specified position and rotation, and assigns it to the specified player.
        /// </summary>
        /// <param name="player">The player to assign the dropped item to.</param>
        /// <param name="position">The position to drop the item at.</param>
        /// <param name="rotation">The rotation of the dropped item.</param>
        /// <param name="forceDestroyItemObject">Whether to force destroy the item object.</param>
        public void Drop(Player player, Vector2 position, Vector3 rotation, bool forceDestroyItemObject);
    }
}