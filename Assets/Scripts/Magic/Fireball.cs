using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Fireball : MonoBehaviour
{
    public float Speed;
    
    public int Damage = 5;

    [HideInInspector]
    public GameObject Explosion, Flash, DestoryAudio;
    [HideInInspector]
    public Rigidbody RB;

    public bool Absorbing;

    public VFXHolder VFX;

    private FMOD.Studio.EventInstance FireballSound;
    public GameObject FireballSphere;
    //public ParticleSystem PS;
    //public Animation Curve;
    //private int MaxParticleEmit;
    public void SetAbsorbed(bool State)
    {
        Absorbing = State;
        VFX.SetNewState(false);
    }
    //public float LifeTime = 3;
    void Update()
    {
        if (GetComponent<PhotonView>().IsMine == false)
            return;
        if (Absorbing == true)
        {
            //direction towards hand(graudal or isntant)
        }
        else
        {
            if(FireballSphere.activeSelf == true)
                RB.velocity = transform.forward * Time.deltaTime * Speed;
        }

    }

    /// <summary>
    /// on absorb parent + keep position
    /// watch for disance to unparent
    /// </summary>

    void OnCollisionEnter(Collision col)
    {
        if (Absorbing == true)
            return;
        if(Explosion != null)
            GameObject.Instantiate(Explosion, this.transform.position, this.transform.rotation);
        if(Flash != null)
            GameObject.Instantiate(Flash, this.transform.position, this.transform.rotation);

        if (col.collider.tag == "HitBox")
            NetworkManager.instance.LocalTakeDamage(Damage);
        gameObject.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);
        GetComponent<PhotonDestroy>().StartCountdown();
    }

    [PunRPC]
    public void OnHit()
    {
        VFX.SetNewState(false);
        FireballSphere.SetActive(false);
        if (SoundManager.instance.CanPlaySound(SoundType.Effect))
            FireballSound.setParameterByName("Exit", 1f);
        gameObject.GetComponent<SphereCollider>().enabled = false;
        DecalSpawner.instance.SpawnDecalAtPosition(transform.position, Quaternion.Euler(new Vector3(90,0,0)));
    }

    [PunRPC]
    public void ChangeSpeed(float speed)
    {
        Speed = speed;
    }
    public void Bounce(Vector3 collisionNormal)
    {
        Vector3 New = Vector3.Reflect(transform.forward, collisionNormal);
        transform.eulerAngles = New;
        //rb.velocity = direction * Mathf.Max(speed, minVelocity);
    }
    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        if(SoundManager.instance.EnableEffectSounds && SoundManager.instance.EnableSounds)
        {
            FireballSound = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.FireballRef);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(FireballSound, GetComponent<Transform>());
            FireballSound.start();
            FireballSound.setParameterByName("Exit", 0f);
        }
        //MaxParticleEmit = PS.
    }
}
