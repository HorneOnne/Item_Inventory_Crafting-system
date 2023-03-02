using UnityEngine;

namespace UltimateItemSystem
{
    [CreateAssetMenu(fileName = "new playerData", menuName = "UltimateItemSystem/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [Header("Ground Layer  Properties")]
        public LayerMask groundLayer;
        public float roundCheckRadius;

        [Header("Movement Properties")]
        public float movementSpeed;
        public float movementForceInAir;
        public float airDragMultiplier;

        /// <summary>
        /// The force applied when jumping.
        /// </summary>
        [Header("Jump Properties")]    
        public float jumpForce;

        /// <summary>
        /// The time in seconds the player can hang on a platform edge.
        /// </summary>
        [Header("Better platform experience properties")]    
        public float hangTime;

        /// <summary>
        /// The length of the jump buffer in seconds.
        /// </summary>
        public float jumpBufferLength = 0.1f;


        /// <summary>
        /// The multiplier applied to the player's falling velocity.
        /// </summary>
        [Header("Fall Properties")]
        public float fallMultiplier;

        /// <summary>
        /// The low multiplier applied to the player's falling velocity when pressing the jump button.
        /// </summary>
        public float lowMultiplier;

        [Header("Velocity Limit Properties")]
        public float maxMovementSpeed;
        public float maxJumpVelocity;
        public float maxFallVelocity;
    }
}