using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AllData
{
    MovementData[] Spike = new MovementData[20];
    MovementData[] Fireball = new MovementData[20];
    MovementData[] Shield = new MovementData[20];

    struct MovementData
    {
        public float[] LocalRight;
        public float[] LocalLeft;

        public float Time;
        public float Interval;

        public int MoveType;
    }

    public AllData()
    {
        for (var i = 0; i < 20; i++)
        {
            for (var j = 0; j < HandDebug.instance.; j++)
            {
                LocalRight[].
                Spike[i]
            } 
        }
        Name = UpgradeScript.Name;
        Jump[0] = UpgradeScript.Vehicles[0].JumpLevel;
    }
}