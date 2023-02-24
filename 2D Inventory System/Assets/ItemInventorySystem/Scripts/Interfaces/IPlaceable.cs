using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Interface for items that can be placed in the game world.
    /// </summary>
    public interface IPlaceable
    {
        /// <summary>
        /// Gets or sets whether to show the ray when checking if the item can be placed.
        /// </summary>
        public bool ShowRay { get; set; }


        /// <summary>
        /// Gets or sets the layer to check for placing the item on it.
        /// </summary>
        public LayerMask PlacedLayer { get; set; }


        /// <summary>
        /// Checks if the item is above the specified layer mask.
        /// </summary>
        /// <param name="player">The player who is placing the item.</param>
        /// <param name="showRay">Whether to show the ray for the check.</param>
        /// <returns>True if the item is above the specified layer mask, otherwise false.</returns>
        public bool IsAboveGround(Player player, bool showRay = false);


        /// <summary>
        /// Places the item at the specified position in the game world.
        /// </summary>
        /// <param name="placedPosition">The position to place the item at.</param>
        /// <param name="player">The player who is placing the item.</param>
        /// <param name="parent">The transform to parent the item to.</param>
        public void Placed(Vector3 placedPosition, Player player = null, Transform parent = null);
    }
}