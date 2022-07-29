using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region classes
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
    public Vector3 SavedDir;
}
[System.Serializable]
public class TorchInfo
{
    public string Name;
    public bool IsActive = false;
    public bool OnCooldown = false;
    //[HideInInspector]
    public bool YPos, Speed, CamDistance, HandRot;

    public GameObject FlameObject;
    public FireController fireControl;
}
[System.Serializable]
public class ShieldInfo
{
    public string Name;
    public float CooldownTimer;
    public float ResetTimer;
    public List<bool> Checks;
    public bool OnCooldown;

    //[HideInInspector]
    public bool GoodRot, GoodPos, SlowRotAndVel, FastRotOrVel, StoppedRot;

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
    public float RotationLeanience;

    public static bool Active;

    [Header("Shield")]
    public List<ShieldInfo> Shields;
    
    public float ResetTime;
    public float Cooldown;
    public float YLeanience;
    public float RotLeanience;
    public bool UseShield;
    public Vector2 DistanceFromHead;
    void Start()
    {
        HM = HandMagic.instance;
        SC = SpellCasts.instance;
        CS = ControllerStats.instance;
    }
    void Update()
    {
        if (Active == false)
            return;
        if(UseFireBall)
            ManageFireBall();
        if(UseTorch)
            ManageTorch();
        if (UseShield)
            ManageShield();
    }
    public void ManageFireBall()
    {
        for (var i = 0; i < FireBall.Count; i++)
        {
            Direction dir = CS.ControllerDir(HM.Controllers[i].Velocity, i);
            //Magnitude
            if (FireBall[i].Current() == 0)
            {
                if (HM.Controllers[i].Magnitude > PunchSpeedThreshold && dir == Direction.Away)
                {
                    FireBall[i].Checks[0] = true;
                    FireBall[i].Timer = 0;
                    FireBall[i].SavedDir = (HM.Controllers[i].PastFrames[0] - HM.Controllers[i].PastFrames[HandActions.PastFrameCount - 1]).normalized;
                }
            }
            else if (FireBall[i].Current() == 1)
            {
                FireBall[i].Timer += Time.deltaTime;
                if (HM.Controllers[i].Magnitude < StopSpeedThreshold && dir == Direction.None)
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
            Torch[i].HandRot = HandRotation();
            bool Active = LevelWithHead() && Speed() && CamDis() && HandRotation();

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
            bool HandRotation()
            {
                float Leanience = RotationLeanience;
                float ZRot = HM.Controllers[i].transform.localEulerAngles.z;
                //350 - 360 || 0 - 10
                return ZRot > 360 - Leanience || ZRot < Leanience;
                //return true;
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
    public void ManageShield()
    {
        //motion is
        for (var i = 0; i < Shields.Count; i++)
        {
            AssignValues();
            if (Shields[i].OnCooldown == false)
            {
                if (Shields[i].Current() == 0)
                {
                    if (FastRotOrVel())
                    {
                        Shields[i].Checks[0] = true;
                        Shields[i].ResetTimer = 0;
                    }
                }
                else if (Shields[i].Current() == 1)
                {
                    Shields[i].ResetTimer += Time.deltaTime;

                    if (GoodRot() && GoodPos() && SlowRotAndVel() && StoppedRot())
                    {
                        Reset();
                        Debug.Log("Blast");
                        SC.ShieldBlast(HM.Controllers[i].transform.position);
                    }
                    else if (Shields[i].ResetTimer > ResetTime)
                    {
                        Reset();
                    }
                }
            }
            else
            {
                Shields[i].CooldownTimer += Time.deltaTime;
                if(Shields[i].CooldownTimer > Cooldown)
                {
                    Shields[i].OnCooldown = false;
                    Shields[i].CooldownTimer = 0;
                }
            }
            
            bool GoodRot()
            {
                float Leanience = RotationLeanience;
                float ZRot = HM.Controllers[i].transform.localEulerAngles.z;
                return ZRot > 0 || ZRot < Leanience;
            }
            bool GoodPos()
            {
                Vector3 Controller = HM.Controllers[i].transform.position;
                Vector3 Cam = MovementProvider.instance.transform.GetChild(0).GetChild(1).position;
                float Difference = Mathf.Abs(Controller.y - Cam.y);

                float Distance = CS.Controllers[i].CameraDistance;

                return Difference < YLeanience && Distance > DistanceFromHead.x && Distance < DistanceFromHead.y;
            }
            bool SlowRotAndVel()
            {
                float RotMag = 0;
                float DirMag = HM.Controllers[i].Magnitude * 1.2f;
                return RotMag + DirMag > 3;
            }
            bool FastRotOrVel()
            {
                return true;
            }
            bool StoppedRot()
            {
                return true;
            }
            void AssignValues()
            {
                Shields[i].GoodRot = GoodRot();
                Shields[i].GoodPos = GoodPos();
                Shields[i].SlowRotAndVel = SlowRotAndVel();
                Shields[i].FastRotOrVel = FastRotOrVel();
                Shields[i].StoppedRot = StoppedRot();
            }
            void Reset()
            {
                for (var j = 0; j < Shields[i].Checks.Count; j++)
                    Shields[i].Checks[j] = false;
                Shields[i].OnCooldown = true;
                Shields[i].CooldownTimer = Cooldown;
            }
            //check for fast rotation or speed
            //check for stop in speed and rotation of hand
            
        }
            
    }
    
}

public enum Direction
{
    Away = 0,
    Towards = 1,
    Side = 2,
    None = 3,
}
public enum FireBallCastType
{
    HandDirection = 0,
    HandMotion = 1,
}