using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RestrictionSystem;
public enum ControlType
{
    OnRotateHand = 0,
    ApplyConstant = 1,
}
public enum SpawnDirection
{
    HeadForward = 0,
    HandForward = 1,
    HandVelocity = 2,
}
[System.Serializable]
public class FireballSide
{
    public bool Active;
    public GameObject Fireball;
    public GameObject Warmup;
    public bool Controlling;
    public Vector3 StartRotation;
    public Vector3 DifferenceToStart;
    public Vector3 LastForwardFrame;
    public Quaternion Difference;
    //public Vector3 InitialFireballForward;
}
public class FireballController : SpellControlClass
{
    public static FireballController instance;
    private void Awake() { instance = this; }

    public bool IsControlling;
    [Header("Stats")]
    public int Damage;

    public float ControlSensitivity;

    public ControlType controlType;
    public SpawnDirection spawnDir;

    [Header("References")]
    public GameObject PositionExample;

    [Header("Debug")]
    public bool ShouldDebug;
    public float Display;

    public List<FireballSide> Sides;

    public float WarmupDistFromHand;


    
    public Quaternion SpawnRotation(Side side)
    {
        //Vector3 RealOutput = new Vector3(AIMagicControl.instance.Hands[(int)side].transform.forward.x, 0f, AIMagicControl.instance.Hands[(int)side].transform.forward.z);

        return Quaternion.LookRotation(AIMagicControl.instance.Hands[(int)side].transform.forward);
        //spawnDir == SpawnDirection.HeadForward || spawnDir == SpawnDirection.HandVelocity
    }
    public Vector3 SpawnPosition(Side side)
    {
        Vector3 Pos = new Vector3(AIMagicControl.instance.Spawn[(int)side].position.x, AIMagicControl.instance.Cam.position.y, AIMagicControl.instance.Spawn[(int)side].position.z);
        return Pos;
    }
    public override void RecieveNewState(Side side, bool State, int Index, int Level)
    {
        if (Index == 0)
        {
            Sides[(int)side].Active = State;
            Sides[(int)side].Warmup.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, State);
        }

        if (State == true && Index == 1)
        {
            Sides[(int)side].Warmup.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, false);
            SpawnFireball(side, Level);
        }

        if(Index == 2)
        {
            if (State == true)
                Sides[(int)side].StartRotation = GetControllerRot((int)side);
            Sides[(int)side].Controlling = State;
        }
    }

    public void SpawnFireball(Side side, int Level)
    {
        if (InGameManager.instance.CanDoMagic() == false)
            return;

        Sides[(int)side].Fireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(CurrentLearn.Fireball, Level), SpawnPosition(side), SpawnRotation(side));
        //NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, CurrentLearn.Fireball);
    }
    public override void InitializeSpells()
    {
        ConditionManager.instance.conditions.MotionConditions[(int)CurrentLearn.Fireball - 1].OnNewState += RecieveNewState;
        for (int i = 0; i < 2; i++)
        {
            Sides[i].Warmup = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.FireballWarmup.name, Vector3.zero, Quaternion.identity);
            Sides[i].Warmup.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, false);
        }
    }
    public Vector3 GetControllerRot(int Side) { return AIMagicControl.instance.Hands[Side].transform.rotation.eulerAngles; }
    private void Update()
    {
        //Debug.DrawRay(AIMagicControl.instance.Hands[0].transform.position, AIMagicControl.instance.Hands[0].transform.forward, Color.red);
        //Debug.DrawRay(AIMagicControl.instance.Spawn[0].transform.position, AIMagicControl.instance.Spawn[0].transform.forward, Color.blue);
        //Debug.DrawRay(AIMagicControl.instance.PositionObjectives[0].transform.position, AIMagicControl.instance.PositionObjectives[0].transform.forward, Color.green);

        for (int i = 0; i < Sides.Count; i++)
        {
            if(Sides[i].Warmup != null)
            {
                Vector3 ForwardRot = Quaternion.Euler(AIMagicControl.instance.Hands[i].eulerAngles) * Vector3.forward;
                Sides[i].Warmup.transform.position = AIMagicControl.instance.Hands[i].position + (WarmupDistFromHand * ForwardRot);
                Sides[i].Warmup.transform.rotation = AIMagicControl.instance.Hands[i].rotation; //forward
            }
            

            if (Sides[i].Controlling)
            {
                Quaternion rotationDifference = Quaternion.identity;

                if (controlType == ControlType.OnRotateHand)
                {
                    rotationDifference = Quaternion.FromToRotation(Sides[i].LastForwardFrame * 180f, GetControllerRot(i) * 180f);
                    Sides[i].Fireball.transform.rotation = AIMagicControl.instance.Hands[i].transform.rotation;
                    //NewDirection = Add(Quaternion.Euler(Direction), AddDir).eulerAngles;         
                }
                else if (controlType == ControlType.ApplyConstant)
                {
                    //rotationDifference = Quaternion.FromToRotation(Sides[i].StartRotation, GetControllerRot(i));

                }
                //Debug.Log(rotationDifference);
                Sides[i].Fireball.transform.rotation *= Quaternion.Euler(rotationDifference.eulerAngles * ControlSensitivity);
                //Sides[i].Fireball.transform.Rotate(DirectionAdd);
                Sides[i].LastForwardFrame = GetControllerRot(i);
                Sides[i].Difference = Quaternion.Euler(rotationDifference.eulerAngles * ControlSensitivity);
            }

            Quaternion RotDifference(Vector3 rotation1, Vector3 rotation2)
            {
                Quaternion q1 = Quaternion.Euler(rotation1);
                Quaternion q2 = Quaternion.Euler(rotation2);

                Quaternion Dif = Diff(q1, q2);
                return Dif;
            }

            Quaternion Diff(Quaternion to, Quaternion from)
            {
                return to * Quaternion.Inverse(from);
            }
            Quaternion Add(Quaternion start, Quaternion diff)
            {
                return diff * start;
            }
        }
    }


}

//    public float ToDegrees(Vector2 value) { return Mathf.Atan2(value.y, value.x) * 180f / Mathf.PI; }
//public Vector2 ToVector(float value) { return new Vector2(Mathf.Cos(value * Mathf.PI / 180f), Mathf.Sin(value * Mathf.PI / 180f)); }