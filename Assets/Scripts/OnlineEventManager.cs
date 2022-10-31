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

    #region Fire
    public float FireForce;
    public static void TriggerFirePushEvent(Vector3 Pos) { FirePushEvent(Pos); }
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
    public const byte RestartCode = 2;
    public static void RestartEvent()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(RestartCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion
    #region basic
    private void OnEnable() { PhotonNetwork.NetworkingClient.EventReceived += ReceiveNewState; }
    private void OnDisable() { PhotonNetwork.NetworkingClient.EventReceived -= ReceiveNewState; }

    public void ReceiveNewState(EventData photonEvent)
    {
        if (photonEvent.Code == PushFire)
        {
            object[] data = (object[])photonEvent.CustomData;
            Vector3 pos = (Vector3)data[0];
            FirePushEvent(pos);
            //FireController.TriggerFirePushEvent(pos);
        }
        if(photonEvent.Code == RestartCode)
        {
            bool Relocate = false; //reset team?
            if (GetPlayerTeam(PhotonNetwork.LocalPlayer) == Team.Spectator)  //if i'm a spectator and theres room fill
            {
                //give me a team
            }
            SetPlayerInt(PlayerHealth, PlayerControl.MaxHealth, PhotonNetwork.LocalPlayer);// reset health
            InGameManager.instance.RespawnToSpawnPoint();//respawn

            if (PhotonNetwork.IsMasterClient)
            {
                //game
                SetGameState(GameState.Waiting);
                //doors
                DoorManager.instance.ResetDoors();

                //for all players in stand, assign random team, and teleport, and allow them to switch in cooldown
                if (Relocate)
                {
                    for (int i = 0; i < InGameManager.instance.Teams[0].Spawns.Count; i++)
                    {
                        string AttackText = "Attack" + i;
                        string DefenseText = "Defense" + i;
                        SetGameBool(AttackText, false);
                        SetGameBool(DefenseText, false);
                    }
                }
                
            }
        }
    }
    #endregion
}
