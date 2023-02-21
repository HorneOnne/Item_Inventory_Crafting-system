using UnityEngine;

namespace DIVH_InventorySystem
{
    public class PlayerLootZone : MonoBehaviour
    {
        public GameObject playerGameObject;
        private Player player;



        // Cached
        private Item itemObject;
        private UIPlayerInventory uiPlayerInventory;

        private void Start()
        {
            player = playerGameObject.GetComponent<Player>();
            uiPlayerInventory = UIPlayerInventory.Instance;
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Item"))
            {
                itemObject = collision.gameObject.GetComponent<Item>();
                if (itemObject != null)
                {
                    itemObject.Collect(player);
                    uiPlayerInventory.UpdateInventoryUI();
                }
            }
        }
    }
}
