using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RestrictionSystem;
public class FireAbsorb : MonoBehaviour
{
    public static FireAbsorb instance;
    void Awake() { instance = this; }
   
    [Header("Stats")]

    public Side side;
    public bool CanCast;

    public float MaxAbsorbRot;
    public float FireballSlowSpeed, FireballRotateSpeed;

    public float AbsorbStartSpeed;

    [Range(0, 360)]
    public float MaxAngle;
    public Vector3 HandRotOffset;

    [Header("References")]
    public Rigidbody Fire;
    public bool FireballControl;

    [Header("Testing")]
    public bool TestBubble = false;
    public GameObject BubbleDisplay;

    public bool InvertSubtractOne;
    public bool InvertSubtractTwo;

    public float RotationDifference;

    public void OnAbsorbStart(Transform Fireball)
    {
        Debug.Log("works: " + Fireball.name);
        Fireball.GetComponent<FireballObject>().SetAbsorbed(true);
        Fire = Fireball.GetComponent<Rigidbody>();
        Fireball.GetComponent<PhotonView>().RPC("ChangeSpeed", RpcTarget.All, AbsorbStartSpeed);
    }

    public void OnAbsorbFinish()
    {
        Debug.Log("absorb");
        //fireball melts into hand
        StartCoroutine(HoldTimerCount());
    }

    void Update()
    {
        if(BubbleDisplay != null)
            BubbleDisplay.SetActive(TestBubble);

        if (Fire == null || CanCast == false)
            return;
        if(FireballControl == false)
        {
            Transform Hand = AIMagicControl.instance.Hands[(int)side];
            

            Vector3 Direction = (InvertSubtractOne) ? (Fire.transform.position - Hand.position).normalized : (Hand.position - Fire.transform.position).normalized;
            float RotDifference = Vector3.Angle(Direction, Hand.forward);
            bool RotWorks = RotDifference < MaxAbsorbRot;
            RotationDifference = RotDifference;

            if (!RotWorks)
            {
                FailedAbsorb();
                return;
            }

            //move to distance
            Fire.velocity = Fire.velocity * -FireballSlowSpeed * Time.deltaTime;

            //move to hand rotation
            float HandFireDistance = Vector3.Distance(Hand.position, Fire.transform.position);
            Vector3 ForwardAtDistance = Hand.forward * HandFireDistance;
            Vector3 NotAdjustedPoint = Vector3.MoveTowards(Fire.transform.position, ForwardAtDistance, FireballRotateSpeed * Time.deltaTime);
            Vector3 AdjustedPoint = (InvertSubtractTwo) ? (NotAdjustedPoint - Hand.position).normalized * HandFireDistance : (Hand.position - NotAdjustedPoint).normalized * HandFireDistance;
            Fire.transform.position = AdjustedPoint;

            //set towards hand
            Fire.transform.LookAt(Hand.position, Vector3.forward);

            if (Fire.velocity.magnitude < 0.0001f) //on absorb
            {
                FireballControl = true;
                OnAbsorbFinish();
            }
        }
    }
    public IEnumerator HoldTimerCount()
    {
        yield return new WaitForSeconds(2f);
        FailedAbsorb();
    }

    public void ResetHolding()
    {
        Fire = null;
        FireballControl = false;
    }
    public void FailedAbsorb()
    {
        NetworkManager.instance.LocalTakeDamage(5);
        PhotonNetwork.Destroy(Fire.gameObject);
        Fire = null;
        FireballControl = false;
        //explode
    }
}
