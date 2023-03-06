using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using static Odin.Net;
using Photon.Pun;
using Photon.Realtime;
using RestrictionSystem;
public enum FireDetectType
{
    Colliders = 0,
    CodeDetect = 1,
}

public class FireController : MonoBehaviour
{
    public static FireController instance;
    private void Awake() { instance = this; }
    public List<bool> Actives = new List<bool>();
    public static float CheckInterval = 0.1f;
    //public bool CanCast;
    public bool ShouldDebug;
    public List<Transform> DebugSpheres;

    [Header("DamageStats")]
    public FireDetectType DamageType;
    public float Spread;
    public float EnemyCooldownTime;
    public int Damage;
    public float CastCooldowntime;
    public float MinVelocity;


    [Header("Colliders")]
    public float SpawnInterval = 0.03f;
    public float ShardSpeed;
    public float ShardLifetime;
    
    public GameObject DamageShard;

    [Header("CodeDetect")]
    public float TargetCheckDistance;
    public float CollisionDistance;

    [Header("VFX")]

    public List<GameObject> OnlineFire = new List<GameObject>();
    //public GameObject PrivateFire;

    [Header("Lists")]
    public List<ShardInfo> Shards = new List<ShardInfo>();
    public List<Transform> Targets = new List<Transform>();
    public List<CooldownInfo> DamageCooldowns = new List<CooldownInfo>();

    //public List<FlameObject> ActiveFires;

    public float BlockForce;

    public bool IsCooldown(Transform hitAttempt)
    {
        for (int i = 0; i < DamageCooldowns.Count; i++)
            if (DamageCooldowns[i].Target == hitAttempt)
                return true;
        return false;
    }

    #region StartStop
    public void StopFire(Side side)
    {
        
        //Index += 1;
        if (OnlineFire[(int)side] != null)
        {
            //OnlineFire[(int)side].GetComponent<FlameObject>().SetFlames(false);
            OnlineFire[(int)side].GetPhotonView().RPC("SetFlamesOnline", RpcTarget.All, false);
            OnlineFire[(int)side].GetComponent<PhotonDestroy>().StartCountdown();
            OnlineFire[(int)side] = null;
        }
        EyeController.instance.ChangeEyes(Eyes.Fire);
    }
    public void StartFire(Side side)
    {
        //Debug.Log(InGameManager.instance.CanDoMagic());
        if (InGameManager.instance.CanDoMagic() == false)
            return;
        //Debug.
        EyeController.instance.ChangeEyes(Eyes.Fire);

        
        NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, CurrentLearn.Flames);
        OnlineFire[(int)side] = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(CurrentLearn.Flames, 0), Vector3.zero, Camera.main.transform.rotation);
        OnlineFire[(int)side].name = "OnlineFire";

        //ActiveFires.Add(PrivateFire.GetComponent<FlameObject>());
        ///add to all fires list
    }
    /*
    public void OnNewState(bool State)
    {
        if (InGameManager.instance.CanDoMagic() == false || frames.CanCast == false)
            return;
        //Debug.Log("Newstate2");
        if (Active == false && FrameWorks() == true && CastTimer > CastCooldowntime)
        {
            //Debug.Log("Newstate3");
            if(RealNewState != null)
                RealNewState(true);
            CastTimer = 0f;
            Active = true;
            StartFire();
        }
        else if(Active == true && FrameWorks() == false)
        {
            //Debug.Log("Stop");
            if (RealNewState != null)
                RealNewState(false);
            Active = false;
            StopFire();
        }
    }
    */
    #endregion


    private void Start()
    {
        ConditionManager.instance.MotionConditions[(int)CurrentLearn.Flames - 1].OnNewState += RecieveNewState;
        //OnlineFire ;
        for (int i = 0; i < 2; i++)
            OnlineFire.Add(null);
        DamageCooldowns = new List<CooldownInfo>();
    }

    public void RecieveNewState(Side side, bool StartOrFinish, int Index)
    {
        Actives[(int)side] = StartOrFinish;
        Debug.Log("side: " + side + "  StartOrFinish: " + StartOrFinish);
        if (StartOrFinish)
        {
            StartFire(side);
        }
        else
        {
            StopFire(side);
        }
    }

    public void ManageEnemyCooldown()
    {
        if(DamageCooldowns.Count > 0)
            for (int i = 0; i < DamageCooldowns.Count; i++)
            {
                DamageCooldowns[i].Time += Time.deltaTime;
                if (DamageCooldowns[i].Time > EnemyCooldownTime)
                    DamageCooldowns.Remove(DamageCooldowns[i]);
            }
    }
    
    public void DoDebugSpheres()
    {
        if (ShouldDebug == false)
            return;
        for (int i = 0; i < Shards.Count; i++)
        {
            if (DebugSpheres.Count > i)
                DebugSpheres[i].position = Shards[i].CurrentPos;
            else
                DebugSpheres[i].position = new Vector3(1000, 1000, 1000);
        }
    }
    private void Update()
    {
        ManageEnemyCooldown();

        DoDebugSpheres();
        for (int i = 0; i < 2; i++)
        {
            Quaternion Rot = Quaternion.LookRotation(AIMagicControl.instance.PositionObjectives[i].transform.position - AIMagicControl.instance.Cam.position);
            Vector3 Pos = AIMagicControl.instance.PositionObjectives[i].position;
            if(OnlineFire[i] != null)
            {
                OnlineFire[i].transform.position = Pos;
                OnlineFire[i].transform.rotation = Rot;
            }
            
        }
            
    }
    /*
    public void SpawnFireCode()
    {
        if (!Active)
            return;

        SpawnTimer += Time.deltaTime;
        if (SpawnTimer < SpawnInterval)
            return;

        SpawnTimer = 0;
        if (DamageType == FireDetectType.Colliders)
        {
            float RandomRad = Random.Range(0, 360) * Mathf.Deg2Rad;
            float x = Mathf.Cos(RandomRad);
            float y = Mathf.Sin(RandomRad);
            Vector3 NewRot = new Vector3(x, y, 0) * Random.Range(0.1f, Spread);
            GameObject shard = Instantiate(DamageShard, AIMagicControl.instance.PositionObjectives[(int)side].position, Camera.main.transform.rotation);
            shard.transform.eulerAngles += NewRot;
            shard.GetComponent<DamageShard>().Speed = ShardSpeed;
        }
        else if (DamageType == FireDetectType.CodeDetect)
        {
            ShardInfo ToAdd = new ShardInfo();
            ToAdd.Timer = 0;
            ToAdd.CurrentPos = AIMagicControl.instance.PositionObjectives[(int)side].position;
            ToAdd.SentRot = AIMagicControl.instance.PositionObjectives[(int)side].TransformDirection(Vector3.forward);
            Shards.Add(ToAdd);
        }
    }
    */
    /*
    void OldArrive()
    {
        for (int i = 0; i < Times.Count; i++)
        {
            Times[i].Timer += Time.deltaTime;
            if (Times[i].Timer > Times[i].Time)
            {
                if (RaycastWorks(i) && AngleWorks(i))
                {
                    //Debug.Log("Damage2");
                    if (IsCooldown(Times[i].Target) == false)
                    {
                        //create cooldown
                        //Debug.Log("Damage3");
                        if (Times[i].Target.GetComponent<PhotonView>())
                        {
                            //Debug.Log("Damage4");
                            PhotonView EnemyPhoton = Times[i].Target.GetComponent<PhotonView>();
                            EnemyPhoton.RPC("takeDamage", RpcTarget.All, EnemyPhoton.Owner.ActorNumber, 1);
                            //Times[i].Target.GetComponent<DamageController>().TakeDamage(1);
                        }
                        //
                        CooldownInfo NewTime = new CooldownInfo();
                        NewTime.Time = WaitTime;
                        NewTime.Target = Times[i].Target;
                        DamageCooldowns.Add(NewTime);
                        //Debug.Log("damage5");
                        //do damage
                        //int oldHealth = GetPlayerInt(PlayerHealth, PhotonNetwork.LocalPlayer);
                        //int newHealth = oldHealth - StayDamage;

                        //SetPlayerInt(PlayerHealth, newHealth, PhotonNetwork.LocalPlayer);
                    }
                }

                Times.Remove(Times[i]);
                bool RaycastWorks(int i)
                {
                    RaycastHit hit;
                    Vector3 Direction = Times[i].SentPos - Times[i].Target.position;
                    int WallCollision = (1 << 14) | (1 << 8) | (1 << 9);
                    return !Physics.Raycast(Times[i].SentPos, Direction, out hit, Mathf.Infinity, WallCollision);
                }
                bool AngleWorks(int i)
                {
                    Vector3 EnemyDirection = Times[i].SentPos - Times[i].Target.position;
                    float BetweenAngle = Vector3.Angle(Times[i].SentRot, EnemyDirection);
                    return BetweenAngle > Spread;
                }
                bool ShieldBlocking(out GameObject shield)
                {
                    RaycastHit hit;
                    Vector3 Direction = Times[i].SentPos - Times[i].Target.position;
                    int WallCollision = 1 << 9;
                    int MagicWallCollision = 1 << 15;
                    if (Physics.Raycast(Times[i].SentPos, Direction, out hit, Mathf.Infinity, WallCollision))
                    {
                        shield = hit.transform.gameObject;
                        return true;
                    }
                    else if (Physics.Raycast(Times[i].SentPos, Direction, out hit, Mathf.Infinity, MagicWallCollision))
                    {
                        shield = null;
                        return false;
                    }
                    else
                    {
                        shield = null;
                        return false;
                    }

                }
                int ShieldSide(GameObject shield)
                {
                    for (int i = 0; i < 2; i++)
                        if (SC.Stats[i].Shield != null)
                            if (SC.Stats[i].Shield == shield)
                                return i;
                    return 2;
                }
            }
        }


    }
    */
}
[System.Serializable]
public class ShardInfo
{
    public float Timer;

    public Vector3 CurrentPos;
    public Vector3 SentRot;
}
[System.Serializable]
public class CooldownInfo
{
    public Transform Target;
    public float Time = 1;
}