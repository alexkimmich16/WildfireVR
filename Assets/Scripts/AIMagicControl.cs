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

    public List<Material> Materials;
    public List<SkinnedMeshRenderer> handsToChange;

    public bool AllActive() { return HeadsetActive && LeftHandActive || RightHandActive; }
    
    private void Update()
    {
        InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.isTracked, out HeadsetActive);
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.isTracked, out RightHandActive);
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.isTracked, out LeftHandActive);

        if (AllActive())
        {
            
            PastFrameRecorder PR = PastFrameRecorder.instance;

            //List<CurrentLearn> TrueMotions = RestrictionManager.instance.AllWorkingMotions(PR.PastFrame(Side.right), PR.GetControllerInfo(Side.right));
            //CurrentLearn DisplayMotion = TrueMotions.Count == 0 ? CurrentLearn.Nothing : TrueMotions[0];
            for (int i = 0; i < handsToChange.Count; i++)
            {
                handsToChange[i].material = Materials[GetMatTestNum((Side)i)]; //set hand
            }
                

            int GetMatTestNum(Side side)
            {
                List<int> Working = new List<int>();
                for (int j = 1; j < RestrictionManager.instance.coefficents.RegressionStats.Count + 1; j++)
                    if (RestrictionManager.instance.MotionWorks(PR.PastFrame(side), PastFrameRecorder.instance.GetControllerInfo(side), (CurrentLearn)j))
                        Working.Add(j);
                if (Working.Count == 0)
                    return 0;
                else if (Working.Count == 1)
                    return Working[0];
                else
                    return RestrictionManager.instance.coefficents.RegressionStats.Count + 1;

            }

        }
        

        

        //PastFrameRecorder.instance.UseSides = new List<bool>() { RightHandActive, LeftHandActive };

        if (!HeadsetActive)
            Set(Cam, IdlePositions[0]);
        if (!RightHandActive)
            Set(Hands[0], IdlePositions[1]);
        if (!LeftHandActive)
            Set(Hands[1], IdlePositions[2]);

        void Set(Transform ToSet, Transform Reference)
        {
            ToSet.position = Reference.position;
            ToSet.rotation = Reference.rotation;
        }
    }
}