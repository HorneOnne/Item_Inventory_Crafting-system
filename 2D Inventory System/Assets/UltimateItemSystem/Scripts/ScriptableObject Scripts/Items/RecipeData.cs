using System;
using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// A scriptable object that represents a crafting recipe in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "New Recipe Object", menuName = "UltimateItemSystem/Recipe", order = 51)]
    public class RecipeData : ScriptableObject
    {
        /// <summary>
        /// The output item that this recipe produces.
        /// </summary>
        public ItemData outputItem;

        /// <summary>
        /// The quantity of the output item that this recipe produces.
        /// </summary>
        public int quantityItemOutput;

        public ItemData item00;
        public ItemData item10;
        public ItemData item20;

        public ItemData item01;
        public ItemData item11;
        public ItemData item21;

        public ItemData item02;
        public ItemData item12;
        public ItemData item22;



        /// <summary>
        /// Gets the item data in the specified position of the crafting grid for this recipe.
        /// </summary>
        /// <param name="x">The x-coordinate of the item in the grid (0-based).</param>
        /// <param name="y">The y-coordinate of the item in the grid (0-based).</param>
        /// <returns>The item data in the specified position, or null if the position is out of bounds.</returns>
        public virtual ItemData GetItem(int x, int y)
        {
            if (x == 0 && y == 0) return item00;
            if (x == 1 && y == 0) return item10;
            if (x == 2 && y == 0) return item20;

            if (x == 0 && y == 1) return item01;
            if (x == 1 && y == 1) return item11;
            if (x == 2 && y == 1) return item21;

            if (x == 0 && y == 2) return item02;
            if (x == 1 && y == 2) return item12;
            if (x == 2 && y == 2) return item22;

            return null;
        }


        /// <summary>
        /// Determines whether the specified object is equal to this recipe data object.
        /// </summary>
        /// <param name="other">The object to compare to this recipe data object.</param>
        /// <returns>true if the objects are equal, otherwise false.</returns>
        public override bool Equals(object other)
        {
            if (item00 != ((RecipeData)other).item00) return false;
            if (item10 != ((RecipeData)other).item10) return false;
            if (item20 != ((RecipeData)other).item20) return false;

            if (item01 != ((RecipeData)other).item01) return false;
            if (item11 != ((RecipeData)other).item11) return false;
            if (item21 != ((RecipeData)other).item21) return false;

            if (item02 != ((RecipeData)other).item02) return false;
            if (item12 != ((RecipeData)other).item12) return false;
            if (item22 != ((RecipeData)other).item22) return false;

            return true;


        }
        public override int GetHashCode()
        {
            return HashCode.Combine(HashCode.Combine(item00, item10, item20),
                                    HashCode.Combine(item01, item11, item21),
                                    HashCode.Combine(item02, item12, item22));
        }
    }
}