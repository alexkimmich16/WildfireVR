using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using static Odin.Net;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
namespace Data
{
    public class ELO : SerializedMonoBehaviour
    {
        void Awake() { instance = this; }
        public static ELO instance;

        private float KFactor = 16f;
        
        private int[] AttackSave;
        private int[] DefenseSave;

        public void SaveAllELO()
        {
            AttackSave = GetTeamELO(Team.Attack);
            DefenseSave = GetTeamELO(Team.Defense);
        }

        
        public int[] GetTeamELO(Team team) { return PhotonNetwork.PlayerList.Where(p => GetPlayerTeam(p) == team).Select(player => (int)GetPlayerVar(ID.ELO, player)).ToArray(); }

        public int NewElo(int MyElo, Team team, OutCome outCome)
        {
            if (outCome == OutCome.UnDefined)
                return MyElo;

            int[] MyTeamScore = team == Team.Attack ? AttackSave : DefenseSave;
            int[] OtherTeamScore = team == Team.Attack ? DefenseSave : AttackSave;

            return GetNewValue(MyElo, (float)MyTeamScore.Average(), (float)OtherTeamScore.Average(), outCome == OutCome.Win);
        }

        

        public int GetNewValue(float MyElo, float MyTeamScore, float OtherScore, bool IsWinner)
        {
            float MyTeamExpected = GetExpectedScore(MyTeamScore, OtherScore);

            return (int)UpdateRating(MyElo, MyTeamExpected, IsWinner);



            float GetExpectedScore(float rating1, float rating2)
            {
                return 1f / (1f + Mathf.Pow(10f, (rating2 - rating1) / 400f));
            }

            float UpdateRating(float oldRating, float expectedScore, bool isWinner)
            {
                return oldRating + KFactor * ((isWinner ? 1 : 0) - expectedScore);
            }
        }

        private void Start()
        {
            InGameManager.OnGameStart += SaveAllELO;
        }


        /*
        public int[] AttackTeam;
        public int[] DefenseTeam;
        public bool AttackWins;

        [Button]
        public void Recalculate()
        {
            int[] PreviousAttack = AttackTeam;
            int[] PreviousDefense = DefenseTeam;

            int[] NewAttack = new int[PreviousAttack.Length];
            int[] NewDefense = new int[PreviousDefense.Length];

            for (int i = 0; i < PreviousAttack.Length; i++)
                NewAttack[i] = GetNewValue(PreviousAttack[i], (float)PreviousAttack.Average(), (float)PreviousDefense.Average(), AttackWins);

            for (int i = 0; i < PreviousDefense.Length; i++)
                NewDefense[i] = GetNewValue(PreviousDefense[i], (float)PreviousDefense.Average(), (float)PreviousAttack.Average(), !AttackWins);

            AttackTeam = NewAttack;
            DefenseTeam = NewDefense;

            //GetNewElo(GetPlayerTeam(PhotonNetwork.LocalPlayer), (int)GetPlayerVar(ID.ELO, PhotonNetwork.LocalPlayer), GetPlayerTeam(PhotonNetwork.LocalPlayer) == Team.Attack ? Team.Attack : Team.Defense);
        }
        
        */
    }
}

