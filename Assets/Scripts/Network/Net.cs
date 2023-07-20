using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Odin
{
    public static class Net
    {
        public static class ID
        {
            public static string PlayerTeam = "Team";
            public static string PlayerHealth = "Health";

            public static string KillCount = "KillCount";
            public static string DamageDone = "DamageDone";
            public static string ELO = "ELO";
            public static string Username = "Username";

            public static string GameState = "State";
            public static string Result = "Result";

            public static string DoorState = "DoorState";
            
        }
        
        

        public static bool ShouldDebug = false;

        public static bool Alive(Player player) { return (int)GetPlayerVar(ID.PlayerHealth, player) > 0; }

        public static bool Contains(List<GameObject> AllObjects, GameObject myObject)
        {
            for (int i = 0; i < AllObjects.Count; i++)
                if (AllObjects[i] == myObject)
                    return true;
            return false;
        }
        public static bool Initialized() { return Exists(ID.DoorState, null) == true; }

        #region Exists
        public static bool Exists(string text, Player player)
        {
            if (player == null)
            {
                if (PhotonNetwork.CurrentRoom == null)
                    return false;
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(text))
                    return true;
                return false;
            }
            return player.CustomProperties.ContainsKey(text);
        }

        #endregion
        #region Get
        public static object GetGameVar(string text)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(text, out object temp))
                return temp;
            else
            {
                Debug.LogError("Game Variable: " + text + " has not been set");
                return null;
            }
        }
        public static object GetPlayerVar(string text, Player player)
        {
            if (player.CustomProperties.TryGetValue(text, out object temp))
                return temp;
            else
            {
                Debug.LogError("Player Variable: " + text + " has not been set");
                return null;
            }
        }
        #endregion


        #region set
        public static void SetGameVar(string text, object var)
        {
            if (ShouldDebug) Debug.Log("SetGameVar of string: " + text);
            Hashtable Hash = new Hashtable();
            Hash.Add(text, var);
            PhotonNetwork.CurrentRoom.SetCustomProperties(Hash);
        }
        public static void SetPlayerVar(string text, object var, Player player)
        {
            if (ShouldDebug) Debug.Log("SetPlayerVar of string: " + text);
            Hashtable Hash = new Hashtable();
            Hash.Add(text, var);
            player.SetCustomProperties(Hash);
        }
        #endregion

        #region ShortCuts
        public static GameState GetGameState() { return (GameState)GetGameVar(ID.GameState); }

        public static Team GetPlayerTeam(Player player) { return (Team)GetPlayerVar(ID.PlayerTeam, player); }
        #endregion
    }
}


