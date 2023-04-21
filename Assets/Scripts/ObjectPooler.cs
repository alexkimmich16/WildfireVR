using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
[System.Serializable]
public class PoolType
{
    public GameObject PoolObject;
    public int PoolSize;
    public bool ForceRecycle;
    public Queue<GameObject> objectPool;
    public Queue<GameObject> toDestory;
}
public class ObjectPooler : MonoBehaviourPunCallbacks, IPunPrefabPool
{
    public static ObjectPooler instance;
    private void Awake() { instance = this; }

    public bool ShouldPool;

    public List<PoolType> Pools;

    public List<GameObject> ReferencePool;

    public DefaultPool RefPool;

    private void Start()
    {
        NetworkManager.OnInitialized += InitalizePool;
    }
    public void InitalizePool()
    {
        if (!ShouldPool)
            return;

        RefPool = PhotonNetwork.PrefabPool as DefaultPool;
        //Debug.Log("c: " + RefPool.ResourceCache.Count);

        //Debug.Log("PhotonNetwork.PrefabPool: " + PrefabPoolSet());
        if (PrefabPoolSet())
            return;


        if (RefPool != null && this.ReferencePool != null)
            foreach (GameObject prefab in this.ReferencePool)
                if(!RefPool.ResourceCache.ContainsKey(prefab.name))
                    RefPool.ResourceCache.Add(prefab.name, prefab);
        
        for (int i = 0; i < Pools.Count; i++)
        {
            Pools[i].objectPool = new Queue<GameObject>();

            if(Pools[i].ForceRecycle)
                Pools[i].toDestory = new Queue<GameObject>();
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
                //if (Pools[i].ForceRecycle)
                // Check if the object pool is empty, instantiate a new object and add it to the pool
                

                if (Pools[i].objectPool.Count == 0)
                {
                    if (Pools[i].ForceRecycle)
                    {
                        //grab oldest object
                        GameObject obj = Pools[i].toDestory.Dequeue();

                        Pools[i].objectPool.Enqueue(obj); 
                        //cue object
                        //
                    }
                    else
                    {
                        //GameObject obj = PhotonNetwork.Instantiate(prefabId, position, rotation);
                        GameObject obj = RefPool.Instantiate(prefabId, position, rotation);
                        obj.SetActive(false);
                        Pools[i].objectPool.Enqueue(obj);
                    }
                        
                }

                


                // Dequeue an object from the pool and return it
                GameObject pooledObject = Pools[i].objectPool.Dequeue();

                if (Pools[i].ForceRecycle)
                    Pools[i].objectPool.Enqueue(pooledObject);

                pooledObject.transform.position = position;
                pooledObject.transform.rotation = rotation;

                pooledObject.SetActive(true);

                return pooledObject;
            }
        }

        for (int i = 0; i < ReferencePool.Count; i++)
        {
            if (prefabId == ReferencePool[i].name)
            {
                return RefPool.Instantiate(prefabId, position, rotation);
            }
        }

        Debug.LogWarning("Trying to instantiate: " + prefabId + " that is not included in the object pool!");
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

    private void Update()
    {
        //if(Started)
            //Debug.Log("PhotonNetwork.PrefabPool: " + PhotonNetwork.PrefabPool.GetType());
            //Debug.Log("PhotonNetwork.PrefabPool Set: " + PrefabPoolSet());

    }
    //public bool PrefabPoolSet() { return !(PhotonNetwork.PrefabPool is DefaultPool); }
    public bool PrefabPoolSet()
    {
        if ((PhotonNetwork.PrefabPool is DefaultPool) == false)
            return false;
        return (PhotonNetwork.PrefabPool as DefaultPool).ResourceCache.Count == ReferencePool.Count + Pools.Count;
    }
}
