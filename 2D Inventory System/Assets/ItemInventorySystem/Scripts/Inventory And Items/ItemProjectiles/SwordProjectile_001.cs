using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// A class that represents a sword projectile that can cause damage to an enemy.
    /// </summary>
    public class SwordProjectile_001 : Projectile, ICanCauseDamage
    {
        private SwordData swordData;
        private Vector3 startRotationAngle;
        private float swingDuration;
        const float maxSwingRotation = 177.0f; // degrees
        private float swingTimer = 0.0f;


        private EdgeCollider2D projectileEdgeCollider;
        private int cachedPlayerFacingDirection;


        protected override void Start()
        {
            base.Start();
            projectileEdgeCollider = GetComponent<EdgeCollider2D>();

            this.swordData = (SwordData)ItemData;

            swingDuration = swordData.duration;

            projectileEdgeCollider.offset = Model.transform.localPosition;
            transform.localScale += new Vector3(swordData.swingSwordIncreaseSize, swordData.swingSwordIncreaseSize, 1);

            SetSwingDirection();
        }




        /// <summary>
        /// Update method called once per frame to rotate the sword while the timer is less than the rotation time.
        /// </summary>
        void Update()
        {
            // increment timer
            swingTimer += Time.deltaTime;
            // rotate the object
            if (swingTimer < swingDuration)
                Swing();
            else
                Destroy(gameObject);
        }


        /// <summary>
        /// Sets the direction of the sword swing based on the direction the player is facing.
        /// </summary>
        private void SetSwingDirection()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePosition.x - transform.position.x > 0)
                cachedPlayerFacingDirection = 1;
            else
                cachedPlayerFacingDirection = -1;


            if (cachedPlayerFacingDirection == 1)
            {
                startRotationAngle = new Vector3(0, 0, 90);
            }
            else
            {
                startRotationAngle = new Vector3(0, 0, 0);
            }

            transform.rotation = Quaternion.Euler(startRotationAngle);

        }

        /// <summary>
        /// Rotates the sword based on the direction the player is facing and the rotation time.
        /// </summary>
        private void Swing()
        {
            if (cachedPlayerFacingDirection == 1)
                transform.Rotate(-Vector3.forward * Time.deltaTime * maxSwingRotation / swingDuration);
            else
                transform.Rotate(Vector3.forward * Time.deltaTime * maxSwingRotation / swingDuration);
        }


        public int GetDamage()
        {
            return ((SwordData)ItemData).damage;
        }
    }
}

