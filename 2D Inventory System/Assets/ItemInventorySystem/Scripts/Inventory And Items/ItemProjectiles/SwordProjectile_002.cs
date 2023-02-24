using UnityEngine;

namespace DIVH_InventorySystem
{
    public class SwordProjectile_002 : Projectile, ICanCauseDamage
    {
        private SwordData swordData;


        private EdgeCollider2D projectileEdgeCollider;
        private int cachedPlayerFacingDirection;

        Vector2 mousePosition;
        Vector2 direction;

        protected override void Start()
        {
            base.Start();
            projectileEdgeCollider = GetComponent<EdgeCollider2D>();               
        }


        public void Shoot(Vector2 shootPosition)
        {
            this.transform.position = shootPosition;
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = mousePosition - (Vector2)transform.position;

            Invoke("ReturnToPool", 3);
        }

        void Update()
        {           
            rb.velocity = direction.normalized * 30;
        }


        private void ResetProperties()
        {
            rb.velocity = Vector2.zero;
        }

        private void ReturnToPool()
        {
            SworldProjectile002Spawner.Instance.Pool.Release(this.gameObject);
        }

        
       

        public int GetDamage()
        {
            return ((SwordData)ItemData).damage;
        }
    }
}