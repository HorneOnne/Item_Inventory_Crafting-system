using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemInHand : MonoBehaviour
{
    [SerializeField] ItemInHand itemInHand;
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
        if(itemInHand.HasItem() && uiSlotDisplay != null)
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
        if (itemInHand.HasItem() == false)
        {
            itemInHand.ClearSlot();
            return;
        }

        if(uiSlotDisplay != null)
        {
            uiSlotDisplay.GetComponent<UIItemSlot>().slotImage.sprite = itemInHand.itemSlot.itemObject.icon;
            uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = itemInHand.itemSlot.itemQuantity.ToString();         
        }
        else
        {
            uiSlotDisplay = Instantiate(uiSlotPrefab, this.transform.parent.transform);
            uiSlotDisplay.GetComponent<UIItemSlot>().slotImage.sprite = itemInHand.itemSlot.itemObject.icon;
            uiSlotDisplay.GetComponent<UIItemSlot>().amountItemInSlotText.text = itemInHand.itemSlot.itemQuantity.ToString();

            if (parent != null)
            {
                uiSlotDisplay.transform.SetParent(parent.transform);
            }

            uiSlotDisplay.name = $"InHandItem";
            uiSlotDisplay.GetComponent<RectTransform>().SetAsLastSibling();
        }
    }


}
