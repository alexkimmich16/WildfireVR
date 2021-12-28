using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballGenerator : MonoBehaviour
{
    public float MaxTime;
    public float Speed;
    public GameObject Fireball;
    public Transform Spawn;
    private float Timer;

    void Update()
    {
        Timer += Time.deltaTime;
        Spawn.LookAt(HandMagic.instance.Cam);
        if (Timer > MaxTime)
        {
            SpawnFireball();
            Timer = 0;
        }
    }

    public void SpawnFireball()
    {
        GameObject Current = Instantiate(Fireball, Spawn.position, Spawn.rotation);
        Current.GetComponent<Fireball>().Speed = Speed;
    }
}
