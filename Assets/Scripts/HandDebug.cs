using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandDebug : MonoBehaviour
{
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

    [Header("OnePackageOnly")]
    public bool ResetWallPackage;
    public int PackageNum;

    [Header("Lists")]
    public List<Vector3> WorldPos = new List<Vector3>();
    public List<Vector3> LocalPos = new List<Vector3>();
    public List<Vector3> DifferencePos = new List<Vector3>();
    public List<MovementData> MoveStorage = new List<MovementData>();

    private float TotalTime;
    
    void Update()
    {
        //MoveNum
        ScriptableNum.text = "Current Scriptable:  " + PackageNum;
        Set.text = "Is Set: " + MoveStorage[PackageNum].Set;

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

    void SetScriptableObject(int Set)
    {
        MoveStorage[Set].RightWorldPos = new List<Vector3>(WorldPos);
        MoveStorage[Set].RightLocalPos = new List<Vector3>(LocalPos);
        MoveStorage[Set].RightDifferencePos = new List<Vector3>(DifferencePos);
        MoveStorage[Set].Time = TotalTime;
        MoveStorage[Set].Interval = MaxTime;
        MoveStorage[Set].Set = true;
    }

    public void ChangeCurrentStorage(int Add)
    {
        if (PackageNum + Add > -1 && PackageNum + Add < MoveStorage.Count)
        {
            PackageNum += Add;
        }
    }

    public void Reset()
    {
        MoveStorage[PackageNum].RightWorldPos.Clear();
        MoveStorage[PackageNum].RightLocalPos.Clear();
        MoveStorage[PackageNum].RightDifferencePos.Clear();
        MoveStorage[PackageNum].Time = 0f;
        MoveStorage[PackageNum].Interval = 0f;
        MoveStorage[PackageNum].Set = false;
    }

    void Start()
    {
        MaxTime = 1f / CountEverySecond;
    }
}
