using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ControllerInfo
{
    public float VelocityCameraAngle;
    public float PositionCameraAngle;

    public float PosCamAngleChange;
    public float PosCamAngleSecond;

    public float CameraDistance;
    public float CamDistanceChange;
    public float CamDistanceSecond;

    //public float 
    public List<float> PastAngles;
}
public class ControllerStats : MonoBehaviour
{
    public static ControllerStats instance;
    void Awake() { instance = this; }
    HandMagic HM;
    NewMagicCheck NM;
    public List<ControllerInfo> Controllers;
    public int Power;
    public int PastFrames;
    public ControllerInfo stats(int Side)
    {
        ControllerInfo Stats = new ControllerInfo();
        HandActions Hand = HM.Controllers[Side];

        Vector3 CamRot = MovementProvider.instance.transform.GetChild(0).GetChild(1).forward;
        Vector3 CamPos = MovementProvider.instance.transform.GetChild(0).GetChild(1).position;

        Vector3 LevelVel = new Vector3(Hand.Velocity.x, 0, Hand.Velocity.z);
        Vector3 LevelCamRot = new Vector3(CamRot.x, 0, CamRot.z);
        Vector3 LevelCamPos = new Vector3(CamPos.x, 0, CamPos.z);
        Vector3 LevelHandPos = new Vector3(Hand.transform.position.x, 0, Hand.transform.position.z);

        Stats.VelocityCameraAngle = Vector3.Angle(LevelVel, LevelCamRot);

        //angle
        Vector3 targetDir = LevelCamPos - LevelHandPos;
        Stats.PositionCameraAngle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up) + 180;
        Stats.PosCamAngleSecond = (Stats.PositionCameraAngle - Controllers[Side].PastAngles[Controllers[Side].PastAngles.Count - 1]) / Time.deltaTime;
        Stats.PosCamAngleChange = (Stats.PositionCameraAngle - Controllers[Side].PastAngles[Controllers[Side].PastAngles.Count - 1]);
        Stats.PastAngles = Controllers[Side].PastAngles;
        //distance
        Stats.CameraDistance = Vector3.Distance(LevelHandPos, LevelCamPos);
        //if (Controllers[Side].CameraDistance != 0)
            //Stats.PastCamDistance = Controllers[Side].CameraDistance;
       // Stats.CamDistanceSecond = (Stats.CameraDistance - Stats.PastCamDistance) / Time.deltaTime;
        //Stats.CamDistanceChange = (Stats.CameraDistance - Stats.PastCamDistance);

        return Stats;
    }

    
    void Start()
    {
        HM = HandMagic.instance;
        NM = NewMagicCheck.instance;
        //for (var i = 0; i < HM.Controllers.Count; i++)
            //LastFrameSave(i);
    }
    private void FixedUpdate()
    {
        for (var i = 0; i < HM.Controllers.Count; i++)
            LastFrameSave(i);
    }
    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < HM.Controllers.Count; i++)
            Controllers[i] = stats(i);
    }

    public void LastFrameSave(int j)
    {
        for (int i = 0; i < Controllers[j].PastAngles.Count; i++)
        {
            //if list not full
            if (Controllers[j].PastAngles.Count < PastFrames)
            {
                Controllers[j].PastAngles.Add(0);
                return;
            }
            //if count is 0
            int Count = Controllers[j].PastAngles.Count - i - 1;
            if (Count == 0)
                Controllers[j].PastAngles[0] = Controllers[j].PositionCameraAngle;
            else
                Controllers[j].PastAngles[Count] = Controllers[j].PastAngles[Count - 1];
        }
    }

    public float Average(List<float> list)
    {
        float average = 0;
        for (int i = 0; i < list.Count; i++)
            average += list[i];
        return average / list.Count;
    }

    public Direction ControllerDir(Vector3 Vel, int Side)
    {
        float Angle = Controllers[Side].VelocityCameraAngle;
        if (Vel.magnitude > NM.DirectionForceThreshold)
        {
            if (Angle > 180 - NM.DirectionLeaniency)
                return Direction.Towards;
            else if (Angle < 0 + NM.DirectionLeaniency && Angle > 0 - NM.DirectionLeaniency)
                return Direction.Away;
            else if (Angle < 90 + NM.DirectionLeaniency && Angle > 90 - NM.DirectionLeaniency)
                return Direction.Side;
        }
        return Direction.None;
    }
}
