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

    /*public bool AddItems(int amount, out int remainder)
    {
        remainder = 0;
        if (HasItem() == false) return false;

        itemQuantity += amount;
        if (itemQuantity > this.itemObject.max_quantity)
        {
            remainder = itemQuantity - this.itemObject.max_quantity;
            itemQuantity = this.itemObject.max_quantity;
            return false;
        }
        return true;
    }*/


    public ItemSlot AddItemsFromAnotherSlot(ItemSlot addedSlot)
    {
        /*if (HasItem() == false) return addedSlot;
      
        itemQuantity += addedSlot.ItemQuantity;
        if (ItemQuantity > this.itemObject.max_quantity)
        {
            addedSlot.itemQuantity = this.ItemQuantity - this.itemObject.max_quantity;
            this.ItemQuantity = this.itemObject.max_quantity;

            return new ItemSlot(addedSlot.itemObject, addedSlot.itemQuantity);
        }
        else
        {
            addedSlot.ClearSlot();
        }

        return new ItemSlot(addedSlot);*/
        
        
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

    /*public ItemSlot TransferItemToAnotherSlot(ItemSlot slotToTransfer)
    {
        Debug.Log("TransferItemToAnotherSlot");

        if (HasItem() == false) return this;

        int totalOfItemQuantity = itemQuantity + slotToTransfer.itemQuantity;
        Debug.Log($"{itemQuantity}\t{slotToTransfer.itemQuantity}\t total: {totalOfItemQuantity}");
        if (totalOfItemQuantity > slotToTransfer.itemObject.max_quantity)
        {
            slotToTransfer.ItemQuantity = this.itemObject.max_quantity;
            this.itemQuantity = totalOfItemQuantity - this.itemObject.max_quantity;

            return new ItemSlot(this.itemObject, this.itemQuantity);
        }
        else
        {
            slotToTransfer.ItemQuantity = totalOfItemQuantity;
            this.ClearSlot();
        }

        return new ItemSlot(this);
    }*/


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




    /*// NEW 
    // ===============================================================
    public bool IsSameItem(ItemScriptableObject itemA, ItemScriptableObject itemB)
    {
        if (itemA == null || itemB == null) return false;
        return itemA == itemB;
    }

    /// <summary>
    /// This method get itemSlot object then return itemSlot object's index in itemSlotList
    /// </summary>
    /// <param name="itemSlot"></param>
    /// <returns>Index itemSlot at itemSlotList</returns>
    private int GetItemContainerSlotIndex(GameObject itemSlot)
    {
        if (itemSlot == null) return -1;
        return itemSlot.GetComponent<UISlot>().SlotIndex;
    }

    /// <summary>
    /// This Method Swap InHandSlot and Slot in inventory at index
    /// </summary>
    /// <param name="index">index for itemSlotList</param>
    private void SwapItemContainerSlot(ref ItemContainerSlot slotA, ref ItemContainerSlot slotB)
    {
        if (slotA.HasItem() && slotB.HasItem())
        {
            var itemSlotToSwap_01 = slotA;
            var itemSlotToSwap_02 = new ItemContainerSlot(slotB);
            slotB.ClearSlot();
            slotA = itemSlotToSwap_02;
            slotB = itemSlotToSwap_01;

            //slotA.uiItemInHand.DisplayItemInHand();
        }


        if (slotA.HasItem() && slotB.HasItem() == false)
        {
            var itemSlotToSwap_01 = slotA;
            var itemSlotToSwap_02 = new ItemContainerSlot(slotB);
            slotB.ClearSlot();
            slotA = itemSlotToSwap_02;
            slotB = itemSlotToSwap_01;
        }
    }




    /// <summary>
    /// Method remove itemSlot in itemSlotList and put it in "itemInHand".
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get.</param>
    private void GetItemSlot(ref ItemContainerSlot slotA)
    {
        var chosenSlot = new ItemContainerSlot(slotA);
        slotA.ClearSlot();
        inventoryA.ItemInHand.itemSlot = chosenSlot;

        inventory.ItemInHand.uiItemInHand.DisplayItemInHand();
    }

    /// <summary>
    /// Method halve amount of item quantity from itemSlotList at index and put it in "itemInHand".
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get.</param>
    private void HalveItemSlotQuantity(int index)
    {
        if (slotB.itemQuantity > 1)
        {
            int splitItemQuantity = slotB.itemQuantity / 2;
            slotB.SetItemQuantity(slotB.itemQuantity - splitItemQuantity);

            var chosenSlot = new ItemContainerSlot(slotB);
            chosenSlot.SetItemQuantity(splitItemQuantity);
            slotA.itemSlot = chosenSlot;

            slotA.uiItemInHand.DisplayItemInHand();
        }
        else
        {
            GetItemSlot(index);
        }
    }

    /// <summary>
    /// Method combine amount of item quantity from itemSlotList at index inHandSlot.
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get</param>
    private void CombineItemSlotQuantity(int index)
    {
        //Debug.Log("CombineItemSlotQuantity");
        if (IsSameItem(slotB.itemObject, slotA.itemSlot.itemObject))
        {
            //Debug.Log("Same Object");
            int currentItemSlotQuantity = slotB.itemQuantity;
            slotB.AddItems(slotA.itemSlot.itemQuantity, out int remainder);

            if (remainder > 0)
            {
                slotA.itemSlot.SetItemQuantity(remainder);

                //UI_InHandSlot.GetComponent<UISlot>().amountItemInSlotText.text = itemInHand.itemSlot.itemQuantity.ToString();
                slotA.uiItemInHand.DisplayItemInHand();
            }
            else
            {
                ClearItemInHand();
            }
        }
        else
        {
            //Debug.Log("Not same object");
            SwapItemInHandAndInventorySlot(index);
        }
    }*/
}

