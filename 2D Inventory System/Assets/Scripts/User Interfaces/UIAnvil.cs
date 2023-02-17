using MyGame.Ultilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIAnvil : Singleton<UIAnvil>
{
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


        for (int i = 0; i < 4; i++)
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

        uiInputItem.SetData(anvil.UpgradeItemInputSlot);


        if(anvil.IsSufficient)
        {
            uiOutputItem.SetData(anvil.UpgradeItemOutputSlot, 1.0f);
        }
        else
        {
            uiOutputItem.SetData(anvil.UpgradeItemOutputSlot, 0.5f);
        }



        if (anvil.HasOuputUpgradeItem() == false)
        {
            for (int i = 0; i < materialSlots.Count; i++)
                materialSlots[i].gameObject.SetActive(false);
        }
        else
        {
            for (int i = 0; i < anvil.MaterialsNeededToUpgrade.Count; i++)
            {
                materialSlots[i].gameObject.SetActive(true);
                if (anvil.MaterialsHasBeenFilled[i].HasItem() == false)
                {
                    materialSlots[i].SetData(anvil.MaterialsNeededToUpgrade[i], 0.5f);
                }
                else
                {
                    materialSlots[i].SetData(anvil.MaterialsHasBeenFilled[i], 1.0f);
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
            OnItemInputSlotLeftClick(clickedObject,
                        PerformItemInputSlot_HandEmptySlotEmptyClick,
                        PerformItemInputSlot_HandEmptySlotHasClick,
                        PerformItemInputSlot_HandHasSlotEmptyClick,
                        PerformItemInputSlot_HandHasSlotHasClick);

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
    private void OnItemInputSlotLeftClick(GameObject clickedObj, Action handEmptySlotEmpty, Action handEmptySlotHas,
        Action handHasSlotEmpty, Action handHasSlotHas)
    {
        bool handHasItem = itemInHand.HasItemData();
        bool slotHasItem = anvil.HasInputUpgradeItem();

        if (handHasItem == false)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                handEmptySlotEmpty();
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                handEmptySlotHas();

            }
        }
        else
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                handHasSlotEmpty();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                handHasSlotHas();
            }
        }


        UIItemInHand.Instance.UpdateItemInHandUI();
        EventManager.InputUpgradeItemChanged();
        UpdateUI();
    }


    private void PerformItemInputSlot_HandEmptySlotEmptyClick()
    {

    }

    private void PerformItemInputSlot_HandEmptySlotHasClick()
    {
        itemInHand.Set(new ItemSlot(anvil.UpgradeItemInputSlot), new ItemSlotData());
        anvil.UpgradeItemInputSlot.ClearSlot();
    }

    private void PerformItemInputSlot_HandHasSlotEmptyClick()
    {
        if (itemInHand.GetItemData() is UpgradeableItemData == false) return;

        anvil.UpgradeItemInputSlot = new ItemSlot(itemInHand.GetSlot());
        itemInHand.ClearSlot();
    }

    private void PerformItemInputSlot_HandHasSlotHasClick()
    {

    }

    #endregion



    #region ItemOutput Slot Click
    public void OnItemOutputSlotPointerDown(BaseEventData baseEvent, GameObject clickedObject)
    {
        if (anvil == null) return;
        PointerEventData pointerEventData = (PointerEventData)baseEvent;

        if (pointerEventData.pointerId == -1)   // Mouse Left Event
        {
            OnItemOutputSlotLeftClick(clickedObject,
                        PerformItemOutputSlot_HandEmptySlotEmptyClick,
                        PerformItemOutputSlot_HandEmptySlotHasClick,
                        PerformItemOutputSlot_HandHasSlotEmptyClick,
                        PerformItemOutputSlot_HandHasSlotHasClick);

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
    private void OnItemOutputSlotLeftClick(GameObject clickedObj, Action handEmptySlotEmpty, Action handEmptySlotHas,
        Action handHasSlotEmpty, Action handHasSlotHas)
    {
        bool handHasItem = itemInHand.HasItemData();
        bool slotHasItem = anvil.HasOuputUpgradeItem();

        if (handHasItem == false)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                handEmptySlotEmpty();
            }
            else
            {
                //Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                handEmptySlotHas();

            }
        }
        else
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                handHasSlotEmpty();
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                handHasSlotHas();
            }
        }


        UIItemInHand.Instance.UpdateItemInHandUI();
        EventManager.InputUpgradeItemChanged();
        UpdateUI();
    }


    private void PerformItemOutputSlot_HandEmptySlotEmptyClick()
    {

    }

    private void PerformItemOutputSlot_HandEmptySlotHasClick()
    {
        if (anvil == null) return;
        if (anvil.IsSufficient == false) return;

        itemInHand.Set(new ItemSlot(anvil.UpgradeItemOutputSlot), new ItemSlotData());
        anvil.UpgradeItemOutputSlot.ClearSlot();

        // Remove itemInputUpgrade and materials need to upgrade.
        anvil.UpgradeItemInputSlot.ClearSlot();
    }

    private void PerformItemOutputSlot_HandHasSlotEmptyClick()
    {
       
    }

    private void PerformItemOutputSlot_HandHasSlotHasClick()
    {

    }


   

    #endregion



    #region Material InputSlot Logic
    public void OnMaterialInputSlotPointerDown(BaseEventData baseEvent, GameObject clickedObject)
    {
        if (anvil == null) return;
        PointerEventData pointerEventData = (PointerEventData)baseEvent;

        if (pointerEventData.pointerId == -1)   // Mouse Left Event
        {
            OnMaterialInputSlotLeftClick(clickedObject,
                        PerformMaterialInputSlot_HandEmptySlotEmptyClick,
                        PerformMaterialInputSlot_HandEmptySlotHasClick,
                        PerformMaterialInputSlot_HandHasSlotEmptyClick,
                        PerformMaterialInputSlot_HandHasSlotHasClick);

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
    private void OnMaterialInputSlotLeftClick(GameObject clickedObj, Action<GameObject> handEmptySlotEmpty, Action<GameObject> handEmptySlotHas,
        Action<GameObject> handHasSlotEmpty, Action<GameObject> handHasSlotHas)
    {
        int index = GetSlotIndex(clickedObj);
        bool handHasItem = itemInHand.HasItemData();
        bool slotHasItem = anvil.MaterialsHasBeenFilled[index].HasItem();


        if (handHasItem == false)
        {
            if (slotHasItem == false)
            {
                //Debug.Log("HAND: EMPTY \t SLOT: EMPTY");
                handEmptySlotEmpty(clickedObj);
            }
            else
            {
                Debug.Log("HAND: EMPTY \t SLOT: HAS ITEM");
                handEmptySlotHas(clickedObj);

            }
        }
        else
        {
            if (slotHasItem == false)
            {
                Debug.Log("HAND: HAS ITEM \t SLOT: EMPTY");
                handHasSlotEmpty(clickedObj);
            }
            else
            {
                //Debug.Log("HAND: HAS ITEM \t SLOT: HAS ITEM");
                handHasSlotHas(clickedObj);
            }
        }


        UIItemInHand.Instance.UpdateItemInHandUI();
        EventManager.MaterialInputUpgradeItemChanged();
        UpdateUI();
    }


    private void PerformMaterialInputSlot_HandEmptySlotEmptyClick(GameObject clickedObject)
    {

    }

    private void PerformMaterialInputSlot_HandEmptySlotHasClick(GameObject clickedObject)
    {
        int index = GetSlotIndex(clickedObject);
        itemInHand.Set(new ItemSlot(anvil.MaterialsHasBeenFilled[index]), new ItemSlotData());
        anvil.MaterialsHasBeenFilled[index].ClearSlot();
    }

    private void PerformMaterialInputSlot_HandHasSlotEmptyClick(GameObject clickedObject)
    {
        int index = GetSlotIndex(clickedObject);
        anvil.MaterialsHasBeenFilled[index] = new ItemSlot(itemInHand.GetSlot());
        itemInHand.ClearSlot();
    }

    private void PerformMaterialInputSlot_HandHasSlotHasClick(GameObject clickedObject)
    {

    }


    private int GetSlotIndex(GameObject clickedObject)
    {
        return clickedObject.GetComponent<UIItemSlot>().SlotIndex;
    }

    #endregion

}
