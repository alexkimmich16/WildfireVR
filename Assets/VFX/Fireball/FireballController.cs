using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireballController : MonoBehaviour
{
    public static FireballController instance;
    void Awake() { instance = this; }

    private bool Active;
    public bool CanSpawn;
    [Header("Stats")]
    public float Speed;
    public float MinFireDistance;
    Vector3 StartPos;
    public GameObject Fireball;


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
    private void Update()
    {
       
    }

    public void EndCount()
    {
        //Debug.Log(Vector3.Distance(StartPos, Hand.localPosition));
        if(Vector3.Distance(StartPos, Hand.localPosition) > MinFireDistance)
        {
            SpawnFireball(FireAbsorb.FireballControl);
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
        if(Redirect == false) 
            Fireball = PhotonNetwork.Instantiate("RealFireball", Spawn.position, Camera.main.transform.rotation);
        else
            Fireball = PhotonNetwork.Instantiate("BetterFireball", Spawn.position, Camera.main.transform.rotation);
    }
}
