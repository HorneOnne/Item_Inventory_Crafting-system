using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// Singleton class that manages the UI of the chest inventory. It displays the chest inventory items as a list of UIItemSlot objects.
    /// </summary>
    public class UIChestInventory : Singleton<UIChestInventory>
    {
        [Header("REFERENCES")]
        public Player player;
        private ItemInHand itemInHand;
        private PlayerInputHandler playerInputHandler;
        [SerializeField] private ChestInventory chestInventory;


        [Header("UI CONTAINER")]
        public List<GameObject> itemSlots;

        [Header("UI SLOT PREFAB")]
        public GameObject itemSlotPrefab;

        // Cached.
        private GameObject currentDraggedSlot;
        private GameObject startingDraggedSlot;
        private GameObject currentClickedSlot;


        [Header("Inventory Settings")]
        public DragType dragType;

        // right press
        [SerializeField] float pressIntervalTime = 1.0f;
        private float pressIntervalTimeCount = 0.0f;

        private const int MAX_NORMAL_CHEST_SLOT = 36;   // The maximum number of slots in a normal chest inventory


        //Cache
        private bool handHasItem;                       // Flag indicating whether the player's hand has an item
        private bool slotHasItem;                       // Flag indicating whether the UIItemSlot object has an item

        #region Properties
        public int SlotCount { get => transform.childCount; }

        #endregion

        private void Awake()
        {
            EventManager.OnChestInventoryUpdated += UpdateInventoryUI;
        }

        private void OnDestroy()
        {
            EventManager.OnChestInventoryUpdated -= UpdateInventoryUI;
        }



        /// <summary>
        /// Initializes the chest inventory UI by creating UIItemSlot objects for each chest inventory slot.
        /// </summary>
        private void Start()
        {
            dragType = DragType.Swap;

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

                itemSlots.Add(slotObject);
            }

            // Update Inventory UI at the first time when start game.
            Invoke("UpdateInventoryUI", .1f);
            UIManager.Instance.ChestInventoryCanvas.SetActive(false);
        }


        /// <summary>
        /// Sets the chest inventory data and initializes the UI with it.
        /// </summary>
        /// <param name="chestInventory">The chest inventory data to set.</param>
        public void SetChestInventoryData(ChestInventory chestInventory)
        {
            this.chestInventory = chestInventory;

            this.player = chestInventory.player;
            playerInputHandler = player.PlayerInputHandler;
            itemInHand = this.player.ItemInHand;

            UpdateInventoryUI();
        }


        /// <summary>
        /// Removes the chest inventory data and resets the UI.
        /// </summary>
        public void RemoveChestInventoryData()
        {
            this.chestInventory = null;

            this.player = null;
            playerInputHandler = null;
            itemInHand = null;
        }



        private void Update()
        {
            if (itemInHand.HasItemData())
            {
                if (playerInputHandler.CurrentMouseState == PointerState.RightPressAfterWait)
                {
                    if (currentClickedSlot != null)
                    {
                        if (Time.time - pressIntervalTimeCount >= pressIntervalTime)
                        {
                            OnRightPress(GetItemSlotIndex(currentClickedSlot));
                            pressIntervalTimeCount = Time.time;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Updates the entire chest inventory UI.
        /// </summary>
        public void UpdateInventoryUI()
        {
            if (chestInventory == null) return;

            for (int i = 0; i < MAX_NORMAL_CHEST_SLOT; i++)
            {
                UpdateInventoryUIAt(i);
            }
        }


        /// <summary>
        /// Updates a specific item slot in the chest inventory UI.
        /// </summary>
        /// <param name="index">The index of the item slot to update.</param>
        public void UpdateInventoryUIAt(int index)
        {
            if (chestInventory == null) return;
            UIItemSlot uiSlot = itemSlots[index].GetComponent<UIItemSlot>();
            uiSlot.SetData(chestInventory.inventory[index]);
        }

        // LOGIC 
        // ===================================================================
        #region Interactive Events
        /// <summary>
        /// Handle the click event on the item slot.
        /// </summary>
        /// <param name="baseEvent">The base event data.</param>
        /// <param name="clickedObject">The clicked game object.</param>
        public void OnClick(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;

            int index = GetItemSlotIndex(clickedObject);
            if (index == -1) return;

            // Handle the left click event.
            if (pointerEventData.pointerId == -1) 
            {
                OnLeftClick(index);
            }

            // Handle the right click event.
            if (pointerEventData.pointerId == -2)  
            {
                OnRightClick(index);
            }

            UpdateInventoryUI();
        }


        /// <summary>
        /// Handle the enter event on the item slot.
        /// </summary>
        /// <param name="clickedObject">The clicked game object.</param>
        public void OnEnter(GameObject clickedObject)
        {
            currentDraggedSlot = clickedObject;
            currentClickedSlot = clickedObject;

            if (clickedObject != null)
            {
                currentDraggedSlot = clickedObject;
            }
        }


        /// <summary>
        /// Handle the exit event on the item slot.
        /// </summary>
        public void OnExit(GameObject clickedObject)
        {
            currentDraggedSlot = null;
            currentClickedSlot = null;
        }


        /// <summary>
        /// Handle the begin drag event on the item slot.
        /// </summary>
        /// <param name="baseEvent">The base event data.</param>
        /// <param name="clickedObject">The clicked game object.</param>
        public void OnBeginDrag(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;
            int index = GetItemSlotIndex(clickedObject);
            if (index == -1) return;

            // Set the current dragged slot.
            if (clickedObject != null)
            {
                currentDraggedSlot = clickedObject;
                startingDraggedSlot = clickedObject;
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
        /// Handle the end drag event on the item slot.
        /// </summary>
        /// <param name="baseEvent">The base event data.</param>
        /// <param name="clickedObject">The clicked game object.</param>
        public void OnEndDrag(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;
            if (pointerEventData.pointerId == -1)
            {
                int index = GetItemSlotIndex(currentDraggedSlot);
                if (chestInventory.HasSlot(index) == false) return;

                // Check if the slot being dragged has an item in it
                if (chestInventory.HasItem(index))
                {
                    // Check if the item being dragged is the same as the item in the slot
                    bool isSameItem = ItemData.IsSameItem(chestInventory.inventory[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        // If the items are the same, add the items in the dragged slot to the slot in the chest
                        // and set the dragged slot to the remaining items (if any)
                        ItemSlot remainItems = chestInventory.inventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.ChestInventory, true);
                    }
                    else
                    {
                        // If the items are different, swap the dragged item with the item in the chest slot
                        itemInHand.Swap(ref chestInventory.inventory, index, StoredType.ChestInventory, true);

                        if (dragType == DragType.Swap)
                        {
                            int startingSlotIndex = GetItemSlotIndex(startingDraggedSlot);
                            if (chestInventory.inventory[startingSlotIndex].HasItem() == false)
                            {
                                startingDraggedSlot = null;
                                itemInHand.Swap(ref chestInventory.inventory, startingSlotIndex, StoredType.ChestInventory, true);
                            }
                        }

                    }
                }
                else
                {
                    // If the slot is empty, swap the dragged item with the empty slot
                    itemInHand.Swap(ref chestInventory.inventory, index, StoredType.ChestInventory, true);
                }

                // Update the inventory UI
                UpdateInventoryUI();
            }
        }
        #endregion



        #region Inventory interactive methods
        /// <summary>
        /// Handles left click on an item slot in the chest inventory.
        /// </summary>
        /// <param name="index">The index of the item slot that was clicked.</param>
        private void OnLeftClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = chestInventory.inventory[index].HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.Swap(ref chestInventory.inventory, index, StoredType.ChestInventory, true);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    itemInHand.Swap(ref chestInventory.inventory, index, StoredType.ChestInventory, true);
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    bool isSameItem = ItemData.IsSameItem(chestInventory.inventory[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = chestInventory.inventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.ChestInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref chestInventory.inventory, index, StoredType.ChestInventory, true);
                    }
                }
            }
        }


        /// <summary>
        /// Handles right click on an item slot in the chest inventory.
        /// </summary>
        /// <param name="index">The index of the item slot that was clicked.</param>
        private void OnRightClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = chestInventory.inventory[index].HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SplitItemSlotQuantityInInventory(ref chestInventory.inventory, index);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    chestInventory.AddNewItemAt(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();

                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    if (ItemData.IsSameItem(itemInHand.GetItemData(), chestInventory.GetItem(index)))
                    {
                        bool isSlotNotFull = chestInventory.AddItem(index);

                        if (isSlotNotFull)
                        {
                            itemInHand.RemoveItem();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Handles right mouse button press on an item slot in the chest inventory.
        /// </summary>
        /// <param name="index">The index of the item slot that was clicked.</param>
        private void OnRightPress(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = chestInventory.inventory[index].HasItem();

            if (handHasItem == true)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    chestInventory.AddNewItemAt(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();

                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");

                    if (ItemData.IsSameItem(itemInHand.GetItemData(), chestInventory.GetItem(index)))
                    {
                        bool isSlotNotFull = chestInventory.AddItem(index);

                        if (isSlotNotFull)
                        {
                            itemInHand.RemoveItem();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// This method retrieves the index of the item slot from the UIItemSlot component attached to the provided game object.
        /// </summary>
        /// <param name="itemSlot">The game object representing the item slot.</param>
        /// <returns>The index of the item slot in the list of item slots.</returns>
        private int GetItemSlotIndex(GameObject itemSlot)
        {
            if (itemSlot == null) return -1;
            return itemSlot.GetComponent<UIItemSlot>().SlotIndex;
        }
        #endregion Inventory interactive methods
    }
}