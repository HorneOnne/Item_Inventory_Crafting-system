using System;
using UnityEngine;

namespace DIVH_InventorySystem
{
    [CreateAssetMenu(fileName = "New Recipe Object", menuName = "ScriptableObject/Recipe", order = 51)]
    public class RecipeData : ScriptableObject
    {
        public ItemData outputItem;
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