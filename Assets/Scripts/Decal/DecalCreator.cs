using System.Collections.Generic;
using UnityEngine;

public class DecalCreator : MonoBehaviour
{
    public ParticleSystem fire;
    public int AccuracyGap = 3;
    private void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        for (int i = 0; i < fire.GetCollisionEvents(other, collisionEvents); i += 1)
        {
            if (collisionEvents[i].colliderComponent.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                DecalSpawner.instance.SpawnDecalWall(collisionEvents[i].intersection, collisionEvents[i].normal);
            } 
            else if(collisionEvents[i].colliderComponent.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                DecalSpawner.instance.SpawnDecalGround(collisionEvents[i].intersection);
            }
        }
    }
}
