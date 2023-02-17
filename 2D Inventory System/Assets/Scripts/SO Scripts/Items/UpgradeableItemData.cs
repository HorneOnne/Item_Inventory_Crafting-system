using System.Collections.Generic;
using UnityEngine;

public class UpgradeableItemData : ItemData
{
    public int currentLevel;
    public int maxLevel;

    [Header("Upgrade Recipe")]
    public ItemUpgradeRecipe upgradeRecipe;
}


[System.Serializable]
public struct ItemUpgradeRecipe
{
    public List<RecipeSlot> materials;
    public RecipeSlot outputItem;


    [System.Serializable]
    public struct RecipeSlot
    {
        public ItemData itemData;
        public int quantity;

        public RecipeSlot(ItemData itemData, int quantity)
        {
            this.itemData = itemData;
            this.quantity = quantity;
        }
        
    }
}
