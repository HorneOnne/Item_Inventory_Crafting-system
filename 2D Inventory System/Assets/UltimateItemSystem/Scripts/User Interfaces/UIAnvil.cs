using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UltimateItemSystem
{
    /// <summary>
    /// Singleton UIAnvil class for handling UI logic of Anvil.
    /// </summary>
    public class UIAnvil : Singleton<UIAnvil>
    {
        [Header("CONST VALUE")]
        // The maximum number of slots for materials.
        private const int MAX_MATERIALS_SLOT = 8;

        [Header("REFERCNES")]
        public Player player;               // A reference to the player.
        private ItemInHand itemInHand;
        private UIItemInHand uiItemInHand;
        private Anvil anvil;                // A reference to the anvil being used.
        [SerializeField] private Transform materialSlotsParent;

        [Header("UI REFERCNES")]
        [SerializeField] private GameObject uIMaterialNeededItemSlotPrefab;
        public UIItemSlot uiInputItem;          // The UI slot for the input item.
        public UIItemSlot uiOutputItem;         // The UI slot for the output item.

        [Header("CONTAINER")]
        public List<UIItemSlot> materialSlots;  // The list of UI slots for the needed materials.



        /// <summary>
        /// Initializes the UIAnvil class by setting default values, instantiating game objects and setting up event listeners.
        /// </summary>
        private void Start()
        {
            itemInHand = player.ItemInHand;
            uiItemInHand = UIItemInHand.Instance;
            materialSlots = new List<UIItemSlot>();

            // Set the data for the UI input item slot and add a pointer down event listener to it.
            uiInputItem.SetData(null);
            Utilities.AddEvent(uiInputItem.gameObject, EventTriggerType.PointerDown, (baseEvent) => OnItemInputSlotPointerDown(baseEvent, uiInputItem.gameObject));


            // Set the data for the UI output item slot and add a pointer down event listener to it.
            uiOutputItem.SetData(null);
            Utilities.AddEvent(uiOutputItem.gameObject, EventTriggerType.PointerDown, (baseEvent) => OnItemOutputSlotPointerDown(baseEvent, uiOutputItem.gameObject));


            // Create the UI slots for the needed materials, set their data, and add a pointer down event listener to them.
            for (int i = 0; i < MAX_MATERIALS_SLOT; i++)
            {
                var materialNeededObject = Instantiate(uIMaterialNeededItemSlotPrefab, materialSlotsParent).GetComponent<UIItemSlot>();
                materialNeededObject.SetIndex(i);
                materialNeededObject.SetData(null);

                Utilities.AddEvent(materialNeededObject.gameObject, EventTriggerType.PointerDown, (baseEvent) => OnMaterialInputSlotPointerDown(baseEvent, materialNeededObject.gameObject));
                materialNeededObject.gameObject.SetActive(false);
                materialSlots.Add(materialNeededObject);
            }


            // Deactivate the anvil UI canvas.
            UIManager.Instance.AnvilCanvas.SetActive(false);
        }


        /// <summary>
        /// Sets the Anvil reference for the UIAnvil class.
        /// </summary>
        /// <param name="anvil">Anvil object reference.</param>
        public void Set(Anvil anvil)
        {
            this.anvil = anvil;
        }



        #region Update UI
        /// <summary>
        /// Updates the UI of the anvil based on the anvil's current state
        /// </summary>
        public void UpdateUI()
        {
            if (anvil == null) return;

            // Set the input item slot data to the upgrade item input slot of the anvil
            uiInputItem.SetData(anvil.upgradeItemInputSlot);


            // Set the output item slot data to the upgrade item output slot of the anvil.
            if (anvil.IsUpgradePossible)
            {
                uiOutputItem.SetData(anvil.upgradeItemOutputSlot, 1.0f);
            }
            else
            {
                uiOutputItem.SetData(anvil.upgradeItemOutputSlot, 0.5f);
            }



            // Deactivate all material slots
            for (int i = 0; i < materialSlots.Count; i++)
            {
                if (materialSlots[i].gameObject.activeInHierarchy)
                    materialSlots[i].gameObject.SetActive(false);
            }

            // If the anvil has an output upgrade item, activate the required material slots and set their data
            if (anvil.HasOuputUpgradeItem() == true)
            {
                for (int i = 0; i < anvil.requiredMaterialSlots.Count; i++)
                {
                    materialSlots[i].gameObject.SetActive(true);
                    if (anvil.filledMaterialSlots[i].HasItem() == false)
                    {
                        materialSlots[i].SetData(anvil.requiredMaterialSlots[i], 0.5f);
                    }
                    else
                    {
                        materialSlots[i].SetData(anvil.filledMaterialSlots[i], 1.0f);
                    }
                }
            }

        }

        #endregion



        #region ItemInput Slot Logic

        /// <summary>
        /// Called when the pointer is down on the item input slot.
        /// </summary>
        /// <param name="baseEvent">The base event data.</param>
        /// <param name="clickedObject">The clicked object.</param>
        public void OnItemInputSlotPointerDown(BaseEventData baseEvent, GameObject clickedObject)
        {
            if (anvil == null) return;
            PointerEventData pointerEventData = (PointerEventData)baseEvent;

            if (pointerEventData.pointerId == -1)   // Mouse Left Event
            {
                OnItemInputSlotLeftClick(clickedObject);

            }
        }


        /// <summary>
        /// Handles the left click on the item input slot.
        /// </summary>
        /// <param name="clickedObj">The clicked object.</param>
        private void OnItemInputSlotLeftClick(GameObject clickedObj)
        {
            bool handHasItem = itemInHand.HasItemData();
            bool slotHasItem = anvil.HasInputUpgradeItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SetItem(new ItemSlot(anvil.upgradeItemInputSlot));
                    anvil.upgradeItemInputSlot.ClearSlot();

                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    bool canAdd = anvil.AddItemInputSlot(itemInHand.GetSlot());

                    if (canAdd)
                        itemInHand.ClearSlot();
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    bool isSameItem = ItemData.IsSameItem(anvil.upgradeItemInputSlot.ItemData, itemInHand.GetItemData());
                    if (isSameItem == false)
                    {
                        var tempUpgradeItemInputSlot = new ItemSlot(anvil.upgradeItemInputSlot);
                        bool canAdd = anvil.AddItemInputSlot(itemInHand.GetSlot());

                        if (canAdd)
                            itemInHand.SetItem(tempUpgradeItemInputSlot, -1, StoredType.Another, true);
                    }
                }
            }


            uiItemInHand.UpdateItemInHandUI();
            EventManager.TriggerInputUpgradeItemChangedEvent();
            UpdateUI();
        }
        #endregion



        #region ItemOutput Slot Click
        /// <summary>
        /// Called when the pointer is pressed down on the output upgrade item slot.
        /// </summary>
        /// <param name="baseEvent">The base event data.</param>
        /// <param name="clickedObject">The clicked game object.</param>
        public void OnItemOutputSlotPointerDown(BaseEventData baseEvent, GameObject clickedObject)
        {
            if (anvil == null) return;
            PointerEventData pointerEventData = (PointerEventData)baseEvent;

            if (pointerEventData.pointerId == -1)   // Mouse Left Event
            {
                OnItemOutputSlotLeftClick(clickedObject);

            }
        }



        /// <summary>
        /// Called when the left mouse button is clicked on the output upgrade item slot.
        /// </summary>
        /// <param name="clickedObj">The clicked game object.</param>
        private void OnItemOutputSlotLeftClick(GameObject clickedObj)
        {
            bool handHasItem = itemInHand.HasItemData();
            bool slotHasItem = anvil.HasOuputUpgradeItem();

            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    if (anvil == null) return;
                    if (anvil.IsUpgradePossible == false) return;

                    anvil.ComsumeMaterials();
                    itemInHand.SetItem(new ItemSlot(anvil.upgradeItemOutputSlot));
                    anvil.upgradeItemOutputSlot.ClearSlot();
                    anvil.upgradeItemInputSlot.ClearSlot();

                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                }
            }

            uiItemInHand.UpdateItemInHandUI();
            EventManager.TriggerInputUpgradeItemChangedEvent();
            UpdateUI();
        }

        #endregion



        #region Material InputSlot Logic
        /// <summary>
        /// Called when the pointer is down on a material input slot.
        /// </summary>
        /// <param name="baseEvent">The base event data.</param>
        /// <param name="clickedObject">The clicked game object.</param>
        public void OnMaterialInputSlotPointerDown(BaseEventData baseEvent, GameObject clickedObject)
        {
            if (anvil == null) return;
            PointerEventData pointerEventData = (PointerEventData)baseEvent;

            if (pointerEventData.pointerId == -1)   // Mouse Left Event
            {
                OnMaterialInputSlotLeftClick(clickedObject);
            }

            if (pointerEventData.pointerId == -2)   // Mouse Right Event
            {
                OnMaterialInputSlotRightClick(clickedObject);
            }
        }



        /// <summary>
        /// Called when the left button of the mouse is clicked on a material input slot.
        /// </summary>
        /// <param name="clickedObject">The clicked game object.</param>
        private void OnMaterialInputSlotLeftClick(GameObject clickedObject)
        {
            int index = GetSlotIndex(clickedObject);
            bool handHasItem = itemInHand.HasItemData();
            bool slotHasItem = anvil.filledMaterialSlots[index].HasItem();


            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.Swap(ref anvil.filledMaterialSlots, index, StoredType.Another, true);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    itemInHand.Swap(ref anvil.filledMaterialSlots, index, StoredType.Another, true);
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    bool isSameItem = ItemData.IsSameItem(anvil.filledMaterialSlots[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        ItemSlot remainItems = anvil.filledMaterialSlots[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                        itemInHand.SetItem(remainItems, index, StoredType.PlayerInventory, true);
                    }
                    else
                    {
                        itemInHand.Swap(ref anvil.filledMaterialSlots, index, StoredType.PlayerInventory, true);
                    }

                }
            }


            uiItemInHand.UpdateItemInHandUI();
            EventManager.TriggerMaterialInputUpgradeItemChangedEvent();
            UpdateUI();
        }


        /// <summary>
        /// Called when the right button of the mouse is clicked on a material input slot.
        /// </summary>
        /// <param name="clickedObject">The clicked game object.</param>
        private void OnMaterialInputSlotRightClick(GameObject clickedObject)
        {
            int index = GetSlotIndex(clickedObject);
            bool handHasItem = itemInHand.HasItemData();
            bool slotHasItem = anvil.filledMaterialSlots[index].HasItem();


            if (handHasItem == false)
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                }
                else
                {
                    //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                    itemInHand.SplitItemSlotQuantityInInventory(ref anvil.filledMaterialSlots, index);
                }
            }
            else
            {
                if (slotHasItem == false)
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                    anvil.filledMaterialSlots[index].AddNewItem(itemInHand.GetItemData());
                    itemInHand.RemoveItem();
                }
                else
                {
                    //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                    bool isSameItem = ItemData.IsSameItem(anvil.filledMaterialSlots[index].ItemData, itemInHand.GetItemData());
                    if (isSameItem)
                    {
                        bool isSlotNotFull = anvil.filledMaterialSlots[index].AddItem();

                        if (isSlotNotFull)
                        {
                            itemInHand.RemoveItem();
                        }
                    }

                }
            }


            uiItemInHand.UpdateItemInHandUI();
            EventManager.TriggerMaterialInputUpgradeItemChangedEvent();
            UpdateUI();
        }


        /// <summary>
        /// Gets the index of the item slot that was clicked.
        /// </summary>
        /// <param name="clickedObject">The GameObject that was clicked.</param>
        /// <returns>The index of the item slot.</returns>
        private int GetSlotIndex(GameObject clickedObject)
        {
            return clickedObject.GetComponent<UIItemSlot>().SlotIndex;
        }

        #endregion

    }
}
