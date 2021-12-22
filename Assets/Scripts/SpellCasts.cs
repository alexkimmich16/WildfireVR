using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpellCasts : MonoBehaviour
{
    public HandMagic HM;
    private GameObject MultiplayerLeftShield;
    private GameObject MultiplayerRightShield;
    private void Update()
    {
        UpdateShieldMultiplayerPosition();
    }
    public void RemoveObjectFromNetwork(GameObject obj)
    {
        PhotonNetwork.Destroy(obj);
    }

    #region Spike
    public void UseSpike(Vector3 Position)
    {
        //Vector3 Direction = new Vector3(0,,0).normalized;
        GameObject spike = Instantiate(HM.Spike, Position, Quaternion.LookRotation(new Vector3(0, HM.Cam.transform.rotation.y, 0).normalized));
        var particleSystemMainModule = spike.GetComponent<ParticleSystem>().main;
        spike.GetComponent<ParticleSystem>().Play();
        particleSystemMainModule.startRotation3D = true;

        particleSystemMainModule.startRotation = new Vector3(0, HM.Cam.transform.rotation.y, 0);
        Destroy(spike, HM.SpikeTimeDelete);

        //eventually check for people and do damage
    }
    #endregion
    #region Fireball
    public void FireballCharge(int Hand)
    {
        ///flame animation on hand
        ///OR
        ///fireball in palm of hand

    }

    public void FireballShoot(int Hand)
    {
        //undue fireball change
        Vector3 VelDirection = HM.Controllers[Hand].PastFrames[0] - HM.Controllers[Hand].PastFrames[HandActions.PastFrameCount - 1];
        VelDirection = VelDirection.normalized;
        //GameObject FireBall = Instantiate(HM.Fireball, HM.Controllers[Hand].transform.position, Quaternion.LookRotation(VelDirection));
        //FireBall.GetComponent<Fireball>().Speed = HM.Speed;
        if (InfoSave.instance.SceneState == SceneSettings.Public)
        {
            GameObject fireball = PhotonNetwork.Instantiate("FireballMultiplayer", HM.Controllers[Hand].transform.position, Quaternion.LookRotation(VelDirection));
            fireball.GetComponent<Fireball>().Speed = HM.Speed;
        }
    }
    #endregion
    #region Shield
    public void StartShield(int Left)
    {
        HM.ChangeMagic(-HM.Spells[2].Cost);
        HM.Shields[Left].Health = HM.MaxShield;
        ChangeShield(Left, true);
        if (InfoSave.instance.SceneState == SceneSettings.Public)
        {
            if(Left == 0)
            {
                MultiplayerLeftShield = PhotonNetwork.Instantiate("ShieldMultiplayer", HM.Controllers[Left].transform.position, HM.Controllers[Left].transform.rotation);
            }
            else if(Left == 1)
            {
                MultiplayerRightShield = PhotonNetwork.Instantiate("ShieldMultiplayer", HM.Controllers[Left].transform.position, HM.Controllers[Left].transform.rotation);
            }
        }
    }
    public void UpdateShieldMultiplayerPosition()
    {
        if (MultiplayerLeftShield != null)
        {
            MultiplayerLeftShield.transform.position = HM.Controllers[0].transform.position;
            MultiplayerLeftShield.transform.rotation = HM.Controllers[0].transform.rotation;
        }
        if (MultiplayerRightShield != null)
        {
            MultiplayerRightShield.transform.position = HM.Controllers[1].transform.position;
            MultiplayerRightShield.transform.rotation = HM.Controllers[1].transform.rotation;
        }
    }
    public void EndShield(int Left)
    {
        HM.Shields[Left].Health = 0;
        ChangeShield(Left, false);
        if (InfoSave.instance.SceneState == SceneSettings.Public)
        {
            if (Left == 0)
            {
                RemoveObjectFromNetwork(MultiplayerLeftShield);
                MultiplayerLeftShield = null;
            }
            else if (Left == 1)
            {
                RemoveObjectFromNetwork(MultiplayerRightShield);
                MultiplayerRightShield = null;
            }
        } 
    }

    public void ShieldDamage(int Damage, int Side)
    {
        HM.Shields[Side].Health -= Damage;
        if (HM.Shields[Side].Health < 1)
        {
            EndShield(Side);
            ChangeShield(Side, false);
        }
    }
    public void ChangeShield(int Side, bool On)
    {
        HM.Shields[Side].Shield.SetActive(On);
    }
    #endregion
    #region ForcePush
    public void UseForcePush()
    {
        //Debug.Log("push");
        Vector3 pos = HM.Cam.transform.position;
        if (HandMagic.AllSounds == true)
        {
            HM.Force.Play();
        }

        Collider[] colliders = Physics.OverlapSphere(pos, HM.PushRadius);
        foreach (Collider pushedOBJ in colliders)
        {
            if (pushedOBJ.tag != "Player" && pushedOBJ.gameObject.GetComponent<Rigidbody>() != null)
            {
                Vector3 directionToTarget = pos - pushedOBJ.transform.position;
                Vector3 ZPlacementObj = new Vector3(pushedOBJ.transform.position.x, HM.Cam.position.y, pushedOBJ.transform.position.z);
                Vector3 targetDir = ZPlacementObj + HM.Cam.transform.position;
                float ObjectAngle = Vector3.Angle(targetDir, HM.Cam.transform.forward);
                float PlayerAngle = HM.Cam.rotation.eulerAngles.y;
                float Difference = (ObjectAngle - PlayerAngle + 180);
                //Debug.Log("ObjectAngle:  " + ObjectAngle + "  PlayerAngle:  " + PlayerAngle + "  Difference:  " + Difference);
                
                if (Difference < HM.AngleMax && Difference > -HM.AngleMax)
                {
                    Rigidbody pushed = pushedOBJ.GetComponent<Rigidbody>();
                    pushed.AddExplosionForce(HM.PushAmount, pos, HM.PushRadius);
                }
               
            }
        }
    }
    #endregion
    #region Fly
    /*
    public bool CheckFlying(HandActions Hand)
    {
        if (Hand.Trigger > HM.TriggerThreshold)
        {
            if (HM.CurrentMagic > 1)
            {
                //Controllers[i].Fly();
                HM.ChangeMagic(-HM.FlyingCost);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public void Fly()
    {
        Side side = HM.
        int SideNum = (int)side;
        if (SideNum == 0)
        {
            RB.AddForce(-transform.right * Power, ForceMode.Impulse);
        }
        else
        {
            RB.AddForce(transform.right * Power, ForceMode.Impulse);
        }
    }
    */
    #endregion
}
