using UnityEngine;
using UnityEngine.EventSystems;
using MyGame.Ultilities;
using UnityEditor;
using System;
using System.Reflection;

[RequireComponent(typeof(CraftingTable))]
public class UICraftingTable : Singleton<UICraftingTable>
{
    [Header("References")]
    public Player player;
    private PlayerInputHandler playerInput;
    private CraftingTable craftingTableManager;
    private ItemInHand itemInHand;


    [SerializeField] private GameObject[] craftingGridSlot = new GameObject[9];
    [SerializeField] private GameObject outputSlot;
    

    // Cache
    private bool handHasItem;
    private bool slotHasItem;


    private void Start()
    {
        craftingTableManager = CraftingTable.Instance;
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
        UpdateOutputSlotCraftingTalbeUI();



 



        
    }

    public void UpdateCraftingTableDisplayUIAt(int index)
    {
        UIItemSlot uiSlot = craftingGridSlot[index].GetComponent<UIItemSlot>();

        var itemSlot = craftingTableManager.GetItemInputSlotAt(index);

        if(itemSlot != null && itemSlot.HasItem())
        {
            uiSlot.SetData(craftingTableManager.GetItemInputSlotAt(index), 1.0f);
        }
        else
        {
            uiSlot.SetData(craftingTableManager.craftingSuggestionInputSlots[index], 0.3f);
        }
        
    }

    private void UpdateOutputSlotCraftingTalbeUI()
    {
        UIItemSlot uiSlot = outputSlot.GetComponent<UIItemSlot>();
        uiSlot.SetData(craftingTableManager.craftingOutputSlot, 1.0f);

        var itemSlot = craftingTableManager.craftingOutputSlot;

        if (itemSlot != null && itemSlot.HasItem())
        {
            uiSlot.SetData(craftingTableManager.craftingOutputSlot, 1.0f);
        }
        else
        {
            uiSlot.SetData(craftingTableManager.craftingSuggestionOutputSlot, 0.3f);
        }
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
            else if (craftingTableManager.HasOutputSlot() && craftingTableManager.craftingOutputSlot.HasItem())
            {
                var outputItemSlot = craftingTableManager.craftingOutputSlot;
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
            if (craftingTableManager.HasOutputSlot() && craftingTableManager.craftingOutputSlot.HasItem())
            {
                var outputItemSlot = craftingTableManager.craftingOutputSlot;
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
        slotHasItem = craftingTableManager.GetItemInputSlotAt(index).HasItem();

        if (handHasItem == false)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                itemInHand.Swap(ref craftingTableManager.crafintInputSlots, index, StoredType.CraftingTable, true);
            }
        }
        else
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                itemInHand.Swap(ref craftingTableManager.crafintInputSlots, index, StoredType.CraftingTable, true);
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                bool isSameItem = ItemData.IsSameItem(craftingTableManager.crafintInputSlots[index].ItemData, itemInHand.GetItemData());
                if (isSameItem)
                {
                    ItemSlot remainItems = craftingTableManager.crafintInputSlots[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                    itemInHand.Set(remainItems, index, StoredType.CraftingTable, true);
                }
                else
                {
                    itemInHand.Swap(ref craftingTableManager.crafintInputSlots, index, StoredType.CraftingTable, true);
                }
            }
        }

        EventManager.GridChanged();
        UpdateCraftingTableDisplayUI();
    }

    private void OnRightClick(int index)
    {
        handHasItem = itemInHand.HasItemData();
        slotHasItem = craftingTableManager.GetItemInputSlotAt(index).HasItem();

        if (handHasItem == false)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                itemInHand.SplitItemSlotQuantityInInventoryAt(ref craftingTableManager.crafintInputSlots, index);
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
        slotHasItem = craftingTableManager.crafintInputSlots[index].HasItem();

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
        slotHasItem = craftingTableManager.GetItemInputSlotAt(index).HasItem();

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
