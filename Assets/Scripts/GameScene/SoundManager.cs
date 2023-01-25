using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
public enum SoundType
{
    Elevator = 0,
    Effect = 1,
    Crowd = 2,
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

public class SoundManager : MonoBehaviour
{
    #region Singleton + classes
    public static SoundManager instance;
    void Awake() { instance = this; }
    
    #endregion

    private Transform Parent;
    //public List<Audio> AudioClips = new List<Audio>();
    public bool EnableSounds;

    public bool EnableEffectSounds;
    public bool EnableElevatorSounds;
    public bool EnableCrowdSounds;
    
    [Header("BasicAmbience")]
    //public GameObject AmbienceOBJ;
    //public GameObject RoarOBJ;
    [Header("References")]
    private FMOD.Studio.EventInstance CrowdInstance, ElevatorInstance;
    public FMODUnity.EventReference CrowdRef, ElevatorRef, FireballRef, FlamesRef, BlockRef;
    public FMODUnity.EventReference PlayerLeftRef;
    public List<FMODUnity.EventReference> DamageRef;
    public List<FMODUnity.EventReference> DeathRef;
    [Header("Misc")]
    public float TimeAfterRoar;
    public List<AudioClip> CrowdShouts;

    public FMODUnity.EventReference RandomSound(List<FMODUnity.EventReference> SoundList) { return SoundList[Random.Range(0, SoundList.Count)]; }

    public bool CanPlaySound(SoundType type)
    {
        if (type == SoundType.Elevator)
            return EnableSounds && EnableElevatorSounds;
        if (type == SoundType.Effect)
            return EnableSounds && EnableEffectSounds;
        if (type == SoundType.Crowd)
            return EnableSounds && EnableCrowdSounds;

        return true;
    }
    
    private void OnInitialize()
    {
        if (CanPlaySound(SoundType.Elevator))
        {
            ElevatorInstance = FMODUnity.RuntimeManager.CreateInstance(ElevatorRef);
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
    }

    public void SetDoorAudio(int State)
    {
        ElevatorInstance.setParameterByName("ElevatorState", State);
    }
    private void Start()
    {
        NetworkManager.OnInitialized += OnInitialize;
        //NetworkManager.Initialize += OnPlayerLeave();
        //NetworkManager.Initialize += OnPlayerDeath();
    }
    private void Update()
    {
        if (!Initialized())
            return;
        
        
        //if (CanPlaySound(SoundType.Crowd) && Initialized())
            //UpdateCrowd();



        //if (UseElevatorSounds == true)
            //UpdateElevator();
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
