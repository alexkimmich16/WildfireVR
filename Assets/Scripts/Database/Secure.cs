using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Odin.Net;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System.Linq;
namespace Data
{
    public class Secure : SerializedMonoBehaviour
    {
        public static Secure instance;
        void Awake() { instance = this; }
        #region server
        private const string ServerUrl = "https://arcane-citadel-75129.herokuapp.com/"; // Replace with your Heroku app URL
        public void GetData() { StartCoroutine(GetDataCoroutine()); }

        private IEnumerator GetDataCoroutine()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(ServerUrl + "/api/data"))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    string response = request.downloadHandler.text;
                    Debug.Log(response);
                    // Process the response data here
                }
            }
        }
        #endregion
        public DataInfo CurrentInfo;

        public delegate void LoadFinish();
        public event LoadFinish OnLoadFinish;


        [Button] public void CallRegister() { StartCoroutine(Register()); }
        [Button] public void LoadDataButton() { StartCoroutine(LoadData()); }
        [Button] public void SaveDataButton() { StartCoroutine(SaveData()); }
       
        
        IEnumerator Register()
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("username", Steam.SteamAccess.ID));
            //Debug.Log(Steam.SteamAccess.ID);
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/register.php", formData);
            yield return www.SendWebRequest();
            Debug.Log(www.downloadHandler.text);
            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                Dictionary<string, string> RestrictionDictionary = new Dictionary<string, string>() { { "0", "User Created Successfully" }, { "001", "Connection Failed" }, { "002", "Name Check Failed" }, { "003", "Name Already Exists" }, { "004", "Insert Query Failed" }, };

                if (RestrictionDictionary.ContainsKey(responseText))
                    Debug.Log(RestrictionDictionary[responseText]);
                else
                    Debug.Log("Unknown Response: " + responseText);
            }
            else
            {
                Debug.Log("User Creation Failed. Error: " + www.error);
            }
        }
        IEnumerator SaveData()
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("username", Steam.SteamAccess.ID));
            formData.Add(new MultipartFormDataSection("EXP", CurrentInfo.Experience.ToString()));
            formData.Add(new MultipartFormDataSection("Currency", CurrentInfo.Currency.ToString()));
            formData.Add(new MultipartFormDataSection("ELO", CurrentInfo.ELO.ToString()));
            formData.Add(new MultipartFormDataSection("WinCount", CurrentInfo.WinCount.ToString()));
            formData.Add(new MultipartFormDataSection("LoseCount", CurrentInfo.LoseCount.ToString()));
            formData.Add(new MultipartFormDataSection("KillCount", CurrentInfo.KillCount.ToString()));
            formData.Add(new MultipartFormDataSection("DamageCount", CurrentInfo.DamageCount.ToString()));
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/savedata.php", formData);

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
        IEnumerator LoadData()
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("username", Steam.SteamAccess.ID));
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/load.php", formData);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                if(www.downloadHandler.text == "003")//no account exists
                {
                    StartCoroutine(Register());
                    yield break;
                }
                else if (www.downloadHandler.text.Length == 3)
                {
                    Debug.LogError("Load Data Failed. Error: " + www.downloadHandler.text);
                    yield break;
                }
                
                Debug.Log(www.downloadHandler.text);
                string[] webResults = www.downloadHandler.text.Split('\t');
                int[] intArray = webResults.Select(str => int.Parse(str)).ToArray();
                DataInfo info = new DataInfo(intArray[0], intArray[1], intArray[2], intArray[3], intArray[4], intArray[5], intArray[6]);
                CurrentInfo = info;
                OnLoadFinish?.Invoke();


            }
            else
            {
                Debug.Log("Save Data Failed. Error: " + www.error);
            }
        }

        public struct DataInfo
        {
            public DataInfo(int Experience, int Currency, int ELO, int WinCount, int LoseCount, int KillCount, int DamageCount)
            {
                this.ELO = ELO;
                this.Currency = Currency;
                this.Experience = Experience;
                this.WinCount = WinCount;
                this.LoseCount = LoseCount;
                this.KillCount = KillCount;
                this.DamageCount = DamageCount;
            }

            public int Experience;
            public int Currency;
            public int ELO;

            public int WinCount, LoseCount;
            public int KillCount;
            public int DamageCount;
        }
        public void EndGameManage(Result result)
        {
            Team MyTeam = GetPlayerTeam(PhotonNetwork.LocalPlayer);
            Team WinningTeam = result == Result.AttackWon ? Team.Attack : Team.Defense;

            OutCome outCome = MyTeam == WinningTeam ? OutCome.Win : OutCome.Loss;

            if (MyTeam == Team.Spectator)
            {
                return;
            } 

            int GameDamageDone = GetPlayerInt(DamageDoneCount, PhotonNetwork.LocalPlayer);
            int GameKills = GetPlayerInt(KillCount, PhotonNetwork.LocalPlayer);

            int OutcomeExperience = EXP.instance.GameEndExperience(MyTeam, outCome);
            int KillExperience = EXP.instance.KillExperience(GameKills);
            int DamageExperience = EXP.instance.DamageExperience(GameDamageDone);
            int SecondsExperience = EXP.instance.SecondsExperience(Mathf.RoundToInt(InGameManager.instance.FinishTime - InGameManager.instance.Timer));

            int TotalEarnedEXP = OutcomeExperience + KillExperience + DamageExperience + SecondsExperience;
            int PreviousEXP = CurrentInfo.Experience;
            int NewEXP = PreviousEXP + TotalEarnedEXP;
            Debug.Log("EXP Changed From " + PreviousEXP + " to " + NewEXP);

            bool LeveledUp = EXP.instance.LeveledUp(PreviousEXP, NewEXP);

            int GameCurrency = Currency.instance.GameEndCurrency(MyTeam, outCome);
            int LevelUpExperience = LeveledUp ? Currency.instance.CurrencyPerLevelup : 0;
            int TotalEarnedCurrency = GameCurrency + LevelUpExperience;
            int NewCurrency = CurrentInfo.Currency + TotalEarnedCurrency;
            Debug.Log("Currency Changed From " + PreviousEXP + " to " + NewCurrency);

            int OldELO = CurrentInfo.ELO;
            int NewELO = ELO.instance.MyNewELO(MyTeam, OldELO, WinningTeam);
            Debug.Log("ELO Changed From " + OldELO + " to " + NewELO);

            int PreviousKillCount = CurrentInfo.KillCount;
            int NewKills = PreviousKillCount + GameKills;
            Debug.Log("Kills Changed From " + PreviousKillCount + " to " + NewKills);

            int NewWinCount = CurrentInfo.WinCount;
            int NewLoseCount = CurrentInfo.LoseCount;

            if (result != Result.UnDefined)
            {
                bool Won = (int)MyTeam == (int)result;
                NewWinCount += Won ? 1 : 0;
                NewLoseCount += !Won ? 1 : 0;
            }

            int PreviousDamage = CurrentInfo.DamageCount;
            int NewDamageCount = PreviousDamage + GameDamageDone;

            DataInfo saveInfo = new DataInfo(NewELO, NewCurrency, NewEXP, NewWinCount, NewLoseCount, NewKills, NewDamageCount);
            CurrentInfo = saveInfo;

            StartCoroutine(SaveData());
            ///wait for database to create saveinfo
        }
        public void Start()
        {
            //OnRecievedInfo += RecieveLoadedStats;
            StartCoroutine(LoadData());
            InGameManager.OnGameEnd += EndGameManage;

            SetPlayerInt(ELOText, CurrentInfo.ELO, PhotonNetwork.LocalPlayer);
        }
    }
    //elo
    //gear
    //currency
    //stats
    //achievements
    //tutorial progress
    //level

    //$updatequery = "UPDATE players SET EXP = " . $EXP. ", Currency = " . $Currency. ", ELO = " . $ELO. " WHERE username = '" . $username. "';";
}
//
