using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using RestrictionSystem;
public enum SpellType
{
    Individual = 0,
    Both = 1,
}
public class AIMagicControl : MonoBehaviour
{
    void Awake() { instance = this; }
    public static AIMagicControl instance;

    public List<Transform> PositionObjectives;
    public List<Transform> Spawn;
    public List<Transform> Hands;
    public List<Transform> IdlePositions;
    public Transform Rig;
    public Transform Cam;
    public Transform CamOffset;
    public Transform MyCharacterDisplay;
    public Transform MyCharacterMetarig;
    public GameObject MyCharacterSkin;
    public SpellContainer spells;
    

    public bool HeadsetActive;
    public bool LeftHandActive;
    public bool RightHandActive;

    private bool HasCalled = false;

    public List<Material> Materials;
    public SkinnedMeshRenderer handToChange;

    public bool AllActive() { return HeadsetActive && LeftHandActive || RightHandActive; }
    
    private void Update()
    {
        if (AllActive())
        {
            PastFrameRecorder PR = PastFrameRecorder.instance;
            List<CurrentLearn> TrueMotions = RestrictionManager.instance.AllWorkingMotions(PR.PastFrame(Side.right), PR.GetControllerInfo(Side.right));
            CurrentLearn DisplayMotion = TrueMotions.Count == 0 ? CurrentLearn.Nothing : TrueMotions[0];
            handToChange.material = Materials[(int)DisplayMotion]; //set hand
        }
        

        InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.isTracked, out HeadsetActive);
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.isTracked, out RightHandActive);
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.isTracked, out LeftHandActive);

        PastFrameRecorder.instance.UseSides = new List<bool>() { RightHandActive, LeftHandActive };

        if (!HeadsetActive)
            Set(Cam, IdlePositions[0]);
        if (!RightHandActive)
            Set(Hands[0], IdlePositions[1]);
        if (!LeftHandActive)
            Set(Hands[1], IdlePositions[2]);
        /*
        if (!AllActive())
        {
            if (HasCalled == false && Initialized() && Exists(PlayerTeam, PhotonNetwork.LocalPlayer))
            {
                HasCalled = true;
                Vector3 Spawn = SpawnManager.instance.Spawns[(int)GetPlayerTeam(PhotonNetwork.LocalPlayer)].position;
                Rig.position = new Vector3(Spawn.x, 1f + DoorManager.instance.Doors[0].OBJ.position.y, Spawn.z);
            }
            
        }
        */

        void Set(Transform ToSet, Transform Reference)
        {
            ToSet.position = Reference.position;
            ToSet.rotation = Reference.rotation;
        }
    }
}