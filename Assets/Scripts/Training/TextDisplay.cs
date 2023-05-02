using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;


namespace TrainingManager
{
    public enum FadeState
    {
        FadingIn = 0,
        FadingOut = 1,
        Nothing = 2,

    }
    public class TextDisplay : SerializedMonoBehaviour
    {
        public static TextDisplay instance;
        void Awake() { instance = this; }

        public Image Fade;
        public GameObject NextButton;
        public GameObject ChooseTraining;
        public TextMeshProUGUI text;

        public int Recompile;

        //private bool FadingIn;
        private bool ActiveCorotine;

        [Header("FadeStats")]
        public float FadeSpeed;
        public float MessageWait;

        public FadeState fadeState;

        public Vector2 MinMaxThreshold;


        //public delegate void FinishDisplay();
        //public static event FinishDisplay disolveEvent;

        public IEnumerator ManageAlpha(string Message)
        {
            //1 = faded
            while (fadeState == FadeState.FadingIn || fadeState == FadeState.FadingOut)
            {
                Fade.color = new Color(Fade.color.r, Fade.color.g, Fade.color.b, Fade.color.a + (-Time.deltaTime * FadeSpeed * ((fadeState == FadeState.FadingIn) ? 1f : -1f))); // add or subtract alpha 
                if (fadeState == FadeState.FadingIn && Fade.color.a < MinMaxThreshold.x)
                {
                    ActiveCorotine = false;
                    yield break;
                }
                else if (fadeState == FadeState.FadingOut && Fade.color.a > MinMaxThreshold.y)
                {
                    fadeState = FadeState.FadingIn;
                    text.text = Message;
                }
                yield return null;
            }
        }
        public void DisplayMessage(string Message)
        {
            //Debug.Log("enter");
            fadeState = FadeState.FadingIn;
            Fade.color = new Color(Fade.color.r, Fade.color.g, Fade.color.b, 1f); // transparent
            text.text = Message;
            StartCoroutine(ManageAlpha(Message));
        }
    }
}

