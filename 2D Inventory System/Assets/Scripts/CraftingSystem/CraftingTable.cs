using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DIVH_InventorySystem
{
    public class CraftingTable : Singleton<CraftingTable>
    {
        [Header("CONST VALUE")]
        private const int NUM_OF_GRID = 9;


        [Header("DATA")]
        [HideInInspector] public ItemSlot[] craftingInputSlots;
        [HideInInspector] public ItemSlot craftingOutputSlot;
        [HideInInspector] public ItemSlot[] craftingSuggestionInputSlots;
        [HideInInspector] public ItemSlot craftingSuggestionOutputSlot;


        [Header("REFERENCES")]
        public Player player;
        private ItemInHand itemInHand;
        private ItemDataManager itemDataManager;


        #region Properties
        public int GridLength { get => NUM_OF_GRID; }
        #endregion


        private void OnEnable()
        {
            EventManager.OnGridChanged += LookupItemFromRecipe;
            EventManager.OnGetOutputItem += ComsumeCrafingMaterial;

        }

        private void OnDisable()
        {
            EventManager.OnGridChanged -= LookupItemFromRecipe;
            EventManager.OnGetOutputItem -= ComsumeCrafingMaterial;
        }


        private void Start()
        {
            itemDataManager = ItemDataManager.Instance;

            itemInHand = player.ItemInHand;
            craftingInputSlots = new ItemSlot[NUM_OF_GRID];
            craftingSuggestionInputSlots = new ItemSlot[NUM_OF_GRID];
            craftingOutputSlot = new ItemSlot();
            craftingSuggestionOutputSlot = new ItemSlot();

            for (int i = 0; i < NUM_OF_GRID; i++)
            {
                craftingInputSlots[i] = new ItemSlot();
                craftingSuggestionInputSlots[i] = new ItemSlot();
            }
        }

        public ItemSlot GetItemInputSlotAt(int index)
        {
            if (index < 0 || index >= NUM_OF_GRID) return null;
            return craftingInputSlots[index];
        }


        private void LookupItemFromRecipe()
        {
            var currentRecipe = CreateRecipe();
            craftingOutputSlot = itemDataManager.GetItemFromRecipe(currentRecipe);
        }



        private void ComsumeCrafingMaterial()
        {
            for (int i = 0; i < craftingInputSlots.Length; i++)
            {
                if (craftingInputSlots[i].HasItem())
                {
                    craftingInputSlots[i].RemoveItem();
                }
            }
        }

        public void AddNewItemAt(int index, ItemData item)
        {
            craftingInputSlots[index].AddNewItem(item);
        }

        public void RemoveItemFromCraftingGridAtIndex(int index)
        {
            craftingInputSlots[index].RemoveItem();
        }

        public RecipeData CreateRecipe()
        {
            RecipeData newRecipe = ScriptableObject.CreateInstance<RecipeData>();
            newRecipe.item00 = craftingInputSlots[0].ItemData;
            newRecipe.item10 = craftingInputSlots[1].ItemData;
            newRecipe.item20 = craftingInputSlots[2].ItemData;

            newRecipe.item01 = craftingInputSlots[3].ItemData;
            newRecipe.item11 = craftingInputSlots[4].ItemData;
            newRecipe.item21 = craftingInputSlots[5].ItemData;

            newRecipe.item02 = craftingInputSlots[6].ItemData;
            newRecipe.item12 = craftingInputSlots[7].ItemData;
            newRecipe.item22 = craftingInputSlots[8].ItemData;

            return newRecipe;
        }


        public void FillCraftingSuggestionData(RecipeData itemRecipe)
        {
            if (itemRecipe == null)
            {
                for (int i = 0; i < craftingSuggestionInputSlots.Length; i++)
                    craftingSuggestionInputSlots[i].ClearSlot();

                craftingSuggestionOutputSlot.ClearSlot();

            }
            else
            {
                craftingSuggestionInputSlots[0] = new ItemSlot(itemRecipe.item00, 1);
                craftingSuggestionInputSlots[1] = new ItemSlot(itemRecipe.item10, 1);
                craftingSuggestionInputSlots[2] = new ItemSlot(itemRecipe.item20, 1);

                craftingSuggestionInputSlots[3] = new ItemSlot(itemRecipe.item01, 1);
                craftingSuggestionInputSlots[4] = new ItemSlot(itemRecipe.item11, 1);
                craftingSuggestionInputSlots[5] = new ItemSlot(itemRecipe.item21, 1);

                craftingSuggestionInputSlots[6] = new ItemSlot(itemRecipe.item02, 1);
                craftingSuggestionInputSlots[7] = new ItemSlot(itemRecipe.item12, 1);
                craftingSuggestionInputSlots[8] = new ItemSlot(itemRecipe.item22, 1);


                craftingSuggestionOutputSlot = itemDataManager.GetItemFromRecipe(itemRecipe);
            }

        }



        public bool HasItem(int index)
        {
            if (HasSlot(index) == false) return false;

            try
            {
                craftingInputSlots[index].HasItem();
            }
            catch
            {
                return false;
            }

            return true;
        }


        public bool HasOutputSlot()
        {
            return craftingOutputSlot != null;
        }

        public bool HasSlot(int index)
        {
            return index >= 0 && index < craftingInputSlots.Length;
        }

        public ItemData GetItem(int index)
        {
            if (HasItem(index) == false) return null;
            return craftingInputSlots[index].ItemData;
        }

        public bool AddItem(int index)
        {
            bool isSlotNotFull = craftingInputSlots[index].AddItem();
            return isSlotNotFull;
        }

        public void StackItem()
        {
            if (itemInHand.GetItemData() == null) return;
            Dictionary<int, int> dict = new Dictionary<int, int>();
            Dictionary<int, int> sortedDict = new Dictionary<int, int>();

            for (int i = 0; i < craftingInputSlots.Length; i++)
            {
                if (craftingInputSlots[i].ItemData == itemInHand.GetItemData())
                {
                    dict.Add(i, craftingInputSlots[i].ItemQuantity);
                }
            }

            // Use OrderBy to sort the dictionary by value
            sortedDict = dict.OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var e in sortedDict)
            {
                itemInHand.GetSlot().AddItemsFromAnotherSlot(craftingInputSlots[e.Key]);
                UIItemInHand.Instance.UpdateItemInHandUI();
                UICraftingTable.Instance.UpdateCraftingTableDisplayUIAt(e.Key);
            }
        }
    }

}