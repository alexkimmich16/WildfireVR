using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAtFloor : MonoBehaviour
{
    private Transform Cam;
    public float FloorHeight = 0.13f;
    public float ElevatorOffset;
    public GameObject NetworkOBJ;
    public bool IsActive = true;
    public void CustomUpdate()
    {
        int SequenceNum = (int)DoorManager.instance.Sequence;
        float TempFloorHeight;
        if (SequenceNum < 2)
        {
            TempFloorHeight = ElevatorOffset + DoorManager.instance.Doors[0].OBJ.position.y;
        }
        else
        {
            TempFloorHeight = FloorHeight;
        }
        transform.position = new Vector3(Cam.position.x, TempFloorHeight, Cam.position.z);

    }
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
        if(IsActive)
            CustomUpdate();
    }
}
