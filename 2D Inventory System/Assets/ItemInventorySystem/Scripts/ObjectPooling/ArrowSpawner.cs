using UnityEngine;
using UnityEngine.Pool;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// A class for managing the creation and use of arrows in a game using an object pool.
    /// </summary>
    public class ArrowSpawner : Singleton<ArrowSpawner>
    {
        public GameObject arrowPrefab;

        /// <summary>
        /// A boolean flag indicating whether to perform collection checks when returning items to the pool.
        /// </summary>
        public bool collectionChecks = true;

        /// <summary>
        /// The maximum size of the object pool.
        /// </summary>
        public int maxPoolSize = 10;

        IObjectPool<GameObject> m_Pool;


        /// <summary>
        /// The object pool for arrows.
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
        /// Creates a new arrow to be added to the object pool.
        /// </summary>
        /// <returns>A new arrow GameObject.</returns>
        private GameObject CreatePooledItem()
        {
            return Instantiate(arrowPrefab, this.gameObject.transform);
        }


        /// <summary>
        /// Called when an item is returned to the pool using Release.
        /// </summary>
        /// <param name="gameObject">The returned GameObject.</param>
        private void OnReturnedToPool(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Called when an item is taken from the pool using Get.
        /// </summary>
        /// <param name="gameObject">The taken GameObject.</param>
        private void OnTakeFromPool(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }



        /// <summary>
        /// Destroys an object when the pool capacity is reached and any items returned will be destroyed.
        /// </summary>
        /// <param name="gameObject">The object to be destroyed.</param>
        private void OnDestroyPoolObject(GameObject gameObject)
        {
            Destroy(gameObject);
        }
    }
}