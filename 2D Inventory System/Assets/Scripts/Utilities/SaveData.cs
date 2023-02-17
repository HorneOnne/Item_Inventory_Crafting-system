using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

[System.Serializable]
public class SaveData
{
    public string path = Application.dataPath + "/Saves/";
    public PlayerInventorySaveData playerInventoryData;
    public ItemOnGroundSaveData itemOnGroundData;
    public WorldChest worldChest;


    public SaveData(PlayerInventory playerInventory, List<Item> itemsOnGround, List<Chest> chestObjects)
    {
        playerInventoryData = new PlayerInventorySaveData(playerInventory);
        itemOnGroundData = new ItemOnGroundSaveData(itemsOnGround);
        worldChest = new WorldChest(chestObjects);
        //chestInventorySaveData = new ChestInventorySaveData(chestObject.transform.position, chestObject.transform.rotation.eulerAngles); ;    
    }

    public void SaveAllData()
    {
        Save(playerInventoryData, "playerInventory");
        Save(itemOnGroundData, "itemsOnGround");

        Save(worldChest, "chestInventory");
    }

    public void LoadAllData()
    {
        playerInventoryData = Load<PlayerInventorySaveData>("playerInventory");
        itemOnGroundData = Load<ItemOnGroundSaveData>("itemsOnGround");

        worldChest = Load<WorldChest>("chestInventory");
    }


    public void Save<T>(T objectToSave, string key)
    {
        Directory.CreateDirectory(path);
        string jsonString = JsonUtility.ToJson(objectToSave);
        using (StreamWriter sw = new StreamWriter($"{path}{key}.txt"))
        {
            sw.Write(jsonString);
        }

        Debug.Log($"Saved at path: {path}{key}.txt");
    }

    public T Load<T>(string key)
    {
        T returnValue = default(T);
        if (File.Exists($"{path}{key}.txt"))
        {
            string jsonString = "";
            // LOAD DATA
            using (StreamReader sr = new StreamReader($"{path}{key}.txt"))
            {
                jsonString = sr.ReadToEnd();
                returnValue = JsonUtility.FromJson<T>(jsonString);
                Debug.Log("Loaded.");
            }
        }
        else
        {
            Debug.Log("NOT FOUND FILE.");
        }

        return returnValue;
    }



}

[System.Serializable]
public struct PlayerInventorySaveData
{
    public List<ItemSlotSaveData> itemDatas;

    public PlayerInventorySaveData(PlayerInventory playerInventory)
    {
        itemDatas = new List<ItemSlotSaveData>();
        for (int i = 0; i < playerInventory.inventory.Count; i++)
        {
            var itemSlot = playerInventory.inventory[i];
            itemDatas.Add(new ItemSlotSaveData(ItemContainerManager.Instance.GetItemID(itemSlot.ItemData), itemSlot.ItemQuantity));
        }
    }
}

[System.Serializable]
public struct ItemSlotSaveData
{
    public int itemID;
    public int itemQuantity;

    public ItemSlotSaveData(int itemID, int itemQuantity)
    {
        this.itemID = itemID;
        this.itemQuantity = itemQuantity;
    }
}

[System.Serializable]
public struct ItemOnGroundSaveData
{
    public List<ItemObjectData> itemDatas;

    public ItemOnGroundSaveData(List<Item> itemsOnGround)
    {
        itemDatas = new List<ItemObjectData>();
        for (int i = 0; i < itemsOnGround.Count; i++)
        {
            int itemID = ItemContainerManager.Instance.GetItemID(itemsOnGround[i].ItemData);

            itemDatas.Add(new ItemObjectData(itemID, itemsOnGround[i].transform.position, itemsOnGround[i].transform.eulerAngles));
        }
        
    }

    [System.Serializable]
    public struct ItemObjectData
    {
        public int itemID;
        public Vector2 position;
        public Vector3 rotation;

        public ItemObjectData(int itemID, Vector2 position, Vector3 rotation)
        {
            this.itemID = itemID;
            this.position = position;
            this.rotation = rotation;
        }
    }
}


[System.Serializable]
public struct WorldChest
{
    public List<ChestInventorySaveData> allChests;

    public WorldChest(List<Chest> chests)
    {
        allChests = new List<ChestInventorySaveData>();
        for (int i = 0; i < chests.Count; i++)
        {
            allChests.Add(new ChestInventorySaveData(chests[i].Inventory, chests[i].transform.position, chests[i].transform.rotation.eulerAngles));
        }
    }



    [System.Serializable]
    public struct ChestInventorySaveData
    {
        public List<ItemSlotSaveData> itemSlotData;
        public Vector2 position;
        public Vector3 rotation;

        public ChestInventorySaveData(ChestInventory chestInventory, Vector2 position, Vector3 rotation)
        {
            this.itemSlotData = new List<ItemSlotSaveData>();
            this.position = position;
            this.rotation = rotation;

            for (int i = 0; i < 36; i++)
            {
                var itemSlot = chestInventory.inventory[i];
                itemSlotData.Add(new ItemSlotSaveData(ItemContainerManager.Instance.GetItemID(itemSlot.ItemData), itemSlot.ItemQuantity));
            }
        }
    }
}




