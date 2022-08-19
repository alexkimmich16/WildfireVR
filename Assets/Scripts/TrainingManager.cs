using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//1 = nothing

public enum TrainingState
{
    None = 0,
    Explaining = 1,
    PlayerTry = 2,
}
public class TrainingManager : MonoBehaviour
{
    public TrainingState state;
    public static string EntryMessage = "Welcome To Training! What Would You Like To Train?";
    public static string StartTrainingMessage = "I'll Show You How To Do A ";
    public static string PlayerTurn = "Now You Try!";

    public static List<string> Successful = new List<string>() { "Thats it!","You Got It!", "Nicely Done!" };

    public static List<string> IndexToName = new List<string>() {"Fireball"};

    public static List<string> FireballCast = new List<string>() {
     "Try To Imagine The Fire Is Contained In The Fist, and your goal is to release it",
     "Move Your hand Fast in a straight line And Bring It To A Jarring Stop",
    };

    public static List<string> FireballEthics = new List<string>() {
     "The FireBall is quite straightforward",
     "Try To Imagine The Fire Is Contained In The Fist",
     "Try To Move Your Hand In A Straight Line",
    };

    public static List<string> FireballErrors = new List<string>() { 
     "Try To Keep Your Hand Level With Your Head",
     "Try To Move A Bit Faster",
     "Try To Move Your Hand In A Straight Line",
    };

    [Header("FadeStats")]
    public float FadeSpeed;
    public float MessageWait;

    [Header("References")]
    public Image Fade;
    public GameObject NextButton;
    public GameObject ChooseTraining;
    public TextMeshProUGUI text;

    private bool FadingIn;
    private bool ActiveCorotine;

    private int CurrentExplain;

    public int CurrentSpell;

    private bool NextActed;

    public Vector2 MinMaxThreshold;
    public void PressNext()
    {
        NextActed = true;
    }
    public IEnumerator ManageAlpha(string Message)
    {
        //1 = faded
        while (true)
        {
            if (FadingIn)
            {
                AddNewAlpha(-Time.deltaTime * FadeSpeed, out float Alpha);
                //Debug.Log("fading " + Alpha);
                if (Alpha < MinMaxThreshold.x)
                {
                    ActiveCorotine = false;
                    yield break;
                }
            }
            else if (FadingIn == false)
            {
                AddNewAlpha(Time.deltaTime * FadeSpeed, out float Alpha);
                if (Alpha > MinMaxThreshold.y)
                {
                    //Debug.Log("fade out " + Alpha);
                    FadingIn = true;
                    text.text = Message;
                }
            }
            yield return null;
        }
    }
    void AddNewAlpha(float Change, out float NewAlpha)
    {
        Color oldColor = Fade.color;
        oldColor.a += Change;
        NewAlpha = oldColor.a;
        Fade.color = oldColor;
    }
    void SetNewAlpha(float New)
    {
        Color oldColor = Fade.color;
        oldColor.a = New;
        Fade.color = oldColor;
    }
    public void DisplayMessage(string Message)
    {
        //Debug.Log("enter");
        if (ActiveCorotine == false)
        {
            //Debug.Log("enter1");
            ActiveCorotine = true;
            FadingIn = false;
            StartCoroutine(ManageAlpha(Message));
        }
        else
        {
            //Debug.Log("enter2");
            SetNewAlpha(1f);
            FadingIn = true;
            text.text = Message;
            StartCoroutine(ManageAlpha(Message));
        }
    }

    public void StartTraining(int Num)
    {
        DisplayMessage(StartTrainingMessage + IndexToName[Num]);
        ChooseTraining.SetActive(false);
        CurrentSpell = Num;
        state = TrainingState.Explaining;
        if (Num == 0)
        {
            StartCoroutine(Explain(FireballCast));
        }
    }
    
    public IEnumerator PlayerTest()
    {
        while (true)
        {
            bool Active = true;
            //on succeed, tell player nice job
            //how to know what is a fail and what is not? 
            //subscribe to 
            if (Active)
            {

            }
            yield return null;
        }
            
    }
    public IEnumerator Explain(List<string> Messages)
    {
        //Debug.Log(Messages[0]);
        while (true)
        {
            if (CurrentExplain == Messages.Count && NextActed == true)
            {
                CurrentExplain = 0;
                DisplayMessage(PlayerTurn);
                state = TrainingState.PlayerTry;
                StartCoroutine(PlayerTest());
                //Start
                yield break;
            }


            if (ActiveCorotine == false && NextActed == true)
            {
                NextActed = false;
                DisplayMessage(Messages[CurrentExplain]);
                CurrentExplain += 1;
            }
            yield return null;
        }
    }
    void Start()
    {
        DisplayMessage(EntryMessage);
        //Color oldColor = Fade.color;
        //oldColor.a = 1f;
        //Fade.color = oldColor;
    }

    private void Update()
    {
        //substitute
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartTraining(0);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            PressNext();
        }
    }
}
