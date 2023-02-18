using MyGame.Ultilities;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class UIChestInventory : Singleton<UIChestInventory>
{
    [Header("Inventory References")]
    public Player player;
    private UIItemInHand uiItemInHand;
    private ItemInHand itemInHand;
    private ChestInventory chestInventory;


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


    private bool hasChestInventoryData;

    private const int MAX_NORMAL_CHEST_SLOT = 36;



    #region Properties
    public int SlotCount { get => transform.childCount; }

    #endregion

    private void Awake()
    {
        EventManager.OnChestInventoryUpdate += UpdateInventoryUI;
    }

    private void OnDestroy()
    {
        EventManager.OnChestInventoryUpdate -= UpdateInventoryUI;
    }


    private void Start()
    {
        hasChestInventoryData = false;

        for (int i = 0; i < MAX_NORMAL_CHEST_SLOT; i++)
        {
            GameObject slotObject = Instantiate(itemSlotPrefab, this.transform);
            slotObject.GetComponent<UIItemSlot>().SetIndex(i);
            slotObject.GetComponent<UIItemSlot>().SetData(null);

            Utilities.AddEvent(slotObject, EventTriggerType.PointerClick, (baseEvent) => OnClick(baseEvent, slotObject));
            Utilities.AddEvent(slotObject, EventTriggerType.PointerEnter, delegate { OnEnter(slotObject); });
            Utilities.AddEvent(slotObject, EventTriggerType.PointerExit, delegate { OnExit(slotObject); });
            Utilities.AddEvent(slotObject, EventTriggerType.BeginDrag, (baseEvent) => OnBeginDrag(baseEvent, slotObject));
            Utilities.AddEvent(slotObject, EventTriggerType.EndDrag, (baseEvent) => OnEndDrag(baseEvent, slotObject));

            itemSlotList.Add(slotObject);
        }

        // Update Inventory UI at the first time when start game.
        Invoke("UpdateInventoryUI", .1f);
        UIManager.Instance.ChestInventoryCanvas.SetActive(false);
    }

    public void SetChestInventoryData(ChestInventory chestInventory)
    {
        this.chestInventory = chestInventory;
        hasChestInventoryData = true;

        this.player = chestInventory.player;
        itemInHand = this.player.ItemInHand;
        uiItemInHand = UIItemInHand.Instance;

        UpdateInventoryUI();
    }


    public void RemoveChestInventoryData()
    {
        this.chestInventory = null;
        hasChestInventoryData = false;

        this.player = null;
        itemInHand = null;
        uiItemInHand = null;
    }


    private void Update()
    {
        if (hasChestInventoryData == false) return;

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
        if (chestInventory == null) return;

        for (int i = 0; i < MAX_NORMAL_CHEST_SLOT; i++)
        {
            UpdateInventoryUIAt(i);
        }
    }

    public void UpdateInventoryUIAt(int index)
    {
        if (chestInventory == null) return;
        UIItemSlot uiSlot = itemSlotList[index].GetComponent<UIItemSlot>();
        uiSlot.SetData(chestInventory.inventory[index]);
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

        if (clickedObject != null)
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
            if (chestInventory.HasSlot(index) == false) return;

            if (chestInventory.HasItem(index))
            {
                if (IsSameItem(chestInventory.inventory[index].ItemData, itemInHand.GetItemData()))
                {
                    CombineItemSlotQuantity(index);
                }
                else
                {
                    // Swap slot the first time
                    SwapItemInHandAndInventorySlot(index);


                    // Swap slot the second time if there are no items in this STRARTING SLOT. 
                    int startingSlotIndex = GetItemSlotIndex(startingSlotDrag);
                    if (chestInventory.inventory[startingSlotIndex].HasItem() == false)
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
            if (chestInventory.inventory[index].HasItem() == false)
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
            if (chestInventory.inventory[index].HasItem() == false)
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
            if (chestInventory.inventory[index].HasItem() == false)
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
            if (chestInventory.inventory[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                chestInventory.AddNewItemIntoInventoryAtIndex(index, itemInHand.GetItemData());
                itemInHand.RemoveItem();

            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if (IsSameItem(itemInHand.GetItemData(), chestInventory.GetItem(index)))
                {
                    //Debug.Log("Same item");
                    bool isSlotNotFull = chestInventory.AddItem(index);

                    if (isSlotNotFull)
                    {
                        itemInHand.RemoveItem();
                    }
                }
            }
        }
    }


    private void OnRightPress(int index)
    {
        if (currentPointerState != PointerState.RightPress) return;

        if (itemInHand.HasItemData() == true)
        {
            if (chestInventory.inventory[index].HasItem() == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                chestInventory.AddNewItemIntoInventoryAtIndex(index, itemInHand.GetItemData());
                itemInHand.RemoveItem();

            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                if (IsSameItem(itemInHand.GetItemData(), chestInventory.GetItem(index)))
                {
                    //Debug.Log("Same item");
                    bool isSlotNotFull = chestInventory.AddItem(index);

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
        if (itemInHand.HasItemData() &&
           chestInventory.inventory[index].HasItem())
        {
            var itemSlotToSwap_01 = itemInHand.GetSlot();
            var itemSlotToSwap_02 = new ItemSlot(chestInventory.inventory[index]);
            chestInventory.inventory[index].ClearSlot();
            itemInHand.Set(itemSlotToSwap_02, index, StoredType.ChestInventory, true);
            chestInventory.inventory[index] = itemSlotToSwap_01;
        }


        if (itemInHand.HasItemData() &&
           chestInventory.inventory[index].HasItem() == false)
        {
            var itemSlotToSwap_01 = itemInHand.GetSlot();
            var itemSlotToSwap_02 = new ItemSlot(chestInventory.inventory[index]);
            chestInventory.inventory[index].ClearSlot();
            itemInHand.Set(itemSlotToSwap_02, index, StoredType.ChestInventory);
            chestInventory.inventory[index] = itemSlotToSwap_01;
        }
    }




    /// <summary>
    /// Method remove itemSlot in itemSlotList and put it in "itemInHand".
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get.</param>
    private void GetItemSlot(int index)
    {
        var chosenSlot = new ItemSlot(chestInventory.inventory[index]);
        chestInventory.inventory[index].ClearSlot();
        itemInHand.Set(chosenSlot, index, StoredType.ChestInventory, true);
    }

    /// <summary>
    /// Method halve amount of item quantity from itemSlotList at index and put it in "itemInHand".
    /// </summary>
    /// <param name="index">Index used in itemSlotList at specific itemSlot you want to get.</param>
    private void HalveItemSlotQuantity(int index)
    {
        if (chestInventory.inventory[index].ItemQuantity > 1)
        {
            int splitItemQuantity = chestInventory.inventory[index].ItemQuantity / 2;
            chestInventory.inventory[index].SetItemQuantity(chestInventory.inventory[index].ItemQuantity - splitItemQuantity);

            var chosenSlot = new ItemSlot(chestInventory.inventory[index]);
            chosenSlot.SetItemQuantity(splitItemQuantity);
            itemInHand.Set(chosenSlot, index, StoredType.ChestInventory, true);
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
        if (IsSameItem(chestInventory.inventory[index].ItemData, itemInHand.GetItemData()))
        {
            /*Debug.Log("Same Object");           
            itemInHand.Set(chestInventory.inventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot()), StoredType.ChestInventory);
            uiItemInHand.DisplayItemInHand();*/


            Debug.Log("Same Object");

            var returnSlot = chestInventory.inventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
            itemInHand.Set(returnSlot, index, StoredType.ChestInventory, true);

        }
        else
        {
            //Debug.Log("Not same object");
            SwapItemInHandAndInventorySlot(index);
        }
    }

    #endregion Inventory interactive methods
}
