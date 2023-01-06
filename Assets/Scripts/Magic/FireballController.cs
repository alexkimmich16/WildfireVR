using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RestrictionSystem;
public enum ControlType
{
    OnMoveHand = 0,
    Constant = 1,
    Directional = 2,
}
public class FireballController : MonoBehaviour
{
    public static FireballController instance;
    private void Awake() { instance = this; }

    public bool SpawnOnline;
    public bool IsControlling;
    [Header("Stats")]
    public float CastDistance;
    public float MinDistanceToHead;
    public float StopControllingDistance;

    public float DirectionLeaniency;
    
    private GameObject OnlineFireball, PrivateFireball;

    public float ControlForce;
    public ControlType controlType;



    [Header("References")]
    public GameObject PositionExample;

    [Header("Debug")]
    public bool ShouldDebug;
    public float Display;

    public int Min, Max;
    public GameObject TestOBJ;

    public List<bool> Actives;
    public List<Vector3> StartPos;
    //public bool justHand, Both;
    public Quaternion SpawnRotation()
    {
        //Vector3 HandDirection = AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().StartEndDirection(Min, Max).normalized;
        //Vector3 HandDirection = AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().StartEndDirection(Min, Max).normalized;
        //Vector3 AdjustedDir = new Vector3(HandDirection.x, 0, HandDirection.z);
        //return Quaternion.LookRotation(AdjustedDir);
        return Quaternion.identity;
        ///do one from machinelearning
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
        CurrentSpell spell = (AIMagicControl.instance.HoldingFire()) ? CurrentSpell.Fireball : CurrentSpell.Fireball;
        if (AIMagicControl.instance.HoldingFire())
            AIMagicControl.instance.ResetHoldingFires();

        if (SpawnOnline)
        {
            OnlineFireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(spell, true), AIMagicControl.instance.Spawn[(int)side].position, SpawnRotation());
            NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, CurrentSpell.Fireball);
        }
        //PrivateFireball = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(spell, false)), AIMagicControl.instance.Spawn[(int)side].position, AIMagicControl.instance.Hands[(int)side].GetComponent<HandActions>().Velocity);
        //PrivateFireball = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(spell, false)), AIMagicControl.instance.Spawn[(int)side].position, SpawnRotation());
    }
    private void Update()
    {
        //if (PositionExample != null)
            //PositionExample.transform.localPosition = StartPos;

        if (TestOBJ != null)
        {
            TestOBJ.transform.rotation = SpawnRotation();
            //TestOBJ.transform.position = AIMagicControl.instance.Spawn[(int)side].position;
        }
        if (IsControlling == false)
            return;
            //get amount to change normalized
            //change each frame
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
