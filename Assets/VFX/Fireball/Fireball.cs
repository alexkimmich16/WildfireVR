using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float Speed;
    public GameObject Explosion, Flash;

    //public float LifeTime = 3;

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * Speed);
    }

    void OnCollisionEnter(Collision col)
    {
        //if (col.transform.GetComponent<pla>())
        GameObject Ex = (GameObject)GameObject.Instantiate(Explosion, this.transform.position, this.transform.rotation);
        GameObject Fl = (GameObject)GameObject.Instantiate(Flash, this.transform.position, this.transform.rotation);
        //destroy
        //leave fire
        //play sound
        Destroy(gameObject);
    }
}
