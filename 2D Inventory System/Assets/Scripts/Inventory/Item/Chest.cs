using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ChestInventory))]
public class Chest : Item, IPlaceable, IPointerClickHandler
{
    [Header("References")]
    public GameObject uiChestInventoryCanvas;
    private Animator anim;
    private ChestInventory chestInventory;      // Data
    //private UIChestInventory uiChestInventory;  // UI and logic
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;


    [Header("Chest Properties")]
    [SerializeField] private Player playerWhoOpenChest;
    private bool isOpen;


    #region Properties
    [field: SerializeField]
    public LayerMask PlacedLayer { get; set; }
    #endregion
    public bool FirstPlaced { get; private set; }
    public ChestInventory Inventory { get => chestInventory; }

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        anim = base.Model.GetComponent<Animator>();
        isOpen = false;
        anim.enabled = false;
        chestInventory = GetComponent<ChestInventory>();    
        uiChestInventoryCanvas = GameObject.FindGameObjectWithTag("UIChestInventoryCanvas");
        //uiChestInventory = UIChestInventory.Instance;
        //uiChestInventoryCanvas = uiChestInventory.gameObject.GetComponentInParent<Canvas>().gameObject;
        DisableChestInventoryUI();
    }


    public bool IsAboveGround(Player player)
    {
        bool canBePlaced = false;
        RaycastHit2D hit = Physics2D.Raycast(UIItemInHand.Instance.uiSlotDisplay.transform.position, Vector2.down, 2.5f, PlacedLayer);

        Debug.DrawRay(UIItemInHand.Instance.uiSlotDisplay.transform.position, Vector2.down * 2.5f, Color.blue, 1);
        if (hit.collider != null)
        {
            canBePlaced = true;
        }

        return canBePlaced;
    }

    public void Placed(Vector3 placedPosition, Player player = null, Transform parent = null)
    {
        FirstPlaced = true;

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


    


    private void TurnOffAnimation()
    {
        anim.enabled = false;
    }

    private void TurnOnAnimation()
    {
        anim.enabled = true;
    }

    private void EnableChestInventoryUI()
    {
        uiChestInventoryCanvas.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void DisableChestInventoryUI()
    {
        uiChestInventoryCanvas.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void SetPlayerOpenChest(Player player)
    {
        if (playerWhoOpenChest != null) return;
        if (isOpen == false) return;

        this.playerWhoOpenChest = player;
        this.chestInventory.Set(playerWhoOpenChest);

        UIChestInventory.Instance.SetChestInventoryData(chestInventory);

    }


    public void OpenChest()
    {
        TurnOnAnimation();
        EnableChestInventoryUI();     // Display Chest Inventory UI     
    }

    public void CloseChest()
    {
        Debug.Log("Close Chest");
        this.playerWhoOpenChest = null;
        this.chestInventory.Set(null);
        
        //Invoke("TurnOffAnimation", 1f);
        DisableChestInventoryUI();

        this.playerWhoOpenChest = null;
        this.chestInventory.Set(null);
        UIChestInventory.Instance.RemoveChestInventoryData();
    }

    public void OnPointerClick(PointerEventData eventData)
    {      
        isOpen = !isOpen;
        FirstPlaced = false;

        if (isOpen)
        {
            OpenChest();
        }            
        else
        {
            CloseChest();
        }


        anim.SetBool("isOpen", isOpen);
        
    }
}


