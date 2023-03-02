using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

namespace UltimateItemSystem
{
    /// <summary>
    /// Class for handling UI Item Slots, implements IPointerEnterHandler and IPointerExitHandler
    /// </summary>
    public class UIItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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


        /// <summary>
        /// Called when the pointer enters the UI element
        /// </summary>
        /// <param name="eventData">The pointer event data</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(currentItemSlot.HasItem())
            {
                ItemDescriptionManager.Instance.SetItemData(currentItemSlot.ItemData);
                StartCoroutine(Show());
            }         
        }

        /// <summary>
        /// Coroutine that shows the item description after a delay
        /// </summary>
        /// <returns>IEnumerator</returns>
        IEnumerator Show()
        {
            yield return new WaitForSeconds(0.3f);
            ItemDescriptionManager.Instance.Show();
            UIManager.Instance.ItemDescCanvas.SetActive(true);               
        }


        /// <summary>
        /// Called when the pointer exit the UI element
        /// </summary>
        /// <param name="eventData">The pointer event data</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            if (currentItemSlot.HasItem())
            {
                ItemDescriptionManager.Instance.SetItemData(null);
                ItemDescriptionManager.Instance.Hide();
                UIManager.Instance.ItemDescCanvas.SetActive(false);
            }
            
        }
    }
}