using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MyGame.Ultilities;

[RequireComponent(typeof(CraftingTableData))]
public class UI_DisplayCraftingTable : MonoBehaviour
{
    public static event System.Action OnGridChanged;
    public static event System.Action OnGetOutputItem;

    [Header("Data")]
    [SerializeField] private CraftingTableData craftingTableData;
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
        for (int i = 0; i < craftingTableData.craftingGridData.Length; i++)
        {
            // craftingGridSlot event
            GameObject slotObject = craftingGridSlot[i];
            slotObject.GetComponent<UIItemSlot>().Set(null, null, (short)i);
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
        outputSlot.GetComponent<UIItemSlot>().Set(null, null);
        Utilities.AddEvent(outputSlot, EventTriggerType.PointerClick, (baseEvent) => OnOutputSlotClick(baseEvent, outputSlot));
    }



    #region UPDATE CRAFTINGTABLE DISPLAY UI REGION.
    private void UpdateCraftingTableDisplayUI()
    {
        // Update Crafting grid slot
        for (int i = 0; i < craftingTableData.craftingGridData.Length; i++)
        {
            UpdateCraftingTableDisplayUIAt(i);
        }

        // Update Output slot
        UpdateOutputSlotCraftingTalbeDisplay();
    }

    private void UpdateCraftingTableDisplayUIAt(int index)
    {
        UIItemSlot uiSlot = craftingGridSlot[index].GetComponent<UIItemSlot>();
        if (craftingTableData.craftingGridData[index].HasItem())
        {
            uiSlot.slotImage.sprite = craftingTableData.craftingGridData[index].itemObject.icon;
            uiSlot.slotImage.GetComponent<RectTransform>().SetAsLastSibling();
            uiSlot.amountItemInSlotText.text = craftingTableData.craftingGridData[index].itemQuantity.ToString();
        }
        else
        {
            uiSlot.slotImage.sprite = null;
            uiSlot.slotImage.GetComponent<RectTransform>().SetAsFirstSibling();
            uiSlot.amountItemInSlotText.text = "";
        }
    }

    private void UpdateOutputSlotCraftingTalbeDisplay()
    {
        UIItemSlot uiSlot = outputSlot.GetComponent<UIItemSlot>();
        if (craftingTableData.outputItemSlot != null)
        {
            if (craftingTableData.outputItemSlot.HasItem())
            {
                uiSlot.slotImage.sprite = craftingTableData.outputItemSlot.itemObject.icon;
                uiSlot.amountItemInSlotText.text = craftingTableData.outputItemSlot.itemQuantity.ToString();
                uiSlot.slotImage.GetComponent<RectTransform>().SetAsLastSibling();
            }
            else
            {
                uiSlot.slotImage.sprite = null;
                uiSlot.amountItemInSlotText.text = "";
                uiSlot.slotImage.GetComponent<RectTransform>().SetAsFirstSibling();
            }
        }
        else
        {
            uiSlot.slotImage.sprite = null;
            uiSlot.amountItemInSlotText.text = "";
            uiSlot.slotImage.GetComponent<RectTransform>().SetAsFirstSibling();
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
            if (Input.GetKey(KeyCode.LeftShift))
            {
                QuickGetAllOutputItem();
            }
            else if (craftingTableData.HasOutputSlot() && craftingTableData.outputItemSlot.HasItem())
            {
                bool canPickup = craftingTableData.ItemInHand.PickupItem(ref craftingTableData.outputItemSlot);

                if (canPickup == true)
                {
                    OnGetOutputItem?.Invoke();
                    OnGridChanged?.Invoke();
                    UpdateCraftingTableDisplayUI();

                }              
            }
        }
    }


    public void QuickGetAllOutputItem()
    {
        while(true)
        {
            if (craftingTableData.HasOutputSlot() && craftingTableData.outputItemSlot.HasItem())
            {
                bool canPickup = craftingTableData.ItemInHand.PickupItem(ref craftingTableData.outputItemSlot);

                if (canPickup == true)
                {
                    OnGetOutputItem?.Invoke();
                    OnGridChanged?.Invoke();
                    UpdateCraftingTableDisplayUI();

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
        if (craftingTableData.ItemInHand.HasItem() == false)
        {
            if (craftingTableData.craftingGridData[index].HasItem() == false)
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
            if (craftingTableData.craftingGridData[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                //craftingTable.craftingGridData[index] = new ItemContainerSlot(itemInHand.itemSlot);
                SwapItemInHandAndCraftingGridSlot(index);
                ClearItemInHand();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                CombineItemSlotQuantity(index);
            }
        }

        OnGridChanged?.Invoke();
        UpdateCraftingTableDisplayUI();
    }

    private void OnRightClick(int index)
    {
        //if (currentPointerState != PointerState.RightClick) return;

        if (craftingTableData.ItemInHand.itemSlot.HasItem() == false)
        {
            if (craftingTableData.craftingGridData[index].HasItem() == false)
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
            if (craftingTableData.craftingGridData[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                craftingTableData.AddItemIntoCraftingGridAtIndex(index, craftingTableData.ItemInHand.itemSlot.itemObject);
                craftingTableData.ItemInHand.RemoveItem();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if (IsSameItem(craftingTableData.ItemInHand.GetItem(), craftingTableData.GetItem(index)))
                {
                    //Debug.Log("Same item");
                    bool isSlotNotFull = craftingTableData.AddItem(index);

                    if (isSlotNotFull)
                    {
                        craftingTableData.ItemInHand.RemoveItem();
                    }
                }

            }
        }

        OnGridChanged?.Invoke();
        UpdateCraftingTableDisplayUI();
    }

    private void OnRightPress(int index)
    {
        if (currentPointerState != PointerState.RightPress) return;

        if (craftingTableData.ItemInHand.HasItem() == true)
        {

            if (craftingTableData.HasItem(index) == false && craftingTableData.HasSlot(index))
            {
                Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                craftingTableData.AddItemIntoCraftingGridAtIndex(index, craftingTableData.ItemInHand.itemSlot.itemObject);
                craftingTableData.ItemInHand.RemoveItem();

            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if (IsSameItem(craftingTableData.ItemInHand.GetItem(), craftingTableData.GetItem(index)))
                {
                    //Debug.Log("Same item");
                    bool isSlotNotFull = craftingTableData.AddItem(index);

                    if (isSlotNotFull)
                    {
                        craftingTableData.ItemInHand.RemoveItem();
                    }

                }
            }
        }

        OnGridChanged?.Invoke();
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




    private void ClearItemInHand()
    {
        craftingTableData.ItemInHand.itemSlot.ClearSlot();
    }


    private void GetItemSlot(int index)
    {
        var chosenSlot = new ItemSlot(craftingTableData.craftingGridData[index]);
        craftingTableData.craftingGridData[index].ClearSlot();
        craftingTableData.ItemInHand.itemSlot = chosenSlot;

        craftingTableData.ItemInHand.uiItemInHand.DisplayItemInHand(this.transform);
    }

    private void HalveItemSlotQuantity(int index)
    {
        if (craftingTableData.craftingGridData[index].itemQuantity > 1)
        {
            int splitItemQuantity = craftingTableData.craftingGridData[index].itemQuantity / 2;
            craftingTableData.craftingGridData[index].SetItemQuantity(craftingTableData.craftingGridData[index].itemQuantity - splitItemQuantity);

            var chosenSlot = new ItemSlot(craftingTableData.craftingGridData[index]);
            chosenSlot.SetItemQuantity(splitItemQuantity);
            craftingTableData.ItemInHand.itemSlot = chosenSlot;

            craftingTableData.ItemInHand.uiItemInHand.DisplayItemInHand();
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
        if (IsSameItem(craftingTableData.craftingGridData[index].itemObject, craftingTableData.ItemInHand.itemSlot.itemObject))
        {
            //Debug.Log("Same Object here");
            
            /*int currentItemSlotQuantity = craftingTableData.craftingGridData[index].itemQuantity;
            craftingTableData.craftingGridData[index].AddItems(craftingTableData.ItemInHand.itemSlot.itemQuantity, out int remainder);
            if (remainder > 0)
            {
                craftingTableData.ItemInHand.itemSlot.SetItemQuantity(remainder);

                //UI_InHandSlot.GetComponent<UISlot>().amountItemInSlotText.text = itemInHand.itemSlot.itemQuantity.ToString();
                craftingTableData.ItemInHand.uiItemInHand.DisplayItemInHand(this.transform);
            }
            else
            {
                ClearItemInHand();
            }*/

            craftingTableData.ItemInHand.itemSlot = craftingTableData.craftingGridData[index].AddItemsFromAnotherSlot(craftingTableData.ItemInHand.GetSlot());
            craftingTableData.ItemInHand.uiItemInHand.DisplayItemInHand();
        }
        else
        {
            //Debug.Log("Not same object");
            SwapItemInHandAndCraftingGridSlot(index);
        }
    }
    private void SwapItemInHandAndCraftingGridSlot(int index)
    {
        if (craftingTableData.ItemInHand.itemSlot.HasItem() &&
           craftingTableData.craftingGridData[index].HasItem())
        {
            var itemToSwap_01 = craftingTableData.ItemInHand.itemSlot;
            var itemToSwap_02 = new ItemSlot(craftingTableData.craftingGridData[index]);
            craftingTableData.craftingGridData[index].ClearSlot();
            craftingTableData.ItemInHand.itemSlot = itemToSwap_02;
            craftingTableData.craftingGridData[index] = itemToSwap_01;

            craftingTableData.ItemInHand.uiItemInHand.DisplayItemInHand(this.transform);
        }


        if (craftingTableData.ItemInHand.itemSlot.HasItem() &&
           craftingTableData.craftingGridData[index].HasItem() == false)
        {
            var itemToSwap_01 = craftingTableData.ItemInHand.itemSlot;
            var itemToSwap_02 = new ItemSlot(craftingTableData.craftingGridData[index]);
            craftingTableData.craftingGridData[index].ClearSlot();
            craftingTableData.ItemInHand.itemSlot = itemToSwap_02;
            craftingTableData.craftingGridData[index] = itemToSwap_01;
        }
    }



    private int GetItemSlotIndex(GameObject itemSlot)
    {
        if (itemSlot == null) return -1;
        return itemSlot.GetComponent<UIItemSlot>().SlotIndex;
    }

    #endregion LOCGIC HANDLER

}
