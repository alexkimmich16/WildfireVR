using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AllData
{
    //static AllTypes allTypes;
    //AllTypes allTypes = ;

    [SerializeField]
    public AllTypes allTypes;
    

    [System.Serializable]
    public struct AllTypes
    {
        public TotalTypes[] TotalTypes;
    }

    [System.Serializable]
    public struct TotalTypes
    {
        public MovementDataAdd[] InsideType;
    }

    [System.Serializable]
    public struct MovementDataAdd
    {
        public float[] LocalRight;
        public float[] LocalLeft;

        public float Time;
        public float Interval;

        public int MoveType;

        public bool Set;
    }

    public AllData()
    {
        
        allTypes.TotalTypes = new TotalTypes[HandDebug.instance.DataFolders.Count];
        //Debug.Log("save1");
        for (var t = 0; t < HandDebug.instance.DataFolders.Count; t++)//for each type
        {
            //Debug.Log("save2");
            
            allTypes.TotalTypes[t].InsideType = new MovementDataAdd[HandDebug.instance.DataFolders[t].Storage.Count];
            for (var i = 0; i < HandDebug.instance.DataFolders[t].Storage.Count; i++)//for all the units in the type type
            {
                MovementData data = HandDebug.instance.DataFolders[t].Storage[i];
                //MovementDataAdd StructData = allTypes.TotalTypes[t].InsideType[i];
                Debug.Log("Data" + data.Set);
                allTypes.TotalTypes[t].InsideType[i].Set = data.Set;
                if (data.Set == true)
                {
                    allTypes.TotalTypes[t].InsideType[i].LocalLeft = new float[data.LeftLocalPos.Count * 3];
                    allTypes.TotalTypes[t].InsideType[i].LocalRight = new float[data.RightLocalPos.Count * 3];
                    for (var j = 0; j < HandDebug.instance.DataFolders[t].Storage[i].LeftLocalPos.Count; j++)//for each localdata in unit
                    {
                        Debug.Log("save8");
                        Vector3 LeftPos = data.LeftLocalPos[j];
                        Vector3 RightPos = data.RightLocalPos[j];
                        int ArrayNum = 0 + j * 3;
                        allTypes.TotalTypes[t].InsideType[i].LocalLeft[ArrayNum] = LeftPos.x;
                        allTypes.TotalTypes[t].InsideType[i].LocalRight[ArrayNum] = RightPos.x;
                        ArrayNum = 1 + j * 3;
                        allTypes.TotalTypes[t].InsideType[i].LocalLeft[ArrayNum] = LeftPos.y;
                        allTypes.TotalTypes[t].InsideType[i].LocalRight[ArrayNum] = RightPos.y;
                        ArrayNum = 2 + j * 3;
                        allTypes.TotalTypes[t].InsideType[i].LocalLeft[ArrayNum] = LeftPos.z;
                        allTypes.TotalTypes[t].InsideType[i].LocalRight[ArrayNum] = RightPos.z;
                    }
                    allTypes.TotalTypes[t].InsideType[i].Time = data.Time;
                    allTypes.TotalTypes[t].InsideType[i].Interval = data.Interval;
                    allTypes.TotalTypes[t].InsideType[i].MoveType = t;
                    
                    // Debug.Log("save4");
                }
                
            }
            //problem inside set
        }

        Debug.Log("0Check" + allTypes.TotalTypes[0].InsideType[0].Set);

    }
}