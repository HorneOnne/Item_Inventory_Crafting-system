using UnityEngine.Pool;
using UnityEngine;

namespace UltimateItemSystem
{
    /// <summary>
    /// A singleton class for managing the object pooling of damage pop-up text GameObjects.
    /// </summary>
    public class DamagePopupSpawner : Singleton<DamagePopupSpawner>
    {
        /// <summary>
        /// The prefab to be used as the basis for creating new damage pop-up text GameObjects.
        /// </summary>
        public GameObject damagePopupTextPrefab;

        /// <summary>
        /// Indicates whether collection checks should be performed to prevent releasing an item that is already in the pool.
        /// </summary>  
        public bool collectionChecks = true;

        /// <summary>
        /// The maximum number of items that can be stored in the object pool.
        /// </summary>
        public int maxPoolSize = 10;

        IObjectPool<GameObject> m_Pool;

        /// <summary>
        /// The object pool for storing the damage pop-up text GameObjects.
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


        // <summary>
        /// Creates a new damage pop-up text GameObject to add to the object pool.
        /// </summary>
        /// <returns>The newly created damage pop-up text GameObject.</returns>
        private GameObject CreatePooledItem()
        {
            return Instantiate(damagePopupTextPrefab, this.gameObject.transform);
        }



        /// <summary>
        /// Deactivates a damage pop-up text GameObject that has been returned to the object pool.
        /// </summary>
        /// <param name="gameObject">The damage pop-up text GameObject to deactivate.</param>
        private void OnReturnedToPool(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Activates a damage pop-up text GameObject that has been taken from the object pool.
        /// </summary>
        /// <param name="gameObject">The damage pop-up text GameObject to activate.</param>
        private void OnTakeFromPool(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }



        /// <summary>
        /// Destroys a damage pop-up text GameObject that has been returned to the object pool when the pool capacity is reached.
        /// </summary>
        /// <param name="gameObject">The damage pop-up text GameObject to destroy.</param>
        private void OnDestroyPoolObject(GameObject gameObject)
        {
            Destroy(gameObject);
        }
    }
}