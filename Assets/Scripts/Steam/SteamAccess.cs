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


            //subscribe to all save events
            //SoundManager.OnVolumeChange += SaveToSteamCloud;
        }

        private void OnApplicationQuit()
        {
            // Shutdown Steamworks
            SteamAPI.Shutdown();
        }
        /*
        public void SaveToSteamCloud()
        {
            //SET ALL VOLUMES
            PlayerPrefs.SetFloat(soundType.ToString(), NewVolume);



            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            SteamRemoteStorage.FileWrite(fileName, bytes, bytes.Length);
        }

        public string LoadFromSteamCloud(string fileName)
        {
            if (!SteamRemoteStorage.FileExists(fileName))
            {
                Debug.LogError("File does not exist in Steam Cloud");
                return null;
            }

            int byteCount = (int)SteamRemoteStorage.FileSize(fileName);
            byte[] bytes = new byte[byteCount];

            int bytesRead = SteamRemoteStorage.FileRead(fileName, bytes, byteCount);
            return System.Text.Encoding.UTF8.GetString(bytes, 0, bytesRead);
        }*/
    }
    public enum Player
    {
        Alex = 0,
        Tester1 = 1,
        Tester2 = 2,
    }
}

