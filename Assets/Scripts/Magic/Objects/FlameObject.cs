using Photon.Pun;
using UnityEngine;
using UnityEngine.VFX;
public class FlameObject : SpellObjectClass
{
    public ParticleSystem fire;
    private Vector3 LastPos;
    protected override void Update()
    {
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        /*
        ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[fire.main.maxParticles];
        int numParticlesAlive = fire.GetParticles(m_Particles);
        Vector3 velocity = (transform.position - LastPos) / Time.deltaTime;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            if (m_Particles[i].startLifetime - m_Particles[i].remainingLifetime < 0.1f)
            {
                float DirectionAdd = CalculateSpeedTowardsDirection(velocity, transform.forward);
                m_Particles[i].velocity = m_Particles[i].velocity.normalized * FireController.instance.RigidbodyVelocityAdd * fire.main.startSpeedMultiplier * DirectionAdd;
            }
            
            
        }
        fire.SetParticles(m_Particles, numParticlesAlive);
        LastPos = transform.position;
        */
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
