using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("References")]
    private PlayerController player;
    private PlayerMovement playerMovement;

  
    private float movementInput;
    private float jumpBufferCount;
    private float hangCounter;

    #region Properties
    public float MovementInput { get { return movementInput; }}
    public bool TriggerJump { get; private set; }
    #endregion Properties



    private void Start()
    {
        player = GetComponent<PlayerController>();
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
    }

    public void ResetJumpInput() => TriggerJump = false;

    public float GetTimeLeftGround()
    {
        return Mathf.Abs(hangCounter - player.playerData.hangTime);
    }
}
