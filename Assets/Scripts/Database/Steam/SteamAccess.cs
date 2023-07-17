using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

namespace Steam
{
    public class SteamAccess : MonoBehaviour
    {
        void Awake() { instance = this; }
        public static SteamAccess instance;

        public ulong SteamID;
        public string SteamName;
        
        public Player playerID;
        public static string ID { get { return instance.SteamID == 0 ? instance.playerID.ToString() : instance.SteamName; } }

        void Start()
        {
            // Initialize Steamworks
            if (!SteamAPI.Init())
            {
                Debug.LogError("SteamAPI.Init() failed!");
                return;
            }

            // Check if Steam is running
            if (!SteamAPI.IsSteamRunning())
            {
                Debug.LogError("Steam is not running!");
                return;
            }

            // Get the Steam account ID
            SteamID = SteamUser.GetSteamID().m_SteamID;
            SteamName = SteamFriends.GetPersonaName();
            Debug.Log("Steam Account ID: " + SteamID.ToString());
        }

        private void OnApplicationQuit()
        {
            // Shutdown Steamworks
            SteamAPI.Shutdown();
        }
    }
    public enum Player
    {
        Alex = 0,
        Tester1 = 1,
        Tester2 = 2,
    }
}

