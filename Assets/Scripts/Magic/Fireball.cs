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

    [HideInInspector]
    public GameObject Explosion, Flash, DestoryAudio;
    public Rigidbody RB;

    public bool Absorbing;

    public VFXHolder VFX;

    private FMOD.Studio.EventInstance FireballSound;
    public GameObject FireballSphere;

    public float TimeActive;
    public float AccelerateSpeed;
    public float StartTime;

    public void SetAbsorbed(bool State)
    {
        Absorbing = State;
        VFX.SetNewState(false);
    }

    private Vector3 ForwardInfo;

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
            if (FireballSphere.activeSelf)
            {
                //transform.position += transform.forward * Time.deltaTime * Speed * Mathf.Pow(TimeActive, AccelerateSpeed);
                transform.forward = ForwardInfo;
                RB.velocity = transform.forward * Time.deltaTime * Speed * Mathf.Pow(TimeActive, MimicTester.instance.AccelerateSpeed);
                TimeActive += Time.deltaTime;
            }

        }

    }
    void OnCollisionEnter(Collision col)
    {
        if (Absorbing == true)
            return;
        if (Explosion != null)
            GameObject.Instantiate(Explosion, this.transform.position, this.transform.rotation);
        if (Flash != null)
            GameObject.Instantiate(Flash, this.transform.position, this.transform.rotation);
        //Debug.Log("Tag: " + col.collider.tag);

        if (col.collider.tag == "HitBox")
        {
            NetworkManager.instance.LocalTakeDamage(FireballController.instance.Damage);
            //Debug.Log("IsMine");
        }
        //Debug.Log("IsMine2");

        GetComponent<PhotonDestroy>().StartCountdown();
        gameObject.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);

    }

    [PunRPC]
    public void OnHit()
    {
        VFX.SetNewState(false);
        FireballSphere.SetActive(false);
        if (SoundManager.instance.CanPlaySound(SoundType.Effect))
            FireballSound.setParameterByName("Exit", 1f);
        gameObject.GetComponent<SphereCollider>().enabled = false;
        DecalSpawner.instance.SpawnDecalAtPosition(transform.position, Quaternion.Euler(new Vector3(90, 0, 0)));
        //Debug.Log("onhit");
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
    private void OnEnable()
    {
        //Debug.Log("start");
        ForwardInfo = transform.forward;
        TimeActive = StartTime;
        VFX.SetNewState(true);//potentail problem
        RB = GetComponent<Rigidbody>();
        if (SoundManager.instance.EnableEffectSounds && SoundManager.instance.EnableSounds)
        {
            FireballSound = FMODUnity.RuntimeManager.CreateInstance(SoundManager.instance.FireballRef);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(FireballSound, GetComponent<Transform>());
            FireballSound.start();
            FireballSound.setParameterByName("Exit", 0f);
        }

        FireballSphere.SetActive(true);
        gameObject.GetComponent<SphereCollider>().enabled = true;

        AmbientVFX.instance.Actives.Add(transform);
        //add to controller
        //MaxParticleEmit = PS.
    }
    private void OnDisable()
    {
        AmbientVFX.instance.Actives.Remove(transform);
    }
}
