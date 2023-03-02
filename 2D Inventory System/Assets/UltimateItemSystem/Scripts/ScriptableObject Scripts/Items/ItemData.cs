using System;
using System.Text;
using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// Base class for all item data.
    /// </summary>
    public abstract class ItemData : ScriptableObject
    {
        /// <summary>
        /// The name of the item, with spaces added before each uppercase character.
        /// </summary>
        public string ItemName { get => AddSpacesBeforeUpperCase(this.name); }

        [Header("ITEM DATA")]
        /// <summary>
        /// The icon used to represent the item.
        /// </summary>
        public Sprite icon;


        /// <summary>
        /// The category type of the item.
        /// </summary>
        public ItemCategory itemCategory;


        /// <summary>
        /// The type of the item.
        /// </summary>
        public ItemType itemType;

        /// <summary>
        /// The maximum quantity of the item that can be stacked in a single inventory slot.
        /// </summary>
        public int max_quantity;

        /// <summary>
        /// The description of the item.
        /// </summary>
        [Multiline(7)]
        public string description;

       
        [Tooltip("The usage velocity property indicates how quickly this item can be used within a specific timeframe. The value represents the number of times the item can be used in one second.")]
        public float usageVelocity = 1.0f;


        /// <summary>
        /// Determines whether this instance and another specified <see cref="ItemData"/> object have the same value.
        /// </summary>
        /// <param name="other">The <see cref="ItemData"/> object to compare to this instance.</param>
        /// <returns><c>true</c> if the value of the <paramref name="other"/> parameter is the same as the value of this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object other)
        {
            if (this.ItemName != ((ItemData)other).ItemName) return false;
            if (this.icon != ((ItemData)other).icon) return false;
            if (this.itemCategory != ((ItemData)other).itemCategory) return false;
            if (this.itemType != ((ItemData)other).itemType) return false;
            if (this.max_quantity != ((ItemData)other).max_quantity) return false;
            if (this.description != ((ItemData)other).description) return false;
            if (this.usageVelocity != ((ItemData)other).usageVelocity) return false;

            return true;
        }


        /// <summary>
        /// Determines whether two specified <see cref="ItemData"/> objects have the same value.
        /// </summary>
        /// <param name="itemA">The first <see cref="ItemData"/> object to compare.</param>
        /// <param name="itemB">The second <see cref="ItemData"/> object to compare.</param>
        /// <returns><c>true</c> if the value of the <paramref name="itemA"/> parameter is the same as the value of the <paramref name="itemB"/> parameter; otherwise, <c>false</c>.</returns>
        public static bool IsSameItem(ItemData itemA, ItemData itemB)
        {
            if (itemA == null || itemB == null) return false;
            return itemA.Equals(itemB);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ItemName, icon, itemCategory, itemType, max_quantity, description, usageVelocity);
        }


        /// <summary>
        /// Adds spaces before each uppercase character in the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The input string with spaces added before each uppercase character
        private string AddSpacesBeforeUpperCase(string input)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]) && i != 0)
                {
                    output.Append(" ");
                }
                output.Append(input[i]);
            }
            return output.ToString();
        }
    }
}