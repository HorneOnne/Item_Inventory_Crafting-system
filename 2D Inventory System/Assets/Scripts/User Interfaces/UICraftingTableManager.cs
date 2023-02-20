using UnityEngine;
using UnityEngine.EventSystems;
using MyGame.Ultilities;
using UnityEditor;
using System;
using System.Reflection;

[RequireComponent(typeof(CraftingTableManager))]
public class UICraftingTableManager : Singleton<UICraftingTableManager>
{
    [Header("References")]
    public Player player;
    private PlayerInputHandler playerInput;
    private CraftingTableManager craftingTableManager;
    private ItemInHand itemInHand;


    [SerializeField] private GameObject[] craftingGridSlot = new GameObject[9];
    [SerializeField] private GameObject outputSlot;
    

    // Cache
    private bool handHasItem;
    private bool slotHasItem;


    private void Start()
    {
        craftingTableManager = CraftingTableManager.Instance;
        itemInHand = player.ItemInHand;
        playerInput = player.PlayerInputHandler;


        for (int i = 0; i < craftingTableManager.GridLength; i++)
        {
            // craftingGridSlot event
            GameObject slotObject = craftingGridSlot[i];
            slotObject.GetComponent<UIItemSlot>().SetIndex(i);
            slotObject.GetComponent<UIItemSlot>().SetData(null);
            Utilities.AddEvent(slotObject, EventTriggerType.PointerDown, (baseEvent) => OnPointerDown(baseEvent, slotObject));
            Utilities.AddEvent(slotObject, EventTriggerType.PointerEnter, delegate { OnEnter(slotObject); });
            
        }

        UpdateCraftingTableDisplayUI();


        // outputSlot Event
        Utilities.AddEvent(outputSlot, EventTriggerType.PointerClick, (baseEvent) => OnOutputSlotClick(baseEvent, outputSlot));
        UIManager.Instance.CraftingTableCanvas.SetActive(false);
    }



    #region UPDATE CRAFTINGTABLE DISPLAY UI REGION.
    public void UpdateCraftingTableDisplayUI()
    {
        // Update Crafting grid slot
        for (int i = 0; i < craftingTableManager.GridLength; i++)
        {
            UpdateCraftingTableDisplayUIAt(i);
        }

        // Update Output slot
        UpdateOutputSlotCraftingTalbeDisplay();
    }

    public void UpdateCraftingTableDisplayUIAt(int index)
    {
        UIItemSlot uiSlot = craftingGridSlot[index].GetComponent<UIItemSlot>();
        uiSlot.SetData(craftingTableManager.GetItemIntputSlotAt(index));
 
    }

    private void UpdateOutputSlotCraftingTalbeDisplay()
    {
        UIItemSlot uiSlot = outputSlot.GetComponent<UIItemSlot>();
        uiSlot.SetData(craftingTableManager.outputSlot);      
    }
    #endregion UPDATE CRAFTINGTABLE DISPLAY UI REGION.




    #region LOGIC HANDLER
    // LOGIC 
    // ===================================================================
    public void OnOutputSlotClick(BaseEventData baseEvent, GameObject clickedObject)
    {
        PointerEventData pointerEventData = (PointerEventData)baseEvent;
 
        if (pointerEventData.pointerId == -1)   // Mouse Left Event
        {
            if (playerInput.PressUtilityKeyInput)
            {
                QuickGetAllOutputItem();
            }
            else if (craftingTableManager.HasOutputSlot() && craftingTableManager.outputSlot.HasItem())
            {
                var outputItemSlot = craftingTableManager.outputSlot;
                bool canPickup = itemInHand.PickupItem(ref outputItemSlot);

                if (canPickup == true)
                {
                    EventManager.GetOutputItem();
                    EventManager.GridChanged();
                    UpdateCraftingTableDisplayUI();
                }        
               
            }
        }
    }


    public void QuickGetAllOutputItem()
    {
        while(true)
        {
            if (craftingTableManager.HasOutputSlot() && craftingTableManager.outputSlot.HasItem())
            {
                var outputItemSlot = craftingTableManager.outputSlot;
                bool canPickup = itemInHand.PickupItem(ref outputItemSlot);

                if (canPickup == true)
                {
                    EventManager.GetOutputItem();
                    EventManager.GridChanged();
                    UpdateCraftingTableDisplayUI();

                    Debug.Log("Pick up all item");
                }
                else
                {
                    break;
                }
            }
            else
            { 
                break;
           
            }
        }
    }


    public void OnPointerDown(BaseEventData baseEvent, GameObject clickedObject)
    {
        Debug.Log("OnPointerDown");
        PointerEventData pointerEventData = (PointerEventData)baseEvent;
        int index = GetItemSlotIndex(clickedObject);
        if (index == -1) return;

        if (pointerEventData.pointerId == -1)   // Mouse Left Event
        {
            OnLeftClick(index);
        }


        if (pointerEventData.pointerId == -2)   // Mouse Right Event
        {
            OnRightClick(index);
        }
    }


    private void OnLeftClick(int index)
    {
        handHasItem = itemInHand.HasItemData();
        slotHasItem = craftingTableManager.GetItemIntputSlotAt(index).HasItem();

        if (handHasItem == false)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                itemInHand.Swap(ref craftingTableManager.inputSlots, index, StoredType.CraftingTable, true);
            }
        }
        else
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                itemInHand.Swap(ref craftingTableManager.inputSlots, index, StoredType.CraftingTable, true);
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                bool isSameItem = ItemData.IsSameItem(craftingTableManager.inputSlots[index].ItemData, itemInHand.GetItemData());
                if (isSameItem)
                {
                    ItemSlot remainItems = craftingTableManager.inputSlots[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                    itemInHand.Set(remainItems, index, StoredType.CraftingTable, true);
                }
                else
                {
                    itemInHand.Swap(ref craftingTableManager.inputSlots, index, StoredType.CraftingTable, true);
                }
            }
        }

        EventManager.GridChanged();
        UpdateCraftingTableDisplayUI();
    }

    private void OnRightClick(int index)
    {
        handHasItem = itemInHand.HasItemData();
        slotHasItem = craftingTableManager.GetItemIntputSlotAt(index).HasItem();

        if (handHasItem == false)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                itemInHand.SplitItemSlotQuantityInInventoryAt(ref craftingTableManager.inputSlots, index);
            }
        }
        else
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                craftingTableManager.AddNewItemAt(index, itemInHand.GetItemData());
                itemInHand.RemoveItem();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                if (ItemData.IsSameItem(itemInHand.GetItemData(), craftingTableManager.GetItem(index)))
                {
                    bool isSlotNotFull = craftingTableManager.AddItem(index);

                    if (isSlotNotFull)
                    {
                        itemInHand.RemoveItem();
                    }
                }

            }
        }

        EventManager.GridChanged();
        UpdateCraftingTableDisplayUI();
    }

    private void OnRightPress(int index)
    {
        handHasItem = itemInHand.HasItemData();
        slotHasItem = craftingTableManager.inputSlots[index].HasItem();

        if (handHasItem == true)
        {

            if (slotHasItem == false && craftingTableManager.HasSlot(index))
            {
                Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                craftingTableManager.AddNewItemAt(index, itemInHand.GetItemData());
                itemInHand.RemoveItem();

            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if (ItemData.IsSameItem(itemInHand.GetItemData(), craftingTableManager.GetItem(index)))
                {
                    //Debug.Log("Same item");
                    bool isSlotNotFull = craftingTableManager.AddItem(index);

                    if (isSlotNotFull)
                    {
                        itemInHand.RemoveItem();
                    }

                }
            }
        }

        EventManager.GridChanged();
        UpdateCraftingTableDisplayUI();
    }


    public void OnEnter(GameObject clickedObject)
    {      
        if(player.PlayerInputHandler.CurrentMouseState != PointerState.RightPressAfterWait)
        {
            //OnRightClick(GetItemSlotIndex(clickedObject));
            return;
        }

        int index = GetItemSlotIndex(clickedObject);
        handHasItem = itemInHand.HasItemData();
        slotHasItem = craftingTableManager.GetItemIntputSlotAt(index).HasItem();

        if (handHasItem == true)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                craftingTableManager.AddNewItemAt(index, itemInHand.GetItemData());
                itemInHand.RemoveItem();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                if (ItemData.IsSameItem(itemInHand.GetItemData(), craftingTableManager.GetItem(index)))
                {
                    bool isSlotNotFull = craftingTableManager.AddItem(index);

                    if (isSlotNotFull)
                    {
                        itemInHand.RemoveItem();
                    }
                }

            }
        }
 
        EventManager.GridChanged();
        UpdateCraftingTableDisplayUI();
    }


    private int GetItemSlotIndex(GameObject itemSlot)
    {
        if (itemSlot == null) return -1;
        return itemSlot.GetComponent<UIItemSlot>().SlotIndex;
    }

    #endregion LOCGIC HANDLER

}
