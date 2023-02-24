using System.Collections.Generic;
using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Manages saving and loading game data, including the player inventory and items and chests on the ground.
    /// </summary>
    public class SaveManager : Singleton<SaveManager>
    {
        /// <summary>
        /// The parent transform of all items on the ground.
        /// </summary>
        public Transform itemContainerParent;

        /// <summary>
        /// The data to be saved and loaded.
        /// </summary>
        private SaveData saveData;

        /// <summary>
        /// The player's inventory.
        /// </summary>
        public PlayerInventory playerInventory;

        /// <summary>
        /// The list of all items on the ground.
        /// </summary>
        private List<Item> itemsOnGround;

        /// <summary>
        /// The list of all chests on the ground.
        /// </summary>
        private List<Chest> chestsOnGround;


        private void Start()
        {
            itemsOnGround = new List<Item>();
            chestsOnGround = new List<Chest>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                itemsOnGround = GetAllItemsOnGround();
                chestsOnGround = GetAllChests();

                saveData = new SaveData(playerInventory, itemsOnGround, chestsOnGround);


                saveData.SaveAllData();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                saveData = new SaveData(playerInventory, itemsOnGround, chestsOnGround);
                saveData.LoadAllData();


                var playerInventoryData = saveData.playerInventoryData;
                for (int i = 0; i < playerInventoryData.itemDatas.Count; i++)
                {
                    int id = playerInventoryData.itemDatas[i].itemID;
                    int amount = playerInventoryData.itemDatas[i].itemQuantity;
                    playerInventory.inventory[i] = new ItemSlot(GameDataManager.Instance.GetItemData(id), amount);
                }


                CreateItemObject();
                CreateChestObject();
                UIPlayerInventory.Instance.UpdateInventoryUI();
            }


        }


        /// <summary>
        /// Gets all items on the ground except for chests.
        /// </summary>
        /// <returns>A list of all items on the ground except for chests.</returns>
        private List<Item> GetAllItemsOnGround()
        {
            int itemCount = itemContainerParent.childCount;
            var itemsOnGround = new List<Item>();


            for (int i = 0; i < itemCount; i++)
            {
                var itemObject = itemContainerParent.GetChild(i).GetComponent<Item>();
                if (itemObject != null && itemObject is not Chest)
                {
                    itemsOnGround.Add(itemObject);
                }
            }

            return itemsOnGround;
        }


        /// <summary>
        /// Gets all chests on the ground.
        /// </summary>
        /// <returns>A list of all chests on the ground.</returns>
        private List<Chest> GetAllChests()
        {
            int numOfChest = itemContainerParent.childCount;
            var chestsInTheWorld = new List<Chest>();

            for (int i = 0; i < numOfChest; i++)
            {
                var chestObject = itemContainerParent.GetChild(i).GetComponent<Chest>();
                if (chestObject != null)
                {
                    chestsInTheWorld.Add(chestObject);
                }
            }

            return chestsInTheWorld;
        }



        /// <summary>
        /// Instantiates all items on the ground from the save data.
        /// </summary>
        private void CreateItemObject()
        {
            for (int i = 0; i < saveData.itemOnGroundData.itemDatas.Count; i++)
            {
                var itemObjectData = saveData.itemOnGroundData.itemDatas[i];
                int itemID = itemObjectData.itemID;
                var itemPosition = itemObjectData.position;
                var itemRotation = itemObjectData.rotation;

                var itemData = GameDataManager.Instance.GetItemData(itemID);
                var itemPrefab = GameDataManager.Instance.GetItemPrefab($"IP_DropItem");

                var itemObject = Instantiate(itemPrefab, itemPosition, Quaternion.Euler(itemRotation), itemContainerParent);

                Debug.Log("Fix here");
                itemObject.GetComponent<DropItem>().Set(new ItemSlot(itemData, 1));

            }
        }


        /// <summary>
        /// Instantiates all chests on the ground from the save data.
        /// </summary>
        private void CreateChestObject()
        {
            var chestPrefab = GameDataManager.Instance.GetItemPrefab("IP_Chest");

            for (int i = 0; i < saveData.worldChest.allChests.Count; i++)
            {
                WorldChest.ChestInventorySaveData chestInventoryData = saveData.worldChest.allChests[i];
                var chestObject = Instantiate(chestPrefab, chestInventoryData.position, Quaternion.Euler(chestInventoryData.rotation), itemContainerParent);
                var chestInventory = chestObject.GetComponent<ChestInventory>();
                chestInventory.inventory.Clear();

                for (int j = 0; j < 36; j++)
                {
                    int id = chestInventoryData.itemSlotData[j].itemID;
                    int amount = chestInventoryData.itemSlotData[j].itemQuantity;

                    chestInventory.inventory.Add(new ItemSlot(GameDataManager.Instance.GetItemData(id), amount));
                }

            }

        }
    }
}
