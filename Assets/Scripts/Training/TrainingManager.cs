using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public enum TrainingState
{
    None = 0,
    Explaining = 1,
    PlayerTry = 2,
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

    [EnumToggleButtons] public TrainingState state;

    //[SerializeField]
    public List<MagicMotionText> Motions = new List<MagicMotionText>();

    [FoldoutGroup("Main")] public string EntryMessage;
    [FoldoutGroup("Main")] public string StartTrainingMessage;
    [FoldoutGroup("Main")] public string PlayerTurn;

    [FoldoutGroup("Main")] public List<string> Successful = new List<string>();

    //[FoldoutGroup("Main")] public List<string> IndexToName = new List<string>();


    [FoldoutGroup("Delete")] public List<string> FireballCast = new List<string>();
    [FoldoutGroup("Delete")] public Dictionary<string, string> NewFireballErrors;

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

    
    
    public void PlayerTest()
    {
        if (state != TrainingState.PlayerTry)
            return;
        
        ///when to update??
        ///on AI changed motion
        ///on speed stop

        Spell CurrentMotion = Spell.Fireball; ///get spell from controller(currentplaceholder), also will repeat so fix that
        if (IsAnotherMotion(CurrentMotion)) //Another Motion
        {
            TextDisplay.instance.DisplayMessage("try preforming the correct motion: " + ((Spell)CurrentSpell).ToString() + "  and not: " + CurrentMotion.ToString());
            return;
        }

        //Correct Motion Errors
        Dictionary<string, bool> ErrorChecks = DetectMotionErrors();
        List<string> ErrorTexts = new List<string>();
        foreach (KeyValuePair<string, bool> pair in ErrorChecks)
        {
            if(pair.Value == false)
            {
                ErrorTexts.Add(Motions[CurrentSpell].Errors[pair.Key]);
            }
        }

        if(ErrorTexts.Count > 0)
        {
            ///should debug ALL errors in future
            TextDisplay.instance.DisplayMessage(ErrorTexts[0]);
            return;
        }

        if(ErrorTexts.Count == 0 && CurrentMotion != (Spell)CurrentSpell) //Wrong but no idea why
        {
            TextDisplay.instance.DisplayMessage("i've got no idea why this motion is wrong!");
            return;
        }
        

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
        bool IsAnotherMotion(Spell PreformedSpell)
        {
            return CurrentMotion != (Spell)CurrentSpell && CurrentMotion != Spell.Nothing;
        }
        Dictionary<string, bool> DetectMotionErrors()
        {
            Dictionary<string, bool> CurrentErrors = new Dictionary<string, bool>();
            //CurrentErrors.Add(false);

            if ((Spell)CurrentSpell == Spell.Fireball)
            {
                CurrentErrors.Add("Crooked", Crooked());
                CurrentErrors.Add("NotParrelelHand", NotParrelelHand());
                CurrentErrors.Add("Slow", Slow());

                bool Crooked()
                {
                    return false;
                }
                bool NotParrelelHand()
                {
                    return false;
                }
                bool Slow()
                {
                    return false;
                }
            }

            return CurrentErrors;
        }
    }
    public void Explain()
    {
        if (state != TrainingState.Explaining || !Waiting)
            return;

        //move to next
        Waiting = false;
        TextDisplay.instance.DisplayMessage(BufferedText[0]);
        BufferedText.RemoveAt(0);

        if (BufferedText.Count == 0)
        {
            TextDisplay.instance.DisplayMessage(PlayerTurn);
            state = TrainingState.PlayerTry;
        }
        //Debug.Log("debug");
    }
    void Start()
    {
        TextDisplay.instance.DisplayMessage(EntryMessage);
    }
    
    private void Update()
    {
        //substitute
        Explain();
        PlayerTest();
        

        if (Input.GetKeyDown(KeyCode.S))
            StartTraining(0);

        if (Input.GetKeyDown(KeyCode.N))
            PressNext();
        
    }
}
