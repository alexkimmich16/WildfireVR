using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class BillBoardManager : MonoBehaviour
{
    public List<TextMeshProUGUI> Health = new List<TextMeshProUGUI>();
    public TextMeshProUGUI DisplayVictory;
    #region Singleton + Classes
    public static BillBoardManager instance;
    void Awake() { instance = this; }
    #endregion
    void Update()
    {
        for (int i = 0; i < Health.Count; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("HEALTH"))
                {
                    Health[i].gameObject.SetActive(true);
                    var oldHealthVAR = PhotonNetwork.PlayerList[i].CustomProperties["HEALTH"];
                    int HealthNum = (int)oldHealthVAR;
                    Health[i].text = "Player" + i + " Health: " + HealthNum;
                }
                else
                    Health[i].gameObject.SetActive(false);
            }
            else
                Health[i].gameObject.SetActive(false);
        }

        if(InGameManager.instance.currentState == GameState.Waiting)
        {
            DisplayVictory.text = "Waiting For Players";
        }
        else if (InGameManager.instance.currentState == GameState.CountDown)
        {
            float adjustedTime = InGameManager.WarmupTime - InGameManager.instance.WarmupTimer;
            string TimeText = adjustedTime.ToString("F2");
            DisplayVictory.text = "Starting in: " + TimeText + " Seconds";
        }
        else if (InGameManager.instance.currentState == GameState.Active)
        {
            DisplayVictory.text = "Started";
        }
        else if (InGameManager.instance.currentState == GameState.Finished)
        {
            DisplayVictory.text = OnWin(InGameManager.instance.result);
        }
    }
    public string OnWin(Result result)
    {
        if (result == Result.DefenseWon)
            return "Red Team Wins!";
        else if (result == Result.AttackWon)
            return "Blue Team Wins!";
        else
            return "Tie!";
    }
    public void ChangeTeam()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[i])
            {
                InGameManager.instance.ChangePlayerSide(i);
                return;
            }
        }
            
    }
}
