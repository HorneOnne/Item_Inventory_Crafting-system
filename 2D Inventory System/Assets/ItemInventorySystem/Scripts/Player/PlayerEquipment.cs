using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Manages the player's equipment, including equipping and unequipping items, and updating the player's appearance based on their equipment.
    /// </summary>
    [RequireComponent(typeof(Player))]
    public class PlayerEquipment : MonoBehaviour
    {
        [Header("REFERENCES")]
        private Player player;

        /// <summary>
        /// The sprite renderer for the helmet.
        /// </summary>
        [SerializeField] private SpriteRenderer helmSr;

        /// <summary>
        /// The sprite renderer for the chest armor.
        /// </summary>

        [SerializeField] private SpriteRenderer chestSr;


        #region Properties
        /// <summary>
        /// The item slot for the helmet.
        /// </summary>
        public ItemSlot Helm { get; private set; }

        /// <summary>
        /// The item slot for the chest armor.
        /// </summary>
        public ItemSlot Chest { get; private set; }

        /// <summary>
        /// The item slot for the shield.
        /// </summary>
        public ItemSlot Shield { get; private set; }
        #endregion


        private void Awake()
        {
            player = GetComponent<Player>();
            Helm = new ItemSlot();
            Chest = new ItemSlot();
            Shield = new ItemSlot();
        }


        /// <summary>
        /// Subscribes to the player equipment changed event.
        /// </summary>
        private void OnEnable()
        {
            EventManager.OnPlayerEquipmentChanged += UpdatePlayerEquipSkin;
        }


        /// <summary>
        /// Unsubscribes from the player equipment changed event.
        /// </summary>
        private void OnDisable()
        {
            EventManager.OnPlayerEquipmentChanged -= UpdatePlayerEquipSkin;
        }


        /// <summary>
        /// Attempts to equip a helmet item.
        /// </summary>
        /// <param name="equipItemSlot">The item slot of the item to be equipped.</param>
        /// <returns>True if the item was equipped, false otherwise.</returns>
        private bool TryEquipHelm(ItemSlot equipItemSlot)
        {
            bool canEquip = false;

            if (equipItemSlot.ItemData.itemType == ItemType.Helm)
            {
                canEquip = true;
                Helm = new ItemSlot(equipItemSlot);
            }

            return canEquip;
        }


        /// <summary>
        /// Attempts to equip a chest armor item.
        /// </summary>
        /// <param name="equipItemSlot">The item slot of the item to be equipped.</param>
        /// <returns>True if the item was equipped, false otherwise.</returns>

        private bool TryEquipChest(ItemSlot equipItemSlot)
        {
            bool canEquip = false;

            if (equipItemSlot.ItemData.itemType == ItemType.ChestArmor)
            {
                canEquip = true;
                Chest = new ItemSlot(equipItemSlot);
            }

            return canEquip;
        }

        /// <summary>
        /// Attempts to equip a shield item.
        /// </summary>
        /// <param name="equipItemSlot">The item slot of the item to be equipped.</param>
        /// <returns>True if the item was equipped, false otherwise.</returns>
        private bool TryEquipShield(ItemSlot equipItemSlot)
        {
            bool canEquip = false;

            if (equipItemSlot.ItemData.itemType == ItemType.Shield)
            {
                canEquip = true;
                Shield = new ItemSlot(equipItemSlot);
            }

            return canEquip;
        }



        /// <summary>
        /// Equips the given item slot to the corresponding equipment slot based on its item type.
        /// </summary>
        /// <param name="itemType">The type of item to equip.</param>
        /// <param name="equipItemSlot">The item slot to equip.</param>
        /// <returns>True if the item was successfully equipped, false otherwise.</returns>
        public bool Equip(ItemType itemType, ItemSlot equipItemSlot)
        {
            bool canEquip;
            switch (itemType)
            {
                case ItemType.Helm:
                    canEquip = TryEquipHelm(equipItemSlot);
                    break;
                case ItemType.ChestArmor:
                    canEquip = TryEquipChest(equipItemSlot);
                    break;
                case ItemType.Shield:
                    canEquip = TryEquipShield(equipItemSlot);
                    break;
                default:
                    canEquip = false;
                    throw new System.Exception();
            }

            return canEquip;
        }

        /// <summary>
        /// Returns the item slot for the given equipment type.
        /// </summary>
        /// <param name="equipmentType">The type of equipment to retrieve.</param>
        /// <returns>The corresponding item slot, or null if the equipment type is not recognized.</returns>
        public ItemSlot GetEquipmentSlot(ItemType equipmentType)
        {
            switch (equipmentType)
            {
                case ItemType.Helm:
                    return Helm;
                case ItemType.ChestArmor:
                    return Chest;
                case ItemType.Shield:
                    return Shield;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Updates the player's armor skin based on the current equipment.
        /// </summary>
        private void UpdatePlayerEquipSkin()
        {
            if (helmSr == null || chestSr == null)
                Debug.LogError("Missing player armor skin references.");

            if (Helm.HasItem())
                helmSr.sprite = Helm.GetItemIcon();
            else
                helmSr.sprite = null;

            if (Chest.HasItem())
                chestSr.sprite = Chest.GetItemIcon();
            else
                chestSr.sprite = null;
        }
    }
}