using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class FirePillar : MonoBehaviour
{
    //subscribe to on fire event
    public delegate void StateEvent(RestrictionSystem.CurrentLearn spell);
    public static event StateEvent OnFire;

    //public delegate void CountdownEvent();
    //public static event CountdownEvent StartCountdown;

    public float StopTime;

    public VFXHolder VFX;
    //Debug.Log("FLAMEEEE");
    public static void CallStartFire(RestrictionSystem.CurrentLearn spell) { OnFire(spell);  }

    public void StartFlame(RestrictionSystem.CurrentLearn spell)
    {
        //Debug.Log("recieved");
        if(spell == RestrictionSystem.CurrentLearn.Flames || spell == RestrictionSystem.CurrentLearn.Fireball)
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
        //Debug.Log("wait");
        yield return new WaitForSeconds(StopTime);
        VFX.SetNewState(false);
    }
}
