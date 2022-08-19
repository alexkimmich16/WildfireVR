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
    public static FireballController instance;
    void Awake() { instance = this; }

    private bool Active;
    public bool CanSpawn;
    public bool IsControlling;
    [Header("Stats")]
    public float Speed;
    public float MinFireDistance;
    Vector3 StartPos;
    public GameObject Fireball;

    private float DistToHead;
    public float DistToHeadThreshold;
    public float RotationThreshold;

    public float ControlForce;
    public ControlType controlType;
    [Header("References")]
    public Transform Spawn;
    public Transform Hand;

    [Header("Frames")]
    public Frames frames;

    public void StartCount()
    {
        //first
        StartPos = Hand.localPosition;
    }

    public void EndCount()
    {
        //Debug.Log(Vector3.Distance(StartPos, Hand.localPosition));
        
        if (Vector3.Distance(StartPos, Hand.localPosition) > MinFireDistance)
        {
            DistToHead = Vector3.Distance(Hand.localPosition, Camera.main.transform.localPosition);
            ControlPos = Hand.localPosition;
            StartCoroutine(WaitForClose());
            SpawnFireball(FireAbsorb.instance.FireballControl);
            FireAbsorb.instance.StopHoldingFireball();
        }
        StartPos = Hand.localPosition;
    }
    public void OnNewState(bool State)
    {
        //Debug.Log("NewState: " + State);
        if (!CanSpawn || Fireball != null)
            return;

        if (frames.AllPastFrames(true) && Active == false)
        {
            Active = true;
            StartCount();
        }
        else if (frames.AllPastFrames(false) && Active == true)
        {
            Active = false;
            EndCount();
        }
    }
    private void Start()
    {
        gameObject.GetComponent<LearningAgent>().NewState += OnNewState;
        gameObject.GetComponent<LearningAgent>().NewState += frames.AddToList;
    }

    public void SpawnFireball(bool Redirect)
    {
        EyeController.instance.ChangeEyes(Eyes.Fire);
        NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, Spell.Fireball);
        //Debug.Log(Redirect);
        if (Redirect == false) 
            Fireball = PhotonNetwork.Instantiate("RealFireball", Spawn.position, Camera.main.transform.rotation);
        else if(Redirect == true)
            Fireball = PhotonNetwork.Instantiate("BetterFireball", Spawn.position, Camera.main.transform.rotation);
    }
    private Vector3 ControlPos;
    private void Update()
    {

        if (IsControlling == false)
            return;
        if (controlType == ControlType.OnMoveHand)
        {

        }
        else if (controlType == ControlType.Constant)
        {

        }
        else
        {

        }
            
            //get amount to change normalized
            //change each frame
        
    }
    public IEnumerator WaitForClose()
    {
        bool PastThreshold = DistToHead > Vector3.Distance(Hand.localPosition, Camera.main.transform.localPosition);
        IsControlling = true;
        while (PastThreshold == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        IsControlling = false;
    }
}
