using UnityEngine;
using Photon.Voice.Unity;
using Sirenix.OdinInspector;
public class PlayerAudio : SerializedMonoBehaviour
{
    public AudioSource AudioSpeaker;
    public Speaker PhotonSpeaker;

    void Update()
    {
        AudioSpeaker.volume = SoundManager.instance.Volume(SoundType.Voice);

        PhotonSpeaker.enabled = SoundManager.instance.CanPlay(SoundType.Voice);
    }
}
