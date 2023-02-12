using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private Player player;
    private PlayerData playerData;
    private PlayerInputHandler playerInputHandler;
    private Rigidbody2D rb;



    [Header("Raycast")]
    [SerializeField] Transform groundCheckPoint;


    #region Properties
    
    public sbyte FacingDirection { get; private set; }
    #endregion Properties   


    public Vector2 CurrentVelocity { get { return rb.velocity; } }



    private void Start()
    {
        player = GetComponent<Player>();
        playerInputHandler = player.PlayerInputHandler;
        playerData = player.playerData;

        rb = GetComponent<Rigidbody2D>();
        FacingDirection = 1;
    }



    private void FixedUpdate()
    {
        // Movement 
        // =========================================================================
        MoveOnGround();

        // Movement in AIR
        MoveInAir();

        // Jump 
        // =========================================================================
        Jump();


        // Pre-Calculate Physics
        AddGravityMultiplier();
        SetMaxVelocity();

        FlipCharacterFace(playerInputHandler.MovementInput);
    }



    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheckPoint.position, playerData.roundCheckRadius, playerData.groundLayer);
    }

 
    private void MoveOnGround()
    {
        if (playerInputHandler.MovementInput != 0)
        {
            rb.velocity = new Vector2(playerInputHandler.MovementInput * playerData.movementSpeed * Time.deltaTime, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void MoveInAir()
    {
        if (IsGrounded() == false && playerInputHandler.MovementInput != 0)
        {
            if (playerInputHandler.MovementInput == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x * playerData.airDragMultiplier, rb.velocity.y);
            }
            else
            {
                Vector2 forceToAdd = new Vector2(playerData.movementForceInAir * playerInputHandler.MovementInput, rb.velocity.y);
                rb.velocity = forceToAdd;

                if (Mathf.Abs(rb.velocity.x) > playerData.maxMovementSpeed)
                {
                    rb.velocity = new Vector2(playerData.maxMovementSpeed * playerInputHandler.MovementInput, rb.velocity.y);
                }
            }
        }


    }

    private void Jump()
    {
        if (playerInputHandler.TriggerJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, playerData.jumpForce * Time.fixedDeltaTime);
            playerInputHandler.ResetJumpInput();
        }
    }



    private void SetMaxVelocity()
    {
        if (rb.velocity.y > playerData.maxJumpVelocity)
            rb.velocity = new Vector2(rb.velocity.x, playerData.maxJumpVelocity);
        if (rb.velocity.y < playerData.maxFallVelocity)
            rb.velocity = new Vector2(rb.velocity.x, playerData.maxFallVelocity);
    }

    private void AddGravityMultiplier()
    {
        if (rb.velocity.y < 0)
            rb.velocity += Vector2.up * Physics2D.gravity * playerData.fallMultiplier * Time.deltaTime;
        else if (rb.velocity.y > 0 && !playerInputHandler.TriggerJump)
            rb.velocity += Vector2.up * Physics2D.gravity * playerData.lowMultiplier * Time.deltaTime;
    }

    public void FlipCharacterFace(float XInput)
    {
        if (XInput != 0 && XInput != FacingDirection)
        {
            Flip();
        }

    }

    private void Flip()
    {
        FacingDirection *= -1;
        //transform.Rotate(0f, 180f, 0f);
    }


    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Debug.Log("TriggerEnter with item");
            Item item = collision.gameObject.GetComponent<Item>();
            if (this.GetComponent<PlayerInventory>().AddItem(item.itemData))
            {
                Destroy(collision.gameObject);
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            Debug.Log("CollisionEnter with item");
            Item item = collision.gameObject.GetComponent<Item>();
            if (this.GetComponent<PlayerInventory>().AddItem(item.itemData))
            {
                Destroy(collision.gameObject);
            }

        }
    }*/

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheckPoint.position, playerData.roundCheckRadius);
    }*/
}
