using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum ControlType
{
    OnMoveHand = 0,
    Constant = 1,
    Directional = 2,
}
public class FireballController : MonoBehaviour
{
    //public bool Active;
    
    public bool SpawnOnline;
    public bool IsControlling;
    [Header("Stats")]
    public float CastDistance;
    public float MinDistanceToHead;
    public float StopControllingDistance;

    public float DirectionLeaniency;
    Vector3 StartPos;
    private GameObject OnlineFireball, PrivateFireball;

    public float ControlForce;
    public ControlType controlType;
    private Side side;



    [Header("References")]
    public GameObject PositionExample;
    [Header("Frames")]
    public Frames frames;

    public delegate void EventHandlerThree(bool State);
    public event EventHandlerThree RealNewState;
    [Header("Debug")]
    public bool ShouldDebug;
    public float Display;

    public int Min, Max;
    public GameObject TestOBJ;
    //public bool justHand, Both;
    public Quaternion SpawnRotation()
    {
        Vector3 HandDirection = AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().StartEndDirection(Min, Max).normalized;
        Vector3 AdjustedDir = new Vector3(HandDirection.x, 0, HandDirection.z);
        return Quaternion.LookRotation(AdjustedDir);
        /*
        Vector3 HeadDirection = new Vector3(AIMagicControl.instance.Cam.forward.x, 0, AIMagicControl.instance.Cam.forward.z);
        if(Both)
            
        if (justHand)
            return Quaternion.LookRotation(HandDirection);
        else
            return Quaternion.LookRotation(HeadDirection);
        */
        //return Quaternion.Euler(HandDirection + HeadDirection);
    }

    public void EndCount()
    {
        if(Vector3.Distance(StartPos, AIMagicControl.instance.Hands[(int)side].localPosition) > 0.2 && ShouldDebug)
            Debug.Log(Vector3.Distance(StartPos, AIMagicControl.instance.Hands[(int)side].localPosition));
        if (Vector3.Distance(StartPos, AIMagicControl.instance.Hands[(int)side].localPosition) > CastDistance)
        {
            //ControlPos = AIMagicControl.instance.Hands[(int)side].localPosition;
            StartCoroutine(WaitForClose());
            SpawnFireball(FireAbsorb.instance.FireballControl);
            ///FireAbsorb.instance.StopHoldingFireball();
        }
        StartPos = AIMagicControl.instance.Hands[(int)side].localPosition;
    }
    public Direction ControllerDir()
    {
        float Angle = Mathf.Abs(GetVelocityAngle(new Vector2(AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().Velocity.x, AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().Velocity.z)));
        //Debug.Log(Angle);
        Display = Angle;
        if (Angle > 180 - DirectionLeaniency) // 180 - 360
            return Direction.Away;
        else if (Angle < 0 + DirectionLeaniency && Angle > 0 - DirectionLeaniency)
            return Direction.Towards;
        else if (Angle < 90 + DirectionLeaniency && Angle > 90 - DirectionLeaniency)
            return Direction.Side;
        return Direction.None;
        float GetVelocityAngle(Vector2 Components)
        {
            return Mathf.Atan2(Components.x, Components.y) * Mathf.Rad2Deg;
        }
    }
    public bool RealState(bool FramesState)
    {
        //Dir = ControllerDir();
        bool VelocityWorks = ControllerDir() == Direction.Away;
        bool CloseEnough = Vector3.Distance(AIMagicControl.instance.Cam.position, AIMagicControl.instance.Hands[(int)side].position) > MinDistanceToHead;
        return FramesState && frames.CanCast && CloseEnough && VelocityWorks;
    }
    public void OnNewState(bool State)
    {
        //Debug.Log("NewState: " + State);
        bool FinalState = RealState(State);
        if (RealNewState != null)
            RealNewState(FinalState);
        if (FinalState == false)
        {
            //get distance to 
            EndCount();
        }
    }

    private void Start()
    {
        gameObject.GetComponent<LearningAgent>().NewState += OnNewState;
        gameObject.GetComponent<LearningAgent>().NewState += frames.AddToList;
        side = GetComponent<LearningAgent>().side;
    }

    public void SpawnFireball(bool Redirect)
    {
        if (InGameManager.instance.CanDoMagic() == false || frames.CanCast == false)
            return;

        EyeController.instance.ChangeEyes(Eyes.Fire);
        ///direction of controller forward
        Spell spell = (Redirect) ? Spell.BlueFireball : Spell.Fireball;
        if (SpawnOnline)
        {
            OnlineFireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(spell, true), AIMagicControl.instance.Spawn[(int)side].position, SpawnRotation());
            NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, Spell.Fireball);
        }
        //PrivateFireball = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(spell, false)), AIMagicControl.instance.Spawn[(int)side].position, AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().Velocity);
        //PrivateFireball = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(spell, false)), AIMagicControl.instance.Spawn[(int)side].position, SpawnRotation());
    }
    private void Update()
    {
        if (PositionExample != null)
            PositionExample.transform.localPosition = StartPos;

        if (TestOBJ != null)
        {
            TestOBJ.transform.rotation = SpawnRotation();
            TestOBJ.transform.position = AIMagicControl.instance.Spawn[(int)side].position;
        }
        if (IsControlling == false)
            return;
            //get amount to change normalized
            //change each frame
    }

    public IEnumerator WaitForClose()
    {
        bool PastThreshold = StopControllingDistance < Vector3.Distance(AIMagicControl.instance.Spawn[(int)side].localPosition, Camera.main.transform.localPosition);
        IsControlling = true;
        while (PastThreshold == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        IsControlling = false;
    }

}
