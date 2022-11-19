using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using static Odin.Net;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    public delegate void StateEvent();
    public static event StateEvent TakeDamage;

    public CamAtFloor AtFloor;
    //public XR
    public static void TakeDamageEventMethod() { if (TakeDamage != null){ TakeDamage(); } }

    void Start()
    {
        CharacterDisplay = AIMagicControl.instance.MyCharacterDisplay;
        RigHead = AIMagicControl.instance.CamOffset;
        RigLeft = AIMagicControl.instance.PositionObjectives[(int)Side.Left]; //rig.transform.Find("Camera Offset/LeftHand Controller");
        RigRight = AIMagicControl.instance.PositionObjectives[(int)Side.Right]; //rig.transform.Find("Camera Offset/RightHand Controller");
        if (photonView.IsMine)
        {
            //AtFloor.IsActive = false;
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Head.gameObject.SetActive(TestingSelf);
            Left.gameObject.SetActive(TestingSelf);
            Right.gameObject.SetActive(TestingSelf);
            SkinRenderer.SetActive(TestingSelf);

            transform.position = CharacterDisplay.position;
            //MapPosition(transform, CharacterDisplay);
            MapPosition(Head, RigHead);
            MapPosition(Left, RigLeft);
            MapPosition(Right, RigRight);
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

    [PunRPC]
    public void takeDamage(int damg)
    {
        if (photonView.IsMine)
        {
            NetworkManager.instance.LocalTakeDamage(damg);
            Debug.Log("Take Damage");
            TakeDamageEventMethod();
        }
    }
}
