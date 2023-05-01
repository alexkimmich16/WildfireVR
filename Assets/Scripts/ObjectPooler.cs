using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;


namespace ObjectPooling
{
    [System.Serializable]
    public class PoolType
    {
        public GameObject PoolObject;
        public int PoolSize;
        public bool ForceRecycle;
        public Queue<GameObject> objectPool;
        //public Queue<GameObject> toDestory;
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
            if (PrefabPoolSet())
                return;


            if (RefPool != null && this.ReferencePool != null)
                foreach (GameObject prefab in this.ReferencePool)
                    if (!RefPool.ResourceCache.ContainsKey(prefab.name))
                        RefPool.ResourceCache.Add(prefab.name, prefab);

            for (int i = 0; i < Pools.Count; i++)
            {
                Pools[i].objectPool = new Queue<GameObject>();
                if (PhotonNetwork.IsMasterClient)
                {
                    for (int j = 0; j < Pools[i].PoolSize; j++)
                    {
                        GameObject obj = PhotonNetwork.Instantiate(Pools[i].PoolObject.name, Vector3.zero, Quaternion.identity);
                        obj.SetActive(false);
                        Pools[i].objectPool.Enqueue(obj);
                    }
                }
                else
                {
                    List<GameObject> objectsWithName = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == Pools[i].PoolObject.name + "(Clone)").ToList();
                    List<GameObject> ActiveList = objectsWithName.Where(obj => obj.activeSelf).ToList();
                    List<GameObject> InActiveList = objectsWithName.Where(obj => !obj.activeSelf).ToList();
                    for (int j = 0; j < ActiveList.Count; j++)
                        Pools[i].objectPool.Enqueue(ActiveList[j]);
                    for (int j = 0; j < InActiveList.Count; j++)
                        Pools[i].objectPool.Enqueue(InActiveList[j]);
                    //Debug.Log("act: " + ActiveList.Count + " In: " + InActiveList.Count);
                }
            }
            //Debug.Log("Pools[i].objectPool: " + Pools[0].objectPool.Count);
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
                            //GameObject obj = Pools[i].toDestory.Dequeue();

                            //Pools[i].objectPool.Enqueue(obj);

                            Debug.LogError("Somehow out of resource: " + Pools[i].PoolObject.name);
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
                    {
                        if(pooledObject.activeSelf)
                            this.Destroy(pooledObject);

                        Pools[i].objectPool.Enqueue(pooledObject);
                    }
                   
                        

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
            // If in pool, Reset the object's position and rotation and return it to the pool
            for (int i = 0; i < Pools.Count; i++)
            {
                if (gameObject.name == Pools[i].PoolObject.name + "(Clone)")
                {
                    Pools[i].objectPool.Enqueue(gameObject);
                    gameObject.SetActive(false);
                    gameObject.transform.position = Vector3.zero;
                    gameObject.transform.rotation = Quaternion.identity;
                    return;
                }
            }
            //else, destroy it
            Object.Destroy(gameObject);
        }

        public bool PrefabPoolSet()
        {
            if ((PhotonNetwork.PrefabPool is DefaultPool) == false)
                return false;
            return (PhotonNetwork.PrefabPool as DefaultPool).ResourceCache.Count == ReferencePool.Count + Pools.Count;
        }
    }
}
