using System.Collections;
using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// Represents a projectile fired by a magic staff that can cause damage to targets it collides with.
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class MagicStaffProjectile_001 : Projectile, ICanCauseDamage
    {
        /// <summary>
        /// The data associated with the magic staff used to fire this projectile.
        /// </summary>
        private MagicStaffData staffData;

        /// <summary>
        /// The position where this projectile was initially fired from.
        /// </summary>
        private Vector2 initialPosition;

        /// <summary>
        /// Indicates whether this projectile has already been returned to the object pool.
        /// </summary>
        private bool hasReturnedToPool;

        /// <summary>
        /// The amount of time elapsed since this projectile was fired and will be returned to the object pool if it has not collided with a target.
        /// </summary>
        private float elapsedTimeToReturn = 0.0f;

        /// <summary>
        /// The maximum amount of time this projectile can be in the game world before being returned to the object pool if it has not collided with a target.
        /// </summary>
        private const float TIME_TO_RETURN = 5.0f;

        /// <summary>
        /// The amount of time this projectile has to return to the object pool after colliding with a target.
        /// </summary>
        private const float TIME_TO_RETURN_WHEN_COLLIDE = 3.0f;
        private WaitForSeconds waitTimeToReturn;

        protected override void Start()
        {
            base.Start();

            // Get the MagicStaffData instance associated with the magic staff used to fire this projectile.
            this.staffData = (MagicStaffData)ItemData;

            // Record the position where this projectile was fired from.
            initialPosition = transform.position;

            // Create a particle effect at the starting position.
            SetDust(staffData.particle);

            // Calculate the direction of the projectile based on the mouse position.
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - initialPosition;

            // Set the velocity of the projectile based on the direction and release speed of the magic staff.
            rb.velocity = direction.normalized * staffData.releaseProjectileSpeed;

            // Set up the wait instruction for returning the projectile to the object pool.
            waitTimeToReturn = new WaitForSeconds(TIME_TO_RETURN_WHEN_COLLIDE);
        }


        private void Update()
        {
            // Check if the maximum time limit for this projectile has been reached.
            elapsedTimeToReturn += Time.deltaTime;
            if (elapsedTimeToReturn > TIME_TO_RETURN)
            {
                elapsedTimeToReturn = 0.0f;
                ReturnToPool();
            }
        }


        public int GetDamage()
        {
            return staffData.damage;
        }


        private IEnumerator PerformReturnToPool()
        {
            yield return waitTimeToReturn;
            ReturnToPool();
        }


        /// <summary>
        /// This method returns the projectile to the object pool for reuse. It checks if the projectile has already been returned to the pool before, if so, it immediately returns.
        /// </summary>
        private void ReturnToPool()
        {
            if (hasReturnedToPool == true) return;
            ArrowSpawner.Instance.Pool.Release(this.gameObject);
            hasReturnedToPool = true;
        }

    }
}