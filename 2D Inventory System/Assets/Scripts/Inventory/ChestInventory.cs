using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DIVH_InventorySystem
{
    public class ChestInventory : MonoBehaviour
    {
        [Header("References")]
        public Player player;
        private ItemInHand itemInHand;


        [Header("Inventory Settings")]
        private const int MAX_NORMAL_CHEST_SLOT = 36;
        public List<ItemSlot> inventory = new List<ItemSlot>();


        private void Start()
        {
            for (int i = 0; i < MAX_NORMAL_CHEST_SLOT; i++)
            {
                inventory.Add(new ItemSlot());
            }
        }

        public void Set(Player player)
        {
            this.player = player;

            if (player != null)
                itemInHand = this.player.ItemInHand;
            else
                itemInHand = null;
        }

        public ItemData GetItem(int slotIndex)
        {
            if (HasSlot(slotIndex))
            {
                return inventory[slotIndex].ItemData;
            }
            return null;
        }


        public bool AddItem(int index)
        {
            bool isSlotNotFull = inventory[index].AddItem();
            EventManager.ChestInventoryUpdate();
            return isSlotNotFull;
        }


        public void AddNewItemAt(int index, ItemData item)
        {
            inventory[index].AddNewItem(item);
            EventManager.ChestInventoryUpdate();
        }

        public void RemoveItemFromInventoryAtIndex(int index)
        {
            inventory[index].RemoveItem();
            EventManager.ChestInventoryUpdate();
        }


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


        public bool HasItem(int slotIndex)
        {
            if (HasSlot(slotIndex))
            {
                return inventory[slotIndex].HasItem();
            }
            return false;
        }

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