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
        Sound.setVolume(GetVolume(Text));
        return Sound;
    }

    public float GetVolume(string ID) { return Effects[ID].Volume *  Sounds[Effects[ID].type].Volume * Sounds[SoundType.Master].Volume; }
    
    //public List<AudioClip> CrowdShouts;

    public FMODUnity.EventReference RandomSound(List<FMODUnity.EventReference> SoundList) { return SoundList[Random.Range(0, SoundList.Count)]; }

    public float Volume(SoundType type) { return Sounds[type].Volume * Sounds[SoundType.Master].Volume; }


    private void Update()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(SoundType)).Length; i++)
            PlayerPrefs.SetFloat(((SoundType)i).ToString(), Sounds[(SoundType)i].Volume);

        List<string> Keys = Effects.Keys.ToList();
        for (int i = 0; i < Keys.Count; i++)
            PlayerPrefs.SetFloat(Keys[i], Effects[Keys[i]].Volume);

        ElevatorInstance.setVolume(GetVolume("elevator"));
    }
    private void OnInitialize()
    {
        ElevatorInstance = CreateSound("elevator");
        ElevatorInstance.setVolume(GetGameState() == GameState.Warmup ? GetVolume("elevator") : 0f);
        ElevatorInstance.start();

    }
    public void SetDoorAudio(int State)
    {
        ElevatorInstance.setParameterByName("ElevatorState", State);
        ElevatorInstance.setVolume(GetVolume("elevator"));
    }
    private void Start()
    {
        NetworkManager.OnInitialized += OnInitialize;
        NetworkManager.OnDoorState += SetDoorAudio;
        for (int i = 0; i < System.Enum.GetValues(typeof(SoundType)).Length; i++)
            Sounds[(SoundType)i].Volume = PlayerPrefs.GetFloat(((SoundType)i).ToString());

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
