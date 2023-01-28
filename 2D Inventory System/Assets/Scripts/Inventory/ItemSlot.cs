using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    public ItemData itemObject;
    public int itemQuantity;


    public ItemSlot(ItemData itemObject = null, int itemQuantity = 0)
    {
        this.itemObject = itemObject;
        this.itemQuantity = itemQuantity;
    }

    public ItemSlot(ItemSlot other)
    {
        this.itemObject = other.itemObject;
        this.itemQuantity = other.itemQuantity;
    }


    public string GetItemName() => itemObject.name;
    public Sprite GetItemIcon() => itemObject.icon;
    public ItemType GetItemType() => itemObject.itemType;
    public int GetItemMaxQuantity() => itemObject.max_quantity;
    public int ItemQuantity
    { 
        get { return itemQuantity; } 
        private set { itemQuantity = value; }
    }


    public bool IsFullSlot()
    {
        return this.itemQuantity >= itemObject.max_quantity;
    }

    public bool AddNewItem(ItemData itemObject)
    {
        this.itemObject = itemObject;
        itemQuantity = 1;
        return false;
    }

    public bool AddItem()
    {
        if (HasItem() == false) return false;

        itemQuantity++;
        if (itemQuantity > this.itemObject.max_quantity)
        {
            itemQuantity = this.itemObject.max_quantity;
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
        if (itemQuantity > 1)
        {
            itemQuantity--;
            return true;
        }
        else
        {
            ClearSlot();
            return false;
        }
    }


    public ItemSlot AddItemsFromAnotherSlot(ItemSlot addedSlot)
    {          
        if (HasItem() == false) return addedSlot;

        int totalOfItemQuantity = itemQuantity + addedSlot.itemQuantity;
        if (totalOfItemQuantity > this.itemObject.max_quantity)
        {
            this.ItemQuantity = this.itemObject.max_quantity;
            addedSlot.itemQuantity = totalOfItemQuantity - this.itemObject.max_quantity;
            
            return new ItemSlot(addedSlot.itemObject, addedSlot.itemQuantity);
        }
        else
        {
            this.ItemQuantity = totalOfItemQuantity;
            addedSlot.ClearSlot();
        }

        return new ItemSlot(addedSlot);      
    }


    /// <summary>
    /// return true if the quantity not over max quantity of item.
    /// return false if else.
    /// </summary>
    /// <returns></returns>
    public bool TryAddItem(ItemSlot addedSlot)
    {
        if (itemObject != addedSlot.itemObject)
            Debug.LogError("Two item added not the same.");

        return itemQuantity + addedSlot.ItemQuantity <= GetItemMaxQuantity();
    }


    public void ClearSlot()
    {
        itemObject = null;
        itemQuantity = 0;
    }

    public void SetItemQuantity(int value)
    {
        if (value > this.itemObject.max_quantity)
            this.itemQuantity = this.itemObject.max_quantity;
        else
            this.itemQuantity = value;
    }


    public bool HasSlot()
    {
        return this != null;
    }

    public bool HasItem()
    {
        return itemObject != null;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(itemObject, itemQuantity);
    }
}

