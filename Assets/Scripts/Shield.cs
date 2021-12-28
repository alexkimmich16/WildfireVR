using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public Transform Follow;
    public Side side;
    void Start()
    {
        if (side == Side.Left)
            Follow = transform.Find("XR Rig/Camera Offset/LeftHand Controller");
        else if(side == Side.Right)
            Follow = transform.Find("XR Rig/Camera Offset/RightHand Controller");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Follow.position;
        transform.rotation = Follow.rotation;
    }
}
