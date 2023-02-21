using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CraftingTable : Singleton<CraftingTable>
{
    private const int NUM_OF_GRID = 9;


    [Header("Container")]
    [HideInInspector] public ItemSlot[] crafintInputSlots;  
    [HideInInspector] public ItemSlot craftingOutputSlot;
    [HideInInspector] public ItemSlot[] craftingSuggestionInputSlots;
    [HideInInspector] public ItemSlot craftingSuggestionOutputSlot;
    


    [Header("References")]
    public Player player;
    private ItemInHand itemInHand;

    [Header("References")]
    private ItemRecipeManager itemRecipeManager;


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
        itemInHand = player.ItemInHand;
        crafintInputSlots = new ItemSlot[NUM_OF_GRID];
        craftingSuggestionInputSlots = new ItemSlot[NUM_OF_GRID];
        craftingOutputSlot = new ItemSlot();
        itemRecipeManager = ItemRecipeManager.Instance;
    }

    public ItemSlot GetItemInputSlotAt(int index)
    {
        if (index < 0 || index >= NUM_OF_GRID) return null;
        return crafintInputSlots[index];
    }


    private void LookupItemFromRecipe()
    {
        var currentRecipe = CreateRecipe();
        craftingOutputSlot = itemRecipeManager.GetItemFromRecipe(currentRecipe);
    }



    private void ComsumeCrafingMaterial()
    {
        for (int i = 0; i < crafintInputSlots.Length; i++)
        {
            if (crafintInputSlots[i].HasItem())
            {
                crafintInputSlots[i].RemoveItem();
            }
        }
    }

    public void AddNewItemAt(int index, ItemData item)
    {
        crafintInputSlots[index].AddNewItem(item);
        //OnInventoryUpdate?.Invoke();
    }

    public void RemoveItemFromCraftingGridAtIndex(int index)
    {
        crafintInputSlots[index].RemoveItem();
        //OnInventoryUpdate?.Invoke();
    }

    public RecipeData CreateRecipe()
    {
        RecipeData newRecipe = ScriptableObject.CreateInstance<RecipeData>();
        newRecipe.item00 = crafintInputSlots[0].ItemData;
        newRecipe.item10 = crafintInputSlots[1].ItemData;
        newRecipe.item20 = crafintInputSlots[2].ItemData;

        newRecipe.item01 = crafintInputSlots[3].ItemData;
        newRecipe.item11 = crafintInputSlots[4].ItemData;
        newRecipe.item21 = crafintInputSlots[5].ItemData;

        newRecipe.item02 = crafintInputSlots[6].ItemData;
        newRecipe.item12 = crafintInputSlots[7].ItemData;
        newRecipe.item22 = crafintInputSlots[8].ItemData;

        return newRecipe;
    }


    public void FillCraftingSuggestionData(RecipeData itemRecipe)
    {
        if (itemRecipe == null)
        {
            for (int i = 0; i < craftingSuggestionInputSlots.Length; i++)
                craftingSuggestionInputSlots[i] = new ItemSlot();

            craftingSuggestionOutputSlot = new ItemSlot();

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


            craftingSuggestionOutputSlot = ItemRecipeManager.Instance.GetItemFromRecipe(itemRecipe);
        }
        
    }



    public bool HasItem(int index)
    {
        if (HasSlot(index) == false) return false;

        try
        {
            crafintInputSlots[index].HasItem();
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
        return index >= 0 && index < crafintInputSlots.Length;
    }

    public ItemData GetItem(int index)
    {
        if (HasItem(index) == false) return null;
        return crafintInputSlots[index].ItemData;
    }

    public bool AddItem(int index)
    {
        bool isSlotNotFull = crafintInputSlots[index].AddItem();
        return isSlotNotFull;
    }

    public void StackItem()
    {
        if (itemInHand.GetItemData() == null) return;
        Dictionary<int, int> dict = new Dictionary<int, int>();
        Dictionary<int, int> sortedDict = new Dictionary<int, int>();

        for (int i = 0; i < crafintInputSlots.Length; i++)
        {
            if (crafintInputSlots[i].ItemData == itemInHand.GetItemData())
            {
                dict.Add(i, crafintInputSlots[i].ItemQuantity);
            }
        }

        // Use OrderBy to sort the dictionary by value
        sortedDict = dict.OrderBy(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (var e in sortedDict)
        {
            itemInHand.GetSlot().AddItemsFromAnotherSlot(crafintInputSlots[e.Key]);
            UIItemInHand.Instance.UpdateItemInHandUI();
            UICraftingTable.Instance.UpdateCraftingTableDisplayUIAt(e.Key);
        }
    }
}


