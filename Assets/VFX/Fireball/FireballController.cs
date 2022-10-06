using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public enum ControlType
{
    OnMoveHand = 0,
    Constant = 1,
    Directional = 2,
}
public class FireballController : MonoBehaviour
{
    private bool Active;
    public bool SpawnOnline;
    public bool IsControlling;
    [Header("Stats")]
    public float Speed;
    public float CastDistance;
    public float StopControllingDistance;
    Vector3 StartPos;
    public GameObject Fireball;
    public GameObject PrivateFireball;

    
    public float RotationThreshold;

    public float ControlForce;
    public ControlType controlType;
    private Side side;

    [Header("References")]

    [Header("Frames")]
    public Frames frames;

    public void StartCount()
    {
        //first
        StartPos = AIMagicControl.instance.Hands[(int)side].localPosition;
    }

    public void EndCount()
    {
        if(Vector3.Distance(StartPos, AIMagicControl.instance.Hands[(int)side].localPosition) > 0.1)
            Debug.Log(Vector3.Distance(StartPos, AIMagicControl.instance.Hands[(int)side].localPosition));
        Debug.Log(Vector3.Distance(StartPos, AIMagicControl.instance.Hands[(int)side].localPosition));
        if (Vector3.Distance(StartPos, AIMagicControl.instance.Hands[(int)side].localPosition) > CastDistance)
        {
            //ControlPos = AIMagicControl.instance.Hands[(int)side].localPosition;
            StartCoroutine(WaitForClose());
            SpawnFireball(FireAbsorb.instance.FireballControl);
            ///FireAbsorb.instance.StopHoldingFireball();
        }
        StartPos = AIMagicControl.instance.Hands[(int)side].localPosition;
    }
    public void OnNewState(bool State)
    {
        //Debug.Log("NewState: " + State);
        if (!frames.CanCast || Fireball != null)
            return;

        if (frames.FramesWork(true) && Active == false)
        {
            Active = true;
            StartCount();
        }
        else if (frames.FramesWork(false) && Active == true)
        {
            Active = false;
            EndCount();
        }
    }
    private void Start()
    {
        gameObject.GetComponent<LearningAgent>().NewState += OnNewState;
        gameObject.GetComponent<LearningAgent>().NewState += frames.AddToList;
        side = GetComponent<LearningAgent>().side;
    }

    public void SpawnFireball(bool Redirect)
    {
        EyeController.instance.ChangeEyes(Eyes.Fire);
        ///direction of controller forward
        if (SpawnOnline)
        {
            if (Redirect == false)
                Fireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(Spell.Fireball,true), AIMagicControl.instance.Spawn[(int)side].position, Camera.main.transform.rotation);
            else if (Redirect == true)
                Fireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(Spell.BlueFireball, true), AIMagicControl.instance.Spawn[(int)side].position, Camera.main.transform.rotation);
            NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, Spell.Fireball);
            Fireball.SetActive(false);
        }
        PrivateFireball = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(Spell.BlueFireball, true)), Vector3.zero, Camera.main.transform.rotation);
    }
    private void Update()
    {

        if (IsControlling == false)
            return;
        if (controlType == ControlType.OnMoveHand)
        {

        }
        else if (controlType == ControlType.Constant)
        {

        }
        else
        {

        }
            
            //get amount to change normalized
            //change each frame
        
    }
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

}
