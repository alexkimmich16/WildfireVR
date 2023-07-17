using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
public class FlameCollision : MonoBehaviour
{
    public ParticleSystem fire;
    public void PushFire(Vector3 PushPos, Vector3 PushDirection)
    {
        //Debug.Log("pushfire");
        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[fire.main.maxParticles];
        int numParticlesAlive = fire.GetParticles(m_Particles);



        for (int i = 0; i < numParticlesAlive; i++)
        {
            if(PushDirection == Vector3.zero)
            {
                Vector3 Pos = m_Particles[i].position;
                Vector3 Dir = (Pos - PushPos).normalized;
                float Distance = Vector3.Distance(Dir, PushPos);
                m_Particles[i].velocity = Dir * FireController.instance.DeflectForce * (Mathf.Pow(Distance, FireController.instance.DeflectDistanceForce));
            }
            else
            {
                Vector3 Pos = m_Particles[i].position;
                float Distance = Vector3.Distance(PushPos, Pos);
                if (Distance < FireManipulation.instance.MaxManipulateRange)
                {
                    m_Particles[i].velocity = m_Particles[i].velocity + (PushDirection * FireManipulation.instance.PushForce);
                }
            }
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
                    OnlineEventManager.PushFireOnlineEvent(AIMagicControl.instance.Cam.position, Vector3.zero);
                }
            }
            
                    
        }
        else if (other.name == AIMagicControl.instance.Rig.name)//hitbox
        {
            //Debug.Log("Hitbox1");
            //flameowner = other, damaged = self,
            if (FlameOwner.IsLocal)//self damage
                return;
            if (!NetworkManager.instance.FriendlyFireWorks(FlameOwner, PhotonNetwork.LocalPlayer))
                return;
            if (FireController.instance.IsCooldown(AIMagicControl.instance.Rig) && FireController.instance.UseCooldowns)
                return;

            FireController.instance.DamageCooldowns.Add(new FireController.CooldownInfo(AIMagicControl.instance.Rig));
            NetworkManager.instance.LocalTakeDamage(FireController.instance.Damage, FlameOwner);

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
