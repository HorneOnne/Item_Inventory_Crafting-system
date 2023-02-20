using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingTableManager : Singleton<CraftingTableManager>
{
    private const int NUM_OF_GRID = 9;


    [Header("Container")]
    [SerializeField] private ItemRecipeManager craftingSystem;
    [HideInInspector] public ItemSlot[] inputSlots;
    [HideInInspector] public ItemSlot outputSlot;


    [Header("References")]
    public Player player;
    private ItemInHand itemInHand;


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
        inputSlots = new ItemSlot[NUM_OF_GRID];
        outputSlot = new ItemSlot();

        craftingSystem.InitialRecipe();
    }

    public ItemSlot GetItemIntputSlotAt(int index)
    {
        if(index < 0 || index >= NUM_OF_GRID) return null;
        return inputSlots[index];
    }


    private void LookupItemFromRecipe()
    {
        var currentRecipe = CreateRecipe();
        outputSlot = craftingSystem.GetRecipeOutput(currentRecipe);
    }



    private void ComsumeCrafingMaterial()
    {
        for(int i = 0; i < inputSlots.Length; i++)
        {
            if (inputSlots[i].HasItem())
            {
                inputSlots[i].RemoveItem();
            }    
        }
    }

    public void AddNewItemAt(int index, ItemData item)
    {
        inputSlots[index].AddNewItem(item);
        //OnInventoryUpdate?.Invoke();
    }

    public void RemoveItemFromCraftingGridAtIndex(int index)
    {
        inputSlots[index].RemoveItem();
        //OnInventoryUpdate?.Invoke();
    }

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



    public bool HasItem(int index)
    {
        if(HasSlot(index) == false) return false;
        
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


    public bool HasOutputSlot()
    {
        return outputSlot != null;
    }

    public bool HasSlot(int index)
    {
        return index >= 0 && index < inputSlots.Length;
    }

    public ItemData GetItem(int index)
    {
        if (HasItem(index) == false) return null;
        return inputSlots[index].ItemData;
    }

    public bool AddItem(int index)
    {
        bool isSlotNotFull = inputSlots[index].AddItem();
        return isSlotNotFull;
    }

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
            UICraftingTableManager.Instance.UpdateCraftingTableDisplayUIAt(e.Key);
        }
    }
}


