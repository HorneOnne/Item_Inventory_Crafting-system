using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainerManager : Singleton<ItemContainerManager>
{
    public List<ItemData> itemList;
    public HashSet<ItemData> itemSet = new HashSet<ItemData>();

    private void Awake()
    {
        GenerateItemDict();
    }

    private void GenerateItemDict()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] != null && itemSet.Contains(itemList[i]))
                Debug.LogError($"[ItemDictionary]: \tItem at {i} already exist.");
            else
            {
                itemSet.Add(itemList[i]);
            }

        }
    }
   


}
