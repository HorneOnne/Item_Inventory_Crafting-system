using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInHand : MonoBehaviour
{
    public static event Action OnItemInHandChanged;


    [SerializeField] private ItemSlot itemSlot;

    [Header("References")]
    private Player player;
    private UIItemInHand uiItemInHand;
    private Item itemObject;


    [Header("Settings")]
    [Tooltip("Prevent using a selected item for the first time.")]
    private bool firstGet = true;

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
        return itemSlot.ItemObject;
    }

    public void Set(ItemSlot takenSlot, StoredType from)
    {
        itemSlot = takenSlot;
        ItemGetFrom = from;

        firstGet = true;
        OnItemInHandChanged?.Invoke();
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
            if(itemSlot.ItemObject.Equals(itemContainerSlot.ItemObject))
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

        OnItemInHandChanged?.Invoke();
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
        uiItemInHand.DisplayItemInHand();

        OnItemInHandChanged?.Invoke();
    }


    private bool IsMouseOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // mouse click is on a UI element
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