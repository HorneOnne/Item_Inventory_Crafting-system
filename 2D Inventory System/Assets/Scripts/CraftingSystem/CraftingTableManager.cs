using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingTableManager : Singleton<CraftingTableManager>
{
    [Header("Container")]
    [SerializeField] private ItemRecipeManager craftingSystem;
    public ItemSlot[] craftingGridData = new ItemSlot[9];
    public ItemSlot outputItemSlot = new ItemSlot();


    [Header("References")]
    public PlayerController player;


    private void OnEnable()
    {
        UICraftingTableManager.OnGridChanged += LookupItemFromRecipe;

        //UI_DisplayCraftingTable.OnGetOutputItem += RemoveOutputItemSlot;
        //UI_DisplayCraftingTable.OnGetOutputItem += GetOutputItemSlot;
        
        UICraftingTableManager.OnGetOutputItem += ComsumeCrafingMaterial;
    }

    private void OnDisable()
    {
        UICraftingTableManager.OnGridChanged -= LookupItemFromRecipe;

        //UI_DisplayCraftingTable.OnGetOutputItem -= RemoveOutputItemSlot;
        //UI_DisplayCraftingTable.OnGetOutputItem -= GetOutputItemSlot;
        
        UICraftingTableManager.OnGetOutputItem -= ComsumeCrafingMaterial;
    }




    private void Start()
    {
        craftingSystem.InitialRecipe();
    }

    private void LookupItemFromRecipe()
    {
        var currentRecipe = CreateRecipe();
        outputItemSlot = craftingSystem.GetRecipeOutput(currentRecipe);
    }

    private void RemoveOutputItemSlot()
    {
        //outputItemSlot.ClearSlot();      
    }

    private void GetOutputItemSlot()
    {
        //Debug.Log("Get output Itemslot");
        //itemInHand.itemSlot.AddItemsFromAnotherSlot(outputItemSlot);
    }


    private void ComsumeCrafingMaterial()
    {
        for(int i = 0; i < craftingGridData.Length; i++)
        {
            if (craftingGridData[i].HasItem())
            {
                craftingGridData[i].RemoveItem();
            }    
        }
    }

    public void AddItemIntoCraftingGridAtIndex(int index, ItemData item)
    {
        craftingGridData[index].AddNewItem(item);
        //OnInventoryUpdate?.Invoke();
    }

    public void RemoveItemFromCraftingGridAtIndex(int index)
    {
        craftingGridData[index].RemoveItem();
        //OnInventoryUpdate?.Invoke();
    }

    public RecipeData CreateRecipe()
    {
        RecipeData newRecipe = ScriptableObject.CreateInstance<RecipeData>();
        newRecipe.item00 = craftingGridData[0].itemObject;
        newRecipe.item10 = craftingGridData[1].itemObject;
        newRecipe.item20 = craftingGridData[2].itemObject;

        newRecipe.item01 = craftingGridData[3].itemObject;
        newRecipe.item11 = craftingGridData[4].itemObject;
        newRecipe.item21 = craftingGridData[5].itemObject;

        newRecipe.item02 = craftingGridData[6].itemObject;
        newRecipe.item12 = craftingGridData[7].itemObject;
        newRecipe.item22 = craftingGridData[8].itemObject;

        return newRecipe;
    }



    public bool HasItem(int index)
    {
        if(HasSlot(index) == false) return false;
        
        try
        {
            craftingGridData[index].HasItem();
        }
        catch
        {
            return false;
        }

        return true;
    }


    public bool HasOutputSlot()
    {
        return outputItemSlot != null;
    }

    public bool HasSlot(int index)
    {
        return index >= 0 && index < craftingGridData.Length;
    }

    public ItemData GetItem(int index)
    {
        if (HasItem(index) == false) return null;
        return craftingGridData[index].itemObject;
    }

    public bool AddItem(int index)
    {
        bool isSlotNotFull = craftingGridData[index].AddItem();
        return isSlotNotFull;
    }
}


