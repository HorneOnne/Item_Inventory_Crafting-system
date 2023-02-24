using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Manages all the item data, recipes, prefabs, and dust particle data,...
    /// </summary>
    public class GameDataManager : Singleton<GameDataManager>
    {
        private const string itemPrefabPrefix = "IP_";
        private const string projectilePrefabPrefix = "PP_";
        private const string itemDataPrefix = "I_";
        private const string upgradeableItemDataPrefix = "UI_";
        private const string craftingRecipePrefix = "CR_";


        /// <summary>
        /// A list of all the item data.
        /// </summary>
        [Header("ITEM DATA")]
        [SerializeField] List<ItemData> itemData;

        /// <summary>
        /// A list of all the item prefabs.
        /// </summary>
        [Header("PREFABS")]
        [SerializeField] List<GameObject> itemPrefabs = new List<GameObject>();


        [SerializeField] List<GameObject> projectilePrefabs = new List<GameObject>();

        /// <summary>
        /// A list of all the recipe data.
        /// </summary>
        [Header("RECIPE")]
        public List<RecipeData> itemRecipeData;

        /// <summary>
        /// A dictionary that maps item data to their index in the <see cref="itemData"/> list.
        /// </summary>
        public Dictionary<ItemData, int> itemDataDict = new Dictionary<ItemData, int>();

        /// <summary>
        /// A dictionary that maps item data indices to their corresponding item data.
        /// </summary>
        public Dictionary<int, ItemData> itemDataById = new Dictionary<int, ItemData>();


        /// <summary>
        /// A dictionary that maps recipe data to their corresponding item slot.
        /// </summary>
        public Dictionary<RecipeData, ItemSlot> recipeToItemDict;

        /// <summary>
        /// A dictionary that maps item data to their corresponding recipe data.
        /// </summary>
        public Dictionary<ItemData, RecipeData> itemToRecipeDict;



        /// <summary>
        /// A dictionary that maps item prefab names to their corresponding game objects.
        /// </summary>
        private Dictionary<string, GameObject> itemPrefabByNameDict = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> projectilePrefabByNameDict = new Dictionary<string, GameObject>();



        /// <summary>
        /// A list of all the dust particle data.
        /// </summary>
        [Header("DUST DATA")]
        public List<ProjectileParticleData> projectileParticleDatas;




        /// <summary>
        /// The parent transform for all the item objects.
        /// </summary>
        [Header("ITEM OBJECT PARENT")]
        public Transform itemContainerParent;



        /// <summary>
        /// Initializes the item data and prefab dictionaries, and generates the recipe dictionaries.
        /// </summary>
        private void Awake()
        {
            GenerateItemDataDict();
            GenerateItemPrefabDictionary();
            GenerateProjectilePrefabDictionary();
            InitializeRecipes();
        }



        /// <summary>
        /// Generates the <see cref="itemDataDict"/> and <see cref="itemDataById"/> dictionaries.
        /// </summary>
        private void GenerateItemDataDict()
        {
            for (int i = 0; i < itemData.Count; i++)
            {
                if (itemData[i] != null && itemDataDict.ContainsKey(itemData[i]))
                    Debug.LogError($"[ItemDictionary]: \tItem at {i} already exist.");
                else
                {
                    itemDataDict.Add(itemData[i], i);
                    itemDataById.Add(i, itemData[i]);
                }
            }
        }


        /// <summary>
        /// Returns the index of the given <paramref name="itemData"/>.
        /// </summary>
        /// <param name="itemData">The item data to look up.</param>
        /// <returns>The index of the <paramref name="itemData"/> in the <see cref="itemData"/> list.</returns>
        public int GetItemID(ItemData itemData)
        {
            if (itemData == null)
                return -1;

            return itemDataDict[itemData];
        }


        /// <summary>
        /// Gets the item data for the specified item ID.
        /// </summary>
        /// <param name="id">The ID of the item data to retrieve.</param>
        /// <returns>The item data with the specified ID, or null if no item data with that ID exists.</returns>
        public ItemData GetItemData(int id)
        {
            if (id == -1)
                return null;

            return itemDataById[id];
        }



        /// <summary>
        /// Generates a dictionary of item prefabs by name.
        /// </summary>
        private void GenerateItemPrefabDictionary()
        {
            for (int i = 0; i < itemPrefabs.Count; i++)
            {
                if (itemPrefabs[i] != null && itemPrefabByNameDict.ContainsKey(itemPrefabs[i].name))
                    Debug.LogError($"[ItemDictionary]: \tItem Prefab at {i} already exist.");
                else
                {
                    itemPrefabByNameDict.Add(itemPrefabs[i].name, itemPrefabs[i]);
                }
            }
        }

        /// <summary>
        /// Generates a dictionary of projectile prefabs by name.
        /// </summary>
        private void GenerateProjectilePrefabDictionary()
        {
            for (int i = 0; i < projectilePrefabs.Count; i++)
            {
                if (projectilePrefabs[i] != null && projectilePrefabByNameDict.ContainsKey(projectilePrefabs[i].name))
                    Debug.LogError($"[ItemDictionary]: \tProjectile Prefab at {i} already exist.");
                else
                {
                    projectilePrefabByNameDict.Add(projectilePrefabs[i].name, projectilePrefabs[i]);
                }
            }
        }



        /// <summary>
        /// Gets the item prefab for the item with the specified name.
        /// </summary>
        /// <param name="name">The name of the item prefab to retrieve.</param>
        /// <returns>The prefab for the item with the specified name, or null if no prefab with that name exists.</returns>
        public GameObject GetItemPrefab(string name)
        {
            if (itemPrefabByNameDict.ContainsKey(name))
                return itemPrefabByNameDict[name];
            else
            {
                throw new System.Exception($"Not found item prefab name {name} in GameDataManager.cs.");
            }
        }

        public GameObject GetItemPrefab<T>(T item)
        {
            string name = $"{itemPrefabPrefix}{typeof(T).Name}";
            if (itemPrefabByNameDict.ContainsKey(name))
                return itemPrefabByNameDict[name];
            else
            {
                throw new System.Exception($"Not found item prefab name {name} in GameDataManager.cs.");
            }
        }



        /// <summary>
        /// Gets the projectile prefab for the item with the specified name.
        /// </summary>
        /// <param name="name">The name of the item prefab to retrieve.</param>
        /// <returns>The prefab for the item with the specified name, or null if no prefab with that name exists.</returns>
        public GameObject GetProjectilePrefab(string name)
        {
            if (projectilePrefabByNameDict.ContainsKey(name))
                return projectilePrefabByNameDict[name];
            else
            {
                throw new System.Exception($"Not found projectile prefab name {name} in GameDataManager.cs.");
            }
        }


        public GameObject GetProjectilePrefab<T>(T item)
        {
            string name = $"{projectilePrefabPrefix}{typeof(T).Name}";
            if (projectilePrefabByNameDict.ContainsKey($"{name}"))
                return projectilePrefabByNameDict[name];
            else
            {
                throw new System.Exception($"Not found projectile prefab name {name} in GameDataManager.cs.");
            }
        }



        /// <summary>
        /// Initializes the recipe dictionaries.
        /// </summary>
        public void InitializeRecipes()
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


        /// <summary>
        /// Gets the item slot for the specified recipe data.
        /// </summary>
        /// <param name="recipe">The recipe data to retrieve the item slot for.</param>
        /// <returns>The item slot for the specified recipe data, or null if no item slot exists for that recipe.</returns>
        public ItemSlot GetItemFromRecipe(RecipeData recipe)
        {
            if (recipeToItemDict.ContainsKey(recipe))
            {
                ItemSlot returnedSlot = new ItemSlot(recipeToItemDict[recipe]);
                return returnedSlot;
            }
            return null;
        }


        /// <summary>
        /// Returns the recipe data that corresponds to the given item data, if it exists.
        /// </summary>
        /// <param name="itemData">The item data to find the corresponding recipe for.</param>
        /// <returns>The recipe data that corresponds to the given item data, or null if it does not exist.</returns>
        public RecipeData GetRecipeFromItem(ItemData itemData)
        {
            if (itemToRecipeDict.ContainsKey(itemData))
            {
                return itemToRecipeDict[itemData];
            }
            return null;
        }


        /// <summary>
        /// Returns a list of sprite frames for the projectile particle data at the given index.
        /// </summary>
        /// <param name="index">The index of the projectile particle data to get sprite frames for.</param>
        /// <returns>A list of sprite frames for the projectile particle data at the given index.</returns>
        public List<Sprite> GetProjectileParticleFrames(int index)
        {
            return projectileParticleDatas[index].frames;
        }
    }
}