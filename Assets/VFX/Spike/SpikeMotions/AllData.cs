using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AllData
{
    //static AllTypes allTypes;
    //public AllTypes allTypes;
    //AllTypes allTypes = ;

    
    
    public AllData()
    {
        
        
        //AllTypes allTypes = new AllTypes();
        for (var t = 0; t < HandDebug.instance.DataFolders.Count; t++)//for each type
        {
            allTypes.TotalTypes[t].InsideType = new MovementDataAdd[HandDebug.instance.DataFolders[t].Storage.Count];
            for (var i = 0; i < HandDebug.instance.DataFolders[t].Storage.Count; i++)//for all the units in the type type
            {
                MovementData data = HandDebug.instance.DataFolders[t].Storage[i];
                MovementDataAdd StructData = allTypes.TotalTypes[t].InsideType[i];
                StructData.LocalLeft = new float[data.LeftLocalPos.Count * 3];
                StructData.LocalRight = new float[data.RightLocalPos.Count * 3];
                for (var j = 0; j < HandDebug.instance.DataFolders[t].Storage[i].LeftLocalPos.Count; j++)//for each localdata in unit
                {
                    Vector3 LeftPos = data.LeftLocalPos[j];
                    Vector3 RightPos = data.RightLocalPos[j];
                    int ArrayNum = 0 + j * 3;
                    StructData.LocalLeft[ArrayNum] = LeftPos.x;
                    StructData.LocalRight[ArrayNum] = RightPos.x;
                    ArrayNum = 1 + j * 3;
                    StructData.LocalLeft[ArrayNum] = LeftPos.x;
                    StructData.LocalRight[ArrayNum] = RightPos.x;
                    ArrayNum = 2 + j * 3;
                    StructData.LocalLeft[ArrayNum] = LeftPos.x;
                    StructData.LocalRight[ArrayNum] = RightPos.x;
                }
                StructData.Time = data.Time;
                StructData.Interval = data.Interval;
                StructData.MoveType = t;
                //Name = UpgradeScript.Name;
                //Jump[0] = UpgradeScript.Vehicles[0].JumpLevel;
            }
            
        }

            
    }
}