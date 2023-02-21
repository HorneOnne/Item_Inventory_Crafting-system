using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// This class used for clone item object and use it as projectile.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Projectile : MonoBehaviour
    {
        #region Properties
        public ItemData ItemData { get; private set; }

        [field: SerializeField]
        public Vector2 OffsetPosition { get; private set; }
        [field: SerializeField]
        public float OffsetZAngle { get; private set; }
        protected GameObject Model { get; private set; }

        public bool useGravity;
        #endregion


        [Header("References")]
        protected ParticleControl particleControl;
        protected SpriteRenderer spriteRenderer;
        protected Rigidbody2D rb;
        protected ItemDataManager itemDataManager;



        private void Awake()
        {
            LoadComponents();
        }

        protected virtual void Start()
        {

        }


        private void LoadComponents()
        {
            Model = GetComponentInChildren<SpriteRenderer>().gameObject;
            spriteRenderer = Model.GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            particleControl = GetComponentInChildren<ParticleControl>();
            itemDataManager = ItemDataManager.Instance;
        }


        public void SetData(ItemData itemData)
        {
            this.ItemData = itemData;
            SetSprite(itemData.icon);
        }

        public void SetData(ItemData itemData, Sprite sprite)
        {
            this.ItemData = itemData;
            SetSprite(sprite);
        }

        public void SetData(ItemData itemData, Sprite sprite, bool useGravity)
        {
            this.ItemData = itemData;
            SetSprite(sprite);
            UseGravity(useGravity);

        }

        public virtual void SetDust(int dustIndex)
        {
            if (particleControl == null) return;
            particleControl.SetParticles(itemDataManager.GetProjectileParticleFrames(dustIndex));
        }


        private void SetSprite(Sprite sprite)
        {
            if (spriteRenderer == null)
                LoadComponents();

            spriteRenderer.sprite = sprite;
        }


        public virtual void SetOffsetPosition()
        {
            Model.transform.localPosition += (Vector3)OffsetPosition;
        }

        public virtual void SetOffsetPosition(Vector3 offsetPosition)
        {
            this.OffsetPosition = offsetPosition;
            Model.transform.localPosition += (Vector3)OffsetPosition;
        }

        public virtual void SetOffsetAngle()
        {
            transform.localRotation = Quaternion.Euler(0, 0, OffsetZAngle);
        }

        public virtual void SetOffsetAngle(float zAngle)
        {
            this.OffsetZAngle = zAngle;
            transform.localRotation = Quaternion.Euler(0, 0, OffsetZAngle);
        }

        private void UseGravity(bool _useGravity)
        {
            this.useGravity = _useGravity;
            if (useGravity)
            {
                rb.isKinematic = false;
                rb.gravityScale = 1.0f;
            }
            else
            {
                rb.isKinematic = true;
            }
        }
    }
}