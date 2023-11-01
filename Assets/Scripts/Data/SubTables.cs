using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Networking;
using System;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
namespace Data
{
    public class SubTables : MonoBehaviour
    {
        public static SubTables instance;
        void Awake() { instance = this; }
        public DateTime FirstOpenedApplication;
        //[Button] public void TestSaveGame() { StartCoroutine(SaveGame(new Game(true, 3, 5))); }
        //[Button] public void TestSaveLogin() { StartCoroutine(SaveLogin(new Login(DateTime.Now.AddHours(-1)))); }
        
        void Start()
        {
            //InGameManager.OnGameEnd += AddGame;
            if(FirstOpenedApplication.Year < 2000)
                FirstOpenedApplication = DateTime.Now;
        }

        private void OnApplicationQuit()
        {
            AddLogin();
            //Debug.Log(FirstOpenedApplication.ToString());
        }
        #region GameInfo
        public void AddGame(Result result)
        {
            bool Won = (int)GetPlayerTeam(PhotonNetwork.LocalPlayer) == (int)result;
            int Kills = (int)GetPlayerVar(ID.KillCount, PhotonNetwork.LocalPlayer);
            int Damage = (int)GetPlayerVar(ID.DamageDone, PhotonNetwork.LocalPlayer);

            StartCoroutine(SaveGame(new Game(Won, Kills, Damage)));
            //StartCoroutine(SaveJson());
        }
        public IEnumerator SaveGame(Game game)
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("username", Steam.SteamAccess.ID));
            formData.Add(new MultipartFormDataSection("Winner", (game.Winner ? 1 : 0).ToString()));
            formData.Add(new MultipartFormDataSection("Kills", game.Kills.ToString()));
            formData.Add(new MultipartFormDataSection("Damage", game.Damage.ToString()));
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/newGame.php", formData);

            yield return www.SendWebRequest();
            //Debug.Log(www.downloadHandler.text);

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Game Saved With Code: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Save Data Failed. Error: " + www.error + "With Text: " + www.downloadHandler.text);
            }
        }

        [System.Serializable]
        public struct Game
        {
            public bool Winner;
            public int Kills, Damage;
            public DateTime End;

            public Game(bool Winner, int Kills, int Damage) { this.Winner = Winner; this.Kills = Kills; this.Damage = Damage; this.End = DateTime.Now; }
        }
        #endregion


        #region LoginInfo
        public void AddLogin()
        {
            StartCoroutine(SaveLogin(new Login(FirstOpenedApplication)));
        }
        public IEnumerator SaveLogin(Login game)
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("username", Steam.SteamAccess.ID));
            formData.Add(new MultipartFormDataSection("StartTime", game.StartTime.ToString()));
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/newLogin.php", formData);

            yield return www.SendWebRequest();
            //Debug.Log(www.downloadHandler.text);

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Game Saved With Code: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Save Data Failed. Error: " + www.error + "With Text: " + www.downloadHandler.text);
            }
        }




        [System.Serializable]
        public struct Login
        {
            public DateTime StartTime;
            public Login(DateTime LoginTime) { this.StartTime = LoginTime; }
        }
        #endregion


    }
}

