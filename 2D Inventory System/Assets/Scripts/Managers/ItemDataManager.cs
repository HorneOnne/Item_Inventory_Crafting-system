using System.Collections.Generic;
using UnityEngine;

namespace DIVH_InventorySystem
{
    public class ItemDataManager : Singleton<ItemDataManager>
    {
        [Header("ITEM DATA")]
        [SerializeField] List<ItemData> itemData;
        public Dictionary<ItemData, int> itemDataDict = new Dictionary<ItemData, int>();
        public Dictionary<int, ItemData> itemDataIDDict = new Dictionary<int, ItemData>();



        [Header("RECIPE")]
        public List<RecipeData> itemRecipeData;
        public Dictionary<RecipeData, ItemSlot> recipeToItemDict;
        public Dictionary<ItemData, RecipeData> itemToRecipeDict;


        [Header("ITEM PREFAB")]
        [SerializeField] List<GameObject> prefabObject = new List<GameObject>();
        private Dictionary<string, GameObject> prefabObjectDict = new Dictionary<string, GameObject>();


        [Header("DUST DATA")]
        public List<ProjectileParticleData> projectileParticleDatas;



        [Header("ITEM OBJECT PARENT")]
        public Transform itemContainerParent;


        private void Awake()
        {
            GenerateItemDict();
            GeneratePrefabObjectDict();
            InitialRecipe();
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



        public void InitialRecipe()
        {
            recipeToItemDict = new Dictionary<RecipeData, ItemSlot>();
            itemToRecipeDict = new Dictionary<ItemData, RecipeData>();
            int index = 0;
            foreach (var recipe in itemRecipeData)
            {
                if (recipeToItemDict.ContainsKey(recipe))
                {
                    Debug.LogError($"[ItemRecipeManager]: \tRecipe at {index} already exist.");
                }
                else
                {
                    //recipeScriptableObjectDict.Add(recipe, recipe.outputItem);
                    recipeToItemDict.Add(recipe, new ItemSlot(recipe.outputItem, recipe.quantityItemOutput));
                }

                index++;

            }

            itemRecipeData.Clear();

            foreach (var recipe in recipeToItemDict)
            {
                if (itemToRecipeDict.ContainsKey(recipe.Value.ItemData) == false)
                {
                    itemToRecipeDict.Add(recipe.Value.ItemData, recipe.Key);
                }
            }
        }

        public ItemSlot GetItemFromRecipe(RecipeData recipe)
        {
            if (recipeToItemDict.ContainsKey(recipe))
            {
                ItemSlot returnedSlot = new ItemSlot(recipeToItemDict[recipe]);
                return returnedSlot;
            }
            return null;
        }


        public RecipeData GetRecipeFromItem(ItemData itemData)
        {
            if (itemToRecipeDict.ContainsKey(itemData))
            {
                return itemToRecipeDict[itemData];
            }
            return null;
        }


        public List<Sprite> GetProjectileParticleFrames(int index)
        {
            return projectileParticleDatas[index].frames;
        }
    }
}