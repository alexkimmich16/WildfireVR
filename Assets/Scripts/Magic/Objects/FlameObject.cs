using Photon.Pun;
using UnityEngine;
public class FlameObject : SpellObjectClass
{
    public ParticleSystem fire;
    private Vector3 LastPos;
    private float StartingSpeed;
    protected override void Update()
    {
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        var main = fire.main;


        float Magnitude = Vector3.Distance(AIMagicControl.instance.Rig.position, LastPos) / Time.deltaTime;
        Vector3 VelocityDirection = (AIMagicControl.instance.Rig.position - LastPos);

        Vector3 Direction = (transform.position - Camera.main.transform.position);

        float ReworkedMultiplier = 0f;


        float DotProduct = CalculateSpeedTowardsDirection(VelocityDirection, Direction);
        
        if (DotProduct > 0)
        {

            ReworkedMultiplier = (Mathf.Lerp(0f, 6f, Mathf.Clamp(DotProduct, 0f, 6f))) * FireController.instance.RigidbodyVelocityMultiplier * Magnitude;
        }


        Debug.Log(CalculateSpeedTowardsDirection(VelocityDirection, Direction));
        main.startSpeedMultiplier = StartingSpeed + ReworkedMultiplier;
        

        //for flame fast enough on movement
        /*
        ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[fire.main.maxParticles];
        int numParticlesAlive = fire.GetParticles(m_Particles);
        Vector3 velocity = (AIMagicControl.instance.Rig.position - LastPos) / Time.deltaTime;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (m_Particles[i].startLifetime - m_Particles[i].remainingLifetime < 0.1f)
            {
                float DirectionAdd = CalculateSpeedTowardsDirection(velocity, transform.forward);
                m_Particles[i].velocity = m_Particles[i].velocity.normalized * FireController.instance.RigidbodyVelocityAdd * fire.main.startSpeedMultiplier * DirectionAdd;
            }
        }
        fire.SetParticles(m_Particles, numParticlesAlive);
        */
        LastPos = AIMagicControl.instance.Rig.position;
        

    }
    private void Start()
    {
        StartingSpeed = fire.main.startSpeedMultiplier;
    }
    float CalculateSpeedTowardsDirection(Vector3 velocity, Vector3 direction)
    {
        // Calculate the dot product of the velocity and direction vectors
        float dotProduct = Vector3.Dot(velocity.normalized, direction.normalized);

        // Calculate the speed towards the given direction
        float speedTowardsDirection = dotProduct * velocity.magnitude;

        return speedTowardsDirection;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, true);
        
    }

    public override void SetAudio(bool State)
    {
        Sound.setParameterByName("Exit", State ? 0f : 1f);
    }

    [PunRPC]
    void SetFlamesOnline(bool NewState)
    {
        gameObject.GetComponent<PhotonView>().RPC("SetOnlineVFX", RpcTarget.All, NewState);
    }

}
