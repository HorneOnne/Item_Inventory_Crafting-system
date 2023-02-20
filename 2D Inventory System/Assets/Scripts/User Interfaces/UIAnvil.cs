using MyGame.Ultilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class UIAnvil : Singleton<UIAnvil>
{
    private const int MAX_MATERIALS_SLOT = 8;

    [Header("References")]
    public Player player;
    private ItemInHand itemInHand;
    private Anvil anvil;
    [SerializeField] private Transform materialSlotsParent;
    [SerializeField] private GameObject uIMaterialNeededItemSlotPrefab;

    public UIItemSlot uiInputItem;
    public UIItemSlot uiOutputItem;
    public List<UIItemSlot> materialSlots;



    private void Start()
    {
        itemInHand = player.ItemInHand;
        materialSlots = new List<UIItemSlot>();

        uiInputItem.SetData(null);
        Utilities.AddEvent(uiInputItem.gameObject, EventTriggerType.PointerDown, (baseEvent) => OnItemInputSlotPointerDown(baseEvent, uiInputItem.gameObject));

        uiOutputItem.SetData(null);
        Utilities.AddEvent(uiOutputItem.gameObject, EventTriggerType.PointerDown, (baseEvent) => OnItemOutputSlotPointerDown(baseEvent, uiOutputItem.gameObject));


        for (int i = 0; i < MAX_MATERIALS_SLOT; i++)
        {
            var materialNeededObject = Instantiate(uIMaterialNeededItemSlotPrefab, materialSlotsParent).GetComponent<UIItemSlot>();
            materialNeededObject.SetIndex(i);
            materialNeededObject.SetData(null);

            Utilities.AddEvent(materialNeededObject.gameObject, EventTriggerType.PointerDown, (baseEvent) => OnMaterialInputSlotPointerDown(baseEvent, materialNeededObject.gameObject));
            materialNeededObject.gameObject.SetActive(false);
            materialSlots.Add(materialNeededObject);
        }


        // Deactive UI after settings.
        UIManager.Instance.AnvilCanvas.SetActive(false);
    }

    public void Set(Anvil anvil)
    {
        this.anvil = anvil;
    }

    #region Update UI
    public void UpdateUI()
    {
        if (anvil == null) return;

        uiInputItem.SetData(anvil.upgradeItemInputSlot);


        if(anvil.IsSufficient)
        {
            uiOutputItem.SetData(anvil.upgradeItemOutputSlot, 1.0f);
        }
        else
        {
            uiOutputItem.SetData(anvil.upgradeItemOutputSlot, 0.5f);
        }



        for (int i = 0; i < materialSlots.Count; i++)
        {
            if(materialSlots[i].gameObject.activeInHierarchy)
                materialSlots[i].gameObject.SetActive(false);
        }
        if (anvil.HasOuputUpgradeItem() == true)
        {
            for (int i = 0; i < anvil.materialsNeededToUpgrade.Count; i++)
            {
                materialSlots[i].gameObject.SetActive(true);
                if (anvil.materialsHasBeenFilled[i].HasItem() == false)
                {
                    materialSlots[i].SetData(anvil.materialsNeededToUpgrade[i], 0.5f);
                }
                else
                {
                    materialSlots[i].SetData(anvil.materialsHasBeenFilled[i], 1.0f);
                }
            }
        }

    }

    #endregion



    #region ItemInput Slot Logic
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
    /// 
    /// </summary>
    /// <param name="clickedObj"></param>
    /// <param name="handEmptySlotEmpty"></param>
    /// <param name="handEmptySlotHas"></param>
    /// <param name="handHasSlotEmpty"></param>
    /// <param name="handHasSlotHas"></param>
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
                itemInHand.Set(new ItemSlot(anvil.upgradeItemInputSlot));
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

                    if(canAdd)
                        itemInHand.Set(tempUpgradeItemInputSlot, -1, StoredType.Another, true);
                }
            }
        }


        UIItemInHand.Instance.UpdateItemInHandUI();
        EventManager.InputUpgradeItemChanged();
        UpdateUI();
    }
    #endregion



    #region ItemOutput Slot Click
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
    /// 
    /// </summary>
    /// <param name="clickedObj"></param>
    /// <param name="handEmptySlotEmpty"></param>
    /// <param name="handEmptySlotHas"></param>
    /// <param name="handHasSlotEmpty"></param>
    /// <param name="handHasSlotHas"></param>
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
                if (anvil.IsSufficient == false) return;

                anvil.ComsumeMaterials();
                itemInHand.Set(new ItemSlot(anvil.upgradeItemOutputSlot));
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

        UIItemInHand.Instance.UpdateItemInHandUI();
        EventManager.InputUpgradeItemChanged();
        UpdateUI();
    }

    #endregion



    #region Material InputSlot Logic
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
    /// 
    /// </summary>
    /// <param name="clickedObj"></param>
    /// <param name="handEmptySlotEmpty"></param>
    /// <param name="handEmptySlotHas"></param>
    /// <param name="handHasSlotEmpty"></param>
    /// <param name="handHasSlotHas"></param>
    private void OnMaterialInputSlotLeftClick(GameObject clickedObject)
    {
        int index = GetSlotIndex(clickedObject);
        bool handHasItem = itemInHand.HasItemData();
        bool slotHasItem = anvil.materialsHasBeenFilled[index].HasItem();


        if (handHasItem == false)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                itemInHand.Swap(ref anvil.materialsHasBeenFilled, index, StoredType.Another, true);
            }
        }
        else
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                itemInHand.Swap(ref anvil.materialsHasBeenFilled, index, StoredType.Another, true);
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                bool isSameItem = ItemData.IsSameItem(anvil.materialsHasBeenFilled[index].ItemData, itemInHand.GetItemData());
                if (isSameItem)
                {
                    ItemSlot remainItems = anvil.materialsHasBeenFilled[index].AddItemsFromAnotherSlot(itemInHand.GetSlot());
                    itemInHand.Set(remainItems, index, StoredType.PlayerInventory, true);
                }
                else
                {
                    itemInHand.Swap(ref anvil.materialsHasBeenFilled, index, StoredType.PlayerInventory, true);
                }

            }
        }


        UIItemInHand.Instance.UpdateItemInHandUI();
        EventManager.MaterialInputUpgradeItemChanged();
        UpdateUI();
    }


    private void OnMaterialInputSlotRightClick(GameObject clickedObject)
    {
        int index = GetSlotIndex(clickedObject);
        bool handHasItem = itemInHand.HasItemData();
        bool slotHasItem = anvil.materialsHasBeenFilled[index].HasItem();


        if (handHasItem == false)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                itemInHand.SplitItemSlotQuantityInInventoryAt(ref anvil.materialsHasBeenFilled, index);
            }
        }
        else
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                anvil.materialsHasBeenFilled[index].AddNewItem(itemInHand.GetItemData());
                itemInHand.RemoveItem();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                bool isSameItem = ItemData.IsSameItem(anvil.materialsHasBeenFilled[index].ItemData, itemInHand.GetItemData());
                if (isSameItem)
                {
                    bool isSlotNotFull =  anvil.materialsHasBeenFilled[index].AddItem();

                    if (isSlotNotFull)
                    {
                        itemInHand.RemoveItem();
                    }
                }

            }
        }


        UIItemInHand.Instance.UpdateItemInHandUI();
        EventManager.MaterialInputUpgradeItemChanged();
        UpdateUI();
    }


    private int GetSlotIndex(GameObject clickedObject)
    {
        return clickedObject.GetComponent<UIItemSlot>().SlotIndex;
    }

    #endregion

}
