using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycodeTesting : MonoBehaviour
{
    public static KeycodeTesting instance;
    void Awake() { instance = this; }


    public bool useTesting;
    public int DamageAmount;
    public bool KeypadTesting()
    {
        return useTesting;
    }
    public TODO keycodes;

    void Update()
    {
        if (useTesting == false)
            return;
        
        if(Input.GetKeyDown(KeyCode.N))
            DoorManager.instance.StartSequence();

        if (Input.GetKey(KeyCode.D))
            FirePillar.CallStartFire(Spell.Flames);

        if (Input.GetKeyDown(KeyCode.A))
            AIMagicControl.instance.Flames[(int)Side.Right].StartFire();
        if (Input.GetKeyDown(KeyCode.S))
            AIMagicControl.instance.Flames[(int)Side.Right].StopFire();

        if (Input.GetKeyDown(KeyCode.G))
            NetworkManager.instance.LocalTakeDamage(DamageAmount);

        if (Input.GetKeyDown(KeyCode.Z))
            AIMagicControl.instance.Fireballs[(int)Side.Right].SpawnFireball(false);

        if (Input.GetKeyDown(KeyCode.R))
            InGameManager.instance.RestartGame();

        if (Input.GetKeyDown(KeyCode.T))
            InGameManager.instance.StartGame();

        if (Input.GetKeyDown(KeyCode.C))
            InGameManager.instance.CancelStartup();

        if (Input.GetKeyDown(KeyCode.J))
            InGameManager.instance.JoinAsSpectator();

        if (Input.GetKeyDown(KeyCode.H))
            SoundManager.instance.OnPlayerHit();
        /*
        if (Input.GetKeyDown(KeyCode.P))
            SetFlames(true);
        if (Input.GetKeyDown(KeyCode.O))
            SetFlames(false);
        */
    }
}
