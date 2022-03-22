using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Odin.MagicHelp;

public class MovementDisplay : MonoBehaviour
{
    public Movements movement;
    private List<GameObject> Sides = new List<GameObject>(2);
    private int Current;
    private HandMagic HM;
    public Vector3 Offset;


    public float FramesPerSecond;
    private float EachFrame;
    private float Timer;

    private GameObject SpawnedObject;

    public static float CastTime = 5f;
    private bool ShouldCountFrames = true;
    [Range(0.1f, 5)]
    public float Scale;

    private void Start()
    {
        ShouldCountFrames = true;
        Sides = new List<GameObject>(2);
        //0 is left
        Sides.Add(transform.GetChild(0).Find("LeftHand").gameObject);
        Sides.Add(transform.GetChild(0).Find("RightHand").gameObject);

        HM = HandMagic.instance;
        for (int i = 0; i < Sides.Count; i++)
            Sides[i].transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = HM.Materials[(int)movement];
        EachFrame = 1 / FramesPerSecond;
    }
    void Update()
    {
        if (ShouldCountFrames == false)
            return;
        
        int move = (int)movement;
        Debug.Log("Count: " + HM.Spells[move].FinalInfo.LeftLocalPos.Count + "Current: " + Current);
        if (Current > HM.Spells[move].FinalInfo.LeftLocalPos.Count - 2)
            CastSpell();
        else if(ShouldCountFrames == true)
        {
            CountTimer();

            for (int i = 0; i < Sides.Count; i++)
            {
                Vector3 CurrentPos;
                if (i == 0)
                    CurrentPos = HandMagic.instance.Spells[move].FinalInfo.LeftLocalPos[Current];
                else
                    CurrentPos = HandMagic.instance.Spells[move].FinalInfo.RightLocalPos[Current];
                Sides[i].transform.position = ConvertPoint(CurrentPos) + Offset;
                Sides[i].transform.eulerAngles = GetRotationSide(i, move, Current);
            }
        }  
    }
    void CastSpell()
    {
        ShouldCountFrames = false;
        if ((int)movement == 0)
        {
            //spike
        }
        else if((int)movement == 1)
        {
            SpawnedObject = Instantiate(Resources.Load("NewFire") as GameObject, Sides[0].transform.position, Sides[0].transform.rotation);
        }
        else if ((int)movement == 2)
        {
            //shield
        }
        else if ((int)movement == 3)
        {
            //push
        }
        SpawnedObject.transform.parent = Sides[0].transform;
        StartCoroutine(WaitForCastEnd());
    }
    void CountTimer()
    {
        if(ShouldCountFrames == true)
        {
            Timer += Time.deltaTime;
            if (Timer > EachFrame)
            {
                Current += 1;
                Timer = 0f;
            }
        }
    }
    public IEnumerator WaitForCastEnd()
    {
        yield return new WaitForSeconds(CastTime);
        Destroy(SpawnedObject);
        Current = 0;
        Timer = 0;
        ShouldCountFrames = true;
        //end cast
    }

    public Vector3 ConvertPoint(Vector3 Local)
    {
        float Distance = Local.x * Scale;
        float RotationOffset = Local.y;
        float HorizonalOffset = Local.z * Scale;
        Quaternion rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + RotationOffset, 0);
        Vector3 Forward = rotation * Vector3.forward;
        Ray r = new Ray(transform.position, Forward);
        Vector3 YPosition = r.GetPoint(Distance);
        return new Vector3(YPosition.x, HorizonalOffset + transform.position.y, YPosition.z);
    }
}
