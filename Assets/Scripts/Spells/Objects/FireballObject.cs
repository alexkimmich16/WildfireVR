using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;

public class FireballObject : SpellObjectClass
{
    public float initialSpeed => FireballController.instance.initialSpeed;
    public float AccelerateSpeed => FireballController.instance.AccelerateSpeed;
    public float turnSpeed  => FireballController.instance.turnSpeed; // Defines the size of the arc.
    public float proximityThreshold => FireballController.instance.proximityThreshold; // Defines the size of the arc.
    public float HeightOffset => FireballController.instance.HeightOffset; // Defines the size of the arc.

    private Rigidbody rb;

    public bool Absorbing;

    public VFXHolder VFX;

    
    public GameObject FireballSphere;

    private float timer;
    

    public Transform target;
    public Vector3 TargetPosition { get { return new Vector3(target.position.x, target.position.y + HeightOffset, target.position.z); } }

    public bool hitFromLeft = true;

    //public Side AttackSide;

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetAbsorbed(bool State)
    {
        Absorbing = State;
        VFX.SetNewState(false);
    }

    float speed { get { return (initialSpeed + initialSpeed * Mathf.Pow(timer, AccelerateSpeed)); } }
    protected override void Update()
    {
        base.Update();
        if (!GetComponent<PhotonView>().IsMine || !FireballSphere.activeSelf)
            return;

        // Update timer
        timer += Time.deltaTime;

        // Update Rigidbody's velocity

        // Compute the direction to the target
        if(target != null)
        {
            Vector3 toTarget = TargetPosition - transform.position;
            if (toTarget.magnitude < proximityThreshold)
            {
                // If within proximity, go straight to target
                transform.forward = toTarget.normalized;
            }
            else
            {
                // Get the rotation that looks at the target
                Quaternion targetRotation = Quaternion.LookRotation(toTarget);

                // Gradually turn the fireball towards this rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
       
        rb.velocity = transform.forward * speed;
        // Move the fireball forward
        //transform.position += transform.forward * speed * Time.deltaTime;
    }
    void OnCollisionEnter(Collision col)
    {
        if (Absorbing == true)
            return;
        Player FireballOwner = GetComponent<PhotonView>().Owner;

        //check if did damage
        if (col.collider.tag == "HitBox")
        {
            //if (FireballOwner.IsLocal)//no self damage
                //return;
            if (NetworkManager.instance.FriendlyFireWorks(FireballOwner, PhotonNetwork.LocalPlayer))
                NetworkManager.instance.LocalTakeDamage(FireballController.instance.Damage, FireballOwner);
        }

        Kill();
    }

    public void Kill()
    {
        //gameObject.GetComponent<SphereCollider>().enabled = false;
        gameObject.GetComponent<PhotonView>().RPC("OnHit", RpcTarget.All);
        OnHit();
    }

    
    [PunRPC]public void OnHit()
    {
        VFX.SetNewState(false);
        FireballSphere.SetActive(false);
        Sound.setParameterByName("Exit", 1f);

        GetComponent<PhotonDestroy>().StartCountdown();

        gameObject.GetComponent<SphereCollider>().enabled = false;
    }
    //[PunRPC]public void SetSide(Side side) { AttackSide = side; }

    [PunRPC]public void Parried()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            target = null;
            transform.rotation = Quaternion.LookRotation(-transform.forward);


            //NEW TARGET: ALWAYS CASTER? RECKECK LIKE SPAWN??
            //TURN BLUE

            //IF BLUE,TURN PURPLE


            //SET NEW SPEED
            //Kill();
        }
        
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        timer =0;
        VFX.SetNewState(true);
        FireballSphere.SetActive(true);
        FireballController.instance.Fireballs.Add(this);
        gameObject.GetComponent<SphereCollider>().enabled = true;

     
    }
    protected override void OnDisable()
    {
        FireballController.instance.Fireballs.Remove(this);
    }

    public override void SetAudio(bool State)
    {
        Sound.setParameterByName("Exit", State ? 0f : 1f);
    }
}
