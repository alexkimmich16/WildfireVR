using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayer : MonoBehaviour
{
    //https://www.youtube.com/watch?v=uM89bDIrmZ0&ab_channel=RugbugRedfern
    public Transform Head;
    public Transform Left;
    public Transform Right;
    public PhotonView photonView;

    private Transform RigHead;
    private Transform RigLeft;
    private Transform RigRight;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XRRig rig = FindObjectOfType<XRRig>();
        RigHead = rig.transform.Find("Camera Offset/Main Camera");
        RigLeft = rig.transform.Find("Camera Offset/LeftHand Controller");
        RigRight = rig.transform.Find("Camera Offset/RightHand Controller");
        //3:30
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Head.gameObject.SetActive(false);
            Left.gameObject.SetActive(false);
            Right.gameObject.SetActive(false);

            MapPosition(Head, RigHead);
            MapPosition(Left, RigLeft);
            MapPosition(Right, RigRight);
        }
    }

    void MapPosition(Transform target,Transform rigTrans)
    {
        target.position = rigTrans.position;
        target.rotation = rigTrans.rotation;
    }
}
