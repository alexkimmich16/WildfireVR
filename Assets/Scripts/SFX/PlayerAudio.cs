using UnityEngine;
using Photon.Voice.Unity;
using Sirenix.OdinInspector;
public class PlayerAudio : SerializedMonoBehaviour
{
    public AudioSource AudioSpeaker;
    public Speaker PhotonSpeaker;
    void Start()
    {
        
    }

    void Update()
    {
        AudioSpeaker.volume = PlayerPrefs.GetFloat("MasterVolume");
    }
}
