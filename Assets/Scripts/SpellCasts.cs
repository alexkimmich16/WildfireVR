using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpellCasts : MonoBehaviour
{
    public HandMagic HM;
    public static bool DirectionalPush = false;
    [System.Serializable]
    public class SidesStats
    {
        public Side side;
        public Transform HeldObject;
        public bool TelekinesisActive;
        public int ShieldHealth;
        public GameObject Fire;
        public GameObject Shield;
        
        public bool Flying;

        public bool Slashing;
        public Vector3 Start;
        public Vector3 End;
    }
    public List<SidesStats> Stats = new List<SidesStats>();
    #region Spike
    public void UseSpike(Vector3 Position)
    {
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
    public IEnumerator FireballMagic()
    {
        yield return new WaitForSeconds(1);
        HM.ChangeMagic(HM.Spells[1].Cost);
    }
    public void FireballStart(int Hand)
    {
        //StartFire()
        //HM.Controllers[Hand].transform.GetComponent<FireController>().StartFire();
        Stats[Hand].Fire = PhotonNetwork.Instantiate("NewFire", HM.Controllers[Hand].transform.position, HM.Controllers[Hand].transform.rotation);
        StartCoroutine(FireballMagic());
    }
    public void StartFireballEnd(int Hand)
    {
        Stats[Hand].Fire.GetComponent<FireController>().StopFire();
        //HM.Controllers[Hand].transform.GetComponent<FireController>().StopFire();
        StopCoroutine(FireballMagic());

        /*
        Vector3 VelDirection = HM.Controllers[Hand].PastFrames[0] - HM.Controllers[Hand].PastFrames[HandActions.PastFrameCount - 1];
        VelDirection = VelDirection.normalized;
        //GameObject FireBall = Instantiate(HM.Fireball, HM.Controllers[Hand].transform.position, Quaternion.LookRotation(VelDirection));
        //FireBall.GetComponent<Fireball>().Speed = HM.Speed;
        GameObject fireball = PhotonNetwork.Instantiate("FireballMultiplayer", HM.Controllers[Hand].transform.position, Quaternion.LookRotation(VelDirection));
        SoundManager.instance.PlayAudio("Fireball", fireball);
        fireball.GetComponent<Fireball>().Speed = HM.Speed;
        */
    }
    public void RemoveFireball(GameObject Fire)
    {
        for (int i = 0; i < 2; i++)
            if (Stats[i].Fire != null)
                if (Stats[i].Fire == Fire)
                {
                    RemoveObjectFromNetwork(Stats[i].Fire);
                    Stats[i].Fire = null;
                }
    }
    #endregion
    #region Shield
    public void StartShield(int Left)
    {
        HM.ChangeMagic(-HM.Spells[2].Cost);
        Stats[Left].Shield = PhotonNetwork.Instantiate("NewShield", HM.Controllers[Left].transform.position, HM.Controllers[Left].transform.rotation);
        Stats[Left].Shield.GetComponent<ShieldManager>().StartShield();
        
        Stats[Left].ShieldHealth = HM.MaxShield;
        SoundManager.instance.PlayAudio("Shield", null);
    }

    public void EndShield(int Left)
    {
        Stats[Left].Shield.GetComponent<ShieldManager>().StopShield();
        Stats[Left].ShieldHealth = 0;
        if (Stats[Left].Shield != null)
        {
            RemoveObjectFromNetwork(Stats[Left].Shield);
        }
        Stats[Left].Shield = null;
        
        //ChangeShield(Left, false);
    }
    
    public void UpdateShieldMultiplayerPosition()
    {
        if (Stats[0].Shield != null)
        {
            Stats[0].Shield.transform.position = HM.Controllers[0].transform.position;
            Stats[0].Shield.transform.rotation = HM.Controllers[0].transform.rotation;
        }
        if (Stats[1].Shield != null)
        {
            Stats[1].Shield.transform.position = HM.Controllers[1].transform.position;
            Stats[1].Shield.transform.rotation = HM.Controllers[1].transform.rotation;
        }
    }
    public void ShieldDamage(int Damage, int Side)
    {
        Stats[Side].ShieldHealth -= Damage;
        if (Stats[Side].ShieldHealth < 1)
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
    #region Telekinesis
    public void SetTelekinesisActive(int side, bool Active)
    {
        Stats[side].TelekinesisActive = Active;
    }
    public void SetTelekineticToNull(int Side)
    {
        if (Stats[Side].HeldObject != null)
        {
            Stats[Side].HeldObject.GetComponent<telekinetic>().Touched = false;
            Stats[Side].HeldObject.GetComponent<Renderer>().material = Stats[Side].HeldObject.GetComponent<telekinetic>().Spare;
        }
        Stats[Side].HeldObject = null;
    }
    public void CheckTelekinesis()
    {
        for (int i = 0; i < Stats.Count; i++)
        {
            Transform BasePos = HM.Controllers[i].transform;
            if (Stats[i].TelekinesisActive == true)
            {
                //raycast object has telekinesis object
                RaycastHit hit = new RaycastHit();
                Ray ray = new Ray(HM.Controllers[i].transform.position, HM.Controllers[i].transform.forward);
                if (Physics.Raycast(ray, out hit))
                    if (hit.transform.GetComponent<telekinetic>())
                    {
                        hit.transform.GetComponent<telekinetic>().Touched = true;
                        Stats[i].HeldObject = hit.transform;
                        hit.transform.GetComponent<Renderer>().material = hit.transform.GetComponent<telekinetic>().Active;
                    }
                    else
                    {
                        SetTelekineticToNull(i);
                    }
                else
                    SetTelekineticToNull(i);
            }
            else
                SetTelekineticToNull(i);

            if (Stats[i].HeldObject != null)
            {
                Ray r = new Ray(BasePos.position, BasePos.forward);
                Vector3 Position = r.GetPoint(3);
                Stats[i].HeldObject.position = Position;
            }
        }
    }
    //trigger, than grip to use as given
    #endregion
    #region Fly
    public void SetFlyingActive(int side, bool Active)
    {
        Stats[side].TelekinesisActive = Active;
    }
    public void CheckFlying()
    {
        for (int i = 0; i < Stats.Count; i++)
        {
            if(Stats[i].Flying == true)
            {
                HM.ChangeMagic(-HM.Spells[5].Cost);
                Fly(i);
            }
        }
            
    }
    public void Fly(int side)
    {
        if (side == 0)
        {
            HandMagic.instance.RB.AddForce(-transform.right * HM.FlightPower, ForceMode.Impulse);
        }
        else
        {
            HandMagic.instance.RB.AddForce(transform.right * HM.FlightPower, ForceMode.Impulse);
        }
    }
    
    #endregion
    #region Slash
    public void PerformSlash(Vector3 Start, Vector3 End, Vector3 Direction)
    {
        //draw curved line from points
        //go in direction of points

    }
    public void AddSlashPosList()
    {
        for (int i = 0; i < Stats.Count; i++)
        {
            if(Stats[i].Start == Vector3.zero)
            {
                if (Stats[i].Slashing == true)
                {
                    if (Stats[i].Start == Vector3.zero)
                    {
                        //zero and slashing
                        Stats[i].Start = HM.Controllers[i].transform.position;
                    }
                }
                else
                {
                    //end
                    Stats[i].End = HM.Controllers[i].transform.position;
                    //PerformSlash()
                    Stats[i].Start = Vector3.zero;
                    Stats[i].End = Vector3.zero;
                }
            }
            
        }
    }
    public void SetSlashingActive(int side, bool Active)
    {
        Stats[side].Slashing = Active;
    }
    #endregion
    private void Update()
    {
        //UpdateShieldMultiplayerPosition();
        UpdatePositions();
        CheckTelekinesis();
        CheckFlying();
        AddSlashPosList();
    }
    private void UpdatePositions()
    {
        for (int i = 0; i < 2; i++)
        {
            if (Stats[i].Fire != null)
            {
                Stats[i].Fire.transform.position = HM.Controllers[i].transform.position;
                Stats[i].Fire.transform.rotation = HM.Controllers[i].transform.rotation;
            }
                
            if (Stats[i].Shield != null)
            {
                Stats[i].Shield.transform.position = HM.Controllers[i].transform.position;
                Stats[i].Shield.transform.rotation = HM.Controllers[i].transform.rotation;
            }
        }
    }
    public void RemoveObjectFromNetwork(GameObject obj)
    {
        PhotonNetwork.Destroy(obj);
    }
}
