using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using ExitGames.Client.Photon;
public class OnlineEventManager : MonoBehaviour
{
    void Awake() { instance = this; }
    public static OnlineEventManager instance;

    #region DeflectFire
    
    public delegate void FirePush(Vector3 Pos);
    public static event FirePush FirePushEvent;
    public const byte PushFire = 1;
    public static void PushFireOnlineEvent(Vector3 Pos)
    {
        object[] content = new object[] { Pos };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(PushFire, content, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion
    #region Restart

    public delegate void Restart();
    public static event Restart RestartEventCallback;
    public const byte RestartCode = 2;
    public void RestartEvent()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(RestartCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion
    #region GameStates
    public const byte NewStateCode = 3;
    public static bool ChangingState;
    public static void NewState(GameState state)
    {
        //Debug.Log("TrySet: " + state.ToString());

        if (ChangingState == true || !PhotonNetwork.IsMasterClient)
            return;

        ChangingState = true;

        Debug.Log("sentGame: " + state.ToString());
        object[] content = new object[] { state };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(NewStateCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion
    #region DoorStates
    public const byte DoorCode = 4;
    public static void DoorEvent(SequenceState state)
    {
        //Debug.Log(result.ToString());
        //Debug.Log("sentDoor: " + state.ToString());
        object[] content = new object[] { (int)state };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(DoorCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion
    #region PlaySound
    public const byte SoundCode = 5;
    public static void SoundEvent(int SoundIndex)
    {
        //Debug.Log(result.ToString());
        object[] content = new object[] { SoundIndex };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(SoundCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion

    #region basic
    private void OnEnable() { PhotonNetwork.NetworkingClient.EventReceived += OnEvent; }
    private void OnDisable() { PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == PushFire)
        {
            Debug.Log("Push Fire");
            object[] data = (object[])photonEvent.CustomData;
            Vector3 pos = (Vector3)data[0];
            FirePushEvent?.Invoke(pos);
            //FireController.TriggerFirePushEvent(pos);
        }
        if(photonEvent.Code == RestartCode)
        {
            //Debug.Log("restart");
            //InGameManager.instance.RespawnToSpawnPoint();//respawn
            SetPlayerInt(PlayerHealth, PlayerControl.MaxHealth, PhotonNetwork.LocalPlayer);// reset health
            RestartEventCallback?.Invoke();
            SpawnManager.instance.RespawnToTeam();// moveback to team
            
            InGameManager.instance.SetNewGameState(GameState.Waiting);
        }
        if(photonEvent.Code == NewStateCode)
        {
            ChangingState = false;
            
            object[] data = (object[])photonEvent.CustomData;
            GameState NewState = (GameState)data[0];
            //Debug.Log("ReceiveState: " + NewState.ToString());
            InGameManager.instance.SetNewGameState(NewState);
            
            //BillBoardManager.instance.SetOutcome(result);
            
            //SetOutcome
        }
        if(photonEvent.Code == DoorCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int state = (int)data[0];
            SoundManager.instance.SetDoorAudio(state);
            DoorManager.instance.RecieveOnlineDoorState((SequenceState)state);
        }
        if (photonEvent.Code == SoundCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            /*
            if ((int)data[0] == 0)
                SoundManager.instance.OnPlayerHit();
            else if ((int)data[0] == 1)
                SoundManager.instance.OnPlayerDeath();
            else if ((int)data[0] == 1)
                SoundManager.instance.OnPlayerLeave();
            else if ((int)data[0] == 1)
                SoundManager.instance.OnPlayerHit();
            */
        }
    }
    #endregion
}
