using DIVH_InventorySystem;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoExpandParentVerticalLayoutGroup : MonoBehaviour
{
    private VerticalLayoutGroup layoutGroup;
    public GameObject textPrefab;
    public ItemData itemData;


    private const char itemNamePrefix = '~';
    private const char itemDataPrefix = '-';
    private const char itemExtraPrefix = '+';

    private List<TextMeshProUGUI> descTexts = new List<TextMeshProUGUI>();


    void Start()
    {
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        ItemDescriptionHandle();
        ExpandParent();
    }

    private void ExpandParent()
    {
        int childCount = transform.childCount;
        float childHeight = textPrefab.GetComponent<RectTransform>().rect.height;
        float calculatedHeight = childHeight * childCount;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, calculatedHeight);
    }

    private void ItemDescriptionHandle()
    {
        string[] descString = itemData.description.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        descTexts.Clear();

        for (int i = 0; i < descString.Length;i++)
        {
            descTexts.Add(Instantiate(textPrefab, this.transform).GetComponent<TextMeshProUGUI>());
        }


        for(int i = 0; i < descTexts.Count;i++)
        {
            descTexts[i].text = descString[i];
        }


        foreach(var s in descString)
        {
            switch(s[0])
            {
                case itemNamePrefix:
                    Debug.Log($"itemName: {s}");
                    break;
                case itemDataPrefix:
                    Debug.Log($"itemData: {s}");
                    break;
                case itemExtraPrefix:
                    Debug.Log($"item extra: {s}");
                    break;
                default:
                    Debug.LogWarning($"Not found item description prefix {s[0]} definition.");
                    break;
            }            
        }
    }
}
