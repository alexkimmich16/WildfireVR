using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAtFloor : MonoBehaviour
{
    private Transform Cam;
    public static float FloorHeight = 0.36f;
    void Start()
    {
        Cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Cam.position.x, FloorHeight, Cam.position.z);
    }
}
