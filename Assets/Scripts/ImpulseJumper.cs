using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using RestrictionSystem;
using Photon.Pun;
public class ImpulseJumper : MonoBehaviour
{
    //HandSide
    //public List<HandSide> Hands;
    public List<bool> TriggerActive;
    public List<bool> Started;
    public List<float> Cooldowns;
    public List<float> Warmups;
    public float GroundForce;
    public float AirForce;
    public float CooldownSeconds;
    public float WarmupSeconds;

    public void DoPush(Side side)
    {
        float Force = XRPlayerMovement.instance.isGrounded ? GroundForce : AirForce;
        //GetTrigger(side)
        XRPlayerMovement.instance.Push(Force, - AIMagicControl.instance.Hands[(int)side].forward);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < TriggerActive.Count; i++)
        {
            Cooldowns[i] -= Time.deltaTime;
            float TriggerValue = GetTrigger((Side)i);

            if (!InGameManager.instance.CanDoMagic())
                return;

            if (TriggerActive[i] == false && TriggerValue > 0.4f && Cooldowns[i] < 0f)
            {
                Started[i] = true;
                Warmups[i] = 0f;
            }
            if (Started[i])
            {
                Warmups[i] += Time.deltaTime;
                if(Warmups[i]> WarmupSeconds)
                {
                    DoPush((Side)i);
                    TriggerActive[i] = true;
                    Cooldowns[i] = CooldownSeconds;
                    Started[i] = false;

                    GameObject ImpulseBlast = PhotonNetwork.Instantiate("ImpulseBlast", GetPos(), GetRot());
                    ImpulseBlast.GetPhotonView().RPC("SetOnlineVFX", RpcTarget.All, true);
                }
                
            }

            else if (TriggerActive[i] == true && TriggerValue < 0.1f)
            {
                TriggerActive[i] = false;
            }

            Quaternion GetRot() { return AIMagicControl.instance.Hands[i].rotation; }
            Vector3 GetPos() { return AIMagicControl.instance.PositionObjectives[i].position; }

        }
    }
    public float GetTrigger(Side side)
    {
        InputDevices.GetDeviceAtXRNode(side == Side.right ? XRNode.RightHand : XRNode.LeftHand).TryGetFeatureValue(CommonUsages.trigger, out float TriggerValue);
        return TriggerValue;
    }
}
