using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
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
            float Distance = Vector3.Distance(Dir, PushPos);
            m_Particles[i].velocity = Dir * FireController.instance.DeflectForce * (Mathf.Pow(Distance, FireController.instance.DeflectDistanceForce));
        }
        fire.SetParticles(m_Particles, numParticlesAlive);
    }
    

    
    //but collider is local
    //if i'm the one being hit
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag != "Shield" && other.tag != "Hitbox")
            return;


        Player FlameOwner = transform.parent.GetComponent<PhotonView>().Owner;
        if (other.tag == "Shield")
        {
            Debug.Log("Shield1");
            //shield=mine and flame=others
            if (other.transform.parent.GetComponent<PhotonView>().IsMine && FlameOwner != PhotonNetwork.LocalPlayer)
            {
                //flame and sheild need to be opposites    
                Debug.Log("Shield2");
                if (GetPlayerTeam(PhotonNetwork.LocalPlayer) != GetPlayerTeam(FlameOwner))
                {
                    Debug.Log("Shield3");
                    OnlineEventManager.PushFireOnlineEvent(AIMagicControl.instance.Cam.position);
                }
            }
            
                    
        }
        else if (other.tag == "Hitbox")
        {
            Debug.Log("Hitbox1");
            if (FlameOwner.IsLocal)//self damage
                return;
            Debug.Log("Hitbox2");
            if (NetworkManager.instance.FriendlyFireWorks(FlameOwner, PhotonNetwork.LocalPlayer))
            {
                Debug.Log("Hitbox3");
                NetworkManager.instance.LocalTakeDamage(FireController.instance.Damage);
            }
                
        }

    }

    private void OnEnable()
    {
        OnlineEventManager.FirePushEvent += PushFire;
    }
    private void OnDisable()
    {
        OnlineEventManager.FirePushEvent -= PushFire;
    }
}
