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

        public float[] LeftRot;
        public float[] RightRot;

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

        public float[] LeftRot;
        public float[] RightRot;

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
                MovementData data = HandDebug.instance.DataFolders[t].Storage[i];
                allTypes.TotalTypes[t].InsideType[i].Set = data.Set;
                if (data.Set == true)
                {
                    //Debug.Log("Test3.3");
                    allTypes.TotalTypes[t].InsideType[i].LocalLeft = new float[data.LeftLocalPos.Count * 3];
                    allTypes.TotalTypes[t].InsideType[i].LocalRight = new float[data.RightLocalPos.Count * 3];

                    allTypes.TotalTypes[t].InsideType[i].WorldLeft = new float[data.LeftLocalPos.Count * 3];
                    allTypes.TotalTypes[t].InsideType[i].WorldRight = new float[data.RightLocalPos.Count * 3];

                    allTypes.TotalTypes[t].InsideType[i].DifferenceLeft = new float[data.LeftLocalPos.Count * 3];
                    allTypes.TotalTypes[t].InsideType[i].DifferenceRight = new float[data.RightLocalPos.Count * 3];

                    allTypes.TotalTypes[t].InsideType[i].LeftRot = new float[data.LeftRotation.Count * 3];
                    allTypes.TotalTypes[t].InsideType[i].RightRot = new float[data.RightRotation.Count * 3];

                    for (var j = 0; j < data.LeftRotation.Count; j++)//LeftLocal
                    {
                        Vector3 LeftRot = data.LeftRotation[j];
                        allTypes.TotalTypes[t].InsideType[i].LeftRot[0 + j * 3] = LeftRot.x;
                        allTypes.TotalTypes[t].InsideType[i].LeftRot[1 + j * 3] = LeftRot.y;
                        allTypes.TotalTypes[t].InsideType[i].LeftRot[2 + j * 3] = LeftRot.z;
                    }

                    for (var j = 0; j < data.RightRotation.Count; j++)//LeftLocal
                    {
                        Vector3 RightRot = data.RightRotation[j];
                        allTypes.TotalTypes[t].InsideType[i].RightRot[0 + j * 3] = RightRot.x;
                        allTypes.TotalTypes[t].InsideType[i].RightRot[1 + j * 3] = RightRot.y;
                        allTypes.TotalTypes[t].InsideType[i].RightRot[2 + j * 3] = RightRot.z;
                    }

                    for (var j = 0; j < data.LeftLocalPos.Count; j++)//LeftLocal
                    {
                        Vector3 LeftPos = data.LeftRotation[j];
                        allTypes.TotalTypes[t].InsideType[i].LocalLeft[0 + j * 3] = LeftPos.x;
                        allTypes.TotalTypes[t].InsideType[i].LocalLeft[1 + j * 3] = LeftPos.y;
                        allTypes.TotalTypes[t].InsideType[i].LocalLeft[2 + j * 3] = LeftPos.z;
                    }

                    allTypes.TotalTypes[t].InsideType[i].Time = data.Time;
                    allTypes.TotalTypes[t].InsideType[i].Interval = data.Interval;
                    allTypes.TotalTypes[t].InsideType[i].MoveType = t;
                    for (var j = 0; j < data.RightLocalPos.Count; j++)//rightlocal
                    {
                        Vector3 RightPos = data.RightLocalPos[j];
                        allTypes.TotalTypes[t].InsideType[i].LocalRight[0 + j * 3] = RightPos.x;
                        allTypes.TotalTypes[t].InsideType[i].LocalRight[1 + j * 3] = RightPos.y;
                        allTypes.TotalTypes[t].InsideType[i].LocalRight[2 + j * 3] = RightPos.z;
                    }
                }
            }
            FinalMovement FinalData = HandMagic.instance.Spells[t].FinalInfo;

            allTypes.TotalTypes[t].Final = new Final();
            allTypes.TotalTypes[t].Final.LocalLeft = new float[FinalData.LeftLocalPos.Count * 3];
            allTypes.TotalTypes[t].Final.LocalRight = new float[FinalData.RightLocalPos.Count * 3];

            allTypes.TotalTypes[t].Final.WorldLeft = new float[FinalData.LeftLocalPos.Count * 3];
            allTypes.TotalTypes[t].Final.WorldRight = new float[FinalData.RightLocalPos.Count * 3];

            allTypes.TotalTypes[t].Final.DifferenceLeft = new float[FinalData.LeftLocalPos.Count * 3];
            allTypes.TotalTypes[t].Final.DifferenceRight = new float[FinalData.RightLocalPos.Count * 3];

            allTypes.TotalTypes[t].Final.LeftRot = new float[FinalData.LeftRotation.Count * 3];
            allTypes.TotalTypes[t].Final.RightRot = new float[FinalData.RightRotation.Count * 3];

            for (var j = 0; j < FinalData.LeftLocalPos.Count; j++)
            {
                Vector3 LeftPos = FinalData.LeftLocalPos[j];
                allTypes.TotalTypes[t].Final.LocalLeft[0 + j * 3] = LeftPos.x;
                allTypes.TotalTypes[t].Final.LocalLeft[1 + j * 3] = LeftPos.y;
                allTypes.TotalTypes[t].Final.LocalLeft[2 + j * 3] = LeftPos.z;
            }
            for (var j = 0; j < FinalData.RightLocalPos.Count; j++)
            {
                Vector3 RightPos = FinalData.RightLocalPos[j];
                allTypes.TotalTypes[t].Final.LocalRight[0 + j * 3] = RightPos.x;
                allTypes.TotalTypes[t].Final.LocalRight[1 + j * 3] = RightPos.y;
                allTypes.TotalTypes[t].Final.LocalRight[2 + j * 3] = RightPos.z;
            }
            for (var j = 0; j < FinalData.LeftRotation.Count; j++)
            {
                Vector3 LeftRot = FinalData.LeftRotation[j];
                allTypes.TotalTypes[t].Final.LeftRot[0 + j * 3] = LeftRot.x;
                allTypes.TotalTypes[t].Final.LeftRot[1 + j * 3] = LeftRot.y;
                allTypes.TotalTypes[t].Final.LeftRot[2 + j * 3] = LeftRot.z;
            }
            for (var j = 0; j < FinalData.RightRotation.Count; j++)
            {
                Vector3 RightRot = FinalData.RightRotation[j];
                allTypes.TotalTypes[t].Final.RightRot[0 + j * 3] = RightRot.x;
                allTypes.TotalTypes[t].Final.RightRot[1 + j * 3] = RightRot.y;
                allTypes.TotalTypes[t].Final.RightRot[2 + j * 3] = RightRot.z;
            }
            allTypes.TotalTypes[t].Final.Time = FinalData.TotalTime;
            allTypes.TotalTypes[t].Final.MoveType = t;
            //allTypes.TotalTypes[t].Final.Interval = FinalData.Interval;
            //problem inside set
        }
    }
}