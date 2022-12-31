using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public bool Active;
    public bool AlwaysTrue;

    [Header("Stats")]

    private Side side;
    public delegate void NewState(bool State);
    public event NewState RealNewState;
    [Header("References")]
    //public Transform Spawn;
    //p//ublic Transform Hand;


    [Header("Frames")]
    public Frames frames;
    public void OnNewState(bool State)
    {
        //Debug.Log("NewState: " + State);
        if (!frames.CanCast)
            return;
        if (AlwaysTrue)
        {
            Active = true;
            return;
        }
            
        if (frames.FramesWork() == true && Active == false)
        {
            Active = true;
            SetNewState(true);
        }
        else if (frames.FramesWork() == false && Active == true)
        {
            Active = false;
            SetNewState(false);
        }
    }
    public void BlockFire()
    {
        //get nearby fires 
        //if close block?
    }
    public void SetNewState(bool NewState)
    {
        if (RealNewState != null)
            RealNewState(NewState);
    }
    private void Start()
    {
        gameObject.GetComponent<LearningAgent>().NewState += OnNewState;
        gameObject.GetComponent<LearningAgent>().NewState += frames.AddToList;
        side = GetComponent<LearningAgent>().side;
    }
    private void Update()
    {
        if (Active == false)
            return;
    }

    
}
