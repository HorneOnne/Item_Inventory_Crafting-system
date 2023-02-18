using System;
using UnityEngine;

/// <summary>
/// ItemSlot is a combination of item data and item quantity.
/// </summary>
[System.Serializable]
public class ItemSlot
{
    public ItemData ItemData { get; private set; }
    public int ItemQuantity { get; protected set; }


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
        if(ItemData == null) return null;
        return ItemData.icon;
    } 
    public ItemType GetItemType() => ItemData.itemType;
    public int GetItemMaxQuantity() => ItemData.max_quantity;


    public bool IsFullSlot()
    {
        return this.ItemQuantity >= ItemData.max_quantity;
    }

    public bool AddNewItem(ItemData itemObject)
    {
        this.ItemData = itemObject;
        ItemQuantity = 1;
        return false;
    }

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
    /// Remove 1 item in slot.
    /// Clear slot if quantity of item = 0.
    /// </summary>
    /// <returns></returns>
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
    /// Combine itemSlot with another itemSlot.
    /// Return ItemSlot information if cannot add or current itemSlot is full.
    /// </summary>
    /// <param name="addedSlot"></param>
    /// <returns></returns>
    public ItemSlot AddItemsFromAnotherSlot(ItemSlot addedSlot)
    {
        if (HasItem() == false)
        {
            this.ItemData = addedSlot.ItemData;
            this.ItemQuantity= addedSlot.ItemQuantity;
            addedSlot = new ItemSlot();
        }
        else
        {
            if(ItemData.Equals(addedSlot.ItemData))
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
    /// return true if the quantity not over max quantity of item.
    /// return false if else.
    /// </summary>
    /// <returns></returns>
    public bool TryAddItem(ItemSlot addedSlot)
    {
        if (ItemData != addedSlot.ItemData)
            Debug.LogError("Two item added not the same.");

        return ItemQuantity + addedSlot.ItemQuantity <= GetItemMaxQuantity();
    }


    public void ClearSlot()
    {
        ItemData = null;
        ItemQuantity = 0;
    }

    public void SetItemQuantity(int value)
    {
        if (value > this.ItemData.max_quantity)
            this.ItemQuantity = this.ItemData.max_quantity;
        else
            this.ItemQuantity = value;
    }


    public bool HasSlot()
    {
        return this != null;
    }

    public bool HasItem()
    {
        return ItemData != null;
    }

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

