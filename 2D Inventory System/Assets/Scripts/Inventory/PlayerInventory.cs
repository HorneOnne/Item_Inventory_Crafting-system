using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Events
    public static event System.Action OnInventoryUpdate;

    [Header("References")]
    private Player player;
    private ItemInHand itemInHand; 


    [Header("Inventory Settings")]
    public int capacity;
    public List<ItemSlot> inventory = new List<ItemSlot>();


    private void Start()
    {
        player = GetComponent<Player>();
        itemInHand = player.ItemInHand;

        for (int i = 0; i < capacity; i++)
        {
            inventory.Add(new ItemSlot());
        }
    }

    public ItemData GetItem(int slotIndex)
    {
        if(HasSlot(slotIndex))
        {
            return inventory[slotIndex].itemObject;
        }
        return null;
    }


    public bool AddItem(int index)
    {
        bool isSlotNotFull = inventory[index].AddItem();
        OnInventoryUpdate?.Invoke();

        return isSlotNotFull;
    }


    public void AddNewItemIntoInventoryAtIndex(int index, ItemData item)
    {
        inventory[index].AddNewItem(item);
        OnInventoryUpdate?.Invoke();
    }

    public void RemoveItemFromInventoryAtIndex(int index)
    {
        inventory[index].RemoveItem();
        OnInventoryUpdate?.Invoke();
    }


    public bool HasSlot(int slotIndex)
    {
        try
        {
            inventory[slotIndex].HasItem();
        }
        catch
        {
            return false;
        }

        return true;
    }


    public bool HasItem(int slotIndex)
    {
        if(HasSlot(slotIndex))
        {
            return inventory[slotIndex].HasItem();
        }
        return false;
    }


    public void StackItem()
    {
        if (itemInHand.GetItem() == null) return;
        Dictionary<int, int> dict = new Dictionary<int, int>();
        Dictionary<int, int> sortedDict = new Dictionary<int, int>();

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemObject == itemInHand.GetItem())
            {
                dict.Add(i, inventory[i].ItemQuantity);
            }
        }

        // Use OrderBy to sort the dictionary by value
        sortedDict = dict.OrderBy(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
     
        foreach(var e in sortedDict)
        {
            itemInHand.GetSlot().AddItemsFromAnotherSlot(inventory[e.Key]);
            UIItemInHand.Instance.DisplayItemInHand();
            UIPlayerInventory.Instance.UpdateInventoryUIAt(e.Key);
        }
    }

    public int? FindArrowSlotIndex()
    {
        for(int i = 0; i < inventory.Count;i++)
        {
            if (inventory[i].HasItem() == false)
                continue;

            if (inventory[i].GetItemType() == ItemType.Arrow)
                return i;
        }

        return null;
    }
}
