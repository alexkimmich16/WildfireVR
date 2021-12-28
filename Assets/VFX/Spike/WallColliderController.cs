using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColliderController : MonoBehaviour
{
    public ParticleSystem wall;
    private float timer;
    public BoxCollider Collider;
    public Vector3 BaseSize;
    //public float TimeOffset;
    void Update()
    {
        //timer += Time.deltaTime;
        timer = wall.time;
        float AdjustedTime = (timer / wall.main.duration);
        Vector3 size = new Vector3(wall.sizeOverLifetime.x.curve.Evaluate(AdjustedTime), 
            wall.sizeOverLifetime.y.curve.Evaluate(AdjustedTime), 
            wall.sizeOverLifetime.z.curve.Evaluate(AdjustedTime));

        Collider.size = new Vector3(size.x * BaseSize.x, size.y * BaseSize.y, size.z * BaseSize.z);
        var particleSystemMainModule = transform.GetComponent<ParticleSystem>().main;
        float AdjustedYRotation = particleSystemMainModule.startRotationY.constant * Mathf.Rad2Deg;
        float FinalAdjustment = (-34.2475f * AdjustedYRotation) -1245.06f;
        transform.rotation = Quaternion.Euler(0, AdjustedYRotation, 0);
    }


}
