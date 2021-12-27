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
    public AudioSource FireSound; 
    public AudioClip ExplosionSound; 

    //public float LifeTime = 3;
    void Start()
    {
        if (HandMagic.AllSounds == true)
        {
            FireSound.Play();
        }
    }
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
    }

    void OnCollisionEnter(Collision col)
    {
        GameObject Ex = (GameObject)GameObject.Instantiate(Explosion, this.transform.position, this.transform.rotation);
        GameObject Fl = (GameObject)GameObject.Instantiate(Flash, this.transform.position, this.transform.rotation);
        if(HandMagic.AllSounds == true)
        {
            GameObject Audio = (GameObject)GameObject.Instantiate(DestoryAudio, this.transform.position, this.transform.rotation);
            Audio.GetComponent<AudioSource>().clip = ExplosionSound;
            Audio.GetComponent<AudioSource>().Play();
        }
        if (col.transform.tag == "Shield")
        {
            int Side = (int)col.transform.parent.GetComponent<HandActions>().side;
            HandMagic.instance.SC.ShieldDamage(Damage, Side);
        }
        else if (col.transform.tag == "Player")
        {
            //photonView.isMine
            //int MainNum =
            for (int i = 0; i < NetworkManager.instance.Players.Count; i++)
            {
                PhotonView photonView = NetworkManager.instance.Players[i].gameObject.GetComponent<PhotonView>();
                if (photonView.IsMine)
                {
                    photonView.transform.GetComponent<PlayerControl>().ChangeHealth(Damage);
                }
            }
        }
        Destroy(gameObject);
    }
}
