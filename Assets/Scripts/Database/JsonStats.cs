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
    /*
    public class JsonStats : SerializedMonoBehaviour
    {
        
        public static JsonStats instance;
        void Awake() { instance = this; }

        public JsonInfo info;

        //JsonInfo.StartStop CurrentTime = new JsonInfo.StartStop(DateTime.Now.AddHours(-1).ToString(), DateTime.Now.ToString());
        //info.AddInfo(CurrentTime);
        private string StartTime;


        public void OnApplicationQuit()
        {
            info.AddTime(new JsonInfo.StartStop(StartTime, DateTime.Now.ToString()));
            StartCoroutine(SaveJson());
        }
        private void Start()
        {
            InGameManager.OnGameEnd += AddGame;

            StartTime = DateTime.Now.ToString();
            //JsonInfo.StartStop CurrentTime = new JsonInfo.StartStop(DateTime.Now.AddHours(-1).ToString(), DateTime.Now.ToString());
            //info.AddInfo(CurrentTime);
        }
        public void AddGame(Result result)
        {
            bool Won = (int)GetPlayerTeam(PhotonNetwork.LocalPlayer) == (int)result;
            int Kills = (int)GetPlayerVar(ID.KillCount, PhotonNetwork.LocalPlayer);
            int Damage = (int)GetPlayerVar(ID.DamageDone, PhotonNetwork.LocalPlayer);
            info.AddGame(new JsonInfo.Game(Won, Kills, Damage, DateTime.Now));
            //StartCoroutine(SaveJson());
        }
        [Button]
        public void Save() { StartCoroutine(SaveJson()); }

        IEnumerator SaveJson()
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            Debug.Log(DataToJson(info));
            formData.Add(new MultipartFormDataSection("username", Steam.SteamAccess.ID));
            formData.Add(new MultipartFormDataSection("JsonInfo", JsonUtility.ToJson(info)));
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/saveJson.php", formData);

            yield return www.SendWebRequest();
            //Debug.Log(www.downloadHandler.text);

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Game Saved With Code: " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Save Data Failed. Error: " + www.error);
            }
        }



        public string DataToJson(JsonInfo data) { return JsonUtility.ToJson(data); }

        public JsonInfo JsonToData(string data) { return JsonUtility.FromJson<JsonInfo>(data); }

        public void SetNewInfo(string Data)
        {
            info = JsonToData(Data);
        }
    }
    [System.Serializable]
    public struct JsonInfo
    {
        public List<StartStop> PlayTimes;
        public List<Game> Games;

        public void AddTime(StartStop Times) { PlayTimes.Add(Times); }
        public void AddGame(Game game) { Games.Add(game); }

        public JsonInfo(List<StartStop> PlayTimes, List<Game> Games)
        {
            this.PlayTimes = PlayTimes;
            this.Games = Games;
        }
        [System.Serializable]
        public struct StartStop
        {
            public string Start, End;
            public StartStop(string Start, string End) { this.Start = Start; this.End = End; }
        }
        [System.Serializable]
        public struct Game
        {
            public bool Winner;
            public int Kills, Damage;
            public DateTime End;

            public Game(bool Winner, int Kills, int Damage, DateTime End) { this.Winner = Winner; this.Kills = Kills; this.Damage = Damage; this.End = End; }
        }
    }
    */
    
}

