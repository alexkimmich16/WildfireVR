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
    //[HideInInspector]
    public float Speed;
    [HideInInspector]
    public static int Damage = 5;

    [HideInInspector]
    public GameObject Explosion, Flash, DestoryAudio;
    [HideInInspector]
    public Rigidbody RB;

    public bool Absorbing;

    //public float TrailRate, BallRate;

    public VFXHolder VFX;

    //public VisualEffect Trail;
    //public VisualEffect Ball;

    //public bool Collided;


    public void SetAbsorbed(bool State)
    {
        Absorbing = State;
        VFX.SetNewState(false);
    }
    //public float LifeTime = 3;
    void Update()
    {
        if (Absorbing == false)
            return;
        if (GetComponent<PhotonView>().IsMine == false)
            return;

        RB.velocity = transform.forward * Time.deltaTime * Speed;
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

        SoundManager.instance.PlayAudio("FireballExplosion", null);

        //gameObject.GetComponent<PhotonDestroy>().LifeTime = 5f;
        //gameObject.GetComponent<PhotonDestroy>().Timer = 0f;

        gameObject.GetComponent<SphereCollider>().enabled = false;

        VFX.SetNewState(false);
        
        if (col.collider.tag == "HitBox")
        {
            NetworkManager.instance.LocalTakeDamage(Damage);
        }
        GetComponent<PhotonDestroy>().DoDestroy();
    }
    public PhotonView MyPhotonView()
    {
        for (int i = 0; i < NetworkManager.instance.PlayerPhotonViews.Count; i++)
            if (NetworkManager.instance.PlayerPhotonViews[i].IsMine)
                return NetworkManager.instance.PlayerPhotonViews[i];
        Debug.LogError("Could Not Get PhotonView");
        return null;
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
        //Speed = FireballController.instance.Speed;
    }
}
