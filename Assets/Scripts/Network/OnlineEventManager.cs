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
    
    public delegate void FirePush(Vector3 Pos, Vector3 Direction);
    public static event FirePush FirePushEvent;
    public const byte PushFire = 1;
    public static void PushFireOnlineEvent(Vector3 Pos, Vector3 Direction)
    {
        object[] content = new object[] { Pos, Direction };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(PushFire, content, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion
    #region PlaySound
    public const byte SoundCode = 2;
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
            //Debug.Log("Push Fire");
            object[] data = (object[])photonEvent.CustomData;
            Vector3 pos = (Vector3)data[0];
            Vector3 dir = (Vector3)data[1];
            //float Force = 
            FirePushEvent?.Invoke(pos, dir);
            //FireController.TriggerFirePushEvent(pos);
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
