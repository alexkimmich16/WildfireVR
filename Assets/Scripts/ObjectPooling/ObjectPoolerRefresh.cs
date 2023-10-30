using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using System;
namespace ObjectPooling
{
    public class ObjectPoolerRefresh : MonoBehaviour
    {
        public static void RefreshObjects()
        {
            for (int i = 0; i < ObjectPooler.instance.Pools.Count; i++)
            {
                if (ObjectPooler.instance.Pools[i].objectPool == null)
                {
                    Debug.LogError("null object pool");
                    return;
                }
                    
                List<GameObject> ActiveList = ObjectPooler.instance.Pools[i].objectPool.ToList().Where(obj => obj.activeSelf).ToList();
                for (int j = 0; j < ActiveList.Count; j++)
                    Destroy(ActiveList[j]);
            }
        }
    }
}

