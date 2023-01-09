using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class FirePillar : MonoBehaviour
{
    //subscribe to on fire event
    public delegate void StateEvent(CurrentSpell spell);
    public static event StateEvent OnFire;

    //public delegate void CountdownEvent();
    //public static event CountdownEvent StartCountdown;

    public float StopTime;

    public VFXHolder VFX;
    //Debug.Log("FLAMEEEE");
    public static void CallStartFire(CurrentSpell spell) { OnFire(spell);  }

    public void StartFlame(CurrentSpell spell)
    {
        //Debug.Log("recieved");
        if(spell == CurrentSpell.Flames || spell == CurrentSpell.Fireball)
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
