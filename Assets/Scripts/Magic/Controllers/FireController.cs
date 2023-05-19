using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RestrictionSystem;
using System.Linq;
public enum FireDetectType
{
    Colliders = 0,
    CodeDetect = 1,
}

public class FireController : SpellControlClass
{
    public static FireController instance;
    private void Awake() { instance = this; }
    public List<bool> Actives = new List<bool>();
    public static float CheckInterval = 0.1f;
    //public bool CanCast;
    public bool ShouldDebug;

    [Header("DamageStats")]
    public float EnemyCooldownTime;
    public int Damage;
    public float CastCooldowntime;
   

    [Header("CodeDetect")]
    public float TargetCheckDistance;
    public float CollisionDistance;

    [Header("VFX")]

    public List<GameObject> OnlineFire = new List<GameObject>();

    [Header("Lists")]
    public List<Transform> Targets = new List<Transform>();
    public List<CooldownInfo> DamageCooldowns = new List<CooldownInfo>();

    [Header("Deflect")]
    public float DeflectForce;
    public float DeflectDistanceForce;

    public float TimeDelay = 0.3f;
    private List<float> DelayTimer;
    public List<bool> IsCountingDelay;

    public bool IsCooldown(Transform hitAttempt) { return DamageCooldowns.Any(x => x.Target == hitAttempt); }

    #region StartStop
    public void StopFire(Side side)
    {
        if (OnlineFire[(int)side] != null)
        {
            OnlineFire[(int)side].GetPhotonView().RPC("SetFlamesOnline", RpcTarget.All, false);
            OnlineFire[(int)side].GetComponent<PhotonDestroy>().StartCountdown();
            OnlineFire[(int)side] = null;

            SetHaptics(side, false);
        }
    }
    public void StartFire(Side side, int Level)
    {
        //Debug.Log(InGameManager.instance.CanDoMagic());
        if (InGameManager.instance.CanDoMagic() == false)
            return;

        if (OnlineFire[(int)side] != null)
        {
            OnlineFire[(int)side].GetPhotonView().RPC("SetFlamesOnline", RpcTarget.All, false);
            OnlineFire[(int)side].GetComponent<PhotonDestroy>().StartCountdown();
        }

        SetHaptics(side, true);
        OnlineFire[(int)side] = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(Spell.Flames, Level), GetPos(side), GetRot(side));
        //OnlineFire[(int)side] = PhotonNetwork.Instantiate(AIMagicControl.instance.spells.SpellName(CurrentLearn.Flames, Level), Vector3.zero, Camera.main.transform.rotation);
        //OnlineFire[(int)side].name = "OnlineFire";
    }
    #endregion

    public override void InitializeSpells()
    {
        OnlineFire = new List<GameObject>() { null, null };
        DelayTimer = new List<float>() { 0f, 0f };
        IsCountingDelay = new List<bool>() { false, false };
        DamageCooldowns = new List<CooldownInfo>();
    }

    public override void RecieveNewState(Side side, bool StartOrFinish, int Index, int Level)
    {
        Actives[(int)side] = StartOrFinish;

        if (StartOrFinish)
        {
            if (IsCountingDelay[(int)side] == true)
            {
                IsCountingDelay[(int)side] = false;
                DelayTimer[(int)side] = 0f;
            }
            else
                StartFire(side, Level);
        }
        else
        {
            IsCountingDelay[(int)side] = true;
            DelayTimer[(int)side] = 0f;
        }
    }

    public void ManageEnemyCooldown()
    {
        if(DamageCooldowns != null)
            if (DamageCooldowns.Count > 0)
                for (int i = 0; i < DamageCooldowns.Count; i++)
                {
                    DamageCooldowns[i].Time += Time.deltaTime;
                    if (DamageCooldowns[i].Time > EnemyCooldownTime)
                        DamageCooldowns.Remove(DamageCooldowns[i]);
                }
    }
    public Quaternion GetRot(Side side) { return Quaternion.LookRotation(AIMagicControl.instance.PositionObjectives[(int)side].transform.position - AIMagicControl.instance.Cam.position); }
    public Vector3 GetPos(Side side) { return AIMagicControl.instance.PositionObjectives[(int)side].position; }
    private void Update()
    {
        for (int i = 0; i < OnlineFire.Count; i++)
        {
            if (IsCountingDelay[i])
                DelayTimer[i] += Time.deltaTime;
            if (DelayTimer[i] > TimeDelay)
            {
                StopFire((Side)i);
                DelayTimer[i] = 0f;
                IsCountingDelay[i] = false;
            }
        }
        ManageEnemyCooldown();

        for (int i = 0; i < OnlineFire.Count; i++)
        {
            if(OnlineFire[i] != null)
            {
                OnlineFire[i].transform.position = GetPos((Side)i);
                OnlineFire[i].transform.rotation = GetRot((Side)i);
            }
            
        }
            
    }
}
[System.Serializable]
public class CooldownInfo
{
    public Transform Target;
    public float Time = 1;
}