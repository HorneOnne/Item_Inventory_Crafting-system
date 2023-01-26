using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(ItemInHand))]
public class PlayerInventory : MonoBehaviour
{
    // Events
    public static event System.Action OnInventoryUpdate;

    [Header("Inventory Settings")]
    public int capacity;
    public List<ItemSlot> slotList = new List<ItemSlot>();


    [Header("References")]
    [SerializeField] private ItemInHand itemInHand;

    public ItemInHand ItemInHand { get { return itemInHand; } }


    private void Start()
    {
        itemInHand = GetComponent<ItemInHand>();

        for (int i = 0; i < capacity; i++)
        {
            slotList.Add(new ItemSlot());
        }
    }

    public ItemData GetItem(int slotIndex)
    {
        if(HasSlot(slotIndex))
        {
            return slotList[slotIndex].itemObject;
        }
        return null;
    }

    public bool AddItem(ItemData itemObject)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (slotList[i].itemObject != null)
            {
                if (itemObject == slotList[i].itemObject)
                {
                    if(slotList[i].IsFullSlot() == false)
                    {
                        slotList[i].AddNewItem(itemObject);
                        OnInventoryUpdate?.Invoke();
                        return true;
                    }
                   
                }
            }
        }

        for (int i = 0; i < capacity; i++)
        {
            if (slotList[i].itemObject == null)
            {
                slotList[i].AddNewItem(itemObject);
                OnInventoryUpdate?.Invoke();
                return true;
            }
        }

        OnInventoryUpdate?.Invoke();
        return false;
    }

    public bool AddItem(int index)
    {
        bool isSlotNotFull = slotList[index].AddItem();
        OnInventoryUpdate?.Invoke();

        return isSlotNotFull;
    }



    // NEW INTERACTIVE METHOD
    // ======================
    public void AddNewItemIntoInventoryAtIndex(int index, ItemData item)
    {
        slotList[index].AddNewItem(item);
        OnInventoryUpdate?.Invoke();
    }

    public void RemoveItemFromInventoryAtIndex(int index)
    {
        slotList[index].RemoveItem();
        OnInventoryUpdate?.Invoke();
    }


    public bool HasSlot(int slotIndex)
    {
        try
        {
            slotList[slotIndex].HasItem();
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
            return slotList[slotIndex].HasItem();
        }
        return false;
    }
}
