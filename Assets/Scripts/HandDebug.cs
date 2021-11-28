using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum Movements
{
    Spike = 0,
    Fireball = 1,
    Shield = 2,
}
public class HandDebug : MonoBehaviour
{
    
    #region Singleton
    public static HandDebug instance;
    void Awake() { instance = this; }

    #endregion

    ///for tomorrow:
    ///player look direction raycast spawns
    ///structs for subscripts

    public float CountEverySecond = 20;
    private float MaxTime;
    private float Timer;
    private bool CurrentMovement = false;

    public Transform Player;

    public HandActions Left;
    public HandActions Right;

    public TextMeshProUGUI ScriptableNum;
    public TextMeshProUGUI Set;
    public TextMeshProUGUI Type;

    [Header("OnePackageOnly")]
    public int PackageNum;

    [Header("Lists")]
    private List<Vector3> LeftWorldPos = new List<Vector3>();
    private List<Vector3> RightWorldPos = new List<Vector3>();

    private List<Vector3> LeftLocalPos = new List<Vector3>();
    private List<Vector3> RightLocalPos = new List<Vector3>();
    
    private List<Vector3> LeftDifferencePos = new List<Vector3>();
    private List<Vector3> RightDifferencePos = new List<Vector3>();

    public List<DataSubscript> DataFolders = new List<DataSubscript>();

    private float TotalTime;

    //inside
    public Movements CurrentMove;

    public float Leanience;

    public void CreateInfo()
    {
        if (Right.TriggerPressed() == true && Right.GripPressed() == true)
        {
            TotalTime += Time.deltaTime;
            CurrentMovement = true;
            Timer += Time.deltaTime;
            if (Timer > MaxTime)
            {
                RightWorldPos.Add(Right.transform.position);
                RightLocalPos.Add(Right.transform.position - Player.position);

                LeftWorldPos.Add(Left.transform.position);
                LeftLocalPos.Add(Left.transform.position - Player.position);
                //each difference is this local - the last one
                if (LeftLocalPos.Count > 1)
                {
                    LeftDifferencePos.Add(LeftLocalPos[LeftLocalPos.Count - 1] - LeftLocalPos[LeftLocalPos.Count - 2]);
                }
                else
                {
                    LeftDifferencePos.Add(Vector3.zero);
                }

                if (RightLocalPos.Count > 1)
                {
                    RightDifferencePos.Add(RightLocalPos[RightLocalPos.Count - 1] - RightLocalPos[RightLocalPos.Count - 2]);
                }
                else
                {
                    RightDifferencePos.Add(Vector3.zero);
                }
            }
        }
        else
        {
            if (CurrentMovement == true)
            {
                SetScriptableObject(PackageNum);

                RightWorldPos.Clear();
                LeftWorldPos.Clear();

                RightDifferencePos.Clear();
                LeftDifferencePos.Clear();
                
                RightLocalPos.Clear();
                LeftLocalPos.Clear();

                CurrentMovement = false;
            }
            Timer = 0f;
            TotalTime = 0f;
        }
    }
    void Update()
    {
        int num = (int)CurrentMove;
        ScriptableNum.text = "Current Scriptable:  " + PackageNum;
        Set.text = "Is Set: " + DataFolders[num].Storage[PackageNum].Set;
        Type.text = "Testing: " + DataFolders[num].Name;
        CreateInfo();
    }
    public void LoadScriptableObjects(AllData Load)
    {
        //Debug.Log(Load.allTypes.TotalTypes[0].InsideType[0].Interval);
        for (var t = 0; t < Load.allTypes.TotalTypes.Length; t++)//for each type
        {
            for (var i = 0; i < Load.allTypes.TotalTypes[t].InsideType.Length; i++)//for all the units in the type type
            {
                MovementData data = HandDebug.instance.DataFolders[t].Storage[i];
                if (Load.allTypes.TotalTypes[t].InsideType[i].Set == true)
                {
                    List<Vector3> LocalLeft = new List<Vector3>();
                    List<Vector3> WorldLeft = new List<Vector3>();
                    List<Vector3> DifferenceLeft = new List<Vector3>();
                    for (var j = 0; j < Load.allTypes.TotalTypes[t].InsideType[i].LocalLeft.Length / 3; j++)//for each localdata in unit
                    {
                        int ArrayNum = j * 3;
                        Vector3 leftLocal = new Vector3(
                            Load.allTypes.TotalTypes[t].InsideType[i].LocalLeft[ArrayNum],
                            Load.allTypes.TotalTypes[t].InsideType[i].LocalLeft[ArrayNum + 1],
                            Load.allTypes.TotalTypes[t].InsideType[i].LocalLeft[ArrayNum + 2]);
                        LocalLeft.Add(leftLocal);

                        Vector3 leftWorld = new Vector3(
                            Load.allTypes.TotalTypes[t].InsideType[i].WorldLeft[ArrayNum],
                            Load.allTypes.TotalTypes[t].InsideType[i].WorldLeft[ArrayNum + 1],
                            Load.allTypes.TotalTypes[t].InsideType[i].WorldLeft[ArrayNum + 2]);
                        WorldLeft.Add(leftWorld);

                        Vector3 leftDifference = new Vector3(
                            Load.allTypes.TotalTypes[t].InsideType[i].DifferenceLeft[ArrayNum],
                            Load.allTypes.TotalTypes[t].InsideType[i].DifferenceLeft[ArrayNum + 1],
                            Load.allTypes.TotalTypes[t].InsideType[i].DifferenceLeft[ArrayNum + 2]);
                        DifferenceLeft.Add(leftDifference);
                    }

                    List<Vector3> LocalRight = new List<Vector3>();
                    List<Vector3> WorldRight = new List<Vector3>();
                    List<Vector3> DifferenceRight = new List<Vector3>();
                    for (var j = 0; j < Load.allTypes.TotalTypes[t].InsideType[i].LocalRight.Length / 3; j++)
                    {
                        int ArrayNum = j * 3;
                        Vector3 right = new Vector3(
                            Load.allTypes.TotalTypes[t].InsideType[i].LocalRight[ArrayNum],
                            Load.allTypes.TotalTypes[t].InsideType[i].LocalRight[ArrayNum + 1],
                            Load.allTypes.TotalTypes[t].InsideType[i].LocalRight[ArrayNum + 2]);
                        LocalRight.Add(right);

                        Vector3 rightWorld = new Vector3(
                            Load.allTypes.TotalTypes[t].InsideType[i].WorldRight[ArrayNum],
                            Load.allTypes.TotalTypes[t].InsideType[i].WorldRight[ArrayNum + 1],
                            Load.allTypes.TotalTypes[t].InsideType[i].WorldRight[ArrayNum + 2]);
                        WorldRight.Add(rightWorld);

                        Vector3 rightDifference = new Vector3(
                            Load.allTypes.TotalTypes[t].InsideType[i].DifferenceRight[ArrayNum],
                            Load.allTypes.TotalTypes[t].InsideType[i].DifferenceRight[ArrayNum + 1],
                            Load.allTypes.TotalTypes[t].InsideType[i].DifferenceRight[ArrayNum + 2]);
                        DifferenceRight.Add(rightDifference);
                    }
                    //Debug.Log(LocalRight.Count);
                    data.Time = Load.allTypes.TotalTypes[t].InsideType[i].Time;
                    data.Interval = Load.allTypes.TotalTypes[t].InsideType[i].Interval;
                    data.MoveType = (Movements)i;

                    data.RightLocalPos = new List<Vector3>(LocalRight);
                    data.LeftLocalPos = new List<Vector3>(LocalLeft);
                    data.RightWorldPos = new List<Vector3>(WorldRight);
                    data.LeftWorldPos = new List<Vector3>(WorldLeft);
                    data.RightDifferencePos = new List<Vector3>(DifferenceRight);
                    data.LeftDifferencePos = new List<Vector3>(DifferenceLeft);

                    data.Set = Load.allTypes.TotalTypes[t].InsideType[i].Set;
                }
            }
            FinalMovement FinalData = DataFolders[t].FinalInfo;
            //Load.allTypes.TotalTypes[t].Final
            List<Vector3> LocalLeftFinal = new List<Vector3>();
            List<Vector3> WorldLeftFinal = new List<Vector3>();
            List<Vector3> DifferenceLeftFinal = new List<Vector3>();
            for (var j = 0; j < Load.allTypes.TotalTypes[t].Final.LocalLeft.Length / 3; j++)//for each localdata in unit
            {
                int ArrayNum = j * 3;
                Vector3 leftLocal = new Vector3(
                    Load.allTypes.TotalTypes[t].Final.LocalLeft[ArrayNum],
                    Load.allTypes.TotalTypes[t].Final.LocalLeft[ArrayNum + 1],
                    Load.allTypes.TotalTypes[t].Final.LocalLeft[ArrayNum + 2]);
                LocalLeftFinal.Add(leftLocal);

                Vector3 leftWorld = new Vector3(
                    Load.allTypes.TotalTypes[t].Final.WorldLeft[ArrayNum],
                    Load.allTypes.TotalTypes[t].Final.WorldLeft[ArrayNum + 1],
                    Load.allTypes.TotalTypes[t].Final.WorldLeft[ArrayNum + 2]);
                WorldLeftFinal.Add(leftWorld);

                Vector3 leftDifference = new Vector3(
                    Load.allTypes.TotalTypes[t].Final.DifferenceLeft[ArrayNum],
                    Load.allTypes.TotalTypes[t].Final.DifferenceLeft[ArrayNum + 1],
                    Load.allTypes.TotalTypes[t].Final.DifferenceLeft[ArrayNum + 2]);
                DifferenceLeftFinal.Add(leftDifference);
            }

            List<Vector3> LocalRightFinal = new List<Vector3>();
            List<Vector3> WorldRightFinal = new List<Vector3>();
            List<Vector3> DifferenceRightFinal = new List<Vector3>();
            for (var j = 0; j < Load.allTypes.TotalTypes[t].Final.LocalRight.Length / 3; j++)
            {
                int ArrayNum = j * 3;
                Vector3 right = new Vector3(
                    Load.allTypes.TotalTypes[t].Final.LocalRight[ArrayNum],
                    Load.allTypes.TotalTypes[t].Final.LocalRight[ArrayNum + 1],
                    Load.allTypes.TotalTypes[t].Final.LocalRight[ArrayNum + 2]);
                LocalRightFinal.Add(right);

                Vector3 rightWorld = new Vector3(
                    Load.allTypes.TotalTypes[t].Final.WorldRight[ArrayNum],
                    Load.allTypes.TotalTypes[t].Final.WorldRight[ArrayNum + 1],
                    Load.allTypes.TotalTypes[t].Final.WorldRight[ArrayNum + 2]);
                WorldRightFinal.Add(rightWorld);

                Vector3 rightDifference = new Vector3(
                    Load.allTypes.TotalTypes[t].Final.DifferenceRight[ArrayNum],
                    Load.allTypes.TotalTypes[t].Final.DifferenceRight[ArrayNum + 1],
                    Load.allTypes.TotalTypes[t].Final.DifferenceRight[ArrayNum + 2]);
                DifferenceRightFinal.Add(rightDifference);
            }
            FinalData.RightLocalPos = new List<Vector3>(LocalRightFinal);
            FinalData.LeftLocalPos = new List<Vector3>(LocalLeftFinal);
            FinalData.RightWorldPos = new List<Vector3>(WorldRightFinal);
            FinalData.LeftWorldPos = new List<Vector3>(WorldLeftFinal);
            FinalData.RightDifferencePos = new List<Vector3>(DifferenceRightFinal);
            FinalData.LeftDifferencePos = new List<Vector3>(DifferenceLeftFinal);
            FinalData.TotalTime = Load.allTypes.TotalTypes[t].Final.Time;
            FinalData.MoveType = (Movements)t;
        }
    }
    void SetScriptableObject(int Num)
    {
        int Type = (int)CurrentMove;
        DataFolders[Type].Storage[Num].RightWorldPos = new List<Vector3>(RightWorldPos);
        DataFolders[Type].Storage[Num].RightLocalPos = new List<Vector3>(RightLocalPos);
        DataFolders[Type].Storage[Num].RightDifferencePos = new List<Vector3>(RightDifferencePos);

        DataFolders[Type].Storage[Num].LeftWorldPos = new List<Vector3>(LeftWorldPos);
        DataFolders[Type].Storage[Num].LeftLocalPos = new List<Vector3>(LeftLocalPos);
        DataFolders[Type].Storage[Num].LeftDifferencePos = new List<Vector3>(LeftDifferencePos);

        DataFolders[Type].Storage[Num].Time = TotalTime;
        DataFolders[Type].Storage[Num].Interval = MaxTime;
        DataFolders[Type].Storage[Num].Set = true;
    }
    public void ChangeCurrentStorage(int Add)
    {
        if (PackageNum + Add > -1 && PackageNum + Add < DataFolders[(int)CurrentMove].Storage.Count)
        {
            PackageNum += Add;
        }
    }

    public void Reset()
    {
        Debug.Log("reset");
        DataFolders[(int)CurrentMove].Storage[PackageNum].RightWorldPos.Clear();
        DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos.Clear();
        DataFolders[(int)CurrentMove].Storage[PackageNum].RightDifferencePos.Clear();
        DataFolders[(int)CurrentMove].Storage[PackageNum].Time = 0f;
        DataFolders[(int)CurrentMove].Storage[PackageNum].Interval = 0f;
        DataFolders[(int)CurrentMove].Storage[PackageNum].Set = false;
    }

    void Start()
    {
        MaxTime = 1f / CountEverySecond;
        SaveScript.LoadGameLarge();
        //MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        //meshRenderer.material = Normal;
    }

    public void ChangeType()
    {
        int Current = (int)CurrentMove;
        int Max = System.Enum.GetValues(typeof(Movements)).Length;
        if (Current + 1 == Max)
        {
            Current = 0;
        }
        else
        {
            Current += 1;
        }
        CurrentMove = (Movements)Current;
    }

    public void GetAverage()
    {
        Debug.Log("getaverage");
        List<Vector3> LocalLeft = new List<Vector3>(1000);
        List<Vector3> LocalRight = new List<Vector3>(1000);

        List<Vector3> AverageLocalLeft = new List<Vector3>(1000);
        List<Vector3> AverageLocalRight = new List<Vector3>(1000);

        int Count = 0;
        for (int i = 0; i < DataFolders[(int)CurrentMove].Storage.Count; i++)
        {
            Count += 1;
            if (DataFolders[(int)CurrentMove].Storage[i] == true)
            {
                for (int j = 0; j < DataFolders[(int)CurrentMove].Storage[i].LeftLocalPos.Count; j++)
                {
                    Vector3 localRight = DataFolders[(int)CurrentMove].Storage[i].RightLocalPos[j];
                    Vector3 localLeft = DataFolders[(int)CurrentMove].Storage[i].LeftLocalPos[j];
                    LocalLeft[j] += localLeft;
                    LocalRight[j] += localRight;
                }
            }
        }
        for (int i = 0; i < LocalLeft.Count; i++)
        {
            AverageLocalLeft[i] = LocalLeft[i] / Count;
            AverageLocalRight[i] = LocalRight[i] / Count;
        }

        DataFolders[(int)CurrentMove].FinalInfo.LeftLocalPos = new List<Vector3>(AverageLocalLeft);
        DataFolders[(int)CurrentMove].FinalInfo.RightLocalPos = new List<Vector3>(AverageLocalRight);
        DataFolders[(int)CurrentMove].FinalInfo.Set = true;
        //
    }

    public void SaveStats()
    {
        SaveScript.SaveStats();
    }
}
