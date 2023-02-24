using UnityEngine;
using UnityEngine.EventSystems;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Class for handling dragging of UI panels.
    /// </summary>
    public class UIDragPanel : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        private ItemInHand itemInHand;
        private RectTransform rt;
        private Canvas canvas;

        /// <summary>
        /// Initializes RectTransform and Canvas components and finds ItemInHand instance.
        /// </summary>
        void Start()
        {
            rt = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();

            itemInHand = GameObject.FindObjectOfType<ItemInHand>();
        }


        /// <summary>
        /// Handles dragging of the UI panel.
        /// </summary>
        /// <param name="eventData">The PointerEventData associated with the drag event.</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }

        }

        /// <summary>
        /// Handles pointer down event on the UI panel.
        /// </summary>
        /// <param name="eventData">The PointerEventData associated with the pointer down event.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right
                && itemInHand.HasItemData() == false)
            {
                rt.transform.SetAsLastSibling();
            }

        }
    }
}
