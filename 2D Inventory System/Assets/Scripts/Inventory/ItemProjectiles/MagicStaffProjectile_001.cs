using System.Collections;
using UnityEngine;

namespace DIVH_InventorySystem
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class MagicStaffProjectile_001 : Projectile, ICanCauseDamage
    {
        private MagicStaffData magicStaffData;
        private Vector2 startPosition;

        private bool wasReturnToPool;
        private float timeToReturnElapse = 0.0f;
        private const float TIME_TO_RETURN = 5.0f;
        private const float TIME_TO_RETURN_WHEN_COLLIDE = 3.0f;
        private WaitForSeconds waitForReturnToPool;

        protected override void Start()
        {
            base.Start();
            this.magicStaffData = (MagicStaffData)ItemData;
            startPosition = transform.position;
            SetDust(magicStaffData.particle);


            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - startPosition;
            rb.velocity = direction.normalized * magicStaffData.releaseProjectileSpeed;

            waitForReturnToPool = new WaitForSeconds(TIME_TO_RETURN_WHEN_COLLIDE);
            //Destroy(this.gameObject, 10f);
        }


        private void Update()
        {
            timeToReturnElapse += Time.deltaTime;
            if (timeToReturnElapse > TIME_TO_RETURN)
            {
                timeToReturnElapse = 0.0f;
                ReturnToPool();
            }
        }


        public int GetDamage()
        {
            return magicStaffData.damage;
        }


        private IEnumerator PerformReturnToPool()
        {
            yield return waitForReturnToPool;
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (wasReturnToPool == true) return;

            //ResetArrowProperties();
            ArrowSpawner.Instance.Pool.Release(this.gameObject);
            wasReturnToPool = true;
        }

    }
}