using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RestrictionSystem;
using UnityEngine.XR;
using Sirenix.OdinInspector;
public enum ControlType
{
    OnMoveHand = 0,
    Constant = 1,
    Directional = 2,
}
public class FireballController : SerializedMonoBehaviour
{
    public static FireballController instance;
    private void Awake() { instance = this; }

    public bool IsControlling;
    [Header("Stats")]
    public float CastDistance;
    public float MinDistanceToHead;
    public float StopControllingDistance;

    public float DirectionLeaniency;
    
    private GameObject OnlineFireball;

    public float ControlForce;
    public ControlType controlType;



    [Header("References")]
    public GameObject PositionExample;

    [Header("Debug")]
    public bool ShouldDebug;
    public float Display;

    //public int Min, Max;
    //public GameObject TestOBJ;

    public List<bool> Actives;
    public List<Vector3> StartPos;
    //public bool justHand, Both;

    public int FramesAgoRotation = 5;


    public float HandDirectionValue, CamDirectionValue;
    public Vector2 OutPutDir;
    public Quaternion SpawnRotation(Side side)
    {
        SingleInfo StartFrame = PastFrameRecorder.instance.PastFrame(side, FramesAgoRotation);
        SingleInfo EndFrame = PastFrameRecorder.instance.GetControllerInfo(side);

        Vector3 HandDirection = (StartFrame.HandPos - EndFrame.HandPos).normalized;
        float HandDirectionValue = ToDegrees(new Vector2(HandDirection.x, HandDirection.z));

        float CamDirectionValue = AIMagicControl.instance.Cam.eulerAngles.y;
        float Both = HandDirectionValue - CamDirectionValue;
        Both = Both - 180;
        Vector2 OutPutDir = ToVector(Both);
        Vector3 RealOutput = new Vector3(OutPutDir.x * 360f, 0, OutPutDir.y * 360f);
        return Quaternion.LookRotation(RealOutput);
    }

    public void EndCount(Side side)
    {
        if (Vector3.Distance(StartPos[(int)side], AIMagicControl.instance.Hands[(int)side].localPosition) > 0.2 && ShouldDebug)
            Debug.Log(Vector3.Distance(StartPos[(int)side], AIMagicControl.instance.Hands[(int)side].localPosition));
        if (Vector3.Distance(StartPos[(int)side], AIMagicControl.instance.Hands[(int)side].localPosition) > CastDistance)
        {
            //ControlPos = AIMagicControl.instance.Hands[(int)side].localPosition;
            //StartCoroutine(WaitForClose());
            SpawnFireball(side);
            ///FireAbsorb.instance.StopHoldingFireball();
        }
        StartPos[(int)side] = AIMagicControl.instance.Hands[(int)side].localPosition;
    }
    public void RecieveNewState(Side side, bool IsStart)
    {
        //Debug.Log("NewState: " + State);
        //Debug.Log("side: " + side + "  StartOrFinish: " + IsStart);
        Actives[(int)side] = IsStart;
        if (IsStart)//startcount
        {
            StartPos[(int)side] = AIMagicControl.instance.Hands[(int)side].localPosition;
        }
        else     //get distance to 
        {
            EndCount(side);
        }
    }
    private void Start()
    {
        MagicReactor.FireballCast += RecieveNewState;
        //gameObject.GetComponent<LearningAgent>().NewState += frames.AddToList;
        //side = GetComponent<LearningAgent>().side;
    }

    public void SpawnFireball(Side side)
    {
        if (InGameManager.instance.CanDoMagic() == false)
            return;
        
        EyeController.instance.ChangeEyes(Eyes.Fire);
        ///direction of controller forward
        ///

        ///absorb code
        //CurrentSpell spell = (AIMagicControl.instance.HoldingFire()) ? CurrentSpell.Fireball : CurrentSpell.Fireball;
        //if (AIMagicControl.instance.HoldingFire())
        //AIMagicControl.instance.ResetHoldingFires();
        OnlineFireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(CurrentSpell.Fireball, true), AIMagicControl.instance.Spawn[(int)side].position, SpawnRotation(side));
        NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, CurrentSpell.Fireball);
    }


    public float ToDegrees(Vector2 value) { return Mathf.Atan2(value.y, value.x) * 180 / Mathf.PI; }
    public Vector2 ToVector(float value) { return new Vector2(Mathf.Cos(value * Mathf.PI / 180f), Mathf.Sin(value * Mathf.PI / 180f)); }
    
    private void Update()
    {
        if (!PastFrameRecorder.instance.AtMax())
            return;
        //SpawnRotation(Side.left);
        //Output = ToVector(InputAngle);
        //ReInputAngle = ToDegrees(Output);
        /*
        SingleInfo StartFrame = PastFrameRecorder.instance.PastFrame(Side.right, FramesAgoRotation);
        SingleInfo EndFrame = PastFrameRecorder.instance.GetControllerInfo(Side.right);

        Vector3 HandDirection = (StartFrame.HandPos - EndFrame.HandPos).normalized;
        HandDirectionValue = ToDegrees(new Vector2(HandDirection.x, HandDirection.z));
        

        HandDirection.y = 0;
        CamRot = AIMagicControl.instance.Cam.eulerAngles;
        CamDirectionValue = CamRot.y;
        float Both = HandDirectionValue - CamDirectionValue;
        Both = Both - 180;
        OutPutDir = ToVector(Both);

        //Vector3 HeadForwardDir = Quaternion.Euler(AIMagicControl.instance.Cam.eulerAngles) * Vector3.forward;

        //HeadForwardTrue = new Vector3(HeadForwardDir.x, 0, HeadForwardDir.z); //this

        //HandDirectionTrue = HandDirection; //this




        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vel);
        Vector3 Trial5 = Vel;
        Vector3 Trial6 = Vel.normalized;


        // Rotate the vector by the angle
        

        Debug.DrawLine(AIMagicControl.instance.Cam.position, AIMagicControl.instance.Cam.position + (HeadForwardTrue * 3f), Color.yellow);

        Debug.DrawLine(AIMagicControl.instance.Cam.position, AIMagicControl.instance.Cam.position + (HandDirectionTrue * 3f), Color.blue);

        Debug.DrawLine(AIMagicControl.instance.Cam.position, AIMagicControl.instance.Cam.position + (new Vector3(OutPutDir.x, 0, OutPutDir.y) * 3f), Color.black);

        //Debug.DrawLine(AIMagicControl.instance.Cam.position, AIMagicControl.instance.Cam.position + (Trial4 * 3f), Color.blue);

        //Debug.DrawLine(AIMagicControl.instance.Cam.position, AIMagicControl.instance.Cam.position + (Trial5 * 3f), Color.green);

        // Debug.DrawLine(AIMagicControl.instance.Cam.position, AIMagicControl.instance.Cam.position + (Trial6 * 3f), Color.red);

        */

    }
    /*
    public IEnumerator WaitForClose()
    {
        bool PastThreshold = StopControllingDistance < Vector3.Distance(AIMagicControl.instance.Spawn[(int)side].localPosition, Camera.main.transform.localPosition);
        IsControlling = true;
        while (PastThreshold == false)
        {
            yield return new WaitForSeconds(0.1f);
        }
        IsControlling = false;
    }
    */
}
