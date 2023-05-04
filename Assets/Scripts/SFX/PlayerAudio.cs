using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using UnityEngine.UI;
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
