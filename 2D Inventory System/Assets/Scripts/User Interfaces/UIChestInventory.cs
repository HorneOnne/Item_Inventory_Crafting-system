using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace DIVH_InventorySystem
{
    public class UIChestInventory : Singleton<UIChestInventory>
    {
        [Header("Inventory References")]
        public Player player;
        private ItemInHand itemInHand;
        private PlayerInputHandler playerInputHandler;
        private ChestInventory chestInventory;


        [Header("Containers")]
        public List<GameObject> itemSlotList;

        [Header("UI")]
        public GameObject itemSlotPrefab;

        [Header("UI Event Properties")]
        private GameObject currentSlotDrag;
        private GameObject startingSlotDrag;
        private GameObject currentSlotClicked;


        [Header("Inventory Settings")]
        public DragType dragType;

        // right press
        [SerializeField] float pressIntervalTime = 1.0f;
        private float pressIntervalTimeCount = 0.0f;



        private bool hasChestInventoryData;

        private const int MAX_NORMAL_CHEST_SLOT = 36;


        //Cache
        private bool handHasItem;
        private bool slotHasItem;

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
            playerInputHandler = player.PlayerInputHandler;
            itemInHand = this.player.ItemInHand;

            UpdateInventoryUI();
        }


        public void RemoveChestInventoryData()
        {
            this.chestInventory = null;
            hasChestInventoryData = false;

            this.player = null;
            playerInputHandler = null;
            itemInHand = null;
        }


        private void Update()
        {
            if (hasChestInventoryData == false) return;


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
                    bool isSameItem = ItemData.IsSameItem(chestInventory.inventory[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = chestInventory.inventory[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.Set(remainItems, index, StoredType.ChestInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref chestInventory.inventory, index, StoredType.ChestInventory, true);

                        if (dragType == DragType.Swap)
                        {
                            int startingSlotIndex = GetItemSlotIndex(startingSlotDrag);
                            if (chestInventory.inventory[startingSlotIndex].HasItem() == false)
                            {
                                startingSlotDrag = null;
                                itemInHand.Swap(ref chestInventory.inventory, startingSlotIndex, StoredType.ChestInventory, true);
                            }
                        }

                    }
                }
                else
                {
                    itemInHand.Swap(ref chestInventory.inventory, index, StoredType.ChestInventory, true);
                }
                UpdateInventoryUI();
            }
        }
        #endregion



        #region Inventory interactive methods

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
                        itemInHand.Set(remainItems, index, StoredType.ChestInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref chestInventory.inventory, index, StoredType.ChestInventory, true);
                    }
                }
            }
        }

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
                    itemInHand.SplitItemSlotQuantityInInventoryAt(ref chestInventory.inventory, index);
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