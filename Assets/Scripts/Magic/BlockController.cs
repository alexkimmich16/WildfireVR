using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestrictionSystem;
public class BlockController : MonoBehaviour
{
    public bool Active;
    public bool AlwaysTrue;

    [Header("Stats")]

    private Side side;
    //public Transform Spawn;
    //p//ublic Transform Hand;
    //Active = false;
    //SetNewState(false);
    public void BlockFire()
    {
        //get nearby fires 
        //if close block?
    }
    public void SetNewState(bool NewState)
    {

    }
    private void Start()
    {
        ///gameObject.GetComponent<LearningAgent>().NewState += OnNewState;
        //gameObject.GetComponent<LearningAgent>().NewState += frames.AddToList;
        //side = GetComponent<LearningAgent>().side;
    }
    private void Update()
    {
        if (Active == false)
            return;
    }

    
}
