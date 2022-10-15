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
    //subscribe to on fire event
    public delegate void StateEvent(Spell spell);
    public static event StateEvent OnFire;

    //public delegate void CountdownEvent();
    //public static event CountdownEvent StartCountdown;

    public float StopTime;

    public VFXHolder VFX;
    public static void CallStartFire(Spell spell) { OnFire(spell); Debug.Log("FLAMEEEE"); }

    public void StartFlame(Spell spell)
    {
        Debug.Log("recieved");
        if(spell == Spell.Flames || spell == Spell.Fireball)
        {
            VFX.SetNewState(true);
            StartCoroutine(WaitEndFire());
        }
    }
    void Start()
    {
        OnFire += StartFlame;
        VFX.SetNewState(false);
        //OnFire(Spell.Flames);
    }

    public IEnumerator WaitEndFire()
    {
        Debug.Log("wait");
        yield return new WaitForSeconds(StopTime);
        VFX.SetNewState(false);
    }
}
