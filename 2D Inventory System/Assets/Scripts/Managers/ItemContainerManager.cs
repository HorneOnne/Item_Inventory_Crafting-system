using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainerManager : Singleton<ItemContainerManager>
{
    [SerializeField] List<ItemData> itemData;
    public Dictionary<ItemData, int> itemDataDict = new Dictionary<ItemData, int>();
    public Dictionary<int, ItemData> itemDataIDDict = new Dictionary<int, ItemData>();


    [SerializeField] List<GameObject> prefabObject = new List<GameObject>();
    private Dictionary<string, GameObject> prefabObjectDict= new Dictionary<string, GameObject>();


    public Transform itemContainerParent;


    private void Awake()
    {
        GenerateItemDict();
        GeneratePrefabObjectDict();
    }

    private void GenerateItemDict()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            if (itemData[i] != null && itemDataDict.ContainsKey(itemData[i]))
                Debug.LogError($"[ItemDictionary]: \tItem at {i} already exist.");
            else
            {
                itemDataDict.Add(itemData[i], i);
                itemDataIDDict.Add(i, itemData[i]);
            }
        }
    }


    public int GetItemID(ItemData itemData)
    {
        if (itemData == null)
            return -1;

        return itemDataDict[itemData];
    }

    public ItemData GetItemData(int id)
    {
        if (id == -1)
            return null;

        return itemDataIDDict[id];
    }


    private void GeneratePrefabObjectDict()
    {
        for (int i = 0; i < prefabObject.Count; i++)
        {
            if (prefabObject[i] != null && prefabObjectDict.ContainsKey(prefabObject[i].name))
                Debug.LogError($"[ItemDictionary]: \tItem Prefab at {i} already exist.");
            else
            {
                prefabObjectDict.Add(prefabObject[i].name, prefabObject[i]);
            }
        }
    }

    public GameObject GetItemPrefab(string name)
    {
        if (prefabObjectDict.ContainsKey(name))
            return prefabObjectDict[name];
        else
            return null;
    }


}
