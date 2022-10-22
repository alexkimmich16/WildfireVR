using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using static Odin.Net;
using Photon.Pun;
using Photon.Realtime;
public enum FireDetectType
{
    Colliders = 0,
    CodeDetect = 1,
}

public class FireController : MonoBehaviour
{
    private bool Active = false;
    public static float CheckInterval = 0.1f;
    //public bool CanCast;
    public bool ShouldDebug;
    public bool SpawnOnline;
    public List<Transform> DebugSpheres;

    [Header("DamageStats")]
    public FireDetectType DamageType;
    public float Spread;
    public float EnemyCooldownTime;
    public int Damage;
    private float SpawnTimer;
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

    private GameObject OnlineFire;
    public GameObject PrivateFire;

    [Header("Frames")]
    public Frames frames;
    
    
    private float CastTimer;

    [Header("Lists")]
    public List<ShardInfo> Shards;
    public List<Transform> Targets;
    public static List<CooldownInfo> DamageCooldowns;

    public delegate void NewState(bool State);
    public event NewState RealNewState;

    public List<FlameObject> ActiveFires;

    public float BlockForce;

    private Side side;
    
    public static bool IsCooldown(Transform hitAttempt)
    {
        for (int i = 0; i < DamageCooldowns.Count; i++)
            if (DamageCooldowns[i].Target == hitAttempt)
                return true;
        return false;
    }
    public List<Transform> CheckForTargets()
    {
        List<Transform> TrueColliders = new List<Transform>();
        Collider[] Colliders = Physics.OverlapSphere(AIMagicControl.instance.PositionObjectives[(int)side].transform.position, TargetCheckDistance);
        for (int i = 0; i < Colliders.Length; i++)
            if (Colliders[i].gameObject.GetComponent<PhotonView>())
                if (Colliders[i].gameObject.layer == LayerMask.NameToLayer("PlayerSee") && Colliders[i].gameObject.GetComponent<PhotonView>().IsMine == false)
                    TrueColliders.Add(Colliders[i].transform);
        return TrueColliders;
    }
    public IEnumerator Wait()
    {
        while (false == false)
        {
            //Debug.Log("check");
            Targets = CheckForTargets();
            yield return new WaitForSeconds(CheckInterval);
        }
    }
    public static void DamageShardHit(GameObject Hit)
    {
        if (IsCooldown(Hit.transform.parent))
            return;
        Debug.Log("doDamage");
        PhotonView EnemyPhoton = Hit.transform.parent.GetComponent<PhotonView>();
        if (EnemyPhoton.IsMine)
            return;
        //if blocking
        if (GetPlayerBool(Blocking, EnemyPhoton.Owner))
        {
            AIMagicControl.instance.PushAllFires(Hit.transform.parent.position);
            return;
        }
        EnemyPhoton.RPC("takeDamage", RpcTarget.All, 2);

        CooldownInfo NewTime = new CooldownInfo();
        NewTime.Target = Hit.transform.parent;
        DamageCooldowns.Add(NewTime);
    }
    #region StartStop
    public void StopFire()
    {
        if (PrivateFire == null) return;

        //Index += 1;
        if (SpawnOnline)
        {
            OnlineFire.GetPhotonView().RPC("SetFlames", RpcTarget.Others, false);
        }
        PrivateFire.GetComponent<FlameObject>().SetFlamesOffline(false);
        PrivateFire.GetComponent<PhotonDestroy>().StartCountdown();
        EyeController.instance.ChangeEyes(Eyes.Fire);

        OnlineFire = null;
        PrivateFire = null;
    }
    public void StartFire()
    {
        if (InGameManager.instance.CanDoMagic() == false || frames.CanCast == false)
            return;
        EyeController.instance.ChangeEyes(Eyes.Fire);
        //Debug.Log("Index: " + Index + "  Start");
        //Index += 1;

        
        PrivateFire = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(Spell.Flames, false)), Vector3.zero, Camera.main.transform.rotation);
        PrivateFire.GetComponent<FlameObject>().SetFlamesOffline(true);
        ActiveFires.Add(PrivateFire.GetComponent<FlameObject>());
        PrivateFire.GetComponent<FlameObject>().FlameParent = this;
        NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, Spell.Flames);
        if (SpawnOnline)
        {
            OnlineFire = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(Spell.Flames, true), Vector3.zero, Camera.main.transform.rotation);
            OnlineFire.GetPhotonView().RPC("SetFlames", RpcTarget.Others, true);
            OnlineFire.transform.SetParent(transform);
            OnlineFire.SetActive(false);
        }
    }
    public bool FrameWorks()
    {
        float Speed = AIMagicControl.instance.Hands[(int)side].transform.GetComponent<HandActions>().Magnitude;
        return frames.FramesWork() && Speed > MinVelocity;
    }
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
    #endregion

    
    private void Start()
    {
        StartCoroutine(Wait());
        gameObject.GetComponent<LearningAgent>().NewState += frames.AddToList;
        gameObject.GetComponent<LearningAgent>().NewState += OnNewState;
        side = GetComponent<LearningAgent>().side;
        DamageCooldowns = new List<CooldownInfo>();
    }
    public void ManageEnemyCooldown()
    {
        for (int i = 0; i < DamageCooldowns.Count; i++)
        {
            DamageCooldowns[i].Time += Time.deltaTime;
            if (DamageCooldowns[i].Time > EnemyCooldownTime)
                DamageCooldowns.Remove(DamageCooldowns[i]);
        }
    }
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
        CastTimer += Time.deltaTime;
        if(side == Side.Right)
            ManageEnemyCooldown();
        //SpawnFireCode();
        DoDebugSpheres();
        //if (Targets.Count < -1 || PrivateFire == null)
        if (PrivateFire == null)
            return;
        if (OnlineFire && SpawnOnline)
        {
            OnlineFire.transform.position = AIMagicControl.instance.PositionObjectives[(int)side].position;
            OnlineFire.transform.rotation = Camera.main.transform.rotation;
        }
        //Debug.Log("made");
        PrivateFire.transform.position = AIMagicControl.instance.PositionObjectives[(int)side].position;

        
        Vector3 Dir = AIMagicControl.instance.PositionObjectives[(int)side].transform.position - AIMagicControl.instance.Cam.position;
        PrivateFire.transform.rotation = Quaternion.LookRotation(Dir);
    }
    void CheckArriveTimes()
    {
        foreach (ShardInfo shard in Shards)
        {
            shard.Timer += Time.deltaTime;
            shard.CurrentPos = shard.SentRot * ShardSpeed * Time.deltaTime;
            for (int j = 0; j < Targets.Count; j++)
            {
                if (Vector3.Distance(shard.CurrentPos, Targets[j].position) < CollisionDistance)
                {
                    DamageShardHit(Targets[j].gameObject);
                    Shards.Remove(shard);
                    continue;
                }
            }
            if (shard.Timer > ShardLifetime)
                Shards.Remove(shard);
        }
    }
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