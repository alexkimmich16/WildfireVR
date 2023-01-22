using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RestrictionSystem;
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

    void Update()
    {
        if (useTesting == false)
            return;
        
        if(Input.GetKeyDown(KeyCode.N))
            DoorManager.instance.StartSequence();

        if (Input.GetKey(KeyCode.D))
            FirePillar.CallStartFire(CurrentLearn.Flames);

       // if (Input.GetKeyDown(KeyCode.A))
           // AIMagicControl.instance.Flames[(int)Side.Right].StartFire();
       // if (Input.GetKeyDown(KeyCode.S))
            //AIMagicControl.instance.Flames[(int)Side.Right].StopFire();

        if (Input.GetKeyDown(KeyCode.G))
            NetworkManager.instance.LocalTakeDamage(DamageAmount);

        //if (Input.GetKeyDown(KeyCode.Z))
            //AIMagicControl.instance.Fireballs[(int)Side.Right].SpawnFireball();

        if (Input.GetKeyDown(KeyCode.R))
            InGameManager.instance.RestartGame();

        if (Input.GetKeyDown(KeyCode.T))
            InGameManager.instance.StartGame();

        if (Input.GetKeyDown(KeyCode.C))
            InGameManager.instance.CancelStartup();

        if (Input.GetKeyDown(KeyCode.J))
            SpawnManager.instance.JoinAsSpectator();

        //if (Input.GetKeyDown(KeyCode.P))
            //NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetComponent<Ragdoll>().EnableRagdoll();
        
        //if (Input.GetKeyDown(KeyCode.P))
            //FireController.instance.RecieveNewState(Side.right, false);
        //if (Input.GetKeyDown(KeyCode.O))
            //FireController.instance.RecieveNewState(Side.right, true);

    }
}
