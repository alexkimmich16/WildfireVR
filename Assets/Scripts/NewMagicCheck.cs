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
    public bool IsActive = false;
    public bool OnCooldown = false;

    public bool YPos;
    public bool Speed;
    public bool CamDistance;

    public GameObject FlameObject;
    public FireController fireControl;
    public List<bool> Checks;
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
    ControllerStats CS;
    
    [Header("FireBallStats")]
    public List<FireBallBallInfo> FireBall;
    public float PunchSpeedThreshold, StopSpeedThreshold;
    public Vector3 FireballOffset;
    public float DirectionLeaniency;
    public float DirectionForceThreshold;
    public float TimeMax1, TimeMax2;
    public FireBallCastType type;
    public bool UseFireBall;

    [Header("TorchStats")]
    public List<TorchInfo> Torch;
    public Vector2 AngleChangeLimits;
    public Vector2 DistanceLimit;
    public float YPosLeanience;
    public bool UseTorch;
    void Start()
    {
        HM = HandMagic.instance;
        SC = SpellCasts.instance;
        CS = ControllerStats.instance;
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
            FireBall[i].dir = CS.ControllerDir(HM.Controllers[i].Velocity, i);
            FireBall[i].Velocity = HM.Controllers[i].Velocity;
            FireBall[i].Magnitude = HM.Controllers[i].Magnitude;
            //Magnitude
            if (FireBall[i].Current() == 0)
            {
                if (HM.Controllers[i].Magnitude > PunchSpeedThreshold && FireBall[i].dir == Direction.Away)
                {
                    FireBall[i].Checks[0] = true;
                    FireBall[i].Timer = 0;
                    FireBall[i].SavedDir = (HM.Controllers[i].PastFrames[0] - HM.Controllers[i].PastFrames[HandActions.PastFrameCount - 1]).normalized;
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
        for (var i = 0; i < Torch.Count; i++)
        {
            Torch[i].Speed = Speed();
            Torch[i].YPos = LevelWithHead();
            Torch[i].CamDistance = CamDis();
            bool Active = LevelWithHead() && Speed() && CamDis();

            if (Active == true && Torch[i].IsActive == false && Torch[i].OnCooldown == false)
            {
                Torch[i].IsActive = true;
                SpellCasts.instance.ToggleTorch(i, true);
            }
            else if (Active == false && Torch[i].IsActive == true)
            {
                Torch[i].IsActive = false;
                SpellCasts.instance.ToggleTorch(i, false);
            }

            bool LevelWithHead()
            {
                Vector3 Controller = HM.Controllers[i].transform.position;
                Vector3 Cam = MovementProvider.instance.transform.GetChild(0).GetChild(1).position;
                float Difference = Mathf.Abs(Controller.y - Cam.y);
                return Difference < YPosLeanience;
            }

            bool Speed()
            {
                float AngleChange = Mathf.Abs(CS.Controllers[i].PosCamAngleChange);
                return AngleChange > AngleChangeLimits.x && AngleChange < AngleChangeLimits.y;
            }

            bool CamDis()
            {
                float Distance = CS.Controllers[i].CameraDistance;
                return Distance > DistanceLimit.x && Distance < DistanceLimit.y;
            }
        }
    }
    
    
}