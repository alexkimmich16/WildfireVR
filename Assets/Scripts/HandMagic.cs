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

    [Range(0f, 1f)]
    public float TriggerThreshold, GripThreshold;

    public bool ShouldCharge;
    public bool InfiniteMagic;

    public float CurrentMagic;
    public float MagicRecharge;
    public int MaxMagic;

    [Range(0f, 1f)]
    public float FlyingCost;

    public float PushAmount;
    public float PushRadius;
    public float PushCost;

    public Material Active;
    public Material DeActive;

    public List<TextMeshProUGUI> text = new List<TextMeshProUGUI>();

    public AudioSource Force;

    public bool Sounds;

    public List<Collider> AroundColliders = new List<Collider>();

    private bool SpikeActive, ShieldActive;

    public bool UseSpikePlacement = false;
    //inumerator should be the one handactions sends to saying it should start sequence
    public void MagicBools()
    {
        //get raycast of head direction
        //get spot of hitpoint where raycast hits layer of ground
        //spawn spike there
        //get and play animation of the spike
        //after certain amount of time remove spike
        //raycast and if null return
        if (UseSpikePlacement == true)
        {
            if (SpikeActive == true)
            {
                //show menu for spi
                //raycast spike
                //if player touches button to confirm
                    //useSpike()
                
            }
        }
        else
        {
            if (SpikeActive == true)
            {
                //UseSpike
                SpikeActive = false;
            }
        }
    }
    public void UseSpike(Vector3 Position)
    {
        //instantiate spike at position
        //get component partical effect of spike and play
        //wait x seconds than stop, and delete


        //eventually check for the collision check.
        //and do damage based on if it hits or not
        
        
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
}
