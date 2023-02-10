using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using MyGame.Ultilities;


public class UIItemContainerManager : Singleton<UIItemContainerManager>
{
    [Header("Containers")]
    public List<ItemData> itemScriptableObjectList;
    public List<GameObject> UI_SlotList;

    [Header("UI")]
    public GameObject itemSlotPrefab;

    [Header("References")]
    [SerializeField] Transform contentPanel;
    [SerializeField] TextMeshProUGUI currentItemTypeText;


    [Header("TagItemContainerReferences")]
    [SerializeField] GameObject allTabObject;
    [SerializeField] GameObject weaponTabObject;
    [SerializeField] GameObject toolTabObject;
    private GameObject currentTagSelected;



    [Header("Logic References")]
    public Player player;
    private ItemInHand itemInHand;
    private UIItemInHand uiItemInHand;



    private void Start()
    {
        itemInHand = player.ItemInHand;
        uiItemInHand = UIItemInHand.Instance;
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
        itemScriptableObjectList.Clear();

        int index = 0;
        foreach(var item in ItemContainerManager.Instance.itemDataDict.Keys)
        {
            GameObject slotObject = Instantiate(itemSlotPrefab, contentPanel);
            Utilities.AddEvent(slotObject, EventTriggerType.PointerClick, (baseEvent) => OnSlotClicked(baseEvent, slotObject));
            slotObject.GetComponent<UIItemSlot>().Set(item.icon, null, (short)index);

            UI_SlotList.Add(slotObject);
            itemScriptableObjectList.Add(item);

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
        itemScriptableObjectList.Clear();

        int index = 0;
        foreach (var item in ItemContainerManager.Instance.itemDataDict.Keys)
        {
            if (GeneralItemTypeContainer.GetAllItemInType(generalItemType).Contains(item.itemType))
            {
                GameObject slotObject = Instantiate(itemSlotPrefab, contentPanel);
                Utilities.AddEvent(slotObject, EventTriggerType.PointerClick, (baseEvent) => OnSlotClicked(baseEvent, slotObject));
                slotObject.GetComponent<UIItemSlot>().Set(item.icon, null, (short)index);

                UI_SlotList.Add(slotObject);
                itemScriptableObjectList.Add(item);

                index++;
            }

        }
    }

    // Logic
    // =======================================================================
    private void OnSlotClicked(BaseEventData baseEvent, GameObject clickedObj)
    {
        ItemData item = itemScriptableObjectList[clickedObj.GetComponent<UIItemSlot>().SlotIndex];
        itemInHand.Set(new ItemSlot(item, item.max_quantity), StoredType.Another);
        uiItemInHand.DisplayItemInHand();
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
