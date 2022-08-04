using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class FireballGenerator : MonoBehaviour
{
    public float MaxTime;
    public float Speed;
    public GameObject Fireball;
    public Transform Spawn;
    private float Timer;
    public bool Active;
    void Update()
    {
        if(Active == true)
        {
            Timer += Time.deltaTime;
            Spawn.LookAt(Camera.main.transform);
            if (Timer > MaxTime)
            {
                SpawnFireball();
                Timer = 0;
            }
        }
        
    }

    public void SpawnFireball()
    {
        GameObject Current = PhotonNetwork.Instantiate("RealFireball", Spawn.position, Spawn.rotation);
        Current.GetComponent<Fireball>().Speed = Speed;
    }
}
