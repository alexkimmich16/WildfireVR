using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using static Odin.Net;
using System.Linq;

public class BillBoardManager : MonoBehaviour
{
    #region Singleton + Classes
    public static BillBoardManager instance;
    void Awake() { instance = this; }
    #endregion

    public List<TextMeshProUGUI> Health = new List<TextMeshProUGUI>();
    public TextMeshProUGUI DisplayVictory;

    public GameObject RestartButton;
    public GameObject ChangeTeamButton;

    public TextMeshProUGUI StateText;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI MyPlayerSpawnText;
    public TextMeshProUGUI MyTeamText;
    public TextMeshProUGUI AliveText;

    public TextMeshProUGUI MyKills;

    public TextMeshProUGUI AttackTeamCount, AttackTeamAlive;
    public TextMeshProUGUI DefenseTeamCount, DefenseTeamAlive;



    public void UpdateWall()
    {
        if (!Initialized())
            return;

        StateText.text = InGameManager.instance.CurrentState.ToString();

        string TimerStateText = InGameManager.instance.CurrentState == GameState.Warmup ? "WarmupTime" : "FinishTime";
        TimerText.text = TimerStateText + ": " + InGameManager.instance.Timer.ToString();
        //FinishTimeText.text = "FinishTime: " + InGameManager.instance.Timer.ToString();

        if (AllPlayersLoaded())
        {
            AttackTeamCount.text = "Attack Team Count: " + InGameManager.instance.SideCount(Team.Attack);
            DefenseTeamCount.text = "Defense Team Count: " + InGameManager.instance.SideCount(Team.Defense);

            AttackTeamAlive.text = "Attack Team Alive: " + InGameManager.instance.TotalAlive(Team.Attack);
            DefenseTeamAlive.text = "Defense Team Alive: " + InGameManager.instance.TotalAlive(Team.Defense);
        }

        if (Exists(ID.PlayerTeam, PhotonNetwork.LocalPlayer))
            MyTeamText.text = "MyTeam: " + GetPlayerTeam(PhotonNetwork.LocalPlayer).ToString();
        if (Exists(ID.PlayerHealth, PhotonNetwork.LocalPlayer))
        {
            AliveText.text = "Alive: " + Alive(PhotonNetwork.LocalPlayer).ToString();
            MyKills.text = "MyKills: " + GetPlayerVar(ID.KillCount, PhotonNetwork.LocalPlayer).ToString();
            
        }
    }
    bool AllPlayersLoaded() { return PhotonNetwork.PlayerList.Any(player => !Exists(ID.PlayerHealth, player) || !Exists(ID.PlayerTeam, player)); }
    
    void Update()
    {
        if (!PhotonNetwork.InRoom)
            return;

        UpdateWall();
        for (int i = 0; i < Health.Count; i++)
        {
            bool Works = i < PhotonNetwork.PlayerList.Length && Exists(ID.PlayerHealth, PhotonNetwork.PlayerList[i]);
            Health[i].gameObject.SetActive(Works);
            if (Works)
            {
                int HealthNum = (int)GetPlayerVar(ID.PlayerHealth, PhotonNetwork.PlayerList[i]);
                string Username = (string)GetPlayerVar(ID.Username, PhotonNetwork.PlayerList[i]);
                Health[i].text = Username + " Health: " + HealthNum;
            }
        }
        if (!Initialized())
            return;

        GameState state = GetGameState();

        

        if (state == GameState.Waiting)
        {
            DisplayVictory.text = "Waiting For Players";
        }
        else if (state == GameState.Warmup)
        {
            DisplayVictory.text = "Starting in: " + InGameManager.instance.Timer.ToString("F2") + " Seconds";
        }
        else if (state == GameState.Active)
        {
            //float FinishTimer = GetGameFloat("FinishTimer");
            if (DoorManager.instance.Sequence > DoorState.OpenOutDoor)
            {
                DisplayVictory.text = "Started!  Time Left: " + InGameManager.instance.Timer;
            }
            else
            {
                DisplayVictory.text = "Waiting For Doors To Open";
            }
            
        }
        else if (state == GameState.Finished)
        {
            if (Exists(ID.Result, null) && Exists(ID.PlayerTeam, PhotonNetwork.LocalPlayer))
            {
                Result result = (Result)GetGameVar(ID.Result);
                if (result != Result.UnDefined)
                    DisplayVictory.text = OnWin(result);
            }
                
        }
    }
    public void SetOutcome(Result result)
    {
        //DisplayVictory.text = OnWin(result);
    }
    public string OnWin(Result result)
    {
        Team team = GetPlayerTeam(PhotonNetwork.LocalPlayer);

        if(team != Team.Spectator)//player
            return GetWinnerText() + " Won so you: " + GetPlayerWonText();
        else//spectator
            return GetWinnerText() + " Won";


        string GetWinnerText()
        {
            return (result == Result.AttackWon) ? "Attack" : "Defense";
        }
        string GetPlayerWonText()
        {
            return (team == Team.Attack && result == Result.AttackWon || team == Team.Defense && result == Result.DefenseWon) ? "Won" : "Lost";
        }
    }
    public void ChangeTeam()
    {
        //int LocalNum = GetLocal();
        //InGameManager.instance.ChangePlayerSide(LocalNum);
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
        GameState state = Net.GetGameState();
        if (state == GameState.Active)
        {

        }
        */
    }
}
