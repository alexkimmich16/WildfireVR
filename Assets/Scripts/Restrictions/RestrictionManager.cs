using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
public enum CurrentSpell
{
    Nothing = 0,
    Fireball = 1,
    Flames = 2,
    FlameBlock = 3,
}

namespace RestrictionSystem
{
    public enum Restriction
    {
        VelocityThreshold = 0,
        VelocityInDirection = 1,
        HandFacingHead = 2,
        HandHeadDistance = 3,
        HandToHeadAngle = 4,
    }
    
    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2,
    }
    public enum VelocityType
    {
        Head = 0,
        Hand = 1,
    }
    public enum Side
    {
        right = 0,
        left = 1,
    }
    
    public delegate float MotionTest(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2);
    public class RestrictionManager : SerializedMonoBehaviour
    {
        public static RestrictionManager instance;
        private void Awake() { instance = this; }

        public static Dictionary<Restriction, MotionTest> RestrictionDictionary = new Dictionary<Restriction, MotionTest>(){
            {Restriction.VelocityThreshold, VelocityThresholdWorks},
            {Restriction.VelocityInDirection, VelocityInDirectionWorks},
            {Restriction.HandFacingHead, HandFacingHeadWorks},
            {Restriction.HandHeadDistance, HandHeadDistanceWorks},
            {Restriction.HandToHeadAngle, HandToHeadAngleWorks},
        };

        public delegate void OutputMotion(CurrentSpell motion, Side side);
        public event OutputMotion NewFrameMotion;

        
        public void TriggerFrameEvents()
        {
            PastFrameRecorder PR = PastFrameRecorder.instance;
            for (int i = 0; i < 2; i++)
                if (NewFrameMotion != null)
                    NewFrameMotion(GetCurrentMotion(PR.PastFrame((Side)i), PastFrameRecorder.instance.GetControllerInfo((Side)i)), (Side)i);
            
        }
        //[ListDrawerSettings(DraggableItems = false, ShowIndexLabels = true, ListElementLabelName = "Title")] public List<MotionRestriction> MotionRestrictions;
        public MotionSettings RestrictionSettings;
        public bool MotionWorks(SingleInfo frame1, SingleInfo frame2, MotionRestriction restriction)
        {
            float TotalWeightValue = 0f;
            float TotalWeight = 0f;
            //Debug.Log(restriction.Restrictions.Count);
            for (int i = 0; i < restriction.Restrictions.Count; i++)
            {
                //Debug.Log(i);
                MotionTest RestrictionType = RestrictionDictionary[restriction.Restrictions[i].restriction];
                float RestrictionWorks = RestrictionType.Invoke(restriction.Restrictions[i], frame1, frame2);
                TotalWeightValue += restriction.Restrictions[i].Active ? RestrictionWorks * restriction.Restrictions[i].Weight : 0;
                TotalWeight += restriction.Restrictions[i].Active ? restriction.Restrictions[i].Weight : 0;
            }
            float MinWeightThreshold = restriction.WeightedValueThreshold * TotalWeight;
            return TotalWeightValue >= MinWeightThreshold;

        }
        public CurrentSpell GetCurrentMotion(SingleInfo frame1, SingleInfo frame2)
        {
            //ask for others
            //if nothing == nothing
            List<bool> AllWorks = new List<bool>();
            
            for (int i = 0; i < RestrictionSettings.MotionRestrictions.Count; i++) //check each motion (3)
            {
                AllWorks.Add(MotionWorks(frame1, frame2, RestrictionSettings.MotionRestrictions[i]));
            }

            List<int> WorkingList = new List<int>();
            for (int i = 0; i < AllWorks.Count; i++)
                if (AllWorks[i] == true)
                    WorkingList.Add(i + 1);

            if (WorkingList.Count == 1)
                return (CurrentSpell)WorkingList[0];
            else if(WorkingList.Count > 1)
            {
                string ErrorString = "Conflict between: ";
                for (int i = 0; i < WorkingList.Count; i++)
                {
                    ErrorString = ErrorString + ((CurrentSpell)WorkingList[i]).ToString() + ", ";
                }
                Debug.LogError(ErrorString);
                //return CurrentSpell.Flames;
            }

            return CurrentSpell.Nothing;
        }

        #region bools
        public static float VelocityThresholdWorks(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            float Distance = Vector3.Distance(frame1.HandPos, frame2.HandPos);
            float Speed = Distance / (1f / 60f);
            return restriction.GetValue(Speed);

        }
        public static float VelocityInDirectionWorks(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            Vector3 VelocityDirection = (frame2.HandPos - frame1.HandPos).normalized;

            Vector3 ForwardInput = restriction.CheckType == VelocityType.Head ? frame2.HeadRot : frame2.HandRot;
            Vector3 forwardDir = (Quaternion.Euler(ForwardInput + restriction.Offset) * restriction.Direction);
            //if(restriction.ShouldDebug)
                //Debug.DrawLine(frame2.HandPos, frame2.HandPos + (forwardDir * DebugRestrictions.instance.LineLength), restriction.CheckType == VelocityType.Head ? Color.yellow : Color.red);

            float AngleDistance = Vector3.Angle(VelocityDirection, forwardDir);

            return restriction.GetValue(AngleDistance);
        }
        public static float HandFacingHeadWorks(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            Vector3 HandDir = (Quaternion.Euler(frame2.HandRot + restriction.Offset) * restriction.Direction);
            Vector3 HandToHeadDir = (-frame2.HandPos).normalized;

            if (restriction.ExcludeHeight)
            {
                HandDir.y = 0f;
                HandToHeadDir.y = 0f;
            }
            /*
            if (restriction.ShouldDebug)
            {
                Debug.DrawLine(frame2.HandPos, frame2.HandPos + (HandToHeadDir * DebugRestrictions.instance.LineLength), Color.yellow);
                Debug.DrawLine(frame2.HandPos, frame2.HandPos + (HandDir * DebugRestrictions.instance.LineLength), Color.red);
            }
             */   

            return restriction.GetValue(Vector3.Angle(HandDir, HandToHeadDir));
        }
        public static float HandHeadDistanceWorks(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            Vector3 HeadPos = frame2.HeadPos;
            Vector3 HandPos = frame2.HandPos;
            if (!restriction.UseAxisList.Contains(Axis.X))//doesn't contain
            {
                HeadPos = new Vector3(0, HeadPos.y, HeadPos.z);
                HandPos = new Vector3(0, HandPos.y, HandPos.z);
            }
            if (!restriction.UseAxisList.Contains(Axis.Y))//doesn't contain
            {
                HeadPos = new Vector3(HeadPos.x, 0, HeadPos.z);
                HandPos = new Vector3(HandPos.x, 0, HandPos.z);
            }
            if (!restriction.UseAxisList.Contains(Axis.Z))//doesn't contain
            {
                HeadPos = new Vector3(HeadPos.x, HeadPos.y, 0);
                HandPos = new Vector3(HandPos.x, HandPos.y, 0);
            }
            float Distance = Vector3.Distance(HeadPos, HandPos);
            return restriction.GetValue(Distance);

        }
        public static float HandToHeadAngleWorks(SingleRestriction restriction, SingleInfo frame1, SingleInfo frame2)
        {
            Vector3 targetDir = new Vector3(frame2.HandPos.x, 0, frame2.HandPos.z).normalized;
            Quaternion quat = Quaternion.Euler(new Vector3(frame2.HeadPos.x, 0, frame2.HeadPos.z));
            Vector3 forwardDir = (quat * Vector3.forward).normalized;
            float Angle = frame2.HeadRot.y + Vector3.SignedAngle(targetDir, forwardDir, Vector3.up) + 180f;
            //Offset
            if (Angle > 360 || Angle < -360)
                Angle += Angle > 360 ? -360 : 360;

            return restriction.GetValue(Angle);
            
        }
        #endregion
    }

    [Serializable]
    public class SingleCondition
    {
        public Condition condition;

        public enum Condition
        {
            Time = 0,
            Distance = 1,
            Position = 2,
        }
    }
    /*
    public bool GripPressed()
    {
        if (Grip > HandMagic.instance.GripThreshold)
        {
            return true;
        }
        return false;
    }
    InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
    public XRNode inputSource;
        device.TryGetFeatureValue(CommonUsages.trigger, out Trigger);
    */
}

