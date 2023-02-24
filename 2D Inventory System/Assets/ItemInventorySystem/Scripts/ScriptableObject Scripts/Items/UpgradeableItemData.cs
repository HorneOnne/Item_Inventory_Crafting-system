using System.Collections.Generic;
using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Represents an upgradeable item that can be leveled up and has an upgrade recipe.
    /// </summary>
    public class UpgradeableItemData : ItemData
    {
        /// <summary>
        /// The current level of the item.
        /// </summary>
        public int currentLevel;

        /// <summary>
        /// The maximum level the item can reach.
        /// </summary>
        public int maxLevel;

        /// <summary>
        /// The recipe required to upgrade the item.
        /// </summary>
        [Header("UPGRADE RECIPE")]
        public ItemUpgradeRecipe upgradeRecipe;
    }


    /// <summary>
    /// Represents a recipe for upgrading an item.
    /// </summary>
    [System.Serializable]
    public struct ItemUpgradeRecipe
    {
        /// <summary>
        /// The materials required for the upgrade recipe.
        /// </summary>
        public List<RecipeSlot> materials;

        /// <summary>
        /// The output item of the upgrade recipe.
        /// </summary>
        public RecipeSlot outputItem;


        [System.Serializable]
        public struct RecipeSlot
        {
            /// <summary>
            /// The data for the item required in the upgrade recipe.
            /// </summary>
            public ItemData itemData;

            /// <summary>
            /// The quantity of the item required in the upgrade recipe.
            /// </summary>
            public int quantity;

            public RecipeSlot(ItemData itemData, int quantity)
            {
                this.itemData = itemData;
                this.quantity = quantity;
            }

        }
    }
}