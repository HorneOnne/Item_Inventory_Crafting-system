using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UltimateItemSystem
{
    /// <summary>
    /// A class representing an anvil item, which can be placed in the game world and used to upgrade other items.
    /// Implements the IPlaceable and IPointerClickHandler interfaces.
    /// </summary>
    public class Anvil : Item, IPlaceable, IPointerClickHandler
    {
        [field: SerializeField]
        public bool ShowRay { get; set; }
        [field: SerializeField]
        public LayerMask PlacedLayer { get; set; }


        /// <summary>
        /// The canvas object for the anvil UI.
        /// </summary>
        [Header("References")]
        private GameObject uiAnvilCanvas;


        /// <summary>
        /// Whether the anvil UI is currently open.
        /// </summary>
        [Header("Anvil Properties")]
        private bool isAnvilOpen;


        [Header("Data container")]
        /// <summary>
        /// The input slot for the item being upgraded.
        /// </summary>
        [Header("Data container")]
        public ItemSlot upgradeItemInputSlot;

        /// <summary>
        /// The output slot for the upgraded item.
        /// </summary>
        public ItemSlot upgradeItemOutputSlot;

        /// <summary>
        /// The list of item slots needed for the upgrade.
        /// </summary>
        public List<ItemSlot> requiredMaterialSlots;

        /// <summary>
        /// The list of item slots that have been filled with materials.
        /// </summary>
        public List<ItemSlot> filledMaterialSlots;


        #region Properties
        /// <summary>
        /// Whether there are sufficient materials to perform the upgrade.
        /// </summary>
        public bool IsUpgradePossible { get; private set; }
        #endregion


        private void OnEnable()
        {
            EventManager.OnInputUpgradeItemChanged += OnItemInputChanged;
            EventManager.OnMaterialInputUpgradeItemChanged += IsSufficientMaterials;
        }

        private void OnDisable()
        {
            EventManager.OnInputUpgradeItemChanged -= OnItemInputChanged;
            EventManager.OnMaterialInputUpgradeItemChanged -= IsSufficientMaterials;
        }


        protected override void Start()
        {
            base.Start();
            uiAnvilCanvas = UIManager.Instance.AnvilCanvas;
            requiredMaterialSlots = new List<ItemSlot>();
            filledMaterialSlots = new List<ItemSlot>();
        }



        public bool IsAboveGround(Player player, bool showRay = false)
        {
            bool canBePlaced = false;
            RaycastHit2D hit = Physics2D.Raycast(UIItemInHand.Instance.uiSlotDisplay.transform.position, Vector2.down, 2.0f, PlacedLayer);

            if (showRay)
                Debug.DrawRay(UIItemInHand.Instance.uiSlotDisplay.transform.position, Vector2.down * 2.0f, Color.blue, 1);

            if (hit.collider != null)
            {
                canBePlaced = true;
            }

            return canBePlaced;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }


        public void Placed(Vector3 placedPosition, Player player = null, Transform parent = null)
        {
            Vector3 cachedLocalScale = transform.localScale;
            base.spriteRenderer.enabled = true;

            if (parent != null)
                transform.parent = parent.transform;

            gameObject.SetActive(true);
            transform.position = placedPosition;
            transform.localScale = cachedLocalScale;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            
            if(player != null)
            {
                player.ItemInHand.RemoveItem();
                UIItemInHand.Instance.UpdateItemInHandUI();
            }    
        }


        /// <summary>
        /// Shows the UI of the crafting table.
        /// </summary>
        private void ShowCraftingTableUI()
        {
            uiAnvilCanvas.SetActive(true);

        }

        /// <summary>
        /// Hides the UI of the crafting table.
        /// </summary>
        private void HideCraftingTableUI()
        {
            uiAnvilCanvas.SetActive(false);
        }


        private void Toggle()
        {
            isAnvilOpen = !isAnvilOpen;

            if (isAnvilOpen)
            {
                Open(true);
            }
            else
            {
                Close(true);
            }
        }

        private void OnItemInputChanged()
        {
            UpdateUpgradedItemOutput();
        }


        /// <summary>
        /// Opens the Anvil.
        /// </summary>
        /// <param name="forceOpenUI">A boolean indicating whether to force opening the UI or not.</param>
        public void Open(bool forceOpenUI = false)
        {
            if (forceOpenUI)
                ShowCraftingTableUI();

            isAnvilOpen = true;

            UIAnvil.Instance.Set(this);

        }


        /// <summary>
        /// Closes the Anvil.
        /// </summary>
        /// <param name="forceCloseUI">A boolean indicating whether to force closing the UI or not.</param>
        public void Close(bool forceCloseUI = false)
        {
            if (forceCloseUI)
                HideCraftingTableUI();

            isAnvilOpen = false;

            UIAnvil.Instance.Set(null);
        }


        public bool HasInputUpgradeItem()
        {
            if (upgradeItemInputSlot == null)
                return false;

            return upgradeItemInputSlot.HasItem();
        }

        public bool HasOuputUpgradeItem()
        {
            if (upgradeItemOutputSlot == null)
                return false;

            return upgradeItemOutputSlot.HasItem();
        }



        /// <summary>
        /// Update the upgraded item output slot and required/filled material slots based on the upgradeable item data in the upgrade item input slot.
        /// </summary>
        public void UpdateUpgradedItemOutput()
        {
            DropRemainingMaterials();
            requiredMaterialSlots.Clear();
            filledMaterialSlots.Clear();
            IsUpgradePossible = false;
            upgradeItemOutputSlot.ClearSlot();


            if (upgradeItemInputSlot.ItemData is UpgradeableItemData == false) return;

            UpgradeableItemData itemData = (UpgradeableItemData)upgradeItemInputSlot.ItemData;
            ItemUpgradeRecipe recipe = itemData.upgradeRecipe;

            upgradeItemOutputSlot = new ItemSlot(recipe.outputItem.itemData, recipe.outputItem.quantity);
            ItemUpgradeRecipe.RecipeSlot material;
            for (int i = 0; i < recipe.materials.Count; i++)
            {
                material = recipe.materials[i];
                requiredMaterialSlots.Add(new ItemSlot(material.itemData, material.quantity));
                filledMaterialSlots.Add(new ItemSlot());
            }
        }


        /// <summary>
        /// Check if the required materials for upgrading an item are sufficient.
        /// </summary>
        private void IsSufficientMaterials()
        {
            if (requiredMaterialSlots == null || filledMaterialSlots == null) return;
            if (requiredMaterialSlots.Count == 0 || filledMaterialSlots.Count == 0) return;

            HashSet<int> indexRemoval = new HashSet<int>();
            IsUpgradePossible = true;
            if (requiredMaterialSlots.Count != filledMaterialSlots.Count)
            {
                IsUpgradePossible = false;
            }
            else
            {
                for (int i = 0; i < requiredMaterialSlots.Count; i++)
                {
                    bool foundMatch = false;

                    for (int j = 0; j < filledMaterialSlots.Count; j++)
                    {
                        if (indexRemoval.Contains(j))
                            continue;

                        if (requiredMaterialSlots[i].ItemData == filledMaterialSlots[j].ItemData)
                        {
                            if (requiredMaterialSlots[i].ItemQuantity <= filledMaterialSlots[j].ItemQuantity)
                            {
                                indexRemoval.Add(j);

                                foundMatch = true;
                                break;
                            }
                        }
                    }

                    if (!foundMatch)
                    {
                        IsUpgradePossible = false;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Check if there are any materials in the filled material slots.
        /// </summary>
        /// <returns>Returns true if there are any materials in the filled material slots, false otherwise.</returns>
        public bool HasMaterials()
        {
            bool hasMaterials = false;

            for (int i = 0; i < filledMaterialSlots.Count; i++)
            {
                if (filledMaterialSlots[i].HasItem())
                {
                    hasMaterials = true;
                    break;
                }
            }

            return hasMaterials;
        }


        /// <summary>
        /// Attempt to add an item to the upgrade item input slot.
        /// </summary>
        /// <param name="itemSlot">The item slot to add to the upgrade item input slot.</param>
        /// <returns>Returns true if the item was successfully added, false otherwise.</returns>
        public bool AddItemInputSlot(ItemSlot itemSlot)
        {
            if (itemSlot == null) return false;
            if (itemSlot.HasItem() == false) return false;
            bool canAdd = false;

            if (itemSlot.ItemData is UpgradeableItemData)
            {
                upgradeItemInputSlot = new ItemSlot(itemSlot);
                canAdd = true;
            }

            return canAdd;
        }


        /// <summary>
        /// Consume the required materials for upgrading an item from the filled material slots.
        /// </summary>
        public void ComsumeMaterials()
        {
            if (IsUpgradePossible)
            {
                for (int i = 0; i < requiredMaterialSlots.Count; i++)
                {
                    for (int j = 0; j < filledMaterialSlots.Count; j++)
                    {
                        if (requiredMaterialSlots[i].ItemData == filledMaterialSlots[j].ItemData)
                        {
                            int remainItemQuantity = (filledMaterialSlots[j].ItemQuantity - requiredMaterialSlots[i].ItemQuantity);

                            if (remainItemQuantity == 0)
                                filledMaterialSlots[j].ClearSlot();
                            else
                                filledMaterialSlots[j].SetItemQuantity(remainItemQuantity);
                        }
                    }
                }
            }

        }


        /// <summary>
        /// Drop any remaining materials in the filled material slots.
        /// </summary>
        public void DropRemainingMaterials()
        {
            for (int i = 0; i < filledMaterialSlots.Count; i++)
            {
                if (filledMaterialSlots[i].HasItem())
                {
                    Item itemObject = Utilities.InstantiateItemObject(filledMaterialSlots[i], GameDataManager.Instance.itemContainerParent);
                    itemObject.SetData(filledMaterialSlots[i]);

                    Vector2 dropPosition = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 2; new Vector2(0, 3);
                    itemObject.Drop(null, dropPosition, Vector3.zero, true);
                }
            }
        }
    }
}

