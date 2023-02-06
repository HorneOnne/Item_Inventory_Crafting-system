using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainerManager : Singleton<ItemContainerManager>
{
    [SerializeField] List<ItemData> itemList;
    public HashSet<ItemData> itemDataSet = new HashSet<ItemData>();


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
