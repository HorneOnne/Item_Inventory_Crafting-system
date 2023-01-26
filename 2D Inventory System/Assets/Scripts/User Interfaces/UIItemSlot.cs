using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIItemSlot : MonoBehaviour
{
    [Header("Internal References")]
    public Image slotImage;
    public TextMeshProUGUI amountItemInSlotText;
    [SerializeField] private short slotIndex;
    public short SlotIndex { get { return slotIndex; } }


    public void Set(Sprite sprite, string amountText = null, short slotIndex = -1)
    {
        this.slotImage.sprite = sprite;
        this.amountItemInSlotText.text = amountText;
        this.slotIndex = slotIndex;
    }
}
