using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
namespace Data
{
    public class EXP : MonoBehaviour
    {
        public static EXP instance;
        void Awake() { instance = this; }

        [System.Serializable]
        public struct ExperienceBoarders
        {
            public int AppliedLevels;
            public int ExperienceToNext;
        } 
        public List<ExperienceBoarders> Stats = new List<ExperienceBoarders>();
        

        public List<int> LevelThresholds; //not updated every frame!! only Start!!

        [Header("Experience")]
        public int KillEXP = 150;
        public int WinEXP = 250;
        public int LoseEXP = 100;
        public int PerDamageEXP = 2;
        public float PerSecondEXP = 1f;

        public int KillExperience(int Kills) { return Kills * KillEXP; }
        public int GameEndExperience(Team team, OutCome outCome)
        {
            if (team == Team.Spectator || outCome == OutCome.UnDefined)
                return 0;
            return outCome == OutCome.Win ? WinEXP : LoseEXP;
        }
        public int DamageExperience(int Damage) { return Damage * PerDamageEXP; }
        public int SecondsExperience(int Seconds) { return Mathf.RoundToInt(Seconds * PerSecondEXP); }
        private void Start()
        {
            LevelThresholds = GetLevelThresholds();
        }
        
        public List<int> GetLevelThresholds()
        {
            List<int> NewList = new List<int>();
            int PreviousExperience = 0;
            for (int i = 0; i < 100; i++)
            {
                int ExperienceAdd = GetExperienceBoarder(i);
                int NewExperienceTotal = ExperienceAdd + PreviousExperience;

                NewList.Add(NewExperienceTotal);
                PreviousExperience = NewExperienceTotal;
            }
            return NewList;
        }
        public int GetLevel(int EXP)
        {
            int Threshold = LevelThresholds.FirstOrDefault(x => EXP < x);
            int IndexNum = LevelThresholds.IndexOf(Threshold);
            int CurrentLevelEXP = IndexNum != 0 ? LevelThresholds[IndexNum - 1] : 0;
            return IndexNum;
        }
        
        public int GetExperienceBoarder(int Index)
        {
            int IndexLeft = Index;
            for (int i = 0; i < Stats.Count; i++)
            {
                IndexLeft -= Stats[i].AppliedLevels;
                if (IndexLeft < 0)
                {
                    return Stats[i].ExperienceToNext;
                }
            }
            Debug.LogError("Couldn't Find Boarder");
            return 0;
        }

        public bool LeveledUp(int OldExperience, int NewExperience) { return GetLevel(OldExperience) != GetLevel(NewExperience); }
    }

    /*

    public int EXPINPUT;
        public int LEVEL;
        public int EXPOver;


    private void Update()
        {
            LevelThresholds = GetLevelThresholds();
            LEVEL = GetLevel(EXPINPUT);
            EXPOver = EXPINPUT - (LEVEL != 0 ? LevelThresholds[LEVEL - 1] : 0);
        }

     
     */
}

