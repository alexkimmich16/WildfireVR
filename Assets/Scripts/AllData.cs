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

        public float[] RotationLock;

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
                    allTypes.TotalTypes[t].InsideType[i].LocalLeft = GetList(data.LeftLocalPos);
                    allTypes.TotalTypes[t].InsideType[i].LeftRot = GetList(data.LeftRotation);
                    
                    allTypes.TotalTypes[t].InsideType[i].LocalRight = GetList(data.RightLocalPos);
                    allTypes.TotalTypes[t].InsideType[i].RightRot = GetList(data.RightRotation);
                    
                    allTypes.TotalTypes[t].InsideType[i].Time = data.Time;
                    allTypes.TotalTypes[t].InsideType[i].Interval = data.Interval;
                    allTypes.TotalTypes[t].InsideType[i].MoveType = t;
                }
            }
            FinalMovement FinalData = HandMagic.instance.Spells[t].FinalInfo;

            allTypes.TotalTypes[t].Final = new Final();

            allTypes.TotalTypes[t].Final.LocalLeft = GetList(FinalData.LeftLocalPos);
            allTypes.TotalTypes[t].Final.LeftRot = GetList(FinalData.LeftRotation);

            allTypes.TotalTypes[t].Final.LocalRight = GetList(FinalData.RightLocalPos);
            allTypes.TotalTypes[t].Final.RightRot = GetList(FinalData.RightRotation);

            allTypes.TotalTypes[t].Final.Time = FinalData.TotalTime;
            allTypes.TotalTypes[t].Final.MoveType = t;
            float[] RotationLock = new float[6];
            for (var j = 0; j < 3; j++)
            {
                int True = j * 2;
                RotationLock[True] = FinalData.RotationLock[j].x;
                RotationLock[True + 1] = FinalData.RotationLock[j].y;
            }
            allTypes.TotalTypes[t].Final.RotationLock = RotationLock;

            float[] GetList(List<Vector3> Stat)
            {
                float[] FloatList = new float[Stat.Count * 3];
                for (int i = 0; i < Stat.Count; i++)
                {
                    int Num = i * 3;
                    FloatList[Num + 0] = Stat[i].x;
                    FloatList[Num + 1] = Stat[i].y;
                    FloatList[Num + 2] = Stat[i].z;
                }
                return FloatList;

            }
        }
    }
}