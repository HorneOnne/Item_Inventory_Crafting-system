using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainerManager : Singleton<ItemContainerManager>
{
    [SerializeField] List<ItemData> itemList;
    public HashSet<ItemData> itemDataSet = new HashSet<ItemData>();


    [SerializeField] List<GameObject> itemsPrefab = new List<GameObject>();
    private Dictionary<string, GameObject> itemPrefabDict= new Dictionary<string, GameObject>();

    private void Awake()
    {
        GenerateItemDict();
        GenerateItemPrefabDict();
    }

    private void GenerateItemDict()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i] != null && itemDataSet.Contains(itemList[i]))
                Debug.LogError($"[ItemDictionary]: \tItem at {i} already exist.");
            else
            {
                itemDataSet.Add(itemList[i]);
            }

        }
    }

    private void GenerateItemPrefabDict()
    {
        for (int i = 0; i < itemsPrefab.Count; i++)
        {
            if (itemsPrefab[i] != null && itemPrefabDict.ContainsKey(itemsPrefab[i].name))
                Debug.LogError($"[ItemDictionary]: \tItem Prefab at {i} already exist.");
            else
            {
                itemPrefabDict.Add(itemsPrefab[i].name, itemsPrefab[i]);
            }
        }
    }

    public GameObject GetItemPrefab(string name)
    {
        if (itemPrefabDict.ContainsKey(name))
            return itemPrefabDict[name];
        else
            return null;
    }


}
