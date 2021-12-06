using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float Speed;

    void Update()
    {
        transform.Translate(-Vector3.left * Time.deltaTime * Speed);
    }

    void OnCollisionEnter(Collision col)
    {
        //if (col.transform.GetComponent<pla>())

        //destroy
        //leave fire
        //play sound
        Destroy(gameObject);
    }
}
