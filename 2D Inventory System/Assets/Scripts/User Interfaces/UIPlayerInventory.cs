using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MyGame.Ultilities;
using System;

public class UIPlayerInventory : Singleton<UIPlayerInventory>
{
    [Header("Inventory References")]
    public Player player;
    private PlayerInventory playerInventory;
    private UIItemInHand uiItemInHand;
    private ItemInHand itemInHand;
    

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
        itemInHand = player.ItemInHand;
        uiItemInHand = UIItemInHand.Instance;
        playerInventory = player.PlayerInventory;


        for (int i = 0; i < playerInventory.capacity; i++)
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


        if (player.ItemInHand.HasItemData())
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

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < playerInventory.capacity; i++)
        {
            UpdateInventoryUIAt(i);
        }
    }

    public void UpdateInventoryUIAt(int index)
    {
        UIItemSlot uiSlot = itemSlotList[index].GetComponent<UIItemSlot>();
        if (playerInventory.inventory[index].HasItem())
        {
            uiSlot.slotImage.sprite = playerInventory.inventory[index].ItemObject.icon;
            uiSlot.slotImage.GetComponent<RectTransform>().SetAsLastSibling();

            int itemQuantity = playerInventory.inventory[index].itemQuantity;
            
            if(itemQuantity > 1)
                uiSlot.amountItemInSlotText.text = playerInventory.inventory[index].itemQuantity.ToString();
            else
                uiSlot.amountItemInSlotText.text = "";
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
            if (playerInventory.HasSlot(index) == false) return;

            if (playerInventory.HasItem(index))
            {
                if (IsSameItem(playerInventory.inventory[index].ItemObject, itemInHand.GetItem()))
                {
                    CombineItemSlotQuantity(index);
                }
                else
                {
                    // Swap slot the first time
                    SwapItemInHandAndInventorySlot(index);


                    // Swap slot the second time if there are no items in this STRARTING SLOT. 
                    int startingSlotIndex = GetItemSlotIndex(startingSlotDrag);
                    if (playerInventory.inventory[startingSlotIndex].HasItem() == false)
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

        if (itemInHand.HasItemData() == false)
        {
            if (playerInventory.inventory[index].HasItem() == false)
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
            if (playerInventory.inventory[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                SwapItemInHandAndInventorySlot(index);
                itemInHand.RemoveItem();
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

        if (itemInHand.HasItemData() == false)
        {
            if (playerInventory.inventory[index].HasItem() == false)
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
            if (playerInventory.inventory[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                playerInventory.AddNewItemIntoInventoryAtIndex(index, itemInHand.GetItem());
                itemInHand.RemoveItem();

            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if(IsSameItem(itemInHand.GetItem(), playerInventory.GetItem(index)))
                {
                    //Debug.Log("Same item");
                    bool isSlotNotFull = playerInventory.AddItem(index);

                    if(isSlotNotFull)
                    {
                        itemInHand.RemoveItem();
                    }   
                }            
            }
        }
    }


    private void OnRightPress(int index)
    {
        if(currentPointerState != PointerState.RightPress) return;

        if(itemInHand.HasItemData() == true)
        {
            if (playerInventory.inventory[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                playerInventory.AddNewItemIntoInventoryAtIndex(index, itemInHand.GetItem());
                itemInHand.RemoveItem();

            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if (IsSameItem(itemInHand.GetItem(), playerInventory.GetItem(index)))
                {
                    //Debug.Log("Same item");
                    bool isSlotNotFull = playerInventory.AddItem(index);

                    if (isSlotNotFull)
                    {
                        itemInHand.RemoveItem();
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
        if(itemInHand.HasItemData() &&
           playerInventory.inventory[index].HasItem())
        {
            var itemSlotToSwap_01 = itemInHand.GetSlot();
            var itemSlotToSwap_02 = new ItemSlot(playerInventory.inventory[index]);
            playerInventory.inventory[index].ClearSlot();
            itemInHand.Set(itemSlotToSwap_02, StoredType.PlayerInventory);
            playerInventory.inventory[index] = itemSlotToSwap_01;

            uiItemInHand.DisplayItemInHand();    
        } 


        if (itemInHand.HasItemData() &&
           playerInventory.inventory[index].HasItem() == false)
        {
            var itemSlotToSwap_01 = itemInHand.GetSlot();
            var itemSlotToSwap_02 = new ItemSlot(playerInventory.inventory[index]);
            playerInventory.inventory[index].ClearSlot();
            itemInHand.Set(itemSlotToSwap_02, StoredType.PlayerInventory);
            playerInventory.inventory[index] = itemSlotToSwap_01;
        }
    }

 


    /// <summary>
    /// Method remove itemSlot in itemSlotList and put it in "itemInHand".
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get.</param>
    private void GetItemSlot(int index)
    {
        var chosenSlot = new ItemSlot(playerInventory.inventory[index]);
        playerInventory.inventory[index].ClearSlot();
        itemInHand.Set(chosenSlot, StoredType.PlayerInventory);

        uiItemInHand.DisplayItemInHand();
    }

    /// <summary>
    /// Method halve amount of item quantity from itemSlotList at index and put it in "itemInHand".
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get.</param>
    private void HalveItemSlotQuantity(int index)
    {
        if (playerInventory.inventory[index].itemQuantity > 1)
        {
            int splitItemQuantity = playerInventory.inventory[index].itemQuantity / 2;
            playerInventory.inventory[index].SetItemQuantity(playerInventory.inventory[index].itemQuantity - splitItemQuantity);

            var chosenSlot = new ItemSlot(playerInventory.inventory[index]);
            chosenSlot.SetItemQuantity(splitItemQuantity);
            itemInHand.Set(chosenSlot, StoredType.PlayerInventory);

            uiItemInHand.DisplayItemInHand();
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
        if (IsSameItem(playerInventory.inventory[index].ItemObject, itemInHand.GetItem()))
        {
            Debug.Log("Same Object");
            itemInHand.Set(playerInventory.inventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot()), StoredType.PlayerInventory);
            uiItemInHand.DisplayItemInHand();
            
        }
        else
        {
            //Debug.Log("Not same object");
            SwapItemInHandAndInventorySlot(index);
        }
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