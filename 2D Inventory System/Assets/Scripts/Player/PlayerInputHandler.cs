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
    }


    private void Update()
    {
        movementInput = Input.GetAxisRaw("Horizontal");

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


        /*if(Input.GetKeyDown(KeyCode.G))
        {
            GameObject bowObject = ItemContainerManager.Instance.GetItemPrefab("Bow");
            bowObject.GetComponent<Bow>().SetData(itemInHand.GetItem());
            bowObject.GetComponent<Bow>().UpdateData();

            Instantiate(bowObject, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        }*/

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
    }


    private void SingleLeftClick()
    {
        if(itemInHand.HasItemData() && itemInHand.itemObject != null)
        {
            itemInHand.UseItem();
        }


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
        mousePosition.x - player.HandPart.position.x,
        mousePosition.y - player.HandPart.position.y
        );

        float offsetAngle = itemInHand.itemObject.OffsetZAngle;
        direction = Quaternion.Euler(0, 0, offsetAngle) * direction;
        player.HandPart.up = direction;
    }


    private void ReInstantiateItem()
    {
        if (player.HandPart.childCount != 0)
        {
            for(int i = 0; i < player.HandPart.childCount; i++)
            {
                Destroy(player.HandPart.GetChild(i).gameObject);           
            }
      
        }

        itemInHand.SetItemObject(null);
        if (itemInHand.HasItemData())
        {
            var itemPrefab = ItemContainerManager.Instance.GetItemPrefab(itemInHand.GetItem().itemType.ToString());

            if (itemPrefab != null)
            {
                var itemObject = Instantiate(ItemContainerManager.Instance.GetItemPrefab(itemInHand.GetItem().itemType.ToString()), player.HandPart.transform);
                itemObject.GetComponent<Item>().SetData(itemInHand.GetItem());
                itemInHand.SetItemObject(itemObject.GetComponent<Item>());
            }
        }
        
    }


    public void ResetJumpInput() => TriggerJump = false;

    public float GetTimeLeftGround()
    {
        return Mathf.Abs(hangCounter - player.playerData.hangTime);
    }
}
