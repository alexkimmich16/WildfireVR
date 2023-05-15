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
    public Quaternion lastControllerRotation;
    public Quaternion targetRotation;
    public Quaternion initialFireballRotation;
    //public Vector3 InitialFireballForward;
}
public class FireballController : SpellControlClass
{
    public static FireballController instance;
    private void Awake() { instance = this; }

    public bool IsControlling;
    [Header("Stats")]
    public int Damage;
    [Header("FireballControl")]
    //public float rotationMultiplier = 1f;
    public float lerpSpeed = 2f;

    public ControlType controlType;
    public SpawnDirection spawnDir;

    [Header("References")]
    public GameObject PositionExample;

    [Header("Debug")]
    public bool ShouldDebug;
    public float Display;

    public List<FireballSide> Sides;

    public float WarmupDistFromHand;

    //public Transform Debugger1;
    //public Transform Debugger2;

    public Vector2 XYMultiplier;

    public void CheckFireballForMine(object fireball)
    {
        for (int i = 0; i < Sides.Count; i++)
        {
            if(Sides[i].Fireball != null)
            {
                if (ReferenceEquals(fireball, Sides[i].Fireball))
                {
                    if (ConditionManager.instance.ConditionStats[i, (int)MotionState.Fireball - 1].SequenceState >= 2)//controlling and fireball collides
                    {
                        MotionConditionInfo Condition = ConditionManager.instance.conditions.MotionConditions[(int)Motion - 1];
                        ConditionManager.instance.ConditionStats[i, (int)MotionState.Fireball - 1].Reset();
                        for (int j = 0; j < ConditionManager.instance.conditions.MotionConditions[(int)Motion - 1].Sequences.Count; j++)
                        {
                            ConditionManager.instance.conditions.MotionConditions[(int)Motion - 1].DoEvent((Side)i, false, j, Condition.CastLevel);
                        }
                    }
                }
            }
        }
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

        if(Index == 3)
        {
            if (State == true)
                Sides[(int)side].Fireball.transform.forward = AIMagicControl.instance.Hands[(int)side].transform.forward;
            Sides[(int)side].Controlling = State;
        }
    }

    public void SpawnFireball(Side side, int Level)
    {
        if (InGameManager.instance.CanDoMagic() == false)
            return;
        SetHaptics(side, true);
        Sides[(int)side].Fireball = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(MotionState.Fireball, Level), new Vector3(AIMagicControl.instance.Spawn[(int)side].position.x, AIMagicControl.instance.Cam.position.y, AIMagicControl.instance.Spawn[(int)side].position.z), Quaternion.LookRotation(AIMagicControl.instance.Hands[(int)side].transform.forward));

        for (int i = 0; i < Sides.Count; i++)
        {
            Sides[i].lastControllerRotation = AIMagicControl.instance.Hands[i].rotation;
            Sides[i].initialFireballRotation = Sides[(int)side].Fireball.transform.rotation;
            Sides[i].targetRotation = Sides[(int)side].Fireball.transform.rotation;
        }

        //NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetPhotonView().RPC("MotionDone", RpcTarget.All, CurrentLearn.Fireball);
    }
    public override void InitializeSpells()
    {
        for (int i = 0; i < 2; i++)
        {
            Sides[i].Warmup = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.FireballWarmup.name, Vector3.zero, Quaternion.identity);
            Sides[i].Warmup.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, false);
        }
    }
    private void Update()
    {
        //if(Sides[0].Fireball != null)
            //Debugger1.rotation = Sides[0].Fireball.transform.rotation;
        //Debugger2.rotation = Sides[0].targetRotation;
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
                Quaternion currentRotation = AIMagicControl.instance.Hands[i].rotation;
                Quaternion rotationDifference = Quaternion.Inverse(Sides[i].lastControllerRotation) * currentRotation;
                Vector2 SideInvert = i == 0 ? new Vector2(-1f, 1f) : new Vector2(1f, -1f);
                rotationDifference = Quaternion.Euler(rotationDifference.eulerAngles.y * XYMultiplier.y * SideInvert.x, rotationDifference.eulerAngles.x * XYMultiplier.x * SideInvert.y, 0f);

                Sides[i].targetRotation = Sides[i].Fireball.transform.rotation * rotationDifference;
                Sides[i].lastControllerRotation = currentRotation;

                Sides[i].Fireball.transform.rotation = Quaternion.Lerp(Sides[i].Fireball.transform.rotation, Sides[i].targetRotation, Time.deltaTime);
                    
                //Sides[i].Fireball.transform.position += Sides[i].Fireball.transform.forward * Time.deltaTime; // Move the fireball forward
            }

            
        }

        
    }
}

//    public float ToDegrees(Vector2 value) { return Mathf.Atan2(value.y, value.x) * 180f / Mathf.PI; }
//public Vector2 ToVector(float value) { return new Vector2(Mathf.Cos(value * Mathf.PI / 180f), Mathf.Sin(value * Mathf.PI / 180f)); }