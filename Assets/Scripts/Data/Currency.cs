using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    public class Currency : MonoBehaviour
    {
        void Awake() { instance = this; }
        public static Currency instance;

        public int WinCurrency = 10, LoseCurrency = 5;
        public int CurrencyPerLevelup = 25;

        public List<bool> SkinsUnlocked;
        

        public int GameEndCurrency(Team team, OutCome outCome)
        {
            if (team == Team.Spectator || outCome == OutCome.UnDefined)
                return 0;
            return outCome == OutCome.Win ? WinCurrency : LoseCurrency;
        }

        public void SpendCurrency(int Cost)
        {
            //reduce currency
            //set settings for bought item
        }

        


    }
}

