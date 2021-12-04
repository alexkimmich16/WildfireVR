using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HandMagic : MonoBehaviour
{
    #region Singleton
    public static HandMagic instance;
    void Awake() { instance = this; }
    #endregion

    public HandActions Left;
    public HandActions Right;
    public List<HandActions> Controllers = new List<HandActions>();
    public Transform Cam;

    [Range(0f, 1f)]
    public float TriggerThreshold, GripThreshold;
    [Header("Magic")]
    public bool ShouldCharge;
    public bool InfiniteMagic;
    public float CurrentMagic;
    public float MagicRecharge;
    public int MaxMagic;
    [Header("Flying")]
    [Range(0f, 1f)]
    public float FlyingCost;
    [Header("Push")]
    public float PushAmount;
    public float PushRadius;
    public float PushCost;
    [Header("Misc")]
    public Material Active;
    public Material DeActive;

    public List<TextMeshProUGUI> text = new List<TextMeshProUGUI>();

    public AudioSource Force;

    public bool Sounds;

    public List<Collider> AroundColliders = new List<Collider>();
    [Header("Spike")]
    private bool SpikeActive, ShieldActive;
    public bool UseSpikePlacement = false;
    public float SpikeTimeDelete;
    public GameObject Spike;
    public float YRise;

    [Header("Shield")]
    public GameObject Shield;
    public int MaxShield;

    //0 is left, 1 is right
    public List<ShieldName> Shields = new List<ShieldName>();
    [Range(0f, 1f)]
    public float ShieldCost;
    //inumerator should be the one handactions sends to saying it should start sequence
    
    public void ShieldDamage(int Damage, int Side)
    {
        Shields[Side].Health -= Damage;
        if (Shields[Side].Health < 1)
        {
            Shields[Side].Health = 0;
            ChangeShield(Side, false);
        }
    }
    //on health smaller than 0 shatter and endshield and take away a bunch of mana
    public void StartShield(int Left)
    {
        Shields[Left].Health = MaxShield;
        ChangeShield(Left, true);
    }
    public void EndShield(int Left)
    {
        Shields[Left].Health = 0;
        ChangeShield(Left, false);
    }
    
    public void ChangeShield(int Side, bool On)
    {
        Shields[Side].Shield.SetActive(On);
    }

    //public void 

    public Vector3 RaycastGround()
    {
        RaycastHit hit;
        int layerMask = 1 << 8;
        //layerMask = ~layerMask;

        if (Physics.Raycast(Cam.position, Cam.forward, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 HitSpot = hit.point;
            HitSpot = new Vector3(HitSpot.x, HitSpot.y + YRise, HitSpot.z);
            return hit.point;
            Debug.Log("Did Hit");
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
            return Vector3.zero;
        }
    }

    public void SpikeBools()
    {
        if (UseSpikePlacement == true)
        {
            //show menu for spi
            Vector3 point = RaycastGround();

            //raycast spike
            //if player touches button to confirm
            //useSpike()
        }
        else
        {
            //UseSpike

            if (RaycastGround() != Vector3.zero)
            {
                UseSpike(RaycastGround());
                SpikeActive = false;
            }
        }

        //check x distance
        //get raycast of head direction

        //get spot of hitpoint where raycast hits layer of ground
        //spawn spike there
        //get and play animation of the spike
        //after certain amount of time remove spike
        //raycast and if null return

    }
    public void UseSpike(Vector3 Position)
    {
        GameObject spike = Instantiate(Spike, Position, Quaternion.identity);
        spike.GetComponent<ParticleSystem>().Play();
        Destroy(spike, SpikeTimeDelete);

        //eventually check for people and do damage
    }
    public void StartSpike()
    {
        SpikeActive = true;
    }

    public void UseForcePush(float ZDirection, Vector3 pos, Vector3 dir)
    {
        //make z relavant
        if(Sounds == true)
        {
            Force.Play();
        }
        
        if (CurrentMagic < PushCost)
        {
            //show not enough magic
            return;
        }
        //do some effect animation
        //play force sound
        ChangeMagic(-PushCost);

        Collider[] colliders = Physics.OverlapSphere(pos, PushRadius);
        foreach (Collider pushedOBJ in colliders)
        {
            if (pushedOBJ.tag != "Player" && pushedOBJ.gameObject.GetComponent<Rigidbody>() != null)
            {
                Vector3 directionToTarget = pos - pushedOBJ.transform.position;
                float angle = Vector3.Angle(dir, directionToTarget);
                float distance = directionToTarget.magnitude;
                //Debug.Log(angle + " " + pushedOBJ);

                Rigidbody pushed = pushedOBJ.GetComponent<Rigidbody>();
                pushed.AddExplosionForce(PushAmount, pos, PushRadius);
                if (Mathf.Abs(angle) < 90 && distance < 10)
                {
                    //Debug.Log("target is in front of me");
                    //Rigidbody pushed = pushedOBJ.GetComponent<Rigidbody>();
                    //pushed.AddExplosionForce(PushAmount, pos, PushRadius);
                }
            }
        }
    }

    public bool CheckFlying(HandActions Hand)
    {
        if (Hand.Trigger > TriggerThreshold)
        {
            if (CurrentMagic > 1)
            {
                //Controllers[i].Fly();
                ChangeMagic(-FlyingCost);
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

    void Update()
    {
        Debug.DrawRay(Cam.position, Cam.forward * 10000f, Color.red);
        if (SpikeActive == true)
        {
            SpikeBools();
        }
        if (ShouldCharge == true)
        {
            Charge();
        }

        bool UsingMagic = false;

        //check for flying
        for (int i = 0; i < 2; i++)
        {
            bool Flying = CheckFlying(Controllers[i]);
            // trigger is held and has enough magic
            if(Flying == true)
            {
                UsingMagic = true;
                if (Controllers[i].Playing == false)
                {
                    Controllers[i].Playing = true;
                    Controllers[i].PS.Play();
                }
            }
            else
            {
                
                if (Controllers[i].Playing == true)
                {
                    Controllers[i].Playing = false;
                    Controllers[i].PS.Stop();
                }
            }
            
        }

        ShouldCharge = !UsingMagic;
    }
    
    void Charge()
    {
        ChangeMagic(MagicRecharge);
    }

    public void ChangeMagic(float BaseChange)
    {
        if (InfiniteMagic == true)
        {
            return;
        }
        //Debug.Log(BaseChange);
        float Positive = Mathf.Abs(BaseChange);
        if (Mathf.Sign(BaseChange) == 1)
        {
            //add: if potential magic after adding is bigger than the max
            if (CurrentMagic + Positive >= MaxMagic)
            {
                CurrentMagic = MaxMagic;
            }
            else
            {
                //Debug.Log(BaseChange);
                CurrentMagic += Positive;
            }
        }
        else if (Mathf.Sign(BaseChange) == -1)
        {
            //subtract: if potential magic after subtracting is smaller than 0
            if (CurrentMagic - Positive <= 0)
            {
                CurrentMagic = 0;
            }
            else
            {
                CurrentMagic -= Positive;
            }
        }
    }

    public void ChangeText(string stuff, int Num)
    {
        text[Num].text = stuff;
    }

    void Start()
    {
        CurrentMagic = MaxMagic;
    }

    [System.Serializable]
    public class ShieldName
    {
        public string name;
        public int Health;
        public GameObject Shield;
    }
}
