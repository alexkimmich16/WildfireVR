using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ZoneController : MonoBehaviour
{
    public static ZoneController instance;
    void Awake() { instance = this; }

    [Range(0f, 1f)]
    public float MagicLinePos;
    public VisualEffect MagicLine;

    public GameObject Zone1Weak;
    public GameObject Zone2Weak;
    public GameObject Zone1Strong;
    public GameObject Zone2Strong;

    public float Weak1;
    public float Weak2;
    public float Strong1;
    public float Strong2;

    public List<GameObject> Players1;
    public List<GameObject> Players2;

    public float Team1Distance;
    public float Team2Distance;

    public Vector2 ZoneWeak(int Zone)
    {
        //0 is low
        if (Zone == 0)
        {
            float SideMin = WorldPos.x;
            float FarSide = SideMin + WeakSpace;
            return new Vector2(SideMin , FarSide);
        }
        else
        {
            float Sidemax = WorldPos.y;
            float FarSide = Sidemax - WeakSpace;
            return new Vector2(FarSide, Sidemax);
        }
    }
    public Vector2 ZoneStrong(int Zone)
    {
        //0 is low
        if (Zone == 0)
        {
            float linePos = MagicLineWorldPos(MagicLinePos);
            float FarSide = linePos - StrongSpace;
            return new Vector2(FarSide, linePos);
            //short of magic line
        }
        else
        {
            float linePos = MagicLineWorldPos(MagicLinePos);
            float FarSide = linePos + StrongSpace;
            return new Vector2(linePos, FarSide);
        }
    }
    [Header("ZoneInfo")]
    public float StrongSpace;
    public float WeakSpace;
    public Vector2 WorldPos;
    private float TotalSize;
    public float LineMoveSpeed;
    void Start()
    {
        TotalSize = WorldPos.y - WorldPos.x;

        //set magic line to half
        MagicLinePos = 0.5f;
    }
    
    public void SetPosition(GameObject obj, Vector2 pos)
    {
        float PosFloat = (pos.x + pos.y) / 2;
        Vector3 newPos = new Vector3(PosFloat, obj.transform.position.y, obj.transform.position.z);
        obj.transform.position = newPos;

        Vector3 newScale = new Vector3(pos.y - pos.x, obj.transform.localScale.y, obj.transform.localScale.z);
        obj.transform.localScale = newScale;
    }
    void Update()
    {
        Vector3 Pos = MagicLine.transform.position;
        MagicLine.transform.position = new Vector3(MagicLineWorldPos(MagicLinePos), Pos.y, Pos.z);

        Weak1 = (ZoneWeak(0).x + ZoneWeak(0).y) / 2;
        Weak2 = (ZoneWeak(1).x + ZoneWeak(1).y) / 2;
        Strong1 = (ZoneStrong(0).x + ZoneStrong(0).y) / 2;
        Strong2 = (ZoneStrong(1).x + ZoneStrong(1).y) / 2;

        SetPosition(Zone1Weak, ZoneWeak(0));
        SetPosition(Zone2Weak, ZoneWeak(1));
        SetPosition(Zone1Strong, ZoneStrong(0));
        SetPosition(Zone2Strong, ZoneStrong(1));

        AdjustLine();
    }


    public float MagicLineWorldPos(float Percent)
    {
        return (TotalSize * Percent) + WorldPos.x;
    }
    float MagicLinePercent(float WorldPosition)
    {
        return (WorldPosition - WorldPos.x) / TotalSize;
    }

    public void AdjustLine()
    {
        float Team1Total = 0;
        for (int i = 0; i < Players1.Count; i++)
        {
            Team1Total += Players1[i].transform.position.x;
        }
        float Team1Average = Team1Total / Players1.Count;
        float Team1Percent = MagicLinePercent(Team1Average);
        Team1Distance = MagicLinePos - Team1Percent;

        float Team2Total = 0;
        for (int i = 0; i < Players2.Count; i++)
        {
            Team2Total += Players2[i].transform.position.x;
        }
        float Team2Average = Team2Total / Players2.Count;
        float Team2Percent = MagicLinePercent(Team2Average);
        Team2Distance = Team2Percent - MagicLinePos;

        if(Team1Distance > Team2Distance)
        {
            MagicLinePos -= Time.deltaTime * LineMoveSpeed;
        }
        else if (Team1Distance < Team2Distance)
        {
            MagicLinePos += Time.deltaTime * LineMoveSpeed;
        }
    }
}
