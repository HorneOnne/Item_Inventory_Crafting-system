using UnityEngine;
using System.Collections;

namespace DIVH_InventorySystem
{
    public class BoomerangProjectile_001 : Projectile, ICanCauseDamage
    {
        private EdgeCollider2D edgeCollider2D;
        private BoomerangData boomerangData;


        private const float scopeRecall = 1.0f;
        private Vector3 rotateVector = new Vector3(0, 0, 360);

        // Cached
        private Player player;
        private bool isCollide = false;
        private float timeRelease;
        private float timeToReturn;
        private float rotateSpeed;
        private float currentBoomerangSpeed;
        private Vector2 mousePosition;
        private Vector2 direction;
        private Vector2 currentVelocity;


        protected override void Start()
        {
            base.Start();
            edgeCollider2D = GetComponent<EdgeCollider2D>();

            edgeCollider2D.isTrigger = true;
        }


        public bool Throw(Player player, BoomerangData data)
        {
            this.boomerangData = data;
            timeToReturn = this.boomerangData.timeToReturn;
            currentBoomerangSpeed = this.boomerangData.releaseSpeed;
            rotateSpeed = this.boomerangData.rotateSpeed;


            // Rereference if null
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (edgeCollider2D == null)
                edgeCollider2D = GetComponent<EdgeCollider2D>();



            this.player = player;
            transform.position = player.transform.position;
            gameObject.SetActive(true);
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = mousePosition - (Vector2)transform.position;
            rb.velocity = direction.normalized * boomerangData.releaseSpeed;

            return true;
        }


   
        private void Update()
        {
            if (player == null) return;


            timeRelease += Time.deltaTime;
            if (timeRelease > timeToReturn)
            {
                BoomerangReturnToPlayer();
                if (Vector2.Distance(player.transform.position, transform.position) < scopeRecall)
                {
                    EventManager.TriggerBoomerangReturnedEvent();
                    ReturnToPool();      
                }

            }

            if (isCollide && timeRelease > 0.1f)
            {
                BoomerangReturnToPlayer();
                if (Vector2.Distance(player.transform.position, transform.position) < scopeRecall)
                {
                    EventManager.TriggerBoomerangReturnedEvent();
                    ReturnToPool();             
                }
            }


            RotateBoomerang();
        }


 
        private void FixedUpdate()
        {
            // Clamp velocity not larger boomerang release speed.
            currentVelocity = rb.velocity;
            currentBoomerangSpeed = currentVelocity.magnitude;
            if (currentBoomerangSpeed > boomerangData.releaseSpeed)
            {
                currentVelocity = currentVelocity.normalized * boomerangData.releaseSpeed;
                rb.velocity = currentVelocity;
            }
        }


        private void RotateBoomerang()
        {
            this.transform.Rotate(rotateVector * Time.deltaTime * rotateSpeed);
        }


        private void BoomerangReturnToPlayer()
        {
            direction = player.transform.position - transform.position;
            rb.velocity = direction.normalized * boomerangData.releaseSpeed * 2f;
        }


        private void ReturnToPool()
        {
            ResetBoomerangState();
            BoomerangProjectileSpawner.Instance.Pool.Release(this.gameObject);
        }

        private void ResetBoomerangState()
        {
            isCollide = false;
            timeRelease = 0f;
            gameObject.SetActive(false);
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Do nothing if collider with player tag.
            if (collision.CompareTag("Player") == true) return;
            if (collision.GetComponent<ICanBeAttacked>() == null) return;

            isCollide = true;
        }

        public int GetDamage()
        {
            return boomerangData.damage;
        }
    }
}

