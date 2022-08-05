using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DamageShard : MonoBehaviour
{
    public float Speed;
    private Rigidbody RB;
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Forward = transform.forward;
        RB.velocity = Forward * Time.deltaTime * Speed;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.gameObject.GetComponent<PhotonView>())
        {
            FireController.instance.DamageShardHit(col.collider.gameObject);
            Debug.Log("TakeDamage1");
            Destroy(gameObject);
        }
        
    }
}
