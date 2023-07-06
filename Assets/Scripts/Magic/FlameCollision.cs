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
        //Debug.Log("other: " + other.name);
        //Debug.Log("other: " + other.tag);
        if (other.tag != "Shield" && other.tag != "Player")
            return;

        
        Player FlameOwner = transform.parent.GetComponent<PhotonView>().Owner;
        if (other.tag == "Shield")
        {
            //Debug.Log("S1");
            //shield=mine and flame=others
            if (other.transform.parent.GetComponent<PhotonView>().IsMine && FlameOwner != PhotonNetwork.LocalPlayer)
            {
                //flame and sheild need to be opposites    
                //Debug.Log("S1");
                if (GetPlayerTeam(PhotonNetwork.LocalPlayer) != GetPlayerTeam(FlameOwner))
                {
                    //Debug.Log("S1");
                    OnlineEventManager.PushFireOnlineEvent(AIMagicControl.instance.Cam.position);
                }
            }
            
                    
        }
        else if (other.name == AIMagicControl.instance.Rig.name)//hitbox
        {
            //Debug.Log("Hitbox1");

            if (FlameOwner.IsLocal)//self damage
                return;
            //Debug.Log("Hitbox2");
            if (NetworkManager.instance.FriendlyFireWorks(FlameOwner, PhotonNetwork.LocalPlayer))
            {
                //Debug.Log("Hitbox3");
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
