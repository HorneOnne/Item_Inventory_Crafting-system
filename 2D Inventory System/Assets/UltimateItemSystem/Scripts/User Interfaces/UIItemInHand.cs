using UnityEngine;
using UnityEngine.UI;

namespace UltimateItemSystem
{
    /// <summary>
    /// Manages the UI display for the player's currently held item. Inherits from Singleton to ensure only one instance exists.
    /// </summary>
    public class UIItemInHand : Singleton<UIItemInHand>
    {
        [Header("References")]
        public Player player;
        private ItemInHand itemInHand;
        [SerializeField] GameObject uiSlotPrefab;
        [HideInInspector] public GameObject uiSlotDisplay;

        public Image UISlotImage { get; private set; }


        // Cached
        Camera mainCamera;
        Vector2 mainCameraPosition;

        private void OnEnable()
        {
            EventManager.OnItemInHandChanged += ResetUIItemInHandColor;
        }

        private void OnDisable()
        {
            EventManager.OnItemInHandChanged -= ResetUIItemInHandColor;
        }


        private void Start()
        {
            mainCamera = Camera.main;
            itemInHand = player.ItemInHand;
        }


        private void Update()
        {
            if (player.ItemInHand.HasItemData() && uiSlotDisplay != null)
            {
                mainCameraPosition = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
                uiSlotDisplay.GetComponent<RectTransform>().transform.position = mainCameraPosition;
            }
            else
            {
                if (uiSlotDisplay != null)
                {
                    DestroyImmediate(uiSlotDisplay);
                    Cursor.visible = true;
                }
            }
        }


        /// <summary>
        /// Updates the UI display for the item in the player's hand.
        /// </summary>
        /// <param name="parent">The parent transform for the UI slot display.</param>
        public void UpdateItemInHandUI(Transform parent = null)
        {
            if (itemInHand.HasItemData() == false)
            {
                itemInHand.ClearSlot();
                Cursor.visible = true;
                return;
            }

            Cursor.visible = false;
            if (uiSlotDisplay != null || UISlotImage != null)
            {
                UISlotImage.sprite = itemInHand.GetItemData().icon;               
                SetItemQuantityText();
            }
            else
            {
                uiSlotDisplay = Instantiate(uiSlotPrefab, this.transform.parent.transform);
                UISlotImage = uiSlotDisplay.GetComponent<UIItemSlot>().mainImage;
                UISlotImage.sprite = itemInHand.GetItemData().icon;
                SetItemQuantityText();

                if (parent != null)
                {
                    uiSlotDisplay.transform.SetParent(parent.transform);
                }

                uiSlotDisplay.name = $"InHandItem";
                uiSlotDisplay.GetComponent<RectTransform>().SetAsLastSibling();
            }
            
        }


        /// <summary>
        /// Sets the text of the UI element that displays the quantity of the item in the hand slot.
        /// </summary>
        private void SetItemQuantityText()
        {
            int itemQuantity = itemInHand.GetSlot().ItemQuantity;
            if (itemQuantity > 1)
                uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = itemInHand.GetSlot().ItemQuantity.ToString();
            else
                uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = "";
        }


        /// <summary>
        /// Resets the color of the image of the UI element that displays the item in the hand slot.
        /// </summary>
        private void ResetUIItemInHandColor()
        {
            if (uiSlotDisplay != null || UISlotImage != null)
                UISlotImage.color = new Color(1, 1, 1, 1);
        }
    }
}