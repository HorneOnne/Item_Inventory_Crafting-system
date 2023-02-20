using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using MyGame.Ultilities;
using System;

public class Anvil : Item, IPlaceable, IPointerClickHandler
{
    [field: SerializeField]
    public bool ShowRay { get; set; }
    [field: SerializeField]
    public LayerMask PlacedLayer { get; set; }


    [Header("References")]
    private GameObject uiAnvilCanvas;



    [Header("Anvil Properties")]
    private bool isOpen;


    [Header("Data container")]
    public ItemSlot upgradeItemInputSlot;
    public ItemSlot upgradeItemOutputSlot;
    public List<ItemSlot> materialsNeededToUpgrade;
    public List<ItemSlot> materialsHasBeenFilled;


    #region Properties
    public bool IsSufficient { get; private set; }
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
        materialsNeededToUpgrade = new List<ItemSlot>();
        materialsHasBeenFilled = new List<ItemSlot>();

        /*for(int i = 0; i < 8; i++)
        {
            materialsHasBeenFilled.Add(new ItemSlot());
        }*/
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

        if (parent != null)
            transform.parent = parent.transform;

        gameObject.SetActive(true);
        transform.position = placedPosition;
        transform.localScale = cachedLocalScale;
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        player.ItemInHand.RemoveItem();
        UIItemInHand.Instance.UpdateItemInHandUI();
    }

    private void ShowCraftingTableUI()
    {
        uiAnvilCanvas.SetActive(true);

    }

    private void HideCraftingTableUI()
    {
        uiAnvilCanvas.SetActive(false);
    }


    private void Toggle()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            //ShowCraftingTableUI();
            Open(true);
        }
        else
        {
            //HideCraftingTableUI();
            Close(true);
        }
    }

    private void OnItemInputChanged()
    {
        UpdateUpgradedItemOutput();
    }

    /// <summary>
    /// Open chest.
    /// </summary>
    /// <param name="player">Player who opens this chest.</param>
    /// <param name="forceOpenUI"></param>
    public void Open(bool forceOpenUI = false)
    {
        if (forceOpenUI)
            ShowCraftingTableUI();

        isOpen = true;

        UIAnvil.Instance.Set(this);

    }


    /// <summary>
    /// Close chest.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="forceCloseUI">Player who closes this chest.</param>
    public void Close(bool forceCloseUI = false)
    {
        if (forceCloseUI)
            HideCraftingTableUI();

        isOpen = false;

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



    public void UpdateUpgradedItemOutput()
    {
        DropRemainingMaterials();
        materialsNeededToUpgrade.Clear();
        materialsHasBeenFilled.Clear();
        IsSufficient = false;
        upgradeItemOutputSlot.ClearSlot();


        if (upgradeItemInputSlot.ItemData is UpgradeableItemData == false) return;

        UpgradeableItemData itemData = (UpgradeableItemData)upgradeItemInputSlot.ItemData;
        ItemUpgradeRecipe recipe = itemData.upgradeRecipe;

        upgradeItemOutputSlot = new ItemSlot(recipe.outputItem.itemData, recipe.outputItem.quantity);
        ItemUpgradeRecipe.RecipeSlot material;
        for (int i = 0; i < recipe.materials.Count; i++)
        {
            material = recipe.materials[i];
            materialsNeededToUpgrade.Add(new ItemSlot(material.itemData, material.quantity));
            materialsHasBeenFilled.Add(new ItemSlot());
        }
    }


    /// <summary>
    /// Check materials need to upgrade item is sufficient.
    /// </summary>
    /// <returns></returns>
    private void IsSufficientMaterials()
    {
        if (materialsNeededToUpgrade == null || materialsHasBeenFilled == null) return;
        if (materialsNeededToUpgrade.Count == 0 || materialsHasBeenFilled.Count == 0) return;

        HashSet<int> indexRemoval = new HashSet<int>();
        IsSufficient = true;
        if (materialsNeededToUpgrade.Count != materialsHasBeenFilled.Count)
        {
            IsSufficient = false;
        }
        else
        {
            for (int i = 0; i < materialsNeededToUpgrade.Count; i++)
            {
                bool foundMatch = false;

                for (int j = 0; j < materialsHasBeenFilled.Count; j++)
                {
                    if (indexRemoval.Contains(j))
                        continue;

                    if (materialsNeededToUpgrade[i].ItemData == materialsHasBeenFilled[j].ItemData)
                    {
                        if (materialsNeededToUpgrade[i].ItemQuantity <= materialsHasBeenFilled[j].ItemQuantity)
                        {
                            indexRemoval.Add(j);

                            foundMatch = true;
                            break;
                        }
                    }
                }

                if (!foundMatch)
                {
                    IsSufficient = false;
                    break;
                }
            }
        }
    }


    /// <summary>
    /// Check materials exist in material slot.
    /// </summary>
    /// <returns></returns>
    public bool HasMaterials()
    {
        bool hasMaterials = false;

        for (int i = 0; i < materialsHasBeenFilled.Count; i++)
        {
            if (materialsHasBeenFilled[i].HasItem())
            {
                hasMaterials = true;
                break;
            }
        }

        return hasMaterials;
    }


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



    public void ComsumeMaterials()
    {
        if (IsSufficient)
        {
            for (int i = 0; i < materialsNeededToUpgrade.Count; i++)
            {
                for (int j = 0; j < materialsHasBeenFilled.Count; j++)
                {
                    if (materialsNeededToUpgrade[i].ItemData == materialsHasBeenFilled[j].ItemData)
                    {
                        int remainItemQuantity = (materialsHasBeenFilled[j].ItemQuantity - materialsNeededToUpgrade[i].ItemQuantity);

                        if (remainItemQuantity == 0)
                            materialsHasBeenFilled[j].ClearSlot();
                        else
                            materialsHasBeenFilled[j].SetItemQuantity(remainItemQuantity);
                    }
                }
            }
        }

    }

    public void DropRemainingMaterials()
    {
        for (int i = 0; i < materialsHasBeenFilled.Count; i++)
        {
            if (materialsHasBeenFilled[i].HasItem())
            {
                Item itemObject = Utilities.InstantiateItemObject(materialsHasBeenFilled[i], ItemContainerManager.Instance.itemContainerParent);
                itemObject.SetData(materialsHasBeenFilled[i]);

                Vector2 dropPosition = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * 2; new Vector2(0, 3);
                itemObject.Drop(null, dropPosition, Vector3.zero, true);
            }
        }
    }
}


