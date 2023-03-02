using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UltimateItemSystem
{
    /// <summary>
    /// ChestInventory class represents the chest inventory functionality in the game.
    /// </summary>
    public class ChestInventory : MonoBehaviour
    {
        [Header("REFERENCES")]
        public Player player;
        private ItemInHand itemInHand;


        [Header("INVENTORY SETTINGS")]
        private const int MAX_NORMAL_CHEST_SLOT = 36;
        /// <summary>
        /// The list of all the item slots in the chest inventory.
        /// </summary>
        public List<ItemSlot> inventory = new List<ItemSlot>();


        private void Start()
        {
            for (int i = 0; i < MAX_NORMAL_CHEST_SLOT; i++)
            {
                inventory.Add(new ItemSlot());
            }
        }


        /// <summary>
        /// Set the player for which the chest inventory is associated with.
        /// </summary>
        /// <param name="player">The player reference.</param>
        public void Set(Player player)
        {
            this.player = player;

            if (player != null)
                itemInHand = this.player.ItemInHand;
            else
                itemInHand = null;
        }

        /// <summary>
        /// Get the item data from the given slot index.
        /// </summary>
        /// <param name="slotIndex">The index of the slot.</param>
        /// <returns>The item data in the given slot or null if slot is empty.</returns>
        public ItemData GetItem(int slotIndex)
        {
            if (HasSlot(slotIndex))
            {
                return inventory[slotIndex].ItemData;
            }
            return null;
        }


        /// <summary>
        /// Add an item to the given slot index in the chest inventory.
        /// </summary>
        /// <param name="index">The index of the slot.</param>
        /// <returns>True if slot is not full, false otherwise.</returns>
        public bool AddItem(int index)
        {
            bool isSlotNotFull = inventory[index].AddItem();
            EventManager.TriggerChestInventoryUpdatedEvent();
            return isSlotNotFull;
        }



        /// <summary>
        /// Add a new item at the given slot index in the chest inventory.
        /// </summary>
        /// <param name="index">The index of the slot.</param>
        /// <param name="item">The item data to be added.</param>
        public void AddNewItemAt(int index, ItemData item)
        {
            inventory[index].AddNewItem(item);
            EventManager.TriggerChestInventoryUpdatedEvent();
        }


        /// <summary>
        /// Remove an item from the given slot index in the chest inventory.
        /// </summary>
        /// <param name="index">The index of the slot.</param>
        public void RemoveItemFromInventoryAtIndex(int index)
        {
            inventory[index].RemoveItem();
            EventManager.TriggerChestInventoryUpdatedEvent();
        }


        /// <summary>
        /// Check if the chest inventory has the given slot index.
        /// </summary>
        /// <param name="slotIndex">The index of the slot.</param>
        /// <returns>True if slot exists, false otherwise.</returns>
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
        /// Determines if there is an item in the specified slot.
        /// </summary>
        /// <param name="slotIndex">The index of the slot to check.</param>
        /// <returns>True if there is an item in the specified slot, otherwise false.</returns>
        public bool HasItem(int slotIndex)
        {
            if (HasSlot(slotIndex))
            {
                return inventory[slotIndex].HasItem();
            }
            return false;
        }



        /// <summary>
        /// Returns the index of the specified item slot in the inventory.
        /// </summary>
        /// <param name="itemSlot">The item slot to search for.</param>
        /// <returns>The index of the specified item slot in the inventory, or null if it is not found.</returns>
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
        /// Stacks items of the same type in the inventory.
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
                UIChestInventory.Instance.UpdateInventoryUIAt(e.Key);
            }
        }
    }
}