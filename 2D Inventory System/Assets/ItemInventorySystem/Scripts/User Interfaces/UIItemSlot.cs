using UnityEngine.UI;
using UnityEngine;
using TMPro;


namespace DIVH_InventorySystem
{
    public class UIItemSlot : MonoBehaviour
    {
        [Header("Internal References")]
        public Image mainImage;
        public Image defaultImage;
        public TextMeshProUGUI amountItemInSlotText;
        [SerializeField] private int slotIndex;
        public int SlotIndex { get { return slotIndex; } protected set { slotIndex = value; } }

        public void SetIndex(int slotIndex = -1)
        {
            this.slotIndex = slotIndex;
        }

        public void SetData(ItemSlot itemSlot)
        {
            if (itemSlot == null || itemSlot.HasItem() == false)
            {
                defaultImage.enabled = true;
                this.mainImage.sprite = null;
                amountItemInSlotText.text = "";
            }
            else
            {
                defaultImage.enabled = false;
                this.mainImage.sprite = itemSlot.GetItemIcon();
                if (itemSlot.ItemQuantity > 1)
                    amountItemInSlotText.text = $"{itemSlot.ItemQuantity}";
                else
                    amountItemInSlotText.text = "";
            }
        }

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
            }
            else
            {
                defaultImage.enabled = false;
                this.mainImage.sprite = itemSlot.GetItemIcon();
                if (itemSlot.ItemQuantity > 1)
                    amountItemInSlotText.text = $"{itemSlot.ItemQuantity}";
                else
                    amountItemInSlotText.text = "";
            }

            mainImage.color = mainImageColor;
            amountItemInSlotText.color = defaultTextColor;
        }
    }
}