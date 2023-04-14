using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
namespace RestrictionSystem
{
    public enum Condition
    {
        Time = 0,
        Distance = 1,
        Restriction = 2,
        ConsecutiveFrames = 4,
    }
    public delegate bool ConditionWorksAndAdd(SingleConditionInfo Condition, SingleInfo CurrentFrame, SingleInfo PastFrame, bool NewState, Side side);
    public delegate void OnNewMotionState(Side side, bool NewState, int Index, int Level);

    public class ConditionManager : SerializedMonoBehaviour
    {
        public static ConditionManager instance;
        private void Awake() { instance = this; }
        public Conditions conditions;

        //[TableMatrix(DrawElementMethod = "DrawElement")]
        //[TableMatrix(HorizontalTitle = "Condition Stats")]
        private ConditionProgress[,] ConditionStats = new ConditionProgress[2, 0];

        [System.Serializable]
        public class ConditionProgress//represents all sequences within motion
        {
            public bool Active() { return StartInfo != null; }
            public int SequenceState;
            public SingleInfo StartInfo;

            public void Reset()
            {
                SequenceState = 0;
                StartInfo = null;
            }
        }

        private void Start()
        {
            ConditionStats = new ConditionProgress[2, conditions.MotionConditions.Count];
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < conditions.MotionConditions.Count; j++)
                    ConditionStats[i, j] = new ConditionProgress();
        }

        public static Dictionary<Condition, RestrictionTest> ConditionDictionary = new Dictionary<Condition, RestrictionTest>(){
            {Condition.Time, Time},
            {Condition.Distance, Distance},
            {Condition.Restriction, RestrictionWorksAndAdd},
            {Condition.ConsecutiveFrames, ConsecutiveWorksAndAdd},
        };
        public static float Time(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2) { return frame2.SpawnTime - frame1.SpawnTime; }

        public static float Distance(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2) { return Vector3.Distance(frame1.HandPos, frame2.HandPos); }
        public static float RestrictionWorksAndAdd(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2) { return RestrictionManager.RestrictionDictionary[restriction.restriction].Invoke(restriction, frame2, frame1); }
        public static float ConsecutiveWorksAndAdd(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2) { return 1f; }
        public void PassValue(bool State, CurrentLearn Motion, Side side)
        {
            ConditionProgress Holder = ConditionStats[(int)side, (int)Motion - 1];
            MotionConditionInfo Condition = conditions.MotionConditions[(int)Motion - 1];
            if (State != Holder.Active())
            {
                if(Condition.conditionType == ConditionType.Prohibit)
                {
                    Condition.DoEvent(side, State, 0, Condition.CastLevel);
                    Holder.StartInfo = State ? PastFrameRecorder.instance.GetControllerInfo(side) : null;
                }

                if (Condition.conditionType == ConditionType.Sequence)
                {
                    if (State == true)
                    {
                        //Debug.Log("Create");
                        Holder.StartInfo = PastFrameRecorder.instance.GetControllerInfo(side);
                    }
                    else
                    {
                        SequenceReset();
                    }
                    
                }
            }


            //if(Motion == CurrentLearn.Fireball)
                //Debug.Log("Motion: " + Motion + " State: " + Holder.Active() + "  State: " + State);
            if (Holder.Active() && Condition.conditionType == ConditionType.Sequence)
            {
                //Debug.Log("IsCounting: " + Motion);
                bool Works = RestrictionManager.instance.TestCondition(Condition.ConditionLists[Holder.SequenceState], Holder.StartInfo, PastFrameRecorder.instance.GetControllerInfo(side));
                if (Works)
                {
                    Condition.DoEvent(side, true, Holder.SequenceState, Condition.CastLevel);
                    Holder.SequenceState += 1;
                    //Debug.Log("next, now is: " + Holder.SequenceState);
                    if (Holder.SequenceState > Condition.ConditionLists.Count - 1)
                    {
                        SequenceReset();
                    }
                }
            }

            ConditionStats[(int)side, (int)Motion - 1] = Holder;
            void SequenceReset()
            {
               // Debug.Log("Reset");
                Holder.Reset();
                for (int i = 0; i < Condition.ConditionLists.Count; i++)
                {
                    Condition.DoEvent(side, false, i, Condition.CastLevel);
                }
            }

        }
    }
    public enum ConditionType
    {
        Sequence = 0,
        Prohibit = 1,
    }
    [Serializable]
    public class SingleSequenceState
    {
        public string StateToActivate;
        public bool RegressionBased;
        public double[] Coefficents;
        public float CutoffValue;
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "Label")] public List<SingleConditionInfo> SingleConditions;
    }
    [Serializable]
    public class MotionConditionInfo
    {
        public string Motion;
        public ConditionType conditionType;
        public static bool ShowRuntime = false;
        public bool ResetOnMax;
        public int CastLevel;
        

        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "StateToActivate"), ShowIf("conditionType", ConditionType.Sequence)] public List<SingleSequenceState> ConditionLists;
        public event OnNewMotionState OnNewState;
        public void DoEvent(Side side, bool Newstate, int Index, int Level) { OnNewState?.Invoke(side, Newstate, Index, Level); }
        /*
        public void PassValueToAll(bool State, Side side)
        {
            if ((State == false || WaitingForFalse[(int)side] == true) && conditionType == ConditionType.Sequence)
            {
                if (State == false)
                {
                    ResetAll(side);
                }

                //Debug.Log("trigger1");
                return;
            }
            if(conditionType == ConditionType.Prohibit)
            {
                //if wait conditions are met, wait for them
                //if(Motion == "flames")
                    //Debug.Log("prohibit: " + Motion + "  State: " + State);
                bool AllWorkingSoFar = true;
                if (ProhibitList.Count != 0)
                    for (int i = 0; i < ProhibitList.Count; i++)
                    {
                        SingleConditionInfo info = ProhibitList[i];
                        ConditionWorksAndAdd WorkingConditionAndUpdate = ConditionManager.ConditionDictionary[info.condition];

                        bool Working = WorkingConditionAndUpdate.Invoke(info, PastFrameRecorder.instance.GetControllerInfo(side), PastFrameRecorder.instance.PastFrame(side), State, side); ///velocity error here

                        if (Working == false)
                            AllWorkingSoFar = false;

                        info.LastState[(int)side] = State;
                        //Debug.Log("once: " + Working);
                    }

                if (WaitingForFalse[(int)side] != State && AllWorkingSoFar)
                {
                    OnNewState?.Invoke(side, State, 0, CastLevel);
                    WaitingForFalse[(int)side] = State;
                }
            }
            else if(conditionType == ConditionType.Sequence)
            {
                bool AllWorkingSoFar = true;
                if (ConditionLists.Count != 0)
                {
                    for (int i = 0; i < ConditionLists[CurrentStage[(int)side]].SingleConditions.Count; i++)
                    {
                        SingleConditionInfo info = ConditionLists[CurrentStage[(int)side]].SingleConditions[i];
                        ConditionWorksAndAdd WorkingConditionAndUpdate = ConditionManager.ConditionDictionary[info.condition];

                        //SingleInfo CurrentFrame = MotionEditor.instance.display.GetFrameInfo(false);
                        //SingleInfo PastFrame = MotionEditor.instance.display.GetFrameInfo(true);

                        bool Working = WorkingConditionAndUpdate.Invoke(info, PastFrameRecorder.instance.GetControllerInfo(side), PastFrameRecorder.instance.PastFrame(side), State, side); ///velocity error here
                        //bool Working = WorkingConditionAndUpdate.Invoke(info, MotionEditor.instance.display.GetFrameInfo(false), MotionEditor.instance.display.GetFrameInfo(true), State, side); ///velocity error here
                        if (Working == false)
                            AllWorkingSoFar = false;
                        info.LastState[(int)side] = State;
                        //Debug.Log("once: " + Working);
                    }
                }


                if (AllWorkingSoFar) //ready to move to next
                {
                    OnNewState?.Invoke(side, true, CurrentStage[(int)side], CastLevel);
                    //Debug.Log(CurrentStage + " < " + (ConditionLists.Count - 1));
                    if (CurrentStage[(int)side] < ConditionLists.Count - 1)
                    {
                        CurrentStage[(int)side] += 1;
                    }
                    else
                    {
                        ResetAll(side);
                    }
                }
            }
            
            
        }
        */
    }
    [Serializable]
    public class SingleConditionInfo
    {
        /// <summary>
        /// tasked with tracking variables and patterns over time
        /// </summary>
        
        
        public string Label;
        //public bool Active;
        public Condition condition;
        //public double[] Coefficents;
        [ShowIf("condition", Condition.ConsecutiveFrames)] public bool WaitSide;
        [ShowIf("condition", Condition.Restriction)] public SingleRestriction restriction;
        ///reset on false?
        ///
        //private static bool ShowValues = true;

        //[FoldoutGroup("Values"), ShowIf("ShowValues")] public List<bool> LastState = new List<bool>() { false, false };
        //[FoldoutGroup("Values"), ShowIf("ShowValues")] public List<float> StartTime = new List<float>() { 0f, 0f };
        //[FoldoutGroup("Values"), ShowIf("ShowValues")] public List<Vector3> StartPos = new List<Vector3>() { Vector3.zero, Vector3.zero };
        //[FoldoutGroup("Values"), ShowIf("ShowValues")] public List<float> Value = new List<float>() { 0, 0 };
    }
}