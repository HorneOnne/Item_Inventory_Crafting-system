using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Manages the player's inventory.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [Header("CONST VALUE")]
        // The total number of slots in the inventory.
        private const int INVENTORY_SIZE = 45;

        // The number of slots in a row.
        private const int INVENTORY_WIDTH = 9;


        // The number of rows in the inventory.
        private const int INVENTORY_HEIGHT = 5;

        [Header("REFERENCES")]
        private Player player;
        private ItemInHand itemInHand;


        [Header("INVENTORY SETTINGS")]
        // The list of all item slots in the inventory.
        public List<ItemSlot> inventory;


        #region Properties
        public int Capacity { get { return INVENTORY_SIZE; } }
        public int WidthSize { get { return INVENTORY_WIDTH; } }
        public int HeightSize { get { return INVENTORY_HEIGHT; } }

        #endregion

        // Initializes the inventory with empty item slots.

        private void Start()
        {
            player = GetComponent<Player>();
            itemInHand = player.ItemInHand;
            inventory = new List<ItemSlot>(INVENTORY_SIZE);

            for (int i = 0; i < INVENTORY_SIZE; i++)
            {
                inventory.Add(new ItemSlot());
            }
        }

        /// <summary>
        /// Gets the item data of the item in the specified inventory slot.
        /// </summary>
        /// <param name="slotIndex">The index of the item slot to get the item data from.</param>
        /// <returns>The item data of the item in the specified slot.</returns>

        public ItemData GetItem(int slotIndex)
        {
            if (HasSlot(slotIndex))
            {
                return inventory[slotIndex].ItemData;
            }
            return null;
        }


        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="itemData">The item data of the item to add.</param>
        /// <returns>True if the item was added to the inventory, false otherwise.</returns>
        public bool AddItem(ItemData itemData)
        {
            bool canAddItem = false;

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].HasItem() == false)
                {
                    inventory[i].AddNewItem(itemData);
                    canAddItem = true;
                    break;
                }
                else
                {
                    if (inventory[i].ItemData == itemData)
                    {
                        bool canAdd = inventory[i].AddItem();

                        if (canAdd == true)
                        {
                            canAddItem = true;
                            break;
                        }
                    }
                }
            }

            return canAddItem;
        }


        /// <summary>
        /// Adds an item slot to the player's inventory, starting from the first available empty slot
        /// </summary>
        /// <param name="itemSlot">The item slot to be added to the player's inventory</param>
        /// <returns>The item slot after it has been added to the inventory</returns>
        public ItemSlot AddItem(ItemSlot itemSlot)
        {
            ItemSlot copyItemSlot = new ItemSlot(itemSlot);
            ItemSlot returnItemSlot = new ItemSlot(itemSlot);

            for (int i = 0; i < inventory.Count; i++)
            {
                returnItemSlot = inventory[i].AddItemsFromAnotherSlot(copyItemSlot);

                if (returnItemSlot.HasItem() == false)
                {
                    break;
                }

            }
            return returnItemSlot;
        }


        /// <summary>
        /// Adds an item to the inventory slot at the given index.
        /// </summary>
        /// <param name="index">The index of the inventory slot where the item should be added</param>
        /// <returns>True if the inventory slot is not full after adding the item, false otherwise</returns>
        public bool AddItemAt(int index)
        {
            bool isSlotNotFull = inventory[index].AddItem();
            EventManager.TriggerInventoryUpdatedEvent();

            return isSlotNotFull;
        }


        /// <summary>
        /// Adds a new item to the inventory slot at the given index.
        /// </summary>
        /// <param name="index">The index of the inventory slot where the new item should be added</param>
        /// <param name="item">The item data for the new item to be added</param>
        public void AddNewItemAt(int index, ItemData item)
        {
            inventory[index].AddNewItem(item);
            EventManager.TriggerInventoryUpdatedEvent();
        }


        /// <summary>
        /// Removes the item from the inventory slot at the given index.
        /// </summary>
        /// <param name="index">The index of the inventory slot where the item should be removed</param>
        public void RemoveItemAt(int index)
        {
            inventory[index].RemoveItem();
            EventManager.TriggerInventoryUpdatedEvent();
        }


        /// <summary>
        /// Returns true if the inventory slot at the given index exists, false otherwise
        /// </summary>
        /// <param name="slotIndex">The index of the inventory slot to check for existence</param>
        /// <returns>True if the inventory slot exists, false otherwise</returns>
        public bool HasSlot(int slotIndex)
        {
            try
            {
                inventory[slotIndex].HasItem();
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Returns true if the inventory slot at the given index has an item in it, false otherwise
        /// </summary>
        /// <param name="slotIndex">The index of the inventory slot to check for an item</param>
        /// <returns>True if the inventory slot has an item, false otherwise</returns>
        public bool HasItem(int slotIndex)
        {
            if (HasSlot(slotIndex))
            {
                return inventory[slotIndex].HasItem();
            }
            return false;
        }

        /// <summary>
        /// Returns the index of the inventory slot that contains the given item slot, if it exists in the inventory
        /// </summary>
        /// <param name="itemSlot">The item slot to search for in the inventory</param>
        /// <returns>The index of the inventory slot containing the item slot, or null if it is not found</returns>
        public int? GetSlotIndex(ItemSlot itemSlot)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].Equals(itemSlot))
                {
                    return i;
                }
            }

            return null;
        }


        /// <summary>
        /// Attempts to stack the item currently in the hand with any matching items in the inventory.
        /// </summary>
        public void StackItem()
        {
            if (itemInHand.GetItemData() == null) return;
            Dictionary<int, int> dict = new Dictionary<int, int>();
            Dictionary<int, int> sortedDict = new Dictionary<int, int>();

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].ItemData == itemInHand.GetItemData())
                {
                    dict.Add(i, inventory[i].ItemQuantity);
                }
            }

            // Use OrderBy to sort the dictionary by value
            sortedDict = dict.OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var e in sortedDict)
            {
                itemInHand.GetSlot().AddItemsFromAnotherSlot(inventory[e.Key]);
                UIItemInHand.Instance.UpdateItemInHandUI();
                UIPlayerInventory.Instance.UpdateInventoryUIAt(e.Key);
            }
        }


        /// <summary>
        /// Finds the index of the first slot containing an arrow in the inventory, or returns null if no such slot exists.
        /// </summary>
        /// <returns>The index of the arrow slot, or null if no arrow slot exists.</returns>
        public int? FindArrowSlotIndex()
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].HasItem() == false)
                    continue;

                if (inventory[i].GetItemType() == ItemType.Arrow)
                    return i;
            }

            return null;
        }
    }
}