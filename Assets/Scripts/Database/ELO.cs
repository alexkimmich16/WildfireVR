using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;
using static Odin.Net;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
namespace Data
{
    public class ELO : MonoBehaviour
    {
        void Awake() { instance = this; }
        public static ELO instance;
        public int StartingElo;
        private const int K = 32; // The constant determining the magnitude of rating changes

        public int[] GetTeamELO(Team team) { return PhotonNetwork.PlayerList.Where(p => GetPlayerTeam(p) == team).Select(player => GetPlayerInt(ELOText, player)).ToArray(); }
        public int MyNewELO(Team team, int PreviousELO, Team Winner)
        {
            CalculateElo(Winner, GetTeamELO(Team.Attack), GetTeamELO(Team.Defense), out int[] NewAttack, out int[] NewDefense);

            int[] MyTeamOld = GetTeamELO(team);
            int MyIndex = MyTeamOld.ToList().FindIndex(x => x == PreviousELO);


            int[] MyTeamNew = team == Team.Attack ? NewAttack : NewDefense;

            return MyTeamNew[MyIndex];
        }
        public static void CalculateElo(Team Winner, int[] Attacking, int[] Defending, out int[] newRatingsAttacking, out int[] newRatingsDefending)
        {
            // Calculate expected probabilities for each team
            double expectedScoreTeamA = CalculateExpectedScore(Attacking, Defending);
            double expectedScoreTeamB = CalculateExpectedScore(Defending, Attacking);

            // Calculate new ratings for each player in Team A
            newRatingsAttacking = CalculateNewRatings(Attacking, expectedScoreTeamA, Winner == Team.Attack ? 1 : 0);

            // Calculate new ratings for each player in Team B
            newRatingsDefending = CalculateNewRatings(Defending, expectedScoreTeamB, Winner == Team.Defense ? 1 : 0);
        }

        private static double CalculateExpectedScore(int[] Attacking, int[] Defending)
        {
            // Calculate the sum of ratings for each team
            double AttackingRatingSum = Attacking.Sum();
            double DefendingRatingSum = Defending.Sum();

            // Calculate the expected score for Team A
            double expectedScore = 1 / (1 + Math.Pow(10, (DefendingRatingSum - AttackingRatingSum) / 400.0));

            return expectedScore;
        }

        private static int[] CalculateNewRatings(int[] team, double expectedScore, int actualScore)
        {
            int[] newRatings = new int[team.Length];

            for (int i = 0; i < team.Length; i++)
            {
                int rating = team[i];
                int newRating = rating + (int)(K * (actualScore - expectedScore));
                newRatings[i] = newRating;
            }

            return newRatings;
        }
    }
}

