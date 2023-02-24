using System;
using UnityEditor;
using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Represents a slot in which an item can be stored.
    /// </summary>
    [System.Serializable]
    public class ItemSlot
    {
        /// <summary>
        /// Gets the item data stored in this item slot.
        /// </summary>
        [field: SerializeField] public ItemData ItemData { get; private set; }

        /// <summary>
        /// Gets the quantity of the item stored in this item slot.
        /// </summary>
        [field: SerializeField] public int ItemQuantity { get; private set; }


        public ItemSlot(ItemData itemData = null, int itemQuantity = 0)
        {
            this.ItemData = itemData;
            this.ItemQuantity = itemQuantity;
        }

        public ItemSlot(ItemSlot other)
        {
            this.ItemData = other.ItemData;
            this.ItemQuantity = other.ItemQuantity;
        }


        public string GetItemName() => ItemData.name;
        public Sprite GetItemIcon()
        {
            if (ItemData == null) return null;
            return ItemData.icon;
        }
        public ItemType GetItemType() => ItemData.itemType;

        /// <summary>
        /// Gets the maximum quantity of the item that can be stored in this item slot.
        /// </summary>
        /// <returns>The maximum quantity of the item that can be stored in this item slot.</returns>
        public int GetItemMaxQuantity() => ItemData.max_quantity;



        /// <summary>
        /// Determines whether this item slot is full.
        /// </summary>
        /// <returns><c>true</c> if this item slot is full; otherwise, <c>false</c>.</returns>
        public bool IsFullSlot()
        {
            return this.ItemQuantity >= ItemData.max_quantity;
        }


        /// <summary>
        /// Adds a new item to this item slot.
        /// </summary>
        /// <param name="itemObject">The item data to add to the slot.</param>
        public void AddNewItem(ItemData itemObject)
        {
            this.ItemData = itemObject;
            ItemQuantity = 1;
        }


        /// <summary>
        /// Increases the quantity of the item in this item slot by one.
        /// </summary>
        /// <returns><c>true</c> if the current item quantity is less than the maximum item quantity; otherwise, <c>false</c>.</returns>
        public bool AddItem()
        {
            if (HasItem() == false) return false;

            ItemQuantity++;
            if (ItemQuantity > this.ItemData.max_quantity)
            {
                ItemQuantity = this.ItemData.max_quantity;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Decreases the item quantity by one and clears the slot if the quantity of the item is zero.
        /// </summary>
        /// <returns>True if the item quantity is greater than one, otherwise false.</returns>
        public bool RemoveItem()
        {
            if (ItemQuantity > 1)
            {
                ItemQuantity--;
                return true;
            }
            else
            {
                ClearSlot();
                return false;
            }
        }


        /// <summary>
        /// Combines the current item slot with another item slot.
        /// </summary>
        /// <param name="addedSlot">The item slot to add to this item slot.</param>
        /// <returns>The new item slot if the current item slot is full or cannot add, otherwise the added item slot.</returns>
        public ItemSlot AddItemsFromAnotherSlot(ItemSlot addedSlot)
        {
            if (HasItem() == false)
            {
                this.ItemData = addedSlot.ItemData;
                this.ItemQuantity = addedSlot.ItemQuantity;
                addedSlot = new ItemSlot();
            }
            else
            {
                if (ItemData.Equals(addedSlot.ItemData))
                {
                    int totalOfItemQuantity = ItemQuantity + addedSlot.ItemQuantity;
                    if (totalOfItemQuantity > this.ItemData.max_quantity)
                    {
                        this.ItemQuantity = this.ItemData.max_quantity;
                        addedSlot.ItemQuantity = totalOfItemQuantity - this.ItemData.max_quantity;

                        addedSlot = new ItemSlot(addedSlot.ItemData, addedSlot.ItemQuantity);
                    }
                    else
                    {
                        this.ItemQuantity = totalOfItemQuantity;
                        addedSlot.ClearSlot();
                    }
                }
                else
                {
                    //throw new Exception("Two item not the same.");
                }
            }
            return addedSlot;
        }


        /// <summary>
        /// Determines if an item slot can be added to this item slot without exceeding the maximum item quantity.
        /// </summary>
        /// <param name="addedSlot">The item slot to be added.</param>
        /// <returns>True if the item quantity will not exceed the maximum, otherwise false.</returns>
        public bool TryAddItem(ItemSlot addedSlot)
        {
            if (ItemData != addedSlot.ItemData)
                Debug.LogError("Two item added not the same.");

            return ItemQuantity + addedSlot.ItemQuantity <= GetItemMaxQuantity();
        }


        /// <summary>
        /// Clears the item data and quantity from the slot.
        /// </summary>
        public void ClearSlot()
        {
            ItemData = null;
            ItemQuantity = 0;
        }

        /// <summary>
        /// Sets the quantity of the item in the slot.
        /// </summary>
        /// <param name="value">The value to set the item quantity to.</param>
        public void SetItemQuantity(int value)
        {
            if (value > this.ItemData.max_quantity)
            {
                Debug.LogWarning("Value exceeds max item quantity.");
            }
            else
                this.ItemQuantity = value;
        }


        /// <summary>
        /// Checks if this item slot has an item data.
        /// </summary>
        /// <returns>Returns true if this item slot has an item data, otherwise false.</returns>
        public bool HasItem()
        {
            return ItemData != null;
        }


        /// <summary>
        /// Checks if two item slots contain the same item data.
        /// </summary>
        /// <param name="itemSlotA">The first item slot to compare.</param>
        /// <param name="itemSlotB">The second item slot to compare.</param>
        /// <returns>Returns true if the two item slots contain the same item data, otherwise false.</returns>
        public static bool IsSameItem(ItemSlot itemSlotA, ItemSlot itemSlotB)
        {
            if (itemSlotA == null || itemSlotB == null) return false;
            if (itemSlotA.ItemData == null || itemSlotB.ItemData == null) return false;
            return itemSlotA.ItemData.Equals(itemSlotB.ItemData);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(ItemData, ItemQuantity);
        }
    }
}
