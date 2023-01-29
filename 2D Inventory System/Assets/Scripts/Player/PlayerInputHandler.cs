using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("References")]
    private Player player;
    private PlayerInventory playerInventory;

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



    private void Start()
    {
        player = GetComponent<Player>();
        playerInventory = player.PlayerInventory;

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



        if (CheckForDoubleLeftClick())
            //playerInventory.StackItem();
            CraftingTableManager.Instance.StackItem();
    }

    
    private bool CheckForDoubleLeftClick()
    {
        bool detectDoubleClick = false;

        if (Input.GetMouseButtonDown(0))
        {
            // check if last click was within doubleClickTime
            if (Time.time - lastClickTime < doubleClickTime)
            {
                // double click detected
                //Debug.Log("Double click detected");
                detectDoubleClick = true;
                click = false;
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
            // single click detected
            //Debug.Log("Single click detected");
            click = false;
        }

        return detectDoubleClick;
    }

    public void ResetJumpInput() => TriggerJump = false;

    public float GetTimeLeftGround()
    {
        return Mathf.Abs(hangCounter - player.playerData.hangTime);
    }
}
