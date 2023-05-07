using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
public class FireballObject : SpellObjectClass
{
    public float Speed;

    private Rigidbody RB;

    public bool Absorbing;

    public VFXHolder VFX;

    
    public GameObject FireballSphere;

    private float Timer;
    public float AccelerateSpeed;
    public float StartTime;

    public void SetAbsorbed(bool State)
    {
        Absorbing = State;
        VFX.SetNewState(false);
    }

    //public float LifeTime = 3;
    protected override void Update()
    {
        base.Update();
        if (!GetComponent<PhotonView>().IsMine)
            return;
        if (Absorbing == true)
        {
            //direction towards hand(graudal or isntant)
        }
        else
        {
            if (FireballSphere.activeSelf)
            {
                //transform.position += transform.forward * Time.deltaTime * Speed * Mathf.Pow(TimeActive, AccelerateSpeed);
                RB.velocity = transform.forward * Time.deltaTime * Speed * Mathf.Pow(Timer, MimicTester.instance.AccelerateSpeed);
                Timer += Time.deltaTime;
            }

        }

    }
    void OnCollisionEnter(Collision col)
    {
        if (Absorbing == true)
            return;
        //Debug.Log("" + col.collider.name);
        //if fireball hits ME or My shield

        //fireball can't be mine
        //shield has to be mine
        //Debug.Log("col: " + col.collider.tag);
        Player FireballOwner = GetComponent<PhotonView>().Owner;
        if (col.collider.tag == "Shield")
        {
            //Debug.Log("FireballOwner.IsLocal: " + FireballOwner.IsLocal);
            //Debug.Log("S1");
            if (FireballOwner.IsLocal && !col.transform.parent.GetComponent<PhotonView>().IsMine)
            {
                //Debug.Log("S2");
                //fireball isn't mine && shield is mine
                if (GetPlayerTeam(PhotonNetwork.LocalPlayer) != GetPlayerTeam(col.transform.parent.GetComponent<PhotonView>().Owner))
                {
                    //Debug.Log("S3");
                    Bounce(col.transform.parent.forward);
                }

            }

        }
        else if (col.collider.tag == "HitBox")
        {
            //Debug.Log("HB1");
            if (FireballOwner.IsLocal)//no self damage
                return;
            //Debug.Log("HB2");
            if (NetworkManager.instance.FriendlyFireWorks(FireballOwner, PhotonNetwork.LocalPlayer))
                NetworkManager.instance.LocalTakeDamage(FireballController.instance.Damage);
        }

        if (col.collider.gameObject.tag != "Shield")
            KillThis();

        void KillThis()
        {
            GetComponent<PhotonDestroy>().StartCountdown();
            gameObject.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);
        }
    }

    [PunRPC]
    public void OnHit()
    {
        VFX.SetNewState(false);
        FireballSphere.SetActive(false);
        Sound.setParameterByName("Exit", 1f);

        gameObject.GetComponent<SphereCollider>().enabled = false;
    }
    public void Bounce(Vector3 New)
    {
        transform.forward = New;
    }

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    protected override void OnEnable()
    {
        //Debug.Log("start");
        base.OnEnable();
        Timer = StartTime;
        VFX.SetNewState(true);//potentail problem
        //FireballSound = SoundManager.instance.CreateSound("fireball", transform);

        
        FireballSphere.SetActive(true);
        gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    public override void SetAudio(bool State)
    {
        Sound.setParameterByName("Exit", State ? 0f : 1f);
    }
}
