using UnityEngine;
using Photon.Pun;
using static Odin.Net;
using RestrictionSystem;
using Sirenix.OdinInspector;
using System.Linq;
public class MimicTester : SerializedMonoBehaviour
{
    public static MimicTester instance;
    void Awake() { instance = this; }
    public float MaxTime;
    public float FireballSpeed;
    public Transform Spawn;
    private float Timer;
    public bool Active;

    [HideIf("OnlineFunction")]public bool LookAtPlayer;

    public Spell Motion;

    public GameObject Flame;

    public float AccelerateSpeed;

    public bool CanSpawn;

    [FoldoutGroup("SpawningFunctions")] public bool OnlineFunction;
    [FoldoutGroup("SpawningFunctions"), ShowIf("OnlineFunction")] public bool SpawnIfMaster;
    [FoldoutGroup("SpawningFunctions"), ShowIf("OnlineFunction")] public bool LookAtMaster;

    Vector3 OnlineLookAt()
    {
        if(OnlineFunction)
            if (LookAtMaster)
                return NetworkManager.instance.GetPlayers().First(x => x.GetComponent<PhotonView>().Owner == PhotonNetwork.MasterClient).transform.position;
            else
                return NetworkManager.instance.GetPlayers().First(x => x.GetComponent<PhotonView>().Owner != PhotonNetwork.MasterClient).transform.position;
        else
            return Camera.main.transform.position;
    }
    bool CanOnlineSpawn() { return SpawnIfMaster == PhotonNetwork.IsMasterClient; }
    void Update()
    {
        if (!Active || !Initialized())
            return;
        Timer += Time.deltaTime;
        CanSpawn = CanOnlineSpawn();
        if (OnlineFunction && !CanOnlineSpawn())
            return;

        if (OnlineFunction || LookAtPlayer)
        {
            Vector3 Pos = OnlineLookAt();
            Pos = new Vector3(Pos.x, Pos.y + 0.5f, Pos.z);
            Spawn.LookAt(Pos);
        }
            

        if (Timer > MaxTime)
        {
            if (Motion == Spell.Fireball)
                SpawnFireball();
            else if (Motion == Spell.Flames)
                ChangeFlames();
            Timer = 0;
        }
    }

    public void SpawnFireball()
    {
        //GameObject Current = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(Spell.Fireball, true)), Spawn.position, Spawn.rotation);
        GameObject Current = PhotonNetwork.Instantiate((AIMagicControl.instance.spells.SpellName(Spell.Fireball, 1)), Spawn.position, Spawn.rotation);
        Current.GetPhotonView().RPC("ChangeSpeed", RpcTarget.All, FireballSpeed);
    }

    public void ChangeFlames()
    {
        //GameObject Current = Instantiate(Resources.Load<GameObject>(AIMagicControl.instance.spells.SpellName(Spell.Fireball, true)), Spawn.position, Spawn.rotation);
        if (Flame == null)
        {
            Flame = PhotonNetwork.Instantiate((AIMagicControl.instance.spells.SpellName(Spell.Flames, 1)), Spawn.position, Spawn.rotation);
            Flame.GetPhotonView().RPC("SetFlamesOnline", RpcTarget.All, true);
        } 
        else if (Flame != null)
        {
            Flame.GetComponent<PhotonDestroy>().DestroyOnline();
            Flame = null;
            
        }
    }
}
