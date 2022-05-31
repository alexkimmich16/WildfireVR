using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region classes
public enum Direction
{
    Away = 0,
    Towards = 1,
    Side = 2,
    None = 3,
}
public enum HookType
{
    Active = 0,
    None = 1,
}

public enum FireBallCastType
{
    HandDirection = 0,
    HandMotion = 1,
}


[System.Serializable]
public class FireBallBallInfo
{
    public string Name;
    public List<bool> Checks;
    public int Current()
    {
        for (var i = 0; i < Checks.Count; i++)
            if (Checks[i] == false)
                return i;
        return Checks.Count;
    }
    public float Timer;
    public Direction dir;
    public Vector3 SavedDir;
    public float Magnitude;
    public float Angle;
    public Vector3 Velocity;
}
[System.Serializable]
public class TorchInfo
{
    public string Name;
    public HookType type;
    public bool Last = false;
    public GameObject FlameObject;
    public List<bool> Checks;
    public bool AbleToSpawn = true;
    public int Current()
    {
        for (var i = 0; i < Checks.Count; i++)
            if (Checks[i] == false)
                return i;
        return Checks.Count;
    }
}
#endregion
public class NewMagicCheck : MonoBehaviour
{
    public static NewMagicCheck instance;
    void Awake() { instance = this; }

    HandMagic HM;
    SpellCasts SC;
    [Header("FireBallStats")]
    public List<FireBallBallInfo> FireBall;
    public float PunchSpeedThreshold, StopSpeedThreshold;
    //public Vector2 AngleMinMax;
    public Vector3 FireballOffset;
    public float DirectionLeaniency;
    public float DirectionForceThreshold;
    public float TimeMax1, TimeMax2;
    public FireBallCastType type;
    public bool UseFireBall;
    

    [Header("TorchStats")]
    public List<TorchInfo> Torch;
    public float HookSpeedMin;
    public bool UseTorch;
    void Start()
    {
        HM = HandMagic.instance;
        SC = SpellCasts.instance;
    }
    
    void Update()
    {
        if(UseFireBall)
            ManageFireBall();
        if(UseTorch)
            ManageTorch();
    }

    

    public void ManageFireBall()
    {
        for (var i = 0; i < FireBall.Count; i++)
        {
            FireBall[i].dir = ControllerDir(HM.Controllers[i].Velocity, i);
            FireBall[i].Velocity = HM.Controllers[i].Velocity;
            FireBall[i].Magnitude = HM.Controllers[i].Magnitude;
            //Magnitude
            if (FireBall[i].Current() == 0)
            {
                if (HM.Controllers[i].Magnitude > PunchSpeedThreshold && FireBall[i].dir == Direction.Away)
                {
                    FireBall[i].Checks[0] = true;
                    FireBall[i].Timer = 0;
                    Vector3 VelDirection = HM.Controllers[i].PastFrames[0] - HM.Controllers[i].PastFrames[HandActions.PastFrameCount - 1];
                    VelDirection = VelDirection.normalized;
                    FireBall[i].SavedDir = VelDirection;
                }
            }
            else if (FireBall[i].Current() == 1)
            {
                FireBall[i].Timer += Time.deltaTime;
                if (HM.Controllers[i].Magnitude < StopSpeedThreshold && FireBall[i].dir == Direction.None)
                {
                    if (type == FireBallCastType.HandDirection)
                        SC.FireballCast(i, HM.Controllers[i].transform.rotation.eulerAngles + FireballOffset);
                    else if(type == FireBallCastType.HandMotion)
                        SC.FireballCast(i, FireBall[i].SavedDir);
                    Reset();
                }
                else if (FireBall[i].Timer > TimeMax2)
                {
                    Reset();
                }
            }

            void Reset()
            {
                for (var j = 0; j < FireBall[i].Checks.Count; j++)
                    FireBall[i].Checks[j] = false;
                FireBall[i].Timer = 0;
                FireBall[i].SavedDir = Vector3.zero;
            }
        }
    }
    public void ManageTorch()
    {
        //hook
        //1) hand moving around player at velocity min
        //2) when above speed play FireBall
        //3) when slower than min restart
        //shoot FireBall perpentiduclar to player hand

        float Angle = 0;
        for (var i = 0; i < Torch.Count; i++)
        {
            if (Torch[i].AbleToSpawn == true)
            {
                if (i == 1)
                {
                    Torch[i].type = Hook(HM.Controllers[i].Velocity, out float angle);
                    Angle = angle;
                }
                if (HM.Controllers[i].Magnitude < HookSpeedMin)
                    Reset();

                if (Torch[i].Current() == 0)
                {
                    if (HM.Controllers[i].Magnitude > HookSpeedMin && Angle < 50)
                    {
                        if (Torch[i].Last == false)
                        {
                            Set();
                            Torch[i].Last = true;
                        }

                        Torch[i].Checks[0] = true;
                    }
                }
                else if (Torch[i].Current() == 1)
                {
                    if (HM.Controllers[i].Magnitude > HookSpeedMin && Angle < 50)
                    {
                        Torch[i].Checks[1] = true;
                    }
                    else if (Angle > 50)
                    {
                        Reset();
                    }
                }
            }

            void Set()
            {
                SpellCasts.instance.ToggleTorch(i, true);
            }
            void Reset()
            {
                for (var j = 0; j < Torch[i].Checks.Count; j++)
                    Torch[i].Checks[j] = false;

                if (Torch[i].Last == true)
                {
                    SpellCasts.instance.ToggleTorch(i, false);
                    Torch[i].Last = false;
                }

            }
        }
    }


    public Direction ControllerDir(Vector3 Vel, int Side)
    {
        //Vector3 targetDir = Target.transform.position - transform.position;
        ///angleToTarget = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
        //angleToTarget += 180;

        Vector3 NewVel = new Vector3(Vel.x, 0, Vel.z);
        Vector3 CamPos = MovementProvider.instance.transform.GetChild(0).GetChild(1).position;
        Vector3 CamRot = MovementProvider.instance.transform.GetChild(0).GetChild(1).forward;
        Vector3 TrueCamRot = new Vector3(CamRot.x, 0, CamRot.z); 
        Vector3 TrueCamPos = new Vector3(CamPos.x, 0, CamPos.z);
        Vector3 TrueController = new Vector3(HM.Controllers[Side].transform.position.x, 0, HM.Controllers[Side].transform.position.z);
        Vector3 CameraToController = (TrueCamPos - TrueController);

        float Angle = Vector3.Angle(NewVel, TrueCamRot);
        FireBall[Side].Angle = Angle;

        float AngleLeanience = DirectionLeaniency;
        if(Vel.magnitude > DirectionForceThreshold)
        {
            if (Angle > 180 - AngleLeanience)
            {
                return Direction.Towards;
            }
            else if (Angle < 0 + AngleLeanience && Angle > 0 - AngleLeanience)
            {
                return Direction.Away;
            }
            else if (Angle < 90 + AngleLeanience && Angle > 90 - AngleLeanience)
            {
                return Direction.Side; 
            }
        }
        return Direction.None;


    }
    public HookType Hook(Vector3 Vel, out float Angle)
    {
        Vector3 Cam = MovementProvider.instance.transform.GetChild(0).GetChild(1).forward;
        Vector3 Camleft = new Vector3(Cam.x, Cam.y - 90, Cam.z);

        Vector3 UsedCam = Cam;
        Vector3 UsedVel = Vel;

        Angle = Vector3.Angle(UsedVel, UsedCam);
        float Threshold = 0.2f;
        //if (HM.Controllers[1].TriggerPressed())
        //Debug.Log("Vel: " + UsedVel.ToString("f3") + "  localEulerAngles: " + UsedCam + "  Angle: " + Angle);
        //Debug.Log("Dir: " + Cam + "  NewDir: " + Camleft);
        bool Rot = Angle < 50 && UsedVel.x > 0 || Angle < 130 && Angle > 50 && UsedVel.x < 0;
        if (Rot == true && UsedVel.magnitude > Threshold)
        {
            return HookType.Active;
        }
        else
        {
            return HookType.None;
        }

    }
}