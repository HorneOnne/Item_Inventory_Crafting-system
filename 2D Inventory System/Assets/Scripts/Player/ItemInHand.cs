using UnityEngine;


public class ItemInHand : MonoBehaviour
{
    public ItemSlot itemSlot;

    [Header("References")]
    private UIItemInHand uiItemInHand;

    private void Start()
    {
        uiItemInHand = UIItemInHand.Instance;
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
