using UnityEngine;
using UnityEngine.XR;
using Sirenix.OdinInspector;
using Photon.Pun;
using Photon.Realtime;
using static Odin.Net;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using RootMotion.FinalIK;
public class HeightCalibration : MonoBehaviourPunCallbacks
{
    private bool isCalibrating = false;

    public VRIK IK;
    //public Transform headOffset;
    private void Start()
    {
        if (PlayerPrefs.HasKey("userHeight"))
        {
            float RealHeight = PlayerPrefs.GetFloat("userHeight");
            IK.transform.parent.localScale = new Vector3(RealHeight, RealHeight, RealHeight);
            SetPlayerVar(ID.Height, RealHeight, PhotonNetwork.LocalPlayer);
        }

        if (PlayerPrefs.HasKey("userArmLength"))
        {
            float RealArmLength = PlayerPrefs.GetFloat("userArmLength");
            IK.solver.leftArm.armLengthMlp = RealArmLength;
            IK.solver.rightArm.armLengthMlp = RealArmLength;
            SetPlayerVar(ID.ArmLength, RealArmLength, PhotonNetwork.LocalPlayer);
        }

        
    }
    void Update()
    {
        // Begin calibration with a key press
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCalibrating = true;
            Debug.Log("Calibration started. Press the trigger when ready.");
        }

        if (isCalibrating)
        {
            // Get the XR device for the user's right hand (you might need to customize this depending on your setup)
            InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            // Check for a trigger press
            if (rightHandDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue) && triggerValue)
            {
                // Calibrate the height and arm length, and update the IK rig
                Calibrate();
                isCalibrating = false;
                Debug.Log("Calibration complete.");
            }
        }
    }

    void Calibrate()
    {
        // Get the XR devices for the head and hands
        InputDevice headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        InputDevice leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        //HEIGHT
        float Height = 0f;
        if (headDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headPosition))
        {
            Height = headPosition.y;
        }
        float RealHeight = (Height / 2f) - AIMagicControl.instance.CamOffset.localPosition.y;
        IK.transform.parent.localScale = new Vector3(RealHeight, RealHeight, RealHeight);
        PlayerPrefs.SetFloat("userHeight", RealHeight);
        SetPlayerVar(ID.Height, RealHeight, PhotonNetwork.LocalPlayer);

        //ARMS
        if (leftHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftHandPosition)
            && rightHandDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightHandPosition))
        {
            float armLength = Vector3.Distance(leftHandPosition, rightHandPosition) / 2f;
            float RealArmLength = (Height) - (armLength / 4f);
            IK.solver.leftArm.armLengthMlp = RealArmLength;
            IK.solver.rightArm.armLengthMlp = RealArmLength;
            PlayerPrefs.SetFloat("userArmLength", RealArmLength);
            SetPlayerVar(ID.ArmLength, RealArmLength, PhotonNetwork.LocalPlayer);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!changedProps.ContainsKey(ID.Height) && !changedProps.ContainsKey(ID.ArmLength))
            return;

        //find player, change player
        VRIK IK = NetworkManager.instance.PlayerList[targetPlayer].GetComponent<NetworkPlayer>().myRagdoll.GetComponent<VRIK>();
        if (changedProps.ContainsKey(ID.ArmLength))
        {
            IK.solver.leftArm.armLengthMlp = (float)changedProps[ID.ArmLength];
            IK.solver.rightArm.armLengthMlp = (float)changedProps[ID.ArmLength];
        }

        if (changedProps.ContainsKey(ID.Height))
        {
            float Height = (float)changedProps[ID.Height]; ;
            IK.transform.parent.localScale = new Vector3(Height, Height, Height);
        }

        //if (changedProps.ContainsKey(ID.Height))
        //SetNewPosition((Team)changedProps[ID.PlayerTeam]);


    }
}