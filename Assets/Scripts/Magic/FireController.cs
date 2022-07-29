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
    [Header("Info")]
    public bool Active = false;

    public List<SendInfo> Times = new List<SendInfo>();

    public List<Transform> Targets;

    public static float CheckInterval = 0.1f;

    public static int StayDamage = 1;
    public static float WaitTime = 1f;

    public float Speed;
    public float Spread;
    public List<CooldownInfo> DamageCooldowns = new List<CooldownInfo>();

    [Header("Test")]
    private SpellCasts SC;

    private bool CubeTest;

    private Transform TestCube;

    [Header("VFX")]
    public VisualEffect Fire;
    public float PlayRateSpeed;

    public GameObject PositionObjective;
    public void OnNewState(bool State)
    {
        //Debug.Log("NewState: " + State);
        if(State == true && Active == false)
        {
            Active = true;
            Fire.Play();
            Fire.enabled = true;
        }
        else if(State == false && Active == true)
        {
            Active = false;
            Fire.Stop();
            Fire.enabled = false;
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

    private void Start()
    {
        StartCoroutine(Wait());

        Fire.Stop();

        SC = HandMagic.instance.transform.GetComponent<SpellCasts>();

        gameObject.GetComponent<LearningAgent>().NewState += OnNewState;

        Fire.playRate = PlayRateSpeed;
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
        Collider[] Colliders = Physics.OverlapSphere(transform.position, BlockMimic.CheckDistance);
        for (int i = 0; i < Colliders.Length; i++)
        {
            if (Colliders[i].transform.tag == "Shield")
                TrueColliders.Add(Colliders[i].transform);
            if(Colliders[i].transform.tag == "Player")
                TrueColliders.Add(Colliders[i].transform);
        }
            return TrueColliders;
    }
    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(CheckInterval);
        Targets = CheckForTargets();
        StartCoroutine(Wait());
    }
    private void Update()
    {
        CheckArriveTimes();
        if (Targets.Count < 0)
            return;

        transform.position = PositionObjective.transform.position;

        ManageTest();

        for (int i = 0; i < Targets.Count; i++)
        {
            Vector3 EnemyAngle = Targets[i].position - transform.position;
            float BetweenAngle = Vector3.Angle(transform.forward, EnemyAngle);

            float Distance = Vector3.Distance(Targets[i].position, transform.position);
            float Travel = Distance / Speed;
            
            if (Spread > BetweenAngle)
            {
                if (Active == true)
                {
                    SendInfo ToAdd = new SendInfo();
                    Debug.Log("Created");
                    ToAdd.Time = Travel;
                    ToAdd.SentPos = transform.position;
                    ToAdd.SentRot = transform.TransformDirection(Vector3.forward);
                    ToAdd.Target = Targets[i];
                    Times.Add(ToAdd);
                }
            }
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
                    else if(RaycastWorks(i) && AngleWorks(i))
                    {
                        if (IsCooldown(Times[i].Target) == false)
                        {
                            //create cooldown
                            if (Times[i].Target.GetComponent<DamageController>())
                            {
                                Times[i].Target.GetComponent<DamageController>().TakeDamage(1);
                            }
                            else if (Times[i].Target.GetComponent<DamageController>())
                            {

                            }
                            //
                            CooldownInfo NewTime = new CooldownInfo();
                            NewTime.Time = WaitTime;
                            NewTime.Target = Times[i].Target;
                            DamageCooldowns.Add(NewTime);
                            Debug.Log("damage");
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

            for (int i = 0; i < DamageCooldowns.Count; i++)
            {
                DamageCooldowns[i].Time -= Time.deltaTime;
                if (DamageCooldowns[i].Time < 0)
                {
                    DamageCooldowns.Remove(DamageCooldowns[i]);
                }
            }
        }
    }
}
