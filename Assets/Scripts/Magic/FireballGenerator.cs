using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Odin.Net;
using RestrictionSystem;
public class FireballGenerator : MonoBehaviour
{
    public float MaxTime;
    public float Speed;
    public Transform Spawn;
    private float Timer;
    public bool Active;
    void Update()
    {
        if (!Active || !Initialized())
            return;
        Timer += Time.deltaTime;
        Spawn.LookAt(Camera.main.transform);
        if (Timer > MaxTime)
        {
            SpawnFireball();
            Timer = 0;
        }
    }

    public void SpawnFireball()
    {
        //GameObject Current = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(Spell.Fireball, true)), Spawn.position, Spawn.rotation);
        GameObject Current = PhotonNetwork.Instantiate((AIMagicControl.instance.spells.SpellName(CurrentLearn.Fireball, 1)), Spawn.position, Spawn.rotation);
        Current.GetPhotonView().RPC("ChangeSpeed", RpcTarget.All, Speed);
    }
}
