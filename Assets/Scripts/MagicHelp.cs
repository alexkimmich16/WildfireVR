using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Odin
{
    public static class MagicHelp
    {
        public static Vector3 GetLocalPosSide(int Side, int i, int Current)
        {
            //Debug.Log("Side:  " + Side + "   i:  " + i + "   Current:  " + Current + "   Count:  " + HandMagic.instance.Spells[i].FinalInfo.LeftLocalPos.Count);
            //if(i > HandDebug.instance.DataFolders[i].FinalInfo.LeftLocalPos.Count)

            if (Side == 0)
                return ConvertDataToPoint(HandMagic.instance.Spells[i].FinalInfo.LeftLocalPos[Current]);
            else
                return ConvertDataToPoint(HandMagic.instance.Spells[i].FinalInfo.RightLocalPos[Current]);
        }
        public static Vector3 GetRotationSide(int Side, int i, int Current)
        {
            //if(i > HandDebug.instance.DataFolders[i].FinalInfo.LeftLocalPos.Count)
            //Debug.Log("Side:  " + Side + "   i:  " + i + "   Current:  " + Current + "   Count:  " + HandDebug.instance.DataFolders[i].FinalInfo.LeftLocalPos.Count);
            if (Side == 0)
                return HandMagic.instance.Spells[i].FinalInfo.LeftRotation[Current];
            else
                return HandMagic.instance.Spells[i].FinalInfo.RightRotation[Current];
        }
        public static Vector3 ConvertDataToPoint(Vector3 Local)
        {
            float Distance = Local.x;
            float RotationOffset = Local.y;
            float HorizonalOffset = Local.z;
            Quaternion rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y + RotationOffset, 0);
            Vector3 Forward = rotation * Vector3.forward;
            //Vector3 downVector = transform.TransformDirection(Forward);
            Ray r = new Ray(Camera.main.transform.position, Forward);
            Vector3 YPosition = r.GetPoint(Distance);
            return new Vector3(YPosition.x, HorizonalOffset + Camera.main.transform.position.y, YPosition.z);
        }

        public static void LoadMainScriptableObjects(AllData Load)
        {
            HandMagic HM = HandMagic.instance;
            int TypeCount;
            if (HandMagic.LoadOnly3 == true)
                TypeCount = 4;
            else
                TypeCount = Load.allTypes.TotalTypes.Length;
            for (var t = 0; t < TypeCount; t++)//for each type
            {
                FinalMovement FinalData = HM.Spells[t].FinalInfo;
                List<Vector3> LocalLeftFinal = new List<Vector3>();
                List<Vector3> RotationLeftFinal = new List<Vector3>();

                List<Vector3> LocalRightFinal = new List<Vector3>();
                List<Vector3> RotationRightFinal = new List<Vector3>();

                List<Vector2> RotationLocks = new List<Vector2>();
                RotationLocks.Clear();
                
                for (var j = 0; j < 3; j++)
                {
                    //Debug.Log("t: " + t + "  j:  " + j);
                    int True = j * 2;
                    if (Load.allTypes.TotalTypes[t].Final.RotationLock[True] != 0 || Load.allTypes.TotalTypes[t].Final.RotationLock[True + 1] != 0)
                        RotationLocks.Add(new Vector2(Load.allTypes.TotalTypes[t].Final.RotationLock[True], Load.allTypes.TotalTypes[t].Final.RotationLock[True + 1]));
                    else
                        RotationLocks.Add(Vector2.zero);
                }
                FinalData.RotationLock = RotationLocks;

                for (var j = 0; j < Load.allTypes.TotalTypes[t].Final.LocalLeft.Length / 3; j++)//for each localdata in unit
                {
                    AllData.Final Final = Load.allTypes.TotalTypes[t].Final;
                    //Debug.Log("LocalLeft T: " + t + "  J:  " + j);
                    LocalLeftFinal.Add(GetVector3(Final.LocalLeft, j));
                    //Debug.Log("LeftRot T: " + t + "  J:  " + j);
                    RotationLeftFinal.Add(GetVector3(Final.LeftRot, j));
                    //Debug.Log("LocalRight T: " + t + "  J:  " + j);
                    LocalRightFinal.Add(GetVector3(Final.LocalRight, j));
                    //Debug.Log("RightRot T: " + t + "  J:  " + j);
                    RotationRightFinal.Add(GetVector3(Final.RightRot, j));
                }
                
                FinalData.RightLocalPos = new List<Vector3>(LocalRightFinal);
                FinalData.LeftLocalPos = new List<Vector3>(LocalLeftFinal);
                FinalData.LeftRotation = new List<Vector3>(RotationLeftFinal);
                FinalData.RightRotation = new List<Vector3>(RotationRightFinal);
                FinalData.TotalTime = Load.allTypes.TotalTypes[t].Final.Time;
                FinalData.MoveType = (Movements)t;

                Vector3 GetVector3(float[] NumList, int ArrayNum)
                {
                    int VecConvert = ArrayNum * 3;
                    //Debug.Log(NumList.Length + " " + VecConvert);
                    return new Vector3(NumList[VecConvert], NumList[VecConvert + 1], NumList[VecConvert + 2]);
                }
            }
        }
    }
}

