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
    
    public TextMeshProUGUI AttackTeamCount;
    public TextMeshProUGUI DefenseTeamCount;

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
        
        AttackTeamCount.text = "Attack Team Count: " + InGameManager.instance.SideCount(Team.Attack);
        DefenseTeamCount.text = "Defense Team Count: " + InGameManager.instance.SideCount(Team.Defense);

        if (InGameManager.instance.FoundSpawn)
        {
            MyTeamText.text = "MyTeam: " + GetPlayerTeam(PhotonNetwork.LocalPlayer).ToString();
            MyPlayerSpawnText.text = "PlayerSpawn: " + GetPlayerInt(PlayerSpawn, PhotonNetwork.LocalPlayer).ToString();
            AliveText.text = "Alive: " + Alive(PhotonNetwork.LocalPlayer).ToString();
        }
    }

    #region Singleton + Classes
    public static BillBoardManager instance;
    void Awake() { instance = this; }
    #endregion
    void Update()
    {
        if(PhotonNetwork.InRoom == true)
        {
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
            if (Initialized())
            {
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
                    DisplayVictory.text = OnWin(InGameManager.instance.result);
                }
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
        int LocalNum = GetLocal();
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
        GameState state = Net.GetGameState();
        if (state == GameState.Active)
        {

        }
        */
    }
}
