using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    public ItemData ItemData { get; private set; }
    public int itemQuantity;


    public ItemSlot(ItemData itemData = null, int itemQuantity = 0)
    {
        this.ItemData = itemData;
        this.itemQuantity = itemQuantity;
    }

    public ItemSlot(ItemSlot other)
    {
        this.ItemData = other.ItemData;
        this.itemQuantity = other.itemQuantity;
    }


    public string GetItemName() => ItemData.name;
    public Sprite GetItemIcon() => ItemData.icon;
    public ItemType GetItemType() => ItemData.itemType;
    public int GetItemMaxQuantity() => ItemData.max_quantity;
    public int ItemQuantity
    { 
        get { return itemQuantity; } 
        private set { itemQuantity = value; }
    }


    public bool IsFullSlot()
    {
        return this.itemQuantity >= ItemData.max_quantity;
    }

    public bool AddNewItem(ItemData itemObject)
    {
        this.ItemData = itemObject;
        itemQuantity = 1;
        return false;
    }

    public bool AddItem()
    {
        if (HasItem() == false) return false;

        itemQuantity++;
        if (itemQuantity > this.ItemData.max_quantity)
        {
            itemQuantity = this.ItemData.max_quantity;
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
            this.itemQuantity= addedSlot.itemQuantity;
            addedSlot = new ItemSlot();
        }
        else
        {
            if(ItemData.Equals(addedSlot.ItemData))
            {
                int totalOfItemQuantity = itemQuantity + addedSlot.itemQuantity;
                if (totalOfItemQuantity > this.ItemData.max_quantity)
                {
                    this.ItemQuantity = this.ItemData.max_quantity;
                    addedSlot.itemQuantity = totalOfItemQuantity - this.ItemData.max_quantity;

                    addedSlot = new ItemSlot(addedSlot.ItemData, addedSlot.itemQuantity);
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

        return itemQuantity + addedSlot.ItemQuantity <= GetItemMaxQuantity();
    }


    public void ClearSlot()
    {
        ItemData = null;
        itemQuantity = 0;
    }

    public void SetItemQuantity(int value)
    {
        if (value > this.ItemData.max_quantity)
            this.itemQuantity = this.ItemData.max_quantity;
        else
            this.itemQuantity = value;
    }


    public bool HasSlot()
    {
        return this != null;
    }

    public bool HasItem()
    {
        return ItemData != null;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ItemData, itemQuantity);
    }
}

