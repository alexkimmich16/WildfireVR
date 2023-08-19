using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using static Odin.Net;
using System.Linq;
public enum SoundType
{
    Master = 0,
    Effect = 1,
    Crowd = 2,
    Voice = 3,
}
public class SoundManager : SerializedMonoBehaviour
{
    #region Singleton + classes
    public static SoundManager instance;
    void Awake() { instance = this; }

    //
    [System.Serializable]
    public class AudioClassInfo
    {    
        [Range(0.0f, 1f)] public float Volume = 1;
    }
    [System.Serializable]
    public class SoundEffect
    {
        public string ID;
        public SoundType type;
        public FMODUnity.EventReference Ref;
        [Range(0.0f, 1f)] public float Volume = 1;
    }
    #endregion

    [FoldoutGroup("References")] private FMOD.Studio.EventInstance CrowdInstance, ElevatorInstance;
    [FoldoutGroup("References")] public FMODUnity.EventReference CrowdRef;
    [FoldoutGroup("References")] public FMODUnity.EventReference PlayerLeftRef;
    [FoldoutGroup("References")] public List<FMODUnity.EventReference> DamageRef;
    [FoldoutGroup("References")] public List<FMODUnity.EventReference> DeathRef;

    //[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions, ValueLabel = "Val")]
    public Dictionary<SoundType, AudioClassInfo> Sounds = new Dictionary<SoundType, AudioClassInfo>();
    public Dictionary<string, SoundEffect> Effects = new Dictionary<string, SoundEffect>();


    //[FoldoutGroup("VoiceChat")] public float ElevatorAudioDistance, GameAudioDistance;
    [FoldoutGroup("OnHit")] public float TimeBetweenTakeDamage;
   


    
    public FMOD.Studio.EventInstance CreateSound(string Text)
    {
        SoundEffect effect = Effects[Text];
        FMOD.Studio.EventInstance Sound = FMODUnity.RuntimeManager.CreateInstance(effect.Ref);
        Sound.setVolume(Volume(Text));
        return Sound;
    }

    public float Volume(string ID) { return Effects[ID].Volume *  Sounds[Effects[ID].type].Volume * Sounds[SoundType.Master].Volume; }
    public float Volume(SoundType type) { return Sounds[type].Volume * Sounds[SoundType.Master].Volume; }

    public FMODUnity.EventReference RandomSound(List<FMODUnity.EventReference> SoundList) { return SoundList[Random.Range(0, SoundList.Count)]; }

    

    public void SetVolume(SoundType soundType, float Change)
    {
        float CurrentVolume = PlayerPrefs.GetFloat(soundType.ToString());
        float NewVolume = Mathf.Clamp(CurrentVolume + Change, 0f, 1f);

        PlayerPrefs.SetFloat(soundType.ToString(), NewVolume);
        Sounds[soundType].Volume = NewVolume;

        Debug.Log("vol2");
    }
    private void Update()
    {
        //handle specific effect volumes
        foreach (string Key in Effects.Keys)
        {
            PlayerPrefs.SetFloat(Key, Effects[Key].Volume);
        }
            

        ElevatorInstance.setVolume(Volume("elevator"));
    }
    private void OnInitialize()
    {
        ElevatorInstance = CreateSound("elevator");
        ElevatorInstance.setVolume(GetGameState() == GameState.Warmup ? Volume("elevator") : 0f);
        ElevatorInstance.start();

    }
    public void SetDoorAudio(int State)
    {
        ElevatorInstance.setParameterByName("ElevatorState", State);
        ElevatorInstance.setVolume(Volume("elevator"));
    }
    private void Start()
    {
        NetworkManager.OnInitialized += OnInitialize;
        NetworkManager.OnDoorState += SetDoorAudio;

        foreach (SoundType sound in Sounds.Keys)
            Sounds[sound].Volume = PlayerPrefs.GetFloat(sound.ToString());

        List<string> Keys = Effects.Keys.ToList();
        for (int i = 0; i < Keys.Count; i++)
            Effects[Keys[i]].Volume = PlayerPrefs.GetFloat(Keys[i]);
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
