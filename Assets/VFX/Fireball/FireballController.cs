using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireballController : MonoBehaviour
{
    private bool Active;
    [Header("Stats")]
    public float Speed;
    public float MinFireDistance;
    Vector3 StartPos;
    
    [Header("References")]
    public GameObject Fireball;
    public Transform Spawn;
    public Transform Hand;
    public void StartCount()
    {
        //first
        StartPos = Hand.localPosition;
    }
    public void EndCount()
    {
        //Debug.Log(Vector3.Distance(StartPos, Hand.localPosition));
        if(Vector3.Distance(StartPos, Hand.localPosition) > MinFireDistance)
        {
            SpawnFireball();
        }
        StartPos = Hand.localPosition;
    }
    public void OnNewState(bool State)
    {
        //Debug.Log("NewState: " + State);
        if (State == true && Active == false)
        {
            Active = true;
            StartCount();
        }
        else if (State == false && Active == true)
        {
            Active = false;
            EndCount();
        }
    }
    private void Start()
    {
        gameObject.GetComponent<LearningAgent>().NewState += OnNewState;
    }
    public void SpawnFireball()
    {
        GameObject Current = PhotonNetwork.Instantiate("RealFireball", Spawn.position, Camera.main.transform.rotation);
        //GameObject Current = Instantiate(Fireball, Spawn.position, Camera.main.transform.rotation);
        Current.GetComponent<Fireball>().Speed = Speed;
    }
}
