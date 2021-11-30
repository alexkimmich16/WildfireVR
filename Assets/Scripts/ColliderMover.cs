using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderMover : MonoBehaviour
{
    public Transform Colliders;
    public Transform Camera;

    public float YOffset;

    void Update()
    {
        Colliders.position = Camera.position;
        Colliders.rotation = Quaternion.Euler(Colliders.rotation.eulerAngles.x, Camera.rotation.eulerAngles.y, Colliders.eulerAngles.z);

        Colliders.position = new Vector3(Colliders.position.x, Camera.position.y - YOffset, Colliders.position.z); 
        //y on colliders spins
    }
}
