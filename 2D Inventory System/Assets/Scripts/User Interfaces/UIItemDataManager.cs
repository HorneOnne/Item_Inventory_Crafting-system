using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


namespace DIVH_InventorySystem
{
    public class UIItemDataManager : Singleton<UIItemDataManager>
    {
        [Header("DATA")]
        public List<ItemData> itemDataList;
        public List<GameObject> UI_SlotList;

        [Header("UI PREFAB")]
        public GameObject itemSlotPrefab;

        [Header("REFERENCES")]
        public Player player;
        private ItemInHand itemInHand;
        private ItemDataManager itemDataManager;
        [SerializeField] Transform contentPanel;
        [SerializeField] TextMeshProUGUI currentItemTypeText;
        private GameObject uiCraftingTableCanvas;



        [Header("TAG")]
        [SerializeField] GameObject allTabObject;
        [SerializeField] GameObject weaponTabObject;
        [SerializeField] GameObject toolTabObject;
        private GameObject currentTagSelected;







        private void Start()
        {
            itemInHand = player.ItemInHand;
            uiCraftingTableCanvas = UIManager.Instance.CraftingTableCanvas;
            itemDataManager = ItemDataManager.Instance;
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
            foreach (var item in ItemDataManager.Instance.itemDataDict.Keys)
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
            foreach (var item in ItemDataManager.Instance.itemDataDict.Keys)
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
                    var recipe = itemDataManager.GetRecipeFromItem(itemData);
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
            if (currentTagSelected == null)
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
            for (float i = 1.0f; i <= 1.3f; i += 0.1f)
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
}