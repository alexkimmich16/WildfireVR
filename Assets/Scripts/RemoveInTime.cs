using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveInTime : MonoBehaviour
{
    public float Timer;
    public float MaxTime;

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > MaxTime)
        {
            HandMagic.instance.SC.RemoveObjectFromNetwork(gameObject);
        }
    }
}
