using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
[System.Serializable]
public class PoolType
{
    public GameObject PoolObject;
    public int PoolSize;
    public Queue<GameObject> objectPool;
}
public class ObjectPooler : MonoBehaviourPunCallbacks, IPunPrefabPool
{
    public static ObjectPooler instance;
    private void Awake() { instance = this; }

    public bool ShouldPool;

    public List<PoolType> Pools;
    private void Start()
    {
        NetworkManager.OnInitialized += WaitToPool;
    }
    public void WaitToPool() { Invoke("InitalizePool", 0.05f); }
    public void InitalizePool()
    {
        if (!ShouldPool)
            return;

        for (int i = 0; i < Pools.Count; i++)
        {
            Pools[i].objectPool = new Queue<GameObject>();
            for (int j = 0; j < Pools[i].PoolSize; j++)
            {
                GameObject obj = PhotonNetwork.Instantiate(Pools[i].PoolObject.name, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                Pools[i].objectPool.Enqueue(obj);
            }
        }
            
        PhotonNetwork.PrefabPool = this;
    }
    

    // IPunPrefabPool implementation
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        // Check if the requested prefab matches the object we want to pool
        for (int i = 0; i < Pools.Count; i++)
        {
            if (prefabId == Pools[i].PoolObject.name)
            {
                // Check if the object pool is empty, instantiate a new object and add it to the pool
                if (Pools[i].objectPool.Count == 0)
                {
                    GameObject obj = PhotonNetwork.Instantiate(Pools[i].PoolObject.name, position, rotation);
                    obj.SetActive(false);
                    Pools[i].objectPool.Enqueue(obj);
                }

                // Dequeue an object from the pool and return it
                GameObject pooledObject = Pools[i].objectPool.Dequeue();
                pooledObject.SetActive(true);

                return pooledObject;
            }
        }
        
        Debug.LogWarning("Trying to instantiate a prefab that is not included in the object pool!");
        // If the requested prefab is not the one we want to pool, return null
        return null;
    }

    public void Destroy(GameObject gameObject)
    {
        // Reset the object's position and rotation and return it to the pool
        gameObject.SetActive(false);
        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;

        for (int i = 0; i < Pools.Count; i++)
        {
            if (gameObject.name == Pools[i].PoolObject.name + "(Clone)")
            {
                Pools[i].objectPool.Enqueue(gameObject);
            }
        }
                
    }
    
}
