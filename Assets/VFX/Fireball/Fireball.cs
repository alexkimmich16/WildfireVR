using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR.Interaction.Toolkit;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Fireball : MonoBehaviour
{
    public float Speed;
    public int Damage;
    public GameObject Explosion, Flash, DestoryAudio;

    //public float LifeTime = 3;
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
        //Debug.Log("update");
    }
    void OnCollisionEnter(Collision col)
    {
        if(Explosion != null)
            GameObject.Instantiate(Explosion, this.transform.position, this.transform.rotation);
        if(Flash != null)
            GameObject.Instantiate(Flash, this.transform.position, this.transform.rotation);
        SoundManager.instance.PlayAudio("FireballExplosion", null);

        Debug.Log(col.gameObject.name);

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
        else if (col.collider.tag == "HitBox")
        {
            for (int i = 0; i < NetworkManager.instance.Players.Count; i++)
            {
                PhotonView photonView = NetworkManager.instance.Players[i].networkPlayer.transform.GetComponent<PhotonView>();
                if (photonView.IsMine)
                {
                    photonView.transform.GetComponent<PlayerControl>().ChangeHealth(Damage);
                }
            }
        }
        Destroy(gameObject);
    }

    public void Bounce(Vector3 collisionNormal)
    {
        Vector3 New = Vector3.Reflect(transform.forward, collisionNormal);
        transform.eulerAngles = New;
        //rb.velocity = direction * Mathf.Max(speed, minVelocity);
    }
}
