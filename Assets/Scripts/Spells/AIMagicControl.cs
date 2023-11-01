using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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

    public List<Material> Materials;
    public List<SkinnedMeshRenderer> handsToChange;

    public Transform hitbox;
    public Transform PreventorCapsule;
    public float HitboxBackDist = 0.5f;

    public bool AllowIdlePositions;

    private Vector3 Offset;
    private void Start()
    {
        Offset = hitbox.position - Cam.position;
    }
    public bool AllActive() { return HeadsetActive && LeftHandActive && RightHandActive; }
    private void FixedUpdate()
    {
        PreventorCapsule.position = new Vector3(Cam.position.x, Rig.position.y, Cam.position.z);
    }
    //public bool CanCast
    private void Update()
    {
        InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.isTracked, out HeadsetActive);
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.isTracked, out RightHandActive);
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.isTracked, out LeftHandActive);

        
        //Vector3 AdjustedOffset = Offset * Quaternion.LookRotation(new Vector3(Cam.forward.x, 0f, Cam.forward.z));
        //Vector3 RealPos = 
        hitbox.position = Cam.position - Cam.forward * HitboxBackDist;

        //handsToChange[0].material = Materials[ConditionManager.instance.ConditionStats[0, 0].SequenceState];

        if (AllowIdlePositions)
        {
            if (!HeadsetActive)
                Set(Cam, IdlePositions[0]);
            if (!RightHandActive)
                Set(Hands[0], IdlePositions[1]);
            if (!LeftHandActive)
                Set(Hands[1], IdlePositions[2]);
        }
        

        void Set(Transform ToSet, Transform Reference)
        {
            ToSet.position = Reference.position;
            ToSet.rotation = Reference.rotation;
        }
    }
}