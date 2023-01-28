using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemInHand : Singleton<UIItemInHand>
{
    [Header("References")]
    public PlayerController player;
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
        if(player.ItemInHand.HasItem() && uiSlotDisplay != null)
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
        if (player.ItemInHand.HasItem() == false)
        {
            player.ItemInHand.ClearSlot();
            return;
        }

        if(uiSlotDisplay != null)
        {
            uiSlotDisplay.GetComponent<UIItemSlot>().slotImage.sprite = player.ItemInHand.itemSlot.itemObject.icon;
            SetItemQuantityText();
        }
        else
        {
            uiSlotDisplay = Instantiate(uiSlotPrefab, this.transform.parent.transform);
            uiSlotDisplay.GetComponent<UIItemSlot>().slotImage.sprite = player.ItemInHand.itemSlot.itemObject.icon;
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
        int itemQuantity = player.ItemInHand.itemSlot.itemQuantity;
        if (itemQuantity > 1)
            uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = player.ItemInHand.itemSlot.itemQuantity.ToString();
        else
            uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = "";
    }
}
