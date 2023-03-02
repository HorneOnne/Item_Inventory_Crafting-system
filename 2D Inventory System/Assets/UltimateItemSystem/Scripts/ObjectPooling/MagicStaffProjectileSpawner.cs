using UnityEngine;
using UnityEngine.Pool;

namespace UltimateItemSystem
{
    /// <summary>
    /// A class that manages the pooling of Magic Staff projectiles for efficient object instantiation
    /// </summary>
    public class MagicStaffProjectileSpawner : Singleton<MagicStaffProjectileSpawner>
    {
        /// <summary>
        /// The prefab for the Magic Staff projectile
        /// </summary>
        public GameObject magicStaffProjectile;

        /// <summary>
        /// Determines whether to perform collection checks when returning an item to the pool
        /// </summary> 
        public bool collectionChecks = true;

        /// <summary>
        /// The maximum number of items that can be stored in the pool
        /// </summary>
        public int maxPoolSize = 10;

        IObjectPool<GameObject> m_Pool;


        /// <summary>
        /// Gets the pool of Magic Staff projectiles and creates a new pool if it does not exist
        /// </summary>
        public IObjectPool<GameObject> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                }

                return m_Pool;
            }
        }


        /// <summary>
        /// Instantiates a new Magic Staff projectile game object
        /// </summary>
        private GameObject CreatePooledItem()
        {
            return Instantiate(magicStaffProjectile, this.gameObject.transform);
        }


        /// <summary>
        /// Disables the game object when it is returned to the pool using Release
        /// </summary>
        private void OnReturnedToPool(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Enables the game object when it is taken from the pool using Get
        /// </summary>
        private void OnTakeFromPool(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }



        /// <summary>
        /// Destroys the game object when the pool capacity is reached
        /// </summary>
        private void OnDestroyPoolObject(GameObject gameObject)
        {
            Destroy(gameObject);
        }
    }
}
