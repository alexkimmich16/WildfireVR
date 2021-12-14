using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float Speed;
    public GameObject Explosion, Flash, DestoryAudio;
    public AudioSource FireSound; 
    public AudioClip ExplosionSound; 

    //public float LifeTime = 3;
    void Start()
    {
        FireSound.Play(0);
    }
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
    }

    void OnCollisionEnter(Collision col)
    {
        //if (col.transform.GetComponent<pla>())
        GameObject Ex = (GameObject)GameObject.Instantiate(Explosion, this.transform.position, this.transform.rotation);
        GameObject Fl = (GameObject)GameObject.Instantiate(Flash, this.transform.position, this.transform.rotation);
        GameObject Audio = (GameObject)GameObject.Instantiate(DestoryAudio, this.transform.position, this.transform.rotation);
        Audio.GetComponent<AudioSource>().clip = ExplosionSound;
        Audio.GetComponent<AudioSource>().Play();
        Destroy(gameObject);
    }
}
