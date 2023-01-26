using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemRecipeManager
{
    public List<RecipeData> recipeScriptableObjectList;
    public Dictionary<RecipeData, ItemSlot> recipeScriptableObjectDict;


    public void InitialRecipe()
    {
        recipeScriptableObjectDict = new Dictionary<RecipeData, ItemSlot>();
        int index = 0;
        foreach (var recipe in recipeScriptableObjectList)
        {
            if(recipeScriptableObjectDict.ContainsKey(recipe))
            {
                Debug.LogError($"[ItemRecipeManager]: \tRecipe at {index} already exist.");
            }
            else
            {
                //recipeScriptableObjectDict.Add(recipe, recipe.outputItem);
                recipeScriptableObjectDict.Add(recipe, new ItemSlot(recipe.outputItem, recipe.quantityItemOutput));
            }

            index++;
            
        }

        recipeScriptableObjectList.Clear();
    }

    public ItemSlot GetRecipeOutput(RecipeData recipe)
    {
        if(recipeScriptableObjectDict.ContainsKey(recipe))
        {
            ItemSlot returnedSlot = new ItemSlot(recipeScriptableObjectDict[recipe]);
            return returnedSlot;
        }
        return null;
    }
}
