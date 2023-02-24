using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// A script to handle the collection of items by the player in a loot zone.
    /// </summary>
    public class PlayerLootZone : MonoBehaviour
    {
        /// <summary>
        /// The game object representing the player.
        /// </summary>
        public GameObject playerGameObject;

        // Cached
        private Player player;
        private ICollectible collectibleObject;
        private UIPlayerInventory uiPlayerInventory;

        private void Start()
        {
            player = playerGameObject.GetComponent<Player>();
            uiPlayerInventory = UIPlayerInventory.Instance;
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            collectibleObject = collision.gameObject.GetComponent<ICollectible>();
            if (collectibleObject != null)
            {
                collectibleObject.Collect(player);
                uiPlayerInventory.UpdateInventoryUI();
            }
        }
    }
}
