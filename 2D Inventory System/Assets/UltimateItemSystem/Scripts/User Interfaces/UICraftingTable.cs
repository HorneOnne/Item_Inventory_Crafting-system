using UnityEngine;
using UnityEngine.EventSystems;


namespace UltimateItemSystem
{
    /// <summary>
    /// Represents the UI for the crafting table and handles updating the display UI.
    /// </summary>
    [RequireComponent(typeof(CraftingTable))]
    public class UICraftingTable : Singleton<UICraftingTable>
    {
        [Header("REFERENCES")]
        public Player player;
        private PlayerInputHandler playerInputHandler;
        private CraftingTable craftingTable;
        private ItemInHand itemInHand;


        [Header("DATA")]
        [SerializeField] private GameObject[] craftingGridSlot = new GameObject[9];
        [SerializeField] private GameObject outputSlot;


        // Cached
        private bool handHasItem;
        private bool slotHasItem;


        private void Start()
        {
            craftingTable = CraftingTable.Instance;
            itemInHand = player.ItemInHand;
            playerInputHandler = player.PlayerInputHandler;


            for (int i = 0; i < craftingTable.GridLength; i++)
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
        /// <summary>
        /// Updates the entire crafting table UI display.
        /// </summary>
        public void UpdateCraftingTableDisplayUI()
        {
            // Update Crafting grid slot
            for (int i = 0; i < craftingTable.GridLength; i++)
            {
                UpdateCraftingTableDisplayUIAt(i);
            }

            // Update Output slot
            UpdateOutputSlotCraftingTalbeUI();
        }


        /// <summary>
        /// Updates the crafting table UI display for the specified grid slot index.
        /// </summary>
        /// <param name="index">The index of the grid slot to update.</param>

        public void UpdateCraftingTableDisplayUIAt(int index)
        {
            UIItemSlot uiSlot = craftingGridSlot[index].GetComponent<UIItemSlot>();

            var itemSlot = craftingTable.GetInputItemSlotAt(index);

            if (itemSlot != null && itemSlot.HasItem())
            {
                uiSlot.SetData(craftingTable.GetInputItemSlotAt(index), 1.0f);
            }
            else
            {
                uiSlot.SetData(craftingTable.suggestionInputSlots[index], 0.3f);
            }

        }


        /// <summary>
        /// Updates the crafting table UI display for the output slot.
        /// </summary>
        private void UpdateOutputSlotCraftingTalbeUI()
        {
            UIItemSlot uiSlot = outputSlot.GetComponent<UIItemSlot>();
            uiSlot.SetData(craftingTable.outputSlot, 1.0f);

            var itemSlot = craftingTable.outputSlot;

            if (itemSlot != null && itemSlot.HasItem())
            {
                uiSlot.SetData(craftingTable.outputSlot, 1.0f);
            }
            else
            {
                uiSlot.SetData(craftingTable.suggestionOutputSlot, 0.3f);
            }
        }

        #endregion UPDATE CRAFTINGTABLE DISPLAY UI REGION.


        #region LOGIC HANDLER
        // LOGIC 
        // ===================================================================
        /// <summary>
        /// Called when the output slot is clicked.
        /// </summary>
        /// <param name="baseEvent">The base event data.</param>
        /// <param name="clickedObject">The game object that was clicked.</param>
        public void OnOutputSlotClick(BaseEventData baseEvent, GameObject clickedObject)
        {
            PointerEventData pointerEventData = (PointerEventData)baseEvent;

            if (pointerEventData.pointerId == -1)   // Mouse Left Event
            {
                if (playerInputHandler.PressUtilityKeyInput)
                {
                    QuickGetAllOutputItem();
                }
                else if (craftingTable.HasOutputSlot() && craftingTable.outputSlot.HasItem())
                {
                    var outputItemSlot = craftingTable.outputSlot;
                    bool canPickup = itemInHand.PickupItem(ref outputItemSlot);

                    if (canPickup == true)
                    {
                        EventManager.TriggerOutputItemReceivedEvent();
                        EventManager.TriggerGridChangedEvent();
                        UpdateCraftingTableDisplayUI();
                    }

                }
            }
        }


        /// <summary>
        /// Picks up all items in the output slot.
        /// </summary>
        public void QuickGetAllOutputItem()
        {
            while (true)
            {
                if (craftingTable.HasOutputSlot() && craftingTable.outputSlot.HasItem())
                {
                    var outputItemSlot = craftingTable.outputSlot;
                    bool canPickup = itemInHand.PickupItem(ref outputItemSlot);

                    if (canPickup == true)
                    {
                        EventManager.TriggerOutputItemReceivedEvent();
                        EventManager.TriggerGridChangedEvent();
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


        /// <summary>
        /// Called when the mouse button is pressed down on an item slot.
        /// </summary>
        /// <param name="baseEvent">The base event data.</param>
        /// <param name="clickedObject">The game object that was clicked.</param>
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


        /// <summary>
        /// Called when the left mouse button is pressed down on an item slot.
        /// </summary>
        /// <param name="index">The index of the clicked item slot.</param>
        private void OnLeftClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = craftingTable.GetInputItemSlotAt(index).HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.Swap(ref craftingTable.inputSlots, index, StoredType.CraftingTable, true);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    itemInHand.Swap(ref craftingTable.inputSlots, index, StoredType.CraftingTable, true);
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    bool isSameItem = ItemData.IsSameItem(craftingTable.inputSlots[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = craftingTable.inputSlots[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.CraftingTable, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref craftingTable.inputSlots, index, StoredType.CraftingTable, true);
                    }
                }
            }

            EventManager.TriggerGridChangedEvent();
            UpdateCraftingTableDisplayUI();
        }


        /// <summary>
        /// Handles the right-click event on a UIItemSlot in the Crafting Table.
        /// </summary>
        /// <param name="index">The index of the UIItemSlot that was clicked.</param>
        private void OnRightClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = craftingTable.GetInputItemSlotAt(index).HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SplitItemSlotQuantityInInventory(ref craftingTable.inputSlots, index);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    craftingTable.AddItemToCraftingGrid(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    if (ItemData.IsSameItem(itemInHand.GetItemData(), craftingTable.GetItem(index)))
                    {
                        bool isSlotNotFull = craftingTable.AddItem(index);

                        if (isSlotNotFull)
                        {
                            itemInHand.RemoveItem();
                        }
                    }

                }
            }

            EventManager.TriggerGridChangedEvent();
            UpdateCraftingTableDisplayUI();
        }


        /// <summary>
        /// Handles the mouse enter event on a UIItemSlot in the Crafting Table.
        /// </summary>
        /// <param name="clickedObject">The UIItemSlot that was clicked.</param>
        public void OnEnter(GameObject clickedObject)
        {
            if (playerInputHandler.CurrentMouseState != PointerState.RightPressAfterWait)
            {
                return;
            }

            int index = GetItemSlotIndex(clickedObject);
            handHasItem = itemInHand.HasItemData();
            slotHasItem = craftingTable.GetInputItemSlotAt(index).HasItem();

            if (handHasItem == true)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    craftingTable.AddItemToCraftingGrid(index, itemInHand.GetItemData());
                    itemInHand.RemoveItem();
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    if (ItemData.IsSameItem(itemInHand.GetItemData(), craftingTable.GetItem(index)))
                    {
                        bool isSlotNotFull = craftingTable.AddItem(index);

                        if (isSlotNotFull)
                        {
                            itemInHand.RemoveItem();
                        }
                    }

                }
            }

            EventManager.TriggerGridChangedEvent();
            UpdateCraftingTableDisplayUI();
        }


        /// <summary>
        /// Returns the index of the UIItemSlot component of the provided GameObject.
        /// </summary>
        /// <param name="itemSlot">The GameObject to get the index from.</param>
        /// <returns>The index of the UIItemSlot component of the provided GameObject, or -1 if the GameObject is null.</returns>
        private int GetItemSlotIndex(GameObject itemSlot)
        {
            if (itemSlot == null) return -1;
            return itemSlot.GetComponent<UIItemSlot>().SlotIndex;
        }

        #endregion LOCGIC HANDLER

    }
}
