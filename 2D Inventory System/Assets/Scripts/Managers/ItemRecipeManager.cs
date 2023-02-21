using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemRecipeManager : Singleton<ItemRecipeManager>
{
    public List<RecipeData> recipeScriptableObjectList;
    public Dictionary<RecipeData, ItemSlot> recipeToItemDict;
    public Dictionary<ItemData ,RecipeData> itemToRecipeDict;

    private void Start()
    {
        InitialRecipe();
    }


    public void InitialRecipe()
    {
        recipeToItemDict = new Dictionary<RecipeData, ItemSlot>();
        itemToRecipeDict = new Dictionary<ItemData, RecipeData>();
        int index = 0;
        foreach (var recipe in recipeScriptableObjectList)
        {
            if(recipeToItemDict.ContainsKey(recipe))
            {
                Debug.LogError($"[ItemRecipeManager]: \tRecipe at {index} already exist.");
            }
            else
            {
                //recipeScriptableObjectDict.Add(recipe, recipe.outputItem);
                recipeToItemDict.Add(recipe, new ItemSlot(recipe.outputItem, recipe.quantityItemOutput));
            }

            index++;
            
        }

        recipeScriptableObjectList.Clear();

        foreach(var recipe in recipeToItemDict)
        {
            if(itemToRecipeDict.ContainsKey(recipe.Value.ItemData) == false)
            {
                itemToRecipeDict.Add(recipe.Value.ItemData, recipe.Key);
            }
        }
    }

    public ItemSlot GetItemFromRecipe(RecipeData recipe)
    {
        if(recipeToItemDict.ContainsKey(recipe))
        {
            ItemSlot returnedSlot = new ItemSlot(recipeToItemDict[recipe]);
            return returnedSlot;
        }
        return null;
    }


    public RecipeData GetRecipeFromItem(ItemData itemData)
    {
        if (itemToRecipeDict.ContainsKey(itemData))
        {
            return itemToRecipeDict[itemData];
        }
        return null;
    }
}
