using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum SpellType
{
    Individual = 0,
    Both = 1,
}
public enum Side
{
    Left = 0,
    Right = 1,
}
public class HandMagic : MonoBehaviour
{
    #region Singleton + classes
    public static HandMagic instance;
    void Awake() { instance = this; }

    [System.Serializable]
    public class MagicInfo
    {
        public string Name;
        public SpellType Type;
        public float Cost;
        //public List<ControllerInfo> Steps = new List<ControllerInfo>();
        public float Leanience;
        public List<bool> Finished = new List<bool>();
        
        //public Vector3 Leanience;
        public List<ControllerInfo> Controllers = new List<ControllerInfo>();

        public List<GameObject> Sides = new List<GameObject>();
    }

    [System.Serializable]
    public class ControllerInfo
    {
        //public Side side;
        public int Current;
        public List<bool> ControllerFinished = new List<bool>();
    }
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
    
    [Header("Misc")]
    public Material Active;
    public Material DeActive;

    public List<TextMeshProUGUI> text = new List<TextMeshProUGUI>();
    public AudioSource Force;
    public bool Sounds;
    public List<Collider> AroundColliders = new List<Collider>();

    [Header("Spike")]
    public float SpikeTimeDelete;
    public GameObject Spike;
    public float YRise;
    private bool UseSpikePlacement = false;

    [Header("Fireball")]

    [Header("Shield")]
    public int MaxShield;
    public GameObject Fireball;
    public List<ShieldName> Shields = new List<ShieldName>();

    [Header("ForcePush")]
    public float PushAmount;
    public float PushRadius;
    public float DistanceCheck;

    public Vector3 Offset;
    public float RotCheck;

    public Transform empty;


    [Header("Other")]
    public List<MagicInfo> Spells = new List<MagicInfo>();

    //public List<FollowInfo> Follows = new List<FollowInfo>();
    public void BothSpellManager()
    {
        for (int i = 0; i < Spells.Count; i++)
        {
            SpellType type = Spells[i].Type;
            int TypeNum = (int)type;
            if (TypeNum == 1)
            {
                //if hand motion 0 and 1 complete
                //if trigger on both
                //if both are now not a trigger
                //HM.Behaviour(i, 0, (int)side);
                //HM.Spells[i].Controllers[(int)side].Current = 0;


                if (Spells[i].Controllers[0].ControllerFinished[0] == true && Spells[i].Controllers[1].ControllerFinished[0] == true)
                {
                    Spells[i].Finished[0] = true;
                }
                if (Spells[i].Finished[0] == true && Controllers[0].TriggerPressed() == true && Controllers[1].TriggerPressed())
                {
                    Spells[i].Finished[1] = true;
                }
                if (Spells[i].Finished[1] == true && Controllers[0].TriggerPressed() == false && Controllers[1].TriggerPressed() == false)
                {
                    Spells[i].Finished[0] = false;
                    Spells[i].Finished[1] = false;

                    Spells[i].Controllers[0].Current = 0;
                    Spells[i].Controllers[1].Current = 0;
                }
            }
        }
    }
    public void Behaviour(int Spell, int Part, int Side)
    {
        if (Spell == 0)
        {
            if (Part == 0)
            {
                //motion
            }
            else if (Part == 1)
            {
                //pressed
            }
            else if (Part == 2)
            {
                ChangeMagic(-Spells[Spell].Cost);
                UseSpike(RaycastGround());
            }
        }
        if (Spell == 1)
        {
            if (Part == 0)
            {
                //motion
                //fireball
            }
            else if (Part == 1)
            {
                FireballCharge(Side);
            }
            else if (Part == 2)
            {
                ChangeMagic(-Spells[Spell].Cost);
                FireballShoot(Side);
            }
        }
        if (Spell == 2)
        {
            if (Part == 0)
            {
                //motion
                //shield
            }
            else if (Part == 1)
            {
                //pressedCost
                ChangeMagic(-Spells[Spell].Cost);
                StartShield(1);
            }
            else if (Part == 2)
            {
                EndShield(1);
            }
        }
        if (Spell == 3)
        {
            if (Part == 0)
            {
                //motion
                //forceblast
            }
            else if (Part == 1)
            {
                //pressed
            }
            else if (Part == 2)
            {
                ChangeMagic(-Spells[Spell].Cost);
                UseForcePush();
            }
        }
    }
    //inumerator should be the one handactions sends to saying it should start sequence
    public void FireballCharge(int Hand)
    {
        
    }

    public void FireballShoot(int Hand)
    {
        GameObject Current = Instantiate(Fireball, Controllers[Hand].transform.position, Quaternion.LookRotation(Controllers[Hand].transform.forward));
       // Controllers[Hand].transform
    }
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
        ChangeMagic(-Spells[2].Cost);
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
    public void FollowMotion()
    {
        //OR

        // value
       
        for (int i = 0; i < Spells.Count; i++)
        {
            for (int j = 0; j < Spells[i].Sides.Count; j++)
            {
                
                int Current = Spells[i].Controllers[j].Current;
                SpellType Type = Spells[i].Type;
                int TypeNum = (int)Type;
                //Debug.Log("1");
                if (TypeNum == 0)
                {
                    //indivigual
                }
                else if (TypeNum == 1)
                {
                    //both
                }
                //Debug.Log("1.1");
                if (Current > HandDebug.instance.DataFolders[i].FinalInfo.RightLocalPos.Count - 1)
                {
                    Current -= 1;
                }
                //Debug.Log("1.2");
                Vector3 Local = Vector3.zero;
                Vector3 Final = Vector3.zero;
                if (i == 0)
                {
                    if (j == 0)
                    {
                        Local = HandDebug.instance.DataFolders[i].FinalInfo.RightLocalPos[Current];
                    }
                    else
                    {
                        Local = HandDebug.instance.DataFolders[i].FinalInfo.LeftLocalPos[Current];
                    }
                }
                
                float Distance = Local.x;
                float RotationOffset = Local.y;
                float HorizonalOffset = Local.z;
                //Debug.Log("2");

                //Vector3 Direction = new Vector3(0, Cam.rotation.eulerAngles.y - RotationOffset + 180, 0);
                //RotCheck = Direction.y;
                //Debug.Log("Direction" + Direction + " " );
                //Vector3 YPosition = Cam.position + Direction * Distance;
                empty.rotation = Quaternion.Euler(0, Cam.rotation.eulerAngles.y + RotationOffset, 0);

                //Debug.Log("3");
                // = ModifiedForward;
                Ray r = new Ray(Cam.position, empty.forward);
                Vector3 YPosition = r.GetPoint(Distance);
                Offset = r.GetPoint(Distance);

                Final = new Vector3(YPosition.x, HorizonalOffset, YPosition.z);
                //Vector3 Spot = Cam.transform.position + Final;
                
                if (i == 0)
                {
                    Spells[i].Sides[j].transform.position = Final;
                }
                //Debug.Log("4");
            }
        }
    }

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
        }
        else
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            return Vector3.zero;
        }
    }

    public void UseSpike(Vector3 Position)
    {
        GameObject spike = Instantiate(Spike, Position, Quaternion.identity);
        spike.GetComponent<ParticleSystem>().Play();
        Destroy(spike, SpikeTimeDelete);

        //eventually check for people and do damage
    }

    public void UseForcePush()
    {
        //float ZDirection;
        Vector3 pos = Cam.transform.position;
        Vector3 dir;
        if (Sounds == true)
        {
            Force.Play();
        }
     
        //do some effect animation
        //play force sound

        Collider[] colliders = Physics.OverlapSphere(pos, PushRadius);
        foreach (Collider pushedOBJ in colliders)
        {
            if (pushedOBJ.tag != "Player" && pushedOBJ.gameObject.GetComponent<Rigidbody>() != null)
            {
                Vector3 directionToTarget = pos - pushedOBJ.transform.position;
                //float angle = Vector3.Angle(dir, directionToTarget);
                //float distance = directionToTarget.magnitude;
                //Debug.Log(angle + " " + pushedOBJ);

                Rigidbody pushed = pushedOBJ.GetComponent<Rigidbody>();
                pushed.AddExplosionForce(PushAmount, pos, PushRadius);
                /*
                if (Mathf.Abs(angle) < 90 && distance < 10)
                {
                    //Debug.Log("target is in front of me");
                    //Rigidbody pushed = pushedOBJ.GetComponent<Rigidbody>();
                    //pushed.AddExplosionForce(PushAmount, pos, PushRadius);
                }
                */
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
        BothSpellManager();
        FollowMotion();
        //Debug.DrawRay(Cam.position, Cam.forward * 10000f, Color.red);
        if (ShouldCharge == true)
        {
            Charge();
        }
        bool UsingMagic = false;
        
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

    public void CheckFlying()
    {

        //check for flying
        for (int i = 0; i < 2; i++)
        {
            bool Flying = CheckFlying(Controllers[i]);
            // trigger is held and has enough magic
            if (Flying == true)
            {
                //UsingMagic = true;
                if (Controllers[i].Playing == false)
                {
                    Controllers[i].Playing = true;
                    //Controllers[i].PS.Play();
                }
            }
            else
            {
                if (Controllers[i].Playing == true)
                {
                    Controllers[i].Playing = false;
                    // Controllers[i].PS.Stop();
                }
            }
        }

    }
}
