using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// An abstract class representing a projectile.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Projectile : MonoBehaviour
    {
        #region Properties

        public ItemData ItemData { get; private set; }
        protected GameObject Model { get; private set; }

        /// <summary>
        /// Whether the projectile uses gravity or not.
        /// </summary>
        public bool useGravity;
        #endregion


        [Header("References")]
        protected ParticleControl particleControl;
        protected SpriteRenderer spriteRenderer;
        protected Rigidbody2D rb;
        protected GameDataManager itemDataManager;



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
            itemDataManager = GameDataManager.Instance;
        }


     
        public void SetData(ItemData itemData, bool updateSprite = true)
        {
            this.ItemData = itemData;

            if(updateSprite)
            {
                SetSprite(itemData.icon);
            }
            
        }

        /// <summary>
        /// Sets the item data of the projectile and updates its sprite.
        /// </summary>
        /// <param name="itemData">The item data to set.</param>
        /// <param name="sprite">The sprite to set.</param>
        public void SetData(ItemData itemData, Sprite sprite, bool updateSprite = true)
        {
            this.ItemData = itemData;

            if (updateSprite)
            {
                SetSprite(itemData.icon);
            }
        }


        /// <summary>
        /// Sets the item data of the projectile, updates its sprite and sets whether it uses gravity.
        /// </summary>
        /// <param name="itemData">The item data to set.</param>
        /// <param name="sprite">The sprite to set.</param>
        /// <param name="useGravity">Whether the projectile uses gravity or not.</param>
        public void SetData(ItemData itemData, Sprite sprite, bool useGravity, bool updateSprite = true)
        {
            this.ItemData = itemData;          
            UseGravity(useGravity);

            if (updateSprite)
            {
                SetSprite(itemData.icon);
            }

        }


        /// <summary>
        /// Sets the particle system to emit dust based on the given dust index.
        /// </summary>
        /// <param name="dustIndex">The index of the dust particles to emit.</param>
        public virtual void SetDust(int dustIndex)
        {
            if (particleControl == null) return;
            particleControl.SetParticles(itemDataManager.GetProjectileParticleFrames(dustIndex));
        }

        /// <summary>
        /// Sets the sprite renderer's sprite to the given sprite.
        /// </summary>
        /// <param name="sprite">The sprite to set.</param>
        private void SetSprite(Sprite sprite)
        {
            if (spriteRenderer == null)
                LoadComponents();

            spriteRenderer.sprite = sprite;
        }


 

        /// <summary>
        /// Enables or disables gravity on the game object.
        /// </summary>
        /// <param name="_useGravity">Whether to use gravity or not.</param>
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