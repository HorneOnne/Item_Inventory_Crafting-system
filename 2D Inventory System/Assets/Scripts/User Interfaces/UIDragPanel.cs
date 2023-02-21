using UnityEngine;
using UnityEngine.EventSystems;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// This class provide drag ability to player.
    /// </summary>
    public class UIDragPanel : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        private ItemInHand itemInHand;
        private RectTransform rt;
        private Canvas canvas;
        void Start()
        {
            rt = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();

            itemInHand = GameObject.FindObjectOfType<ItemInHand>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
            }

        }

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
