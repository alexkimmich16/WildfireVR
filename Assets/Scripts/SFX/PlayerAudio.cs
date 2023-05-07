using UnityEngine;
using Photon.Voice.Unity;
using Sirenix.OdinInspector;
public class PlayerAudio : SerializedMonoBehaviour
{
    public AudioSource AudioSpeaker;
    public Speaker PhotonSpeaker;

    void Update()
    {
        //maybe transition as door opens
        AudioSpeaker.volume = SoundManager.instance.Volume(SoundType.Voice);
        AudioSpeaker.maxDistance = DoorManager.instance.Sequence >= SequenceState.OpenOutDoor ? SoundManager.instance.GameAudioDistance : SoundManager.instance.ElevatorAudioDistance;
    }
}
