using UnityEngine;
using RestrictionSystem;
using System.Linq;
using System;
public class KeycodeTesting : MonoBehaviour
{
    public static KeycodeTesting instance;
    void Awake() { instance = this; }


    public bool useTesting;
    public int DamageAmount;

    public KeyCode TakeDamage;
    public KeyCode StartSequence;
    public KeyCode Restart;
    public KeyCode StartGame;
    public KeyCode CancelStartup;
    public KeyCode JoinAsPlayer;
    

    public KeyCode Fireball;
    public KeyCode Flames;

    public KeyCode GetData;

    public KeyCode OpenMenu;
    public KeyCode OpenOptions;

    public KeyCode[] Codes;

    //public ob

    void Update()
    {
        if (useTesting == false)
            return;
        
        if(Input.GetKeyDown(StartSequence))
            DoorManager.instance.StartSequence();

        if (Input.GetKeyDown(Fireball))
            FireballController.instance.SpawnFireball(Side.right, 1);

        if (Input.GetKeyDown(Flames))
            FireController.instance.StartFire(Side.right, 1);

        if (Input.GetKeyUp(Flames))
            FireController.instance.StopFire(Side.right);

        // if (Input.GetKeyDown(KeyCode.A))
        // AIMagicControl.instance.Flames[(int)Side.Right].StartFire();
        // if (Input.GetKeyDown(KeyCode.S))
        //AIMagicControl.instance.Flames[(int)Side.Right].StopFire();

        if (Input.GetKeyDown(TakeDamage))
            NetworkManager.instance.LocalTakeDamage(DamageAmount, null);

        if (Input.GetKeyDown(Restart))
            InGameManager.instance.RestartGame();

        if (Input.GetKeyDown(StartGame))
            InGameManager.instance.StartGame();

        if (Input.GetKeyDown(CancelStartup))
            InGameManager.instance.CancelStartup();

        if (Input.GetKeyDown(JoinAsPlayer))
            SpawnManager.instance.JoinAsPlayer();


        if (Input.GetKeyDown(GetData))
            Data.Secure.instance.GetData();

        if (Input.GetKeyDown(OpenMenu))
            Menu.MenuController.instance.OnMenuToggle();

        if (Input.GetKeyDown(OpenOptions))
        {
            Menu.MenuController.instance.ButtonTouched(Menu.MenuButtonType.Options);
        }
            

        if (Codes.Any(x => Input.GetKeyDown(x)))
        {

            int Index = Array.FindIndex(Codes, x => Input.GetKeyDown(x) == true);
            //Debug.Log(Index);
            SettingsControl.quality = (Quality)Index;
            //Debug.Log(SettingsControl.quality.ToString());
        }



        //if (Input.GetKeyDown(KeyCode.P))
        //NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetComponent<Ragdoll>().EnableRagdoll();

        //if (Input.GetKeyDown(KeyCode.P))
        //FireController.instance.RecieveNewState(Side.right, false);
        //if (Input.GetKeyDown(KeyCode.O))
        //FireController.instance.RecieveNewState(Side.right, true);

    }
}
