using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RestrictionSystem;
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
    public List<GameObject> FireballWarmups;
    //public bool justHand, Both;

    public int FramesAgoRotation = 5;


    public float HandDirectionValue, CamDirectionValue;
    public Vector2 OutPutDir;

    public float WarmupDistFromHand;
    public Quaternion SpawnRotation(Side side)
    {
        SingleInfo StartFrame = PastFrameRecorder.instance.PastFrame(side);
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

    public void RecieveNewState(Side side, bool IsStart, int Index)
    {
        if (Index == 0)
        {
            Actives[(int)side] = IsStart;
            FireballWarmups[(int)side].GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, IsStart);
        }
        else if(Index == 1)
        {
            SpawnFireball(side);
        }
        
        
    }
    private void Start()
    {
        ConditionManager.instance.MotionConditions[(int)CurrentLearn.Fireball - 1].OnNewState += RecieveNewState;
        NetworkManager.OnInitialized += InitializeWarmups;
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
        OnlineFireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(CurrentLearn.Fireball, true), AIMagicControl.instance.Spawn[(int)side].position, SpawnRotation(side));
        NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, CurrentLearn.Fireball);
    }


    public float ToDegrees(Vector2 value) { return Mathf.Atan2(value.y, value.x) * 180 / Mathf.PI; }
    public Vector2 ToVector(float value) { return new Vector2(Mathf.Cos(value * Mathf.PI / 180f), Mathf.Sin(value * Mathf.PI / 180f)); }

    public void InitializeWarmups()
    {
        for (int i = 0; i < 2; i++)
        {
            FireballWarmups.Add(PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellNameVariant(CurrentLearn.Fireball, true, 1), Vector3.zero, Quaternion.identity));
            FireballWarmups[i].GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, false);
        }
    }

    private void Update()
    {
        if(FireballWarmups.Count == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 ForwardRot = Quaternion.Euler(AIMagicControl.instance.Hands[i].eulerAngles) * Vector3.forward;
                FireballWarmups[i].transform.position = AIMagicControl.instance.Hands[i].position + (WarmupDistFromHand * ForwardRot);
                FireballWarmups[i].transform.rotation = AIMagicControl.instance.Hands[i].rotation; //forward
                //FireballWarmups[i].transform.rotation = Quaternion.Euler((PastFrameRecorder.instance.GetControllerInfo((Side)i).HandPos - PastFrameRecorder.instance.PastFrame((Side)i).HandPos).normalized); //velocity
            }
                
        }
        
        
        
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
