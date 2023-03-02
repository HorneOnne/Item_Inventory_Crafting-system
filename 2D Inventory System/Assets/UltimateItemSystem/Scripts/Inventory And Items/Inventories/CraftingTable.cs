using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateItemSystem
{
    public class CraftingTable : Singleton<CraftingTable>
    {
        [Header("CONST VALUE")]
        private const int NUM_OF_GRID = 9;


        [Header("DATA")]
        [HideInInspector] public ItemSlot[] inputSlots;             // ingredient slots
        [HideInInspector] public ItemSlot outputSlot;               // resultSlot
        [HideInInspector] public ItemSlot[] suggestionInputSlots;   // suggestion ingredient slots
        [HideInInspector] public ItemSlot suggestionOutputSlot;     // suggestion resultSlot


        [Header("REFERENCES")]
        public Player player;
        private ItemInHand itemInHand;
        private GameDataManager itemDataManager;


        #region Properties
        public int GridLength { get => NUM_OF_GRID; }
        #endregion


        private void OnEnable()
        {
            EventManager.OnGridChanged += LookupItemFromRecipe;
            EventManager.OnOutputItemReceived += ComsumeCrafingMaterial;

        }

        private void OnDisable()
        {
            EventManager.OnGridChanged -= LookupItemFromRecipe;
            EventManager.OnOutputItemReceived -= ComsumeCrafingMaterial;
        }


        private void Start()
        {
            itemDataManager = GameDataManager.Instance;

            itemInHand = player.ItemInHand;
            inputSlots = new ItemSlot[NUM_OF_GRID];
            suggestionInputSlots = new ItemSlot[NUM_OF_GRID];
            outputSlot = new ItemSlot();
            suggestionOutputSlot = new ItemSlot();

            for (int i = 0; i < NUM_OF_GRID; i++)
            {
                inputSlots[i] = new ItemSlot();
                suggestionInputSlots[i] = new ItemSlot();
            }
        }

        /// <summary>
        /// Retrieves the input item slot at the specified index within the crafting system.
        /// </summary>
        /// <param name="index">The index of the crafting input item slot to retrieve.</param>
        /// <returns>The item slot data at the specified index, or null if the index is out of range.</returns>
        public ItemSlot GetInputItemSlotAt(int index)
        {
            if (index < 0 || index >= NUM_OF_GRID) return null;
            return inputSlots[index];
        }


        /// <summary>
        /// Looks up the item to be crafted based on the current recipe and sets the crafting output slot to that item.
        /// </summary>
        private void LookupItemFromRecipe()
        {
            var currentRecipe = CreateRecipe();
            outputSlot = itemDataManager.GetItemFromRecipe(currentRecipe);
        }


        /// <summary>
        /// Consumes the items in the crafting input slots after a successful craft.
        /// </summary>
        private void ComsumeCrafingMaterial()
        {
            for (int i = 0; i < inputSlots.Length; i++)
            {
                if (inputSlots[i].HasItem())
                {
                    inputSlots[i].RemoveItem();
                }
            }
        }

        /// <summary>
        /// Adds a new item to the crafting input slot at the specified index.
        /// </summary>
        /// <param name="index">The index of the crafting input slot to add the item to.</param>
        /// <param name="item">The item to add to the crafting input slot.</param>
        public void AddItemToCraftingGrid(int index, ItemData item)
        {
            inputSlots[index].AddNewItem(item);
        }

        /// <summary>
        /// Removes the item from the crafting input slot at the specified index.
        /// </summary>
        /// <param name="index">The index of the crafting input slot to remove the item from.</param>
        public void RemoveItemFromCraftingGrid(int index)
        {
            inputSlots[index].RemoveItem();
        }


        /// <summary>
        /// Creates a new RecipeData object based on the items in the crafting input slots.
        /// </summary>
        /// <returns>A new RecipeData object representing the items in the crafting input slots.</returns>
        public RecipeData CreateRecipe()
        {
            RecipeData newRecipe = ScriptableObject.CreateInstance<RecipeData>();
            newRecipe.item00 = inputSlots[0].ItemData;
            newRecipe.item10 = inputSlots[1].ItemData;
            newRecipe.item20 = inputSlots[2].ItemData;

            newRecipe.item01 = inputSlots[3].ItemData;
            newRecipe.item11 = inputSlots[4].ItemData;
            newRecipe.item21 = inputSlots[5].ItemData;

            newRecipe.item02 = inputSlots[6].ItemData;
            newRecipe.item12 = inputSlots[7].ItemData;
            newRecipe.item22 = inputSlots[8].ItemData;

            return newRecipe;
        }


        /// <summary>
        /// Updates the crafting suggestion slots with the item recipe data.
        /// </summary>
        /// <param name="itemRecipe">The recipe data for the item being crafted.</param>
        public void UpdateCraftingSuggestionSlots(RecipeData itemRecipe)
        {
            if (itemRecipe == null)
            {
                for (int i = 0; i < suggestionInputSlots.Length; i++)
                    suggestionInputSlots[i].ClearSlot();

                suggestionOutputSlot.ClearSlot();

            }
            else
            {
                suggestionInputSlots[0] = new ItemSlot(itemRecipe.item00, 1);
                suggestionInputSlots[1] = new ItemSlot(itemRecipe.item10, 1);
                suggestionInputSlots[2] = new ItemSlot(itemRecipe.item20, 1);

                suggestionInputSlots[3] = new ItemSlot(itemRecipe.item01, 1);
                suggestionInputSlots[4] = new ItemSlot(itemRecipe.item11, 1);
                suggestionInputSlots[5] = new ItemSlot(itemRecipe.item21, 1);

                suggestionInputSlots[6] = new ItemSlot(itemRecipe.item02, 1);
                suggestionInputSlots[7] = new ItemSlot(itemRecipe.item12, 1);
                suggestionInputSlots[8] = new ItemSlot(itemRecipe.item22, 1);


                suggestionOutputSlot = itemDataManager.GetItemFromRecipe(itemRecipe);
            }

        }


        /// <summary>
        /// Returns true if there is an item in the crafting input slot at the specified index, otherwise returns false.
        /// </summary>
        /// <param name="index">The index of the crafting input slot to check.</param>
        /// <returns>True if there is an item in the crafting input slot at the specified index, otherwise false.</returns>
        /// <seealso cref="HasSlot(int)"/>
        /// <seealso cref="ItemSlot.HasItem()"/>
        public bool HasItem(int index)
        {
            if (HasSlot(index) == false) return false;

            try
            {
                inputSlots[index].HasItem();
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Checks if the crafting table has an output slot.
        /// </summary>
        /// <returns>True if the crafting table has an output slot, false otherwise.</returns>
        public bool HasOutputSlot()
        {
            return outputSlot != null;
        }


        /// <summary>
        /// Checks if the given index is within the range of valid crafting input slots.
        /// </summary>
        /// <param name="index">The index to check.</param>
        /// <returns>True if the index is valid, false otherwise.</returns>
        private bool HasSlot(int index)
        {
            return index >= 0 && index < inputSlots.Length;
        }


        /// <summary>
        /// Gets the item data at the specified index of the crafting grid.
        /// </summary>
        /// <param name="index">The index of the crafting grid.</param>
        /// <returns>The item data at the specified index of the crafting grid, or null if the index is invalid or the slot is empty.</returns>
        public ItemData GetItem(int index)
        {
            if (HasItem(index) == false) return null;
            return inputSlots[index].ItemData;
        }


        /// <summary>
        /// Adds an item to the specified index of the crafting grid.
        /// </summary>
        /// <param name="index">The index of the crafting grid.</param>
        /// <returns>True if the item was successfully added, false otherwise.</returns>
        public bool AddItem(int index)
        {
            bool isSlotNotFull = inputSlots[index].AddItem();
            return isSlotNotFull;
        }



        /// <summary>
        /// Attempts to stack the item in hand into the crafting grid.
        /// </summary>
        /// <remarks>
        /// This method searches for empty slots in the crafting grid that contain the same item as the one in hand, 
        /// and adds the items in hand to the slots with the lowest item quantity.
        /// </remarks>
        /// /// <seealso cref="ItemSlot"/>
        /// <seealso cref="UIItemInHand"/>
        /// <seealso cref="UICraftingTable"/>
        public void StackItem()
        {
            if (itemInHand.GetItemData() == null) return;
            Dictionary<int, int> dict = new Dictionary<int, int>();
            Dictionary<int, int> sortedDict = new Dictionary<int, int>();

            for (int i = 0; i < inputSlots.Length; i++)
            {
                if (inputSlots[i].ItemData == itemInHand.GetItemData())
                {
                    dict.Add(i, inputSlots[i].ItemQuantity);
                }
            }

            // Use OrderBy to sort the dictionary by value
            sortedDict = dict.OrderBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var e in sortedDict)
            {
                itemInHand.GetSlot().AddItemsFromAnotherSlot(inputSlots[e.Key]);
                UIItemInHand.Instance.UpdateItemInHandUI();
                UICraftingTable.Instance.UpdateCraftingTableDisplayUIAt(e.Key);
            }
        }
    }

}