using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.
public class FlameCollision : MonoBehaviour
{
    public ParticleSystem fire;
    public bool Push;
    public Transform Example;
    public float Force;
    public void PushFire(Vector3 PushPos, float InputForce)
    {
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[fire.main.maxParticles];
        int numParticlesAlive = fire.GetParticles(m_Particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 Pos = m_Particles[i].position;
            Vector3 Dir = (Pos - PushPos).normalized;
            m_Particles[i].velocity = Dir * (Force);
        }
        fire.SetParticles(m_Particles, numParticlesAlive);
    }
    // Update is called once per frame
    void Update()
    {
        if(Push == true && Example != null)
        {
            Push = false;
            PushFire(Example.position, Force);
        }
    }
    
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Collisison");
        if(other.tag == "VRPerson")
        {
            Debug.Log("DAMAGE!");
            FireController.DamageShardHit(other);
        }
            
    }
}
