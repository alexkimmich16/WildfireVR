using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Odin.Net;
using Photon.Pun;
using Photon.Realtime;
public enum Movements
{
    Spike = 0,
    Fireball = 1,
    Shield = 2,
    Push = 3,
    Telekinetic = 4,
    Flight = 5,
    Slice = 6,
}
public enum SpellType
{
    Individual = 0,
    Both = 1,
}
public enum Side
{
    Left = 0,
    Right = 1,
}
public class AIMagicControl : MonoBehaviour
{
    public NewControllerInfo Info;

    public List<Transform> PositionObjectives;
    public List<Transform> Hands;
    public List<Transform> Spawn;

    public Rigidbody PlayerRB;
    public Transform Cam;
    public Transform CamOffset;
    public Transform MyCharacterDisplay;
    public SpellContainer spells;
    
    public static AIMagicControl instance;

    public bool PlayerInHeadset;

    public List<FireController> Flames;
    public List<FireballController> Fireballs;
    public List<FireAbsorb> Absorbs;
    public List<BlockController> Blocks;

    public float DirectionLeaniency;
    public float DirectionForceThreshold;


    void Awake() { instance = this; }
    public HandActions GetHandActions(Side side)
    {
        return Hands[(int)side].GetComponent<HandActions>();
    }
    public Direction ControllerDir(Vector3 Vel, Side side)
    {
        float Angle = GetHandActions(side).VelocityCameraAngle();

        if (Vel.magnitude > DirectionForceThreshold)
        {
            if (Angle > 180 - DirectionLeaniency)
                return Direction.Towards;
            else if (Angle < 0 + DirectionLeaniency && Angle > 0 - DirectionLeaniency)
                return Direction.Away;
            else if (Angle < 90 + DirectionLeaniency && Angle > 90 - DirectionLeaniency)
                return Direction.Side;
        }
        return Direction.None;
    }

    public bool IsBlocking()
    {
        return Blocks[0].Active == true && Blocks[1].Active == true;
    }
    public void PushAllFires(Vector3 Pos)
    {
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < Flames[i].ActiveFires.Count; j++)
                Flames[i].ActiveFires[j].FlameCol.PushFire(Pos, 2);
    }
    private void Update()
    {
        if (InGameManager.instance.KeypadTesting)
        {
            //Debug.Log("First");
            if (Input.GetKey(KeyCode.D))
            {
                //Debug.Log("call");
                FirePillar.CallStartFire(Spell.Flames);
            }
        }
        SetPlayerBool(Blocking, IsBlocking(), PhotonNetwork.LocalPlayer);
        /*
        if(PlayerInHeadset == true)
        {

        }
        */
    }
}

[System.Serializable]
public class NewControllerInfo
{

    public List<Transform> TestMain;
    public List<Transform> TestCam;
    public List<Transform> TestHand;

    HandActions MyHand(Side side)
    {
        if (side == Side.Right)
            return LearnManager.instance.Right;
        else
            return LearnManager.instance.Left;
    }
    public SingleInfo GetControllerInfo(Side side)
    {
        Transform Cam = LearnManager.instance.Cam;
        SetReferences();
        Vector3 CamPos = Cam.localPosition;
        TestCam[(int)side].position = Vector3.zero;
        TestHand[(int)side].position = TestHand[(int)side].position - CamPos;

        float YDifference = -Cam.localRotation.eulerAngles.y;

        //invert main to y distance
        if (side == Side.Left)
        {
            TestMain[(int)side].localScale = new Vector3(-1, 1, 1);
            Vector3 Rot = TestCam[(int)side].eulerAngles;
            TestCam[(int)side].eulerAngles = new Vector3(Rot.x, -Rot.y, -Rot.z);
        }

        TestMain[(int)side].rotation = Quaternion.Euler(0, YDifference, 0);
        //TestCam[(int)side].localRotation = Cam.localRotation;
        return ReturnTest();


        SingleInfo ReturnTest()
        {
            SingleInfo newInfo = new SingleInfo();
            newInfo.HeadPos = TestCam[(int)side].position;
            newInfo.HeadRot = TestCam[(int)side].rotation.eulerAngles;
            newInfo.HandPos = TestHand[(int)side].position;
            newInfo.HandRot = TestHand[(int)side].rotation.eulerAngles;
            return newInfo;
        }
        void SetReferences()
        {
            TestMain[(int)side].position = Vector3.zero;
            TestMain[(int)side].rotation = Quaternion.identity;
            TestMain[(int)side].localScale = new Vector3(1, 1, 1);
            SetEqual(Cam, TestCam[(int)side]);
            SetEqual(MyHand(side).transform, TestHand[(int)side]);
            void SetEqual(Transform Info, Transform Set)
            {
                Set.localPosition = Info.localPosition;
                Set.localRotation = Info.localRotation;
            }
        }
    }
}
