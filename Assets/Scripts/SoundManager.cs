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

    public bool UseCrowd;


    public AudioClip Ambience;
    public AudioClip Roar;
    public AudioClip Boo;

    ///
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
        //ambience

        GameObject Sound = Instantiate((GameObject)Resources.Load("Sound"), Vector3.zero, transform.rotation);
        Sound.transform.parent = Parent;
        Sound.GetComponent<AudioSource>().volume = 1f;
        Sound.GetComponent<AudioSource>().loop = true;
        Sound.GetComponent<AudioSource>().clip = Ambience;

        //Sound.
        Destroy(Sound.GetComponent<PhotonView>());
        Destroy(Sound.GetComponent<MagicalFX.FX_LifeTime>());
        Sound.GetComponent<AudioSource>().Play();
    }
    private void Update()
    {
        UpdateCrowd();
        //play ambience on repeat
        if (Input.GetKeyDown(KeyCode.D))
            OnPlayerDeath();
        if (Input.GetKeyDown(KeyCode.H))
            OnPlayerHit();
        if (Input.GetKeyDown(KeyCode.L))
            OnPlayerLeave();
    }
    

    public void OnPlayerDeath()
    {
        Roar
    }

    public void OnPlayerHit()
    {

    }

    public void OnPlayerLeave()
    {
        ///booooooo
    }
    public void UpdateCrowd()
    {

    }
}
