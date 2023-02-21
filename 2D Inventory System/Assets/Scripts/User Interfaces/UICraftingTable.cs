using UnityEngine;
using UnityEngine.EventSystems;


namespace DIVH_InventorySystem
{
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


        // Cache
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

        public void UpdateCraftingTableDisplayUIAt(int index)
        {
            UIItemSlot uiSlot = craftingGridSlot[index].GetComponent<UIItemSlot>();

            var itemSlot = craftingTable.GetItemInputSlotAt(index);

            if (itemSlot != null && itemSlot.HasItem())
            {
                uiSlot.SetData(craftingTable.GetItemInputSlotAt(index), 1.0f);
            }
            else
            {
                uiSlot.SetData(craftingTable.craftingSuggestionInputSlots[index], 0.3f);
            }

        }

        private void UpdateOutputSlotCraftingTalbeUI()
        {
            UIItemSlot uiSlot = outputSlot.GetComponent<UIItemSlot>();
            uiSlot.SetData(craftingTable.craftingOutputSlot, 1.0f);

            var itemSlot = craftingTable.craftingOutputSlot;

            if (itemSlot != null && itemSlot.HasItem())
            {
                uiSlot.SetData(craftingTable.craftingOutputSlot, 1.0f);
            }
            else
            {
                uiSlot.SetData(craftingTable.craftingSuggestionOutputSlot, 0.3f);
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
                if (playerInputHandler.PressUtilityKeyInput)
                {
                    QuickGetAllOutputItem();
                }
                else if (craftingTable.HasOutputSlot() && craftingTable.craftingOutputSlot.HasItem())
                {
                    var outputItemSlot = craftingTable.craftingOutputSlot;
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
            while (true)
            {
                if (craftingTable.HasOutputSlot() && craftingTable.craftingOutputSlot.HasItem())
                {
                    var outputItemSlot = craftingTable.craftingOutputSlot;
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
            slotHasItem = craftingTable.GetItemInputSlotAt(index).HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.Swap(ref craftingTable.craftingInputSlots, index, StoredType.CraftingTable, true);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    itemInHand.Swap(ref craftingTable.craftingInputSlots, index, StoredType.CraftingTable, true);
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    bool isSameItem = ItemData.IsSameItem(craftingTable.craftingInputSlots[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = craftingTable.craftingInputSlots[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.Set(remainItems, index, StoredType.CraftingTable, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref craftingTable.craftingInputSlots, index, StoredType.CraftingTable, true);
                    }
                }
            }

            EventManager.GridChanged();
            UpdateCraftingTableDisplayUI();
        }

        private void OnRightClick(int index)
        {
            handHasItem = itemInHand.HasItemData();
            slotHasItem = craftingTable.GetItemInputSlotAt(index).HasItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SplitItemSlotQuantityInInventoryAt(ref craftingTable.craftingInputSlots, index);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    craftingTable.AddNewItemAt(index, itemInHand.GetItemData());
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

            EventManager.GridChanged();
            UpdateCraftingTableDisplayUI();
        }


        public void OnEnter(GameObject clickedObject)
        {
            if (playerInputHandler.CurrentMouseState != PointerState.RightPressAfterWait)
            {
                return;
            }

            int index = GetItemSlotIndex(clickedObject);
            handHasItem = itemInHand.HasItemData();
            slotHasItem = craftingTable.GetItemInputSlotAt(index).HasItem();

            if (handHasItem == true)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    craftingTable.AddNewItemAt(index, itemInHand.GetItemData());
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
}
