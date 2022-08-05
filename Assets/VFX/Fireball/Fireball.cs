using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
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
    
    //public float LifeTime = 3;
    void Update()
    {
        if (Absorbing == false)
        {
            Vector3 Forward = transform.forward;
            //transform.Translate(Vector3.forward * Time.deltaTime * Speed);
            RB.velocity = Forward * Time.deltaTime * Speed;
            //Debug.Log("update");
        }

    }



    void OnCollisionEnter(Collision col)
    {
        if(Explosion != null)
            GameObject.Instantiate(Explosion, this.transform.position, this.transform.rotation);
        if(Flash != null)
            GameObject.Instantiate(Flash, this.transform.position, this.transform.rotation);
        SoundManager.instance.PlayAudio("FireballExplosion", null);

        Debug.Log(col.gameObject.name);
        
        if (col.collider.tag == "HitBox")
        {
            Debug.Log("TakeDamage2");
            //MyPhotonView().transform.GetComponent<PlayerControl>().ChangeHealth(Damage);
            //TakeDamage
            NetworkManager.instance.LocalTakeDamage(Damage);


            Debug.Log("TakeDamage1");
            
        }
        Destroy(gameObject);
        
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
        Speed = FireballController.instance.Speed;
    }

    /*
        if (col.collider.tag == "Shield")
        {
            if(HandMagic.instance.SC.Stats[0].Shield != null)
            {
                if (col.collider.transform == HandMagic.instance.SC.Stats[0].Shield.transform)
                {
                    HandMagic.instance.SC.ShieldDamage(Damage, 0);
                }
            }
            else if (HandMagic.instance.SC.Stats[1].Shield != null)
            {
                if (col.collider.transform == HandMagic.instance.SC.Stats[1].Shield.transform)
                {
                    HandMagic.instance.SC.ShieldDamage(Damage, 1);
                }
            }
        }
        */
}
