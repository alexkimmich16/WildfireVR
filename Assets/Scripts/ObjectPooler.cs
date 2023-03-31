using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ObjectPooler : MonoBehaviourPunCallbacks, IPunPrefabPool
{
    public GameObject objectToPool;
    public int poolSize = 10;
    public static float CurrentTime = 0.05f;
    private float MyTime = 0f;
    public Queue<GameObject> objectPool = new Queue<GameObject>();
    
    private void Start()
    {
        NetworkManager.OnInitialized += WaitToPool;
        MyTime = CurrentTime;
        CurrentTime += 0.05f;
    }
    public void WaitToPool() { Invoke("InitalizePool", MyTime); }
    public void InitalizePool()
    {
        // Instantiate objects in the object pool
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = PhotonNetwork.Instantiate(objectToPool.name, Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
        //PhotonNetwork.PrefabPool = this;
    }

    // IPunPrefabPool implementation
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        // Check if the requested prefab matches the object we want to pool
        if (prefabId == objectToPool.name)
        {
            // Check if the object pool is empty, instantiate a new object and add it to the pool
            if (objectPool.Count == 0)
            {
                GameObject obj = PhotonNetwork.Instantiate(objectToPool.name, Vector3.zero, Quaternion.identity);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            // Dequeue an object from the pool and return it
            GameObject pooledObject = objectPool.Dequeue();
            pooledObject.SetActive(true);
            return pooledObject;
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
        objectPool.Enqueue(gameObject);
    }
}
