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
    public static bool DirectionalPush = false;
    public void RemoveObjectFromNetwork(GameObject obj)
    {
        PhotonNetwork.Destroy(obj);
    }

    #region Spike
    public void UseSpike(Vector3 Position)
    {
        //Vector3 Direction = new Vector3(0,,0).normalized;
        GameObject spike = PhotonNetwork.Instantiate("MultiplayerWall", Position, Quaternion.LookRotation(new Vector3(0, HM.Cam.transform.rotation.y, 0).normalized));
        var particleSystemMainModule = spike.GetComponent<ParticleSystem>().main;
        spike.GetComponent<ParticleSystem>().Play();
        particleSystemMainModule.startRotation3D = true;

        particleSystemMainModule.startRotationX = new ParticleSystem.MinMaxCurve(0);
        particleSystemMainModule.startRotationY = new ParticleSystem.MinMaxCurve(HM.Cam.transform.rotation.y + 90f);
        particleSystemMainModule.startRotationZ = new ParticleSystem.MinMaxCurve(0);
        spike.GetComponent<RemoveInTime>().MaxTime = spike.GetComponent<ParticleSystem>().main.duration;
        SoundManager.instance.PlayAudio("Spike", null);
        //HM.SpikeTimeDelete
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
            SoundManager.instance.PlayAudio("Fireball", fireball);
            fireball.GetComponent<Fireball>().Speed = HM.Speed;
        }
    }
    #endregion
    #region Shield
    public void StartShield(int Left)
    {
        HM.ChangeMagic(-HM.Spells[2].Cost);
        HM.Shields[Left].Health = HM.MaxShield;
        SoundManager.instance.PlayAudio("Shield", null);
        //ChangeShield(Left, true);
        if (InfoSave.instance.SceneState == SceneSettings.Public)
        {
            if(Left == 0)
            {
                MultiplayerLeftShield = PhotonNetwork.Instantiate("ShieldMultiplayer", HM.Controllers[Left].transform.position, HM.Controllers[Left].transform.rotation);
                MultiplayerLeftShield.SetActive(false);
                HM.Shields[0].Shield.SetActive(true);
                //MultiplayerLeftShield.GetComponent<Shield>().side = Side.Left;
            }
            else if(Left == 1)
            {
                MultiplayerRightShield = PhotonNetwork.Instantiate("ShieldMultiplayer", HM.Controllers[Left].transform.position, HM.Controllers[Left].transform.rotation);
                MultiplayerRightShield.SetActive(false);
                HM.Shields[1].Shield.SetActive(true);
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
        //ChangeShield(Left, false);
        if (InfoSave.instance.SceneState == SceneSettings.Public)
        {
            
            if (Left == 0)
            {
                
                if (MultiplayerLeftShield != null)
                {
                    RemoveObjectFromNetwork(MultiplayerLeftShield);

                }
                MultiplayerLeftShield = null;
                HM.Shields[0].Shield.SetActive(false);
            }
            else if (Left == 1)
            {
                if (MultiplayerRightShield != null)
                {
                    RemoveObjectFromNetwork(MultiplayerRightShield);
                }  
                MultiplayerRightShield = null;
                HM.Shields[1].Shield.SetActive(false);
            }
        } 
    }
    public void ShieldDamage(int Damage, int Side)
    {
        HM.Shields[Side].Health -= Damage;
        if (HM.Shields[Side].Health < 1)
        {
            EndShield(Side);
        }
    }
    #endregion
    #region ForcePush
    public void UseForcePush()
    {
        Vector3 pos = HM.Cam.transform.position;
        SoundManager.instance.PlayAudio("Force", null);
        Collider[] colliders = Physics.OverlapSphere(pos, HM.PushRadius);
        foreach (Collider pushedOBJ in colliders)
        {
            //Debug.Log("PT1");
            if (pushedOBJ.tag != "Player" && pushedOBJ.gameObject.GetComponent<Rigidbody>() != null)
            {
                
                Vector3 ZPlacementObj = new Vector3(pushedOBJ.transform.position.x, HM.Cam.position.y, pushedOBJ.transform.position.z);
                Vector3 targetDir = ZPlacementObj + HM.Cam.transform.position;
                float ObjectAngle = Vector3.Angle(targetDir, HM.Cam.transform.forward);
                float PlayerAngle = HM.Cam.rotation.eulerAngles.y;
                float Difference = (ObjectAngle - PlayerAngle + 90);
                //Debug.Log("PT1.1  " + "Difference:  " + Difference + "  HM.AngleMax:  " + HM.AngleMax);
                if (DirectionalPush == true)
                {
                    if (Difference < HM.AngleMax && Difference > -HM.AngleMax)
                    {
                        //Debug.Log("PT2");
                        
                    }
                }
                else if (DirectionalPush == false)
                {
                    if (pushedOBJ.GetComponent<Fireball>())
                    {
                        //Debug.Log("PT3");
                        Vector3 difference = pushedOBJ.transform.position - HM.Cam.position;
                        pushedOBJ.GetComponent<Fireball>().Bounce(difference);
                    }
                    else
                    {
                        //Debug.Log("PT4");
                        pushedOBJ.GetComponent<Rigidbody>().AddExplosionForce(HM.PushAmount, pos, HM.PushRadius);
                    }
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

    private void Update()
    {
        UpdateShieldMultiplayerPosition();
    }
}
