using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAtFloor : MonoBehaviour
{
    private Transform Cam;
    public float FloorHeight = 0.13f;
    public GameObject NetworkOBJ;
    void Start()
    {
        if (NetworkOBJ == null)
            Cam = Camera.main.transform;
        else
            Cam = NetworkOBJ.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Cam.position.x, FloorHeight, Cam.position.z);
    }
}
