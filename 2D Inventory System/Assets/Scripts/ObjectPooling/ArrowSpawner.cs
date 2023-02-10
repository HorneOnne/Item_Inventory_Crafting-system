using UnityEngine;
using UnityEngine.Pool;

public class ArrowSpawner : Singleton<ArrowSpawner> 
{
    public GameObject arrowPrefab;

    // Collection checks will throw errors if we try to release an item that is already
    // in the pool.   
    public bool collectionChecks = true;
    public int maxPoolSize = 10;

    IObjectPool<GameObject> m_Pool;

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if(m_Pool == null)
            {
                m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
            }

            return m_Pool;
        }
    }



    private GameObject CreatePooledItem()
    {
        return Instantiate(arrowPrefab, this.gameObject.transform);
    }


    // Called when an item is returned to the pool using Release
    private void OnReturnedToPool(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }


    // Called when an item is taken from the pool using Get
    private void OnTakeFromPool(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }



    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    private void OnDestroyPoolObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }

}