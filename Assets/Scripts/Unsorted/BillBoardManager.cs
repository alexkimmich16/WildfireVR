using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;

public class BillBoardManager : MonoBehaviour
{
    public List<TextMeshProUGUI> Health = new List<TextMeshProUGUI>();
    public TextMeshProUGUI DisplayVictory;

    public GameObject RestartButton;
    public GameObject ChangeTeamButton;

    public TextMeshProUGUI StateText;
    public TextMeshProUGUI WarmupTimeText;
    public TextMeshProUGUI FinishTimeText;
    public TextMeshProUGUI MyPlayerSpawnText;
    public TextMeshProUGUI MyTeamText;
    public TextMeshProUGUI AliveText;
    
    public TextMeshProUGUI AttackTeamCount, AttackTeamAlive;
    public TextMeshProUGUI DefenseTeamCount, DefenseTeamAlive;

    public List<TextMeshProUGUI> DefenseSpawnText = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> AttackSpawnsText = new List<TextMeshProUGUI>();



    public void UpdateWall()
    {
        if (!Initialized())
            return;

        StateText.text = GetGameState().ToString();
        WarmupTimeText.text = "WarmupTime: " + GetGameFloat(GameWarmupTimer).ToString();
        FinishTimeText.text = "FinishTime: " + GetGameFloat(GameFinishTimer).ToString();

        for (int i = 0; i < DefenseSpawns.Count; i++)
        {
            DefenseSpawnText[i].text = DefenseSpawns[i] + ":  " + GetGameBool(DefenseSpawns[i]);
            AttackSpawnsText[i].text = AttackSpawns[i] + ":  " + GetGameBool(AttackSpawns[i]);
        }

        if (AllPlayersLoaded())
        {
            AttackTeamCount.text = "Attack Team Count: " + InGameManager.instance.SideCount(Team.Attack);
            DefenseTeamCount.text = "Defense Team Count: " + InGameManager.instance.SideCount(Team.Defense);

            AttackTeamAlive.text = "Attack Team Alive: " + InGameManager.instance.TotalAlive(Team.Attack);
            DefenseTeamAlive.text = "Defense Team Alive: " + InGameManager.instance.TotalAlive(Team.Defense);
        }

        if (Exists(PlayerTeam, PhotonNetwork.LocalPlayer))
            MyTeamText.text = "MyTeam: " + GetPlayerTeam(PhotonNetwork.LocalPlayer).ToString();
        if (Exists(PlayerSpawn, PhotonNetwork.LocalPlayer))
            MyPlayerSpawnText.text = "PlayerSpawn: " + GetPlayerInt(PlayerSpawn, PhotonNetwork.LocalPlayer).ToString();
        if (Exists(PlayerHealth, PhotonNetwork.LocalPlayer))
            AliveText.text = "Alive: " + Alive(PhotonNetwork.LocalPlayer).ToString();
    }
    bool AllPlayersLoaded()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (Exists(PlayerHealth, PhotonNetwork.PlayerList[i]) == false || Exists(PlayerTeam, PhotonNetwork.PlayerList[i]) == false)
                return false;
        return true;
    }
    #region Singleton + Classes
    public static BillBoardManager instance;
    void Awake() { instance = this; }
    #endregion
    void Update()
    {
        if (!PhotonNetwork.InRoom)
            return;

        UpdateWall();
        for (int i = 0; i < Health.Count; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length && Exists(PlayerHealth, PhotonNetwork.PlayerList[i]) == true)
            {
                int HealthNum = GetPlayerInt(PlayerHealth, PhotonNetwork.PlayerList[i]);
                Health[i].gameObject.SetActive(true);
                Health[i].text = "Player" + i + " Health: " + HealthNum;
            }
            else
                Health[i].gameObject.SetActive(false);
        }
        if (!Initialized())
            return;

        GameState state = GetGameState();
        if (state == GameState.Waiting)
        {
            DisplayVictory.text = "Waiting For Players";
        }
        else if (state == GameState.CountDown)
        {
            float WarmupTimer = GetGameFloat("WarmupTimer");
            float adjustedTime = InGameManager.instance.WarmupTime - WarmupTimer;
            string TimeText = adjustedTime.ToString("F2");
            DisplayVictory.text = "Starting in: " + TimeText + " Seconds";
        }
        else if (state == GameState.Active)
        {
            float FinishTimer = GetGameFloat("FinishTimer");
            float Left = InGameManager.instance.FinishTime - FinishTimer;
            DisplayVictory.text = "Started!  Time Left: " + Left;
        }

        else if (state == GameState.Finished)
        {
            if (Exists(GameOutcome, null))
                if (GetGameResult() != Result.UnDefined)
                    DisplayVictory.text = OnWin(GetGameResult());
        }
    }
    public void SetOutcome(Result result)
    {
        //DisplayVictory.text = OnWin(result);
    }
    public string OnWin(Result result)
    {
        Team team = GetPlayerTeam(PhotonNetwork.LocalPlayer);
        //Debug.Log(result.ToString());
        //Debug.Log(team.ToString());

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
