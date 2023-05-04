using UnityEngine;
using Photon.Pun;
public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    public static NetworkPlayerSpawner instance;
    void Awake() { instance = this; }
    public GameObject SpawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, Quaternion.Euler(0,0,0));
        SpawnedPlayerPrefab.name = "My Player";
        //SpawnedPlayerPrefab.transform.SetParent(NetworkManager.instance.playerList);
        //NetworkManager.instance.RefreshPlayerList();
        //get player side

        if (SceneLoader.instance.CurrentSetting == CurrentGame.Battle)
        {
            NetworkManager.instance.PlayerPhotonViews.Add(SpawnedPlayerPrefab.GetComponent<PhotonView>());
        }
        //NetworkManager.instance.Players.Add(SpawnedPlayerPrefab);
        

    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        NetworkManager.instance.PlayerPhotonViews.Remove(SpawnedPlayerPrefab.GetComponent<PhotonView>());
        DestroyIfActive(SpawnedPlayerPrefab);

        DestroyIfActive(FireController.instance.OnlineFire[0]);
        DestroyIfActive(FireController.instance.OnlineFire[1]);

        DestroyIfActive(FireballController.instance.Sides[0].Warmup);
        DestroyIfActive(FireballController.instance.Sides[1].Warmup);

        DestroyIfActive(BlockController.instance.BlockVFXObject);

        void DestroyIfActive(GameObject obj)
        {
            if (obj != null)
                PhotonNetwork.Destroy(obj);
        }
    }

    //override void ON
}
