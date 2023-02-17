using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInHand : MonoBehaviour
{
    //public static event Action OnItemInHandChanged;


    [SerializeField] private ItemSlot itemSlot;

    [Header("References")]
    private Player player;
    private UIItemInHand uiItemInHand;
    private Item itemObject;


    [Header("Settings")]
    [Tooltip("Prevent using a selected item for the first time.")]
    private bool firstGet = true;

    #region Properties
    //public StoredType ItemGetFrom { get; private set; }
    public ItemSlotData ItemGetFrom { get; private set; }

    #endregion

    private void Start()
    {
        player = GetComponent<Player>();    
        uiItemInHand = UIItemInHand.Instance;
    }





    public ItemData GetItemData()
    {
        return itemSlot.ItemData;
    }

    public void Set(ItemSlot takenSlot, ItemSlotData from)
    {
        itemSlot = takenSlot;
        ItemGetFrom = from;

        firstGet = true;
        EventManager.ItemInHandChanged();
    }

    public void SetItemObject(Item item)
    {
        itemObject = item;      
    }

    public Item GetItemObject()
    {
        return itemObject;
    }


    public bool PickupItem(ref ItemSlot itemContainerSlot)
    {
        bool canPickupItem;
        

        if (HasItemData() == false)
        {
            itemSlot = new ItemSlot(itemContainerSlot);
            itemContainerSlot.ClearSlot();
            uiItemInHand.UpdateItemInHandUI();
            canPickupItem = true;
        }
        else
        {
            if(itemSlot.ItemData.Equals(itemContainerSlot.ItemData))
            {
                if(itemSlot.TryAddItem(itemContainerSlot) == true)
                {
                    itemContainerSlot = itemSlot.AddItemsFromAnotherSlot(itemContainerSlot);
                    uiItemInHand.UpdateItemInHandUI();
                    canPickupItem = true;
                }
                else
                {
                    //Debug.Log("The item quantity > item maxquantity");
                    canPickupItem = false;
                }
            }
            else
            {
                //Debug.Log("Not same");
                canPickupItem = false;
            }            
        }

        EventManager.ItemInHandChanged();
        return canPickupItem;
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
        ItemGetFrom = new ItemSlotData
        {
            slotStoredType = StoredType.Another,
            slotIndex = -1
        };

        EventManager.ItemInHandChanged();
    }


    public bool UseItem()
    {
        if (HasItemData() == false || HasItemObject() == false) return false;
        if(IsMouseOverUI() == true) return false;

        if (firstGet)
        {
            firstGet = false;
            return false;
        }

        return itemObject.Use(player);
    }


    /// <summary>
    /// substract 1 quantity of item in hand.
    /// </summary>
    public void RemoveItem()
    {
        itemSlot.RemoveItem();
        uiItemInHand.UpdateItemInHandUI();

        EventManager.ItemInHandChanged();
    }


    private bool IsMouseOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // mouse click is on a UI element.
            return true;
        }
        else
        {
            // mouse click is not on a UI element
            return false;
        }
    }
}

public enum StoredType
{
    PlayerInventory,
    ChestInventory,
    CraftingTable,
    Another
}

public struct ItemSlotData
{
    public StoredType slotStoredType;
    public int slotIndex;
}