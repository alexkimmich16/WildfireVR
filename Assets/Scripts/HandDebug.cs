using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum Movements
{
    Spike = 0,
    Fireball = 1,
    Shield = 2,
    Push = 3,
}
public class HandDebug : MonoBehaviour
{
    //check all motions through list
    //store info inside list 
    #region Singleton + Classes
    public static HandDebug instance;
    void Awake() { instance = this; }

    [System.Serializable]
    public class DataSubscript
    {
        public string Name;
        public FinalMovement FinalInfo;
        public List<MovementData> Storage = new List<MovementData>();
    }
    [System.Serializable]
    public class StaticFinalMovements
    {
        public string Name;
        public StaticDataFolder FinalInfo;
    }
    [System.Serializable]
    public class StaticDataFolder
    {
        public string Name;
        public FinalMovement FinalInfo;

        public List<Vector3> LeftLocalPos;
        public List<Vector3> LeftWorldPos;
        public List<Vector3> LeftDifferencePos;

        public List<Vector3> RightLocalPos;
        public List<Vector3> RightWorldPos;
        public List<Vector3> RightDifferencePos;

        public float Time;
        public float Interval;

        public bool Set = false;
    }
    #endregion

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

    //predetermined, and works for everyone
    public List<StaticFinalMovements> StaticFolders = new List<StaticFinalMovements>();

    private float TotalTime;

    //inside
    public Movements CurrentMove;

    public float Leanience;

    public bool EngineStats = false;

    public void CreateInfo()
    {
        if (Right.TriggerPressed() == true && Right.GripPressed() == true)
        {
            Left.SettingStats = true;
            Right.SettingStats = true;
            TotalTime += Time.deltaTime;
            CurrentMovement = true;
            Timer += Time.deltaTime;
            
            if (Timer > MaxTime)
            {
                SpellType CastType = HandMagic.instance.Spells[(int)CurrentMove].Type;
                if ((int)CastType == 0)
                {
                    //individual so mirror on right for left

                    //x is distance
                    //y is direction of vr y axis
                    //z is up/down add
                    
                    
                    Vector3 ZPlacementController = new Vector3(Right.transform.position.x - Player.position.x, Player.position.y, Right.transform.position.z - Player.position.z );
                    float YChange = Right.transform.position.y - Player.position.y;
                    //than add this back
                    float Dist = Vector3.Distance(ZPlacementController, Player.position);

                    Vector3 targetDir = ZPlacementController + Player.transform.position;
                    //angle


                    //float RotationFromHead = Vector3.SignedAngle(targetDir, Player.transform.forward, Vector3.up);

                    //if PROBLEM IS THIS
                    float angle = Vector3.Angle(targetDir, Player.transform.forward);
                    Vector3 RightLocalInfo = new Vector3(Dist, 0, YChange);
                    RightLocalPos.Add(RightLocalInfo);

                    //for left just invert angle of other

                    //Vector3 LeftLocalInfo = Right.transform.position - Player.position;
                    LeftLocalPos.Add(new Vector3(Dist, -0, YChange));
                    //each difference is this local - the last one
                }
                else if ((int)CastType == 1)
                {
                    //track both seperately
                    //individual so mirror on right for left

                    LeftLocalPos.Add(Left.transform.position - Player.position);
                    RightLocalPos.Add(Right.transform.position - Player.position);
                }
                
            }
        }
        else
        {
            Left.SettingStats = false;
            Right.SettingStats = false;
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
                MovementData data = DataFolders[t].Storage[i];
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
        //Debug.Log("getaverage1");
        List<Vector3> LocalLeft = new List<Vector3>();
        List<Vector3> LocalRight = new List<Vector3>();

        List<Vector3> AverageLocalLeft = new List<Vector3>();
        List<Vector3> AverageLocalRight = new List<Vector3>();
        //Debug.Log("getaverage2");
        int Count = 0;
        for (int i = 0; i < DataFolders[(int)CurrentMove].Storage.Count; i++)
        {
           // Debug.Log("getaverage3");
            if (DataFolders[(int)CurrentMove].Storage[i].Set == true)
            {
                Count += 1;
                //Debug.Log("getaverage4");
                for (int j = 0; j < DataFolders[(int)CurrentMove].Storage[i].LeftLocalPos.Count; j++)
                {
                    //Debug.Log("getaverage5");
                    if (j + 1 > LocalLeft.Count)
                    {
                        //Debug.Log("getaverage6");
                        LocalLeft.Add(new Vector3(0, 0, 0));
                        LocalRight.Add(new Vector3(0, 0, 0));
                    }
                    //Debug.Log("getaverage7");
                    Vector3 localRight = DataFolders[(int)CurrentMove].Storage[i].RightLocalPos[j];
                    Vector3 localLeft = DataFolders[(int)CurrentMove].Storage[i].LeftLocalPos[j];
                    LocalLeft[j] = new Vector3(LocalLeft[j].x + localLeft.x, LocalLeft[j].y + localLeft.y, LocalLeft[j].z + localLeft.z);
                    LocalRight[j] = new Vector3(LocalRight[j].x + localRight.x, LocalRight[j].y + localRight.y, LocalRight[j].z + localRight.z);

                }
            }
        }
        for (int i = 0; i < LocalLeft.Count; i++)
        {
            AverageLocalLeft.Add(new Vector3(LocalLeft[i].x / Count, LocalLeft[i].y / Count, LocalLeft[i].z / Count));
            AverageLocalRight.Add(new Vector3(LocalRight[i].x / Count, LocalRight[i].y / Count, LocalRight[i].z / Count));
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

public enum DirectionShip
{
    Down = 0,
    Forward = 1,
    Back = 2,
    Left = 3,
    Right = 4,
    Up = 5,
}
//LeftWorldPos.Add(new Vector3(Left.transform.position.x, Right.transform.position.y, Right.transform.position.z));
//RightWorldPos.Add(Right.transform.position);
/*
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
*/