using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections;

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


    #region Properties
    [field: SerializeField] public ItemSlot UpgradeItemInputSlot { get; set; }
    [field: SerializeField] public ItemSlot UpgradeItemOutputSlot { get; private set; }

    [field: SerializeField] public List<ItemSlot> MaterialsNeededToUpgrade { get; private set; }
    [field: SerializeField] public List<ItemSlot> MaterialsHasBeenFilled { get; private set; }
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
        MaterialsNeededToUpgrade = new List<ItemSlot>();
        MaterialsHasBeenFilled = new List<ItemSlot>();
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
        Debug.Log("Item Input changed.");
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
        if (UpgradeItemInputSlot == null)
            return false;

        return UpgradeItemInputSlot.HasItem();
    }

    public bool HasOuputUpgradeItem()
    {
        if (UpgradeItemOutputSlot == null)
            return false;

        return UpgradeItemOutputSlot.HasItem();
    }



    public void UpdateUpgradedItemOutput()
    {
        MaterialsNeededToUpgrade.Clear();
        MaterialsHasBeenFilled.Clear();

        UpgradeItemOutputSlot.ClearSlot();
        if (UpgradeItemInputSlot == null)
        {
            return;
        }

        if (UpgradeItemInputSlot.ItemData is UpgradeableItemData == false) return;

        UpgradeableItemData itemData = (UpgradeableItemData)UpgradeItemInputSlot.ItemData;
        ItemUpgradeRecipe recipe = itemData.upgradeRecipe;

        UpgradeItemOutputSlot = new ItemSlot(recipe.outputItem.itemData, recipe.outputItem.quantity);
        ItemUpgradeRecipe.RecipeSlot material;
        for (int i = 0; i < recipe.materials.Count; i++)
        {
            material = recipe.materials[i];
            MaterialsNeededToUpgrade.Add(new ItemSlot(material.itemData, material.quantity));
            MaterialsHasBeenFilled.Add(new ItemSlot());
        }
    }


    /// <summary>
    /// Check materials need to upgrade item is sufficient.
    /// </summary>
    /// <returns></returns>
    private void IsSufficientMaterials()
    {
        if (MaterialsNeededToUpgrade == null || MaterialsHasBeenFilled == null) return;
        if (MaterialsNeededToUpgrade.Count == 0 || MaterialsHasBeenFilled.Count == 0) return;

        IsSufficient = true;
        if (MaterialsNeededToUpgrade.Count != MaterialsHasBeenFilled.Count)
        {
            IsSufficient = false;
        }
        else
        {
            for (int i = 0; i < MaterialsNeededToUpgrade.Count; i++)
            {
                bool foundMatch = false;

                for (int j = 0; j < MaterialsHasBeenFilled.Count; j++)
                {
                    if (MaterialsNeededToUpgrade[i].ItemData == MaterialsHasBeenFilled[j].ItemData)
                    {
                        if (MaterialsNeededToUpgrade[i].ItemQuantity <= MaterialsHasBeenFilled[j].ItemQuantity)
                        {
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

        for(int i = 0; i < MaterialsHasBeenFilled.Count; i++)
        {
            if (MaterialsHasBeenFilled[i].HasItem())
            {
                hasMaterials = true;
                break;
            }
        }

        return hasMaterials;
    }
}


