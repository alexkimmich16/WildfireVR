using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColliderController : MonoBehaviour
{
    public ParticleSystem wall;
    private float timer;
    public BoxCollider Collider;
    public Vector3 BaseSize;
    public float TimeOffset;
    void Update()
    {
        timer += Time.deltaTime;
        float AdjustedTime = (timer / wall.main.duration) + TimeOffset;
        Vector3 size = new Vector3(wall.sizeOverLifetime.x.curve.Evaluate(AdjustedTime), 
            wall.sizeOverLifetime.y.curve.Evaluate(AdjustedTime), 
            wall.sizeOverLifetime.z.curve.Evaluate(AdjustedTime));

        //Vector3 ObjectAdjust = new Vector3(1 / transform.position.x, 1 / transform.position.y, 1 / transform.position.z);

        Collider.size = new Vector3(size.x * BaseSize.x, size.y * BaseSize.y, size.z * BaseSize.z);
    }
}
