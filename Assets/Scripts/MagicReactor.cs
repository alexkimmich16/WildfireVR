using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace RestrictionSystem
{
    public delegate void SpellEvent(Side side, bool StartOrFinish);
    public class MagicReactor : SerializedMonoBehaviour
    {
        public static MagicReactor instance;
        private void Awake() { instance = this; }
        public List<CurrentSpell> LastStateSide = new List<CurrentSpell>() { CurrentSpell.Nothing, CurrentSpell.Nothing};
        [ListDrawerSettings(DraggableItems = false, ShowIndexLabels = true, ListElementLabelName = "FunctionName")] public List<SpellTransition> SpellTransitions = new List<SpellTransition>();
        public static event SpellEvent FireballCast;
        public static event SpellEvent FlamesCast;
        public static event SpellEvent BlockCast;

        public Dictionary<CurrentSpell, SpellEvent> RestrictionDictionary = new Dictionary<CurrentSpell, SpellEvent>(){
            { CurrentSpell.Fireball, FireballCast },
            { CurrentSpell.Flames, FlamesCast },
            { CurrentSpell.FlameBlock, BlockCast }
        };
        void Start()
        {
            ///subscribe ReactToState to newlearnmanagerAI both sides
            RestrictionManager.instance.NewFrameMotion += ReactToState;
        }

        public void ReactToState(CurrentSpell newState, Side side)
        {
            for (int i = 0; i < SpellTransitions.Count; i++)
            {
                if(SpellTransitions[i].SpellWorks(LastStateSide[(int)side], newState))
                {
                    RestrictionDictionary[SpellTransitions[i].OutputMotion].Invoke(side, SpellTransitions[i].OutputState);
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

        [System.Serializable]
        public class SpellList
        {
            
            public bool AllExcept;
            public List<CurrentSpell> List;

            public bool Contains(CurrentSpell spell)
            {
                List<CurrentSpell> RealList = AllExcept ? List : OppositeList(List);
                return AllExcept ? List.Contains(spell) : OppositeList(List).Contains(spell);

                List<CurrentSpell> OppositeList(List<CurrentSpell> CurrentList)
                {
                    List<CurrentSpell> NewList = new List<CurrentSpell>();
                    for (int i = 0; i < 4; i++)
                        if (!CurrentList.Contains((CurrentSpell)i))
                            NewList.Add((CurrentSpell)i);
                    return NewList;
                }
            }
        }
    }

}

