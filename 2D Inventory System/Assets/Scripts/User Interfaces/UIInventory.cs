using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MyGame.Ultilities;
using System;

public class UIInventory : MonoBehaviour
{
    [Header("Inventory References")]
    public PlayerInventory inventory;
    

    [Header("Containers")]
    public List<GameObject> itemSlotList;

    [Header("UI")]
    public GameObject itemSlotPrefab;

    [Header("UI Event Properties")]
    private GameObject currentSlotDrag;
    private GameObject startingSlotDrag;
    private GameObject currentSlotClicked;

    // right press
    [SerializeField] float pressIntervalTime = 1.0f;
    private float pressIntervalTimeCount = 0.0f;

    [SerializeField] float clickToPressInterval = 0.3f;
    float clickToPressTimeCount = 0.0f;


    // =================================
    PointerState currentPointerState = PointerState.Null;

    private void OnEnable()
    {
        PlayerInventory.OnInventoryUpdate += UpdateInventoryUI;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryUpdate -= UpdateInventoryUI;
    }


    private void Start()
    {
        for (int i = 0; i < inventory.capacity; i++)
        {
            GameObject slotObject = Instantiate(itemSlotPrefab, this.transform);
            slotObject.GetComponent<UIItemSlot>().Set(null, null, (short)i);
            Utilities.AddEvent(slotObject, EventTriggerType.PointerClick, (baseEvent) => OnClick(baseEvent, slotObject));
            Utilities.AddEvent(slotObject, EventTriggerType.PointerEnter, delegate { OnEnter(slotObject); });
            Utilities.AddEvent(slotObject, EventTriggerType.PointerExit, delegate { OnExit(slotObject); });
            Utilities.AddEvent(slotObject, EventTriggerType.BeginDrag, (baseEvent) => OnBeginDrag(baseEvent, slotObject));
            Utilities.AddEvent(slotObject, EventTriggerType.EndDrag, (baseEvent) => OnEndDrag(baseEvent, slotObject));
                     
            itemSlotList.Add(slotObject);
        }

        // Update Inventory UI at the first time when start game.
        Invoke("UpdateInventoryUI", .1f);
    }


    
    private void Update()
    {
        currentPointerState = GetPointerState();


        if (inventory.ItemInHand.HasItem())
        {
            if (Input.GetMouseButton(1) && currentSlotClicked != null)
            {
                Utilities.InvokeMethodByInterval(() => OnRightPress(GetItemSlotIndex(currentSlotClicked)), pressIntervalTime, ref pressIntervalTimeCount);
            }
        }
    }

    private PointerState GetPointerState()
    {
        // Mosue Press
        if (Input.GetMouseButtonDown(1))
        {
            return PointerState.RightClick;
        }
        if (Input.GetMouseButton(1))
        {
            clickToPressTimeCount += Time.deltaTime;
            if (clickToPressTimeCount > clickToPressInterval)
                return PointerState.RightPress;
        }
        if (Input.GetMouseButtonUp(1))
        {
            clickToPressTimeCount = 0f;
            return PointerState.Null;
        }


        return currentPointerState;
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventory.capacity; i++)
        {
            UpdateInventoryUIAt(i);
        }
    }

    private void UpdateInventoryUIAt(int index)
    {
        UIItemSlot uiSlot = itemSlotList[index].GetComponent<UIItemSlot>();
        if (inventory.slotList[index].HasItem())
        {
            uiSlot.slotImage.sprite = inventory.slotList[index].itemObject.icon;
            uiSlot.slotImage.GetComponent<RectTransform>().SetAsLastSibling();
            uiSlot.amountItemInSlotText.text = inventory.slotList[index].itemQuantity.ToString();
        }
        else
        {
            uiSlot.slotImage.sprite = null;
            uiSlot.slotImage.GetComponent<RectTransform>().SetAsFirstSibling();
            uiSlot.amountItemInSlotText.text = "";
        }
    }

    // LOGIC 
    // ===================================================================
    #region Interactive Events
    public void OnClick(BaseEventData baseEvent, GameObject clickedObject)
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

        UpdateInventoryUI();
    }

    public void OnEnter(GameObject clickedObject)
    {      
        currentSlotDrag = clickedObject;
        currentSlotClicked = clickedObject;

        if(clickedObject != null)
        {
            currentSlotDrag = clickedObject;       
        }           
    }

    public void OnExit(GameObject clickedObject)
    {
        //isEnter = false;
        currentPointerState = PointerState.Null;

        currentSlotDrag = null;
        currentSlotClicked = null;
    }

    public void OnBeginDrag(BaseEventData baseEvent, GameObject clickedObject)
    {
        PointerEventData pointerEventData = (PointerEventData)baseEvent;
        int index = GetItemSlotIndex(clickedObject);
        if (index == -1) return;

        if (clickedObject != null)
        {
            currentSlotDrag = clickedObject;
            startingSlotDrag = clickedObject;
        }
        
   
        if (pointerEventData.pointerId == -1)   // Mouse Left Event
        {
            OnLeftClick(index);
        }


        if (pointerEventData.pointerId == -2)   // Mouse Right Event
        {
            OnRightClick(index);
        }

        UpdateInventoryUI();
    }


    public void OnEndDrag(BaseEventData baseEvent, GameObject clickedObject)
    {
        PointerEventData pointerEventData = (PointerEventData)baseEvent;
        if (pointerEventData.pointerId == -1)
        {
            int index = GetItemSlotIndex(currentSlotDrag);
            if (inventory.HasSlot(index) == false) return;

            if (inventory.HasItem(index))
            {
                if (IsSameItem(inventory.slotList[index].itemObject, inventory.ItemInHand.itemSlot.itemObject))
                {
                    CombineItemSlotQuantity(index);
                }
                else
                {
                    // Swap slot the first time
                    SwapItemInHandAndInventorySlot(index);


                    // Swap slot the second time if there are no items in this STRARTING SLOT. 
                    int startingSlotIndex = GetItemSlotIndex(startingSlotDrag);
                    if (inventory.slotList[startingSlotIndex].HasItem() == false)
                    {
                        startingSlotDrag = null;
                        SwapItemInHandAndInventorySlot(startingSlotIndex);
                    }


                }
            }
            else
            {
                SwapItemInHandAndInventorySlot(index);
            }
            UpdateInventoryUI();
        }        
    }
    #endregion



    #region Inventory interactive methods

    private void OnLeftClick(int index)
    {
        //if (currentPointerState != PointerState.LeftClick) return;

        if (inventory.ItemInHand.HasItem() == false)
        {
            if (inventory.slotList[index].HasItem() == false)
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
            if (inventory.slotList[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                SwapItemInHandAndInventorySlot(index);
                ClearItemInHand();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                CombineItemSlotQuantity(index);
            }
        }
    }

    private void OnRightClick(int index)
    {
        if (currentPointerState != PointerState.RightClick) return;

        if (inventory.ItemInHand.HasItem() == false)
        {
            if (inventory.slotList[index].HasItem() == false)
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
            if (inventory.slotList[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                inventory.AddNewItemIntoInventoryAtIndex(index, inventory.ItemInHand.itemSlot.itemObject);
                inventory.ItemInHand.RemoveItem();

            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if(IsSameItem(inventory.ItemInHand.GetItem(), inventory.GetItem(index)))
                {
                    //Debug.Log("Same item");
                    bool isSlotNotFull = inventory.AddItem(index);

                    if(isSlotNotFull)
                    {
                        inventory.ItemInHand.RemoveItem();
                    }   
                }            
            }
        }
    }


    private void OnRightPress(int index)
    {
        if(currentPointerState != PointerState.RightPress) return;

        if(inventory.ItemInHand.HasItem() == true)
        {
            if (inventory.slotList[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                inventory.AddNewItemIntoInventoryAtIndex(index, inventory.ItemInHand.itemSlot.itemObject);
                inventory.ItemInHand.RemoveItem();

            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if (IsSameItem(inventory.ItemInHand.GetItem(), inventory.GetItem(index)))
                {
                    //Debug.Log("Same item");
                    bool isSlotNotFull = inventory.AddItem(index);

                    if (isSlotNotFull)
                    {
                        inventory.ItemInHand.RemoveItem();
                    }

                }
            }
        }
    }

    public bool IsSameItem(ItemData itemA, ItemData itemB)
    {
        if (itemA == null || itemB == null) return false;
        return itemA == itemB;
    }

    /// <summary>
    /// This method get itemSlot object then return itemSlot object's index in itemSlotList
    /// </summary>
    /// <param name="itemSlot"></param>
    /// <returns>Index itemSlot at itemSlotList</returns>
    private int GetItemSlotIndex(GameObject itemSlot)
    {
        if (itemSlot == null) return -1;
        return itemSlot.GetComponent<UIItemSlot>().SlotIndex;
    }

    /// <summary>
    /// This Method Swap InHandSlot and Slot in inventory at index
    /// </summary>
    /// <param name="index">index for itemSlotList</param>
    private void SwapItemInHandAndInventorySlot(int index)
    {
        if(inventory.ItemInHand.itemSlot.HasItem() &&
           inventory.slotList[index].HasItem())
        {
            var itemSlotToSwap_01 = inventory.ItemInHand.itemSlot;
            var itemSlotToSwap_02 = new ItemSlot(inventory.slotList[index]);
            inventory.slotList[index].ClearSlot();
            inventory.ItemInHand.itemSlot = itemSlotToSwap_02;
            inventory.slotList[index] = itemSlotToSwap_01;

            inventory.ItemInHand.uiItemInHand.DisplayItemInHand();    
        } 


        if (inventory.ItemInHand.itemSlot.HasItem() &&
           inventory.slotList[index].HasItem() == false)
        {
            var itemSlotToSwap_01 = inventory.ItemInHand.itemSlot;
            var itemSlotToSwap_02 = new ItemSlot(inventory.slotList[index]);
            inventory.slotList[index].ClearSlot();
            inventory.ItemInHand.itemSlot = itemSlotToSwap_02;
            inventory.slotList[index] = itemSlotToSwap_01;
        }
    }

 


    /// <summary>
    /// Method remove itemSlot in itemSlotList and put it in "itemInHand".
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get.</param>
    private void GetItemSlot(int index)
    {
        var chosenSlot = new ItemSlot(inventory.slotList[index]);
        inventory.slotList[index].ClearSlot();
        inventory.ItemInHand.itemSlot = chosenSlot;

        inventory.ItemInHand.uiItemInHand.DisplayItemInHand();
    }

    /// <summary>
    /// Method halve amount of item quantity from itemSlotList at index and put it in "itemInHand".
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get.</param>
    private void HalveItemSlotQuantity(int index)
    {
        if (inventory.slotList[index].itemQuantity > 1)
        {
            int splitItemQuantity = inventory.slotList[index].itemQuantity / 2;
            inventory.slotList[index].SetItemQuantity(inventory.slotList[index].itemQuantity - splitItemQuantity);

            var chosenSlot = new ItemSlot(inventory.slotList[index]);
            chosenSlot.SetItemQuantity(splitItemQuantity);
            inventory.ItemInHand.itemSlot = chosenSlot;

            inventory.ItemInHand.uiItemInHand.DisplayItemInHand();
        }
        else
        {
            GetItemSlot(index);
        }
    }

    /// <summary>
    /// Method combine amount of item quantity from itemSlotList at index inHandSlot.
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get</param>
    private void CombineItemSlotQuantity(int index)
    {
        //Debug.Log("CombineItemSlotQuantity");
        if (IsSameItem(inventory.slotList[index].itemObject, inventory.ItemInHand.itemSlot.itemObject))
        {
            Debug.Log("Same Object");
            inventory.ItemInHand.itemSlot = inventory.slotList[index].AddItemsFromAnotherSlot(inventory.ItemInHand.GetSlot());
            inventory.ItemInHand.uiItemInHand.DisplayItemInHand();
            
        }
        else
        {
            //Debug.Log("Not same object");
            SwapItemInHandAndInventorySlot(index);
        }
    }

    private void ClearItemInHand()
    {
        inventory.ItemInHand.itemSlot.ClearSlot();
    }



    #endregion Inventory interactive methods
}


public enum PointerState
{
    RightClick,
    LeftClick,
    RightPress,
    LeftPress,
    DoubleRightClick,
    DoubleLeftClick,
    Null
}