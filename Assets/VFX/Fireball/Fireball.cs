using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            col.transform.GetComponent<PlayerControl>().ChangeHealth(Damage);
        }
        if (InfoSave.instance.SceneState == SceneSettings.Public)
        {
            //HandMagic.instance.SC.RemoveObjectFromNetwork(gameObject);
            //HandMagic.instance.SC.Fireballs.Remove(gameObject);

        }
        Destroy(gameObject);
    }
}
