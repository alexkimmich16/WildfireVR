using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using static Odin.Net;
using Photon.Pun;
using Photon.Realtime;

public class FireController : MonoBehaviour
{
    public static FireController instance;
    void Awake() { instance = this; }

    [Header("Info")]
    public bool Active = false;

    public List<SendInfo> Times = new List<SendInfo>();

    public List<Transform> Targets;

    public static float CheckInterval = 0.1f;

    public static int StayDamage = 1;
    public static float WaitTime = 1f;

    public float Speed;
    public float Spread;
    public float MaxDistance;
    public List<CooldownInfo> DamageCooldowns = new List<CooldownInfo>();
    //public int ConsecutiveFrames;
    //public List<bool> PastFrames;
    //private static int Capacity = 10;

    [Header("VFX")]
    //public VisualEffect Fire;
    public float PlayRateSpeed;

    public GameObject PositionObjective;

    private GameObject CurrentFire;
    public GameObject FirePrefab;

    //public float DestoryInTime;

    [Header("CubeTest")]
    private SpellCasts SC;
    private bool CubeTest;
    private Transform TestCube;

    public static bool DebugDamageShard;
    public GameObject DamageShard;

    private float Timer;
    public float Interval = 0.03f;
    public float ShardSpeed;
    [Header("Frames")]
    public Frames frames;

    //private float FireStart
    public void StopFire()
    {
        if (CurrentFire == null)
            return;
        CurrentFire.GetPhotonView().RPC("SetFlames", RpcTarget.All, false, PlayRateSpeed);
        EyeController.instance.ChangeEyes(Eyes.Fire);
        //Destroy(CurrentFire, DestoryInTime);
        CurrentFire = null;
    }
    public void StartFire()
    {
        CurrentFire = PhotonNetwork.Instantiate("RealFlames", Vector3.zero, Camera.main.transform.rotation);
        CurrentFire.GetPhotonView().RPC("SetFlames", RpcTarget.All, true, PlayRateSpeed);

        CurrentFire.transform.SetParent(transform);
        EyeController.instance.ChangeEyes(Eyes.Fire);

        NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, Spell.Flames);
    }
    public void OnNewState(bool State)
    {
        // hasn't started yet InGameManager.MagicBeforeStart &&
        if (InGameManager.CanCast == false)
            return;

        if(Active == false && frames.AllPastFrames(true))
        {
            Active = true;
            StartFire();
        }
        else if(Active == true && frames.AllPastFrames(false))
        {
            Active = false;
            StopFire();
        }
    }

    

    private void Start()
    {
        StartCoroutine(Wait());

        SC = HandMagic.instance.transform.GetComponent<SpellCasts>();
        gameObject.GetComponent<LearningAgent>().NewState += frames.AddToList;
        gameObject.GetComponent<LearningAgent>().NewState += OnNewState;
    }
    public bool IsCooldown(Transform hitAttempt)
    {
        for (int i = 0; i < DamageCooldowns.Count; i++)
            if (DamageCooldowns[i].Target == hitAttempt)
                return true;
        return false;
    }
    public List<Transform> CheckForTargets()
    {
        List<Transform> TrueColliders = new List<Transform>();
        Collider[] Colliders = Physics.OverlapSphere(PositionObjective.transform.position, MaxDistance);
        //Debug.Log("colliders: " + Colliders.Length);
        for (int i = 0; i < Colliders.Length; i++)
            if (Colliders[i].gameObject.GetComponent<PhotonView>())
                if (Colliders[i].gameObject.layer == LayerMask.NameToLayer("PlayerSee") && Colliders[i].gameObject.GetComponent<PhotonView>().IsMine == false)
                    TrueColliders.Add(Colliders[i].transform);
        return TrueColliders;
    }
    public IEnumerator Wait()
    {
        while(false == false)
        {
            //Debug.Log("check");
            Targets = CheckForTargets();
            yield return new WaitForSeconds(CheckInterval);
        }
    }
    public void DamageShardHit(GameObject Hit)
    {
        if (IsCooldown(Hit.transform))
            return;
        PhotonView EnemyPhoton = Hit.GetComponent<PhotonView>();
        EnemyPhoton.RPC("takeDamage", RpcTarget.All, EnemyPhoton.Owner.ActorNumber, 1);

        CooldownInfo NewTime = new CooldownInfo();
        NewTime.Time = WaitTime;
        NewTime.Target = Hit.transform;
        DamageCooldowns.Add(NewTime);
    }
    private void Update()
    {
        CheckArriveTimes();
        for (int i = 0; i < DamageCooldowns.Count; i++)
        {
            DamageCooldowns[i].Time -= Time.deltaTime;
            if (DamageCooldowns[i].Time < 0)
            {
                DamageCooldowns.Remove(DamageCooldowns[i]);
            }
        }
        if (Active)
        {
            float RandomRad = Random.Range(0, 360) * Mathf.Deg2Rad;
            float x = Mathf.Cos(RandomRad);
            float y = Mathf.Sin(RandomRad);
            Vector3 NewRot = new Vector3(x, y, 0) * Random.Range(0.1f, Spread);
            GameObject shard = Instantiate(DamageShard, PositionObjective.transform.position, Camera.main.transform.rotation);
            shard.transform.eulerAngles += NewRot;
            shard.GetComponent<DamageShard>().Speed = ShardSpeed;
            Timer = 0;
            Timer += Time.deltaTime;
            if (Timer > Interval)
            {
                
            }
        }
        if (Targets.Count < -1 || CurrentFire == null)
            return;



        CurrentFire.transform.position = PositionObjective.transform.position;
        CurrentFire.transform.rotation = Camera.main.transform.rotation;

        ManageTest();

        
        for (int i = 0; i < Targets.Count; i++)
        {
            Vector3 RealEnemyPos = new Vector3(Targets[i].position.x, Targets[i].position.y + 1.5f, Targets[i].position.z);
            Vector3 EnemyAngle = RealEnemyPos - CurrentFire.transform.position;
            float BetweenAngle = Vector3.Angle(Camera.main.transform.forward, EnemyAngle);

            float Distance = Vector3.Distance(Targets[i].position, CurrentFire.transform.position);
            float Travel = Distance / Speed;

            /*
            SendInfo ToAdd = new SendInfo();
            
            ToAdd.Time = Travel;
            ToAdd.SentPos = CurrentFire.transform.position;
            ToAdd.SentRot = CurrentFire.transform.TransformDirection(Vector3.forward);
            ToAdd.Target = Targets[i];
            Times.Add(ToAdd);
            
            
            //Debug.Log(BetweenAngle);
            if (Spread > BetweenAngle)
            {
                if (Active == true)
                {
                    SendInfo ToAdd = new SendInfo();
                    //Debug.Log("Created");
                    ToAdd.Time = Travel;
                    ToAdd.SentPos = CurrentFire.transform.position;
                    ToAdd.SentRot = CurrentFire.transform.TransformDirection(Vector3.forward);
                    ToAdd.Target = Targets[i];
                    Times.Add(ToAdd);
                }
            }
            */
        }

        
        void ManageTest()
        {
            if (CubeTest == true && this == NewMagicCheck.instance.Torch[1].fireControl)
            {
                float BlockTargetAngle = Vector3.Angle(transform.forward, TestCube.position - transform.position);
                if (Spread > BlockTargetAngle)
                    TestCube.GetComponent<MeshRenderer>().material.color = Color.red;
                else
                    TestCube.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
        

        void CheckArriveTimes()
        {
            for (int i = 0; i < Times.Count; i++)
            {
                Times[i].Timer += Time.deltaTime;
                if (Times[i].Timer > Times[i].Time)
                {
                    //on hit
                    //give priority to shields in front
                    /*
                    if (ShieldBlocking(out GameObject shield))
                    {
                        //if my shields
                        
                        if (IsCooldown(shield.transform) == false)
                        {
                            //create cooldown
                            CooldownInfo NewTime = new CooldownInfo();
                            NewTime.Time = WaitTime;
                            NewTime.Target = shield.transform;
                            DamageCooldowns.Add(NewTime);

                            //do damage
                            if (ShieldSide(shield) != 2)
                                SC.ShieldDamage(StayDamage, ShieldSide(shield));
                        }
                    }
                    */
                    //Debug.Log("Damage1");
                    if(RaycastWorks(i) && AngleWorks(i))
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
    }

    
}
public class SendInfo
{
    public Transform Target;

    public float Timer;
    public float Time;

    public Vector3 SentPos;
    public Vector3 SentRot;
}
public class CooldownInfo
{
    public Transform Target;
    public float Time;
}