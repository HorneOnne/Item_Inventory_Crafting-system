using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UltimateItemSystem
{
    /// <summary>
    /// Class for handling UI Item Slots, implements IPointerEnterHandler and IPointerExitHandler
    /// </summary>
    public class UIItemSlot : MonoBehaviour
    {
        [Header("Internal References")]
        public Image mainImage;
        public Image defaultImage;
        public TextMeshProUGUI amountItemInSlotText;
        [SerializeField] private int slotIndex;

        private ItemSlot currentItemSlot = new ItemSlot();
        public int SlotIndex { get { return slotIndex; } protected set { slotIndex = value; } }


        /// <summary>
        /// Sets the index of the slot
        /// </summary>
        /// <param name="slotIndex">The index of the slot</param>
        public void SetIndex(int slotIndex = -1)
        {
            this.slotIndex = slotIndex;
        }


        /// <summary>
        /// Sets the data for the item slot
        /// </summary>
        /// <param name="itemSlot">The item slot to set the data for</param>
        public void SetData(ItemSlot itemSlot)
        {
            if (itemSlot == null || itemSlot.HasItem() == false)
            {
                defaultImage.enabled = true;
                this.mainImage.sprite = null;
                amountItemInSlotText.text = "";

                currentItemSlot.ClearSlot();
            }
            else
            {
                defaultImage.enabled = false;
                this.mainImage.sprite = itemSlot.GetItemIcon();
                if (itemSlot.ItemQuantity > 1)
                    amountItemInSlotText.text = $"{itemSlot.ItemQuantity}";
                else
                    amountItemInSlotText.text = "";

                currentItemSlot = new ItemSlot(itemSlot);
            }
        }


        /// <summary>
        /// Sets the data and opacity for the item slot
        /// </summary>
        /// <param name="itemSlot">The item slot to set the data for</param>
        /// <param name="opacity">The opacity of the item slot</param>
        public void SetData(ItemSlot itemSlot, float opacity)
        {
            Color mainImageColor = mainImage.color;
            mainImageColor.a = opacity;

            Color defaultTextColor = amountItemInSlotText.color;
            defaultTextColor.a = opacity;

            if (itemSlot == null || itemSlot.HasItem() == false)
            {
                defaultImage.enabled = true;
                this.mainImage.sprite = null;
                amountItemInSlotText.text = "";

                currentItemSlot.ClearSlot();
            }
            else
            {
                defaultImage.enabled = false;
                this.mainImage.sprite = itemSlot.GetItemIcon();
                if (itemSlot.ItemQuantity > 1)
                    amountItemInSlotText.text = $"{itemSlot.ItemQuantity}";
                else
                    amountItemInSlotText.text = "";

                currentItemSlot = new ItemSlot(itemSlot);
            }

            mainImage.color = mainImageColor;
            amountItemInSlotText.color = defaultTextColor;
        }
    }
}