using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class FireAbsorb : MonoBehaviour
{
    public static FireAbsorb instance;
    void Awake() { instance = this; }
   
    [Header("Stats")]

    public Side side;
    public bool CanCast;
    [Range(0,1)]
    public float FireDecaySpeed;
    public float AbsorbThreshold;
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
    
    public void OnAbsorbStart(Transform Fireball)
    {
        Debug.Log("works: " + Fireball.name);
        Fireball.GetComponent<Fireball>().SetAbsorbed(true);
        Fire = Fireball.GetComponent<Rigidbody>();
        Fireball.GetComponent<PhotonView>().RPC("ChangeSpeed", RpcTarget.All, AbsorbStartSpeed);
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
