using System.Collections;
using UnityEngine;


namespace UltimateItemSystem
{
    /// <summary>
    /// Represents an arrow projectile that can cause damage on collision.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class ArrowProjectile_001 : Projectile, ICanCauseDamage
    {
        [Header("REFERENCES")]
        [SerializeField] private GameObject explosionObjectPrefab;
        [SerializeField] private Transform explosionSpawnPosition;


        // Cached variables
        private BoxCollider2D arrowCollider;
        private bool hasReturnedToPool;
        private float timeElapsedSinceShot = 0.0f;
        private const float TIME_TO_RETURN = 5.0f;
        private const float TIME_TO_RETURN_WHEN_COLLIDE = 3.0f;
        private WaitForSeconds waitForReturnOnCollision;
        private BowData currentBowData;
        private ArrowData currentArrowData;



        protected override void Start()
        {
            base.Start();
            arrowCollider = GetComponent<BoxCollider2D>();
            waitForReturnOnCollision = new WaitForSeconds(TIME_TO_RETURN_WHEN_COLLIDE);
        }


        /// <summary>
        /// Shoots the arrow with given bow and arrow data.
        /// </summary>
        /// <param name="bowData">The bow data to use for the shot.</param>
        /// <param name="arrowData">The arrow data to use for the shot.</param>
        public void Shoot(BowData bowData, ArrowData arrowData)
        {
            hasReturnedToPool = false;
            timeElapsedSinceShot = 0.0f;


            this.currentBowData = bowData;
            this.currentArrowData = arrowData;
            SetDust(arrowData.particle);


            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            rb.velocity = Quaternion.Euler(0, 0, 0) * transform.right * bowData.releaseSpeed;
        }


        private void Update()
        {
            timeElapsedSinceShot += Time.deltaTime;
            if (timeElapsedSinceShot > TIME_TO_RETURN)
            {
                ReturnToPool();
            }

            Vector2 direction = rb.velocity;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, -45) * Quaternion.AngleAxis(angle, Vector3.forward);
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            ArrowPropertiesWhenCollide();
            StartCoroutine(PerformReturnToPool());

            var explosionObject = Instantiate(explosionObjectPrefab, explosionSpawnPosition.position, Quaternion.identity);
            Destroy(explosionObject, 0.5f);
        }


        /// <summary>
        /// Performs the return of the arrow projectile to the pool after a delay.
        /// </summary>
        IEnumerator PerformReturnToPool()
        {
            yield return waitForReturnOnCollision;
            ReturnToPool();
        }


        /// <summary>
        /// Returns the arrow projectile to the pool.
        /// </summary>
        private void ReturnToPool()
        {
            if (hasReturnedToPool == true) return;

            ResetArrowProperties();
            ArrowProjectileSpawner.Instance.Pool.Release(this.gameObject);
            hasReturnedToPool = true;
        }

        /// <summary>
        /// Resets the arrow properties to default.
        /// </summary>
        private void ResetArrowProperties()
        {
            rb.isKinematic = false;
            spriteRenderer.enabled = true;
            rb.velocity = Vector2.zero;
            arrowCollider.enabled = true;
        }


        /// <summary>
        /// Sets the arrow properties when it collides with something.
        /// </summary>
        private void ArrowPropertiesWhenCollide()
        {
            rb.isKinematic = true;
            spriteRenderer.enabled = false;
            rb.velocity = Vector2.zero;
            arrowCollider.enabled = false;
        }


        public int GetDamage()
        {
            return currentArrowData.damage + currentBowData.baseAttackDamage;
        }
    }
}
