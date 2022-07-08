using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SingleInfo
{
    public Vector3 HeadPos, HeadRot, HandPos, HandRot, HandVel;
    public Vector2 StartEnd;
    public bool Works;
    //if x = y than no time
}
[System.Serializable]
public class Motion
{
    public List<SingleInfo> Infos;

    //public bool IsWorkingMotion;
}
public class RecordMotions : MonoBehaviour
{
    public int FramesPerSecond;
    private float FrameInterval;
    public List<Motion> Motions;
    private HandMagic HM;

    public Motion currentMotion;
    private bool RecordingMotion;
    public bool ShouldRecord;

    private float Timer;
    void Start()
    {
        HM = HandMagic.instance;
    }

    public bool ButtonsPressed()
    {
        return HM.Controllers[1].TriggerPressed() && HM.Controllers[1].GripPressed();
    }

    void Update()
    {
        FrameInterval = 1 / FramesPerSecond;
        if (ShouldRecord == false)
            return;
        if (RecordingMotion == false && ButtonsPressed() == true)
        {
            RecordingMotion = true;
        }
        else if (RecordingMotion == true && ButtonsPressed() == false)
        {
            RecordingMotion = false;
            Motion FinalMotion = new Motion();
            FinalMotion.Infos = new List<SingleInfo>(currentMotion.Infos);
            Motions.Add(FinalMotion);
            currentMotion.Infos.Clear();
        }

        if (RecordingMotion == false)
            return;
        Timer += Time.deltaTime;

        if (Timer < FrameInterval)
            return;
        Timer = 0;

        //better way to always do at the same time
        SingleInfo newInfo = new SingleInfo();
        newInfo.HeadPos = HM.Cam.position;
        newInfo.HeadRot = HM.Cam.transform.localRotation.eulerAngles;

        newInfo.HandPos = HM.Controllers[1].transform.position;
        newInfo.HandRot = HM.Controllers[1].transform.localRotation.eulerAngles;
        newInfo.HandVel = HM.Controllers[1].Velocity;

        //newInfo
        currentMotion.Infos.Add(newInfo);




        //Motions[Motions.Count ]
        //Info.Add(newInfo);
        /*
        Vector3 ZPlacementController = new Vector3(HM.Controllers[1].transform.position.x, HM.Cam.position.y, HM.Controllers[1].transform.position.z);
        float YChange = HM.Controllers[1].transform.position.y - HM.Cam.position.y;
        float Dist = Vector3.Distance(ZPlacementController, HM.Cam.position);
        Vector3 targetDir = ZPlacementController + HM.Cam.transform.position;
        float angle = Vector3.Angle(targetDir, HM.Cam.transform.forward);
        RightLocalPos.Add(new Vector3(Dist, angle + 180, YChange));
        //LeftLocalPos.Add(new Vector3(Dist, -angle + 180, YChange));
        //LeftRotation.Add(new Vector3(HM.Controllers[0].transform.rotation.x, HM.Controllers[0].transform.rotation.y, HM.Controllers[0].transform.rotation.z));
        RightRotation.Add(new Vector3(HM.Controllers[1].transform.rotation.x, HM.Controllers[1].transform.rotation.y, HM.Controllers[1].transform.rotation.z));
        */
    }
}
