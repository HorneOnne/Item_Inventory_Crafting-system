using System;
using UnityEngine;


public class ItemInHand : MonoBehaviour
{
    public static event Action OnItemInHandChanged;


    [SerializeField] private ItemSlot itemSlot;

    [Header("References")]
    private Player player;
    private UIItemInHand uiItemInHand;
    public Item itemObject;


    #region Properties
    public StoredType ItemGetFrom { get; private set; }

    #endregion

    private void Start()
    {
        player = GetComponent<Player>();    
        uiItemInHand = UIItemInHand.Instance;
    }


    public ItemData GetItem()
    {
        return itemSlot.itemObject;
    }

    public void Set(ItemSlot takenSlot, StoredType from)
    {
        itemSlot = takenSlot;
        ItemGetFrom = from;

        OnItemInHandChanged?.Invoke();
    }

    public void SetItemObject(Item item)
    {
        itemObject = item;
    }


    public bool PickupItem(ref ItemSlot itemContainerSlot)
    {
        OnItemInHandChanged?.Invoke();

        if (HasItemData() == false)
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

    public bool HasItemData()
    {
        if (itemSlot == null) 
            return false;

        return itemSlot.HasItem();
    }


    public bool HasItemObject()
    {
        return itemObject != null;
    }


    public ItemSlot GetSlot() => itemSlot;

    public void ClearSlot()
    {
        itemSlot.ClearSlot();
        ItemGetFrom = StoredType.Another;
    }


    public void UseItem()
    {
        if (HasItemData() == false || HasItemObject() == false) return;
        itemObject.Use(player);
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

public enum StoredType
{
    PlayerInventory,
    CraftingTable,
    Another
}