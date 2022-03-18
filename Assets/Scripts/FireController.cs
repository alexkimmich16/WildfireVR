using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

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

    public class SendInfo
    {
        public Transform Target;
        
        public float Timer;
        public float Time;

        public Vector3 SentPos;
        public Vector3 SentRot;
    }

    private void Start()
    {
        //fire.Stop();
        StartCoroutine(Wait());

        StartFire();
        Active = true;

        Spread = fire.GetFloat("Spread") * 180;
    }

    public List<Transform> CheckForTargets()
    {
        List<Transform> TrueColliders = new List<Transform>();
        Collider[] Colliders = Physics.OverlapSphere(transform.position, BlockMimic.CheckDistance);
        //Debug.Log(Colliders.Length);
        for (int i = 0; i < Colliders.Length; i++)
            if (Colliders[i].transform.tag == "VRPerson")
                TrueColliders.Add(Colliders[i].transform);
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
                    if(RaycastWorks() == true && AngleWorks() == true)
                    {
                        Debug.Log("Hit!!");
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
    }
}
