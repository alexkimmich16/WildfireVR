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
        [HideInInspector] public float Float;
        [HideInInspector] public int Percent;
        [HideInInspector] public int Change;
    }

    #endregion
    public List<AudioType> AudioMixers = new List<AudioType>();

    // Update is called once per frame
    void Update()
    {
        //effects
        for(int i = 0; i < AudioMixers.Count; i++)
        {
            AudioType A = AudioMixers[i];
            A.Float = A.slider.value * 100;
            A.Percent = (int)A.Float / 100;
            string VolumeText = A.Type + ":  "+ A.Percent + "%";
            A.text.text = VolumeText;
            int VolumeChangePT = -(A.Percent * 8 / 10) + 160;
            A.Change = 80 - VolumeChangePT;
            A.audioMixer.SetFloat("Volume", A.Change);
        }
    }

    public void ChangeSlider(int Percent, int Slider)
    {
        AudioMixers[Slider].slider.value = Percent;
    }
    void Start()
    {
        //SoundSlider.value = 100;
        //MusicSlider.value = 100;

        for (int i = 0; i < AudioMixers.Count; i++)
        {
            AudioMixers[i].audioMixer.SetFloat("Volume", AudioMixers[i].Change);
            AudioMixers[i].slider.value = 100;
        }

    }
}
