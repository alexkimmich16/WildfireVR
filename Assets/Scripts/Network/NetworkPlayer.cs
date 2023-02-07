using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using static Odin.Net;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using RestrictionSystem;
public class NetworkPlayer : MonoBehaviourPun
{
    public Transform Head;
    public Transform Left;
    public Transform Right;

    private Transform RigHead;
    private Transform RigLeft;
    private Transform RigRight;
    private Transform CharacterDisplay;

    public GameObject SkinRenderer;
    public bool TestingSelf;
    public bool Dead;
    

    void Start()
    {
        CharacterDisplay = AIMagicControl.instance.MyCharacterDisplay;
        RigHead = AIMagicControl.instance.CamOffset;
        RigLeft = AIMagicControl.instance.PositionObjectives[(int)Side.left]; //rig.transform.Find("Camera Offset/LeftHand Controller");
        RigRight = AIMagicControl.instance.PositionObjectives[(int)Side.right]; //rig.transform.Find("Camera Offset/RightHand Controller");

        transform.SetParent(NetworkManager.instance.playerList);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Head.gameObject.SetActive(TestingSelf || Dead);
            Left.gameObject.SetActive(TestingSelf || Dead);
            Right.gameObject.SetActive(TestingSelf || Dead);
            SkinRenderer.SetActive(TestingSelf || Dead);

            //MapPosition(transform, CharacterDisplay);
            if (!Dead)
            {
                transform.position = CharacterDisplay.position;
                MapPosition(Head, RigHead);
                MapPosition(Left, RigLeft);
                MapPosition(Right, RigRight);
            }
            
        }
        if (SceneLoader.instance.CurrentSetting != CurrentGame.Battle)
            return;
        //if()
        if (transform.position.x < ZoneController.instance.MagicLineWorldPos(ZoneController.instance.MagicLinePos))
            if (!Contains(ZoneController.instance.Players1, gameObject))
                ZoneController.instance.Players1.Add(gameObject);
            else if (transform.position.x > ZoneController.instance.MagicLineWorldPos(ZoneController.instance.MagicLinePos))
                if (!Contains(ZoneController.instance.Players2, gameObject))
                    ZoneController.instance.Players2.Add(gameObject);
    }

    void MapPosition(Transform target,Transform rigTrans)
    {
        target.position = rigTrans.position;
        target.rotation = rigTrans.rotation;
    }

    
}
