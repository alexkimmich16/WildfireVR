using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region classes
public enum Direction
{
    Away = 0,
    Towards = 1,
    None = 2,
}
public enum HookType
{
    Active = 0,
    None = 1,
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
    public float VelThreshold;
    public Vector2 AngleMinMax;
    public float Threshold;
    public float TimeMax1, TimeMax2;

    [Header("TorchStats")]
    public List<TorchInfo> Torch;
    public float HookSpeedMin;
    //public float StopVelMin;
    //public float PullVelMax;
    //public float TimeMax1, TimeMax2;
    void Start()
    {
        HM = HandMagic.instance;
        SC = SpellCasts.instance;
    }
    public Direction ControllerDir(Vector3 Vel)
    {
        Vector3 NewVel = new Vector3(Vel.x, 0, Vel.z);
        Vector3 Dir = MovementProvider.instance.transform.GetChild(0).GetChild(1).forward;
        Vector3 NewDir = new Vector3(0, Dir.y, Dir.z);
        float Angle = Vector3.Angle(NewVel, NewDir);
        //if (HM.Controllers[1].TriggerPressed())
            //Debug.Log("Vel: " + Vel + "  localEulerAngles: " + Dir + "  Angle: " + Angle);
        if (Angle < AngleMinMax.x && Vel.magnitude > Threshold)
        {
            return Direction.Away;
        }
        else if (Angle > AngleMinMax.y && Vel.magnitude > Threshold)
        {
            return Direction.Towards;
        }
        else
        {
            return Direction.None;
        }

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
    void Update()
    {
        ManageFireBall();
        ManageTorch();
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

    public void ManageFireBall()
    {
        for (var i = 0; i < FireBall.Count; i++)
        {
            FireBall[i].dir = ControllerDir(HM.Controllers[i].Velocity);

            if (FireBall[i].Current() != 0)
            {
                FireBall[i].Timer += Time.deltaTime;
            }

            if (FireBall[i].Current() == 0)
            {
                if (HM.Controllers[i].Magnitude > VelThreshold && FireBall[i].dir == Direction.Away)
                {
                    FireBall[i].Checks[0] = true;
                    FireBall[i].Timer = 0;
                    //if (i == 1)
                        //Debug.Log(HM.Controllers[i].AverageDirection());
                    Vector3 VelDirection = HM.Controllers[i].PastFrames[0] - HM.Controllers[i].PastFrames[HandActions.PastFrameCount - 1];
                    VelDirection = VelDirection.normalized;
                    FireBall[i].SavedDir = VelDirection;
                }
            }
            else if (FireBall[i].Current() == 1)
            {
                if (HM.Controllers[i].Magnitude > VelThreshold && FireBall[i].dir == Direction.Towards)
                {
                    Debug.Log(FireBall[i].SavedDir);
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
}