using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalCreator : MonoBehaviour
{
    public ParticleSystem fire;
    public float Multiplier;
    private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        //Debug.Log("other.layer: " + other.layer);
        if (other.layer == LayerMask.NameToLayer("Wall"))
        {
            for (int i = 0; i < fire.GetCollisionEvents(other, collisionEvents); i++)
            {
                //DecalSpawner.instance.SpawnDecalAtPosition(collisionEvents[i].intersection, Quaternion.Euler(new Vector3(90, 0, 0)));
                Vector3 YAxisView = collisionEvents[i].normal;
                Vector3 ZAxisView = new Vector3(1, 0, 0);
                


                DecalSpawner.instance.SpawnDecalAtPosition(collisionEvents[i].intersection, Quaternion.LookRotation(ZAxisView, YAxisView));
                //collisionEvents[i].
            }
        }
        else if (other.layer == LayerMask.NameToLayer("Ground"))
        {
            for (int i = 0; i < fire.GetCollisionEvents(other, collisionEvents); i++)
            {
                
                Vector3 YAxisView = (collisionEvents[i].normal * Multiplier).normalized;
                Vector3 ZAxisView = new Vector3(1, 0, 0);

                Vector3 Position = new Vector3(collisionEvents[i].intersection.x, 0, collisionEvents[i].intersection.z);
                DecalSpawner.instance.SpawnDecalAtPosition(Position, Quaternion.Euler(new Vector3(90, 0, 0)));
                //DecalSpawner.instance.SpawnDecalAtPosition(collisionEvents[i].intersection, Quaternion.LookRotation(ZAxisView, YAxisView));
                //collisionEvents[i].
            }
        }
    }
}
