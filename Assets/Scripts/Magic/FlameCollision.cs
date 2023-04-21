using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class FlameCollision : MonoBehaviour
{
    public ParticleSystem fire;
    
    public void PushFire(Vector3 PushPos)
    {
        Debug.Log("pushfire");
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[fire.main.maxParticles];
        int numParticlesAlive = fire.GetParticles(m_Particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 Pos = m_Particles[i].position;
            Vector3 Dir = (Pos - PushPos).normalized;
            m_Particles[i].velocity = Dir * (OnlineEventManager.instance.FireForce);
        }
        fire.SetParticles(m_Particles, numParticlesAlive);
    }
    
    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("Tag: " + other.t);
        if (other.transform == AIMagicControl.instance.Rig)
        {
            if (BlockController.instance.IsBlocking())
                PushFire(AIMagicControl.instance.Cam.position);
            else if (!BlockController.instance.IsBlocking())
                NetworkManager.instance.LocalTakeDamage(FireController.instance.Damage);
        }
    }
    public void UnsubscribeToFire() { OnlineEventManager.FirePushEvent -= PushFire; }
    private void OnEnable()
    {
        OnlineEventManager.FirePushEvent += PushFire;
        if (transform.parent != null)
            if(transform.parent.GetComponent<PhotonDestroy>())
                transform.parent.GetComponent<PhotonDestroy>().DestoryEvent += UnsubscribeToFire;
        else if (GetComponent<PhotonDestroy>())
            GetComponent<PhotonDestroy>().DestoryEvent += UnsubscribeToFire;
    }
}
