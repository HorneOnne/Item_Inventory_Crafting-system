using UnityEngine;
using UnityEngine.UI;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Represents a UI equipment slot used to display equipment icons in the UI.
    /// </summary>
    public class UIEquipSlot : MonoBehaviour
    {
        /// <summary>
        /// The image component used to display the equipment icon.
        /// </summary>
        public Image equipmentImage;

        /// <summary>
        /// The type of equipment that this slot is intended for.
        /// </summary>
        public Sprite defaultEquipmentIcon;

        public ItemType equipmentType;


        private void Start()
        {
            SetDefault();
        }


        /// <summary>
        /// Sets the equipment icon for this slot.
        /// </summary>
        /// <param name="equipmentIcon">The sprite to use as the equipment icon.</param>
        public void Set(Sprite equipmentIcon)
        {
            if (equipmentIcon == null)
                return;

            Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            equipmentImage.color = color;

            this.equipmentImage.sprite = equipmentIcon;
        }


        /// <summary>
        /// Sets the default equipment icon for this slot.
        /// </summary>
        public void SetDefault()
        {
            Color color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            equipmentImage.color = color;

            this.equipmentImage.sprite = defaultEquipmentIcon;
        }
    }
}
