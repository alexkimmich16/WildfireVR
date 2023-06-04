using System.Collections;
using UnityEngine;
using RestrictionSystem;
public class FirePillar : MonoBehaviour
{
    //subscribe to on fire event
    public delegate void StateEvent(MotionState spell);
    public static event StateEvent OnFire;

    //public delegate void CountdownEvent();
    //public static event CountdownEvent StartCountdown;

    public float StopTime;

    public VFXHolder VFX;
    public static bool AbleToActivate;
    //Debug.Log("FLAMEEEE");
    public static void CallStartFire(MotionState spell) { if (AbleToActivate) { OnFire(spell); }  }

    public void StartFlame(MotionState spell)
    {
        if (!AbleToActivate)
            return;
        //Debug.Log("recieved");
        if(spell == MotionState.Flames || spell == MotionState.Fireball)
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
