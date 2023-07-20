using UnityEngine;
using Photon.Voice.Unity;
using Sirenix.OdinInspector;
using Photon.Pun;
using static Odin.Net;
public class PlayerAudio : SerializedMonoBehaviour
{
    public AudioSource AudioSpeaker;
    public Speaker PhotonSpeaker;

    void Update()
    {
        //maybe transition as door opens
        AudioSpeaker.volume = SoundManager.instance.Volume(SoundType.Voice);
        bool TeamSpeakDoor = DoorManager.instance.Sequence < DoorState.OpenOutDoor;
        //AudioSpeaker.maxDistance = DoorManager.instance.Sequence >= SequenceState.OpenOutDoor ? SoundManager.instance.GameAudioDistance : SoundManager.instance.ElevatorAudioDistance;
        if (DoorManager.instance.Sequence >= DoorState.OpenOutDoor)
        {
            AudioSpeaker.enabled = TeamSpeakDoor ? GetPlayerTeam(transform.parent.parent.GetComponent<PhotonView>().Owner) == GetPlayerTeam(PhotonNetwork.LocalPlayer) : true;
        }

        
    }
}
