using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public enum SoundTypes
{
    Start = 0,
    During = 1,
}
[System.Serializable]
public class Audio
{
    public string Name;
    public AudioClip Sound;
    public SoundTypes type;

    [Range(0.0f, 1f)]
    public float Volume = 1;
}
public class SoundManager : MonoBehaviour
{
    #region Singleton + classes
    public Priority priority;
    public static Priority HighestPriority = Priority.None;
    public static SoundManager instance;
    void Awake()
    {
        if ((int)priority > (int)HighestPriority)
        {
            HighestPriority = priority;
            instance = this;
        }
        else
            Destroy(this);
    }
    
    #endregion

    private Transform Parent;
    public List<Audio> AudioClips = new List<Audio>();
    public bool UseSounds;

    public bool UseCrowdSounds;
    public bool UseElevatorSounds;


    
    [Header("BasicAmbience")]
    public GameObject AmbienceOBJ;
    public GameObject RoarOBJ;

    public float TimeAfterRoar;

    public AnimationCurve RoarCurve;
    public AnimationCurve AmbienceCurve;

    public AudioClip Ambience;
    public AudioClip Roar;
    public AudioClip Boo;

    public List<AudioClip> CrowdShouts;
    [Header("Elevator")]
    public AudioClip ElevatorRising;
    public AudioClip ElevatorStop;
    public AudioClip InDoorRising;
    public AudioClip InDoorStop;
    public AudioClip OutDoorRising;
    public AudioClip OutDoorStop;

    public AudioClip InDoorDrop;
    public AudioClip OutDoorDrop;

    public List<GameObject> ActiveElevatorSounds;

    ///public Transform ElevatorSoundPos;

    public void PlayAudio(string Name, GameObject ToParent)
    {
        //Debug.Log("PT1");
        if (UseSounds == true)
        {
            if (ToParent != null)
            {
                Parent = ToParent.transform;
            }
            //Debug.Log("PT2" + AudioClips.Count);
            for (int i = 0; i < AudioClips.Count; i++)
            {
                //Debug.Log("PT3");
                if (Name == AudioClips[i].Name)
                {
                    //Debug.Log("PT4");
                    if (AudioClips[i].type == SoundTypes.Start)
                    {
                        SpawnAudio(i, false);
                    }
                    else if (AudioClips[i].type == SoundTypes.During)
                    {
                        SpawnAudio(i, true);
                    }           
                    return;
                }
            }
        }
        
        
    }
    public void SpawnAudio(int Num, bool ShouldParent)
    {
        if (ShouldParent == false)
        {
            GameObject Sound = PhotonNetwork.Instantiate("Sound", Vector3.zero, transform.rotation);
            Sound.GetComponent<AudioSource>().clip = AudioClips[Num].Sound;
            Sound.GetComponent<AudioSource>().volume = AudioClips[Num].Volume;
            Sound.GetComponent<AudioSource>().Play();
        }
        else
        {
            GameObject Sound = PhotonNetwork.Instantiate("ParentSound", Vector3.zero, transform.rotation);
            Sound.transform.parent = Parent;
            Sound.GetComponent<AudioSource>().clip = AudioClips[Num].Sound;
            Sound.GetComponent<AudioSource>().volume = AudioClips[Num].Volume;
            Sound.GetComponent<AudioSource>().Play();
            Parent = null;
        }
    }
    private void Start()
    {
        if (UseCrowdSounds == true)
        {
            AmbienceOBJ = SpawnSound(Ambience, 1f, true);
            Destroy(AmbienceOBJ.GetComponent<PhotonView>());
            Destroy(AmbienceOBJ.GetComponent<MagicalFX.FX_LifeTime>());
            AmbienceOBJ.GetComponent<AudioSource>().Play();
            AmbienceOBJ.name = "Ambience";

            RoarOBJ = SpawnSound(Roar, 1f, false);
            RoarOBJ.name = "Roar";
            Destroy(RoarOBJ.GetComponent<PhotonView>());
            Destroy(RoarOBJ.GetComponent<MagicalFX.FX_LifeTime>());

            NetworkPlayer.TakeDamage += OnPlayerHit;
        }
        if(UseElevatorSounds == true)
        {
            DoorManager.OnDoorChange += ElevatorSound;
        }
        
    }
    private void Update()
    {
        if (UseCrowdSounds == true)
            UpdateCrowd();
        //if (UseElevatorSounds == true)
            //UpdateElevator();
    }

    public GameObject SpawnSound(AudioClip sound, float Volume, bool Loop)
    {
        GameObject SpawnObject = PhotonNetwork.Instantiate("Sound", Vector3.zero, transform.rotation);
        SpawnObject.GetComponent<AudioSource>().clip = sound;
        SpawnObject.GetComponent<AudioSource>().volume = Volume;
        SpawnObject.GetComponent<AudioSource>().loop = Loop;
        SpawnObject.GetComponent<AudioSource>().Play();
        return SpawnObject;
    }
    public void ElevatorSound(SequenceState state)
    {
        for (int i = 0; i < ActiveElevatorSounds.Count; i++)
            Destroy(ActiveElevatorSounds[i]);
        ActiveElevatorSounds.Clear();
        if (state == SequenceState.Waiting || state == SequenceState.ElevatorMove)
        {
            //repeat ElevatorRising
            if(ElevatorRising != null)
                ActiveElevatorSounds.Add(SpawnSound(ElevatorRising, 1f, true));
        }
        if (state == SequenceState.OpenInDoor)
        {
            //play ElevatorStop
            if (ElevatorStop != null)
                ActiveElevatorSounds.Add(SpawnSound(ElevatorStop, 1f, false));

            //repeat InDoorRising
            if (InDoorRising != null)
                ActiveElevatorSounds.Add(SpawnSound(InDoorRising, 1f, true));
        }
        if (state == SequenceState.OpenOutDoor)
        {
            //play InDoorStop
            if (InDoorStop != null)
                ActiveElevatorSounds.Add(SpawnSound(InDoorStop, 1f, false));

            //repeat OutDoorRising
            if (OutDoorRising != null)
                ActiveElevatorSounds.Add(SpawnSound(OutDoorRising, 1f, true));
        }
        if (state == SequenceState.WaitingForAllExit)
        {
            //play OutDoorStop
            if (OutDoorStop != null)
                ActiveElevatorSounds.Add(SpawnSound(OutDoorStop, 1f, false));
        }
        if (state == SequenceState.Closing)
        {
            //repeat InDoorDrop
            if (InDoorDrop != null)
                ActiveElevatorSounds.Add(SpawnSound(InDoorDrop, 1f, false));

            //repeat OutDoorDrop
            if (OutDoorDrop != null)
                ActiveElevatorSounds.Add(SpawnSound(OutDoorDrop, 1f, false));
        }
    }
    public void OnPlayerDeath()
    {
        //Roar
    }

    public void OnPlayerHit()
    {
        RoarOBJ.GetComponent<AudioSource>().Play();
        TimeAfterRoar = 0f;
    }

    public void OnPlayerLeave()
    {
        ///booooooo
    }
    public void UpdateCrowd()
    {
        TimeAfterRoar += Time.deltaTime;
        AmbienceOBJ.GetComponent<AudioSource>().volume = AmbienceCurve.Evaluate(TimeAfterRoar);
        RoarOBJ.GetComponent<AudioSource>().volume = RoarCurve.Evaluate(TimeAfterRoar);
        //RoarCurve
        //if (Input.GetKeyDown(KeyCode.D))
        //OnPlayerDeath();
        if (Input.GetKeyDown(KeyCode.H))
            OnPlayerHit();
        //if (Input.GetKeyDown(KeyCode.L))
        //OnPlayerLeave();
    }
}
