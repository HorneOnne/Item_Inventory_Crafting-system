using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace UltimateItemSystem
{
    /// <summary>
    /// Class representing the save data for the game. Contains the saved data for the player inventory, items on the ground, and world chests.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        /// <summary>
        /// The path where the save files will be stored.
        /// </summary>
        public string path = Application.dataPath + "/UltimateItemSystem/Saves/";

        public PlayerInventorySaveData playerInventoryData;
        public ItemOnGroundSaveData itemOnGroundData;
        public WorldChest worldChest;


        /// <summary>
        /// Initializes a new instance of the <see cref="SaveData"/> class.
        /// </summary>
        /// <param name="playerInventory">The player inventory.</param>
        /// <param name="itemsOnGround">The list of items on the ground.</param>
        /// <param name="chestObjects">The list of chest objects in the world.</param>
        public SaveData(PlayerInventory playerInventory, List<Item> itemsOnGround, List<Chest> chestObjects)
        {
            playerInventoryData = new PlayerInventorySaveData(playerInventory);
            itemOnGroundData = new ItemOnGroundSaveData(itemsOnGround);
            worldChest = new WorldChest(chestObjects);            
        }


        /// <summary>
        /// Saves all the data to files.
        /// </summary>
        public void SaveAllData()
        {
            Save(playerInventoryData, "playerInventory");
            Save(itemOnGroundData, "itemsOnGround");
            Save(worldChest, "chestInventory");
        }


        /// <summary>
        /// Loads all the data from files.
        /// </summary>
        public void LoadAllData()
        {
            playerInventoryData = Load<PlayerInventorySaveData>("playerInventory");
            itemOnGroundData = Load<ItemOnGroundSaveData>("itemsOnGround");
            worldChest = Load<WorldChest>("chestInventory");
        }


        /// <summary>
        /// Saves the specified object to a file with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the object to save.</typeparam>
        /// <param name="objectToSave">The object to save.</param>
        /// <param name="key">The key to use for the file name.</param>
        public void Save<T>(T objectToSave, string key)
        {
            Directory.CreateDirectory(path);
            string jsonString = JsonUtility.ToJson(objectToSave);
            using (StreamWriter sw = new StreamWriter($"{path}{key}.json"))
            {
                sw.Write(jsonString);
            }

            Debug.Log($"Saved at path: {path}{key}.json");
        }


        /// <summary>
        /// Loads the specified type of object from a file with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the object to load.</typeparam>
        /// <param name="key">The key to use for the file name.</param>
        /// <returns>The loaded object.</returns>

        public T Load<T>(string key)
        {
            T returnValue = default(T);
            if (File.Exists($"{path}{key}.json"))
            {
                string jsonString = "";
                // LOAD DATA
                using (StreamReader sr = new StreamReader($"{path}{key}.json"))
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


    /// <summary>
    /// A struct representing the save data for a player's inventory.
    /// </summary>
    [System.Serializable]
    public struct PlayerInventorySaveData
    {
        /// <summary>
        /// The list of ItemSlotSaveData representing the player's items.
        /// </summary>
        public List<ItemSlotSaveData> itemDatas;


        /// <summary>
        /// Initializes a new instance of the PlayerInventorySaveData struct from a given PlayerInventory.
        /// </summary>
        /// <param name="playerInventory">The PlayerInventory to create the save data from.</param>
        public PlayerInventorySaveData(PlayerInventory playerInventory)
        {
            itemDatas = new List<ItemSlotSaveData>();
            for (int i = 0; i < playerInventory.inventory.Count; i++)
            {
                var itemSlot = playerInventory.inventory[i];
                itemDatas.Add(new ItemSlotSaveData(GameDataManager.Instance.GetItemID(itemSlot.ItemData), itemSlot.ItemQuantity));
            }
        }
    }


    /// <summary>
    /// A struct representing the save data for an item slot.
    /// </summary>
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


    /// <summary>
    /// A struct representing the save data for all items on the ground.
    /// </summary>
    [System.Serializable]
    public struct ItemOnGroundSaveData
    {
        public List<ItemObjectData> itemDatas;

        public ItemOnGroundSaveData(List<Item> itemsOnGround)
        {
            itemDatas = new List<ItemObjectData>();
            for (int i = 0; i < itemsOnGround.Count; i++)
            {
                int itemID = GameDataManager.Instance.GetItemID(itemsOnGround[i].ItemData);

                itemDatas.Add(new ItemObjectData(itemID, itemsOnGround[i].transform.position, itemsOnGround[i].transform.eulerAngles));
            }

        }


        /// <summary>
        /// A struct representing the save data for a single item on the ground.
        /// </summary>
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


    /// <summary>
    /// Represents a data structure that contains the saved data of all chests in the game world.
    /// </summary>
    [System.Serializable]
    public struct WorldChest
    {
        /// <summary>
        /// A list of saved data of all chests in the game world.
        /// </summary>
        public List<ChestInventorySaveData> allChests;


        public WorldChest(List<Chest> chests)
        {
            allChests = new List<ChestInventorySaveData>();
            for (int i = 0; i < chests.Count; i++)
            {
                allChests.Add(new ChestInventorySaveData(chests[i].Inventory, chests[i].transform.position, chests[i].transform.rotation.eulerAngles));
            }
        }


        /// <summary>
        /// Represents the saved data of a chest's inventory.
        /// </summary>
        [System.Serializable]
        public struct ChestInventorySaveData
        {
            /// <summary>
            /// A list of saved data of all items in the chest's inventory.
            /// </summary>
            public List<ItemSlotSaveData> itemSlotData;

            /// <summary>
            /// The position of the chest in the game world.
            /// </summary>
            public Vector2 position;

            /// <summary>
            /// The rotation of the chest in the game world.
            /// </summary>
            public Vector3 rotation;

            public ChestInventorySaveData(ChestInventory chestInventory, Vector2 position, Vector3 rotation)
            {
                this.itemSlotData = new List<ItemSlotSaveData>();
                this.position = position;
                this.rotation = rotation;

                for (int i = 0; i < 36; i++)
                {
                    var itemSlot = chestInventory.inventory[i];
                    itemSlotData.Add(new ItemSlotSaveData(GameDataManager.Instance.GetItemID(itemSlot.ItemData), itemSlot.ItemQuantity));
                }
            }
        }
    }
}




