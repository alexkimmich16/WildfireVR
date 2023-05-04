using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
namespace ObjectPooling
{
    public class ObjectPoolerRefresh : MonoBehaviour
    {
        void Start()
        {
            OnlineEventManager.RestartEventCallback += RefreshObjects;
        }

        public void RefreshObjects()
        {
            for (int i = 0; i < ObjectPooler.instance.Pools.Count; i++)
            {
                List<GameObject> ActiveList = ObjectPooler.instance.Pools[i].objectPool.ToList().Where(obj => obj.activeSelf).ToList();
                for (int j = 0; j < ActiveList.Count; j++)
                    PhotonNetwork.Destroy(ActiveList[j]);
            }
        }
    }
}

