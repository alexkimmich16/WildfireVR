using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMagicControl : MonoBehaviour
{
    public NewControllerInfo Info;

    public List<Transform> PositionObjectives;
    public List<Transform> Hands;
    public List<Transform> Spawn;
    public Transform Cam;
    public SpellContainer spells;
    
    public static AIMagicControl instance;

    public bool PlayerInHeadset;
    void Awake() { instance = this; }

    private void Update()
    {
        if(InGameManager.instance.KeypadTesting)
            if (Input.GetKey(KeyCode.D))
            {
                Debug.Log("call");
                FirePillar.CallStartFire(Spell.Flames);
            }
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
