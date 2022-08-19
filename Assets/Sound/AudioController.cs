using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioController : MonoBehaviour
{
    public static int SingletonNum = 2;
    #region Singleton
    public static AudioController instance;
    void Awake()
    {
        instance = this;
    }
    [System.Serializable]
    public class AudioType
    {
        public string Type;
        public AudioMixer audioMixer;
        public Slider slider;
        public TextMeshProUGUI text;
        [HideInInspector] public int Percent;
        [HideInInspector] public float TrueVolume;
    }

    #endregion
    public List<AudioType> AudioMixers = new List<AudioType>();

    public void ChangeSlider(int Percent, int Slider)
    {
        AudioMixers[Slider].slider.value = Percent;

    }

    public void OnValueChanged(int Num)
    {
        AudioType A = AudioMixers[Num];
        A.Percent = (int)((A.slider.value + 80) / 160) * 100;
        A.text.text = A.Type + ":  " + A.Percent + "%";

        A.TrueVolume = A.slider.value;
        A.audioMixer.SetFloat("Volume", A.TrueVolume);
        PlayerPrefs.SetFloat(AudioMixers[Num].Type, A.TrueVolume);
    }
    void Start()
    {
        for (int i = 0; i < AudioMixers.Count; i++)
        {
            AudioMixers[i].audioMixer.SetFloat("Volume", PlayerPrefs.GetFloat(AudioMixers[i].Type));
            AudioMixers[i].slider.value = PlayerPrefs.GetFloat(AudioMixers[i].Type);
        }
    }
}
