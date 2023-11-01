using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using static Odin.Net;
using Sirenix.OdinInspector;
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

    public bool UseWarmups;
    [ShowIf("UseWarmups")] public float WarmupDistFromHand;
    public List<FireballSide> Sides;

    public float initialSpeed;
    public float AccelerateSpeed;
    public float proximityThreshold = 2.0f;  // Distance at which fireball stops arcing and goes directly.

    public float HeightOffset;


    public List<FireballObject> Fireballs;

    public Transform PlaceholderTarget;


    //public float HitPlayerAngle;
    //public float rotationSpeed;

    //public float desiredWorldAngle = 0f; // Desired world angle to hit the target from (0 to 360)
    //public float arcModifier = 1f; // Defines the size of the arc.

    public float turnSpeed = 0.1f;  // How much it tries to get closer to the target directly.

    public static GameObject CreateFireball(Vector3 Pos, Quaternion Rot, Transform Target = null, float Speed = 0.5f)
    {
        GameObject Current = PhotonNetwork.Instantiate((AIMagicControl.instance.spells.SpellName(Spell.Fireball, 1)), Pos, Rot);
        //Current.GetPhotonView().RPC("SetSide", RpcTarget.All, side);

        Current.GetComponent<FireballObject>().target = Target;

        return Current;
    }


    public override void RecieveNewState(Side side, int state)
    {
        if (InGameManager.instance.CanDoMagic == false)
            return;

        //bool State = state == 1;
        if(state == 1)
        {
            SpawnFireball(side, 1);
        }
    }


    public Transform GetFireballTargetPlayer(Vector3 Position, Vector3 Direction, Team team) { return NetworkManager.instance.GetPlayers(team).OrderBy(enemy => Vector3.Angle(Direction, enemy.transform.position - Position)).FirstOrDefault().transform; }
    public void SpawnFireball(Side side, int Level)
    {
        SetHaptics(side, true);
        Vector3 Pos = new Vector3(AIMagicControl.instance.Spawn[(int)side].position.x, AIMagicControl.instance.Cam.position.y, AIMagicControl.instance.Spawn[(int)side].position.z);
        Quaternion Rot = Quaternion.LookRotation(AIMagicControl.instance.Hands[(int)side].transform.forward);

        //check enough players to find target
        Team OtherTeam = GetPlayerTeam(PhotonNetwork.LocalPlayer) == Team.Attack ? Team.Defense : Team.Attack;
        Transform Target = GetTarget();

        
        
        Sides[(int)side].Fireball = CreateFireball(Pos, Rot, Target);

        for (int i = 0; i < Sides.Count; i++)
        {
            Sides[i].lastControllerRotation = AIMagicControl.instance.Hands[i].rotation;
            Sides[i].initialFireballRotation = Sides[(int)side].Fireball.transform.rotation;
            Sides[i].targetRotation = Sides[(int)side].Fireball.transform.rotation;
        }

        Transform GetTarget()
        {
            if (PlaceholderTarget != null)
                return PlaceholderTarget;
            if (NetworkManager.instance.GetPlayers(OtherTeam).Count > 0 && GetPlayerTeam(PhotonNetwork.LocalPlayer) != Team.Spectator)
                return GetFireballTargetPlayer(Pos, AIMagicControl.instance.Hands[(int)side].transform.forward, OtherTeam).GetComponent<NetworkPlayer>().Head;
            return null;
        }
    }
    public override void InitializeSpells()
    {
        if (!UseWarmups)
            return;
        
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
            if(Sides[i].Warmup != null && UseWarmups)
            {
                Vector3 ForwardRot = Quaternion.Euler(AIMagicControl.instance.Hands[i].eulerAngles) * Vector3.forward;
                Sides[i].Warmup.transform.position = AIMagicControl.instance.Hands[i].position + (WarmupDistFromHand * ForwardRot);
                Sides[i].Warmup.transform.rotation = AIMagicControl.instance.Hands[i].rotation; //forward
            }
            
            /*
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
            */
            
        }

        
    }
}

//    public float ToDegrees(Vector2 value) { return Mathf.Atan2(value.y, value.x) * 180f / Mathf.PI; }
//public Vector2 ToVector(float value) { return new Vector2(Mathf.Cos(value * Mathf.PI / 180f), Mathf.Sin(value * Mathf.PI / 180f)); }