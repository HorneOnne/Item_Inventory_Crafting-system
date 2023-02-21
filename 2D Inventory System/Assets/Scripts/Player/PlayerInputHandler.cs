using MyGame.Ultilities;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("References")]
    private Player player;
    private PlayerInventory playerInventory;
    private PlayerEquipment playerEquipment;
    private ItemInHand itemInHand;
    private PlayerMovement playerMovement;
    private PlayerData playerData;

  
    private float movementInput;
    private float jumpBufferCount;
    private float hangCounter;

    #region Properties
    public float MovementInput { get { return movementInput; }}
    public bool TriggerJump { get; private set; }
    public bool PressUtilityKeyInput { get; private set; }
    public bool JumpInput { get; private set; }
    [field: SerializeField] public PointerState CurrentMouseState{get; private set;}
    #endregion Properties



    float doubleClickTime = 0.2f; // time in seconds between clicks to register as a double click
    float lastClickTime = 0; // time of last click
    float rightPressIntervalTime = 1.0f;
    private float lastRightPressIntervalTimeCount = 0.0f;
    private float elapsedTime = 0.0f;

    bool isLeftClicking = false; // variable to track if a click has occurred
    private bool firstUseItem = true;   // Use item immediately if it is the first time use, not wait for elapsedTime


    // Cached
    private Vector2 mousePosition;
    private Vector2 direction;
    private float offsetAngle;


    [Header("Key binding")]
    public KeyCode utilityKeyBinding = KeyCode.LeftShift;
    public KeyCode dropItemKey = KeyCode.T;


    private void OnEnable()
    {
        EventManager.OnItemInHandChanged += ReInstantiateItem;
        EventManager.OnItemInHandChanged += FastEquipItem;
    }

    private void OnDisable()
    {
        EventManager.OnItemInHandChanged -= ReInstantiateItem;
        EventManager.OnItemInHandChanged -= FastEquipItem;
    }



    private void Start()
    {
        player = GetComponent<Player>();
        playerInventory = player.PlayerInventory;
        playerEquipment = player.PlayerEquipment;
        itemInHand = player.ItemInHand;
        playerMovement = player.PlayerMovement;
        playerData = player.playerData;
    }


    private void Update()
    {       
        movementInput = Input.GetAxisRaw("Horizontal");
        PressUtilityKeyInput = Input.GetKey(utilityKeyBinding);
        JumpInput = Input.GetButtonDown("Jump");

        elapsedTime += Time.deltaTime;
        

        // Calculate hang time (Time leave ground)
        if (playerMovement.IsGrounded())
            hangCounter = player.playerData.hangTime;
        else
            hangCounter -= Time.deltaTime;


        // calculate Jump Buffer
        if (JumpInput)
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


        if(Input.GetKeyDown(dropItemKey))
        {
            if (itemInHand.GetItemObject() != null)
            {
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                itemInHand.GetItemObject().Drop(player, mousePosition, Vector3.zero, true);
                itemInHand.ClearSlot();
            }
        }


        if (itemInHand.HasItemObject() && itemInHand.GetItemObject().canShow == true)
            RotateHoldItem();



        HandleMouseEvents();


        if(CurrentMouseState == PointerState.DoubleLeftClick)
        {
            StackItem();
        }
        else if(CurrentMouseState == PointerState.SingleLeftClick)
        {
            OpenCloseChest();
            UseItem();
        }
        else if(CurrentMouseState == PointerState.LeftPress)
        {
            UseItem();
        }
        
    }



    private void HandleMouseEvents()
    {  
        if (Input.GetMouseButtonDown(0))
        {
            // check if last click was within doubleClickTime
            if (Time.time - lastClickTime < doubleClickTime)
            {
                //Debug.Log("Double click detected");                
                isLeftClicking = false;
                CurrentMouseState = PointerState.DoubleLeftClick;
            }
            else
            {
                // first click
                lastClickTime = Time.time;
                isLeftClicking = true;
                CurrentMouseState = PointerState.SingleLeftClick;
            }
        }
        else if (Input.GetMouseButtonUp(0) && isLeftClicking)
        {
            //Debug.Log("Single click detected");           
            isLeftClicking = false;
            CurrentMouseState = PointerState.Null;
        }
        else if(Input.GetMouseButton(0))
        {
            CurrentMouseState = PointerState.LeftPress;
        }
        else if (Input.GetMouseButton(1))
        {
            CurrentMouseState = PointerState.RightPress;
            if(Time.time - lastRightPressIntervalTimeCount >= rightPressIntervalTime)
            {
                CurrentMouseState = PointerState.RightPressAfterWait;            
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            CurrentMouseState = PointerState.Null;
            lastRightPressIntervalTimeCount = Time.time;
        }
    }



    private void StackItem()
    {
        switch (itemInHand.ItemGetFrom.slotStoredType)
        {
            case StoredType.PlayerInventory:
                playerInventory.StackItem();
                break;
            case StoredType.ChestInventory:
                player.currentOpenChest.Inventory.StackItem();
                break;
            case StoredType.CraftingTable:
                CraftingTable.Instance.StackItem();
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
            var itemObject = Utilities.InstantiateItemObject(itemInHand.GetSlot(), player.HandHoldItem);
            itemInHand.SetItemObject(itemObject.GetComponent<Item>());
            if (itemObject.canShow == false)
            {
                itemObject.gameObject.SetActive(false);
            }
        }       
    }


    private void FastEquipItem()
    {
        if(PressUtilityKeyInput)
        {
            if (itemInHand.HasItemData() == false) return;
            if (itemInHand.ItemGetFrom.slotStoredType == StoredType.PlayerInventory)
            {
                ItemSlot equipItemSlot = itemInHand.GetSlot();
                ItemSlot currentEquipmentSlot = null;
                bool canEquip;

                switch(itemInHand.GetItemData().itemType)
                {
                    case ItemType.Helm:
                        if(playerEquipment.Helm.HasItem() == true)
                            currentEquipmentSlot = new ItemSlot(playerEquipment.Helm);
                        canEquip = playerEquipment.Equip(ItemType.Helm, equipItemSlot);
                        break;
                    case ItemType.ChestArmor:
                        if (playerEquipment.Chest.HasItem() == true)
                            currentEquipmentSlot = new ItemSlot(playerEquipment.Chest);
                        canEquip = playerEquipment.Equip(ItemType.ChestArmor, equipItemSlot);
                        break;
                    case ItemType.Shield:
                        if (playerEquipment.Shield.HasItem() == true)
                            currentEquipmentSlot = new ItemSlot(playerEquipment.Shield);
                        canEquip = playerEquipment.Equip(ItemType.Shield, equipItemSlot);
                        break;
                    default:
                        canEquip = false;
                        break;
                }

                if (canEquip)
                {
                    if (currentEquipmentSlot != null)
                    {
                        playerInventory.AddNewItemAt(itemInHand.ItemGetFrom.slotIndex, currentEquipmentSlot.ItemData);          
                    }
                    itemInHand.RemoveItem();
                    UIPlayerEquipment.Instance.UpdateEquipmentUI();
                    EventManager.PlayerEquipmentChanged();

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
            var chest = hit.collider.GetComponent<Chest>();
            if (chest == null) return;

            if (player.currentOpenChest != null && chest.ChestState != Chest.ChestStateEnum.Placed)
            {
                if (player.currentOpenChest != chest)
                {
                    player.currentOpenChest.Close(player);
                    chest.Open(player);
                    return;
                }
            }
            chest.Toggle(player);
        }
    }




    public void ResetJumpInput() => TriggerJump = false;

    public float GetTimeLeftGround()
    {
        return Mathf.Abs(hangCounter - player.playerData.hangTime);
    }
}

public enum PointerState
{
    SingleLeftClick,
    SingleRightClick,
    LeftPress,
    RightPress,
    RightPressAfterWait,
    DoubleLeftClick,
    Null
}