using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShieldManager : MonoBehaviour
{
    public VisualEffect Boarder;
    public VisualEffect Interior;

    public float StartWaitTime;

    public bool ShieldActive;

    private void Start()
    {
        Boarder.Stop();
        Interior.Stop();
        //StartCoroutine(Wait(4f));
        StartShield();
    }

    public IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        StartShield();
    }

    public void StartShield()
    {
        StartCoroutine(StartMagicTime());
    }
    public void StopShield()
    {
        StartCoroutine(StartMagicTime());
    }

    public IEnumerator StartMagicTime()
    {
        Boarder.Play();
        yield return new WaitForSeconds(StartWaitTime);
        ShieldActive = true;
        Interior.Play();
    }
    public IEnumerator StopMagicTime()
    {
        Boarder.Stop();
        yield return new WaitForSeconds(StartWaitTime);
        ShieldActive = false;
        Interior.Stop();
    }
}
