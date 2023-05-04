using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using RestrictionSystem;

namespace TrainingManager
{
    public enum TrainingState
    {
        None = 0,
        PressedMotion = 1,
        Explaining = 2,
        PlayerTry = 3,
    }
    public enum Spell
    {
        Fireball = 0,
        BlueFireball = 1,
        Flames = 2,
        Block = 3,
        Redirect = 4,
        Nothing = 5,
    }
    public class TrainingManager : SerializedMonoBehaviour
    {
        [ShowOdinSerializedPropertiesInInspector]
        public class MagicMotionText
        {
            //[FoldoutGroup("$Motion")] 
            public string Motion;
            //[FoldoutGroup("$Motion"), Multiline(2)] 
            public List<string> Instructions = new List<string>();
            //[FoldoutGroup("$Motion"), Multiline(2)] 
            public List<string> Ethics = new List<string>();
            [OdinSerialize, System.NonSerialized] public Dictionary<string, string> Errors = new Dictionary<string, string>();
            //public Dictionary<string, string> Errors;
        }

        public static TrainingManager instance;
        void Awake() { instance = this; }

        [EnumToggleButtons, FoldoutGroup("Info")] public TrainingState state;
        public int MotionNum;
        public int SideNum;
        //[SerializeField]
        public List<MagicMotionText> Motions = new List<MagicMotionText>();

        [FoldoutGroup("Text")] public string EntryMessage;
        [FoldoutGroup("Text")] public string StartTrainingMessage;
        [FoldoutGroup("Text")] public string PlayerTurn;

        [FoldoutGroup("Text")] public List<string> Successful = new List<string>();

        public MotionSettings motions;

        public Dictionary<Restriction, string> Errors;

        public int CurrentSpell;
        public bool Waiting;
        public List<string> BufferedText = new List<string>();

        [FoldoutGroup("PracticeStats")] public int CurrentSucessfulMotions;
        [FoldoutGroup("PracticeStats")] public int RequiredSucessfulMotions;
        [FoldoutGroup("PracticeStats")] public MinMaxSliderAttribute SucessfulMotionCount;

        public void PressNext()
        {
            Waiting = true;
        }

        public void StartTraining(int Num)
        {
            TextDisplay.instance.DisplayMessage(StartTrainingMessage + Motions[Num].Motion);
            //ChooseTraining.SetActive(false);
            CurrentSpell = Num;
            state = TrainingState.Explaining;
            BufferedText.AddRange(Motions[Num].Instructions);
        }

        public IEnumerator ManageMotionTestSequence()
        {
            //start
            TextDisplay.instance.DisplayMessage(EntryMessage);

            yield return new WaitUntil(() => state == TrainingState.PressedMotion);
            //just pressed motion

            yield return new WaitUntil(() => state == TrainingState.Explaining);


            while (state != TrainingState.Explaining)
            {
                if (Waiting)
                {
                    Waiting = false;
                    TextDisplay.instance.DisplayMessage(BufferedText[0]);
                    BufferedText.RemoveAt(0);

                    if (BufferedText.Count == 0)
                    {
                        TextDisplay.instance.DisplayMessage(PlayerTurn);
                        state = TrainingState.PlayerTry;
                    }
                }

            }

            while (state != TrainingState.PlayerTry)
            {
                ///later add debug wrong motion
                PastFrameRecorder PR = PastFrameRecorder.instance;
                List<Restriction> RestrictionFails = FailedRestrictions(PR.PastFrame((Side)SideNum), PR.GetControllerInfo((Side)SideNum), motions.MotionRestrictions[CurrentSpell]);
                //List<string> ErrorTexts = RestrictionFails.;
                List<string> ErrorTexts = new List<string>();
                for (int i = 0; i < RestrictionFails.Count; i++)
                    ErrorTexts.Add(Errors[RestrictionFails[i]]);

                if (ErrorTexts.Count > 0)
                    TextDisplay.instance.DisplayMessage(ErrorTexts[0]);

                //if (ErrorTexts.Count == 0 && CurrentMotion != (Spell)CurrentSpell) //Wrong but no idea why
                //TextDisplay.instance.DisplayMessage("i've got no idea why this motion is wrong!");


                OnSuccessfulMotion();

                void OnSuccessfulMotion()
                {
                    string Message = Successful[Random.Range(0, Successful.Count - 1)];
                    CurrentSucessfulMotions += 1;
                    if (CurrentSucessfulMotions == RequiredSucessfulMotions)
                    {
                        state = TrainingState.None;
                        TextDisplay.instance.DisplayMessage(EntryMessage);
                        CurrentSucessfulMotions = 0;
                        return;
                    }
                    TextDisplay.instance.DisplayMessage(Message + "  " + (RequiredSucessfulMotions - CurrentSucessfulMotions) + " Left");
                }
            }
            //move to next
        }

        public List<Restriction> FailedRestrictions(SingleInfo frame1, SingleInfo frame2, MotionRestriction restriction)
        {
            List<Restriction> Restrictions = new List<Restriction>();


            /*
            float TotalWeightValue = 0f;
            float TotalWeight = 0f;
            for (int i = 0; i < restriction.Restrictions.Count; i++)
            {
                RestrictionTest RestrictionType = RestrictionDictionary[restriction.Restrictions[i].restriction];
                float RawRestrictionValue = RestrictionType.Invoke(restriction.Restrictions[i], frame1, frame2);
                float RestrictionValue = restriction.Restrictions[i].GetValue(RawRestrictionValue);
                TotalWeightValue += restriction.Restrictions[i].Active ? RestrictionValue * restriction.Restrictions[i].Weight : 0;
                TotalWeight += restriction.Restrictions[i].Active ? restriction.Restrictions[i].Weight : 0;
            }
            float MinWeightThreshold = restriction.WeightedValueThreshold * TotalWeight;
            */
            return Restrictions;

        }
        void Start()
        {

            ManageMotionTestSequence();
        }
        public void Select(int SelectNum, int Value)
        {
            if (SelectNum == 0)
                state = TrainingState.PressedMotion;
            else if (SelectNum == 1)
                state = TrainingState.Explaining;
            MotionNum = SelectNum == 0 ? Value : MotionNum;
            SideNum = SelectNum == 1 ? Value : SideNum;
        }
        private void Update()
        {
            //substitute
            // PlayerTest();


            if (Input.GetKeyDown(KeyCode.S))
                StartTraining(0);

            if (Input.GetKeyDown(KeyCode.N))
                PressNext();

        }
    }
}