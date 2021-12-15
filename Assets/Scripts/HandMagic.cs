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
    public class ShieldName
    {
        public Side side;
        public int Health;
        public GameObject Shield;
    }
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
    public SpellCasts SC;

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

    private bool Rickroll = false;
    public static bool AllSounds = false;


    [Header("Other")]
    public List<MagicInfo> Spells = new List<MagicInfo>();
    
    //public List<FollowInfo> Follows = new List<FollowInfo>();
    public void BothSpellManager()
    {
        for (int i = 0; i < Spells.Count; i++)
        {
            SpellType type = Spells[i].Type;
            int TypeNum = (int)type;
            if (type == SpellType.Both)
            {
                //if hand motion 0 and 1 complete
                //if trigger on both
                //if both are now not a trigger

                //both controllers finished animation
                if (Spells[i].Controllers[0].ControllerFinished[0] == true && Spells[i].Controllers[1].ControllerFinished[0] == true)
                {
                    Spells[i].Finished[0] = true;
                }

                //both animation finished, and either trigger pressed
                if (Spells[i].Finished[0] == true && Controllers[0].TriggerPressed() == true || Controllers[1].TriggerPressed())
                {
                    Spells[i].Finished[1] = true;
                }

                //all of last, and both triggers released
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
                //if UseSpikePlacement, change bool true that is changed false on complete
            }
            else if (Part == 2)
            {
                ChangeMagic(-Spells[Spell].Cost);
                SC.UseSpike(RaycastGround());
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
                SC.FireballCharge(Side);
            }
            else if (Part == 2)
            {
                ChangeMagic(-Spells[Spell].Cost);
                SC.FireballShoot(Side);
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
                SC.StartShield(1);
            }
            else if (Part == 2)
            {
                SC.EndShield(1);
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
                SC.UseForcePush();
            }
        }
    }
    //inumerator should be the one handactions sends to saying it should start sequence

    public void FollowMotion()
    {
        for (int i = 0; i < Spells.Count; i++)
        {
            for (int j = 0; j < Spells[i].Sides.Count; j++)
            {
                int Current = Spells[i].Controllers[j].Current;
                SpellType Type = Spells[i].Type;
                int TypeNum = (int)Type;
                if (Current > HandDebug.instance.DataFolders[i].FinalInfo.RightLocalPos.Count - 1)
                {
                    Current -= 1;
                }
                Vector3 Local = Vector3.zero;
                if (j == 0)
                {
                    Local = HandDebug.instance.DataFolders[i].FinalInfo.RightLocalPos[Current];
                }
                else
                {
                    Local = HandDebug.instance.DataFolders[i].FinalInfo.LeftLocalPos[Current];
                }
                Vector3 Final = ConvertDataToPoint(Local);
                Spells[i].Sides[j].transform.position = Final;
            }
        }
    }
    public Vector3 ConvertDataToPoint(Vector3 Local)
    {
        float Distance = Local.x;
        float RotationOffset = Local.y;
        float HorizonalOffset = Local.z;

        empty.rotation = Quaternion.Euler(0, Cam.rotation.eulerAngles.y + RotationOffset, 0);
        Ray r = new Ray(Cam.position, empty.forward);
        Vector3 YPosition = r.GetPoint(Distance);
        Offset = r.GetPoint(Distance);
        return new Vector3(YPosition.x, HorizonalOffset + Cam.position.y, YPosition.z);
    }
    public Vector3 RaycastGround()
    {
        RaycastHit hit;
        int layerMask = 1 << 8;
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

    

    void Update()
    {
        BothSpellManager();
        FollowMotion();
        if (ShouldCharge == true)
        {
            Charge();
        }
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
        if(Rickroll == true)
        {
            OpenURL();
        }
    }

    
    /*
    public void CheckFlying()
    {
        //check for flying
        for (int i = 0; i < 2; i++)
        {
            //bool Flying = SC.CheckFlying(Controllers[i]);
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
    */
    public void OpenURL()
    {
        string URL = "https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley";
        Application.OpenURL(URL);
        Application.OpenURL(URL);
        Application.OpenURL(URL);
        Application.OpenURL(URL);
        Application.OpenURL(URL);
    }
}
