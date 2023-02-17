using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        if(player.ItemInHand.HasItemData() && uiSlotDisplay != null)
        {
            mainCameraPosition = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            uiSlotDisplay.GetComponent<RectTransform>().transform.position = mainCameraPosition;                    
        }
        else
        {
            if(uiSlotDisplay!= null)
            {
                DestroyImmediate(uiSlotDisplay);
            }
        }
    }


    public void UpdateItemInHandUI(Transform parent = null)
    {
        if (itemInHand.HasItemData() == false)
        {
            itemInHand.ClearSlot();
            return;
        }

        if(uiSlotDisplay != null || UISlotImage != null)
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

    private void SetItemQuantityText()
    {
        int itemQuantity = itemInHand.GetSlot().ItemQuantity;
        if (itemQuantity > 1)
            uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = itemInHand.GetSlot().ItemQuantity.ToString();
        else
            uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = "";
    }

    private void ResetUIItemInHandColor()
    {
        if(uiSlotDisplay != null || UISlotImage != null)
            UISlotImage.color = new Color(1, 1, 1, 1);
    }
}
