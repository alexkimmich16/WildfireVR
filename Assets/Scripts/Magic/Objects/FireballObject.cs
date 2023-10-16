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

    private Rigidbody rb;

    public bool Absorbing;

    public VFXHolder VFX;

    
    public GameObject FireballSphere;

    private float timer;
    

    public Transform target;

    public bool hitFromLeft = true;
    private Vector3 adjustedTargetPosition;


    public void SetAbsorbed(bool State)
    {
        Absorbing = State;
        VFX.SetNewState(false);
    }

    //public float LifeTime = 3;


    float speed { get { return (initialSpeed + initialSpeed * Mathf.Pow(timer, AccelerateSpeed)); } }
    protected override void Update()
    {
        base.Update();
        if (!GetComponent<PhotonView>().IsMine || !FireballSphere.activeSelf)
            return;

        // Update timer
        timer += Time.deltaTime;

        // Update Rigidbody's velocity
        rb.velocity = Vector3.zero;
        //rb.velocity = transform.forward * (initialSpeed + initialSpeed * Mathf.Pow(timer, AccelerateSpeed));




        // Compute the direction to the target
        Vector3 toTarget = target.position - transform.position;

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

        // Move the fireball forward
        transform.position += transform.forward * speed * Time.deltaTime;


        if (timer > 2)
            Destroy(gameObject);
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
                NetworkManager.instance.LocalTakeDamage(FireballController.instance.Damage, FireballOwner);
        }

        if (col.collider.gameObject.tag != "Shield")
            KillThis();

        void KillThis()
        {
            GetComponent<PhotonDestroy>().StartCountdown();
            if(GetComponent<PhotonView>().IsMine)
                FireballController.instance.CheckFireballForMine(gameObject);
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
        rb = GetComponent<Rigidbody>();

        if (target)
        {
            // Compute the direction to the target
            Vector3 toTarget = (target.position - transform.position).normalized;

            // Blend between the fireball's current forward direction and the target direction
            Vector3 newDirection = Vector3.Slerp(transform.forward, toTarget, turnSpeed * Time.deltaTime);

            // Move the fireball
            transform.position += newDirection * speed * Time.deltaTime;
            transform.forward = newDirection;
        }
    }

    protected override void OnEnable()
    {
        //Debug.Log("start");
        base.OnEnable();
        timer =0;
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
