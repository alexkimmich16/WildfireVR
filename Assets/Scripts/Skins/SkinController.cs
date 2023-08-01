using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using static Odin.Net;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace Skin
{
    public class SkinController : MonoBehaviourPunCallbacks
    {
        public static SkinController instance;
        void Awake() { instance = this; }


        public List<ColorSet> Colors;

        public PlayerSkin MyRigSkin;
        //public bool


        [System.Serializable]
        public struct ColorSet
        {
            public Color BodyColor;
            public Color HoodColor;
        }

        public const byte SkinChangeCode = 6;
        void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == SkinChangeCode)
            {
                object[] data = (object[])photonEvent.CustomData;
                int viewId = (int)data[0];
                int skinIndex = (int)data[1];

                PhotonView targetView = PhotonView.Find(viewId);
                if (targetView != null)
                {
                    targetView.GetComponent<PlayerSkin>().ChangeSkin(skinIndex);

                    if (targetView.Owner.IsLocal)
                    {
                        MyRigSkin.ChangeSkin(skinIndex);
                    }
                }
            }
        }


        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            Debug.Log("pt1");
            if (changedProps.ContainsKey(ID.PlayerTeam) && targetPlayer.IsLocal)
            {
                Debug.Log("pt2");

                GameObject player = NetworkPlayerSpawner.instance.SpawnedPlayerPrefab;

                int TeamNum = (int)((Team)changedProps[ID.PlayerTeam]);

                player.GetComponent<PlayerSkin>().RaiseSkinChangeEvent(player.GetPhotonView().ViewID, TeamNum); 
            }
                //NetworkPlayerSpawner.instance.SpawnedPlayerPrefab.GetComponent<PlayerSkin>().ChangeSkin((int)((Team)changedProps[ID.PlayerTeam]));
                    
        }


        public override void OnEnable() { PhotonNetwork.NetworkingClient.EventReceived += OnEvent; }
        public override void OnDisable() { PhotonNetwork.NetworkingClient.EventReceived -= OnEvent; }

        /*
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (targetPlayer.IsLocal)
            {
                RaiseSkinChangeEvent(photonView.ViewID, SkinNum);
            }
                

            if (targetPlayer.IsLocal && changedProps.ContainsKey(ID.PlayerTeam))
                SetNewPosition((Team)changedProps[ID.PlayerTeam]);
        }
        */
    }
}