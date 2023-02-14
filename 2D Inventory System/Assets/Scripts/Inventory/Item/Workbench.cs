using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using static Chest;

public class Workbench : Item, IPlaceable, IPointerClickHandler
{
    [field: SerializeField]
    public bool ShowRay { get; set; }
    [field: SerializeField]
    public LayerMask PlacedLayer { get ; set; }


    [Header("References")]
    private GameObject uiCraftingTableCanvas;

    [Header("Workbench Properties")]
    private bool isOpen;

    protected override void Start()
    {
        base.Start();
        uiCraftingTableCanvas = GameObject.FindGameObjectWithTag("UICraftingTableCanvas");
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
        UIItemInHand.Instance.DisplayItemInHand();
    }

    private void ShowCraftingTableUI()
    {
        uiCraftingTableCanvas.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void HideCraftingTableUI()
    {
        uiCraftingTableCanvas.transform.GetChild(0).gameObject.SetActive(false);
    }


    private void Toggle()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            ShowCraftingTableUI();
        }
        else
        {
            HideCraftingTableUI();
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }
}


