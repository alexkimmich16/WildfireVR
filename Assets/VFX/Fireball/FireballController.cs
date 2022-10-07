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
    public float Speed;
    public float CastDistance;
    public float MinDistanceToHead;
    public float StopControllingDistance;
    public float MinVelCam;
    public float MinVelHand;

    public float DirectionLeaniency;
    Vector3 StartPos;
    public GameObject OnlineFireball;
    public GameObject PrivateFireball;
    

    public float RotationThreshold;

    public float ControlForce;
    public ControlType controlType;
    private Side side;



    [Header("References")]
    public GameObject PositionExample;
    [Header("Frames")]
    public Frames frames;

    public delegate void EventHandlerThree(bool State);
    public event EventHandlerThree RealNewState;

    public void StartCount()
    {
        //first
        StartPos = AIMagicControl.instance.Hands[(int)side].localPosition;
    }

    public void EndCount()
    {
        if(Vector3.Distance(StartPos, AIMagicControl.instance.Hands[(int)side].localPosition) > 0.2)
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
        Vector3 LevelVel = new Vector3(AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().Velocity.x, 0, AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().Velocity.z);
        Vector3 LevelCamRot = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        float VelCamAngle = Vector3.Angle(LevelVel, LevelCamRot);
        float Angle = VelCamAngle;
        Debug.Log(Angle);
        if (Angle > 180 - DirectionLeaniency)
            return Direction.Towards;
        else if (Angle < 0 + DirectionLeaniency && Angle > 0 - DirectionLeaniency)
            return Direction.Away;
        else if (Angle < 90 + DirectionLeaniency && Angle > 90 - DirectionLeaniency)
            return Direction.Side;
        return Direction.None;
    }
    public bool RealState(bool FramesState)
    {
        /*
        Vector3 CamForward = new Vector3(0, Camera.main.transform.eulerAngles.y, 0).normalized;
        Vector3 HandForward = AIMagicControl.instance.Hands[(int)side].eulerAngles.normalized;
        Vector3 Velocity = AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().Velocity.normalized;
        float CamVelDist = Vector3.Angle(Velocity, CamForward);
        float HandVelDist = Vector3.Angle(Velocity, HandForward);
        */
        bool VelocityWorks = ControllerDir() == Direction.Away;
        //Debug.Log(HandVelDist);
        //Debug.Log(Vector3.Distance(Camera.main.transform.position, AIMagicControl.instance.Hands[(int)side].position));
        return FramesState && frames.CanCast && Vector3.Distance(Camera.main.transform.position, AIMagicControl.instance.Hands[(int)side].position) > MinDistanceToHead && VelocityWorks;
    }
    public void OnNewState(bool State)
    {
        //Debug.Log("NewState: " + State);
        bool FinalState = RealState(State);
        if (RealNewState != null)
            RealNewState(FinalState);


        if (FinalState == true)
        {

        }
        else if (FinalState == false)
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
        EyeController.instance.ChangeEyes(Eyes.Fire);
        ///direction of controller forward
        Spell spell;
        if (Redirect == false)
            spell = Spell.Fireball;
        else
            spell = Spell.BlueFireball;

        if (SpawnOnline)
        {
            OnlineFireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(spell, true), AIMagicControl.instance.Spawn[(int)side].position, Camera.main.transform.rotation);
            NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, Spell.Fireball);
            OnlineFireball.SetActive(false);
        }
        PrivateFireball = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(spell, true)), AIMagicControl.instance.Spawn[(int)side].position, Camera.main.transform.rotation);
    }
    private void Update()
    {
        if (PositionExample != null)
            PositionExample.transform.localPosition = StartPos;

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
