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
public enum SpawnDirection
{
    HeadForward = 0,
    HandForward = 1,
    HandVelocity = 2,
}

public class FireballController : SpellClass
{
    public static FireballController instance;
    private void Awake() { instance = this; }

    public bool IsControlling;
    [Header("Stats")]
    public int Damage;
    public float MinDistanceToHead;
    public float StopControllingDistance;

    public float DirectionLeaniency;


    
    private GameObject OnlineFireball;

    public float ControlForce;
    public ControlType controlType;
    public SpawnDirection spawnDir;



    [Header("References")]
    public GameObject PositionExample;

    [Header("Debug")]
    public bool ShouldDebug;
    public float Display;

    public List<bool> Actives;
    public List<GameObject> FireballWarmups;

    public int FramesAgoRotation = 5;


    public float HandDirectionValue, CamDirectionValue;
    public Vector2 OutPutDir;

    public float WarmupDistFromHand;

    public Quaternion SpawnRotation(Side side)
    {
        if(spawnDir == SpawnDirection.HeadForward)
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
        else
        {
            Vector3 RealOutput = new Vector3(AIMagicControl.instance.Hands[(int)side].transform.forward.x, 0f, AIMagicControl.instance.Hands[(int)side].transform.forward.z);
            
            return Quaternion.LookRotation(RealOutput);
        }
        //spawnDir == SpawnDirection.HeadForward || spawnDir == SpawnDirection.HandVelocity
    }
    public Vector3 SpawnPosition(Side side)
    {
        Vector3 Pos = new Vector3(AIMagicControl.instance.Spawn[(int)side].position.x, AIMagicControl.instance.Cam.position.y, AIMagicControl.instance.Spawn[(int)side].position.z);
        return Pos;
    }
    public void RecieveNewState(Side side, bool State, int Index, int Level)
    {
        //Debug.Log("State: " + State + "  Index: " + Index);
        
        
        //if (State == false && Index == 0 && ShouldDebug)
            //Debug.Log("StopCharge");

        if (Index == 0)
        {
            Actives[(int)side] = State;
            FireballWarmups[(int)side].GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, State);
        }

        if (State == true && Index == 1)
        {

            FireballWarmups[(int)side].GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, false);
            SpawnFireball(side, Level);
        }
    }
    private void Start()
    {
        NetworkManager.OnInitialized += InitializeWarmups;
    }

    public void SpawnFireball(Side side, int Level)
    {
        if (InGameManager.instance.CanDoMagic() == false)
            return;
        //Debug.Log("Spawn");
        EyeController.instance.ChangeEyes(Eyes.Fire);
        ///direction of controller forward
        ///

        ///absorb code
        //CurrentSpell spell = (AIMagicControl.instance.HoldingFire()) ? CurrentSpell.Fireball : CurrentSpell.Fireball;
        //if (AIMagicControl.instance.HoldingFire())
        //AIMagicControl.instance.ResetHoldingFires();
        OnlineFireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(CurrentLearn.Fireball, Level), SpawnPosition(side), SpawnRotation(side));
        NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, CurrentLearn.Fireball);
    }


    public float ToDegrees(Vector2 value) { return Mathf.Atan2(value.y, value.x) * 180f / Mathf.PI; }
    public Vector2 ToVector(float value) { return new Vector2(Mathf.Cos(value * Mathf.PI / 180f), Mathf.Sin(value * Mathf.PI / 180f)); }
    public void InitializeWarmups()
    {
        ConditionManager.instance.conditions.MotionConditions[(int)CurrentLearn.Fireball - 1].OnNewState += RecieveNewState;
        for (int i = 0; i < 2; i++)
        {
            FireballWarmups.Add(PhotonNetwork.Instantiate(AIMagicControl.instance.spells.FireballWarmup.name, Vector3.zero, Quaternion.identity));
            FireballWarmups[i].GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, false);
        }
    }

    private void Update()
    {
        //Debug.DrawRay(AIMagicControl.instance.Hands[0].transform.position, AIMagicControl.instance.Hands[0].transform.forward, Color.red);
        //Debug.DrawRay(AIMagicControl.instance.Spawn[0].transform.position, AIMagicControl.instance.Spawn[0].transform.forward, Color.blue);
        //Debug.DrawRay(AIMagicControl.instance.PositionObjectives[0].transform.position, AIMagicControl.instance.PositionObjectives[0].transform.forward, Color.green);

        if (FireballWarmups.Count == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 ForwardRot = Quaternion.Euler(AIMagicControl.instance.Hands[i].eulerAngles) * Vector3.forward;
                FireballWarmups[i].transform.position = AIMagicControl.instance.Hands[i].position + (WarmupDistFromHand * ForwardRot);
                FireballWarmups[i].transform.rotation = AIMagicControl.instance.Hands[i].rotation; //forward
                //FireballWarmups[i].transform.rotation = Quaternion.Euler((PastFrameRecorder.instance.GetControllerInfo((Side)i).HandPos - PastFrameRecorder.instance.PastFrame((Side)i).HandPos).normalized); //velocity
            }
                
        }
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
