using UnityEngine;


[CreateAssetMenu(fileName = "new playerData", menuName = "ScriptableObject/Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Ground Layer  Properties")]
    public LayerMask groundLayer;
    public float roundCheckRadius;

    [Header("Movement Properties")]
    public float movementSpeed;
    public float movementForceInAir;
    public float airDragMultiplier;

    [Header("Jump Properties")]
    public float jumpForce;

    [Header("Better platform experience properties")]
    public float hangTime;
    public float jumpBufferLength = 0.1f;

    [Header("Fall Properties")]
    public float fallMultiplier;
    public float lowMultiplier;

    [Header("Velocity Limit Properties")]
    public float maxMovementSpeed;
    public float maxJumpVelocity;
    public float maxFallVelocity;

    [Space(20)]
    [Header("Stats Properties")]
    public int currentHealth;
    public int maxHealth;
    public int attackDamage;
    public int attackSpeed;
    public int armor;
}