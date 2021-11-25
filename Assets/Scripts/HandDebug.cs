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
    void Awake() { instance = this; DontDestroyOnLoad(this.gameObject); }

    #endregion

    public float CountEverySecond = 20;
    private float MaxTime;
    private float Timer;
    public int MoveNum;
    private bool CurrentMovement = false;

    public Transform Player;

    public HandActions Left;
    public HandActions Right;

    //public WallUI wall;

    public TextMeshProUGUI ScriptableNum;
    public TextMeshProUGUI Set;
    public TextMeshProUGUI Type;

    [Header("OnePackageOnly")]
    public bool ResetWallPackage;
    public int PackageNum;

    [Header("Lists")]
    private List<Vector3> WorldPos = new List<Vector3>();
    private List<Vector3> LocalPos = new List<Vector3>();
    private List<Vector3> DifferencePos = new List<Vector3>();

    public List<DataSubscript> DataFolders = new List<DataSubscript>();

    private float TotalTime;

    //inside
    public Movements CurrentMove;

    public float Leanience;
    /*
    public Material Normal;
    public Material Fire;
    */

    void Update()
    {
        //MoveNum
        
        int num = (int)CurrentMove;
        ScriptableNum.text = "Current Scriptable:  " + PackageNum;
        Set.text = "Is Set: " + DataFolders[num].Storage[PackageNum].Set;
        Type.text = "Testing: " + DataFolders[num].Name;

        if (Right.TriggerPressed() == true && Right.GripPressed() == true)
        {
            TotalTime += Time.deltaTime;
            CurrentMovement = true;
            Timer += Time.deltaTime;
            if (Timer > MaxTime)
            {
                WorldPos.Add(Right.transform.position);
                LocalPos.Add(Right.transform.position - Player.position);
                //PackageNum
                //each difference is this local - the last one
                if (LocalPos.Count > 1)
                {
                    DifferencePos.Add(LocalPos[LocalPos.Count - 1] - LocalPos[LocalPos.Count - 2]);
                }
                else
                {
                    DifferencePos.Add(Vector3.zero);
                }
            }
        }
        else
        {
            if (CurrentMovement == true)
            {
                if (ResetWallPackage == true)
                {
                    SetScriptableObject(PackageNum);
                }
                else
                {
                    SetScriptableObject(MoveNum);
                }
                
                WorldPos.Clear();
                LocalPos.Clear();
                DifferencePos.Clear();

                CurrentMovement = false;
                MoveNum += 1;
            }
            Timer = 0f;
            TotalTime = 0f;
        }
    }

    void SetScriptableObject(int Num)
    {
        int Type = (int)CurrentMove;
        DataFolders[Type].Storage[Num].RightWorldPos = new List<Vector3>(WorldPos);
        DataFolders[Type].Storage[Num].RightLocalPos = new List<Vector3>(LocalPos);
        DataFolders[Type].Storage[Num].RightDifferencePos = new List<Vector3>(DifferencePos);
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
}
