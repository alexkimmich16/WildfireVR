using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using UnityEngine.UI;
using Photon.Voice.Unity;
using Sirenix.OdinInspector;
public enum SoundType
{
    Elevator = 0,
    Effect = 1,
    Crowd = 2,
    Voice = 3,
}
/*
[System.Serializable]
public class Audio
{
    public string Name;
    public AudioClip Sound;
    public SoundTypes type;

    [Range(0.0f, 1f)]
    public float Volume = 1;
}
*/

public class SoundManager : SerializedMonoBehaviour
{
    #region Singleton + classes
    public static SoundManager instance;
    void Awake() { instance = this; }

    #endregion
    [FoldoutGroup("Enable")]
    public bool EnableSounds, EnableEffectSounds, EnableElevatorSounds, EnableCrowdSounds, EnableVoice;


    [FoldoutGroup("References")] private FMOD.Studio.EventInstance CrowdInstance, ElevatorInstance;
    [FoldoutGroup("References")] public FMODUnity.EventReference CrowdRef, ElevatorRef, FireballRef, FlamesRef, BlockRef;
    [FoldoutGroup("References")] public FMODUnity.EventReference PlayerLeftRef;
    [FoldoutGroup("References")] public List<FMODUnity.EventReference> DamageRef;
    [FoldoutGroup("References")] public List<FMODUnity.EventReference> DeathRef;
    
    //public List<AudioClip> CrowdShouts;
    //public Slider MasterSlider, EffectSlider, ElevatorSlider, VoiceSlider;

    //public Recorder recorder;
    [Range(0f, 1f), FoldoutGroup("Volume")] public float MasterVolume, EffectVolume, ElevatorVolume, CrowdVolume, VoiceVolume;

    //public static float PercentToDecibles(float Percent) { return Mathf.Lerp(0f, -80f, Percent / 200f); }

    public FMODUnity.EventReference RandomSound(List<FMODUnity.EventReference> SoundList) { return SoundList[Random.Range(0, SoundList.Count)]; }

    public bool CanPlaySound(SoundType type)
    {
        if (type == SoundType.Elevator)
            return EnableSounds && EnableElevatorSounds;
        if (type == SoundType.Effect)
            return EnableSounds && EnableEffectSounds;
        if (type == SoundType.Crowd)
            return EnableSounds && EnableCrowdSounds;
        if (type == SoundType.Voice)
            return EnableSounds && EnableVoice;
        return true;
    }
    private void Update()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("EffectVolume", EffectVolume);
        PlayerPrefs.SetFloat("ElevatorVolume", ElevatorVolume);
        PlayerPrefs.SetFloat("CrowdVolume", CrowdVolume);
        PlayerPrefs.SetFloat("VoiceVolume", VoiceVolume);

        ElevatorInstance.setVolume(ElevatorVolume * MasterVolume);
    }
    private void OnInitialize()
    {
        if (CanPlaySound(SoundType.Elevator))
        {
            ElevatorInstance = FMODUnity.RuntimeManager.CreateInstance(ElevatorRef);
            ElevatorInstance.setVolume(ElevatorVolume);
            ElevatorInstance.start();
        }
        if (CanPlaySound(SoundType.Effect))
        {
            //DoorManager.OnDoorChange += ElevatorSound;v
            //PlayerControl.OnTakeDamage += OnPlayerHit;
        }
        if (CanPlaySound(SoundType.Crowd))
        {

        }
        if (CanPlaySound(SoundType.Voice))
        {
            //recorder.(volume);
        }
        //ElevatorSlider.onValueChanged. += OnElevatorVolumeChanged;
    }
    public void SetDoorAudio(int State)
    {
        ElevatorInstance.setParameterByName("ElevatorState", State);
    }
    private void Start()
    {
        NetworkManager.OnInitialized += OnInitialize;

        MasterVolume = PlayerPrefs.GetFloat("MasterVolume");
        EffectVolume = PlayerPrefs.GetFloat("EffectVolume");
        ElevatorVolume = PlayerPrefs.GetFloat("ElevatorVolume");
        CrowdVolume = PlayerPrefs.GetFloat("CrowdVolume");
        VoiceVolume = PlayerPrefs.GetFloat("VoiceVolume");

        

        //NetworkManager.Initialize += OnPlayerLeave();
        //NetworkManager.Initialize += OnPlayerDeath();
    }
    public void OnPlayerDeath()
    {
        //Roar
    }

    public void OnPlayerHit()
    {
        //RoarOBJ.GetComponent<AudioSource>().Play();
        //TimeAfterRoar = 0f;
    }

    public void OnPlayerLeave()
    {
        ///booooooo
    }
}
