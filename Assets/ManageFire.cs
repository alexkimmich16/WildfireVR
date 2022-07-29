using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
public class ManageFire : MonoBehaviour
{
    public float Speed;
    public VisualEffect Fire;
    public Transform Cam;
    public Transform Hand;
    public Vector3 Rot;

    private void Update()
    {
        Fire.playRate = Speed;
        //gameObject.transform.position = Hand.position;
        //gameObject.transform.rotation = Cam.rotation;
        //Fire.SetVector3("Rot", new Vector3(Rot.x, Rot.y, Rot.z));
    }
}
