using UnityEngine;

public class CamAtFloor : MonoBehaviour
{
    private Transform Cam;
    public float FloorHeight = 0.13f;
    public float ElevatorOffset;
    public GameObject NetworkOBJ;
    public float HitOffset;
    public bool IsActive = true;

    public bool RestrictAll = false;
    public bool AllowRaycast;

    public LayerMask LM;

    public void CustomUpdate()
    {
        if (RestrictAll)
            return;

        if(Physics.Raycast(Cam.transform.position, -Vector3.up, out RaycastHit hit, Mathf.Infinity, LM) && AllowRaycast)
        {
            transform.position = new Vector3(Cam.position.x, hit.point.y + HitOffset, Cam.position.z);
        }
        else
        {
            transform.position = new Vector3(Cam.position.x, ElevatorOffset + DoorManager.instance.Doors[0].OBJ.position.y, Cam.position.z);
        }
        
        

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
