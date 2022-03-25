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
    public VisualEffect fire;
    private bool Active = false;
    private Vector3 Normalized;
    //public Transform Target;

    public List<SendInfo> Times = new List<SendInfo>();

    public List<Transform> Targets;

    private float Spread;

    public static float CheckInterval = 0.1f;

    public static int StayDamage = 1;
    public static float WaitTime = 1f;

    public List<DamageInfo> DamageCooldowns = new List<DamageInfo>();

    private SpellCasts SC;

    public class SendInfo
    {
        public Transform Target;
        
        public float Timer;
        public float Time;

        public Vector3 SentPos;
        public Vector3 SentRot;
    }
    public class DamageInfo
    {
        public Transform Target;
        public float Time;
    }
    private void Start()
    {
        //fire.Stop();
        StartCoroutine(Wait());

        //StartFire();
        Active = true;

        Spread = fire.GetFloat("Spread") * 180;

        SC = HandMagic.instance.transform.GetComponent<SpellCasts>();
    }
    public bool IsCooldown(Transform hitAttempt)
    {
        for (int i = 0; i < DamageCooldowns.Count; i++)
        {
            if (DamageCooldowns[i].Target == hitAttempt)
            {
                return true;
            }
        }
        return false;
    }
    public List<Transform> CheckForTargets()
    {
        List<Transform> TrueColliders = new List<Transform>();
        Collider[] Colliders = Physics.OverlapSphere(transform.position, BlockMimic.CheckDistance);
        //Debug.Log(Colliders.Length);
        for (int i = 0; i < Colliders.Length; i++)
        {
            /*if (Colliders[i].transform.tag == "VRPerson")
                TrueColliders.Add(Colliders[i].transform);*/

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
        fire.SetVector3("Angle", transform.rotation.eulerAngles);
        CheckArriveTimes();
        if (Targets.Count < 0)
            return;
        for (int i = 0; i < Targets.Count; i++)
        {
            Vector3 EnemyAngle = Targets[i].position - transform.position;
            float BetweenAngle = Vector3.Angle(transform.forward, EnemyAngle);
            //Debug.Log("angle: " + BetweenAngle);

            float Speed = fire.GetFloat("Speed");
            float Distance = Vector3.Distance(Targets[i].position, transform.position);
            float Travel = Distance / Speed;
            if (Spread > BetweenAngle)
            {
                if (Active == true)
                {
                    SendInfo ToAdd = new SendInfo();
                    ToAdd.Time = Travel;
                    ToAdd.SentPos = transform.position;
                    ToAdd.SentRot = transform.TransformDirection(Vector3.forward);
                    ToAdd.Target = Targets[i];
                    Times.Add(ToAdd);
                }
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
                    if (ShieldBlocking(out GameObject shield) == true)
                    {
                        //if my shields
                        
                        if (IsCooldown(shield.transform) == false)
                        {
                            //create cooldown
                            DamageInfo NewTime = new DamageInfo();
                            NewTime.Time = WaitTime;
                            NewTime.Target = shield.transform;
                            DamageCooldowns.Add(NewTime);

                            //do damage
                            if (ShieldSide(shield) != 2)
                                SC.ShieldDamage(StayDamage, ShieldSide(shield));
                        }
                    }
                    else if(RaycastWorks() == true && AngleWorks() == true)
                    {
                        // damage player
                        if (IsCooldown(Times[i].Target) == false)
                        {
                            //create cooldown
                            DamageInfo NewTime = new DamageInfo();
                            NewTime.Time = WaitTime;
                            NewTime.Target = Times[i].Target;
                            DamageCooldowns.Add(NewTime);

                            //do damage
                            int oldHealth = GetPlayerInt(PlayerHealth, PhotonNetwork.LocalPlayer);
                            int newHealth = oldHealth - StayDamage;
                            SetPlayerInt(PlayerHealth, newHealth, PhotonNetwork.LocalPlayer);
                        }
                    }
                    
                    Times.Remove(Times[i]);
                    bool RaycastWorks()
                    {
                        RaycastHit hit;
                        Vector3 Direction = Times[i].SentPos - Times[i].Target.position;
                        int WallCollision = (1 << 14) | (1 << 8) | (1 << 9);
                        if (!Physics.Raycast(Times[i].SentPos, Direction, out hit, Mathf.Infinity, WallCollision))
                            return true;
                        else
                            return false;
                    }
                    bool AngleWorks()
                    {
                        Vector3 EnemyDirection = Times[i].SentPos - Times[i].Target.position;
                        float BetweenAngle = Vector3.Angle(Times[i].SentRot, EnemyDirection);
                        if (BetweenAngle > Spread)
                            return true;
                        else
                            return false;
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
    
    public void StartFire()
    {
        fire.Play();
        Active = true;
    }
    public void StopFire()
    {
        fire.Stop();
        Active = false;
        StartCoroutine(DestroyWait());
        
    }
    public IEnumerator DestroyWait()
    {
        yield return new WaitForSeconds(2);
        SC.RemoveFireball(gameObject);
    }

}
