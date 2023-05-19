using System.Collections;
using UnityEngine;
using RestrictionSystem;
public class FirePillar : MonoBehaviour
{
    //subscribe to on fire event
    public delegate void StateEvent(Spell spell);
    public static event StateEvent OnFire;

    //public delegate void CountdownEvent();
    //public static event CountdownEvent StartCountdown;

    public float StopTime;

    public VFXHolder VFX;
    public static bool AbleToActivate;
    //Debug.Log("FLAMEEEE");
    public static void CallStartFire(Spell spell) { if (AbleToActivate) { OnFire(spell); }  }

    public void StartFlame(Spell spell)
    {
        if (!AbleToActivate)
            return;
        //Debug.Log("recieved");
        if(spell == Spell.Flames || spell == Spell.Fireball)
        {
            VFX.SetNewState(true);
            StartCoroutine(WaitEndFire());
        }
    }
    void Start()
    {
        if(AbleToActivate)
            OnFire += StartFlame;
        VFX.SetNewState(false);
        //OnFire(Spell.Flames);
    }

    public IEnumerator WaitEndFire()
    {
        //Debug.Log("wait");
        yield return new WaitForSeconds(StopTime);
        VFX.SetNewState(false);
    }
}
