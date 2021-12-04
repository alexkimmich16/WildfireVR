using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballGenerator : MonoBehaviour
{
    public float MaxTime;
    public float Speed;
    public GameObject Fireball;
    private float Timer;

    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > MaxTime)
        {
            SpawnFireball();
            Timer = 0;
        }
    }

    public void SpawnFireball()
    {
        GameObject Current = Instantiate(Fireball, transform.position, Quaternion.identity);
        Current.GetComponent<Fireball>().Speed = Speed;
    }
}
