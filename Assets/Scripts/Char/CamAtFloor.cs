using UnityEngine;

public class CamAtFloor : MonoBehaviour
{
    private Transform Cam;
    public float FloorHeight = 0.13f;
    public float ElevatorOffset;
    public GameObject NetworkOBJ;
    public bool IsActive = true;

    public bool RestrictAll = false;
    public void CustomUpdate()
    {
        if (RestrictAll)
            return;
        //int SequenceNum = (int)DoorManager.instance.Sequence;
        /*
        float TempFloorHeight;
        if (SequenceNum < 2)
        {
            TempFloorHeight = ElevatorOffset + DoorManager.instance.Doors[0].OBJ.position.y;
        }
        else
        {
            
            //TempFloorHeight = FloorHeight;
        }
        */
        transform.position = new Vector3(Cam.position.x, ElevatorOffset + DoorManager.instance.Doors[0].OBJ.position.y, Cam.position.z);

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
        
    }
    private void FixedUpdate()
    {
        if (IsActive)
            CustomUpdate();
    }
}
