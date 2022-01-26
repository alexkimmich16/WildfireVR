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

    public GameObject RestartButton;
    public GameObject ChangeTeamButton;
    #region Singleton + Classes
    public static BillBoardManager instance;
    void Awake() { instance = this; }
    #endregion
    void Update()
    {
        if(PhotonNetwork.InRoom == true)
        {
            for (int i = 0; i < Health.Count; i++)
            {
                if (i < PhotonNetwork.PlayerList.Length)
                {
                    int HealthNum = NetworkManager.GetPlayerInt("HEALTH", PhotonNetwork.PlayerList[i]);
                    Health[i].gameObject.SetActive(true);
                    Health[i].text = "Player" + i + " Health: " + HealthNum;
                }
                else
                    Health[i].gameObject.SetActive(false);
            }
            GameState state = NetworkManager.GetGameState();
            if (state == GameState.Waiting)
            {
                DisplayVictory.text = "Waiting For Players";
            }
            else if (state == GameState.CountDown)
            {
                float WarmupTimer = NetworkManager.GetGameFloat("WarmupTimer");
                float adjustedTime = InGameManager.WarmupTime - WarmupTimer;
                string TimeText = adjustedTime.ToString("F2");
                DisplayVictory.text = "Starting in: " + TimeText + " Seconds";
            }
            else if (state == GameState.Active)
            {
                float FinishTimer = NetworkManager.GetGameFloat("FinishTimer");
                float Left = InGameManager.FinishTime - FinishTimer;
                DisplayVictory.text = "Started!  Time Left: " + Left;
            }
            else if (state == GameState.Finished)
            {
                DisplayVictory.text = OnWin(InGameManager.instance.result);
            }
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
        int LocalNum = NetworkManager.GetLocal();
        InGameManager.instance.ChangePlayerSide(LocalNum);
    }
    public void SetResetButton(bool Set)
    {
        RestartButton.SetActive(Set);
    }
    public void SetChangeButton(bool Set)
    {
        ChangeTeamButton.SetActive(Set);
    }

    private void Start()
    {
        /*
        GameState state = NetworkManager.GetGameState();
        if (state == GameState.Active)
        {

        }
        */
    }
}
