using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipSlot : MonoBehaviour
{
    public Image equipmentImage;
    public Sprite defaultEquipmentIcon;
    public ItemType equipmentType;


    private void Start()
    {
        SetDefault();
    }

    public void Set(Sprite equipmentIcon)
    {
        if (equipmentIcon == null)
            return;

        Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        equipmentImage.color = color;

        this.equipmentImage.sprite = equipmentIcon;
    }

    public void SetDefault()
    {
        Color color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        equipmentImage.color = color;

        this.equipmentImage.sprite = defaultEquipmentIcon;
    }
}
