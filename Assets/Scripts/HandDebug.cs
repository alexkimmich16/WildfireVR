using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public TextMeshProUGUI ScriptableNum;
    public TextMeshProUGUI Set;
    public TextMeshProUGUI Type;
    public TextMeshProUGUI RecordingEnum;

    [Header("OnePackageOnly")]
    public int PackageNum;

    [Header("Lists")]
    private List<Vector3> LeftWorldPos = new List<Vector3>();
    private List<Vector3> RightWorldPos = new List<Vector3>();

    private List<Vector3> LeftLocalPos = new List<Vector3>();
    private List<Vector3> RightLocalPos = new List<Vector3>();
    
    private List<Vector3> LeftDifferencePos = new List<Vector3>();
    private List<Vector3> RightDifferencePos = new List<Vector3>();

    private List<Vector3> LeftRotation = new List<Vector3>();
    private List<Vector3> RightRotation = new List<Vector3>();

    public List<DataSubscript> DataFolders = new List<DataSubscript>();

    private HandMagic HM;
    //predetermined, and works for everyone 

    private float TotalTime;

    //inside
    public Movements CurrentMove;

    public float Leanience;
    
    [Header("Lerp")]
    public float EndLerp; // 10
    public float StartLerp; // 1
    public TextMeshProUGUI Max;
    public TextMeshProUGUI Min;
    public bool InvertAngle;
    public Toggle InvertedToggle;

    [Header("Frames")]
    public TextMeshProUGUI CurrentFrames;
    public int Frames;

    [Header("Health")]
    public List<TextMeshProUGUI> Health = new List<TextMeshProUGUI>();
    public Slider MagicSlider;

    public Transform Spawn;

    public void InvertAng()
    {
        InvertAngle = InvertedToggle.isOn;
    }
    public void ChangeFrame(int Change)
    {
        int Positive = Mathf.Abs(Change);
        MovementData data = DataFolders[(int)CurrentMove].Storage[PackageNum];
        for (var t = 0; t < Positive; t++)
        {
            int Count = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos.Count;
            if (Mathf.Sign(Change) == 1)
            {
                data.RightLocalPos.Add(data.RightLocalPos[Count - 1]);
                data.LeftLocalPos.Add(data.LeftLocalPos[Count - 1]);
            }
            else
            {
                data.RightLocalPos[Count - 2] = data.RightLocalPos[Count - 1];
                data.LeftLocalPos[Count - 2] = data.LeftLocalPos[Count - 1];
                data.RightLocalPos.RemoveAt(Count - 1);
                data.LeftLocalPos.RemoveAt(Count - 1);
            }
        }
        int FinCount = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos.Count - 1;

        float StartX = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos[0].x;
        float EndX = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos[FinCount].x;
        //Debug.Log("startx:  " + StartX + "  End:  " + EndX);
        Lerp(0, StartX, EndX);

        float StartY = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos[0].y;
        float EndY = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos[FinCount].y;
        //Debug.Log("starty:  " + StartY + "  End:  " + EndY);
        Lerp(1, StartY, EndY);

        float StartZ = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos[0].z;
        float EndZ = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos[FinCount].z;
        //Debug.Log("startz:  " + StartZ + "  End:  " + EndZ);
        Lerp(2, StartZ, EndZ);
        //stretch out motion to final frame
    }
    #region Lerp
    public void ButtonLerp(int Type)
    {
        Lerp(Type, StartLerp, EndLerp);
    }
    public void Lerp(int Type, float Start, float End)
    {
        int Inside = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos.Count - 1; //26
        float Difference = End - Start; //0.5
        float StepAdd = Difference / Inside;
        float Current = Start;
        MovementData data = DataFolders[(int)CurrentMove].Storage[PackageNum];
        //Debug.Log("inside:  " + Inside + "  diff:  " + Difference + "   step:  " + StepAdd.ToString("F3") + "  AveragePos:  " + Current.ToString("F3"));
        for (var t = 0; t < Inside; t++)
        {
            if (Type == 0)
            {
                data.RightLocalPos[t] = new Vector3(Current, data.RightLocalPos[t].y, data.RightLocalPos[t].z);
                data.LeftLocalPos[t] = new Vector3(Current, data.LeftLocalPos[t].y, data.LeftLocalPos[t].z);
                if (t == Inside)
                {
                    data.RightLocalPos[t + 1] = new Vector3(End, data.RightLocalPos[t].y, data.RightLocalPos[t].z);
                    data.LeftLocalPos[t + 1] = new Vector3(End, data.LeftLocalPos[t].y, data.LeftLocalPos[t].z);
                }
            }
            else if (Type == 1)
            {
                if (InvertAngle == false)
                {
                    data.RightLocalPos[t] = new Vector3(data.RightLocalPos[t].x, Current, data.RightLocalPos[t].z);
                    data.LeftLocalPos[t] = new Vector3(data.LeftLocalPos[t].x, 360f - Current, data.LeftLocalPos[t].z);
                    if (t == Inside)
                    {
                        data.RightLocalPos[t + 1] = new Vector3(data.RightLocalPos[t].x, End, data.RightLocalPos[t].z);
                        data.LeftLocalPos[t + 1] = new Vector3(data.LeftLocalPos[t].x, 360f - End, data.LeftLocalPos[t].z);
                    }
                }
                else
                {
                    data.RightLocalPos[t] = new Vector3(data.RightLocalPos[t].x, 360f - Current, data.RightLocalPos[t].z);
                    data.LeftLocalPos[t] = new Vector3(data.LeftLocalPos[t].x, Current, data.LeftLocalPos[t].z);
                    if (t == Inside)
                    {
                        data.RightLocalPos[t + 1] = new Vector3(data.RightLocalPos[t].x, 360f - End, data.RightLocalPos[t].z);
                        data.LeftLocalPos[t + 1] = new Vector3(data.LeftLocalPos[t].x, End, data.LeftLocalPos[t].z);
                    }
                }
                
            }
            else if (Type == 2)
            {
                data.RightLocalPos[t] = new Vector3(data.RightLocalPos[t].x, data.RightLocalPos[t].y, Current);
                data.LeftLocalPos[t] = new Vector3(data.LeftLocalPos[t].x, data.LeftLocalPos[t].y, Current);
                if (t == Inside)
                {
                    data.RightLocalPos[t + 1] = new Vector3(data.RightLocalPos[t].x, data.RightLocalPos[t].y, End);
                    data.LeftLocalPos[t + 1] = new Vector3(data.LeftLocalPos[t].x, data.LeftLocalPos[t].y, End);
                }
            }
            Current += StepAdd;
        }
        
    }
    public void ChangeMax(float Add)
    {
        EndLerp += Add;
    }
    public void ChangeMin(float Add)
    {
        StartLerp += Add;
    }
    #endregion
    public void CreateInfo()
    {
        if (HM.Controllers[1].TriggerPressed() == true && HM.Controllers[1].GripPressed() == true)
        {
            HM.Controllers[0].SettingStats = true;
            HM.Controllers[1].SettingStats = true;
            TotalTime += Time.deltaTime;
            CurrentMovement = true;
            Timer += Time.deltaTime;
            
            if (Timer > MaxTime)
            {
                
                Vector3 ZPlacementController = new Vector3(HM.Controllers[1].transform.position.x, HM.Cam.position.y, HM.Controllers[1].transform.position.z);
                float YChange = HM.Controllers[1].transform.position.y - HM.Cam.position.y;
                float Dist = Vector3.Distance(ZPlacementController, HM.Cam.position);
                Vector3 targetDir = ZPlacementController + HM.Cam.transform.position;
                float angle = Vector3.Angle(targetDir, HM.Cam.transform.forward);
                RightLocalPos.Add(new Vector3(Dist, angle + 180, YChange));
                LeftLocalPos.Add(new Vector3(Dist, -angle + 180, YChange));
                LeftRotation.Add(new Vector3(HM.Controllers[0].transform.rotation.x, HM.Controllers[0].transform.rotation.y, HM.Controllers[0].transform.rotation.z));
                RightRotation.Add(new Vector3(HM.Controllers[1].transform.rotation.x, HM.Controllers[1].transform.rotation.y, HM.Controllers[1].transform.rotation.z));

                SpellType CastType = HandMagic.instance.Spells[(int)CurrentMove].Type;
                if (CastType == SpellType.Individual)
                {
                    
                }
                else if (CastType == SpellType.Both)
                {
                    
                    
                }
                
            }
        }
        else
        {
            HM.Controllers[0].SettingStats = false;
            HM.Controllers[1].SettingStats = false;
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
        Frames = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos.Count;
        CurrentFrames.text = "Frames:  " + Frames;
        ScriptableNum.text = "Current Scriptable:  " + PackageNum;
        Set.text = "Is Set: " + DataFolders[num].Storage[PackageNum].Set;
        Type.text = "Testing: " + DataFolders[num].Name;
        Max.text = "End: " + EndLerp.ToString("F1");
        Min.text = "Start: " + StartLerp.ToString("F1");
        RecordingEnum.text = "State: " + RockVR.Video.VideoCaptureCtrl.instance.status.ToString();

        if (HM.InfiniteMagic == false)
            MagicSlider.value = HM.CurrentMagic;
        else
            MagicSlider.value = HM.MaxMagic;

        CreateInfo();
    }
    public void LoadChildScriptableObjects(AllData Load)
    {
        HandMagic HM = HandMagic.instance;
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

                    data.Set = Load.allTypes.TotalTypes[t].InsideType[i].Set;
                }
            }
        }
    }
    void SetScriptableObject(int Num)
    {
        int Type = (int)CurrentMove;
        //DataFolders[Type].Storage[Num].RightWorldPos = new List<Vector3>(RightWorldPos);
        DataFolders[Type].Storage[Num].RightLocalPos = new List<Vector3>(RightLocalPos);
        //DataFolders[Type].Storage[Num].RightDifferencePos = new List<Vector3>(RightDifferencePos);

        //DataFolders[Type].Storage[Num].LeftWorldPos = new List<Vector3>(LeftWorldPos);
        DataFolders[Type].Storage[Num].LeftLocalPos = new List<Vector3>(LeftLocalPos);
        //DataFolders[Type].Storage[Num].LeftDifferencePos = new List<Vector3>(LeftDifferencePos);

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
        //Debug.Log("reset");
        DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos.Clear();
        DataFolders[(int)CurrentMove].Storage[PackageNum].LeftLocalPos.Clear();

        DataFolders[(int)CurrentMove].Storage[PackageNum].LeftRotation.Clear();
        DataFolders[(int)CurrentMove].Storage[PackageNum].RightRotation.Clear();
        DataFolders[(int)CurrentMove].Storage[PackageNum].Time = 0f;
        DataFolders[(int)CurrentMove].Storage[PackageNum].Interval = 0f;
        DataFolders[(int)CurrentMove].Storage[PackageNum].Set = false;

        //DataFolders[(int)CurrentMove].Storage[PackageNum].RightDifferencePos.Clear();
        //DataFolders[(int)CurrentMove].Storage[PackageNum].LeftDifferencePos.Clear();
    }
    void Start()
    {
        HM = HandMagic.instance;
        Spawn = GameObject.Find("/Objects/Emptys/Spawn").transform;
        MaxTime = 1f / CountEverySecond;
        SaveScript.LoadGameLarge();
        MagicSlider.maxValue = HM.MaxMagic;
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
        HandMagic.instance.Spells[(int)CurrentMove].FinalInfo.LeftLocalPos = new List<Vector3>(AverageLocalLeft);
        HandMagic.instance.Spells[(int)CurrentMove].FinalInfo.RightLocalPos = new List<Vector3>(AverageLocalRight);
        HandMagic.instance.Spells[(int)CurrentMove].FinalInfo.Set = true;
        //
    }
    public void SaveStats()
    {
        SaveScript.SaveStats();
    }
    public void InvertSides()
    {
        int Inside = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos.Count - 1;
        List<Vector3> LeftSide = DataFolders[(int)CurrentMove].Storage[PackageNum].LeftLocalPos;
        List<Vector3> RightSide = DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos;
        DataFolders[(int)CurrentMove].Storage[PackageNum].RightLocalPos = LeftSide;
        DataFolders[(int)CurrentMove].Storage[PackageNum].LeftLocalPos = RightSide;

    }
}