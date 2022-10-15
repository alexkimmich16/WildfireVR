using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class FireAbsorb : MonoBehaviour
{
    public static FireAbsorb instance;
    void Awake() { instance = this; }
    public Side side;
    public bool CanCast;
    public Rigidbody Fire;

    [Range(0,1)]
    public float FireDecaySpeed;
    public float AbsorbThreshold;

    [Range(0, 360)]
    public float MaxAngle;
    public Vector3 HandRotOffset;

    public bool FireballControl;
    [Header("Testing")]
    public bool TestBubble = false;
    public GameObject BubbleDisplay;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if (other.GetComponent<Fireball>())
        {
            Debug.Log("works: " + other.name);
            other.GetComponent<Fireball>().SetAbsorbed(true);
            DetectAbsorbStart(other.GetComponent<Rigidbody>());
        }
    }

    public void DetectAbsorbStart(Rigidbody Fireball)
    {
        Fire = Fireball;
        //Debug.Log(Vector3.Angle(Fireball.velocity.normalized, Hand.transform.eulerAngles));

        ///hand is facing fireball
        ///
        /*
        if (Vector3.Angle(Fireball.velocity.normalized, Hand.transform.eulerAngles + HandRotOffset) < MaxAngle)
        {
            Fire = Fireball;
            //face 
        }
        else
        {
            FailedAbsorb();
        }
        */
    }

    void Update()
    {
        if(BubbleDisplay != null)
            BubbleDisplay.SetActive(TestBubble);

        if (Fire == null || CanCast == false)
            return;
        if(FireballControl == false)
        {
            float HandSpeed = AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().Velocity.magnitude;
            float FireSpeed = Fire.velocity.magnitude;
            //Debug.Log("HandSpeed: " + HandSpeed + "  FireSpeed: " + FireSpeed);
            if (FireSpeed - HandSpeed < AbsorbThreshold)
            {
                //float ReduceSpeedWeight = (FireSpeed - HandSpeed) * FireDecaySpeed;

                Fire.velocity = Fire.velocity * FireDecaySpeed;
                //reduce speed
                //match forward
                if(FireSpeed < 0.0001f)
                {
                    FireballControl = true;
                }
            }
            else
            {
                FailedAbsorb();
            }
        }
        if(FireballControl == true)
        {
            Fire.transform.position = AIMagicControl.instance.PositionObjectives[(int)side].position;

            //gameObject.GetComponent<LearningAgent>().NewState += ReDirectFireball();

            ///adjust threshold later
            ///
            /*
            if (Vector3.Distance(LearnManager.instance.Left.transform.position, Fire.transform.position) < 0.001f)
            {
                StopHoldingFireball();
                AbsorbFireball();
            }
            */
            

            //set position

            //check for speed(redirect)
            //check for distance to player(absorb) or hands together

            //float OtherHandDistance =  = 
            //if()

            //hold time
        }
    }
    public void StopHoldingFireball()
    {
        //stop emitting THAN delete in future
        PhotonNetwork.Destroy(Fire.gameObject);
        Fire = null;
        FireballControl = false;
    }
    void AbsorbFireball()
    {
        //destroy
        //heal magic
        //heal health
        //destory partical effect
    }

    public void FailedAbsorb()
    {
        NetworkManager.instance.LocalTakeDamage(1);
        PhotonNetwork.Destroy(Fire.gameObject);
        Fire = null;
        //explode
    }
}
