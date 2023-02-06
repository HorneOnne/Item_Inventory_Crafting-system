using System;
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


        LeftClickHandler(SingleLeftClick, DoubleLeftClick);



        if(itemInHand.HasItemData() && itemInHand.GetItem().itemType == ItemType.Bow)
            RotateHoldItem();
    }



    private void LeftClickHandler(Action singleClick, Action doubleClick = null)
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            // check if last click was within doubleClickTime
            if (Time.time - lastClickTime < doubleClickTime)
            {
                //Debug.Log("Double click detected");                
                click = false;

                doubleClick();
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

            singleClick();
            
        }
        else if(Input.GetMouseButton(0))
        {
            singleClick();
        }
    }


    private void SingleLeftClick()
    {
        UseItem();
    }

    private void DoubleLeftClick()
    {
        switch (itemInHand.ItemGetFrom)
        {
            case StoredType.PlayerInventory:
                playerInventory.StackItem();
                break;
            case StoredType.CraftingTable:
                CraftingTableManager.Instance.StackItem();
                break;

            default: break;

        }
    }


    private void RotateHoldItem()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = new Vector2(
        mousePosition.x - player.HandHoldItem.position.x,
        mousePosition.y - player.HandHoldItem.position.y
        );

        float offsetAngle = itemInHand.GetItemObject().OffsetZAngle;
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
            var itemPrefab = ItemContainerManager.Instance.GetItemPrefab(itemInHand.GetItem().itemType.ToString());

            if (itemPrefab != null)
            {
                var itemObject = Instantiate(ItemContainerManager.Instance.GetItemPrefab(itemInHand.GetItem().itemType.ToString()), player.HandHoldItem.transform);
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
            if (elapsedTime >= itemInHand.GetItem().duration)
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

 

    public void ResetJumpInput() => TriggerJump = false;

    public float GetTimeLeftGround()
    {
        return Mathf.Abs(hangCounter - player.playerData.hangTime);
    }
}
