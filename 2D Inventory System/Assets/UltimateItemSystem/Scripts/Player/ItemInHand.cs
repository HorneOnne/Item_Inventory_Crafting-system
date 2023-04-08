using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UltimateItemSystem
{
    /// <summary>
    /// Represents an item that is currently held in the player's hand.
    /// </summary>
    public class ItemInHand : MonoBehaviour
    {
        /// <summary>
        /// The item slot that contains the item data and item quantity in this hand.
        /// </summary>
        [SerializeField] private ItemSlot itemSlot;

        [Header("References")]
        /// <summary>
        /// The player who is holding this item.
        /// </summary>
        private Player player;

        /// <summary>
        /// The UI element that displays the item in this hand.
        /// </summary>
        private UIItemInHand uiItemInHand;

        /// <summary>
        /// The current item that is held in this hand.
        /// </summary>
        private Item currentItem;


        /// <summary>
        /// Indicates whether this is the first time that the item has been used.
        /// </summary>
        [Header("Settings")]
        [Tooltip("Prevent using a selected item for the first time.")]
        private bool isFirstTimeUsingItem = true;

        #region Properties
        /// <summary>
        /// The item slot data that the item was obtained from.
        /// </summary>
        public ItemSlotData ItemGetFrom { get; private set; }

        #endregion

        private void Start()
        {
            player = GetComponent<Player>();
            uiItemInHand = UIItemInHand.Instance;
        }


        /// <summary>
        /// Gets the item data for the item in this hand.
        /// </summary>
        /// <returns>The item data for the item in this hand.</returns>
        public ItemData GetItemData()
        {
            return itemSlot.ItemData;
        }


        /// <summary>
        /// Sets the item in this hand to the specified item slot.
        /// </summary>
        /// <param name="takenSlot">The item slot containing the item to set.</param>
        /// <param name="slotIndex">The index of the item slot.</param>
        /// <param name="storageType">The storage type of the item slot.</param>
        /// <param name="forceUpdateUI">Whether to force an update of the UI.</param>
        /// <param name="parent">The parent transform of the item.</param>
        public void SetItem(ItemSlot takenSlot, int slotIndex = -1, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, Transform parent = null)
        {
            itemSlot = new ItemSlot(takenSlot);
            ItemGetFrom = new ItemSlotData
            {
                slotIndex = slotIndex,
                slotStoredType = storageType
            };

            isFirstTimeUsingItem = true;
            EventManager.TriggerItemInHandChangedEvent();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
        }


        /// <summary>
        /// Swaps the item in this hand with an item in the specified inventory.
        /// </summary>
        /// <param name="inventory">The inventory to swap with.</param>
        /// <param name="slotIndex">The index of the item slot to swap with.</param>
        /// <param name="storageType">The storage type of the item slot.</param>
        /// <param name="forceUpdateUI">Whether to force an update of the UI.</param>
        /// <param name="parent">The parent transform of the item.</param>
        public void Swap(ref List<ItemSlot> inventory, int slotIndex, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, Transform parent = null)
        {
            var inHandSlotTemp = new ItemSlot(this.itemSlot);
            var invSlotChoosen = inventory[slotIndex];

            this.itemSlot = new ItemSlot(invSlotChoosen);
            inventory[slotIndex] = new ItemSlot(inHandSlotTemp);

            ItemGetFrom = new ItemSlotData
            {
                slotIndex = slotIndex,
                slotStoredType = storageType
            };


            isFirstTimeUsingItem = true;
            EventManager.TriggerItemInHandChangedEvent();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
        }


        /// <summary>
        /// Swaps the current item with the item at the given slot index in the specified inventory.
        /// </summary>
        /// <param name="inventory">The inventory array to swap with.</param>
        /// <param name="slotIndex">The index of the item slot to swap with.</param>
        /// <param name="storageType">The type of storage where the swap occurs. Default is StoredType.Another.</param>
        /// <param name="forceUpdateUI">Whether to force update the UI after the swap. Default is false.</param>
        /// <param name="parent">The parent Transform for the UI. Default is null.</param>
        public void Swap(ref ItemSlot[] inventory, int slotIndex, StoredType storageType = StoredType.Another, bool forceUpdateUI = false, Transform parent = null)
        {
            var inHandSlotTemp = new ItemSlot(this.itemSlot);
            var invSlotChoosen = inventory[slotIndex];

            this.itemSlot = new ItemSlot(invSlotChoosen);
            inventory[slotIndex] = new ItemSlot(inHandSlotTemp);

            ItemGetFrom = new ItemSlotData
            {
                slotIndex = slotIndex,
                slotStoredType = storageType
            };


            isFirstTimeUsingItem = true;
            EventManager.TriggerItemInHandChangedEvent();

            if (forceUpdateUI)
            {
                uiItemInHand.UpdateItemInHandUI(parent);
            }
        }



        /// <summary>
        /// Splits the item slot quantity in the specified inventory at the given slot index.
        /// </summary>
        /// <param name="inventory">The inventory array to split the item quantity in.</param>
        /// <param name="slotIndex">The index of the item slot to split the quantity of.</param>
        public void SplitItemSlotQuantityInInventory(ref List<ItemSlot> inventory, int slotIndex)
        {
            int itemQuantity = inventory[slotIndex].ItemQuantity;
            if (itemQuantity > 1)
            {
                int splitItemQuantity = itemQuantity / 2;
                inventory[slotIndex].SetItemQuantity(itemQuantity - splitItemQuantity);

                var chosenSlot = new ItemSlot(inventory[slotIndex]);
                chosenSlot.SetItemQuantity(splitItemQuantity);
                SetItem(chosenSlot, slotIndex, StoredType.PlayerInventory, true);
            }
            else
            {
                Swap(ref inventory, slotIndex, StoredType.PlayerInventory, true);
            }
        }

        /// <summary>
        /// Splits the item slot quantity in the specified inventory at the given slot index.
        /// </summary>
        /// <param name="inventory">The inventory array to split the item quantity in.</param>
        /// <param name="slotIndex">The index of the item slot to split the quantity of.</param>
        public void SplitItemSlotQuantityInInventory(ref ItemSlot[] inventory, int slotIndex)
        {
            int itemQuantity = inventory[slotIndex].ItemQuantity;
            if (itemQuantity > 1)
            {
                int splitItemQuantity = itemQuantity / 2;
                inventory[slotIndex].SetItemQuantity(itemQuantity - splitItemQuantity);

                var chosenSlot = new ItemSlot(inventory[slotIndex]);
                chosenSlot.SetItemQuantity(splitItemQuantity);
                SetItem(chosenSlot, slotIndex, StoredType.PlayerInventory, true);
            }
            else
            {
                Swap(ref inventory, slotIndex, StoredType.PlayerInventory, true);
            }
        }


        /// <summary>
        /// Sets the current item to the specified item.
        /// </summary>
        /// <param name="item">The item to set as the current item.</param>
        public void SetICurrentItem(Item item)
        {
            currentItem = item;
        }


        /// <summary>
        /// Gets the current item.
        /// </summary>
        /// <returns>The current item.</returns>
        public Item GetICurrenttem()
        {
            return currentItem;
        }


        /// <summary>
        /// Attempts to pick up an item from the given ItemSlot in the world and places it in the player's hand.
        /// </summary>
        /// <param name="itemContainerSlot">The ItemSlot in the world to pick up the item from.</param>
        /// <returns>A boolean indicating whether the item was successfully picked up or not.</returns>
        public bool PickupItem(ref ItemSlot itemContainerSlot)
        {
            bool canPickupItem;


            if (HasItemData() == false)
            {
                itemSlot = new ItemSlot(itemContainerSlot);
                itemContainerSlot.ClearSlot();
                uiItemInHand.UpdateItemInHandUI();
                canPickupItem = true;
            }
            else
            {
                if (itemSlot.ItemData.Equals(itemContainerSlot.ItemData))
                {
                    if (itemSlot.TryAddItem(itemContainerSlot) == true)
                    {
                        itemContainerSlot = itemSlot.AddItemsFromAnotherSlot(itemContainerSlot);
                        uiItemInHand.UpdateItemInHandUI();
                        canPickupItem = true;
                    }
                    else
                    {
                        //Debug.Log("The item quantity > item maxquantity");
                        canPickupItem = false;
                    }
                }
                else
                {
                    //Debug.Log("Not same");
                    canPickupItem = false;
                }
            }

            EventManager.TriggerItemInHandChangedEvent();
            return canPickupItem;
        }


        /// <summary>
        /// Checks whether the player currently has an item data in their hand or not.
        /// </summary>
        /// <returns>A boolean indicating whether the player currently has an item data in their hand or not.</returns>
        public bool HasItemData()
        {
            if (itemSlot == null)
                return false;

            return itemSlot.HasItem();
        }


        /// <summary>
        /// Checks whether the player currently has an item object in their hand or not.
        /// </summary>
        /// <returns>A boolean indicating whether the player currently has an item object in their hand or not.</returns>
        public bool HasItem()
        {
            return currentItem != null;
        }


        /// <summary>
        /// Gets the ItemSlot currently in the player's hand.
        /// </summary>
        /// <returns>The ItemSlot currently in the player's hand.</returns>
        public ItemSlot GetSlot() => itemSlot;


        /// <summary>
        /// Removes the item from the player's hand.
        /// </summary>
        public void ClearSlot()
        {
            itemSlot.ClearSlot();
            ItemGetFrom = new ItemSlotData
            {
                slotStoredType = StoredType.Another,
                slotIndex = -1
            };

            EventManager.TriggerItemInHandChangedEvent();
        }



        /// <summary>
        /// Uses the item currently in the player's hand.
        /// </summary>
        /// <returns>A boolean indicating whether the item was successfully used or not.</returns>
        public bool UseItem()
        {
            if (HasItemData() == false || HasItem() == false) return false;
            if (IsMouseOverUI() == true) return false;

            if (isFirstTimeUsingItem)
            {
                isFirstTimeUsingItem = false;
                return false;
            }

            return currentItem.Use(player);
        }


        /// <summary>
        /// Removes one quantity of the item in the player's hand.
        /// </summary>
        public void RemoveItem()
        {
            itemSlot.RemoveItem();
            uiItemInHand.UpdateItemInHandUI();

            EventManager.TriggerItemInHandChangedEvent();
        }


        /// <summary>
        /// Checks whether the player's mouse is currently over a UI element or not.
        /// </summary>
        /// <returns>A boolean indicating whether the player's mouse is currently over a UI element or not.</returns>
        private bool IsMouseOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // mouse click is on a UI element.
                return true;
            }
            else
            {
                // mouse click is not on a UI element
                return false;
            }
        }
    }
}

