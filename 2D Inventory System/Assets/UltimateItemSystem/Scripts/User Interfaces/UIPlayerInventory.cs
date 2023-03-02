using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UltimateItemSystem
{
    /// <summary>
    /// Singleton UI component for player inventory, responsible for updating and displaying the inventory UI.
    /// </summary>
    public class UIPlayerInventory : Singleton<UIPlayerInventory>
    {
        [Header("REFERENCES")]
        public Player player;
        private PlayerInventory playerInventory;
        private PlayerInputHandler playerInputHandler;
        private ItemInHand itemInHand;


        [Header("DATA")]
        [HideInInspector] public List<GameObject> itemSlotList;


        [Header("UI PREFAB")]
        public GameObject itemSlotPrefab;
        public GameObject numericItemSlotPrefab;


        [Header("UI Event Properties")]
        private GameObject currentSlotDrag;
        private GameObject startingSlotDrag;
        private GameObject currentSlotClicked;

        /// <summary>
        /// The type of drag that can be performed on inventory items.
        /// </summary>
        [Header("INVENTORY SETTINGS")]
        public DragType dragType;
        /// <summary>
        /// The interval time before the right mouse button press can be repeated.
        /// </summary>
        [SerializeField] private float pressIntervalTime = 1.0f;
        

        // CACHED
        private bool handHasItem;
        private bool slotHasItem;
        private float pressIntervalTimeCount = 0.0f;


        private void OnEnable()
        {
            EventManager.OnPlayerInventoryUpdated += UpdateInventoryUI;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerInventoryUpdated -= UpdateInventoryUI;
        }


        private void Start()
        {
            itemInHand = player.ItemInHand;
            playerInventory = player.PlayerInventory;
            playerInputHandler = player.PlayerInputHandler;
            dragType = DragType.Swap;


            for (int i = 0; i < playerInventory.Capacity; i++)
            {
                GameObject slotObject;

                if(i < playerInventory.WidthSize)
                {
                    slotObject = Instantiate(numericItemSlotPrefab, this.transform);
                    slotObject.GetComponent<UINumericItemSlot>().slotIndexText.text = $"{i + 1}";
                }    
                else
                    slotObject = Instantiate(itemSlotPrefab, this.transform);


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
        }



        private void Update()
        {
            if (itemInHand.HasItemData())
            {
                if (playerInputHandler.CurrentMouseState == PointerState.RightPressAfterWait)
                {
                    if (currentSlotClicked != null)
                    {
                        if (Time.time - pressIntervalTimeCount >= pressIntervalTime)
                        {
                            OnRightPress(GetItemSlotIndex(currentSlotClicked));
                            pressIntervalTimeCount = Time.time;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Updates the entire inventory UI.
        /// </summary>
        public void UpdateInventoryUI()
        {
            for (int i = 0; i < playerInventory.Capacity; i++)
            {
                UpdateInventoryUIAt(i);
            }
        }


        /// <summary>
        /// Updates the inventory UI at the given index.
        /// </summary>
        /// <param name="index">The index of the inventory slot to update.</param>
        public void UpdateInventoryUIAt(int index)
        {
            UIItemSlot uiSlot = itemSlotList[index].GetComponent<UIItemSlot>();
            uiSlot.SetData(playerInventory.inventory[index]);
        }

        // LOGIC 
        // ===================================================================
        #region Interactive Events

        /// <summary>
        /// Handles the click event on an item slot.
        /// </summary>
        /// <param name="baseEvent">The BaseEventData associated with the click event.</param>
        /// <param name="clickedObject">The GameObject that was clicked on.</param>
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

        /// <summary>
        /// Handles the pointer enter event on an item slot.
        /// </summary>
        /// <param name="clickedObject">The GameObject that was entered.</param>
        public void OnEnter(GameObject clickedObject)
        {
            currentSlotDrag = clickedObject;
            currentSlotClicked = clickedObject;

            if (clickedObject != null)
            {
                currentSlotDrag = clickedObject;
            }
        }


        /// <summary>
        /// Handles the pointer exit event on an item slot.
        /// </summary>
        /// <param name="clickedObject">The GameObject that was exited.</param>
        public void OnExit(GameObject clickedObject)
        {
            currentSlotDrag = null;
            currentSlotClicked = null;
        }


        /// <summary>
        /// Handles the begin drag event on an item slot.
        /// </summary>
        /// <param name="baseEvent">The BaseEventData associated with the begin drag event.</param>
        /// <param name="clickedObject">The GameObject that was dragged.</param>
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


        /// <summary>
        /// Called when the user has finished dragging an item in the inventory UI.
        /// </summary>
        /// <param name="baseEvent">The BaseEventData object representing the drag event.</param>
        /// <param name="clickedObject">The GameObject that was clicked to initiate the drag.</param>
        public void OnEndDrag(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;
            if (pointerEventData.pointerId == -1)
            {
                int index = GetItemSlotIndex(currentSlotDrag);
                if (playerInventory.HasSlot(index) == false) return;

                if (playerInventory.HasItem(index))
                {
                    bool isSameItem = ItemData.IsSameItem(playerInventory.inventory[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = playerInventory.inventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.PlayerInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref playerInventory.inventory, index, StoredType.PlayerInventory, true);

                        if (dragType == DragType.Swap)
                        {
                            int startingSlotIndex = GetItemSlotIndex(startingSlotDrag);
                            if (playerInventory.inventory[startingSlotIndex].HasItem() == false)
                            {
                                startingSlotDrag = null;
                                itemInHand.Swap(ref playerInventory.inventory, startingSlotIndex, StoredType.PlayerInventory, true);
                            }
                        }
                    }
                }
                else
                {
                    itemInHand.Swap(ref playerInventory.inventory, index, StoredType.PlayerInventory, true);
                }

                UpdateInventoryUI();
            }
        }
        #endregion



        #region Inventory interactive methods
        /// <summary>
        /// Called when the user left-clicks on an inventory slot.
        /// </summary>
        /// <param name="index">The index of the inventory slot that was clicked.</param>
        private void OnLeftClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = playerInventory.inventory[index].HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.Swap(ref playerInventory.inventory, index, StoredType.PlayerInventory, true);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    itemInHand.Swap(ref playerInventory.inventory, index, StoredType.PlayerInventory, true);
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    bool isSameItem = ItemData.IsSameItem(playerInventory.inventory[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = playerInventory.inventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.PlayerInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref playerInventory.inventory, index, StoredType.PlayerInventory, true);
                    }
                }
            }
        }


        /// <summary>
        /// Handles the right click event on a UIItemSlot and performs the appropriate action based on the player's inventory and the item in hand.
        /// </summary>
        /// <param name="index">The index of the clicked UIItemSlot.</param>
        private void OnRightClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = playerInventory.inventory[index].HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SplitItemSlotQuantityInInventory(ref playerInventory.inventory, index);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    playerInventory.AddNewItemAt(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();

                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    if (ItemData.IsSameItem(itemInHand.GetItemData(), playerInventory.GetItem(index)))
                    {
                        bool isSlotNotFull = playerInventory.AddItemAt(index);

                        if (isSlotNotFull)
                        {
                            itemInHand.RemoveItem();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Handles the right press event on a UIItemSlot and performs the appropriate action based on the player's inventory and the item in hand.
        /// </summary>
        /// <param name="index">The index of the clicked UIItemSlot.</param>
        private void OnRightPress(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = playerInventory.inventory[index].HasItem();

            if (handHasItem == true)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    playerInventory.AddNewItemAt(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();

                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                    if (ItemData.IsSameItem(itemInHand.GetItemData(), playerInventory.GetItem(index)))
                    {
                        bool isSlotNotFull = playerInventory.AddItemAt(index);

                        if (isSlotNotFull)
                        {
                            itemInHand.RemoveItem();
                        }

                    }
                }
            }
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

        #endregion Inventory interactive methods
    }
}


