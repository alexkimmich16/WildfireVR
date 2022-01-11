using System.Collections;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerControl : MonoBehaviourPunCallbacks, IPunObservable
{
    public int Health;
    public int MaxHealth;
    public static float DeathTime = 1f;
    public delegate void DisolveAll();
    public event DisolveAll disolveEvent;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(Health);
        else
            Health = (int)stream.ReceiveNext();
    }
    public void ChangeHealth(int Change)
    {
        int oldHealth = 0;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("HEALTH", out object temp))
        {
            oldHealth = (int)temp;
        }
        int newHealth = oldHealth - Change;
        Hashtable HealthHash = new Hashtable();
        HealthHash.Add("HEALTH", newHealth);
        PhotonNetwork.LocalPlayer.SetCustomProperties(HealthHash);
        if (newHealth < 1)
            StartCoroutine(Respawn());
    }
    IEnumerator Respawn()
    {
        disolveEvent();
        yield return new WaitForSeconds(DeathTime);
        Health = MaxHealth;
        if(HandMagic.Respawn == true && SceneLoader.instance.CurrentSetting == CurrentGame.Testing)
            HandMagic.instance.RB.transform.position = HandDebug.instance.Spawn.position;
        else if (HandMagic.Respawn == true && SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
        {
            Transform Spawn = InGameManager.instance.SpectatorSpawns[Random.Range(0, InGameManager.instance.SpectatorSpawns.Count)];
            HandMagic.instance.RB.transform.position = Spawn.position;
        }
    }
}