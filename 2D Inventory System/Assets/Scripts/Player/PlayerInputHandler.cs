using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("References")]
    private Player player;
    private PlayerInventory playerInventory;
    private ItemInHand itemInHand;
    private PlayerMovement playerMovement;
    private PlayerData playerData;

  
    private float movementInput;
    private float jumpBufferCount;
    private float hangCounter;

    #region Properties
    public float MovementInput { get { return movementInput; }}
    public bool TriggerJump { get; private set; }
    #endregion Properties



    float doubleClickTime = 0.2f; // time in seconds between clicks to register as a double click
    float lastClickTime = 0; // time of last click
    bool click = false; // variable to track if a click has occurred


    private float elapsedTime = 0.0f;
    private bool firstUseItem = true;   // Use item immediately if it is the first time use, not wait for elapsedTime


    // Cached
    private Vector2 mousePosition;
    private Vector2 direction;
    private float offsetAngle;

    private void OnEnable()
    {
        ItemInHand.OnItemInHandChanged += ReInstantiateItem;
    }

    private void OnDisable()
    {
        ItemInHand.OnItemInHandChanged -= ReInstantiateItem;
    }



    private void Start()
    {
        player = GetComponent<Player>();
        playerInventory = player.PlayerInventory;
        itemInHand = player.ItemInHand;
        playerMovement = player.PlayerMovement;
        playerData = player.playerData;
    }


    private void Update()
    {       
        movementInput = Input.GetAxisRaw("Horizontal");
        elapsedTime += Time.deltaTime;


        // Calculate hang time (Time leave ground)
        if (playerMovement.IsGrounded())
            hangCounter = player.playerData.hangTime;
        else
            hangCounter -= Time.deltaTime;


        // calculate Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCount = player.playerData.jumpBufferLength;
        }         
        else
        {
            jumpBufferCount -= Time.deltaTime;
        }
            
        if (jumpBufferCount > 0 && hangCounter > 0)
        {
            TriggerJump = true;
            jumpBufferCount = 0;
        }


        if(Input.GetKeyDown(KeyCode.T))
        {
            if (itemInHand.GetItemObject() != null)
            {
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                itemInHand.GetItemObject().Drop(player, mousePosition, Vector3.zero);
            }
        }


        LeftClickHandler();

        if(itemInHand.HasItemObject() && itemInHand.GetItemObject().canShow == true)
            RotateHoldItem();
    }



    private void LeftClickHandler()
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            // check if last click was within doubleClickTime
            if (Time.time - lastClickTime < doubleClickTime)
            {
                //Debug.Log("Double click detected");                
                click = false;

                DoubleLeftClick();
            }
            else
            {
                // first click
                lastClickTime = Time.time;
                click = true;
            }
        }
        else if (Input.GetMouseButtonUp(0) && click)
        {
            //Debug.Log("Single click detected");           
            click = false;

            SingleLeftClick();
        }
        else if(Input.GetMouseButton(0))
        {
            UseItem();
            //SingleLeftClick();
        }
    }


    private void SingleLeftClick()
    {
        OpenCloseChest();
        UseItem();
    }

    private void DoubleLeftClick()
    {
        switch (itemInHand.ItemGetFrom)
        {
            case StoredType.PlayerInventory:
                playerInventory.StackItem();
                break;
            case StoredType.ChestInventory:
                Debug.Log("Chest Stack Item");
                player.currentOpenChest.StackItem();
                break;
            case StoredType.CraftingTable:
                CraftingTableManager.Instance.StackItem();
                break;

            default: break;

        }
    }


    private void RotateHoldItem()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = new Vector2(
        mousePosition.x - player.HandHoldItem.position.x,
        mousePosition.y - player.HandHoldItem.position.y
        );

        offsetAngle = itemInHand.GetItemObject().OffsetZAngle;
        direction = Quaternion.Euler(0, 0, offsetAngle) * direction;
        player.HandHoldItem.up = direction;
    }


    private void ReInstantiateItem()
    {
        firstUseItem = true;

        if (player.HandHoldItem.childCount != 0)
        {
            for(int i = 0; i < player.HandHoldItem.childCount; i++)
            {
                Destroy(player.HandHoldItem.GetChild(i).gameObject);           
            }   
        }


        itemInHand.SetItemObject(null);
        if (itemInHand.HasItemData())
        {
            var itemPrefab = ItemContainerManager.Instance.GetItemPrefab(itemInHand.GetItemData().itemType.ToString());

            if (itemPrefab != null)
            {
                var itemObject = Instantiate(ItemContainerManager.Instance.GetItemPrefab(itemInHand.GetItemData().itemType.ToString()), player.HandHoldItem.transform);
                itemObject.GetComponent<Item>().SetData(itemInHand.GetSlot());
                itemInHand.SetItemObject(itemObject.GetComponent<Item>());

                if (itemObject.GetComponent<Item>().canShow == false)
                {
                    itemObject.SetActive(false);
                }
            }
        }
        
    }


    private void UseItem()
    {
       
        if (itemInHand.HasItemData() && itemInHand.GetItemObject() != null)
        {
            // Check if it's time to attack
            if (elapsedTime >= itemInHand.GetItemData().duration)
            {               
                elapsedTime = 0;
                itemInHand.UseItem();
            }        
            else if(firstUseItem)
            {             
                bool canUseItem = itemInHand.UseItem();
                if(canUseItem)
                {
                    firstUseItem = false;
                    elapsedTime = 0;
                }              
            }
        }
    }

 
    private void OpenCloseChest()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            var chestObject = hit.collider.GetComponent<Chest>();
            if (chestObject != null)
            {
                Debug.Log("This is chest");
                if(chestObject.FirstPlaced == false)
                {
                    chestObject.SetPlayerOpenChest(this.player);
                    player.currentOpenChest = chestObject.Inventory;
                }
                    
            }
        }
    }


    public void ResetJumpInput() => TriggerJump = false;

    public float GetTimeLeftGround()
    {
        return Mathf.Abs(hangCounter - player.playerData.hangTime);
    }
}
