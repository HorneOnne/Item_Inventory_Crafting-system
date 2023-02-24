using System.Collections.Generic;
using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Handles player input and sends events to other components.
    /// </summary>
    [RequireComponent(typeof(Player))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("REFERENCES")]
        private Player player;
        private PlayerInventory playerInventory;
        private PlayerEquipment playerEquipment;
        private ItemInHand itemInHand;
        private PlayerMovement playerMovement;

        /// <summary>
        /// The value of the horizontal movement input.
        /// </summary>
        private float movementInput;

        /// <summary>
        /// The number of seconds left in the jump buffer.
        /// </summary>
        private float jumpBufferCount;

        /// <summary>
        /// The time left for the player to hang in the air after leaving the ground.
        /// </summary>
        private float hangCounter;

        #region Properties
        public float MovementInput { get { return movementInput; } }
        public bool TriggerJump { get; private set; }
        public bool PressUtilityKeyInput { get; private set; }
        public bool JumpInput { get; private set; }

        /// <summary>
        /// State of mouse pointer
        /// </summary>
        [field: SerializeField] public PointerState CurrentMouseState { get; private set; }
        #endregion Properties


        /// <summary>
        /// The time in seconds between clicks to register as a double click.
        /// </summary>
        float doubleClickTime = 0.2f;

        /// <summary>
        /// The time of the last click.
        /// </summary>
        float lastClickTime = 0;

        /// <summary>
        /// The time in seconds between presses of the right mouse button to register as a double press.
        /// </summary>
        float rightPressIntervalTime = 1.0f;

        /// <summary>
        /// The time left in the right mouse button double press interval.
        /// </summary>
        private float lastRightPressIntervalTimeCount = 0.0f;

        /// <summary>
        /// The time elapsed since the last mouse press.
        /// </summary>
        private float elapsedTime = 0.0f;


        /// <summary>
        /// Tracks whether the left mouse button is currently being clicked.
        /// </summary>
        bool isLeftClicking = false;

        /// <summary>
        /// Whether the current item being used is being used for the first time.
        /// </summary>
        private bool firstUseItem = true;   


        // Cached
        private Vector2 mousePosition;
        private Vector2 direction;
        private float offsetAngle;
        private int quickUseItemKeySize;

        /// <summary>
        /// The key used to activate the utility ability.
        /// </summary>
        [Header("KEY BINDING")]
        public KeyCode utilityKeyBinding = KeyCode.LeftShift;

        /// <summary>
        /// The key used to drop the current item.
        /// </summary>
        public KeyCode dropItemKey = KeyCode.T;


        /// <summary>
        /// List of keycodes that correspond to the quick use item slots.
        /// </summary>
        public List<KeyCode> quickUseItemKey = new List<KeyCode>() 
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };


        #region Events handler
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
        #endregion


        private void Start()
        {
            player = GetComponent<Player>();
            playerInventory = player.PlayerInventory;
            playerEquipment = player.PlayerEquipment;
            itemInHand = player.ItemInHand;
            playerMovement = player.PlayerMovement;
            quickUseItemKeySize = quickUseItemKey.Count;
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


            if (Input.GetKeyDown(dropItemKey))
            {
                if (itemInHand.GetICurrenttem() != null)
                {
                    mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    itemInHand.GetICurrenttem().Drop(player, mousePosition, Vector3.zero, true);
                    itemInHand.ClearSlot();
                }
            }


            if (itemInHand.HasItem() && itemInHand.GetICurrenttem().showIconWhenHoldByHand == true)
                RotateHoldItem();



            HandleMouseEvents();


            if (CurrentMouseState == PointerState.DoubleLeftClick)
            {
                StackItem();
            }
            else if (CurrentMouseState == PointerState.SingleLeftClick)
            {
                OpenCloseChest();
                //UseItem();
            }
            else if (CurrentMouseState == PointerState.LeftPress)
            {
                UseItem();
            }

            
            for(int i = 0; i < quickUseItemKeySize; i++)
            {
                if (Input.GetKeyDown(quickUseItemKey[i]))
                {
                    Debug.Log(i);
                }
            }
        }



        //// <summary>
        /// Handles mouse events and updates the current mouse state.
        /// </summary>
        private void HandleMouseEvents()
        {
            CurrentMouseState = PointerState.Null;

            if (Input.GetMouseButtonDown(0))
            {
                CurrentMouseState = PointerState.SingleLeftClick;

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
                }
            }
            else if (Input.GetMouseButtonUp(0) && isLeftClicking)
            {
                //Debug.Log("Single click detected");           
                isLeftClicking = false;
                CurrentMouseState = PointerState.Null;
            }
            else if (Input.GetMouseButton(0))
            {
                CurrentMouseState = PointerState.LeftPress;
            }
            else if (Input.GetMouseButton(1))
            {
                CurrentMouseState = PointerState.RightPress;
                if (Time.time - lastRightPressIntervalTimeCount >= rightPressIntervalTime)
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



        /// <summary>
        /// Stack items if double left clicked
        /// </summary>
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



        /// <summary>
        /// Rotate hold item if it can be shown
        /// </summary>
        private void RotateHoldItem()
        {
            Utilities.RotateObjectTowardMouse2D(player.HandHoldItem, -45);
        }


 
        /// <summary>
        /// Handles the re-instantiation of the item in the player's hand. Destroys any existing items in the player's hand and
        /// creates a new item object in the hand if the item inventory slot is not empty.
        /// </summary>
        private void ReInstantiateItem()
        {
            firstUseItem = true;

            if (player.HandHoldItem.childCount != 0)
            {
                for (int i = 0; i < player.HandHoldItem.childCount; i++)
                {
                    Destroy(player.HandHoldItem.GetChild(i).gameObject);
                }
            }
            itemInHand.SetICurrentItem(null);


            if (itemInHand.HasItemData())
            {
                var itemObject = Utilities.InstantiateItemObject(itemInHand.GetSlot(), player.HandHoldItem);
                itemInHand.SetICurrentItem(itemObject.GetComponent<Item>());
                if (itemObject.showIconWhenHoldByHand)
                {
                    itemObject.spriteRenderer.enabled = true;
                }
                else
                    itemObject.spriteRenderer.enabled = false;

            }
        }


        /// <summary>
        /// Handles fast equipment of items with a utility key press.
        /// </summary>
        private void FastEquipItem()
        {
            if (PressUtilityKeyInput)
            {
                if (itemInHand.HasItemData() == false) return;
                if (itemInHand.ItemGetFrom.slotStoredType == StoredType.PlayerInventory)
                {
                    ItemSlot equipItemSlot = itemInHand.GetSlot();
                    ItemSlot currentEquipmentSlot = null;
                    bool canEquip;

                    switch (itemInHand.GetItemData().itemType)
                    {
                        case ItemType.Helm:
                            if (playerEquipment.Helm.HasItem() == true)
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
                        EventManager.TriggerPlayerEquipmentChangedEvent();

                    }
                }
            }
        }


        /// <summary>
        /// Use item if left mouse button is pressed
        /// </summary>
        private void UseItem()
        {

            if (itemInHand.HasItemData() && itemInHand.GetICurrenttem() != null)
            {
                // Check if it's time to attack
                if (elapsedTime >= itemInHand.GetItemData().duration)
                {
                    elapsedTime = 0;
                    itemInHand.UseItem();
                }
                else if (firstUseItem)
                {
                    bool canUseItem = itemInHand.UseItem();
                    if (canUseItem)
                    {
                        firstUseItem = false;
                        elapsedTime = 0;
                    }
                }
            }
        }



        /// <summary>
        /// Open or close chest if single left clicked
        /// </summary>
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
}
