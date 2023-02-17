using UnityEngine;
using UnityEngine.EventSystems;
using MyGame.Ultilities;
using UnityEditor;
using System;

[RequireComponent(typeof(CraftingTableManager))]
public class UICraftingTableManager : Singleton<UICraftingTableManager>
{
    [Header("References")]
    public Player player;
    private PlayerInputHandler playerInput;
    private CraftingTableManager craftingTableManager;
    private ItemInHand itemInHand;
    private UIItemInHand ui_itemInHand;


    [SerializeField] private GameObject[] craftingGridSlot = new GameObject[9];
    [SerializeField] private GameObject outputSlot;
    

    [Header("UI Event Properties")]
    PointerState currentPointerState = PointerState.Null;
    private GameObject currentSlotDrag;
    private GameObject startingSlotDrag;

    private GameObject currentSlotClicked;
    private GameObject lastSlotClicked;

    [SerializeField] float clickToPressInterval = 0.3f;
    float clickToPressTimeCount = 0.0f;


    private void Start()
    {
        craftingTableManager = CraftingTableManager.Instance;
        itemInHand = player.ItemInHand;
        playerInput = player.PlayerInputHandler;
        ui_itemInHand = UIItemInHand.Instance;


        for (int i = 0; i < craftingTableManager.craftingGridData.Length; i++)
        {
            // craftingGridSlot event
            GameObject slotObject = craftingGridSlot[i];
            slotObject.GetComponent<UIItemSlot>().SetIndex(i);
            slotObject.GetComponent<UIItemSlot>().SetData(null);

            Utilities.AddEvent(slotObject, EventTriggerType.PointerDown, (baseEvent) => OnPointerDown(baseEvent, slotObject));
            Utilities.AddEvent(slotObject, EventTriggerType.PointerEnter, delegate { OnEnter(slotObject); });
            Utilities.AddEvent(slotObject, EventTriggerType.PointerEnter, delegate { OnExit(slotObject); });
            /*Utilities.AddEvent(slotObject, EventTriggerType.BeginDrag, (baseEvent) => OnBeginDrag(baseEvent, slotObject));
            Utilities.AddEvent(slotObject, EventTriggerType.EndDrag, (baseEvent) => OnEndDrag(baseEvent, slotObject));*/

            Utilities.AddEvent(slotObject, EventTriggerType.BeginDrag, (baseEvent) => OnBeginDrag(baseEvent, slotObject));
            Utilities.AddEvent(slotObject, EventTriggerType.Drag, (baseEvent) => OnDrag(baseEvent, slotObject));
            Utilities.AddEvent(slotObject, EventTriggerType.EndDrag, (baseEvent) => OnEndDrag(baseEvent, slotObject));   
        }

        UpdateCraftingTableDisplayUI();


        // outputSlot Event
        //outputSlot.GetComponent<UIItemSlot>().Set(null, null);
        Utilities.AddEvent(outputSlot, EventTriggerType.PointerClick, (baseEvent) => OnOutputSlotClick(baseEvent, outputSlot));
        UIManager.Instance.CraftingTableCanvas.SetActive(false);
    }



    #region UPDATE CRAFTINGTABLE DISPLAY UI REGION.
    public void UpdateCraftingTableDisplayUI()
    {
        // Update Crafting grid slot
        for (int i = 0; i < craftingTableManager.craftingGridData.Length; i++)
        {
            UpdateCraftingTableDisplayUIAt(i);
        }

        // Update Output slot
        UpdateOutputSlotCraftingTalbeDisplay();
    }

    public void UpdateCraftingTableDisplayUIAt(int index)
    {
        UIItemSlot uiSlot = craftingGridSlot[index].GetComponent<UIItemSlot>();
        uiSlot.SetData(craftingTableManager.craftingGridData[index]);
 
    }

    private void UpdateOutputSlotCraftingTalbeDisplay()
    {
        UIItemSlot uiSlot = outputSlot.GetComponent<UIItemSlot>();
        uiSlot.SetData(craftingTableManager.outputItemSlot);      
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
            else if (craftingTableManager.HasOutputSlot() && craftingTableManager.outputItemSlot.HasItem())
            {
                bool canPickup = itemInHand.PickupItem(ref craftingTableManager.outputItemSlot);

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
            if (craftingTableManager.HasOutputSlot() && craftingTableManager.outputItemSlot.HasItem())
            {
                bool canPickup = itemInHand.PickupItem(ref craftingTableManager.outputItemSlot);

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
            currentSlotClicked = clickedObject;
            //Debug.Log(GetItemSlotIndex(currentSlotClicked));
            OnRightClick(index);
        }
    }


    private void OnLeftClick(int index)
    {
        if (itemInHand.HasItemData() == false)
        {
            if (craftingTableManager.craftingGridData[index].HasItem() == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                GetItemSlot(index);
            }
        }
        else
        {
            if (craftingTableManager.craftingGridData[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                //craftingTable.craftingGridData[index] = new ItemContainerSlot(itemInHand.itemSlot);
                SwapItemInHandAndCraftingGridSlot(index);
                itemInHand.RemoveItem();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                CombineItemSlotQuantity(index);
            }
        }

        EventManager.GridChanged();
        UpdateCraftingTableDisplayUI();
    }

    private void OnRightClick(int index)
    {
        //if (currentPointerState != PointerState.RightClick) return;

        if (itemInHand.HasItemData() == false)
        {
            if (craftingTableManager.craftingGridData[index].HasItem() == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                HalveItemSlotQuantity(index);
            }
        }
        else
        {
            if (craftingTableManager.craftingGridData[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                craftingTableManager.AddItemIntoCraftingGridAtIndex(index, itemInHand.GetItemData());
                itemInHand.RemoveItem();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if (IsSameItem(itemInHand.GetItemData(), craftingTableManager.GetItem(index)))
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

    private void OnRightPress(int index)
    {
        if (currentPointerState != PointerState.RightPress) return;

        if (itemInHand.HasItemData() == true)
        {

            if (craftingTableManager.HasItem(index) == false && craftingTableManager.HasSlot(index))
            {
                Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                craftingTableManager.AddItemIntoCraftingGridAtIndex(index, itemInHand.GetItemData());
                itemInHand.RemoveItem();

            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if (IsSameItem(itemInHand.GetItemData(), craftingTableManager.GetItem(index)))
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
        if(currentPointerState == PointerState.RightPress)
        {
            currentSlotClicked = clickedObject;
            if (lastSlotClicked != currentSlotClicked)
            {
                lastSlotClicked = currentSlotClicked;             
            }

            //Debug.Log($"OnEnter: {GetItemSlotIndex(currentSlotClicked)}");
            OnRightClick(GetItemSlotIndex(clickedObject));
        }
    }

    public void OnExit(GameObject clickedObject)
    {
        if (currentPointerState != PointerState.Null) return;

        currentSlotClicked = null;
        lastSlotClicked = null;
    }


    public void OnBeginDrag(BaseEventData baseEvent, GameObject clickedObject)
    {
        //Debug.Log("Begin Drag");
        PointerEventData pointerEventData = (PointerEventData)baseEvent;
        if(pointerEventData.pointerId == -2)
        {
            currentPointerState = PointerState.RightPress;
        }
        
    }


    public void OnDrag(BaseEventData baseEvent, GameObject clickedObject)
    {
        //Debug.Log($"Drag: {GetItemSlotIndex(currentSlotClicked)}");
    }

    public void OnEndDrag(BaseEventData baseEvent, GameObject clickedObject)
    {
        //Debug.Log("End Drag");

        PointerEventData pointerEventData = (PointerEventData)baseEvent;
        if (pointerEventData.pointerId == -2)
        {
            currentPointerState = PointerState.Null;
        }  
    }


    
    /*public void OnBeginDrag(BaseEventData baseEvent, GameObject clickedObject)
    {
       
        PointerEventData pointerEventData = (PointerEventData)baseEvent;
        int index = GetItemSlotIndex(clickedObject);
        if (index == -1) return;

        if (clickedObject != null)
        {
            currentSlotDrag = clickedObject;
            startingSlotDrag = clickedObject;
        }




        //Debug.Log("OnBeginDrag");
        if (pointerEventData.pointerId == -1)   // Mouse Left Event
        {
            OnLeftClick(index);
        }


        if (pointerEventData.pointerId == -2)   // Mouse Right Event
        {
            currentPointerState = PointerState.RightPress;
        }

        UpdateCraftingTableDisplayUI();
    }


    public void OnEndDrag(BaseEventData baseEvent, GameObject clickedObject)
    {      
        PointerEventData pointerEventData = (PointerEventData)baseEvent;
        if (pointerEventData.pointerId == -1)
        {
            int index = GetItemSlotIndex(currentSlotDrag);
            if (craftingTable.HasSlot(index) == false) return;

            if (craftingTable.HasItem(index))
            {
                if (IsSameItem(craftingTable.GetItem(index), itemInHand.GetItem()))
                {
                    CombineItemSlotQuantity(index);
                }
                else
                {
                    // Swap slot the first time
                    SwapItemInHandAndCraftingGridSlot(index);


                    // Swap slot the second time if there are no items in this STRARTING SLOT. 
                    int startingSlotIndex = GetItemSlotIndex(startingSlotDrag);
                    if (craftingTable.HasItem(startingSlotIndex) == false)
                    {
                        startingSlotDrag = null;
                        SwapItemInHandAndCraftingGridSlot(startingSlotIndex);
                    }


                }
            }
            else
            {
                SwapItemInHandAndCraftingGridSlot(index);
            }
            UpdateCraftingTableDisplayUI();
        }

        if(pointerEventData.pointerId == -2)
        {
            currentPointerState = PointerState.Null;
        }
    }*/



    private void GetItemSlot(int index)
    {
        var chosenSlot = new ItemSlot(craftingTableManager.craftingGridData[index]);
        craftingTableManager.craftingGridData[index].ClearSlot();
        itemInHand.Set(chosenSlot, new ItemSlotData 
        {
            slotStoredType = StoredType.CraftingTable, 
            slotIndex = index
        });

        ui_itemInHand.UpdateItemInHandUI(this.transform);
    }

    private void HalveItemSlotQuantity(int index)
    {
        if (craftingTableManager.craftingGridData[index].ItemQuantity > 1)
        {
            int splitItemQuantity = craftingTableManager.craftingGridData[index].ItemQuantity / 2;
            craftingTableManager.craftingGridData[index].SetItemQuantity(craftingTableManager.craftingGridData[index].ItemQuantity - splitItemQuantity);

            var chosenSlot = new ItemSlot(craftingTableManager.craftingGridData[index]);
            chosenSlot.SetItemQuantity(splitItemQuantity);
            itemInHand.Set(chosenSlot, new ItemSlotData
            {
                slotStoredType = StoredType.CraftingTable,
                slotIndex = index
            });

            ui_itemInHand.UpdateItemInHandUI();
        }
        else
        {
            GetItemSlot(index);
        }
    }

    public bool IsSameItem(ItemData itemA, ItemData itemB)
    {
        if (itemA == null || itemB == null) return false;
        return itemA == itemB;
    }

    /// <summary>
    /// Method combine amount of item quantity from itemSlotList at index inHandSlot.
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get</param>
    private void CombineItemSlotQuantity(int index)
    {
        if (IsSameItem(craftingTableManager.craftingGridData[index].ItemData, itemInHand.GetItemData()))
        {
            //Debug.Log("Same Object here");
            
            /*int currentItemSlotQuantity = craftingTableManager.craftingGridData[index].itemQuantity;
            craftingTableManager.craftingGridData[index].AddItems(itemInHand.itemSlot.itemQuantity, out int remainder);
            if (remainder > 0)
            {
                itemInHand.itemSlot.SetItemQuantity(remainder);

                //UI_InHandSlot.GetComponent<UISlot>().amountItemInSlotText.text = itemInHand.itemSlot.itemQuantity.ToString();
                itemInHand.uiItemInHand.DisplayItemInHand(this.transform);
            }
            else
            {
                ClearItemInHand();
            }*/

            itemInHand.Set(craftingTableManager.craftingGridData[index].AddItemsFromAnotherSlot(itemInHand.GetSlot()), new ItemSlotData
            {
                slotStoredType = StoredType.CraftingTable,
                slotIndex = index
            });
            ui_itemInHand.UpdateItemInHandUI();
        }
        else
        {
            //Debug.Log("Not same object");
            SwapItemInHandAndCraftingGridSlot(index);
        }
    }
    private void SwapItemInHandAndCraftingGridSlot(int index)
    {
        if (itemInHand.HasItemData() &&
           craftingTableManager.craftingGridData[index].HasItem())
        {
            var itemToSwap_01 = itemInHand.GetSlot();
            var itemToSwap_02 = new ItemSlot(craftingTableManager.craftingGridData[index]);
            craftingTableManager.craftingGridData[index].ClearSlot();
            itemInHand.Set(itemToSwap_02, new ItemSlotData
            {
                slotStoredType = StoredType.CraftingTable,
                slotIndex = index
            });
            craftingTableManager.craftingGridData[index] = itemToSwap_01;

            ui_itemInHand.UpdateItemInHandUI(this.transform);
        }


        if (itemInHand.HasItemData() &&
           craftingTableManager.craftingGridData[index].HasItem() == false)
        {
            var itemToSwap_01 = itemInHand.GetSlot();
            var itemToSwap_02 = new ItemSlot(craftingTableManager.craftingGridData[index]);
            craftingTableManager.craftingGridData[index].ClearSlot();
            itemInHand.Set(itemToSwap_02, new ItemSlotData
            {
                slotStoredType = StoredType.CraftingTable,
                slotIndex = index
            });;
            craftingTableManager.craftingGridData[index] = itemToSwap_01;
        }
    }



    private int GetItemSlotIndex(GameObject itemSlot)
    {
        if (itemSlot == null) return -1;
        return itemSlot.GetComponent<UIItemSlot>().SlotIndex;
    }

    #endregion LOCGIC HANDLER

}
