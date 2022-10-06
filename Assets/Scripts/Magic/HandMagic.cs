using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Odin.MagicHelp;
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
#region classes
[System.Serializable]
public class MagicInfo
{
    public string Name;
    public SpellType Type;
    public FinalMovement FinalInfo;
    public float Cost;
    public float Leanience;
    public List<bool> Finished = new List<bool>();
    public bool Active;
    //public Vector3 Leanience;
    public float Time;
    public List<BaseControllerInfo> Controllers = new List<BaseControllerInfo>();

    public List<GameObject> Sides = new List<GameObject>();
    public float RotLeanience;
}
//Spells[i].Controllers[j].RotDifference
[System.Serializable]
public class BaseControllerInfo
{
    //public Side side;
    public int Current;
    public float RotDifference;
    public float Distance;
    public List<bool> ControllerFinished = new List<bool>();
    public float Time;
    public GameObject Trail;
}
#endregion
public class HandMagic : MonoBehaviour
{
    #region Singleton + classes
    public static HandMagic instance;
    void Awake()
    {
        if (InfoSave.Changeable == true)
        {
            if ((int)priority > (int)HighestPriority)
            {
                HighestPriority = priority;
                instance = this;
            }
            else
                Destroy(this);
        }
    }
    
    #endregion

    public Priority priority;
    public static Priority HighestPriority = Priority.None;
    [HideInInspector]
    public List<HandActions> Controllers = new List<HandActions>();

    [HideInInspector]
    public Transform Cam;
    [HideInInspector]
    public SpellCasts SC;

    [Range(0f, 1f)]
    public float TriggerThreshold, GripThreshold;
    [Header("Magic")]
    public bool ShouldCharge;
    public bool InfiniteMagic;
    public float CurrentMagic;
    public float MagicRecharge;
    public int MaxMagic;
    
    [Header("Misc")]
    
    public List<Collider> AroundColliders = new List<Collider>();

    public float UnusedResetTime;
    
    [Header("Spike")]
    public float SpikeTimeDelete;
    public float YRise;
    private bool UseSpikePlacement = false;

    [Header("Fireball")]
    public float Speed;

    [Header("Shield")]
    public int MaxShield;

    [Header("ForcePush")]
    public float PushAmount;
    public float PushRadius;
    public float AngleMax;

    [Header("Telekinesis")]

    [Header("Flight")]
    public float FlightPower;

    [Header("Slash")]
    public float SlashSize;

    private static bool Rickroll = false;
    public static bool Respawn = true;
    public static bool UseMaxTime = true;
    public static bool LoadOnly3 = true;

    [Header("Other")]
    public List<MagicInfo> Spells = new List<MagicInfo>();
    //public Transform empty;
    [HideInInspector]
    public Rigidbody RB;

    public List<Material> Materials = new List<Material>(0);

    //public Vector3 Offset;
    public float SpellCheckTime;
    private float SpellCheckTimer;

    public void ChangeTrail(Movements type, bool Set, Side side)
    {
        if (Spells[(int)type].Controllers[(int)side].Trail != null)
        {
            if (Spells[(int)type].Type == SpellType.Both)
            {
                Spells[(int)type].Controllers[0].Trail.SetActive(Set);
                Spells[(int)type].Controllers[1].Trail.SetActive(Set);
            }
            else if (Spells[(int)type].Type == SpellType.Individual)
            {
                Spells[(int)type].Controllers[(int)side].Trail.SetActive(Set);
            }
        }
        
    }
    public void BothSpellManager()
    {
        for (int i = 0; i < Spells.Count; i++)
        {
            if(Spells[i].Active == true)
            {
                if (Spells[i].Type == SpellType.Both)
                {
                    //only right use
                    FinalMovement info = Spells[i].FinalInfo;
                    if (Spells[i].Controllers[0].Current != info.Frames)
                    {
                        for (int j = 0; j < Spells[i].Sides.Count; j++)
                        {
                            int Current = Spells[i].Controllers[j].Current;
                            if (info.Frames > 1)
                            {
                                if (RotationWorks(Controllers[j].LocalRotation) == true && DistanceWorks(Controllers[j].transform.position) == true)
                                    Spells[i].Controllers[j].Current += 1;
                                else
                                {
                                    DistanceWorks(Controllers[j].transform.position);
                                    Spells[i].Controllers[0].Current = 0;
                                    Spells[i].Controllers[1].Current = 0;
                                }


                                if (Current > info.Frames - 1)
                                {
                                    Current = info.Frames - 1;
                                }
                                Vector3 Converted = ConvertDataToPoint(GetLocalPosSide(j,i,Current));
                                float distance = Vector3.Distance(Converted, Controllers[j].transform.position);

                                if (Spells[i].Leanience > distance)
                                    Spells[i].Controllers[j].Current += 1;
                            }
                            bool DistanceWorks(Vector3 ControllerPos)
                            {
                                //Debug.Log("i: " + i + "  SideNum: " + SideNum);
                                Vector3 Converted = GetLocalPosSide(j, i, Current);
                                float distance = Vector3.Distance(Converted, ControllerPos);
                                Spells[i].Controllers[j].Distance = distance;

                                if (Spells[i].Leanience > distance)
                                    return true;
                                else
                                    return false;
                            }
                            bool RotationWorks(Vector3 MyRotation)
                            {
                                Vector3 ObjectiveRotation = GetRotationSide(j, i, Current);
                                if (info.RotationLock[0] != Vector2.zero)
                                {
                                    //check for limit reached
                                    if (MyRotation.x > Spells[i].FinalInfo.RotationLock[0].x && MyRotation.x < Spells[i].FinalInfo.RotationLock[0].y)
                                    {
                                        ObjectiveRotation.x = MyRotation.x;
                                    }
                                    else
                                        return false;
                                }
                                if (info.RotationLock[1] != Vector2.zero)
                                {
                                    //check for limit reached
                                    if (MyRotation.y > Spells[i].FinalInfo.RotationLock[1].x && MyRotation.y < Spells[i].FinalInfo.RotationLock[1].y)
                                    {
                                        ObjectiveRotation.y = MyRotation.y;
                                    }
                                    else
                                        return false;
                                }
                                if (info.RotationLock[2] != Vector2.zero)
                                {
                                    //check for limit reached
                                    if (MyRotation.z > Spells[i].FinalInfo.RotationLock[2].x && MyRotation.z < Spells[i].FinalInfo.RotationLock[2].y)
                                    {
                                        ObjectiveRotation.z = MyRotation.z;
                                    }
                                    else
                                        return false;
                                }
                                Quaternion a = Quaternion.Euler(ObjectiveRotation);
                                Quaternion b = Quaternion.Euler(MyRotation);
                                float angle = Quaternion.Angle(a, b);

                                Spells[i].Controllers[j].RotDifference = angle;
                                if (Spells[i].RotLeanience > angle)
                                    return true;
                                else
                                    return false;

                                //Debug.Log("i: " + i + "  SideNum: " + SideNum);

                            }
                        }
                    }
                    else
                    {
                        if (Spells[i].Finished[0] == false)
                            Behaviour(i, 0, 0);
                        Spells[i].Finished[0] = true;

                    }

                    //both animation finished, and both trigger pressed
                    if (Spells[i].Finished[0] == true && Controllers[0].TriggerPressed() == true && Controllers[1].TriggerPressed())
                    {
                        Spells[i].Finished[1] = true;
                        Behaviour(i, 1, 0);
                    }

                    //all of last, and both triggers released
                    if (Spells[i].Finished[1] == true && Controllers[0].TriggerPressed() == false && Controllers[1].TriggerPressed() == false)
                    {
                        Behaviour(i, 2, 0);
                        Spells[i].Finished[0] = false;
                        Spells[i].Finished[1] = false;

                        Spells[i].Controllers[0].Current = 0;
                        Spells[i].Controllers[1].Current = 0;
                    }
                }

                
            }
        }
    }
    public void Behaviour(int Spell, int Part, int Side)
    {
        if (CurrentMagic - Spells[Spell].Cost < 0)
        {
            return;
        }
        Movements move = (Movements)Spell;
        if (move == Movements.Spike)
        {
            if (Part == 0)
            {
                ChangeTrail(move, true, (Side)Side);
            }
            else if (Part == 1)
            {
                //pressed
                //if UseSpikePlacement, change bool true that is changed false on complete
            }
            else if (Part == 2)
            {
                ChangeMagic(-Spells[Spell].Cost);
                SC.UseSpike(RaycastGround());
                ChangeTrail(move, false, (Side)Side);
            }
        }
        else if (move == Movements.Fireball)
        {
            if (Part == 0)
            {
                //ChangeTrail(move, true, (Side)Side);
            }
            else if (Part == 1)
            {
                //SC.FireballCharge(Side);
                SC.FireballStart(Side);
            }
            else if (Part == 2)
            {

                //SC.StartFireballEnd(Side);
                //SC.FireballShoot(Side);
                //ChangeTrail(move, false, (Side)Side);
            }
        }
        else if (move == Movements.Shield)
        {
            if (Part == 0)
            {
                ChangeTrail(move, true, (Side)Side);
            }
            else if (Part == 1)
            {
                //pressedCost
                ChangeTrail(move, false, (Side)Side);
                ChangeMagic(-Spells[Spell].Cost);
                SC.StartShield(Side);
            }
            else if (Part == 2)
            {
                SC.EndShield(Side);
            }
        }
        else if (move == Movements.Push)
        {
            if (Part == 0)
            {
                ChangeTrail(move, true, (Side)Side);
            }
            else if (Part == 1)
            {
                //pressed
            }
            else if (Part == 2)
            {
                ChangeMagic(-Spells[Spell].Cost);
                SC.UseForcePush();
                ChangeTrail(move, false, (Side)Side);
            }
        }
        else if (move == Movements.Telekinetic)
        {
            if (Part == 0)
            {
                ChangeTrail(move, true, (Side)Side);
            }
            else if (Part == 1)
            {
                SC.SetTelekinesisActive(Side, true);
                ChangeMagic(-Spells[Spell].Cost);
            }
            else if (Part == 2)
            {
                SC.SetTelekinesisActive(Side, false);
                ChangeTrail(move, false, (Side)Side);
            }
        }
        else if (move == Movements.Flight)
        {
            if (Part == 0)
            {
                ChangeTrail(move, true, (Side)Side);
            }
            else if (Part == 1)
            {
                SC.SetFlyingActive(Side, true);
                ChangeTrail(move, false, (Side)Side);
            }
            else if (Part == 2)
            {
                SC.SetFlyingActive(Side, false);
            }
        }
        else if (move == Movements.Slice)
        {
            if (Part == 0)
            {
                ChangeTrail(move, true, (Side)Side);
            }
            else if (Part == 1)
            {
                SC.SetSlashingActive(Side, true);
            }
            else if (Part == 2)
            {
                SC.SetSlashingActive(Side, false);
                ChangeTrail(move, false, (Side)Side);
            }
        }
    }
    public void FollowMotion()
    {
        for (int i = 0; i < Spells.Count; i++)
        {
            if (Spells[i].Active == true)
            {
                SpellType type = Spells[i].Type;
                if (type == SpellType.Both)
                {
                    int Current = Mathf.Min(Spells[i].Controllers[0].Current, Spells[i].Controllers[1].Current);
                    if (Current > Spells[i].FinalInfo.Frames - 1)
                        Current -= 1;
                    for (int j = 0; j < Spells[i].Sides.Count; j++)
                    {
                        Spells[i].Sides[j].transform.position = GetLocalPosSide(j, i, Current);
                        Vector3 Rotation = GetRotationSide(j, i, Current);
                        Rotation.y = Rotation.y + Camera.main.transform.eulerAngles.y;
                        Spells[i].Sides[j].transform.rotation = Quaternion.Euler(Rotation);
                    }
                }
                else if (type == SpellType.Individual)
                {
                    for (int j = 0; j < Spells[i].Sides.Count; j++)
                    {
                        int Current = Spells[i].Controllers[j].Current;
                        if (Current > Spells[i].FinalInfo.Frames - 1)
                            Current -= 1;
                        Spells[i].Sides[j].transform.position = GetLocalPosSide(j, i, Current);
                        Vector3 Rotation = GetRotationSide(j, i, Current);
                        Rotation.y = Rotation.y + Camera.main.transform.eulerAngles.y;
                        Spells[i].Sides[j].transform.eulerAngles = Rotation;
                    }
                }
            }
        }
    }
    public void CheckUnused()
    {
        for (int i = 0; i < Spells.Count; i++)
        {
            if (Spells[i].Type == SpellType.Both)
            {
                if(Spells[i].Finished[1] == true)
                {
                    if (Spells[i].Time > UnusedResetTime)
                    {
                        Spells[i].Time = 0;
                        ResetWithoutMotion((Movements)i, 0);
                    }
                    else
                        Spells[i].Time = 0;
                    Spells[i].Time += Time.deltaTime;
                }
            }
            else if (Spells[i].Type == SpellType.Individual)
            {
                for (int j = 0; j < Spells[i].Controllers.Count; j++)
                {
                    if (Spells[i].Finished[1] == true)
                    {
                        if (Spells[i].Controllers[j].Time > UnusedResetTime)
                        {
                            Spells[i].Controllers[j].Time = 0;
                            ResetWithoutMotion((Movements)i, (Side)j);
                        }
                        Spells[i].Controllers[j].Time += Time.deltaTime;
                    }
                    else
                        Spells[i].Controllers[j].Time = 0;
                }
            }
        }
    }
    public void ResetWithoutMotion(Movements move, Side side)
    {
        SpellType type = Spells[(int)move].Type;
        if (type == SpellType.Individual)
        {
            Spells[(int)move].Controllers[(int)side].Current = 0;
            Spells[(int)move].Controllers[(int)side].ControllerFinished[0] = false;
            Spells[(int)move].Controllers[(int)side].ControllerFinished[1] = false;
        }
        else if (type == SpellType.Both)
        {
            Spells[(int)move].Finished[0] = false;
            Spells[(int)move].Finished[1] = false;
            Spells[(int)move].Controllers[0].Current = 0;
            Spells[(int)move].Controllers[1].Current = 0;
        }
    }
    void Start()
    {
        SC = transform.GetComponent<SpellCasts>();
        Cam = GameObject.Find("XR Rig/Camera Offset/Main Camera").transform;
        RB = GameObject.Find("XR Rig").GetComponent<Rigidbody>();
        Controllers.Clear();
        Controllers.Add(GameObject.Find("Camera Offset/LeftHand Controller").GetComponent<HandActions>());
        Controllers.Add(GameObject.Find("Camera Offset/RightHand Controller").GetComponent<HandActions>());

        CurrentMagic = MaxMagic;
        if (Rickroll == true)
        {
            OpenURL();
        }
        

        //initialise trails
        for (int i = 0; i < Spells.Count; i++)
        {
            ChangeTrail((Movements)i, false, (Side)0);
            ChangeTrail((Movements)i, false, (Side)1);
        }

        if (SceneLoader.BattleScene() == true)
            EnableCubes(false);
    }
    void Update()
    {
        if (SceneLoader.BattleScene() == true)
        {
            if (InGameManager.instance.CanDoMagic() == true)
            {
                BothSpellManager();
                FollowMotion();
            }
        }
        else
        {
            BothSpellManager();
            FollowMotion();
            EnableCubes(true);
        }

        if (ShouldCharge == true)
            ChangeMagic(MagicRecharge * ZoneController.instance.Multiplier(MovementProvider.instance.gameObject));

        if (UseMaxTime == true)
            CheckUnused();
    }
    public void ChangeMagic(float BaseChange)
    {
        if (InfiniteMagic == true)
        {
            return;
        }
        //Debug.Log(BaseChange);
        float Positive = Mathf.Abs(BaseChange);
        if (Mathf.Sign(BaseChange) == 1)
        {
            //add: if potential magic after adding is bigger than the max
            if (CurrentMagic + Positive >= MaxMagic)
            {
                CurrentMagic = MaxMagic;
            }
            else
            {
                //Debug.Log(BaseChange);
                CurrentMagic += Positive;
            }
        }
        else if (Mathf.Sign(BaseChange) == -1)
        {
            //subtract: if potential magic after subtracting is smaller than 0
            if (CurrentMagic - Positive <= 0)
            {
                CurrentMagic = 0;
            }
            else
            {
                CurrentMagic -= Positive;
            }
        }
    }
    public Vector3 RaycastGround()
    {
        RaycastHit hit;
        int layerMask = 1 << 8;
        if (Physics.Raycast(Cam.position, Cam.forward, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 HitSpot = hit.point;
            HitSpot = new Vector3(HitSpot.x, HitSpot.y + YRise, HitSpot.z);
            return hit.point;
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            return Vector3.positiveInfinity;
        }
    }
    public void OpenURL()
    {
        string URL = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley";
        Application.OpenURL(URL);
        Application.OpenURL(URL);
        Application.OpenURL(URL);
        Application.OpenURL(URL);
        Application.OpenURL(URL);
    }
    public void EnableCubes(bool State)
    {
        for (int i = 0; i < Spells.Count; i++)
        {
            //set active if
            if(Spells[i].Sides[0] != null)
            {
                Spells[i].Sides[0].SetActive(State);
                Spells[i].Sides[1].SetActive(State);
            }
            
            //if setting true and spell is inactive, don't set at all
            if (State == true || Spells[i].Active == false)
            {
                //Spells[i].Sides[0].SetActive(false);
                //Spells[i].Sides[1].SetActive(false);
            }
        }
    }
}
