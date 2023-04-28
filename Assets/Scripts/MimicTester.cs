using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Odin.Net;
using RestrictionSystem;
public class MimicTester : MonoBehaviour
{
    public static MimicTester instance;
    void Awake() { instance = this; }
    public float MaxTime;
    public float FireballSpeed;
    public Transform Spawn;
    private float Timer;
    public bool Active;

    public bool LookAtPlayer;

    public CurrentLearn Motion;

    private bool PastState;
    public GameObject Flame;

    public float AccelerateSpeed;
    void Update()
    {
        if (!Active || !Initialized())
            return;
        Timer += Time.deltaTime;
        if (LookAtPlayer)
            Spawn.LookAt(Camera.main.transform);
        if (Timer > MaxTime)
        {
            if(Motion == CurrentLearn.Fireball)
                SpawnFireball();
            else if (Motion == CurrentLearn.Flames)
                ChangeFlames();
            Timer = 0;
        }
    }

    public void SpawnFireball()
    {
        //GameObject Current = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(Spell.Fireball, true)), Spawn.position, Spawn.rotation);
        GameObject Current = PhotonNetwork.Instantiate((AIMagicControl.instance.spells.SpellName(CurrentLearn.Fireball, 1)), Spawn.position, Spawn.rotation);
        Current.GetPhotonView().RPC("ChangeSpeed", RpcTarget.All, FireballSpeed);
    }

    public void ChangeFlames()
    {
        //GameObject Current = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(Spell.Fireball, true)), Spawn.position, Spawn.rotation);
        if (Flame == null)
        {
            Flame = PhotonNetwork.Instantiate((AIMagicControl.instance.spells.SpellName(CurrentLearn.Flames, 1)), Spawn.position, Spawn.rotation);
            Flame.GetPhotonView().RPC("SetFlamesOnline", RpcTarget.All, true);
        } 
        else if (Flame != null)
        {
            Flame.GetComponent<PhotonDestroy>().DestroyOnline();
            Flame = null;
            
        }
    }
}
