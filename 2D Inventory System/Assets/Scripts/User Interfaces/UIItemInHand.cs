using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemInHand : Singleton<UIItemInHand>
{
    [Header("References")]
    public Player player;
    [SerializeField] GameObject uiSlotPrefab;
    private GameObject uiSlotDisplay;

    // Cached
    Camera mainCamera;
    Vector2 mainCameraPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
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


    public void DisplayItemInHand(Transform parent = null)
    {
        if (player.ItemInHand.HasItemData() == false)
        {
            player.ItemInHand.ClearSlot();
            return;
        }

        if(uiSlotDisplay != null)
        {
            uiSlotDisplay.GetComponent<UIItemSlot>().slotImage.sprite = player.ItemInHand.GetItem().icon;
            SetItemQuantityText();
        }
        else
        {
            uiSlotDisplay = Instantiate(uiSlotPrefab, this.transform.parent.transform);
            uiSlotDisplay.GetComponent<UIItemSlot>().slotImage.sprite = player.ItemInHand.GetItem().icon;
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
        int itemQuantity = player.ItemInHand.GetSlot().itemQuantity;
        if (itemQuantity > 1)
            uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = player.ItemInHand.GetSlot().itemQuantity.ToString();
        else
            uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = "";
    }
}
