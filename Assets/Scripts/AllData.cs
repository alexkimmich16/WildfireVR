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
        public Final Final;
    }

    [System.Serializable]
    public struct Final
    {
        public float[] LocalRight;
        public float[] LocalLeft;

        public float[] WorldRight;
        public float[] WorldLeft;

        public float[] DifferenceRight;
        public float[] DifferenceLeft;

        public float Time;
        public float Interval;

        public int MoveType;

        public bool Set;
    }

    [System.Serializable]
    public struct MovementDataAdd
    {
        public float[] LocalRight;
        public float[] LocalLeft;

        public float[] WorldRight;
        public float[] WorldLeft;

        public float[] DifferenceRight;
        public float[] DifferenceLeft;

        public float Time;
        public float Interval;

        public int MoveType;

        public bool Set;
    }

    

    public AllData()
    {
        //Debug.Log("Test1");
        allTypes.TotalTypes = new TotalTypes[HandDebug.instance.DataFolders.Count];
        for (var t = 0; t < HandDebug.instance.DataFolders.Count; t++)//for each type
        {
            //Debug.Log("Test2");
            allTypes.TotalTypes[t].InsideType = new MovementDataAdd[HandDebug.instance.DataFolders[t].Storage.Count];
            for (var i = 0; i < HandDebug.instance.DataFolders[t].Storage.Count; i++)//for all the units in the type type
            {
                //Debug.Log("Test3");
                MovementData data = HandDebug.instance.DataFolders[t].Storage[i];
                allTypes.TotalTypes[t].InsideType[i].Set = data.Set;
                if (data.Set == true)
                {
                    allTypes.TotalTypes[t].InsideType[i].LocalLeft = new float[data.LeftLocalPos.Count * 3];
                    allTypes.TotalTypes[t].InsideType[i].LocalRight = new float[data.RightLocalPos.Count * 3];

                    allTypes.TotalTypes[t].InsideType[i].WorldLeft = new float[data.LeftLocalPos.Count * 3];
                    allTypes.TotalTypes[t].InsideType[i].WorldRight = new float[data.RightLocalPos.Count * 3];

                    allTypes.TotalTypes[t].InsideType[i].DifferenceLeft = new float[data.LeftLocalPos.Count * 3];
                    allTypes.TotalTypes[t].InsideType[i].DifferenceRight = new float[data.RightLocalPos.Count * 3];
                    for (var j = 0; j < data.LeftLocalPos.Count; j++)//LeftLocal
                    {
                        Vector3 LeftPos = data.LeftLocalPos[j];
                        Vector3 LeftWorld = data.LeftWorldPos[j];
                        Vector3 LeftDif = data.LeftDifferencePos[j];
                        
                        int ArrayNum = 0 + j * 3;
                        allTypes.TotalTypes[t].InsideType[i].LocalLeft[ArrayNum] = LeftPos.x;
                        allTypes.TotalTypes[t].InsideType[i].WorldLeft[ArrayNum] = LeftWorld.x;
                        allTypes.TotalTypes[t].InsideType[i].DifferenceLeft[ArrayNum] = LeftDif.x;
                        
                        ArrayNum = 1 + j * 3;
                        allTypes.TotalTypes[t].InsideType[i].LocalLeft[ArrayNum] = LeftPos.y;
                        allTypes.TotalTypes[t].InsideType[i].WorldLeft[ArrayNum] = LeftWorld.y;
                        allTypes.TotalTypes[t].InsideType[i].DifferenceLeft[ArrayNum] = LeftDif.y;

                        ArrayNum = 2 + j * 3;
                        allTypes.TotalTypes[t].InsideType[i].LocalLeft[ArrayNum] = LeftPos.z;
                        allTypes.TotalTypes[t].InsideType[i].WorldLeft[ArrayNum] = LeftWorld.z;
                        allTypes.TotalTypes[t].InsideType[i].DifferenceLeft[ArrayNum] = LeftDif.z;

                    }
                    //Debug.Log("Test4");
                    allTypes.TotalTypes[t].InsideType[i].Time = data.Time;
                    allTypes.TotalTypes[t].InsideType[i].Interval = data.Interval;
                    allTypes.TotalTypes[t].InsideType[i].MoveType = t;
                    for (var j = 0; j < data.RightLocalPos.Count; j++)//rightlocal
                    {
                        Vector3 RightPos = data.RightLocalPos[j];
                        Vector3 RightWorld = data.RightWorldPos[j];
                        Vector3 RightDif = data.RightDifferencePos[j];

                        int ArrayNum = 0 + j * 3;
                        allTypes.TotalTypes[t].InsideType[i].LocalRight[ArrayNum] = RightPos.x;
                        allTypes.TotalTypes[t].InsideType[i].WorldRight[ArrayNum] = RightWorld.x;
                        allTypes.TotalTypes[t].InsideType[i].DifferenceRight[ArrayNum] = RightDif.x;

                        ArrayNum = 1 + j * 3;
                        allTypes.TotalTypes[t].InsideType[i].LocalRight[ArrayNum] = RightPos.y;
                        allTypes.TotalTypes[t].InsideType[i].WorldRight[ArrayNum] = RightWorld.y;
                        allTypes.TotalTypes[t].InsideType[i].DifferenceRight[ArrayNum] = RightDif.y;

                        ArrayNum = 2 + j * 3;
                        allTypes.TotalTypes[t].InsideType[i].LocalRight[ArrayNum] = RightPos.z;
                        allTypes.TotalTypes[t].InsideType[i].WorldRight[ArrayNum] = RightWorld.z;
                        allTypes.TotalTypes[t].InsideType[i].DifferenceRight[ArrayNum] = RightDif.z;
                    }
                    //Debug.Log("Test5");
                }
            }
            //Debug.Log("Test6");
            FinalMovement FinalData = HandDebug.instance.DataFolders[t].FinalInfo;

            //Debug.Log(FinalData.LeftLocalPos.Count);
            allTypes.TotalTypes[t].Final = new Final();
            allTypes.TotalTypes[t].Final.LocalLeft = new float[FinalData.LeftLocalPos.Count * 3];
            allTypes.TotalTypes[t].Final.LocalRight = new float[FinalData.RightLocalPos.Count * 3];

            allTypes.TotalTypes[t].Final.WorldLeft = new float[FinalData.LeftLocalPos.Count * 3];
            allTypes.TotalTypes[t].Final.WorldRight = new float[FinalData.RightLocalPos.Count * 3];

            allTypes.TotalTypes[t].Final.DifferenceLeft = new float[FinalData.LeftLocalPos.Count * 3];
            allTypes.TotalTypes[t].Final.DifferenceRight = new float[FinalData.RightLocalPos.Count * 3];
            //Debug.Log("Test7");
            for (var j = 0; j < FinalData.LeftLocalPos.Count; j++)
            {
                //Debug.Log("Test8");
                Vector3 LeftPos = FinalData.LeftLocalPos[j];
                //Vector3 LeftWorld = FinalData.LeftWorldPos[j];
                //Vector3 LeftDif = FinalData.LeftDifferencePos[j];
                //Debug.Log("Test8.1");
                int ArrayNum = 0 + j * 3;
                allTypes.TotalTypes[t].Final.LocalLeft[ArrayNum] = LeftPos.x;
               // allTypes.TotalTypes[t].Final.WorldLeft[ArrayNum] = LeftWorld.x;
                //allTypes.TotalTypes[t].Final.DifferenceLeft[ArrayNum] = LeftDif.x;
                //Debug.Log("Test8.2");
                ArrayNum = 1 + j * 3;
                allTypes.TotalTypes[t].Final.LocalLeft[ArrayNum] = LeftPos.y;
                //allTypes.TotalTypes[t].Final.WorldLeft[ArrayNum] = LeftWorld.y;
                //allTypes.TotalTypes[t].Final.DifferenceLeft[ArrayNum] = LeftDif.y;
                //Debug.Log("Test8.3");
                ArrayNum = 2 + j * 3;
                allTypes.TotalTypes[t].Final.LocalLeft[ArrayNum] = LeftPos.z;
                //allTypes.TotalTypes[t].Final.WorldLeft[ArrayNum] = LeftWorld.z;
                //allTypes.TotalTypes[t].Final.DifferenceLeft[ArrayNum] = LeftDif.z;
                //Debug.Log("Test9");
            }
            //Debug.Log("Test10");
            for (var j = 0; j < FinalData.RightLocalPos.Count; j++)
            {
                //Debug.Log("Test11");
                Vector3 RightPos = FinalData.RightLocalPos[j];
                //Vector3 RightWorld = FinalData.RightWorldPos[j];
                //Vector3 RightDif = FinalData.RightDifferencePos[j];

                int ArrayNum = 0 + j * 3;
                allTypes.TotalTypes[t].Final.LocalRight[ArrayNum] = RightPos.x;
                //allTypes.TotalTypes[t].Final.WorldRight[ArrayNum] = RightWorld.x;
                //allTypes.TotalTypes[t].Final.DifferenceRight[ArrayNum] = RightDif.x;

                ArrayNum = 1 + j * 3;
                allTypes.TotalTypes[t].Final.LocalRight[ArrayNum] = RightPos.y;
                //allTypes.TotalTypes[t].Final.WorldRight[ArrayNum] = RightWorld.y;
                //allTypes.TotalTypes[t].Final.DifferenceRight[ArrayNum] = RightDif.y;

                ArrayNum = 2 + j * 3;
                allTypes.TotalTypes[t].Final.LocalRight[ArrayNum] = RightPos.z;
                //allTypes.TotalTypes[t].Final.WorldRight[ArrayNum] = RightWorld.z;
                //allTypes.TotalTypes[t].Final.DifferenceRight[ArrayNum] = RightDif.z;
                //Debug.Log("Test12");
            }
            allTypes.TotalTypes[t].Final.Time = FinalData.TotalTime;
            allTypes.TotalTypes[t].Final.MoveType = t;
            //Debug.Log("Test13");
            //allTypes.TotalTypes[t].Final.Interval = FinalData.Interval;
            //problem inside set
        }

        //Debug.Log("0Check" + allTypes.TotalTypes[0].InsideType[0].Set);

    }
}