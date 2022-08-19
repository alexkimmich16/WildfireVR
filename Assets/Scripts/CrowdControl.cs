using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdControl : MonoBehaviour
{
    public float CrowdDensity;
    [Range(0,1)]
    public float CheerRate;


    ///where are hit sounds from? random direction? closest?

    public void OnPlayerHit()
    {
        ///play light sound
    }
    public void OnPlayerKilled()
    {
        ///play heavy sound
    }
    
    public float TotalElo()
    {
        return 1;
        ///all players
    }
}
