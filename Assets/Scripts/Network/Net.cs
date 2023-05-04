using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Odin
{
    public static class Net
    {
        public static string PlayerTeam = "Team";
        //public static string PlayerAlive = "Alive";
        public static string PlayerHealth = "Health";

        //public static string GameWarmupTimer = "WarmupTimer";
        //public static string GameFinishTimer = "FinishTimer";
        public static string GameStateText = "State";

        public static string GameOutcome = "Outcome";

        //public static string AttackTeamCount = "AttackTeam";
        //public static string DefenseTeamCount = "DefenseTeam";

        //public static List<string> DoorNames = new List<string>() { "ElevatorHeight", "InnerGateHeight", "OuterGateHeight" };

        public static string DoorState = "DoorState";

        public static bool ShouldDebug = false;

        public static bool Alive(Player player)
        {
            return GetPlayerInt(PlayerHealth, player) > 0;
        }

        public static bool Contains(List<GameObject> AllObjects, GameObject myObject)
        {
            for (int i = 0; i < AllObjects.Count; i++)
                if (AllObjects[i] == myObject)
                    return true;
            return false;
        }
        public static bool Initialized()
        {
            return Exists(DoorState, null) == true;
        }
        public static int GetLocal()
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[i])
                {
                    return i;
                }
            }
            Debug.LogError("Get Local Failure");
            return 100;
        }
        #region NetworkGet
        public static float GetGameFloat(string text)
        {
            //Debug.Log("Getfloat");
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(text, out object temp))
                return (float)temp;
            else
            {
                Debug.LogError("GetGameFloat.main with string: " + text + " has not been set");
                return 0f;
            }
        }
        public static GameState GetGameState()
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameStateText, out object temp))
                return (GameState)temp;
            else
            {
                Debug.LogError("GetGameState.CurrentRoom with string: " + GameStateText + " has not been set");
                return GameState.Finished;
            }
        }
        public static Team GetPlayerTeam(Player player)
        {
            if (player.CustomProperties.TryGetValue(PlayerTeam, out object temp))
                return (Team)temp;
            else
            {
                Debug.LogError("GetInt.GetPlayerTeam with string: " + PlayerTeam + " has not been set");
                return Team.Attack;
            }
        }
        public static int GetGameInt(string text)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(text, out object temp))
                return (int)temp;
            else
            {
                Debug.LogError("GetHash.GetInt.OfRoom with string: " + text + " has not been set");
                return 100;
            }
        }
        public static Result GetGameResult()
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameOutcome, out object temp))
                return (Result)temp;
            else
            {
                Debug.LogError("GetHash.GetInt.OfRoom with string: " + GameOutcome + " has not been set");
                return Result.UnDefined;
            }
        }
        public static int GetPlayerInt(string text, Player player)
        {
            if (player.CustomProperties.TryGetValue(text, out object temp))
                return (int)temp;
            else
            {
                Debug.LogError("GetHash.GetInt.OfPlayer with string: " + text + " has not been set");
                return 100;
            }
        }
        public static bool GetGameBool(string text)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(text, out object temp))
                return (bool)temp;
            else
            {
                Debug.LogError("GetHash.GetBool with string: " + text + "has not been set");
                return true;
            }
        }

        public static bool GetPlayerBool(string text, Player player)
        {
            if (player.CustomProperties.TryGetValue(text, out object temp))
                return (bool)temp;
            else
            {
                Debug.LogError("GetHash.GetBool with string: " + text + "has not been set");
                return true;
            }
        }
        #endregion Exists
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
        #region NetworkSet
        public static void SetGameResult(Result result)
        {
            if (ShouldDebug) Debug.Log("SetGameResult");
            Hashtable TeamHash = new Hashtable();
            TeamHash.Add(GameOutcome, result);
            PhotonNetwork.CurrentRoom.SetCustomProperties(TeamHash);
        }
        public static void SetGameFloat(string text, float Num)
        {
            if(ShouldDebug)Debug.Log("SetGameFloat");
            Hashtable TeamHash = new Hashtable();
            TeamHash.Add(text, Num);
            PhotonNetwork.CurrentRoom.SetCustomProperties(TeamHash);
        }
        public static void SetGameState(GameState state)
        {
            if (ShouldDebug) Debug.Log("SetGameState");
            Hashtable TeamHash = new Hashtable();
            TeamHash.Add(GameStateText, state);
            PhotonNetwork.CurrentRoom.SetCustomProperties(TeamHash);
        }
        public static void SetPlayerTeam(Team team, Player player)
        {
            if (ShouldDebug) Debug.Log("SetPlayerTeam");
            Hashtable TeamHash = new Hashtable();
            TeamHash.Add(PlayerTeam, team);
            player.SetCustomProperties(TeamHash);
        }
        public static void SetPlayerBool(string text, bool State, Player player)
        {
            if (ShouldDebug) Debug.Log("SetPlayerBool");
            Hashtable HealthHash = new Hashtable();
            HealthHash.Add(text, State);
            player.SetCustomProperties(HealthHash);
        }
        public static void SetGameBool(string text, bool State)
        {
            if (ShouldDebug) Debug.Log("SetGameBool");
            Hashtable HealthHash = new Hashtable();
            //Debug.Log("set");
            HealthHash.Add(text, State);
            PhotonNetwork.CurrentRoom.SetCustomProperties(HealthHash);
        }
        public static void SetPlayerInt(string text, int SetNum, Player player)
        {
            if (ShouldDebug) Debug.Log("SetPlayerInt");
            Hashtable HealthHash = new Hashtable();
            HealthHash.Add(text, SetNum);
            player.SetCustomProperties(HealthHash);
        }
        public static void SetGameInt(string text, int SetNum)
        {
            if (ShouldDebug) Debug.Log("SetGameInt");
            Hashtable HealthHash = new Hashtable();
            HealthHash.Add(text, SetNum);
            PhotonNetwork.CurrentRoom.SetCustomProperties(HealthHash);
        }
        #endregion
    }
}


