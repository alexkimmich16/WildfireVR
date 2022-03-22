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


    public float FramesPerSecond;
    private float EachFrame;
    private float Timer;

    private GameObject SpawnedObject;

    public static float CastTime = 5f;
    private bool ShouldCountFrames = true;

    private void Start()
    {
        ShouldCountFrames = true;
        Sides = new List<GameObject>(2);
        //0 is left
        //hands:hands_geom
        Sides.Add(transform.Find("LeftHand").gameObject);
        Sides.Add(transform.Find("RightHand").gameObject);

        HM = HandMagic.instance;
        for (int i = 0; i < Sides.Count; i++)
            Sides[i].transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material = HM.Materials[(int)movement];
        EachFrame = 1 / FramesPerSecond;
    }
    void Update()
    {
        CountTimer();
        int move = (int)movement;
        if (Current > HM.Spells[move].FinalInfo.LeftLocalPos.Count - 1)
        {
            CastSpell();
        }

        for (int i = 0; i < Sides.Count; i++)
        {
            Sides[i].transform.position = ConvertPoint(GetLocalPosSide(i, move, Current));
            Sides[i].transform.eulerAngles = GetRotationSide(i, move, Current);
        }

        
        //then display frames    
    }
    void CastSpell()
    {
        Current = 0;
        Timer = 0;
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
        ShouldCountFrames = true;
        //end cast
    }

    public Vector3 ConvertPoint(Vector3 Local)
    {
        float Distance = Local.x;
        float RotationOffset = Local.y;
        float HorizonalOffset = Local.z;
        Quaternion rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + RotationOffset, 0);
        Vector3 Forward = rotation * Vector3.forward;
        Ray r = new Ray(transform.position, Forward);
        Vector3 YPosition = r.GetPoint(Distance);
        return new Vector3(YPosition.x, HorizonalOffset + transform.position.y, YPosition.z);
    }
}
