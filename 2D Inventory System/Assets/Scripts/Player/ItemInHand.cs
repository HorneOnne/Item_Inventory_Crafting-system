using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

/*
    -- EN
    This Script use for handle item when player interact with item in 
    inventory ui like get item.
    
    -- VN
    Script nay dung de xu ly vat pham khi nguoi choi tuong tac voi inventory ui
    giong nhu lay vat pham ra khoi inventory....
    
 */
[RequireComponent(typeof(PlayerInventory))]
public class ItemInHand : MonoBehaviour
{   
    private PlayerInventory inventory;
    public ItemSlot itemSlot;

    [Header("UI Display")]
    public UIItemInHand uiItemInHand;

    private void Start()
    {
        inventory = this.GetComponent<PlayerInventory>(); 
    }


    public ItemData GetItem()
    {
        return itemSlot.itemObject;
    }


    public bool PickupItem(ref ItemSlot itemContainerSlot)
    {
        if (HasItem() == false)
        {
            itemSlot = new ItemSlot(itemContainerSlot);
            itemContainerSlot.ClearSlot();
            uiItemInHand.DisplayItemInHand();
            return true;
        }
        else
        {
            if(itemSlot.itemObject.Equals(itemContainerSlot.itemObject))
            {
                if(itemSlot.TryAddItem(itemContainerSlot) == true)
                {
                    itemContainerSlot = itemSlot.AddItemsFromAnotherSlot(itemContainerSlot);
                    uiItemInHand.DisplayItemInHand();
                    return true;
                }
                else
                {
                    //Debug.Log("The item quantity > item maxquantity");
                    return false;
                }
            }
            else
            {
                //Debug.Log("Not same");
                return false;
            }            
        }
    }

    public bool HasItem()
    {
        return itemSlot.HasItem();
    }


    public ItemSlot GetSlot() => itemSlot;

    public void ClearSlot()
    {
        itemSlot.ClearSlot();
    }


    public void UseItem()
    {
        if (HasItem() == false) return;

        Debug.Log("UseItem");
        //itemSlot.itemObject.Use();
    }


    /// <summary>
    /// substract 1 quantity of item in hand.
    /// </summary>
    public void RemoveItem()
    {
        itemSlot.RemoveItem();
        uiItemInHand.DisplayItemInHand();
    }

}
