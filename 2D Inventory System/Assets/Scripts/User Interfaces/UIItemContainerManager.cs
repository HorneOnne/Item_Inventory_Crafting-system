using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using MyGame.Ultilities;


public class UIItemContainerManager : Singleton<UIItemContainerManager>
{
    [Header("Containers")]
    public List<ItemData> itemDataList;
    public List<GameObject> UI_SlotList;

    [Header("UI")]
    public GameObject itemSlotPrefab;

    [Header("References")]
    [SerializeField] Transform contentPanel;
    [SerializeField] TextMeshProUGUI currentItemTypeText;
    private GameObject uiCraftingTableCanvas;


    [Header("TagItemContainerReferences")]
    [SerializeField] GameObject allTabObject;
    [SerializeField] GameObject weaponTabObject;
    [SerializeField] GameObject toolTabObject;
    private GameObject currentTagSelected;



    [Header("Logic References")]
    public Player player;
    private ItemInHand itemInHand;



    private void Start()
    {
        itemInHand = player.ItemInHand;
        uiCraftingTableCanvas = UIManager.Instance.CraftingTableCanvas;
        LoadAllItems();
    }



    public void LoadAllItem()
    {
        currentItemTypeText.text = "All Items";

        foreach (var slot in UI_SlotList)
        {
            Destroy(slot.gameObject);
        }
        UI_SlotList.Clear();
        itemDataList.Clear();

        int index = 0;
        foreach(var item in ItemContainerManager.Instance.itemDataDict.Keys)
        {
            GameObject slotObject = Instantiate(itemSlotPrefab, contentPanel);
            Utilities.AddEvent(slotObject, EventTriggerType.PointerClick, (baseEvent) => OnSlotClicked(baseEvent, slotObject));
            slotObject.GetComponent<UIItemSlot>().SetIndex(index);
            slotObject.GetComponent<UIItemSlot>().SetData(new ItemSlot(item, 1));

            UI_SlotList.Add(slotObject);
            itemDataList.Add(item);

            index++;
        }
    }

    private void LoadAllItemInType(GeneralItemType generalItemType)
    {
        foreach (var slot in UI_SlotList)
        {
            Destroy(slot.gameObject);
        }
        UI_SlotList.Clear();
        itemDataList.Clear();

        int index = 0;
        foreach (var item in ItemContainerManager.Instance.itemDataDict.Keys)
        {
            if (GeneralItemTypeContainer.GetAllItemInType(generalItemType).Contains(item.itemType))
            {
                GameObject slotObject = Instantiate(itemSlotPrefab, contentPanel);
                Utilities.AddEvent(slotObject, EventTriggerType.PointerClick, (baseEvent) => OnSlotClicked(baseEvent, slotObject));
                slotObject.GetComponent<UIItemSlot>().SetIndex(index);
                slotObject.GetComponent<UIItemSlot>().SetData(new ItemSlot(item, 1));

                UI_SlotList.Add(slotObject);
                itemDataList.Add(item);

                index++;
            }

        }
    }

    // Logic
    // =======================================================================
    private void OnSlotClicked(BaseEventData baseEvent, GameObject clickedObject)
    {
        PointerEventData pointerEventData = (PointerEventData)baseEvent;

        if (pointerEventData.pointerId == -1)   // Mouse Left Event
        {
            ItemData itemData = itemDataList[clickedObject.GetComponent<UIItemSlot>().SlotIndex];
            if (player.PlayerInputHandler.PressUtilityKeyInput)
                itemInHand.Set(new ItemSlot(itemData, itemData.max_quantity), -1, StoredType.Another, true);
            else
                itemInHand.Set(new ItemSlot(itemData, 1), -1, StoredType.Another, true);
        }


        if (pointerEventData.pointerId == -2)   // Mouse Right Event
        {
            ItemData itemData = itemDataList[clickedObject.GetComponent<UIItemSlot>().SlotIndex];
            if (uiCraftingTableCanvas.activeInHierarchy)
            {
                Debug.Log("Do here.");
                var recipe = ItemRecipeManager.Instance.GetRecipeFromItem(itemData);
                CraftingTable.Instance.FillCraftingSuggestionData(recipe);
                UICraftingTable.Instance.UpdateCraftingTableDisplayUI();

            }
        } 
    }

    // =======================================================================

    public void LoadAllItems()
    {
        if (currentTagSelected == allTabObject) return;

        AnimateUITagSelected(allTabObject);
        LoadAllItem();
    }

    public void LoadTools()
    {
        if (currentTagSelected == toolTabObject) return;

        AnimateUITagSelected(toolTabObject);

        LoadAllItemInType(GeneralItemType.Tools);
        currentItemTypeText.text = "Tools";
    }

    public void LoadWeapons()
    {
        if (currentTagSelected == weaponTabObject) return;

        AnimateUITagSelected(weaponTabObject);

        LoadAllItemInType(GeneralItemType.Weapons);
        currentItemTypeText.text = "Weapons";
    }



   

    #region Animation Item Type Tag Selected;
    // =====================================
    private void AnimateUITagSelected(GameObject newItemSlotTag)
    {
        if(currentTagSelected == null)
            currentTagSelected = newItemSlotTag;
        else
        {
            if (currentTagSelected == newItemSlotTag) return;
        }
        
        StartCoroutine(ScaleDownUITagSelected());      
        currentTagSelected = newItemSlotTag;
        StartCoroutine(ScaleUpUITagSelected());
    }

    private IEnumerator ScaleUpUITagSelected()
    {
        var rt = currentTagSelected.GetComponent<RectTransform>();
        for (float i = 1.0f; i <= 1.3f; i+= 0.1f)
        {
            rt.localScale = Vector3.one * i;
            yield return null;
        }
    }

    private IEnumerator ScaleDownUITagSelected()
    {
        var rt = currentTagSelected.GetComponent<RectTransform>();
        if (rt.localScale == Vector3.one) yield break;

        for (float i = 1.3f; i >= 1.0; i -= 0.1f)
        {
            rt.localScale = Vector3.one * i;
            yield return null;
        }

        rt.localScale = Vector3.one;
    }
    // =====================================
    #endregion Animation Item Type Tag Selected;
}
