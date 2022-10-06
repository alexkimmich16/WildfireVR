using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public enum Spell
{
    Fireball = 0,
    BlueFireball = 1,
    Flames = 2,
}
public class FirePillar : MonoBehaviour
{
    public FlameObject FireVFX;
    //subscribe to on fire event
    public delegate void StateEvent(Spell spell);
    public static event StateEvent OnFire;
    public float StopTime;
    public static void CallStartFire(Spell spell) { OnFire(spell); }

    public void StartFlame(Spell spell)
    {
        FireVFX.SetFlamesOffline(true);
        StartCoroutine(WaitEndFire());
        
    }
    void Start()
    {
        OnFire += StartFlame;
        FireVFX.SetFlamesOffline(false);
    }
    
    public IEnumerator WaitEndFire()
    {
        yield return new WaitForSeconds(StopTime);
        FireVFX.SetFlamesOffline(false);
    }
}
