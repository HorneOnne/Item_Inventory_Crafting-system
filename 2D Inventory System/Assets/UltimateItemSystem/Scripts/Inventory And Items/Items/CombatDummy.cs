using UnityEngine;


namespace UltimateItemSystem
{
    public class CombatDummy : Item, IDamageable, IPlaceable, IShowDamage
    {
        [Header("References")]
        private Animator anim;

        [Header("CombatDummy Properties")]
        private bool playerOnLeft;


        #region Properties
        [field: SerializeField]
        public bool ShowRay { get; set; }
        [field: SerializeField]
        public LayerMask PlacedLayer { get; set; }
        [field: SerializeField]
        public float Cooldown { get; set; }

        #endregion


        [Header("CombatDummy Properties")]
        private bool canTrigger = true;


        // Cached
        private GameObject textObject;
        private Vector3 moveTextObjectVector;
        private Vector3 textObjectRotation;





        protected override void Start()
        {
            base.Start();
            anim = base.Model.GetComponent<Animator>();
        }

        private bool IsLeftSideChecker(Transform object01, Transform object02)
        {
            float xDifference = object02.position.x - object01.position.x;
            bool returnBool;
            if (xDifference > 0)
            {
                returnBool = false;
            }
            else if (xDifference < 0)
            {
                returnBool = true;
            }
            else
            {
                returnBool = true;
            }

            return returnBool;
        }


        public override bool Use(Player player)
        {
            return true;
        }


        public bool IsAboveGround(Player player, bool showRay = false)
        {
            bool canBePlaced = false;
            RaycastHit2D hit = Physics2D.Raycast(UIItemInHand.Instance.uiSlotDisplay.transform.position, Vector2.down, 2.5f, PlacedLayer);

            if (showRay)
                Debug.DrawRay(UIItemInHand.Instance.uiSlotDisplay.transform.position, Vector2.down * 2.5f, Color.blue, 1);
            if (hit.collider != null)
            {
                canBePlaced = true;
            }

            return canBePlaced;

        }

        public void Placed(Vector3 placedPosition, Player player = null, Transform parent = null)
        {
            Vector3 cachedLocalScale = transform.localScale;
            base.spriteRenderer.enabled = true;

            if (parent != null)
                transform.parent = parent.transform;

            gameObject.SetActive(true);
            transform.position = placedPosition;
            transform.localScale = cachedLocalScale;
            transform.localRotation = Quaternion.Euler(0, 0, 0);

            if(player != null)
            {
                player.ItemInHand.RemoveItem();
                UIItemInHand.Instance.UpdateItemInHandUI();
            }       
        }

        public void ShowDamage(int damaged)
        {
            textObject = DamagePopupSpawner.Instance.Pool.Get();
            textObject.transform.position = transform.position;

            moveTextObjectVector = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f));


            if (moveTextObjectVector.x > 0)
                textObjectRotation = new Vector3(0, 0, Random.Range(-30f, 0));
            else
                textObjectRotation = new Vector3(0, 0, Random.Range(0f, 30f));

            textObject.GetComponent<DamagePopup>().SetUp(damaged, GetDamageColor(damaged), GetDamageSize(damaged), moveTextObjectVector, textObjectRotation);

        }


        private Color GetDamageColor(float damage)
        {
            switch (damage)
            {
                case float n when (n >= 0 && n < 25):
                    return Color.green;
                case float n when (n >= 25 && n < 50):
                    return Color.blue;
                case float n when (n >= 50 && n < 75):
                    return new Color(0.6f, 0.2f, 1f); // Purple
                case float n when (n >= 75 && n < 90):
                    return Color.red;
                case float n when (n >= 90):
                    return Color.yellow;
                default:
                    return Color.white;
            }
        }

        private float GetDamageSize(float damage)
        {
            switch (damage)
            {
                case float n when (n >= 0 && n < 25):
                    return 15;
                case float n when (n >= 25 && n < 50):
                    return 16;
                case float n when (n >= 50 && n < 75):
                    return 17; // Purple
                case float n when (n >= 75 && n < 90):
                    return 19;
                case float n when (n >= 90):
                    return 25;
                default:
                    return 15;
            }
        }


        public void TakeDamage(int damaged)
        {
            anim.SetBool("playerOnLeft", playerOnLeft);
            anim.SetTrigger("damage");

            ShowDamage(damaged);
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (canTrigger)
            {
                if (collision.collider.CompareTag("Item"))
                {
                    playerOnLeft = IsLeftSideChecker(this.transform, collision.transform);

                    if (collision.gameObject.GetComponent<ICanCauseDamage>() != null)
                    {
                        TakeDamage(collision.gameObject.GetComponent<ICanCauseDamage>().GetDamage());
                    }

                }

                canTrigger = false;
                Invoke("ResetTrigger", Cooldown);
            }
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (canTrigger)
            {
                if (collision.CompareTag("Item"))
                {
                    playerOnLeft = IsLeftSideChecker(this.transform, collision.transform);

                    if (collision.gameObject.GetComponent<ICanCauseDamage>() != null)
                    {
                        TakeDamage(collision.gameObject.GetComponent<ICanCauseDamage>().GetDamage());
                    }

                }


                canTrigger = false;
                Invoke("ResetTrigger", Cooldown);
            }
        }

        private void ResetTrigger()
        {
            canTrigger = true;
        }
    }
}

