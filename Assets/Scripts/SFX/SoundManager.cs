using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public enum SoundType
{
    Master = 0,
    Elevator = 1,
    Effect = 2,
    Crowd = 3,
    Voice = 4,
}

[System.Serializable]
public class AudioClassInfo
{
    public bool CanPlay;
    [Range(0.0f, 1f)] public float Volume = 1;
}

public class SoundManager : SerializedMonoBehaviour
{
    #region Singleton + classes
    public static SoundManager instance;
    void Awake() { instance = this; }

    #endregion

    [FoldoutGroup("References")] private FMOD.Studio.EventInstance CrowdInstance, ElevatorInstance;
    [FoldoutGroup("References")] public FMODUnity.EventReference CrowdRef, ElevatorRef, FireballRef, FlamesRef, BlockRef;
    [FoldoutGroup("References")] public FMODUnity.EventReference PlayerLeftRef;
    [FoldoutGroup("References")] public List<FMODUnity.EventReference> DamageRef;
    [FoldoutGroup("References")] public List<FMODUnity.EventReference> DeathRef;

    //[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions, ValueLabel = "Val")]
    public Dictionary<SoundType, AudioClassInfo> Sounds = new Dictionary<SoundType, AudioClassInfo>();

    //public List<AudioClip> CrowdShouts;

    public FMODUnity.EventReference RandomSound(List<FMODUnity.EventReference> SoundList) { return SoundList[Random.Range(0, SoundList.Count)]; }

    public bool CanPlay(SoundType type) { return Sounds[type].CanPlay && Sounds[SoundType.Master].CanPlay; }
    public float Volume(SoundType type) { return Sounds[type].Volume * Sounds[SoundType.Master].Volume; }


    private void Update()
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(SoundType)).Length; i++)
            PlayerPrefs.SetFloat(((SoundType)i).ToString(), Sounds[(SoundType)i].Volume);

        ElevatorInstance.setVolume(Volume(SoundType.Elevator));
    }
    private void OnInitialize()
    {
        if (CanPlay(SoundType.Elevator))
        {
            ElevatorInstance = FMODUnity.RuntimeManager.CreateInstance(ElevatorRef);
            ElevatorInstance.setVolume(Volume(SoundType.Elevator));
            ElevatorInstance.start();
        }
    }
    public void SetDoorAudio(int State)
    {
        ElevatorInstance.setParameterByName("ElevatorState", State);
    }
    private void Start()
    {
        NetworkManager.OnInitialized += OnInitialize;
        for (int i = 0; i < System.Enum.GetValues(typeof(SoundType)).Length; i++)
            Sounds[(SoundType)i].Volume = PlayerPrefs.GetFloat(((SoundType)i).ToString());
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
