using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
namespace RestrictionSystem
{
    public delegate void SpellEvent(Side side, bool StartOrFinish);
    public class MagicReactor : SerializedMonoBehaviour
    {
        public static MagicReactor instance;
        private void Awake() { instance = this; }
        public List<CurrentSpell> LastStateSide = new List<CurrentSpell>() { CurrentSpell.Nothing, CurrentSpell.Nothing};
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "FunctionName")] public List<SpellTransition> SpellTransitions = new List<SpellTransition>();
        public static event SpellEvent FireballCast;
        public static event SpellEvent FlamesCast;
        public static event SpellEvent BlockCast;

        public static Dictionary<CurrentSpell, SpellEvent> RestrictionDictionary = new Dictionary<CurrentSpell, SpellEvent>(){
            { CurrentSpell.Fireball, FireballCast },
            { CurrentSpell.Flames, FlamesCast },
            { CurrentSpell.FlameBlock, BlockCast }
        };

        void Start()
        {
            ///subscribe ReactToState to newlearnmanagerAI both sides
            RestrictionManager.instance.NewFrameMotion += ReactToStateChange;
        }
        private void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //FireballCast.Invoke(Side.right, true);
                //events[2](Side.right, true);
                RestrictionDictionary[CurrentSpell.Fireball](Side.right, true);
            }
            */
        }
        public void ReactToStateChange(CurrentSpell newState, Side side)
        {
            //if(side == Side.right)
                //Debug.Log("Past: " + LastStateSide[(int)Side.right] + "  Current: " + newState);
            for (int i = 0; i < SpellTransitions.Count; i++)
            {
                if(SpellTransitions[i].SpellWorks(LastStateSide[(int)side], newState))
                {
                    //Debug.Log("SpellTransitions[i].OutputMotion: " + SpellTransitions[i].OutputMotion.ToString() + "  i: " + i + "  LastStateSide[(int)side]: " + LastStateSide[(int)side] + "  newState: " + newState);
                    //if(side == Side.right)
                        //Debug.Log(SpellTransitions[i].FunctionName);
                    if (SpellTransitions[i].OutputMotion == CurrentSpell.Fireball)
                        FireballCast(side, SpellTransitions[i].OutputState);
                    if (SpellTransitions[i].OutputMotion == CurrentSpell.Flames)
                        FlamesCast(side, SpellTransitions[i].OutputState);
                    if (SpellTransitions[i].OutputMotion == CurrentSpell.FlameBlock)
                        BlockCast(side, SpellTransitions[i].OutputState);
                    //RestrictionDictionary[SpellTransitions[i].OutputMotion](side, SpellTransitions[i].OutputState);
                }
            }

            LastStateSide[(int)side] = newState;
        }

    }
    [System.Serializable]
    public class SpellTransition
    {
        public string FunctionName;
        public SpellList Old;
        public SpellList New;

        public CurrentSpell OutputMotion;
        public bool OutputState;

        public bool SpellWorks(CurrentSpell OldSpell, CurrentSpell NewSpell) { return Old.Contains(OldSpell) && New.Contains(NewSpell); }

        [Serializable]
        public class SpellList
        {
            public bool AllExcept;
            public List<CurrentSpell> List;

            public bool Contains(CurrentSpell spell)
            {
                return AllExcept ? OppositeList(List).Contains(spell) : List.Contains(spell);

                List<CurrentSpell> OppositeList(List<CurrentSpell> CurrentList)
                {
                    List<CurrentSpell> NewList = new List<CurrentSpell>();
                    for (int i = 0; i < Enum.GetValues(typeof(CurrentSpell)).Length; i++)
                        if (!CurrentList.Contains((CurrentSpell)i))
                            NewList.Add((CurrentSpell)i);
                    return NewList;
                }
            }
        }
    }

}

