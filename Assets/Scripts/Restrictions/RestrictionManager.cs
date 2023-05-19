using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace RestrictionSystem
{
    public enum Restriction
    {
        VelocityThreshold = 0,
        VelocityInDirection = 1,
        HandFacing = 2,
        HandHeadDistance = 3,
        HandToHeadAngle = 4,
        TimedAngleChange = 5,
        AngleChange = 6,
    }
    
    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2,
    }
    public enum CheckType
    {
        Head = 0,
        Hand = 1,
        Other = 2,
    }
    public enum Side
    {
        right = 0,
        left = 1,
    }
    public enum Spell
    {
        Nothing = 0,
        Fireball = 1,
        Flames = 2,
        SideParry = 3,
        UpParry = 4,
    }



    public delegate float RestrictionTest(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2);
    public class RestrictionManager : SerializedMonoBehaviour
    {
        public static RestrictionManager instance;
        private void Awake() { instance = this; }

        public static Dictionary<Restriction, RestrictionTest> RestrictionDictionary = new Dictionary<Restriction, RestrictionTest>(){
            {Restriction.VelocityThreshold, VelocityMagnitude},
            {Restriction.VelocityInDirection, VelocityDirection},
            {Restriction.HandFacing, HandFacing},
            {Restriction.HandHeadDistance, HandHeadDistance},
            {Restriction.HandToHeadAngle, HandToHeadAngle},
            {Restriction.TimedAngleChange, TimedAngleChange},
            {Restriction.AngleChange, AngleChange},
        };
        public MotionSettings RestrictionSettings;
        
        public void TriggerFrameEvents()
        {
            PastFrameRecorder PR = PastFrameRecorder.instance;
            
            for (int i = 0; i < 2; i++)
            {
                Side side = (Side)i;
                if (PR.HandActive(side))
                {
                    for (int j = 1; j < RestrictionSettings.Coefficents.Count + 1; j++)
                    {
                        Spell motion = (Spell)j;

                        FrameLogic.instance.InputRawMotionState(side, motion, MotionWorks(PR.PastFrame(side), PR.GetControllerInfo(side), motion), PR.GetControllerInfo(side).SpawnTime - PR.PastFrame(side).SpawnTime);
                        bool Works = FrameLogic.instance.Calculate(side, motion);

                        //works -> frame logic = actual motion
                        ConditionManager.instance.PassValue(Works, motion, side);

                    }
                }
                
            }
        }
        public bool MotionWorks(SingleInfo frame1, SingleInfo frame2, Spell motionType)
        {
            List<float> TestValues = new List<float>();
            MotionRestriction restriction = RestrictionSettings.MotionRestrictions[(int)motionType - 1];
            for (int i = 0; i < restriction.Restrictions.Count; i++)
            {
                TestValues.Add(RestrictionDictionary[restriction.Restrictions[i].restriction].Invoke(restriction.Restrictions[i], frame1, frame2));
            }

            
            //bool Works = new LogisticRegression().Works(RestrictionSettings.Coefficents[(int)motionType - 1].GetDoubleCoefficents(), TestValues.Select(f => (double)f).ToArray());

            float Total = RestrictionSettings.Coefficents[(int)motionType - 1].Intercept;
            for (int j = 0; j < RestrictionSettings.Coefficents[(int)motionType - 1].Coefficents.Count; j++)//each  variable
                for (int k = 0; k < RestrictionSettings.Coefficents[(int)motionType - 1].Coefficents[j].Degrees.Count; k++)//powers
                    Total += Mathf.Pow(TestValues[j], k + 1) * RestrictionSettings.Coefficents[(int)motionType - 1].Coefficents[j].Degrees[k];

            //insert formula
            float GuessValue = 1f / (1f + Mathf.Exp(-Total));
            bool Guess = GuessValue > 0.5f;
            bool Correct = Guess;
            //Debug.Log("IsCorrect: " + Correct);
            return Correct;
        }
        public static Vector3 EliminateAxis(List<Axis> AllAxis, Vector3 Value) { return new Vector3(AllAxis.Contains(Axis.X) ? Value.x : 0, AllAxis.Contains(Axis.Y) ? Value.y : 0, AllAxis.Contains(Axis.Z) ? Value.z : 0); }
        #region Values
        public static float VelocityMagnitude(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            float Distance = Vector3.Distance(EliminateAxis(restriction.UseAxisList, frame1.HandPosType(restriction.UseLocalHandPos)), EliminateAxis(restriction.UseAxisList, frame2.HandPosType(restriction.UseLocalHandPos)));
            float Speed = Distance / (frame2.SpawnTime - frame1.SpawnTime);
            //restriction.Value = Speed;
            return Speed;
        }
        public static float VelocityDirection(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            Vector3 ForwardInput = restriction.checkType == CheckType.Other ? restriction.OtherDirection : restriction.checkType == CheckType.Head ? frame2.HeadRot : frame2.HandRotType(restriction.UseLocalHandRot);
            Vector3 forwardDir = EliminateAxis(restriction.UseAxisList, Quaternion.Euler(ForwardInput + restriction.Offset) * restriction.Direction);

            float AngleDistance = Vector3.Angle(EliminateAxis(restriction.UseAxisList, (frame2.HandPosType(restriction.UseLocalHandPos) - frame1.HandPosType(restriction.UseLocalHandPos)).normalized), forwardDir);
            //restriction.Value = AngleDistance;
            return AngleDistance;
        }
        public static float HandFacing(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            Vector3 HandDir = EliminateAxis(restriction.UseAxisList, (Quaternion.Euler(frame2.HandRotType(restriction.UseLocalHandRot) + restriction.Offset) * restriction.Direction));
            Vector3 OtherDir = EliminateAxis(restriction.UseAxisList, restriction.checkType == CheckType.Other ? restriction.OtherDirection : -(frame2.HandPosType(restriction.UseLocalHandPos)).normalized);
            //restriction.Value = Vector3.Angle(HandDir, OtherDir);
            return Vector3.Angle(HandDir, OtherDir);
        }
        public static float HandHeadDistance(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            Vector3 HeadPos = EliminateAxis(restriction.UseAxisList, frame2.HeadPos);
            Vector3 HandPos = EliminateAxis(restriction.UseAxisList, frame2.HandPos);
            //restriction.Value = Vector3.Distance(HeadPos, HandPos);
            return Vector3.Distance(HeadPos, HandPos);
        }
        public static float HandToHeadAngle(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            List<Axis> YGone = new List<Axis>() { Axis.X, Axis.Z };
            Vector3 targetDir = EliminateAxis(YGone, frame2.HandPos).normalized;
            //Bug.Log(targetDir);

            Quaternion quat = Quaternion.Euler(EliminateAxis(YGone, frame2.HeadPos));//inside always 0
            Vector3 forwardDir = (quat * Vector3.forward).normalized;

            float Angle = frame2.HeadRot.y + Vector3.SignedAngle(targetDir, forwardDir, Vector3.up) + 180f;
            if (Angle > 360f || Angle < -360f)
                Angle += Angle > 360f ? -360f : 360f;
            //restriction.Value = Angle;
            return Angle;
            
        }
        public static float TimedAngleChange(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            Vector3 OldDir = EliminateAxis(restriction.UseAxisList, frame1.HandPosType(restriction.UseLocalHandPos).normalized);
            Vector3 NewDir = EliminateAxis(restriction.UseAxisList, frame2.HandPosType(restriction.UseLocalHandPos).normalized);
            //restriction.Value = Vector3.Angle(HandDir, OtherDir);
            return Vector3.Angle(OldDir, NewDir) / (frame2.SpawnTime - frame1.SpawnTime);
        }
        public static float AngleChange(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            Vector3 OldDir = EliminateAxis(restriction.UseAxisList, frame1.HandPosType(restriction.UseLocalHandPos).normalized);
            Vector3 NewDir = EliminateAxis(restriction.UseAxisList, frame2.HandPosType(restriction.UseLocalHandPos).normalized);
            //restriction.Value = Vector3.Angle(HandDir, OtherDir);
            return Vector3.Angle(OldDir, NewDir);
        }
        #endregion
    }
}

